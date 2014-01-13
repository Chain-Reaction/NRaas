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
    class S3SAListItem : ListItem
    {
        ScriptResource.ScriptResource mResource = null;

        public S3SAListItem()
            : base(ResourceType.S3SA)
        {
            BackColor = Color.FromKnownColor(KnownColor.LightCyan);
        }

        protected override void Set(IResourceIndexEntry entry, IPackage package)
        {
            base.Set(entry, package);

            mResource = ResourceHandlers.CreateResource(entry, package) as ScriptResource.ScriptResource;
        }

        public override void GetHandledTypes(List<ResourceType> types)
        {
            types.Add(ResourceType.S3SA);
        }

        public string Version
        {
            get
            {
                return mResource.GameVersion;
            }
            set
            {
                if (mResource.Version == 2)
                {
                    mResource.GameVersion = value;

                    mPackage.ReplaceResource(mEntry, mResource);
                }
            }
        }

        public override Importer GetImporter()
        {
            return new S3SAImporter(mEntry);
        }

        public override Exporters.Exporter GetExporter()
        {
            return new S3SAExporter(mEntry);
        }

        public override string GetFilename(bool autoSet, bool fileMustExist)
        {
            string defaultName = null;
            if (!string.IsNullOrEmpty(Text))
            {
                defaultName = Text;
            }
            else if (mEntry != null)
            {
                defaultName = mEntry["Instance"] + ".dll";
            }

            return PrivateGetFilename("dll", "DLL Files|*.dll|All Files|*.*", new List<string>(), defaultName, autoSet, fileMustExist);
        }

        protected override string CreateInstance(string filename, ref ulong instance)
        {
            string name = Path.GetFileName(filename);

            instance = FNV64.GetHash(name);

            return name;
        }

        public override object Clone()
        {
            return new S3SAListItem();
        }
    }
}
