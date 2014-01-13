using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class ProductVersions
    {
        public static string GetLocalizedName(ProductVersion value)
        {
            string entryKey = null;
            if (value != ProductVersion.BaseGame)
            {
                string str3 = value.ToString();
                if (str3.Length == 0x3)
                {
                    str3 = str3.Replace("P", "P0");
                }
                entryKey = "Engine:Caption" + str3;
            }
            else
            {
                entryKey = "Ui/Caption/BuildBuy/CatalogProductFilter:BaseGame";
            }

            return Common.LocalizeEAString(entryKey);
        }
    }
}

