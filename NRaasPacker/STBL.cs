using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRaasPacker
{
    public class STBL
    {
        static Dictionary<string, string> sProperNames = new Dictionary<string, string>();

        static STBL()
        {
            sProperNames.Add("00","English");
            sProperNames.Add("01","Chinese");
            sProperNames.Add("02","Taiwanese");
            sProperNames.Add("03","Czech");
            sProperNames.Add("04","Danish");
            sProperNames.Add("05","Dutch");
            sProperNames.Add("06","Finnish");
            sProperNames.Add("07","French");
            sProperNames.Add("08","German");
            sProperNames.Add("09","Greek");
            sProperNames.Add("0A","Hungarian");
            sProperNames.Add("0B","Italian");
            sProperNames.Add("0C","Japanese");
            sProperNames.Add("0D","Korean");
            sProperNames.Add("0E","Norwegian");
            sProperNames.Add("0F","Polish");
            sProperNames.Add("10", "PortugueseStandard");
            sProperNames.Add("11", "PortugueseBrazilian");
            sProperNames.Add("12","Russian");
            sProperNames.Add("13", "SpanishStandard");
            sProperNames.Add("14", "SpanishMexican");
            sProperNames.Add("15","Swedish");
            sProperNames.Add("16", "Thai");
            sProperNames.Add("17", "UnhashedKeys");
        }

        public STBL()
        { }

        public static Dictionary<string, string>.KeyCollection Keys
        {
            get
            {
                return sProperNames.Keys;
            }
        }

        public static int Count
        {
            get { return sProperNames.Count; }
        }

        public static string GetPrefix(string instance)
        {
            if (instance == null) return null;

            return instance.Substring(2, 2);
        }

        public static string GetProperName(string instance)
        {
            string prefix = GetPrefix(instance);
            if (prefix == null) return null;

            if (!sProperNames.ContainsKey(prefix)) return null;

            return sProperNames[prefix];
        }
    }
}
