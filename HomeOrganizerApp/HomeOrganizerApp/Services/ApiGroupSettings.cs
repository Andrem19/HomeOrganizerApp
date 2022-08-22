using HomeOrganizer.DTOs;
using HomeOrganizerApp.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace HomeOrganizerApp.Services
{
    public static class ApiGroupSettings
    {
        public static async Task<bool> ChangeGroupName(string Name, string groupId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetAsync(AppSettings.ApiUrl + "api/Group/changeName?Name=" + Name + "&groupId=" + groupId);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
        public static async Task<bool> PostGroupAvatar(MultipartFormDataContent content, string groupId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/Group/Avatar?groupId=" + groupId, content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
        public static async Task<bool> DeleteGroup(string groupId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.DeleteAsync(AppSettings.ApiUrl + "api/Group?groupId=" + groupId);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
        public static async Task<List<UserInGroupDto>> LoadUsers(string groupId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetAsync(AppSettings.ApiUrl + "api/UserSettings?groupId=" + groupId);
            var resp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<UserInGroupDto>>(resp);
            return result;
        }
        public static async Task<bool> AddUserToTheGroup(string inviteCode, string groupId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.PutAsync(AppSettings.ApiUrl + "api/Group/addUser?InviteCode=" + inviteCode + "&GroupId=" + groupId, null);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
        public static async Task<bool> SetRole(SetRoleDto SetRole)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(SetRole);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.PutAsync(AppSettings.ApiUrl + "api/Group/setRole", content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
        public static async Task<bool> DeleteUserFromGroup(string userId, string groupId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.DeleteAsync(AppSettings.ApiUrl + "api/UserSettings?userId=" + userId + "&groupId=" + groupId);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
        public static async Task<bool> LeaveGroup(string groupId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetAsync(AppSettings.ApiUrl + "api/Group/leaveGroup?GroupId=" + groupId);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
    }
}
