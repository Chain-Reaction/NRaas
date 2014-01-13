using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BootStrapCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FileStream inFile = new FileStream("NRaasBootStrap.dll", FileMode.Open, FileAccess.Read);

                byte[] oldStream = null;

                using (BinaryReader file = new BinaryReader(inFile))
                {
                    oldStream = new byte[inFile.Length];
                    file.Read(oldStream, 0, (int)inFile.Length);
                }

                inFile.Close();

                List<byte> newStream = new List<byte>();

                byte[] bootStrap = new byte[] { (byte)'N', (byte)'R', (byte)'a', (byte)'a', (byte)'s', (byte)'B', (byte)'o', (byte)'o', (byte)'t', (byte)'S', (byte)'t', (byte)'r', (byte)'a', (byte)'p' };
                int bootIndex = 0;

                List<byte> store = new List<byte>();

                Random rand = new Random();

                byte[] replacement = new byte[bootStrap.Length];

                string filename = null;

                for (int i = 0; i < replacement.Length; i++)
                {
                    char c = (char)('0' + rand.Next(10));

                    replacement[i] = (byte)c;

                    filename += c;
                }

                filename += ".dll";

                int index = 0;
                while (index < oldStream.Length)
                {
                    byte b = oldStream[index];
                    index++;

                    if (b == bootStrap[bootIndex])
                    {
                        store.Add(b);

                        bootIndex++;
                        if (bootIndex >= bootStrap.Length)
                        {
                            foreach (char c in replacement)
                            {
                                newStream.Add((byte)c);
                            }

                            store.Clear();
                            bootIndex = 0;
                        }
                    }
                    else
                    {
                        foreach (byte s in store)
                        {
                            newStream.Add(s);
                        }

                        store.Clear();
                        bootIndex = 0;

                        newStream.Add(b);
                    }
                }

                FileStream outFile = new FileStream(filename, FileMode.Create, FileAccess.Write);

                using (BinaryWriter file = new BinaryWriter(outFile, Encoding.Unicode))
                {
                    file.Write(newStream.ToArray());
                }

                outFile.Close();
            }
            catch (Exception e)
            {
                using (StreamWriter error = new StreamWriter ("bootstrapcreator.log"))
                {
                    error.Write(e.ToString());
                }
            }
        }
    }
}
