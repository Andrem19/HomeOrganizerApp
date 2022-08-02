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
        public ObservableCollection<AdDto> AdsCollection;
        public AdsPage()
        {
            InitializeComponent();
            GroupCollection = new ObservableCollection<GroupDto>();
            AdsCollection = new ObservableCollection<AdDto>();
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

        public void ChooseGroup(int index)
        {
            var groups = GroupCollection.ToList();

                for (int i = 0; i < groups[index].Ad.Count; i++)
                {
                    AdsCollection.Add(groups[index].Ad[i]);
                }

            CvAds.ItemsSource = AdsCollection;
        }

        public async void LoadMyGroups()
        {
            var groups = await ApiService.GetMyGroups();
            if (groups.Count<1)
            {
                await Navigation.PushModalAsync(new InviteCodePage());
            }
            while (GroupCollection.Count<5)
            {
                foreach (var group in groups)
                {
                    GroupCollection.Add(group);
                }
            }
            CvGroups.ItemsSource = GroupCollection;
            ChooseGroup(0);
        }

        private async void CvGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentSelection = e.CurrentSelection.FirstOrDefault() as GroupDto;
            var ads = await ApiService.GetAdsByGroupId(currentSelection.Id);
            AdsCollection.Clear();
            for (int i = 0; i < ads.Count; i++)
            {
                AdsCollection.Add(ads[i]);
            }

            CvAds.ItemsSource = AdsCollection;
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

        private void TodoTask_Change(object sender, CheckedChangedEventArgs e)
        {

        }
    }
}