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
using System.Threading;
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
        public List<AdDto> AllAds { get; set; } = new List<AdDto>();
        public int currentCount { get; set; }
        private MediaFile _mediaFile;
        public AdsPage()
        {
            InitializeComponent();
        }
        public async void isPlusVisible()
        {
            var currentGroup = Preferences.Get("CurrentGroup", string.Empty);
            var role = "none";
            if (!string.IsNullOrEmpty(currentGroup))
            {
                await ApiService.MyRoleInTheGroup(currentGroup);
                role = Preferences.Get("MyRole", string.Empty);
            }
            plus_ads.IsVisible = false;
            if (role == "CREATOR" || role == "MODERATOR")
            {
                plus_ads.IsVisible = true;
            }
        }
        public async void setUpAvatar()
        {
            AvatarImg.Source = "touchface.png";
            LblUserName.Text = Preferences.Get("userName", string.Empty);
            string curGroup = Preferences.Get("CurrentGroup", string.Empty);
            if (!string.IsNullOrEmpty(curGroup))
            {
                var picture = await ApiService.GetAvatar(curGroup);
                if (!string.IsNullOrEmpty(picture))
                {
                    AvatarImg.Source = picture;
                }
            }
        }

        public async void ChooseGroup(int index)
        {
            var groups = GroupCollection.ToList();
            if (groups.Count > 1)
            {
                AllAds = await ApiService.GetAdsByGroupId(groups[index].Id);
                AllAds.Reverse();

                Group_Label.Text = $"{groups[index].GroupName} Group";
                Preferences.Set("CurrentGroup", groups[index].Id.ToString());
                currentCount = AllAds.Count > 10 ? 10 : AllAds.Count;
                
                for (int i = 0; i < currentCount; i++)
                {
                    AdsCollection.Add(AllAds[i]);
                }
                CvAds.ItemsSource = AdsCollection;
                CvGroups.SelectedItem = GroupCollection[index];
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GroupCollection = new ObservableCollection<GroupDto>();
            AdsCollection = new ObservableCollection<AdDto>();
            LoadMyGroups();
            setUpAvatar();
            isPlusVisible();

            CvAds.RemainingItemsThreshold = 3;
            CvAds.RemainingItemsThresholdReached += CvAds_RemainingItemsThresholdReached;
        }

        private void CvAds_RemainingItemsThresholdReached(object sender, EventArgs e)
        {
            var count = AllAds.Count > (currentCount + 10) ? currentCount + 10 : AllAds.Count;
            currentCount = count;
            for (int i = 0; i < currentCount; i++)
            {
                AdsCollection.Add(AllAds[i]);
            }
            Task.Delay(2000);
        }

        public async void LoadMyGroups()
        {
            var groups = await ApiService.GetMyGroups();
            if (groups != null)
            {
                foreach (var group in groups)
                {
                    if (string.IsNullOrEmpty(group.PictureUrl))
                    {
                        group.PictureUrl = "people.png";
                    }
                    GroupCollection.Add(group);
                }
            }
            GroupDto addGroup = new GroupDto();
            addGroup.Id = 0;
            addGroup.GroupName = "Create New";
            addGroup.PictureUrl = "addgroup.png";
            GroupCollection.Add(addGroup);
            CvGroups.ItemsSource = GroupCollection;

            string grId = Preferences.Get("CurrentGroup", string.Empty);
            int index = 0;
            if (!string.IsNullOrEmpty(grId))
            {
                List<GroupDto> grp = GroupCollection.ToList();
                index = grp.FindIndex(x => x.Id == Convert.ToInt32(grId));
            }
            ChooseGroup(index);
        }

        private async void CvGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var currentSelection = e.CurrentSelection.FirstOrDefault() as GroupDto;
            if (currentSelection.Id != 0)
            {
                AllAds = await ApiService.GetAdsByGroupId(currentSelection.Id);
                AllAds.Reverse();
                Group_Label.Text = $"{currentSelection.GroupName} Group";
                Preferences.Set("CurrentGroup", currentSelection.Id.ToString());
                setUpAvatar();
                isPlusVisible();
                AdsCollection.Clear();
                currentCount = AllAds.Count > 10 ? 10 : AllAds.Count;
                for (int i = 0; i < currentCount; i++)
                {
                    AdsCollection.Add(AllAds[i]);
                }

                CvAds.ItemsSource = AdsCollection;
            }
            else if (currentSelection.Id == 0)
            {
                List<GroupDto> grp = GroupCollection.ToList();
                int index = grp.FindIndex(x => x.Id == Convert.ToInt32(Preferences.Get("CurrentGroup", string.Empty)));
                CvGroups.SelectedItem = GroupCollection[index];
                await Navigation.PushModalAsync(new CreateNewGroup());
            }
        }

        private void TapLogout_Tapped(object sender, EventArgs e)
        {
            Preferences.Set("accessToken", string.Empty);
            Preferences.Set("tokenExpirationTime", 0);
            Preferences.Set("CurrentGroup", string.Empty);
            Preferences.Set("InviteCode", string.Empty);
            Preferences.Set("Email", string.Empty);
            Preferences.Set("userName", string.Empty);
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

        private async void TapInviteCode_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new InviteCodePage());
        }

        private async void TapCreateGroup_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CreateNewGroup());
        }

        private async void GroupSettings_Tapped(object sender, EventArgs e)
        {
            string grId = Preferences.Get("CurrentGroup", string.Empty);
            var role = Preferences.Get("MyRole", string.Empty);
            if (!string.IsNullOrEmpty(grId))
            {
                List<GroupDto> grp = GroupCollection.ToList();
                var group = grp.FirstOrDefault(x => x.Id == Convert.ToInt32(grId));
                if (role == "CREATOR")
                {
                    await Navigation.PushModalAsync(new GroupSettings(group.GroupName));
                }
                else
                {
                    await Navigation.PushModalAsync(new MemberGroupSettings(group.GroupName));
                }
            }
            
        }
    }
}