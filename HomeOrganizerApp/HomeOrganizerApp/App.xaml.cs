﻿using HomeOrganizerApp.Pages;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeOrganizerApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var accesstoken = Preferences.Get("accessToken", string.Empty);
            if (string.IsNullOrEmpty(accesstoken))
            {
                MainPage = new NavigationPage(new SignupPage());
            }
            else
            {
                MainPage = new ShellHomePage();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
