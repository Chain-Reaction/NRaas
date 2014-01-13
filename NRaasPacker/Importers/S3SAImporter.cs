using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NRaasPacker.Importers
{
    class S3SAImporter : Importer
    {
        public S3SAImporter(IResourceIndexEntry entry)
            : base (entry)
        { }

        public override bool Import(string filename, IPackage package)
        {
            ScriptResource.ScriptResource resource = CreateResource(package) as ScriptResource.ScriptResource;

            FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            try
            {
                resource.Assembly = new BinaryReader(stream);

                package.ReplaceResource(mEntry, resource);
                return true;
            }
            catch (Exception ex)
            {
                MainForm.IssueError(ex, "Import failed.");
                return false;
            }
            finally
            {
                stream.Close();
            }
        }

        public override bool Import(Exporters.Exporter exporter, IPackage package)
        {
            throw new NotImplementedException();
        }
    }
}
