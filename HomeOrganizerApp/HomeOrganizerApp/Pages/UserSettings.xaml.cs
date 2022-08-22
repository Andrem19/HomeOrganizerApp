using HomeOrganizer.DTOs;
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
    public partial class UserSettings : ContentPage
    {
        private string UserName { get; set; }
        private string Id { get; set; }
        private string UserRole { get; set; }
        public UserSettings(string Role, string UserId, string Name)
        {
            InitializeComponent();
            UserName = Name;
            Id = UserId;
            UserRole = Role;
            Init();
        }
        public void Init()
        {
            UsersInGroupTitle.Text = UserName;
            if (UserRole == "MEMBER")
            {
                picker.SelectedIndex = 2;
            }
            else if (UserRole == "MODERATOR")
            {
                picker.SelectedIndex = 1;
            }
            else if (UserRole == "CREATOR")
            {
                picker.SelectedIndex = 0;
            }
        }

        private async void ApplyNewRole(object sender, EventArgs e)
        {
            SetRoleDto SetRole = new SetRoleDto();
            SetRole.GroupId = Convert.ToInt32(Preferences.Get("CurrentGroup", string.Empty));
            SetRole.UserId = Id;
            SetRole.Role = (ROLE)picker.SelectedIndex;
            if (UserRole != "CREATOR")
            {
                bool res = await ApiGroupSettings.SetRole(SetRole);
                if (res)
                {
                    await DisplayAlert("", "New role has been applied", "Alright");
                }
            }
            else
            {
                var users = await ApiGroupSettings.LoadUsers(Preferences.Get("CurrentGroup", string.Empty));
                var Creators = users.Where(x => x.Role == "CREATOR").ToList();
                if (Creators.Count>1)
                {
                    bool res = await ApiGroupSettings.SetRole(SetRole);
                    if (res)
                    {
                        await DisplayAlert("", "New role has been applied", "Alright");
                    }
                    Application.Current.MainPage = new ShellHomePage();
                }
                else
                {
                    await DisplayAlert("", "You can't change your role if you one creator of this group. Pass creator role to another user.", "Alright");
                }
            }
        }

        private async void DeleteUserFromGroup(object sender, EventArgs e)
        {
            bool res = await ApiGroupSettings.DeleteUserFromGroup(Id, Preferences.Get("CurrentGroup", string.Empty));
            if (res)
            {
                await DisplayAlert("", "User has been deleted", "Alright");
            }
            await Navigation.PopModalAsync();
        }

        private async void TapBack_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}