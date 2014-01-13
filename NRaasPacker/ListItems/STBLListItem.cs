using NRaasPacker.Exporters;
using NRaasPacker.Importers;
using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace NRaasPacker.ListItems
{
    class STBLListItem : ListItem, IPreviewListItem
    {
        static Dictionary<ulong, string> sTranslationKeys = new Dictionary<ulong, string>();

        public STBLListItem()
            : base (ResourceType.STBL)
        {
            BackColor = Color.FromKnownColor(KnownColor.LightYellow);
        }

        public static ulong AddKey(string key)
        {
            ulong result = FNV64.GetHash(key);

            sTranslationKeys.Remove(result);
            sTranslationKeys.Add(result, key);

            return result;
        }

        public static string GetKey(ulong key)
        {
            string result = null;
            if (sTranslationKeys.TryGetValue(key, out result))
            {
                return result;
            }

            return "0x" + key.ToString("X16");
        }

        public override void GetHandledTypes(List<ResourceType> types)
        {
            types.Add(ResourceType.STBL);
        }

        public override Importer GetImporter()
        {
            return new STBLImporter(mEntry);
        }

        public override Exporters.Exporter GetExporter()
        {
            return new STBLExporter(mEntry);
        }

        public string GetContents(IPackage package)
        {
            STBLExporter export = GetExporter() as STBLExporter;

            return export.ConvertToString(package);
        }

        public void SetContents(string value, IPackage package)
        {
            STBLImporter import = GetImporter() as STBLImporter;

            import.ImportFromString(value, package);
        }

        public void Test(IPackage package)
        {
            STBLExporter export = GetExporter() as STBLExporter;

            MainForm.Log("Test: " + STBL.GetProperName(Instance), false);

            export.ConvertToString(package);
        }

        public override string GetFilename(bool autoSet, bool fileMustExist)
        {
            string prefix = STBL.GetProperName(Instance);
            if (prefix == null) return "Default";

            string defaultName = null;

            string text = Text.Replace("Strings ", "");
            if (!string.IsNullOrEmpty(text))
            {
                defaultName = "StringTable" + text.Replace(" ", "_") + ".txt";

                if (prefix == "UnhashedKeys")
                {
                    defaultName = defaultName.Replace("UnhashedKeys", "English");
                }
            }
            else
            {
                defaultName = "StringTable" + prefix + ".txt";
            }

            List<string> prefixes = new List<string>();

            prefixes.Add(prefix);
            if (prefix.Contains("Portuguese"))
            {
                if (prefix == "PortugueseStandard")
                {
                    prefixes.Add("StandardPortuguese");
                    prefixes.Add("BrazilianPortuguese");
                }
                else if (prefix == "PortugueseBrazilian")
                {
                    prefixes.Add("BrazilianPortuguese");
                    prefixes.Add("StandardPortuguese");
                }
                prefixes.Add("PortugueseStandard");
                prefixes.Add("PortugueseBrazilian");
                prefixes.Add("Portuguese");
            }
            else if (prefix.Contains("Spanish"))
            {
                if (prefix == "SpanishStandard")
                {
                    prefixes.Add("StandardSpanish");
                    prefixes.Add("MexicanSpanish");
                }
                else if (prefix == "SpanishMexican")
                {
                    prefixes.Add("MexicanSpanish");
                    prefixes.Add("StandardSpanish");
                }
                prefixes.Add("SpanishStandard");
                prefixes.Add("SpanishMexican");
                prefixes.Add("Spanish");
            }
            else if (prefix == "Chinese")
            {
                prefixes.Add("Taiwanese");
            }
            else if (prefix == "Taiwanese")
            {
                prefixes.Add("Chinese");
            }

            prefixes.Add("English");

            return PrivateGetFilename("stbl", "KEYSTR Text Files|*.txt|STBL Files|*.stbl|All Files|*.*", prefixes, defaultName, autoSet, fileMustExist);
        }

        protected override string CreateInstance(string filename, ref ulong instance)
        {
            return "Strings " + STBL.GetPrefix("0x" + instance.ToString("X16"));
        }

        public override object Clone()
        {
            return new STBLListItem();
        }
    }
}
