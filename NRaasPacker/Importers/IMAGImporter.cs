using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NRaasPacker.Importers
{
    class IMAGImporter : Importer
    {
        public IMAGImporter(IResourceIndexEntry entry)
            : base (entry)
        { }

        public override bool Import(string filename, IPackage package)
        {
            ImageResource.ImageResource resource = CreateResource(package) as ImageResource.ImageResource;

            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            try
            {
                using (BinaryReader stream = new BinaryReader(fileStream))
                {
                    resource.Stream.Position = 0;
                    resource.Stream.SetLength(stream.BaseStream.Length);
                    resource.Stream.Write(stream.ReadBytes((int)stream.BaseStream.Length), 0, (int)stream.BaseStream.Length);
                }

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
                fileStream.Close();
            }
        }

        public override bool Import(Exporters.Exporter exporter, IPackage package)
        {
            throw new NotImplementedException();
        }
    }
}
