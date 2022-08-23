using HomeOrganizerApp.Models.DTOs;
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
    public partial class AddNewAd : ContentPage
    {
        public AddNewAd()
        {
            InitializeComponent();
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void Send_Button(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Editor_Name.Text))
            {
                AdDto ad = new AdDto();
                ad.AuthorName = Preferences.Get("userName", string.Empty);
                ad.TextBody = Editor_Name.Text;
                ad.AuthorId = "none";
                ad.AuthorAvatar = "none";
                ad.IsVoting = false;
                ad.GroupId = Convert.ToInt32(Preferences.Get("CurrentGroup", string.Empty));
                await ApiService.PostAd(ad);
                await Navigation.PopModalAsync();
            }
        }
    }
}