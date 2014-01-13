using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NRaasPacker.Exporters
{
    class S3SAExporter : Exporter
    {
        public S3SAExporter(IResourceIndexEntry entry)
            : base (entry)
        { }

        public override bool Export(string filename, IPackage package)
        {
            ScriptResource.ScriptResource resource = CreateResource(package) as ScriptResource.ScriptResource;

            FileStream fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                BinaryReader stream = resource.Assembly;

                if (stream.BaseStream.CanSeek) stream.BaseStream.Position = 0;

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
