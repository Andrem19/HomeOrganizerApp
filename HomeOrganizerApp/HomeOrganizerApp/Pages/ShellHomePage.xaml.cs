using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeOrganizerApp.Pages
{
    public partial class ShellHomePage : Shell
    {
        public ShellHomePage()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(TodoPage), typeof(TodoPage));
            Routing.RegisterRoute(nameof(AdsPage), typeof(AdsPage));

        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {

        }
    }
}