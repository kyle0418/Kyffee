using MvvmHelpers;
using MyCoffeeApp.Models;
using MyCoffeeApp.Services;
using MyCoffeeApp.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyCoffeeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ToAdd), nameof(ToAdd))]
    [QueryProperty(nameof(TitleFromAdd), nameof(TitleFromAdd))]
    public partial class CoffeeEquipmentPage : ContentPage
    {
        public CoffeeEquipmentPage()
        {
            InitializeComponent();
        }

        public string ToAdd { get; set; } = "false";
        public string TitleFromAdd { get; set; } = "";

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // create db file for each user
            if (CoffeeService.db == null)
            {
                (BindingContext as CoffeeEquipmentViewModel).Coffee.Clear();
                string username = Preferences.Get("username", "default_name");
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, $"data_{username}.db");
                CoffeeService.db = new SQLiteAsyncConnection(databasePath);
                await CoffeeService.db.CreateTableAsync<Coffee>();
                var coffees = await CoffeeService.GetAllCoffee();
                (BindingContext as CoffeeEquipmentViewModel).Coffee.AddRange(coffees);
            }

            if (ToAdd == "true")
            {
                await (BindingContext as CoffeeEquipmentViewModel).Refresh();

                ObservableRangeCollection<Coffee> coffees = (this.BindingContext as CoffeeEquipmentViewModel).Coffee;
                foreach(Coffee coffee in coffees)
                {
                    if (coffee.Name == TitleFromAdd)
                    {
                        mainListView.ScrollTo(coffee, ScrollToPosition.Center, true);
                        return;
                    }
                }

                ToAdd = "false";
            }

            if(ToAdd == "truefromedit")
            {
                await (BindingContext as CoffeeEquipmentViewModel).Refresh();
                ToAdd = "false";
            }
        }

        private async void Search_Clicked(object sender, EventArgs e)
        {
            string name = await Application.Current.MainPage.DisplayPromptAsync("", "Input item title to search:", "search", "cancel");
            if (String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            foreach (Coffee coffee in (this.BindingContext as CoffeeEquipmentViewModel).Coffee)
            {
                if (coffee.Name == name)
                {
                    mainListView.ScrollTo(coffee, ScrollToPosition.Center, true);
                    return;
                }
            }

            await Shell.Current.DisplayAlert("", $"No item matches title \"{name}\"", "OK");
        }
    }
}