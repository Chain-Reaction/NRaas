using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRaasPacker.Importers
{
    public abstract class Importer : ResourceBase
    {
        public Importer(IResourceIndexEntry entry)
            : base(entry)
        { }

        public abstract bool Import(string filename, IPackage package);

        public abstract bool Import(Exporters.Exporter exporter, IPackage package);
    }
}
