﻿using System.Collections.Generic;

namespace NRaas.CommonSpace.Converters
{
    public class ListToString<T>
    {
        protected virtual string PrivateConvert(T value)
        {
            return value.ToString();
        }

        public delegate string OnConvert(T value);

        public string Convert(IEnumerable<T> list)
        {
            return StaticConvert(list, PrivateConvert);
        }

        public static string StaticConvert(IEnumerable<T> list, OnConvert convert)
        {
            if (list == null) return ""; // Must return a non-null

            string results = ""; // Must return a non-null

            foreach (T key in list)
            {
                if (!string.IsNullOrEmpty(results))
                {
                    results += ",";
                }

                results += convert(key);
            }

            return results;
        }
    }
}
