﻿using FilePocket.BlazorClient.Features;
using FilePocket.BlazorClient.Services.Pockets.Models;
using Newtonsoft.Json;
using System.Text;

namespace FilePocket.BlazorClient.Services.Pockets.Requests
{
    public class PocketRequests : IPocketRequests
    {
        private readonly FilePocketApiClient _apiClient;

        public PocketRequests(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<PocketModel>> GetAllCustomAsync()
        {
            var content = await _apiClient.GetAsync(PocketUrl.GetAllCustom());

            return JsonConvert.DeserializeObject<IEnumerable<PocketModel>>(content)!;
        }

        public async Task<PocketModel> GetDefaultAsync()
        {
            var content = await _apiClient.GetAsync(PocketUrl.GetDefault());

            return JsonConvert.DeserializeObject<PocketModel>(content)!;
        }

        public async Task<PocketModel> GetInfoAsync(Guid pocketId)
        {
            var url = PocketUrl.GetInfo(pocketId);
            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<PocketModel>(content)!;
        }

        public async Task<bool> CreateAsync(CreatePocketModel pocket)
        {
            var content = GetStringContent(pocket);

            var url = PocketUrl.BaseUrl;

            var response = await _apiClient.PostAsync(PocketUrl.BaseUrl, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(PocketModel pocket)
        {
            var content = GetStringContent(pocket);

            var response = await _apiClient.PutAsync(PocketUrl.Update(pocket.Id), content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid pocketId)
        {
            var response = await _apiClient.DeleteAsync(PocketUrl.Update(pocketId));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> MoveToTrashAsync(Guid pocketId)
        {
            var response = await _apiClient.PutAsync(PocketUrl.MoveToTrash(pocketId));

            return response.IsSuccessStatusCode;
        }

        public Task<bool> CheckPocketCapacityAsync(Guid pocket, long fileSize)
        {
            throw new NotImplementedException();
        }

        private static StringContent? GetStringContent(object? obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
