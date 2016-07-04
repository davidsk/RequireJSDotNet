// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

namespace ClassLibrary1.Configuration
{
    using ClassLibrary1.Models;

    internal interface IConfigWriter
    {
        string Path { get; }

        void WriteConfig(ConfigurationCollection conf);
    }
}
