using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NRaasPacker.Exporters
{
    class IMAGExporter : Exporter
    {
        public IMAGExporter(IResourceIndexEntry entry)
            : base (entry)
        { }

        public override bool Export(string filename, IPackage package)
        {
            ImageResource.ImageResource resource = CreateResource(package) as ImageResource.ImageResource;

            FileStream fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                BinaryReader stream = new BinaryReader(resource.Stream);

                stream.BaseStream.Position = 0;

                new BinaryWriter(fileStream).Write(stream.ReadBytes((int)stream.BaseStream.Length));
                return true;
            }
            catch (Exception ex)
            {
                MainForm.IssueError(ex, "Export failed.");
                return false;
            }
            finally
            {
                fileStream.Close();
            }
        }
    }
}
