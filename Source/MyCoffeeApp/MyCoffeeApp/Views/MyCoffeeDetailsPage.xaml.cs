using MyCoffeeApp.Models;
using MyCoffeeApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyCoffeeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(CoffeeId), nameof(CoffeeId))]
    public partial class MyCoffeeDetailsPage : ContentPage
    {
        public string CoffeeId { get; set; }
        public MyCoffeeDetailsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(CoffeeId, out var result);

            BindingContext = await CoffeeService.GetCoffee(result);
            originalTitle = titleEntry.Text;
        }

        protected override bool OnBackButtonPressed()
        {
            Shell.Current.GoToAsync("..?ToAdd=false", animate: true);
            return base.OnBackButtonPressed();
        }

        int editCounter = 0;
        bool isEdited = false;
        string originalTitle;
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (isEdited)
            {
                IEnumerable<Coffee> coffees = await CoffeeService.GetAllCoffee();
                List<string> namelist = coffees.ToList().Select(x => x.Name).ToList();
                if (namelist.Contains(titleEntry.Text.Trim()) && titleEntry.Text.Trim() != originalTitle)
                {
                    await Application.Current.MainPage.DisplayAlert("", "Duplicate Title!", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(titleEntry.Text))
                {
                    await Application.Current.MainPage.DisplayAlert("", "Title can't be empty!", "OK");
                    return;
                }

                await CoffeeService.UpdateCoffee(Convert.ToInt32(CoffeeId), titleEntry.Text.Trim(), contentEditor.Text);
                await Shell.Current.GoToAsync("..?ToAdd=truefromedit");
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            editCounter++;
            if (editCounter <= 2) // load entry and editor
                return;
            isEdited = true;
            saveButton.Text = "💾";
        }
    }
}