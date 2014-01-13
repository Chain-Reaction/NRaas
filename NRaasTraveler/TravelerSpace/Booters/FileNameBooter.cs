using NRaas.CommonSpace.Booters;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.TravelerSpace.Booters
{
    public class FileNameBooter : BooterHelper.ByRowBooter
    {
        static Dictionary<string, string> sLoadedNames = new Dictionary<string, string>();

        static FileNameBooter()
        {
            BooterHelper.Add(new FileNameBooter());
        }

        public FileNameBooter()
            : base("WorldToFile", VersionStamp.sNamespace + ".FileNameList")
        { }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            string worldName = row.GetString("WorldName");
            if (string.IsNullOrEmpty(worldName)) return;

            worldName = worldName.ToLower().Replace(".world", "").Replace(".World", "").Replace(".WORLD","");

            if (sLoadedNames.ContainsKey(worldName)) return;

            string fileName = row.GetString("FileName");
            if (string.IsNullOrEmpty(fileName)) return;

            fileName = fileName.Replace(".nhd", "").Replace(".Nhd", "").Replace(".NHD", "");

            sLoadedNames.Add(worldName, fileName);
        }

        public static string GetFileName(string worldName)
        {
            string fileName = null;
            if (sLoadedNames.TryGetValue(worldName.ToLower(), out fileName))
            {
                return fileName;
            }
            else
            {
                return worldName + "_0x00000000";
            }
        }

        public static string LookupToString()
        {
            Common.StringBuilder msg = new Common.StringBuilder(Common.NewLine + "FileNameBooter");

            foreach (KeyValuePair<string, string> data in sLoadedNames)
            {
                msg += Common.NewLine + "  " + data.Key + " : " + data.Value;
            }

            return msg.ToString();
        }
    }
}

