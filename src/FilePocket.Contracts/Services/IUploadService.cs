using FilePocket.Domain.Models;


namespace FilePocket.Contracts.Services
{
    public interface IUploadService
    {
        Session CreateSession(Guid userId, Guid pocketId, string fileName, long fileSize);

        Session GetSession(string id);

        List<Session> GetAllSessions();

        Task PersistBlock(string sessionId, int chunkNumber, byte[] buffer);

        void WriteToStream(Stream stream, Session session);

        Stream GetFileStream(Session session);

        void DeleteSession(string sessionId);
    }
}
