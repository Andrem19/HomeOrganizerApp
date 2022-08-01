using HomeOrganizerApp.Models.DTOs;
using HomeOrganizerApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeOrganizerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdsPage : ContentPage
    {
        public ObservableCollection<GroupDto> GroupCollection;
        public AdsPage()
        {
            InitializeComponent();
            GroupCollection = new ObservableCollection<GroupDto>();
            setUpAvatar();
            LoadMyGroups();
        }

        public void setUpAvatar()
        {
            AvatarImg.Source = "touchface.png";
            LblUserName.Text = Preferences.Get("userName", string.Empty);
            string picture = Preferences.Get("Avatar", string.Empty);
            if (!string.IsNullOrEmpty(picture))
            {
                AvatarImg.Source = picture;
            }
        }

        public async void LoadMyGroups()
        {
            var groups = await ApiService.GetMyGroups();
            foreach (var group in groups)
            {
                GroupCollection.Add(group);
            }
            CvGroups.ItemsSource = GroupCollection;

        }

        private void CvGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TapLogout_Tapped(object sender, EventArgs e)
        {
            Preferences.Set("accessToken", string.Empty);
            Preferences.Set("tokenExpirationTime", 0);
            Application.Current.MainPage = new NavigationPage(new SignupPage());
        }

        private void TapCloseMenu_Tapped(object sender, EventArgs e)
        {
            CloseHamBurgerMenu();
        }
        private async void CloseHamBurgerMenu()
        {
            await SlMenu.TranslateTo(-250, 0, 400, Easing.Linear);
            GridOverlay.IsVisible = false;
        }
        private async void ImgMenu_Tapped(object sender, EventArgs e)
        {
            GridOverlay.IsVisible = true;
            await SlMenu.TranslateTo(0, 0, 400, Easing.Linear);
        }
    }
}