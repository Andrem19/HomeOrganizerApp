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
    public partial class ListSettings : ContentPage
    {
        public ListSettings()
        {
            InitializeComponent();
        }

        private async void Delete_List(object sender, EventArgs e)
        {
            bool res = await DisplayAlert("", "Do you want to delete this list?", "Yes", "No");
            if (res)
            {
                await ApiPayloadsService.DeletePayload(Preferences.Get("CurrentGroup", string.Empty), TodoPage.CurrentPayloadId);
                await Navigation.PopModalAsync();
            }
        }

        private async void TapBack_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}