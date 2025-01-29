namespace FilePocket.Client.Features
{
    public class NavigationHistoryService
    {
        private readonly List<string> _history = new();

        public void AddToHistory(string url)
        {
            _history.Add(url);
        }

        public string? GetPreviousUrl()
        {
            if (_history.Count > 1)
            {
                return _history[^2];
            }

            return null;
        }
    }
}
