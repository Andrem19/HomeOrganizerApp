using HomeOrganizerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeOrganizerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage
    {
        public SignupPage()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoginPage());
        }

        private async void BtnSignUp_Clicked(object sender, EventArgs e)
        {
            if (!EntPassword.Text.Equals(EntConfirmPassword.Text))
            {
                await DisplayAlert("Password missmatch", "Plese check your confirm password", "Cancel");
            }
            else
            {
                var response = await ApiService.RegisterUser(EntEmail.Text, EntPassword.Text);
                if (response)
                {
                    await DisplayAlert("Hi", "Your account has been created\nPlease confirm your email, then you can login", "Alright");
                    await Navigation.PushModalAsync(new LoginPage());
                }
                else
                {
                    await DisplayAlert("oops", "Something went wrong", "Cancel");
                }
            }
        }
    }
}