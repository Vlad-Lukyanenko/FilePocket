namespace FilePocket.Admin.Services
{
    public class QueryStringConverter
    {
        public string ToQueryString(object? obj)
        {
            var query = new QueryString();

            if (obj == null) return string.Empty;

            var props = obj.GetType().GetProperties();

            foreach (var prop in props)
            {
                query += QueryString.Create(prop.Name, $"{prop.GetValue(obj)}");
            }

            return query.ToString();
        }
    }
}
