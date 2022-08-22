using HomeOrganizerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeOrganizerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateNewGroup : ContentPage
    {
        public CreateNewGroup()
        {
            InitializeComponent();
        }

        private async void TapBack_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Create_Button(object sender, EventArgs e)
        {
            string id = await ApiService.CreateGroup(NameOfGroup.Text);
            Preferences.Set("CurrentGroup", id);
            await Navigation.PopModalAsync();
        }
    }
}