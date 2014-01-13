using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRaasPacker
{
    public class PathStore
    {
        static Dictionary<string, string> sPaths = null;

        public PathStore()
        { }

        private static Dictionary<string, string> Paths
        {
            get
            {
                if (sPaths == null)
                {
                    sPaths = new Dictionary<string, string>();

                    if (NRaasPacker.Properties.Settings.Default.PathStore != null)
                    {
                        foreach (string value in NRaasPacker.Properties.Settings.Default.PathStore)
                        {
                            string[] values = value.Split('|');

                            sPaths.Add(values[0], values[1]);
                        }
                    }
                }

                return sPaths;
            }
        }

        public static string GetPath(string packageName, string extension)
        {
            string path;
            if (Paths.TryGetValue(packageName + extension, out path))
            {
                if (path.EndsWith("\\"))
                {
                    path = path.Substring(0, path.Length - 1);
                }

                return path;
            }
            else
            {
                return null;
            }
        }

        public static void SetPath(string packageName, string extension, string newPath)
        {
            string oldPath = GetPath(packageName, extension);

            Paths.Remove(packageName + extension);

            if (newPath != null)
            {
                Paths.Add(packageName + extension, newPath);
            }

            if (NRaasPacker.Properties.Settings.Default.PathStore == null)
            {
                NRaasPacker.Properties.Settings.Default.PathStore = new System.Collections.Specialized.StringCollection();
            }

            if (oldPath != null)
            {
                NRaasPacker.Properties.Settings.Default.PathStore.Remove(packageName + extension + "|" + oldPath);
            }

            if (newPath != null)
            {
                NRaasPacker.Properties.Settings.Default.PathStore.Add(packageName + extension + "|" + newPath);
            }
        }

        public static void Reset(string packageName)
        {
            if (NRaasPacker.Properties.Settings.Default.PathStore != null)
            {
                SetPath(packageName, "stbl", null);
                SetPath(packageName, "dll", null);
                SetPath(packageName, "xml", null);
            }
        }
    }
}
