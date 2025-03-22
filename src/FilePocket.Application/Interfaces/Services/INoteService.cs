using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePocket.Application.Interfaces.Services
{
    public interface INoteService
    {
        Task<IEnumerable<NoteModel>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<NoteModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<NoteCreateResponse> CreateAsync(NoteCreateModel note, CancellationToken cancellationToken = default);
        Task<NoteUpdateResponse> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
