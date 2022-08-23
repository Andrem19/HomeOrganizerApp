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
    public partial class UsersInGroup : ContentPage
    {
        public ObservableCollection<UserInGroupDto> UserCollection;
        public UsersInGroup()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            UserCollection = new ObservableCollection<UserInGroupDto>();
            LoadUsers();
        }
        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        public async void LoadUsers()
        {
            var users = await ApiGroupSettings.LoadUsers(Preferences.Get("CurrentGroup", string.Empty));
            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.AvatarUrl))
                {
                    user.AvatarUrl = "touchface.png";
                }
                UserCollection.Add(user);
            }
            CvUsers.ItemsSource = UserCollection;
        }
        private async void AddNewUser(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(InviteCodeField.Text))
            {
                await ApiGroupSettings.AddUserToTheGroup(InviteCodeField.Text, Preferences.Get("CurrentGroup", string.Empty));
                UserCollection.Clear();
                InviteCodeField.Text = "";
                LoadUsers();
            }
        }

        private async void UserSettingsTapped(object sender, EventArgs e)
        {
            Image lblClicked = (Image)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            string id = item.CommandParameter.ToString();
            var users = UserCollection.ToList();
            var user = users.FirstOrDefault(x => x.UserId == id);
            await Navigation.PushModalAsync(new UserSettings(user.Role, user.UserId, user.Name));
        }
    }
}