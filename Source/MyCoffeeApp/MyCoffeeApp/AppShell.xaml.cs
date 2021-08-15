using System;
using System.Collections.Generic;
using MyCoffeeApp.ViewModels;
using MyCoffeeApp.Views;
using Xamarin.Forms;

namespace MyCoffeeApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShellViewModel appShellViewModel;
        public AppShell() 
        {
            InitializeComponent();

            this.BindingContext = new AppShellViewModel();
            appShellViewModel = this.BindingContext as AppShellViewModel;

            Routing.RegisterRoute(nameof(AddMyCoffeePage), typeof(AddMyCoffeePage));
            Routing.RegisterRoute(nameof(MyCoffeeDetailsPage), typeof(MyCoffeeDetailsPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        }
    }
}
