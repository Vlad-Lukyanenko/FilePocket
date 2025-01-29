using FilePocket.Domain.Models;

namespace FilePocket.Shared.Helpers
{
    public class FileChunksManager(string root)
    {
        private readonly string _root = root;

        public async Task Persist(string chunksDirectory, int chunkNumber, byte[] buffer)
        {
            string path = Path.Combine(chunksDirectory, $"{chunkNumber:D8}");
            await File.WriteAllBytesAsync(path, buffer);
        }

        public byte[] Read(Guid id, int chunkNumber)
        {
            string targetPath = Path.Combine(_root, id.ToString(), chunkNumber.ToString());
            return File.ReadAllBytes(targetPath);
        }

        public void WriteToStream(Stream stream, Session session)
        {
            using (var sw = new BinaryWriter(stream))
            {
                for (int i = 1; i <= session.FileInfo.TotalNumberOfChunks; i++)
                {
                    sw.Write(Read(session.Id, i));
                }
            }

            stream.Flush();
        }

        public virtual Stream GetFileStream(Session session)
            => new ChunkedFileStream(this, session);

        private class ChunkedFileStream : Stream
        {
            public override long Position { get; set; }
            public override long Length => Session.FileInfo.FileSize;
            public override bool CanWrite { get; }
            public override bool CanSeek { get; }
            public override bool CanRead { get; } = true;

            private Dictionary<long, byte[]> ChunkCache { get; set; } = new Dictionary<long, byte[]>();

            public ChunkedFileStream(FileChunksManager manager, Session session)
            {
                Manager = manager;
                Session = session;

            }
            public FileChunksManager Manager { get; private set; }

            public Session Session { get; private set; }

            public override void Flush() { }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var bytesRead = 0;

                for (int i = 0; i < count; i++)
                {
                    byte b;

                    if (TryReadByte(Position, out b))
                    {
                        buffer[i] = b;
                        Position++;
                        bytesRead++;
                    }
                    else
                    {
                        return bytesRead;
                    }
                }

                return bytesRead;
            }

            private bool TryReadByte(long byteIndex, out byte b)
            {
                b = 0;

                if (byteIndex >= Session.FileInfo.FileSize)
                    return false;

                // calculate chunk index by byte number
                long chunkNumber = (byteIndex / Session.FileInfo.ChunkSize) + 1;

                if (!ChunkCache.ContainsKey(chunkNumber))
                {
                    ChunkCache.Clear();
                    ChunkCache.Add(chunkNumber, Manager.Read(Session.Id, (int)chunkNumber));
                }

                // get the i-th byte inside that chunk
                b = ChunkCache[chunkNumber][byteIndex % Session.FileInfo.ChunkSize];
                return true;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }
            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
    }
}
