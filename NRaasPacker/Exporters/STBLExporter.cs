using NRaasPacker.Importers;
using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NRaasPacker.Exporters
{
    class STBLExporter : Exporter
    {
        static string[] sSuffixes = new string[] { "_Female", "_Male", "_FemaleMale", "_FemaleFemale", "_MaleFemale", "_MaleMale", "_UniMale", "_UniFemale" };

        public STBLExporter(IResourceIndexEntry entry)
            : base (entry)
        { }

        protected static bool HasSuffix(string key)
        {
            string value = key.ToLower();

            foreach (string suffix in sSuffixes)
            {
                if (value.EndsWith(suffix.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        protected static Dictionary<ulong, STBLImporter.Lookup> ReadTable(string prefix, string instance, IPackage package, bool addOtherKeys)
        {
            instance = prefix + instance.Substring(4);

            Dictionary<ulong, STBLImporter.Lookup> lookup = new Dictionary<ulong, STBLImporter.Lookup>();

            IResourceIndexEntry entry = package.Find(key => ((key.ResourceType == (uint)ResourceType.STBL) && (key["Instance"] == instance)));
            if (entry != null)
            {
                StblResource.StblResource resource = ResourceHandlers.CreateResource(entry, package) as StblResource.StblResource;

                foreach (KeyValuePair<ulong,string> element in resource)
                {
                    bool space = false;

                    string keyValue = element.Value;
                    if (prefix == "0x17")
                    {
                        if (keyValue.StartsWith("PackerSpace|"))
                        {
                            keyValue = keyValue.Replace("PackerSpace|", "");
                            space = true;
                        }
                    }

                    if (!lookup.ContainsKey(element.Key))
                    {
                        STBLImporter.Lookup recent = new STBLImporter.Lookup(keyValue, keyValue, space);
                        lookup.Add(element.Key, recent);
                    }

                    if ((addOtherKeys) && (!HasSuffix(keyValue)))
                    {
                        foreach (string suffix in sSuffixes)
                        {
                            string value = keyValue + suffix;

                            ulong key = FNV64.GetHash(value);

                            if (lookup.ContainsKey(key)) continue;

                            STBLImporter.Lookup recent = new STBLImporter.Lookup(value, value, space);
                            lookup.Add(key, recent);
                        }
                    }
                }
            }

            return lookup;
        }

        public string ConvertToString(IPackage package)
        {
            Dictionary<ulong, STBLImporter.Lookup> lookup = ReadTable("0x17", mEntry["Instance"], package, true);

            return ConvertToString(lookup, lookup, package);
        }
        public string ConvertToString(Dictionary<ulong, STBLImporter.Lookup> nameLookup, Dictionary<ulong, STBLImporter.Lookup> keyLookup, IPackage package)
        {
            Dictionary<ulong, STBLImporter.Lookup> englishLookup = ReadTable("0x00", mEntry["Instance"], package, false);

            StringBuilder builder = new StringBuilder();

            StblResource.StblResource resource = CreateResource(package) as StblResource.StblResource;

            // List of all translations from the 0x17 STBL
            foreach (KeyValuePair<ulong, STBLImporter.Lookup> element in nameLookup)
            {
                // Search the string in this resource
                string value;
                if (!resource.TryGetValue(element.Key, out value))
                {
                    STBLImporter.Lookup lookup;
                    if (englishLookup.TryGetValue(element.Key, out lookup))
                    {
                        value = lookup.mText;

                        if (HasSuffix(element.Value.mKey)) continue;

                        if (resource.ContainsKey(FNV64.GetHash(element.Value.mKey + "_UniMale"))) continue;

                        if (resource.ContainsKey(FNV64.GetHash(element.Value.mKey + "_Male"))) continue;

                        if (resource.ContainsKey(FNV64.GetHash(element.Value.mKey + "_MaleMale"))) continue;

                        MainForm.Log("  Missing Translation: " + element.Value.mKey, true);
                    }
                    else
                    {
                        //value = element.Value.mText;
                        continue;
                    }
                }

                if (!keyLookup.ContainsKey(element.Key)) continue;

                builder.Append("<KEY>" + element.Value.mKey + "</KEY>\r\n");
                builder.Append("<STR>" + value + "</STR>\r\n");

                if (element.Value.mSpace)
                {
                    builder.Append("\r\n");
                }
            }

            bool first = true;

            foreach (KeyValuePair<ulong, string> element in resource)
            {
                if (keyLookup.ContainsKey(element.Key)) continue;

                string key = NRaasPacker.ListItems.STBLListItem.GetKey(element.Key);

                MainForm.Log("  Unknown Translation: " + key + " - " + element.Value, true);

                if (first)
                {
                    builder.Append("\r\n<KEY>UNHANDLED</KEY>\r\n");
                    builder.Append("<STR>UNHANDLED</STR>\r\n\r\n");
                    first = false;
                }

                builder.Append("<KEY>" + key + "</KEY>\r\n");
                builder.Append("<STR>" + element.Value + "</STR>\r\n");
            }

            return builder.ToString();
        }

        public override bool Export(string filename, IPackage package)
        {
            try
            {
                Dictionary<ulong, STBLImporter.Lookup> nameLookup = ReadTable("0x17", mEntry["Instance"], package, true);
                Dictionary<ulong, STBLImporter.Lookup> englishLookup = ReadTable("0x00", mEntry["Instance"], package, false);

                Dictionary<ulong, STBLImporter.Lookup> lookup = new Dictionary<ulong, STBLImporter.Lookup>(nameLookup);

                Dictionary<ulong, STBLImporter.Lookup> secondaryLookup = STBLImporter.ReadFile(filename);

                foreach (KeyValuePair<ulong, STBLImporter.Lookup> element in secondaryLookup)
                {
                    if (lookup.ContainsKey(element.Key)) continue;

                    lookup.Add(element.Key, element.Value);
                }

                using (StreamWriter file = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    file.Write(ConvertToString(lookup, nameLookup, package));
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
