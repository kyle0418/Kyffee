using MyCoffeeApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyCoffeeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        INotificationManager notificationManager;

        public LoginPage()
        {
            InitializeComponent();

            notificationManager = DependencyService.Get<INotificationManager>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            bool hasKey = Preferences.ContainsKey("haveLoggedin");
            if (hasKey)
            {
                var loggedin = Preferences.Get("haveLoggedin", false);
                if (loggedin)
                {
                    await Shell.Current.GoToAsync($"//{nameof(CoffeeEquipmentPage)}");
                    var appShellInstance = Xamarin.Forms.Shell.Current as AppShell;
                    appShellInstance.appShellViewModel.Username = Preferences.Get("username", "");
                    appShellInstance.appShellViewModel.AvatarImage = Preferences.Get("avatar", "");
                    return;
                }
            }

            username.Text = string.Empty;
            password.Text = string.Empty;

        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        Dictionary<string, List<string>> userDictionary = new Dictionary<string, List<string>>();

        //login
        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(username.Text) ||
                string.IsNullOrWhiteSpace(password.Text))
            {
                await Shell.Current.DisplayAlert("", "Username/Password can't be empty!", "OK");
                return;
            }

            // check network
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                var webRequest = WebRequest.Create(@"https://gitee.com/kyle0418/user-info/raw/master/userlist");
                webRequest.Timeout = 1500;
                try
                {
                    using (var response = webRequest.GetResponse())
                    using (var content = response.GetResponseStream())
                    using (var reader = new StreamReader(content))
                    {
                        var strContent = reader.ReadToEnd();
                        List<string> userinfo = strContent.Split(';').ToList();
                        userinfo.RemoveAt(userinfo.Count - 1);
                        foreach (string info in userinfo)
                        {
                            string username = info.Split(',')[0].Split(':')[1];
                            string password = info.Split(',')[1].Split(':')[1];
                            string gender = info.Split(',')[2].Split(':')[1];
                            string avatar = info.Split(',')[3].Split(':')[1];
                            List<string> details = new List<string>();
                            details.Add(password);
                            details.Add(gender);
                            details.Add(avatar);
                            if (!userDictionary.ContainsKey(username))
                                userDictionary.Add(username, details);
                        }
                    }
                }
                catch
                {
                    var messageOptions = new MessageOptions
                    {
                        Foreground = Color.Red,
                        Font = Font.SystemFontOfSize(16),
                        Message = "Internet connection timed out!"
                    };
                    var options = new ToastOptions
                    {
                        MessageOptions = messageOptions,
                        Duration = TimeSpan.FromMilliseconds(1500),
                        BackgroundColor = Color.Silver,
                        IsRtl = false
                    };
                    await this.DisplayToastAsync(options);
                    return;
                }
            }
            else
            {
                var messageOptions = new MessageOptions
                {
                    Foreground = Color.Red,
                    Font = Font.SystemFontOfSize(16),
                    Message = "Internet connection might be down!"
                };
                var options = new ToastOptions
                {
                    MessageOptions = messageOptions,
                    Duration = TimeSpan.FromMilliseconds(1500),
                    BackgroundColor = Color.Silver,
                    IsRtl = false
                };
                await this.DisplayToastAsync(options);
                return;
            }

            string pwd = password.Text;
            if (userDictionary.ContainsKey(username.Text ?? "") && userDictionary[username.Text][0] == pwd)
            {
                Preferences.Set("haveLoggedin", true);
                Preferences.Set("username", username.Text);
                Preferences.Set("gender", userDictionary[username.Text][1]);
                string title = $"Hi, {username.Text}!";
                string message = $"Welcome to Kyffee and enjoy it.";
                if (Preferences.Get("notificationON", false))
                {
                    notificationManager.SendNotification(title, message);
                }
                await Shell.Current.GoToAsync($"//{nameof(CoffeeEquipmentPage)}");
                var appShellInstance = Xamarin.Forms.Shell.Current as AppShell;
                appShellInstance.appShellViewModel.Username = username.Text;
                if (!string.IsNullOrWhiteSpace(userDictionary[username.Text][2]))
                {
                    string url = "https://" + userDictionary[username.Text][2];
                    appShellInstance.appShellViewModel.AvatarImage = url;
                    Preferences.Set("avatar", url);
                }
                else
                {
                    Preferences.Set("avatar", "");
                    appShellInstance.appShellViewModel.AvatarImage = Preferences.Get("avatar", "");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("", "Wrong account or password!", "OK");
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(RegistrationPage)}");
        }

        private void username_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (username.Text == string.Empty)
            {
                password.Text = string.Empty;
            }
        }
    }
}