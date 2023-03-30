
using BingChat.ViewModels;
using System.Windows;

namespace BingChat
{
    /// <summary>
    /// SettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SettingDialog : Window
    {
        public SettingDialog(SettingVM vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            this.Close();

        }
    }
}
