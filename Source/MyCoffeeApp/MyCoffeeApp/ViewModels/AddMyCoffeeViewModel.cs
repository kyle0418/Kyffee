using MvvmHelpers.Commands;
using MyCoffeeApp.Models;
using MyCoffeeApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyCoffeeApp.ViewModels
{
    class AddMyCoffeeViewModel : ViewModelBase
    {
        List<string> types = new List<string> { "Personal", "Work", "Life", "Other" };
        public List<string> Types { get => types; set => SetProperty(ref types, value); }
        public int SelectedTypeIndex { get; set; } = -1;

        string name, roaster;
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Roaster { get => roaster; set => SetProperty(ref roaster, value); }
        public AsyncCommand SaveCommand { get; }

        public AddMyCoffeeViewModel()
        {
            Title = "Add Coffee";
            SaveCommand = new AsyncCommand(Save);
        }

        async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Roaster) ||
                SelectedTypeIndex == -1)
            {
                await Application.Current.MainPage.DisplayAlert("", "Title/Content/Type can't be empty!", "OK");
                return;
            }

            IEnumerable<Coffee> coffees = await CoffeeService.GetAllCoffee();
            List<string> namelist = coffees.ToList().Select(x => x.Name).ToList();
            if (namelist.Contains(Name))
            {
                await Application.Current.MainPage.DisplayAlert("", "Duplicate Title!", "OK");
                return;
            }


            await CoffeeService.AddCoffee(Name, Roaster, types[SelectedTypeIndex]);
            await Shell.Current.GoToAsync($"..?ToAdd=true&TitleFromAdd={Name}");
        }
    }
}
