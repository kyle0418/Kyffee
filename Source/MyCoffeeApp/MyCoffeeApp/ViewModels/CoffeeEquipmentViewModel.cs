using MvvmHelpers;
using MvvmHelpers.Commands;
using MyCoffeeApp.Models;
using MyCoffeeApp.Services;
using MyCoffeeApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Mcmd = MvvmHelpers.Commands;

namespace MyCoffeeApp.ViewModels
{
    public class CoffeeEquipmentViewModel : ViewModelBase
    {
        public ObservableRangeCollection<Coffee> Coffee { get; set; }
        public ObservableRangeCollection<Grouping<string, Coffee>> CoffeeGroups { get; set; }

        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand AddCommand { get; }
        public AsyncCommand<object> SelectedCommand { get; }
        public AsyncCommand<Coffee> RemoveCommand { get; }

        public Mcmd.Command LoadMoreCommand { get; }
        public Mcmd.Command DelayLoadMoreCommand { get; }
        public Mcmd.Command ClearCommand { get; }


        public CoffeeEquipmentViewModel()
        {
            Title = "Coffe Equipment";
            Coffee = new ObservableRangeCollection<Coffee>();
            CoffeeGroups = new ObservableRangeCollection<Grouping<string, Coffee>>();

            //LoadMore();
            Task.Run(Refresh);


            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand(Add);
            SelectedCommand = new AsyncCommand<object>(Selected);
            RemoveCommand = new AsyncCommand<Coffee>(Remove);

            LoadMoreCommand = new Mcmd.Command(LoadMore);
            ClearCommand = new Mcmd.Command(Clear);
            DelayLoadMoreCommand = new Mcmd.Command(DelayLoadMore);
        }

        public async Task Refresh()
        {
            IsBusy = true;
            await Task.Delay(500);
            Coffee.Clear();
            var coffees = await CoffeeService.GetAllCoffee();
            Coffee.AddRange(coffees);
            IsBusy = false;
        }

        async Task Add()
        {
            //var name = await App.Current.MainPage.DisplayPromptAsync("Name", "Name");
            //var roaster = await App.Current.MainPage.DisplayPromptAsync("Roaster", "Roaster");
            //if (name == "" || roaster == "" || name == null || roaster == null)
            //    return;
            //await CoffeeService.AddCoffee(name, roaster);
            //await Refresh();

            var route = $"{nameof(AddMyCoffeePage)}";
            await Shell.Current.GoToAsync(route);
        }

        async Task Remove(Coffee coffee)
        {
            await CoffeeService.RemoveCoffee(coffee.Id);
            await Refresh();
        }

        Coffee selectedCoffee;
        public Coffee SelectedCoffee
        {
            get => selectedCoffee;
            set => SetProperty(ref selectedCoffee, value);
        }

        async Task Selected(object args)
        {
            var coffee = args as Coffee;
            if (coffee == null)
                return;

            SelectedCoffee = null;
            var route = $"{nameof(MyCoffeeDetailsPage)}?CoffeeId={coffee.Id}";
            await Shell.Current.GoToAsync(route);
        }

        void LoadMore()
        {
            if (Coffee.Count >= 20)
                return;

            var image = "https://www.yesplz.coffee/app/uploads/2020/11/emptybag-min.png";
            Coffee.Add(new Coffee { Roaster = "Yes Plz", Name = "Sip of Sunshine", Image = image });
            Coffee.Add(new Coffee { Roaster = "Yes Plz", Name = "Potent Potable", Image = image });
            Coffee.Add(new Coffee { Roaster = "Yes Plz", Name = "Potent Potable", Image = image });
            Coffee.Add(new Coffee { Roaster = "Blue Bottle", Name = "Kenya Kiambu Handege", Image = image });
            Coffee.Add(new Coffee { Roaster = "Blue Bottle", Name = "Kenya Kiambu Handege", Image = image });

            CoffeeGroups.Clear();

            CoffeeGroups.Add(new Grouping<string, Coffee>("Blue Bottle", Coffee.Where(c => c.Roaster == "Blue Bottle")));
            CoffeeGroups.Add(new Grouping<string, Coffee>("Yes Plz", Coffee.Where(c => c.Roaster == "Yes Plz")));
        }

        void DelayLoadMore()
        {
            Console.WriteLine("delay");
            if (Coffee.Count <= 10)
                return;

            LoadMore();
        }

        void Clear()
        {
            Coffee.Clear();
            CoffeeGroups.Clear();
        }
    }
}
