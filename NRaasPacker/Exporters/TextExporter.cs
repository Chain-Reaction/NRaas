using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NRaasPacker.Exporters
{
    class TextExporter : Exporter
    {
        public TextExporter(IResourceIndexEntry entry)
            : base (entry)
        { }

        public override bool Export(string filename, IPackage package)
        {
            TextResource.TextResource resource = CreateResource(package) as TextResource.TextResource;

            try
            {
                using (StreamWriter file = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    StreamReader stream = resource.TextFileReader as StreamReader;

                    if (stream.BaseStream.CanSeek) stream.BaseStream.Position = 0;

                    file.Write(stream.ReadToEnd());
                }
                return true;
            }
            catch (Exception ex)
            {
                MainForm.IssueError(ex, "Export failed.");
                return false;
            }
        }
    }
}
