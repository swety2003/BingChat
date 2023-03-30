using System;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Websocket.Client;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using MyWidgets.SDK;


namespace BingBot.Core.Common;

/// <summary>
/// A chat conversation, enables us to chat multiple times in the same context.
/// </summary>
public partial class BingChatConversation
{

    ILogger<BingChatConversation> logger = Logger.CreateLogger<BingChatConversation>();

    private const char TerminalChar = '\u001e';
    private readonly BingChatRequest _request;

    public BingChatConversation(string clientId, string conversationId, string conversationSignature)
    {
        _request = new BingChatRequest(clientId, conversationId, conversationSignature);
    }

    public Task<string> AskAsync(string message)
    {
        var wsClient = new WebsocketClient(new Uri("wss://sydney.bing.com/sydney/ChatHub"));
        var tcs = new TaskCompletionSource<string>();

        void Cleanup()
        {
            wsClient.Stop(WebSocketCloseStatus.Empty, string.Empty).ContinueWith(t =>
            {
                if (t.IsFaulted) tcs.SetException(t.Exception!);
                wsClient.Dispose();
                wsClient = null;
            });
        }

        string? GetAnswer(ConversationResponseModel response)
        {
            var responseItem = response.Item;
            
            if(responseItem.Result.Value != "Success") return responseItem.Result.Message;

            for (var index = responseItem.Messages.Length - 1; index >= 0; index--)
            {
                var itemMessage = responseItem.Messages[index];

                if (itemMessage.ContentOrigin == "TurnLimiter")
                {
                    return itemMessage.Text;
                }
                else
                {
                    if (itemMessage.MessageType != null) continue;
                    if (itemMessage.Author != "bot") continue;

                    // maybe is possible to use itemMessage.Text directly, but some extra information will be lost
                    return itemMessage.AdaptiveCards?[0].Body?[0].Text ?? itemMessage.Text;
                }
            }

            return null;
        }

        void MessageReceived(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            try
            {
                foreach (var part in text.Split(TerminalChar, StringSplitOptions.RemoveEmptyEntries))
                {
                    var json = JsonConvert.DeserializeObject<ConversationResponseModel>(part);

                    if (json is not { Type: 2 }) continue;

                    Cleanup();

                    //Console.WriteLine(part);

                    tcs.SetResult(GetAnswer(json) ?? "<empty answer>");
                    return;
                }
            }
            catch (Exception e)
            {
                Cleanup();
                logger.LogError(e.Message, e);

                tcs.SetException(e);
            }
        }

        wsClient.MessageReceived.Where(msg => msg.MessageType == WebSocketMessageType.Text).Select(msg => msg.Text).Subscribe(MessageReceived);

        // Start the WebSocket client
        wsClient.Start().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Cleanup();
                tcs.SetException(t.Exception!);
                return;
            }

            // Send initial messages
            wsClient.Send("{\"protocol\":\"json\",\"version\":1}" + TerminalChar);
            wsClient.Send(_request.ConstructInitialPayload(message) + TerminalChar);
        });

        return tcs.Task;
    }
}

public partial class BingChatConversation
{
    const string create_api = "https://www.bing.com/turing/conversation/create";

    public static async Task<BingChatConversation> Create(string Cookie)
    {
        var handler = new HttpClientHandler() { UseCookies = false };
        var client = new HttpClient(handler);// { BaseAddress = baseAddress };
        var message = new HttpRequestMessage(HttpMethod.Get, create_api);
        message.Headers.Add("Cookie", Cookie);
        try
        {
            var r = await client.SendAsync(message);

            r.EnsureSuccessStatusCode();
            var rsp = await r.Content.ReadAsStringAsync();

            var rsp_d = JsonConvert.DeserializeObject<CreateChatRspModel>(rsp);

            return new BingChatConversation(rsp_d.clientId, rsp_d.conversationId, rsp_d.conversationSignature);

        }
        catch (Exception ex)
        {

            throw;
        }

    }


}

public class CreateChatRspModel {
    
    public class Result
    {
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public string conversationId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string clientId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string conversationSignature { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Result result { get; set; }
    


}
