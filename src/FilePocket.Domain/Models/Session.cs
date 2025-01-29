namespace FilePocket.Domain.Models
{
    public class Session
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid PocketId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public long Timeout { get; private set; }
        public FileChunkedUploadModel FileInfo { get; private set; }

        private static long DEFAULT_TIMEOUT = 3600L;
        private bool _failed = false;

        public string ChunksDirectory { get; set; }

        public Session(Guid userId, Guid pocketId, FileChunkedUploadModel fileUpload) : this(userId, pocketId, fileUpload, DEFAULT_TIMEOUT) { }

        public double Progress
        {
            get
            {
                if (FileInfo.TotalNumberOfChunks == 0)
                    return 0;

                return SuccessfulChunks / (FileInfo.TotalNumberOfChunks * 1f);
            }
        }

        public string Status
        {
            get
            {
                if (_failed)
                    return "failed";
                else if (IsConcluded)
                    return "done";

                return "ongoing";
            }
        }

        public bool IsConcluded
        {
            get => FileInfo.TotalNumberOfChunks == FileInfo.AlreadyPersistedChunks.Count;
        }

        public int SuccessfulChunks
        {
            get => FileInfo.AlreadyPersistedChunks.Count;
        }

        public bool IsExpired
        {
            get
            {
                TimeSpan span = DateTime.Now - LastUpdate;
                return span.TotalSeconds >= Timeout;
            }
        }
        public bool HasFailed() => _failed;

        public void MaskAsFailed() => _failed = true;

        public void RenewTimeout() => LastUpdate = DateTime.Now;

        public Session(Guid userId, Guid pocketId, FileChunkedUploadModel fileUpload, long timeout)
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            LastUpdate = this.CreatedDate;
            UserId = userId;
            PocketId = pocketId;
            FileInfo = fileUpload;
            Timeout = timeout;
        }
    }
}

