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
    class IMAGListItem : ListItem
    {
        ImageResource.ImageResource mResource = null;

        public IMAGListItem()
            : base(ResourceType.IMAG)
        {
            BackColor = Color.FromKnownColor(KnownColor.LightSalmon);
        }

        protected override void Set(IResourceIndexEntry entry, IPackage package)
        {
            base.Set(entry, package);

            mResource = ResourceHandlers.CreateResource(entry, package) as ImageResource.ImageResource;
        }

        public override void GetHandledTypes(List<ResourceType> types)
        {
            types.Add(ResourceType.IMAG);
        }

        public override Importer GetImporter()
        {
            return new IMAGImporter(mEntry);
        }

        public override Exporters.Exporter GetExporter()
        {
            return new IMAGExporter(mEntry);
        }

        public override string GetFilename(bool autoSet, bool fileMustExist)
        {
            string defaultName = null;
            if (!string.IsNullOrEmpty(Text))
            {
                defaultName = Text;
                if (defaultName.Contains(".dll"))
                {
                    defaultName = MainForm.PackageName.Replace("_", "");
                }

                defaultName = defaultName.Replace(".", "");

                string packagePrefix = MainForm.PackagePrefix;
                if (!string.IsNullOrEmpty(packagePrefix))
                {
                    defaultName = defaultName.Replace(packagePrefix, "");
                }

                defaultName += ".png";
            }

            return PrivateGetFilename("png", "PNG Files|*.png|All Files|*.*", new List<string>(), defaultName, autoSet, fileMustExist);
        }

        protected override string CreateInstance(string filename, ref ulong instance)
        {
            string name = Path.GetFileName(filename);

            instance = FNV64.GetHash(name);

            return name;
        }

        public override object Clone()
        {
            return new IMAGListItem();
        }
    }
}
