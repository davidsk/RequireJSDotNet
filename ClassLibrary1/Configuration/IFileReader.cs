using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Configuration
{
    interface IFileReader
    {
        string ReadFile(string path);
    }
}
