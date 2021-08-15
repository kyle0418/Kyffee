using MyCoffeeApp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyCoffeeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserInfoPage : ContentPage
    {
        INotificationManager notificationManager;
        int notificationNumber = 0;

        public UserInfoPage()
        {
            InitializeComponent();

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationSwitch.IsToggled = Preferences.Get("notificationON", false);
            if (notificationSwitch.IsToggled)
            {
                notificationSwitch.ThumbColor = Color.LimeGreen;
            }
            else
            {
                notificationSwitch.ThumbColor = Color.Gray;
            }
            notificationSwitch.Toggled += Switch_Toggled;
        }

        void OnSendClick(object sender, EventArgs e)
        {
            notificationNumber++;
            string username = Preferences.Get("username", "default_name");
            string title = $"Welcome {username}!";
            string message = $"Welcome to kyffee and have a good time.";
            notificationManager.SendNotification(title, message);
        }

        void OnScheduleClick(object sender, EventArgs e)
        {
            notificationNumber++;
            string username = Preferences.Get("username", "default_name");
            string title = $"Welcome {username}!";
            string message = $"Welcome to kyffee and have a good time.";
            notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(10));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            bool hasKey = Preferences.ContainsKey("username");
            if (hasKey)
            {
                LabelUsername.Text = Preferences.Get("username", "default_name");
                LabelGender.Text = Preferences.Get("gender", "default_gender");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Shell.Current.GoToAsync($"//{nameof(CoffeeEquipmentPage)}");
            return true;
        }

        private async void ButtonLogout_Clicked(object sender, EventArgs e)
        {
            bool hasKey = Preferences.ContainsKey("haveLoggedin");
            if (hasKey)
            {
                Preferences.Set("haveLoggedin", false);
                Preferences.Set("username", "default_name");
                Preferences.Set("gender", "default_gender");
                Preferences.Set("avatar", "");
            }
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");

            CoffeeService.db = null;
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("notificationON", (sender as Switch).IsToggled);
            if ((sender as Switch).IsToggled)
            {
                (sender as Switch).ThumbColor = Color.LimeGreen;
                Application.Current.MainPage.DisplayAlert("",
                    "To display on the top of the status bar, you also need to set it in Settings -> Notifications -> Kyffee -> Banners.", "OK");
            }
            else
            {
                (sender as Switch).ThumbColor = Color.Gray;
            }
        }
    }

    public class NotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}