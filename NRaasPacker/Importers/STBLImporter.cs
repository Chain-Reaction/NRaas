using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NRaasPacker.Importers
{
    public class STBLImporter : Importer
    {
        public class Lookup
        {
            public ulong mID;

            public string mKey;
            public string mText;

            public bool mSpace;

            public Lookup(string key, string text, bool space)
            {
                Set(key, text, space);
            }

            public void Set(string key, string text, bool space)
            {
                mKey = key;
                mID = NRaasPacker.ListItems.STBLListItem.AddKey(mKey);
                mText = text;
                mSpace = space;
            }
        }

        public STBLImporter(IResourceIndexEntry entry)
            : base (entry)
        { }

        public static Dictionary<ulong,Lookup> ReadFile(string filename)
        {
            MainForm.Log("Import: " + Path.GetFileNameWithoutExtension(filename), false);

            if (filename.ToUpper().EndsWith(".STBL"))
            {
                return ReadBinaryFile(filename);
            }
            else
            {
                return ReadTextFile(filename);
            }
        }

        public static Dictionary<ulong,Lookup> ReadBinaryFile(string filename)
        {
            Dictionary<ulong, Lookup> lookup = new Dictionary<ulong, Lookup>();

            if (File.Exists(filename))
            {
                FileStream inFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
                using (BinaryReader file = new BinaryReader(inFile, Encoding.Unicode))
                {
                    byte[] buffer = new byte[7];
                    file.Read(buffer, 0, 7);

                    uint count = file.ReadUInt32();

                    file.Read(buffer, 0, 6);

                    for (uint i = 0; i < count; i++)
                    {
                        ulong id = file.ReadUInt64();

                        uint len = file.ReadUInt32();

                        char[] text = file.ReadChars((int)len);

                        Lookup value;
                        if (!lookup.TryGetValue(id, out value))
                        {
                            value = new Lookup("0x" + id.ToString("X16"), new string(text), false);
                            lookup.Add(id, value);
                        }
                        else
                        {
                            value.mText = new string (text);
                        }
                    }
                }
            }

            return lookup;
        }

        public static Dictionary<ulong, Lookup> ConvertFromText(string line)
        {
            if (line.StartsWith("Unknown1:"))
            {
                return ConvertFromS3PE(line);
            }
            else
            {
                return ConvertFromKeyStr(line);
            }
        }

        protected static Dictionary<ulong, Lookup> ConvertFromS3PE(string line)
        {
            Dictionary<ulong, Lookup> lookup = new Dictionary<ulong, Lookup>();

            int previousStart = 0;

            while (true)
            {
                int end = line.IndexOf('\n', previousStart);
                if (end < 0) break;

                string element = line.Substring(previousStart, end - previousStart);

                previousStart = end+1;

                if (element.StartsWith("Unknown")) continue;

                if (element.StartsWith("-")) continue;

                element = element.Substring(element.IndexOf(']')+1).Trim();

                int strStart = element.IndexOf(':');

                string key = element.Substring(0, strStart);
                string str = element.Substring(strStart+1).Trim();

                ulong ID = 0;
                if ((key.StartsWith("0x")) || (key.StartsWith("0X")))
                {
                    ID = Convert.ToUInt64(key, 16);
                }
                else
                {
                    ID = NRaasPacker.ListItems.STBLListItem.AddKey(key);
                }

                if (lookup.ContainsKey(ID))
                {
                    MainForm.Log("  Duplicate Key: " + key, true);

                    lookup[ID].Set(key, str, false);
                }
                else
                {
                    lookup.Add(ID, new Lookup(key, str, false));
                }
            }

            return lookup;
        }

        protected static Dictionary<ulong, Lookup> ConvertFromKeyStr(string line)
        {
            Dictionary<ulong, Lookup> lookup = new Dictionary<ulong, Lookup>();

            int previousStart = line.IndexOf("<KEY>");

            bool bEnd = false;
            while (!bEnd)
            {
                string element = null;

                int keyEnd = line.IndexOf("<KEY>", previousStart + 1);
                if (keyEnd == -1)
                {
                    bEnd = true;

                    if (previousStart < 0) break;

                    element = line.Substring(previousStart);
                }
                else
                {
                    element = line.Substring(previousStart, keyEnd - previousStart);
                }

                previousStart = keyEnd;

                int strStart = element.IndexOf("<STR>");
                if (strStart == -1)
                {
                    MainForm.Log("  No <STR>: " + element, true);
                    continue;
                }

                string key = element.Substring(0, strStart);
                string str = element.Substring(strStart);

                if (str.LastIndexOf("<STR>") != str.IndexOf("<STR>"))
                {
                    MainForm.Log("  Multiple <STR>: " + str, true);
                }

                key = key.Replace("<KEY>", "").Replace("</KEY>", "");
                key = key.Replace("\r", "").Replace("\n", "");
                key = key.Replace("PackerSpace|", "");

                bool space = false;
                if ((str.Contains("</STR>\r\n\r\n")) || (str.Contains("</STR>\n\n")))
                {
                    space = true;
                }

                str = str.Replace("<STR>", "").Replace("</STR>", "");
                str = str.Replace("<TEXT>", "").Replace("</TEXT>", "").Replace("<text>", "").Replace("</text>", "");

                str = str.Replace("\r", "").Replace("\n", "");

                if (key.Contains("</STR>"))
                {
                    MainForm.Log("  Misplaced </STR>: " + key + " : " + str, true);
                }
                else if (key.Contains("<STR>"))
                {
                    MainForm.Log("  Misplaced <STR>: " + key + " : " + str, true);
                }

                if (str.Contains("</KEY>"))
                {
                    MainForm.Log("  Misplaced </KEY>: " + key + " : " + str, true);
                }
                else if (str.Contains("<KEY>"))
                {
                    MainForm.Log("  Misplaced <KEY>: " + key + " : " + str, true);
                }

                ulong ID = 0;
                if ((key.StartsWith("0x")) || (key.StartsWith("0X")))
                {
                    ID = Convert.ToUInt64(key, 16);
                }
                else
                {
                    ID = NRaasPacker.ListItems.STBLListItem.AddKey(key);
                }

                if (lookup.ContainsKey(ID))
                {
                    MainForm.Log("  Duplicate Key: " + key, true);

                    lookup[ID].Set(key, str, space);
                }
                else
                {
                    lookup.Add(ID, new Lookup(key, str, space));
                }
            }

            return lookup;
        }

        public static Dictionary<ulong,Lookup> ReadTextFile(string filename)
        {
            if (File.Exists(filename))
            {
                using (StreamReader file = new StreamReader(filename, true))
                {
                    return ConvertFromText(file.ReadToEnd());
                }
            }

            return new Dictionary<ulong, Lookup>();
        }

        public override bool Import(string filename, IPackage package)
        {
            return Import(ReadFile(filename), package);
        }

        public override bool Import(Exporters.Exporter exporter, IPackage package)
        {
            Exporters.STBLExporter export = exporter as Exporters.STBLExporter;
            if (export == null) return false;

            return ImportFromString(export.ConvertToString(package), package);
        }

        public bool ImportFromString(string value, IPackage package)
        {
            return Import(ConvertFromText(value), package);
        }

        public bool Import(Dictionary<ulong, Lookup> elements, IPackage package)
        {
            StblResource.StblResource resource = CreateResource(package) as StblResource.StblResource;

            resource.Clear();

            string instance = mEntry["Instance"];

            bool keyLoad = false;
            if (instance.StartsWith("0x17"))
            {
                keyLoad = true;
            }

            foreach (KeyValuePair<ulong, Lookup> element in elements)
            {
                if (keyLoad)
                {
                    string value = element.Value.mKey;

                    if (element.Value.mSpace)
                    {
                        value = "PackerSpace|" + value;
                    }

                    resource.Add(element.Key, value);
                }
                else
                {
                    resource.Add(element.Key, element.Value.mText);
                }
            }

            package.ReplaceResource(mEntry, resource);
            return true;
        }
    }
}
