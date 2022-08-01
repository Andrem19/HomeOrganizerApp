using HomeOrganizerApp.Models;
using HomeOrganizerApp.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace HomeOrganizerApp.Services
{
    static public class ApiService
    {
        public static async Task<bool> RegisterUser(string email, string password)
        {
            var register = new Register()
            {
                displayName = email,
                email = email,
                password = password
            };
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(register);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/Account/register", content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public static async Task<bool> Login(string email, string password)
        {
            var login = new Login()
            {
                Email = email,
                Password = password
            };

            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/Account/login", content);
            if (!response.IsSuccessStatusCode) return false;
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserDto>(jsonResult);
            Preferences.Set("accessToken", result.Token);
            Preferences.Set("userName", result.DisplayName);
            Preferences.Set("Email", result.Email);
            Preferences.Set("Avatar", result.AvatarUrl);

            return true;
        }

        public static async Task<List<GroupDto>> GetMyGroups()
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetAsync(AppSettings.ApiUrl + "api/Group");
            var resp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<GroupDto>>(resp);
            return result;
        }
    }
}
