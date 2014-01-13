using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public class Baby
    {
        public static CASAgeGenderFlags SelectGender()
        {
            List<Item> options = new List<Item>();
            options.Add(new Item(MasterController.Localize("BabyGender:Male"), CASAgeGenderFlags.Male));
            options.Add(new Item(MasterController.Localize("BabyGender:Female"), CASAgeGenderFlags.Female));
            options.Add(new Item(MasterController.Localize("Criteria.Random:MenuName"), CASAgeGenderFlags.GenderMask));

            Item selection = new CommonSelection<Item>(MasterController.Localize("BabyGender:MenuName"), options).SelectSingle();
            if (selection == null) return CASAgeGenderFlags.None;

            return selection.Value;
        }

        public static CASAgeGenderFlags InterpretGender(CASAgeGenderFlags gender)
        {
            if (gender == CASAgeGenderFlags.GenderMask)
            {
                if (RandomUtil.CoinFlip())
                {
                    return CASAgeGenderFlags.Male;
                }
                else
                {
                    return CASAgeGenderFlags.Female;
                }
            }
            else
            {
                return gender;
            }
        }

        public class Item : ValueSettingOption<CASAgeGenderFlags>
        {
            public Item(string title, CASAgeGenderFlags gender)
                : base(gender, title, 0)
            { }
        }
    }
}
