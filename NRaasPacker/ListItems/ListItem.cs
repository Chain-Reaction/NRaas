using NRaasPacker.Exporters;
using NRaasPacker.Importers;
using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NRaasPacker.ListItems
{
    public abstract class ListItem : ListViewItem
    {
        static Dictionary<ResourceType, ListItem> sHandlers = null;

        protected IResourceIndexEntry mEntry;

        protected IPackage mPackage;

        ListViewSubItem mInstance = null;
        ListViewSubItem mFilename = null;

        ResourceType mType;

        public ListItem(ResourceType type)
        {
            mType = type;
        }

        public IResourceIndexEntry Entry
        {
            get { return mEntry; }
        }

        public bool Delete(IPackage package)
        {
            package.DeleteResource(mEntry);

            Remove();
            return true;
        }

        protected virtual void Set(IResourceIndexEntry entry, IPackage package)
        {
            mEntry = entry;
            mPackage = package;

            mType = (ResourceType)mEntry.ResourceType;

            if (mEntry != null)
            {
                // NameColumn
                NameMapResource.NameMapResource nameMap = MainForm.CreateKeyResource(package) as NameMapResource.NameMapResource;
                if ((nameMap != null) && (nameMap.ContainsKey(mEntry.Instance)))
                {
                    Text = nameMap[mEntry.Instance];
                }

                // TagColumn
                SubItems.Add(Type.ToString());

                // Instance
                mInstance = SubItems.Add(Instance);

                // Filename
                mFilename = SubItems.Add("");
            }
        }

        public abstract void GetHandledTypes(List<ResourceType> types);

        public ResourceType Type
        {
            get 
            {
                if (mEntry != null)
                {
                    return (ResourceType)mEntry.ResourceType;
                }
                else
                {
                    return mType;
                }
            }
            set
            {
                if (mEntry != null) return;

                mType = value;
            }
        }

        protected static Dictionary<ResourceType, ListItem> Handlers
        {
            get
            {
                if (sHandlers == null)
                {
                    sHandlers = new Dictionary<ResourceType, ListItem>();

                    List<ListItem> items = DerivativeSearch.Find<ListItem>(true);

                    foreach (ListItem item in items)
                    {
                        List<ResourceType> types = new List<ResourceType>();
                        item.GetHandledTypes(types);

                        foreach (ResourceType type in types)
                        {
                            sHandlers.Add(type, item);
                        }
                    }
                }

                return sHandlers;
            }
        }

        public static ListItem CreateHandler(IResourceIndexEntry entry, IPackage package)
        {
            ResourceType type = (ResourceType)entry.ResourceType;

            ListItem item;
            if (Handlers.TryGetValue(type, out item))
            {
                item = item.Clone() as ListItem;
                item.Set(entry, package);
                return item;
            }

            return null;
        }

        public bool IsImportable
        {
            get
            {
                return (ResourceHandlers.GetHandler("0x" + ((uint)Type).ToString("X16")) != null); 
            }
        }

        public string Instance
        {
            get
            {
                if (mEntry == null) return null;

                return mEntry["Instance"];
            }
            set
            {
                mEntry.Instance = Convert.ToUInt64(value, value.StartsWith("0x") ? 16 : 10);

                mInstance.Text = Instance;
            }
        }

        public abstract Importer GetImporter();

        public abstract Exporter GetExporter();

        public string Filename
        {
            set
            {
                Importer importer = GetImporter();
                if (importer == null) return;

                mFilename.Text = value;
                if (value == null) return;

                importer.Import(value, mPackage);

                // Required to ensure that the resource is saved (sets the dirty flag)
                mEntry.Compressed = 0xffff;
            }
        }

        public bool Changed
        {
            get
            {
                return !string.IsNullOrEmpty(mFilename.Text);
            }
        }

        public new string Name
        {
            set
            {
                IResourceIndexEntry keyEntry = mPackage.Find(key => key.ResourceType == (uint)ResourceType._KEY);
                if (keyEntry == null)
                {
                    keyEntry = mPackage.AddResource(new AResource.TGIBlock(0, null, (uint)ResourceType._KEY, 0, 0), null, false);
                }

                IResource keyResource = MainForm.CreateKeyResource(mPackage);

                IDictionary<ulong, string> nameMap = keyResource as IDictionary<ulong, string>;

                if (nameMap.ContainsKey(mEntry.Instance))
                {
                    nameMap[mEntry.Instance] = value;
                }
                else
                {
                    nameMap.Add(mEntry.Instance, value);
                }

                mPackage.ReplaceResource(keyEntry, keyResource);

                Text = value;
            }
        }

        protected string PrivateGetFilename(string defaultExt, string filter, List<string> prefixes, string defaultName, bool autoSet, bool fileMustExist)
        {
            if (prefixes.Count == 0)
            {
                prefixes.Add(null);
            }

            OpenFileDialog dialog = new OpenFileDialog();

            dialog.DefaultExt = defaultExt;
            dialog.Filter = filter;
            dialog.FilterIndex = 1;
            dialog.InitialDirectory = PathStore.GetPath(MainForm.PackageName, dialog.DefaultExt);
            dialog.CheckFileExists = fileMustExist;
            dialog.CheckPathExists = true;

            string oldPath = dialog.InitialDirectory;

            string filename = null;

            foreach (string prefix in prefixes)
            {
                string name = defaultName;

                string path = oldPath;
                if (!string.IsNullOrEmpty(prefix))
                {
                    name = name.Replace(prefixes[0], prefix);

                    path += "\\" + prefix;
                }

                if (!Directory.Exists(path))
                {
                    path = oldPath;
                }
                else if (prefix == prefixes[0])
                {
                    dialog.InitialDirectory = path;
                }

                filename = path + "\\" + name;
                if (File.Exists(filename)) break;

                filename = null;
            }

            if ((filename == null) || (!autoSet))
            {
                dialog.FileName = defaultName;

                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return null;

                filename = dialog.FileName;

                string directory = Path.GetDirectoryName(filename);

                foreach(string prefix in prefixes)
                {
                    if (prefix == null) continue;

                    if (directory.Contains("\\" + prefix))
                    {
                        directory = directory.Replace("\\" + prefix, "");
                        break;
                    }
                }

                PathStore.SetPath(MainForm.PackageName, dialog.DefaultExt, directory);
            }

            return filename;
        }

        public abstract string GetFilename(bool autoSet, bool fileMustExist);

        public bool Import(bool autoSet)
        {
            string filename = GetFilename(autoSet, true);

            Filename = filename;

            return (filename != null);
        }

        public bool Export(bool autoSet)
        {
            return Export(autoSet, true);
        }
        public bool Export(bool autoSet, bool promptToOverwrite)
        {
            Exporter export = GetExporter();
            if (export == null) return false;

            string filename = GetFilename(autoSet, false);
            if (string.IsNullOrEmpty(filename)) return false;

            if ((File.Exists(filename)) && (promptToOverwrite))
            {
                if (MessageBox.Show("Do you wish to overwrite '" + Path.GetFileName(filename) + "' ?", "File Exists", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return false;
                }
            }

            return export.Export(filename, mPackage);
        }

        protected abstract string CreateInstance(string filename, ref ulong instance);

        public bool AddNew(ulong instance, IPackage package, bool autoSet)
        {
            mPackage = package;

            string filename = GetFilename(autoSet, true);
            if (string.IsNullOrEmpty(filename)) return false;

            string name = CreateInstance(filename, ref instance);
            if (instance == 0) return false;

            Set (mPackage.AddResource(new AResource.TGIBlock(0, null, (uint)Type, 0, instance), null, false), mPackage);
            if (mEntry == null) return false;

            if (File.Exists(filename))
            {
                Filename = filename;
            }
            else
            {
                mPackage.ReplaceResource(mEntry, ResourceHandlers.CreateResource(mEntry, mPackage));
            }

            Name = name;

            return true;
        }
    }
}
