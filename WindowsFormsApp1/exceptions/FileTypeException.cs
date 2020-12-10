using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.exceptions
{
    class FileTypeException : Exception
    {
        public FileTypeException(string message) : base(message)
        {

        }
    }
}
