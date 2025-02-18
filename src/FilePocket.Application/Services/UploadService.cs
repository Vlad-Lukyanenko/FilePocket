using FilePocket.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;


namespace FilePocket.Application.Services
{
    public class UploadService(IConfiguration configuration) : IUploadService
    {
        //private readonly Dictionary<string, Session> _sessions = [];
        //private readonly FileChunksManager _chunksManager =
        //    new(configuration.GetValue<string>("AppRootFolder")!);

        //public Session CreateSession(Guid userId, Guid pocketId, string fileName, long fileSize)
        //{
        //    if (string.IsNullOrWhiteSpace(fileName))
        //        throw new Exception("File name missing");

        //    if (fileSize < 1)
        //        throw new Exception("Total size must be greater than zero");

        //    var chankSize = GetChunkSize(fileSize);
        //    var session = new Session(userId, pocketId, new FileChunkedUploadModel(fileSize, fileName, chankSize));

        //    _sessions.Add(session.Id.ToString(), session);

        //    return session;
        //}

        //public Session GetSession(string id)
        //{
        //    return _sessions[id];
        //}

        //public List<Session> GetAllSessions()
        //{
        //    return [.. _sessions.Values];
        //}

        //public async Task PersistBlock(string sessionId, int chunkNumber, byte[] buffer)
        //{
        //    var session = GetSession(sessionId);

        //    try
        //    {
        //        if (session == null)
        //        {
        //            throw new Exception("Session not found");
        //        }

        //        await _chunksManager.Persist(session.ChunksDirectory, chunkNumber, buffer);

        //        session.FileInfo.MarkChunkAsPersisted(chunkNumber);
        //        session.RenewTimeout();
        //    }
        //    catch (Exception e)
        //    {
        //        if (session != null)
        //            session.MaskAsFailed();

        //        throw e;
        //    }
        //}

        //public void WriteToStream(Stream stream, Session session)
        //{
        //    _chunksManager.WriteToStream(stream, session);
        //}

        //public Stream GetFileStream(Session session)
        //{
        //    return _chunksManager.GetFileStream(session);
        //}

        //public void DeleteSession(string sessionId)
        //{
        //    _sessions.Remove(sessionId);
        //}

        //private static int GetChunkSize(long fileSize)
        //{
        //    // temporary - due to issue in Files.razor.cs with max chunk size = 32256 B
        //    var chunkSize = (int)(1024 * 31.5);

        //    if (fileSize < chunkSize) return (int)fileSize;
        //    return chunkSize;
        //}
    }
}
