using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


namespace FilePocket.Infrastructure.Persistence.Repositories.MongoDbRepositories
{
    public class NotesRepository(IOptions<MongoDbSettings> options) : INotesRepository
    {
        private readonly IMongoCollection<Note> _notes =
            MongoDbConnector.ConnectToMongoDb<Note>(
                options.Value.ConnectionString,
                options.Value.DatabaseName,
                options.Value.NotesCollectionName);

        public async Task CreateAsync(Note note, CancellationToken cancellationToken = default)
        {
            await _notes.InsertOneAsync(note, cancellationToken: cancellationToken);
        }

        public async Task UpdateAsync(Note note, CancellationToken cancellationToken = default)
        {
            await _notes.UpdateOneAsync(n => n.Id == note.Id, Builders<Note>.Update
                                                                 .Set(n => n.Title, note.Title)
                                                                 .Set(n => n.Content, note.Content)
                                                                 .Set(n => n.UpdatedAt, DateTime.UtcNow),null, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await _notes.DeleteOneAsync(note => note.Id == id, cancellationToken: cancellationToken);
        }

        public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await _notes.UpdateOneAsync(n => n.Id == id, Builders<Note>.Update
                                                            .Set(n => n.IsDeleted, true)
                                                            .Set(n => n.DeletedAt, DateTime.UtcNow), null, cancellationToken);
        }

        public async Task<List<Note>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _notes.Find(note => note.UserId == userId && !note.IsDeleted).ToListAsync(cancellationToken);
        }

        public async Task<Note> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _notes.Find(n=>n.Id==id).SingleOrDefaultAsync(cancellationToken);
        }
    }
}
