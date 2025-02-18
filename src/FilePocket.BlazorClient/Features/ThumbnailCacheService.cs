using FilePocket.BlazorClient.Features.Files.Models;

namespace FilePocket.BlazorClient.Features
{
    public class ThumbnailCacheService
    {
        //private const string Storename = "thumbnails";

        //private readonly IndexedDBManager _dbManager;

        //public ThumbnailCacheService(IndexedDBManager dbManager)
        //{
        //    _dbManager = dbManager;
        //}

        //public async Task<ThumbnailRecord?> GetThumbnailAsync(string fileName)
        //{
        //    var record = await _dbManager.GetRecordById<string, ThumbnailRecord>(Storename, fileName);

        //    return record;
        //}

        //public async Task AddThumbnailAsync(ThumbnailRecord thumbnailRecord)
        //{
        //    await _dbManager.AddRecord(new StoreRecord<ThumbnailRecord>
        //    {
        //        Storename = Storename,
        //        Data = thumbnailRecord
        //    });
        //}

        //public async Task UpdateThumbnailAsync(ThumbnailRecord thumbnailRecord)
        //{
        //    await _dbManager.UpdateRecord(new StoreRecord<ThumbnailRecord>
        //    {
        //        Storename = Storename,
        //        Data = thumbnailRecord
        //    });
        //}

        //public async Task DeleteThumbnailAsync(string fileName)
        //{
        //    await _dbManager.DeleteRecord(Storename, fileName);
        //}
    }

    public class ThumbnailRecord
    {
        public Guid Id { get; set; }

        public Guid? PocketId { get; set; }

        public string? OriginalName { get; set; }

        public string DataUrl { get; set; } = string.Empty;

        public FileTypes? FileType { get; set; }

        public double FileSize { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
