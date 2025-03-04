namespace FilePocket.BlazorClient.Features;

public class NavigationHistoryService
{
    private readonly List<string> _history = new();
    private bool _getPreviousWasCalled = false;

    public void AddToHistory(string url)
    {
        if (_getPreviousWasCalled)
        {
            _getPreviousWasCalled = false;

            return;
        }

        _history.Add(url);
    }

    public string? GetPreviousUrl()
    {
        if (_history.Count > 1)
        {
            var previousUrl = _history[^2];
            _history.RemoveAt(_history.Count - 1);
            _getPreviousWasCalled = true;

            return previousUrl;
        }

        return _history[0];
    }
}
