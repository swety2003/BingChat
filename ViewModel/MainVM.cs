using BingBot.Core.Common;
using BingChat.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyWidgets.SDK.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BingChat.ViewModel
{
    public partial class MainVM : ObservableObject
    {
        public Config Config { get; set; }

        public void LoadCfg(string p)
        {
            Config= ConfigBase.Load<Config>(p)??new Config(p);
        }
        
        public MainVM(Main main)
        {
            MainView = main;
        }

        [RelayCommand]
        void ShowSetting()
        {
            MainView.ShowSetting();
        }

        public AsyncRelayCommand InitCommand => new AsyncRelayCommand(async () =>
        { 

            Conversation = await BingChatConversation
                .Create(Encoding.ASCII.GetString(Encoding.Default.GetBytes(Config.cookie)));

        });

        [ObservableProperty]
        ObservableCollection<MessageItem> conversastions = new ObservableCollection<MessageItem>();

        BingChatConversation Conversation;

        [ObservableProperty]
        string userInput;

        public AsyncRelayCommand ChatCommand => new AsyncRelayCommand(async () =>
        {
            if (String.IsNullOrEmpty(UserInput))
            {
                return;
            }
            Conversastions.Add(new MessageItem(UserInput, MsgType.User));

            var ret = await Conversation.AskAsync(UserInput);

            Conversastions.Add(new MessageItem(ret,MsgType.Bot));

            UserInput = "";

        });

        public Main MainView { get; }
    }

    public enum MsgType
    {
        User,
        Bot,
        System,
    }

    public class MessageItem
    {
        public string Message { get; set; }

        public MessageItem(string message, MsgType type = MsgType.User)
        {
            Message = String.Join("\r\n", Regex.Split(message, "\r?\n").Select(ln => ln.TrimStart()));
            Type = type;
        }

        public MsgType Type { get; set; }

    }
}
