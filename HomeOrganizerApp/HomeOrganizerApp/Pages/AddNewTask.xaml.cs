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
    public partial class AddNewTask : ContentPage
    {
        public AddNewTask()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            picker.SelectedIndex = 1;
        }
        private async void TapBack_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Send_Button(object sender, EventArgs e)
        {
            TaskItemDto TaskToCreate = new TaskItemDto();
            TaskToCreate.Title = Title_Name.Text;
            TaskToCreate.Description = Description_Name.Text;
            TaskToCreate.Type = (TYPE)picker.SelectedIndex;
            TaskToCreate.GroupId = Convert.ToInt32(Preferences.Get("CurrentGroup", string.Empty));
            TaskToCreate.PayloadId = Convert.ToInt32(Preferences.Get("PayloadId", string.Empty));

            bool res = await ApiPayloadsService.CreateNewTask(TaskToCreate);
            if (res)
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}