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
    public partial class InviteCodePage : ContentPage
    {
        public InviteCodePage()
        {
            InitializeComponent();
            InitInviteCode();
        }
        public void InitInviteCode()
        {
            ButtonCode.Text = Preferences.Get("InviteCode", string.Empty);
        }

        private async void ButtonCode_Button(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(Preferences.Get("InviteCode", string.Empty));
            await DisplayAlert("", "Code Copied", "OK");
        }
        private void TapBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}