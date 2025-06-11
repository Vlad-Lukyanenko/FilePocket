using FilePocket.BlazorClient.Features.Notes.Models;
using System.Net.Http.Json;
using Newtonsoft.Json;


namespace FilePocket.BlazorClient.Features.Notes.Requests
{
    public class NoteRequests : INoteRequests
    {
        private readonly FilePocketApiClient _apiClient;
        public NoteRequests(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<NoteCreateResponse> CreateAsync(NoteCreateModel note)
        {
            var response = await _apiClient.PostAsJsonAsync(NoteUrl.Create(), note);

            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<NoteCreateResponse>();

                return result!;
            }

            return new NoteCreateResponse();
        }

        public async Task<bool> DeleteAsync(Guid noteId)
        {
            var response = await _apiClient.DeleteAsync(NoteUrl.Delete(noteId));

            return response.IsSuccessStatusCode;
        }

        public async Task<List<NoteModel>> GetAllByFolderId(Guid? folderId)
        {
            var content = await _apiClient.GetAsync(NoteUrl.GetAllByUserIdAndFolderId(folderId));
            var notes = JsonConvert.DeserializeObject<List<NoteModel>>(content) ?? [];

            return notes;
        }

        public async Task<NoteModel> GetByIdAsync(Guid id)
        {
            var content = await _apiClient.GetAsync(NoteUrl.GetById(id));
            var note = JsonConvert.DeserializeObject<NoteModel>(content) ?? new();

            return note;
        }

        public async Task<bool> IrreversiblyDeleteAsync(Guid noteId)
        {
            var response = await _apiClient.DeleteAsync(NoteUrl.IrreversiblyDelete(noteId));

            return response.IsSuccessStatusCode;
        }

        public async Task<NoteUpdateResponse> UpdateAsync(NoteModel note)
        {
            var response = await _apiClient.PutAsJsonAsync(NoteUrl.Update(), note);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<NoteUpdateResponse>();

                return result!;
            }

            return new NoteUpdateResponse();
        }
    }
}
