using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCoffeeApp.ViewModels
{
    class MyCoffeeDetailsViewModel : ViewModelBase
    {
        public AsyncCommand EnableEditCommand { get; }

        public MyCoffeeDetailsViewModel()
        {
            EnableEditCommand = new AsyncCommand(EnableEdit);
        }

        async Task EnableEdit()
        {

        }
    }
}
