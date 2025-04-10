using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePocket.Application.Exceptions
{
    public class NoteNotFoundException : NotFoundException
    {
        public NoteNotFoundException(Guid pocketId)
            : base($"The note with id '{pocketId}' doesn't exist in the database.")
        {
        }
    }

}
