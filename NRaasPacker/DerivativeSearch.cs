using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaasPacker
{
    public class DerivativeSearch
    {
        protected static Dictionary<Type, List<object>> sItems = new Dictionary<Type, List<object>>();

        public static List<T> Find<T>(bool useCache)
            where T : class
        {
            List<T> list = new List<T>();

            List<object> existing = null;
            if ((useCache) && (sItems.TryGetValue(typeof(T), out existing)))
            {
                foreach (T item in existing)
                {
                    list.Add(item);
                }
            }
            else
            {
                if (useCache)
                {
                    existing = new List<object>();
                    sItems.Add(typeof(T), existing);
                }

                CheckAssembly(typeof(DerivativeSearch).Assembly, list, existing);

                string folder = Path.GetDirectoryName(typeof(DerivativeSearch).Assembly.Location);
                foreach (string path in Directory.GetFiles(folder, "*.dll"))
                {
                    CheckAssembly(Assembly.LoadFile(path), list, existing);
                };
            }

            return list;
        }

        protected static void CheckAssembly<T>(Assembly assembly, List<T> list, List<object> existing)
            where T : class
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract) continue;

                if (typeof(T).IsAssignableFrom(type))
                {
                    System.Reflection.ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                    if (constructor != null)
                    {
                        T item = constructor.Invoke(new object[0]) as T;
                        if (item != null)
                        {
                            list.Add(item);

                            if (existing != null)
                            {
                                existing.Add(item);
                            }
                        }
                    }
                }
            }
        }        
    }
}
