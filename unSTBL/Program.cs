using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace unSTBL
{
    class Program
    {
        static ulong FNV(string data)
        {
            data = data.ToLower();

            ulong hash = 0xCBF29CE484222325;
            foreach (byte b in data)
            {
                hash *= 0x00000100000001B3;
                hash ^= b;
            }
            return hash;
        }

        protected class Lookup
        {
            public ulong mID;

            public string mKey;
            public string mText;

            public Lookup(string key)
            {
                mKey = key;
                mID = FNV(mKey);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0) return;

                Dictionary<ulong, Lookup> lookup = new Dictionary<ulong, Lookup>();

                List<Lookup> elements = new List<Lookup>();

                string textFile = args[0];
                textFile = textFile.Substring(0, textFile.LastIndexOf('.'));
                textFile += ".txt";

                using (StreamReader file = new StreamReader(textFile, true))
                {
                    string line = file.ReadToEnd();

                    int previousStart = line.IndexOf("<KEY>");

                    bool bEnd = false;
                    while (!bEnd)
                    {
                        string element = null;

                        int keyEnd = line.IndexOf("<KEY>", previousStart + 1);
                        if (keyEnd == -1)
                        {
                            element = line.Substring(previousStart);
                            bEnd = true;
                        }
                        else
                        {
                            element = line.Substring(previousStart, keyEnd - previousStart);
                        }

                        previousStart = keyEnd;

                        element = element.Replace("\r", "").Replace("\n", "");

                        int strStart = element.IndexOf("<STR>");
                        if (strStart == -1) continue;

                        string key = element.Substring(0, strStart);
                        string str = element.Substring(strStart);

                        key = key.Replace("<KEY>", "").Replace("</KEY>", "");
                        str = str.Replace("<STR>", "").Replace("</STR>", "");

                        Lookup value = new Lookup(key);

                        if (!lookup.ContainsKey(value.mID))
                        {
                            lookup.Add(value.mID, value);

                            elements.Add(value);
                        }
                    }
                }

                string filename = args[0];

                /*
        DWORD		// 'STBL'
        BYTE		// version (2)
        BYTE[2]		// null
        DWORD count
        BYTE[6]		// null
        --repetition count:
            QWORD id	// fnv64 hash of whatever text you want to localize to this
            DWORD len
            unicodeLE[len]         
                 */

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
                        if (lookup.TryGetValue(id, out value))
                        {
                            value.mText = new string (text);
                        }
                    }
                }

                inFile = null;

                textFile = args[0];
                textFile = textFile.Substring(0, textFile.LastIndexOf('.'));
                textFile += "New.txt";

                using (StreamWriter file = new StreamWriter(textFile, false, Encoding.Unicode))
                {
                    foreach (Lookup value in elements)
                    {
                        file.Write("<KEY>" + value.mKey + "</KEY>\n");
                        file.Write("<STR>" + value.mText + "</STR>\n");
                    }
                }
            }
            catch (Exception e)
            {
                string filename = args[0];
                if ((filename == null) || (filename == ""))
                {
                    filename = "stbl.log";
                }
                else
                {
                    filename = filename.Substring(0, filename.LastIndexOf('.'));
                    filename += ".log";
                }

                using (StreamWriter error = new StreamWriter (filename))
                {
                    error.Write(e.ToString());
                }
            }
        }
    }
}
