using HomeOrganizerApp.Models.DTOs;
using HomeOrganizerApp.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
        private MediaFile _mediaFile;
        public AdsPage(string groupId)
        {
            InitializeComponent();
            GroupCollection = new ObservableCollection<GroupDto>();
            AdsCollection = new ObservableCollection<AdDto>();
            LoadMyGroups(groupId);
            setUpAvatar();
            isPlusVisible();

        }
        public AdsPage()
        {
            InitializeComponent();
            GroupCollection = new ObservableCollection<GroupDto>();
            AdsCollection = new ObservableCollection<AdDto>();
            LoadMyGroups();
            setUpAvatar();
            isPlusVisible();
            
        }
        public async void isPlusVisible()
        {
            await ApiService.MyRoleInTheGroup(Preferences.Get("CurrentGroup", string.Empty));
            var role = Preferences.Get("MyRole", string.Empty);
            if (role == "CREATOR" || role == "MODERATOR")
            {
                plus_ads.IsVisible = true;
            }
            else
            {
                plus_ads.IsVisible = false;
            }
        }
        public async void setUpAvatar()
        {
            AvatarImg.Source = "touchface.png";
            LblUserName.Text = Preferences.Get("userName", string.Empty);
            var picture = await ApiService.GetAvatar(Preferences.Get("CurrentGroup", string.Empty));
            if (!string.IsNullOrEmpty(picture))
            {
                AvatarImg.Source = picture;
            }
        }

        public async void ChooseGroup(int index)
        {
            var groups = GroupCollection.ToList();
            var ads = await ApiService.GetAdsByGroupId(groups[index].Id);
            ads.Reverse();
            
            Group_Label.Text = $"{groups[index].GroupName} Group";
            Preferences.Set("CurrentGroup", groups[index].Id.ToString());
            for (int i = 0; i < ads.Count; i++)
            {
                AdsCollection.Add(ads[i]);
            }
            CvAds.ItemsSource = AdsCollection;
        }

        public async void LoadMyGroups(string groupId = "0")
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
            int index = 0;
            if (groupId != "0")
            {
                List<GroupDto> grp = GroupCollection.ToList();
                index = grp.FindIndex(x => x.Id == Convert.ToInt32(groupId));
            }
            ChooseGroup(index);
        }

        private async void CvGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var currentSelection = e.CurrentSelection.FirstOrDefault() as GroupDto;
            var ads = await ApiService.GetAdsByGroupId(currentSelection.Id);
            ads.Reverse();
            Group_Label.Text = $"{currentSelection.GroupName} Group";
            Preferences.Set("CurrentGroup", currentSelection.Id.ToString());
            setUpAvatar();
            isPlusVisible();
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

        private async void OnImageAvatarTapped(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No PickPhoto", ":( No PickPhoto available.", "OK");
                return;
            }

            _mediaFile = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFile == null)
                return;

            var content = new MultipartFormDataContent();

            content.Add(new StreamContent(_mediaFile.GetStream()),
                "\"file\"",
                $"\"{_mediaFile.Path}\"");
            string currentGroup = Preferences.Get("CurrentGroup", string.Empty);
            await ApiService.PostAvatar(content, currentGroup);

            setUpAvatar();
        }

        private async void OnPlusTapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddNewAd());
        }

        private async void OnGroupTapped(object sender, EventArgs e)
        {
            Image lblClicked = (Image)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            var id = item.CommandParameter;

            var ads = await ApiService.GetAdsByGroupId(Convert.ToInt32(id));
            ads.Reverse();
            GroupDto currentGroup = GroupCollection.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
            Group_Label.Text = $"{currentGroup.GroupName} Group";
            Preferences.Set("CurrentGroup", id.ToString());
            setUpAvatar();
            isPlusVisible();
            AdsCollection.Clear();
            for (int i = 0; i < ads.Count; i++)
            {
                AdsCollection.Add(ads[i]);
            }

            CvAds.ItemsSource = AdsCollection;
        }
    }
}