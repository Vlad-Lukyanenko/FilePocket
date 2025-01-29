namespace FilePocket.Domain.Models
{
    public class FileChunkedUploadModel
    {
        public virtual ISet<int> AlreadyPersistedChunks { get; private set; } = new HashSet<int>();
        public long FileSize { get; set; }
        public string OriginalName { get; set; }
        public int ChunkSize { get; set; }

        public FileChunkedUploadModel(long fileSize, string fileName, int chunkSize)
        {
            FileSize = fileSize;
            OriginalName = fileName;
            ChunkSize = chunkSize;
        }

        public virtual int TotalNumberOfChunks
        {
            get
            {
                return (int)Math.Ceiling(FileSize / (ChunkSize * 1F));
            }
        }

        public virtual void MarkChunkAsPersisted(int chunkNumber)
        {
            AlreadyPersistedChunks.Add(chunkNumber);
        }
    }
}
