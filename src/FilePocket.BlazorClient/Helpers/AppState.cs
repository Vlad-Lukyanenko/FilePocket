namespace FilePocket.BlazorClient.Helpers;

public class AppState
{
    public event Action? OnStateChange;
    public void NotifyStateChanged() => OnStateChange?.Invoke();
}
