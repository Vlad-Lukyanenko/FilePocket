using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePocket.Shared.Exceptions
{
    public class InvalidFileTypeException(string fileType)
        : ArgumentException($"Invalid file type '{fileType}' for image file")
    { }
}
