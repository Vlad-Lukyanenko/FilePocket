using FilePocket.Domain.Entities.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePocket.Domain.Entities
{
    public class FileBaseMetadata : IBaseMetadata
    {
        public Guid Id { get; init; }
        public string ActualName { get; init; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
