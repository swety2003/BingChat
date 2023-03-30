
using BingChat;
using BingChat.View;
using BingChat.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using MyWidgets.SDK;
using System;
using System.ComponentModel;
using System.IO;

namespace BingChat.ViewModels;
//Todo: 加上配置页面
public partial class SettingVM : ObservableObject
{

    public SettingVM(MainVM mvm)
    {
        Appcfg= mvm.Config;
        if (Appcfg.cookie != null)
        {

            Cookie = Appcfg.cookie;
        }

        this.mvm = mvm;
    }

    [ObservableProperty]
    string _cookie;
    private readonly MainVM mvm;



    public Config Appcfg { get; }

    [RelayCommand]
    void Apply()
    {
        if (Cookie != null)
        {
            Appcfg.cookie = Cookie;

            Appcfg.Save();

            mvm.InitCommand.Execute(null);
        }
    }


}




public class ConfigBase
{
    public static T? Load<T>(string path) where T : class
    {
        try
        {

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static void Save<T>(string path, T obj)
    {
        File.WriteAllText(JsonConvert.SerializeObject(obj), path);
    }

    public void Save(string path)
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(this));
    }
}
