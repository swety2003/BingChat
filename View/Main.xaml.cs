using BingChat.ViewModel;
using MyWidgets.SDK;
using MyWidgets.SDK.Core.SideBar;
using MyWidgets.SDK.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BingChat.View
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : UserControl, ISideBarItem
    {
        public static SideBarItemInfo info = new SideBarItemInfo("BingChat","",typeof(Main));

        Pop pop_view = new Pop();
        MainVM vm;

        public Main()
        {
            InitializeComponent();
            vm = new MainVM(this);
        }


        public SideBarItemInfo Info => info;

        public Guid GUID => throw new NotImplementedException();

        public void OnDisabled()
        {

        }

        public void OnEnabled()
        {
            vm.LoadCfg(this.GetPluginConfigFilePath());
            vm.InitCommand.Execute(null);

        }

        public void ShowSetting()
        {
            new SettingDialog(new ViewModels.SettingVM(vm)).ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            pop_view.DataContext = vm;
            this.Show(pop_view);
        }
    }
}
