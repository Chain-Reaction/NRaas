using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.PorterSpace.Selection
{
    public class HouseholdItem : ValueSettingOption<Household>
    {
        public HouseholdItem(Household target, bool selected)
            : base(target, GetQualifiedName(target), Households.NumSims(target))
        {
            if (selected)
            {
                mName = "(" + mName + ")";
            }

            if (target.LotHome != null)
            {
                mThumbnail = target.LotHome.GetThumbnailKey();
            }
        }

        public static string GetQualifiedName(Household house)
        {
            string name = house.Name;
            if (house.IsServiceNpcHousehold)
            {
                return Common.Localize("Household:Service");
            }
            else
            {
                SimDescription sim = SimTypes.HeadOfFamily(house);
                if (sim != null)
                {
                    name += " - " + sim.FirstName;
                    if (sim.LastName != house.Name)
                    {
                        name += " " + sim.LastName;
                    }
                }
            }
            return name;
        }
    }
}
