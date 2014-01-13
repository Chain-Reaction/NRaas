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
    class TextListItem : ListItem, IPreviewListItem
    {
        public TextListItem()
            : this(ResourceType.UNKN)
        { }
        public TextListItem(ResourceType type)
            : base (type)
        {
            BackColor = Color.FromKnownColor(KnownColor.LightGreen);
        }

        public override void GetHandledTypes(List<ResourceType> types)
        {
            types.Add(ResourceType._XML);
            types.Add(ResourceType.LAYO);
            types.Add(ResourceType.ITUN);
        }

        public override Importer GetImporter()
        {
            return new TextImporter(mEntry);
        }

        public override Exporters.Exporter GetExporter()
        {
            return new TextExporter(mEntry);
        }

        public string GetContents(IPackage package)
        {
            TextResource.TextResource resource = ResourceHandlers.CreateResource(mEntry, mPackage) as TextResource.TextResource;

            StreamReader reader = resource.TextFileReader as StreamReader;

            reader.BaseStream.Position = 0;

            return reader.ReadToEnd();
        }

        public void SetContents(string value, IPackage package)
        {
            TextResource.TextResource resource = ResourceHandlers.CreateResource(mEntry, mPackage) as TextResource.TextResource;

            resource.TextFileReader = new StringReader(value);

            mPackage.ReplaceResource(mEntry, resource);
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

                defaultName += ".xml";
            }

            string fileType = null;
            switch (Type)
            {
                case ResourceType.ITUN:
                    fileType = "ITUN Files";
                    break;
                case ResourceType.LAYO:
                    fileType = "LAYO Files";
                    break;
                default:
                    fileType = "XML Files";
                    break;
            }

            return PrivateGetFilename("xml", fileType + "|*.xml;*.txt|All Files|*.*", new List<string>(), defaultName, autoSet, fileMustExist);
        }

        protected override string CreateInstance(string filename, ref ulong instance)
        {
            string name = Path.GetFileNameWithoutExtension(filename);

            string prefix = MainForm.PackagePrefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += ".";
            }

            name = prefix + name;

            instance = FNV64.GetHash(name);

            return name;
        }

        public override object Clone()
        {
            return new TextListItem();
        }
    }
}
