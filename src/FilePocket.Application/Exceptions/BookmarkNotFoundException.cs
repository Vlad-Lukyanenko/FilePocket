namespace FilePocket.Application.Exceptions;

public class BookmarkNotFoundException : NotFoundException
{
    public BookmarkNotFoundException(Guid bookmarkId)
        : base($"The bookmark with id '{bookmarkId}' doesn't exist in the database.")
    {
    }
}
