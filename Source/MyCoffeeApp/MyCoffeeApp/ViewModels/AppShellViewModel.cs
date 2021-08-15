using System;
using System.Collections.Generic;
using System.Text;

namespace MyCoffeeApp.ViewModels
{
    public class AppShellViewModel : ViewModelBase
    {
        string username = "";
        public string Username
        {
            get => username;
            set
            {
                SetProperty(ref username, value);
                OnPropertyChanged();
            }
        }

        string avatarImage = "";
        public string AvatarImage
        {
            get => avatarImage;
            set
            {
                SetProperty(ref avatarImage, value);
                OnPropertyChanged();
            }
        }
    }
}
