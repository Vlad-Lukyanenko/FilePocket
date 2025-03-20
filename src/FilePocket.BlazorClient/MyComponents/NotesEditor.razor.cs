using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TinyMCE.Blazor;

namespace FilePocket.BlazorClient.MyComponents
{
    public partial class NotesEditor : IDisposable
    {
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        private DotNetObjectReference<NotesEditor>? _editorRef;
        private string _editorId = $"NotesEditor-{Guid.NewGuid()}";

        protected override void OnParametersSet()
        {
            _editorRef ??= DotNetObjectReference.Create(this);
            base.OnParametersSet();
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
        public Task OnSave(string content)
        {
            Console.WriteLine($"Content saved: {content}");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Console.WriteLine("*************DISPOSE*****************");
            _editorRef?.Dispose();
            GC.SuppressFinalize(this);
        }

        protected class Note
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}
