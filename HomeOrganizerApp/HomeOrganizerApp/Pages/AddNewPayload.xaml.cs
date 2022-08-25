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
    public partial class AddNewPayload : ContentPage
    {
        public AddNewPayload()
        {
            InitializeComponent();
        }

        private async void Create_Button(object sender, EventArgs e)
        {
            string id = await ApiPayloadsService.CreateNewPayload(Preferences.Get("CurrentGroup", string.Empty), NameOfList.Text);
            Preferences.Set("PayloadId", id);
            await Navigation.PopModalAsync();
        }

        private async void TapBack_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}