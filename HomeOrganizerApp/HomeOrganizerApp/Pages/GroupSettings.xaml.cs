using HomeOrganizerApp.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
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
    public partial class GroupSettings : ContentPage
    {
        private MediaFile _mediaFile;
        private string groupN = "";
        public GroupSettings(string groupName)
        {
            InitializeComponent();
            groupN = groupName;
            GroupName();
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        public void GroupName()
        {
            Title.Text = groupN;
        }

        private async void ChangeName_Button(object sender, EventArgs e)
        {
            await ApiGroupSettings.ChangeGroupName(NameOfGroup.Text, Preferences.Get("CurrentGroup", string.Empty));
            await Navigation.PopModalAsync();
        }

        private async void SetAvatar_Button(object sender, EventArgs e)
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
            await ApiGroupSettings.PostGroupAvatar(content, currentGroup);

            await Navigation.PopModalAsync();
        }

        private async void DeleteGroup_Button(object sender, EventArgs e)
        {
            var action = await DisplayAlert($"Delete Group {groupN}", "Are you sure?", "Yes", "No");
            if (action)
            {
                await ApiGroupSettings.DeleteGroup(Preferences.Get("CurrentGroup", string.Empty));
                Preferences.Set("CurrentGroup", string.Empty);
                await Navigation.PopModalAsync();
            }
        }

        private async void UserSettings_Button(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new UsersInGroup());
        }
    }
}