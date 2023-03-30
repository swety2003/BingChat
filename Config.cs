using MyWidgets.SDK.Common;

namespace BingChat
{
    public class Config:ConfigBase
    {
        public Config(string file_path) : base(file_path)
        {
        }

        public string cookie { get; set; } = "";
    }
}