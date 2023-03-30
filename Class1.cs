using MyWidgets.SDK;
using MyWidgets.SDK.Core.SideBar;
using System;
using System.Collections.Generic;

namespace BingChat
{
    public class PluginInfo : IPlugin
    {
        public Version version { get; } = new Version();
        public string url { get; } = "";
        public string author { get; } = "";


        public PluginInfo()
        {
            MdXaml.MarkdownScrollViewer viewer =new();
        }

        public static List<SideBarItemInfo> sbis { get; } = new List<SideBarItemInfo>()
        {
            //DevTest.info
            //ProjManager.info
            View.Main.info
        };



        public string name => "BingChat-GUI";


        public IEnumerable<object> GetAllTypeInfo()
        {
            return sbis;
        }
    }
}
