using HomeOrganizer.DTOs;
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
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeOrganizerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodoPage : ContentPage
    {
        public ObservableCollection<GroupDto> GroupCollection;
        public ObservableCollection<PayloadDto> PayloadCollection;
        public ObservableCollection<TaskItemDto> TasksCollection;
        public List<GroupDto> Groups { get; set; } = new List<GroupDto>();
        public static int groupIndex { get; set; }
        public static string CurrentPayload { get; set; }
        public int currentCount { get; set; }
        private MediaFile _mediaFile;

        public TodoPage()
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
            AddTask_Name.IsVisible = false;
            plus_ads.IsVisible = false;
            if (role == "CREATOR" || role == "MODERATOR")
            {
                plus_ads.IsVisible = true;
                AddTask_Name.IsVisible = true;
            }
        }
        public async void setUpAvatar()
        {
            AvatarImg.Source = "touchface.png";
            LblUserName.Text = Preferences.Get("userName", string.Empty);
            string curGroup = Preferences.Get("CurrentGroup", string.Empty);
            if (!string.IsNullOrEmpty(curGroup))
            {
                string picture = await ApiService.GetAvatar(curGroup);
                if (!string.IsNullOrEmpty(picture))
                {
                    AvatarImg.Source = picture;
                }
            }
        }

        public void ChooseGroup(int index)
        {
            groupIndex = index;
            var groups = GroupCollection.ToList();
            if (groups.Count > 1)
            {
                Group_Label.Text = $"{groups[index].GroupName} Group";
                Preferences.Set("CurrentGroup", groups[index].Id.ToString());
                CvGroups.SelectedItem = GroupCollection[index];
            }
        }
        protected override void OnAppearing()
        {
            GroupCollection = new ObservableCollection<GroupDto>();
            PayloadCollection = new ObservableCollection<PayloadDto>();
            TasksCollection = new ObservableCollection<TaskItemDto>();
            LoadMyGroups();

            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //Preferences.Set("PayloadId", string.Empty);
            groupIndex = 0;
            CurrentPayload = null;
            GroupCollection = new ObservableCollection<GroupDto>();
            PayloadCollection = new ObservableCollection<PayloadDto>();
            TasksCollection = new ObservableCollection<TaskItemDto>();
            CvTodoList.ItemsSource = TasksCollection;
        }

        public async void LoadMyGroups()
        {
            Groups = await ApiService.GetMyGroupsWithPayloads();
            if (Groups != null)
            {
                foreach (var group in Groups)
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
            Preferences.Set("PayloadId", string.Empty);
            var currentSelection = e.CurrentSelection.FirstOrDefault() as GroupDto;
            
            if (currentSelection.Id != 0)
            {
                groupIndex = Groups.IndexOf(currentSelection);
                Preferences.Set("CurrentGroup", currentSelection.Id.ToString());
                setUpAvatar();
                isPlusVisible();
                LoadPayloads();

            }
            else if (currentSelection.Id == 0)
            {
                List<GroupDto> grp = GroupCollection.ToList();
                int index = grp.FindIndex(x => x.Id == Convert.ToInt32(Preferences.Get("CurrentGroup", string.Empty)));
                CvGroups.SelectedItem = GroupCollection[index];
                await Navigation.PushModalAsync(new CreateNewGroup());
            }
        }
        public async void LoadPayloads()
        {
            //Payloads = await ApiPayloadsService.GetPayloadsByGroupId(Preferences.Get("CurrentGroup", string.Empty));
            
            if (Groups[groupIndex].Payloads != null && Groups[groupIndex].Payloads.Count > 0)
            {
                PayloadCollection.Clear();
                foreach (var payload in Groups[groupIndex].Payloads)
                {
                    PayloadCollection.Add(payload);
                }
                CvPayloads.ItemsSource = PayloadCollection;


                string payloadId = Preferences.Get("PayloadId", string.Empty);
                int index = 0;
                if (!string.IsNullOrEmpty(payloadId))
                {
                    List<PayloadDto> payList = PayloadCollection.ToList();
                    index = payList.FindIndex(x => x.Id.ToString() == payloadId);
                }
                LoadTasks(index);
                CvPayloads.SelectedItem = PayloadCollection[index];
            }
            else
            {
                TasksCollection.Clear();
                CvTodoList.ItemsSource = TasksCollection;
                PayloadCollection.Clear();
                CvPayloads.ItemsSource = PayloadCollection;
            }
        }
        public async void LoadTasks(int index)
        {
            //var tasks = await ApiPayloadsService.GetTasksByPayloadId(Preferences.Get("CurrentGroup", string.Empty), CurrentPayload);
            if (TasksCollection.Count > 0)
            {
                TasksCollection.Clear();
            }
            if (Groups[groupIndex].Payloads[index].Tasks != null)
            {
                foreach (var task in Groups[groupIndex].Payloads[index].Tasks)
                {
                    if (task.Complete)
                    {
                        task.Color = "#92E1AF";
                    }
                    else
                    {
                        task.Color = "#F7D2DF";
                    }
                    TasksCollection.Add(task);
                }
                CvTodoList.ItemsSource = TasksCollection;
            }
        }


        private void TapLogout_Tapped(object sender, EventArgs e)
        {
            Preferences.Set("accessToken", string.Empty);
            Preferences.Set("tokenExpirationTime", 0);
            Preferences.Set("CurrentGroup", string.Empty);
            Preferences.Set("PayloadId", string.Empty);
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
            await Navigation.PushModalAsync(new AddNewPayload());
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

        private async void TaskCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            var taskItem = (CheckBox)sender;
            var item = (TaskItemDto)taskItem.BindingContext;
            
            if (item != null)
            {
                //lastTaskChangeId = item.Id;
                var id = item.Id;
                CheckedChangeDto checkedChangeDto = new CheckedChangeDto();
                checkedChangeDto.GroupId = Convert.ToInt32(Preferences.Get("CurrentGroup", string.Empty));
                checkedChangeDto.PayloadId = Convert.ToInt32(Preferences.Get("PayloadId", string.Empty));
                checkedChangeDto.TaskId = id;
                checkedChangeDto.Value = e.Value;
                var newtasks = await ApiPayloadsService.ChangeTaskComplete(checkedChangeDto);
                int index = TasksCollection.IndexOf(item);
                item.Complete = e.Value;
                if (e.Value)
                {
                    item.Color = "#92E1AF";
                }
                else
                {
                    item.Color = "#F7D2DF";
                }
                TasksCollection[index] = item;
            }
        }

        private void OnImageInfoTapped(object sender, EventArgs e)
        {

        }

        private void OnImageRemoveTapped(object sender, EventArgs e)
        {

        }

        private void CvPayloads_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentSelection = e.CurrentSelection.FirstOrDefault() as PayloadDto;
            Preferences.Set("PayloadId", currentSelection.Id.ToString());
            int index = Groups[groupIndex].Payloads.IndexOf(currentSelection);
            LoadTasks(index);
        }

        private void RefreshMyTasks(object sender, EventArgs e)
        {
            RefreshV.IsRefreshing = true;
            OnAppearing();
            RefreshV.IsRefreshing = false;
        }

        private async void OnAddTaskTapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddNewTask());
        }
    }
}