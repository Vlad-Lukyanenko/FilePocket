namespace FilePocket.Domain.Extensions;

public static class LinqExtensions
{
    public static void ForEach<T>(this ICollection<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
}