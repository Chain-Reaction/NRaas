using NRaasPacker.ListItems;
using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NRaasPacker
{
    public abstract class ResourceHandlers
    {
        static Dictionary<string, Type> sHandlers = null;

        protected static Dictionary<string, Type> Handlers
        {
            get
            {
                if (sHandlers == null)
                {
                    sHandlers = new Dictionary<string, Type>();

                    List<AResourceHandler> handlers = DerivativeSearch.Find<AResourceHandler>(true);

                    foreach (AResourceHandler handler in handlers)
                    {
                        foreach (Type k in handler.Keys)
                        {
                            foreach (string s in handler[k])
                            {
                                sHandlers.Add(s, k);
                            }
                        }
                    }
                }

                return sHandlers;
            }
        }

        public static Type GetHandler(string tag)
        {
            if (tag == null) return null;

            Type t = null;
            if (Handlers.TryGetValue(tag, out t))
            {
                return t;
            }

            return null;
        }

        public static IResource CreateResource(IResourceIndexEntry entry, IPackage package)
        {
            Type t = GetHandler(entry["ResourceType"]);
            if (t == null) return null;

            APackage localPackage = package as APackage;

            try
            {
                return t.GetConstructor(new Type[] { typeof(int), typeof(Stream), }).Invoke(new object[] { 0, localPackage.GetResource(entry) }) as IResource;
            }
            catch
            { }

            return null;
        }
    }
}
