using FilePocket.BlazorClient.Features.Notes.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace FilePocket.BlazorClient.MyComponents
{
    public partial class NotesEditor : IDisposable
    {

        [Parameter] public string Content { get; set; } = default!;

        [Parameter] public Func<string, Task> OnSaveOrUpdate { get; set; } = default!;

        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

        private DotNetObjectReference<NotesEditor>? _editorRef;
        private readonly string _editorId = $"NotesEditor-{Guid.NewGuid()}";

        protected override async Task  OnParametersSetAsync()
        {
              _editorRef ??= DotNetObjectReference.Create(this);

            await base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeTinyMCE", $"#{_editorId}", _editorRef);
            }
        }

        public async Task<string> GetContentAsync()
        {
            return await JSRuntime.InvokeAsync<string>("getTinyMceContent", $"#{_editorId}");
        }

        [JSInvokable]
        public async Task OnSave(string content)
        {
            await OnSaveOrUpdate.Invoke(content);
        }

        public void Dispose()
        {
            _editorRef?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
