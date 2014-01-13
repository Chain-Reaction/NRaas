using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace STBL
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

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0) return;

                Dictionary<string, string> elements = new Dictionary<string, string>();

                string filename = args[0];

                using (StreamReader file = new StreamReader(filename, true))
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
                        str = str.Replace("<TEXT>", "").Replace("</TEXT>", "").Replace("<text>", "").Replace("</text>", "");

                        if (elements.ContainsKey(key))
                        {
                            elements[key] = str;
                        }
                        else
                        {
                            elements.Add(key, str);
                        }
                    }
                }
                /*
        DWORD			// 'STBL'
        BYTE			// version (2)
        BYTE[2]		// null
        DWORD count
        BYTE[6]		// null
        --repetition count:
            QWORD id	// fnv64 hash of whatever text you want to localize to this
            DWORD len
            unicodeLE[len]         
                 */

                if (args.Length >= 3)
                {
                    filename = args[2];
                }
                else
                {
                    filename = filename.Substring(0, filename.LastIndexOf('.'));
                    filename += ".stbl";
                }
                //0x8FD4D7A8E1DCAC7C
                //0x87C40B8319C20384
                FileStream outFile = new FileStream(filename, FileMode.Create, FileAccess.Write);

                using (BinaryWriter file = new BinaryWriter(outFile, Encoding.Unicode))
                {
                    file.Write(new byte[] { (byte)'S', (byte)'T', (byte)'B', (byte)'L' });
                    file.Write(new byte[] { 2 });
                    file.Write(new byte[] { 0, 0 });
                    file.Write(elements.Count);
                    file.Write(new byte[] { 0, 0, 0, 0, 0, 0 });

                    foreach (KeyValuePair<string, string> element in elements)
                    {
                        ulong key = FNV(element.Key);

                        file.Write(key);
                        file.Write(element.Value.Length);

                        char[] value = element.Value.ToCharArray();

                        file.Write(value);
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
