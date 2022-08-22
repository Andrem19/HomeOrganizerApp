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
    public partial class MemberGroupSettings : ContentPage
    {
        public string groupName { get; set; }
        public MemberGroupSettings(string GroupName)
        {
            InitializeComponent();
            groupName = GroupName;
            SetGroupName();
        }

        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        public void SetGroupName()
        {
            Title.Text = groupName;
        }

        private async void LeaveGroup_Button(object sender, EventArgs e)
        {
            await ApiGroupSettings.LeaveGroup(Preferences.Get("CurrentGroup", string.Empty));
            Preferences.Set("CurrentGroup", string.Empty);
            await Navigation.PopModalAsync();
        }
    }
}