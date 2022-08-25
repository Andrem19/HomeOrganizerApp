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
    public static class ApiPayloadsService
    {
        public static async Task<List<PayloadDto>> GetPayloadsByGroupId(string groupId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetAsync(AppSettings.ApiUrl + "api/Payload/payloads?GroupId=" + groupId);
            var resp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<PayloadDto>>(resp);
            return result;
        }
        public static async Task<List<TaskItemDto>> GetTasksByPayloadId(string groupId, string payloadId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetAsync(AppSettings.ApiUrl + "api/Payload/tasks?GroupId=" + groupId + "&PayloadId=" + payloadId);
            var resp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<TaskItemDto>>(resp);
            return result;
        }
        public static async Task<List<TaskItemDto>> ChangeTaskComplete(CheckedChangeDto changeDto)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(changeDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/Payload/checkedChange", content);
            var resp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<TaskItemDto>>(resp);
            return result;
        }
        public static async Task<string> CreateNewPayload(string groupId, string Name)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Payload/create?GroupId=" + groupId + "&Name=" + Name);
            return response;
        }
        public static async Task<bool> CreateNewTask(TaskItemDto NewTask)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(NewTask);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/Payload", content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
        public static async Task<bool> DeleteTask(string groupId, string payloadId, string taskId)
        {
            string token = Preferences.Get("accessToken", string.Empty);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var response = await httpClient.DeleteAsync(AppSettings.ApiUrl + "api/Payload/task?TaskId=" + taskId + "&PayloadId=" + payloadId + "&GroupId=" + groupId);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }
    }
}
