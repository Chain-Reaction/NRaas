using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class EnumInjection
    {
        public static void InjectEnums<T>(string[] names, object[] values, bool convert)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            EnumParser parser;
            Dictionary<Type, EnumParser> dictionary_ic = ParserFunctions.sCaseInsensitiveEnumParsers;
            Dictionary<Type, EnumParser> dictionary_c = ParserFunctions.sCaseSensitiveEnumParsers;

            if (!dictionary_ic.TryGetValue(typeof(T), out parser))
            {
                parser = new EnumParser(typeof(T), true);
                dictionary_ic.Add(typeof(T), parser);
            }

            for (int i = 0; i < names.Length; i++)
            {
                string key = names[i].ToLowerInvariant();
                if (!parser.mLookup.ContainsKey(key))
                {
                    parser.mLookup.Add(key, convert ? Convert.ToUInt32(values[i]) : values[i]);
                }
            }

            if (!dictionary_c.TryGetValue(typeof(T), out parser))
            {
                parser = new EnumParser(typeof(T), true);
                dictionary_c.Add(typeof(T), parser);
            }
            for (int i = 0; i < names.Length; i++)
            {
                if (!parser.mLookup.ContainsKey(names[i]))
                {
                    parser.mLookup.Add(names[i], convert ? Convert.ToUInt32(values[i]) : values[i]);
                }
            }
        }
    }
}
