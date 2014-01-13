using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRaasPacker.Exporters
{
    public abstract class Exporter : ResourceBase
    {
        public Exporter(IResourceIndexEntry entry)
            : base(entry)
        { }

        public abstract bool Export(string filename, IPackage package);
    }
}
