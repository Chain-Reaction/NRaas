using NRaasPacker.Exporters;
using NRaasPacker.Importers;
using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace NRaasPacker.ListItems
{
    interface IPreviewListItem
    {
        string GetContents(IPackage package);

        void SetContents(string value, IPackage package);
    }
}
