using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class RemoveOutfit : OutfitBase, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveOutfit";
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<OutfitCategories> categories = new List<OutfitCategories>();
                foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                {
                    if (category == OutfitCategories.Special) continue;

                    categories.Add(category);
                }

                List<Item> allOptions = GetOptions(me, categories);

                CommonSelection<Item>.Results choices = new CommonSelection<Item>(Name, me.FullName, allOptions).SelectMultiple();
                if ((choices == null) || (choices.Count == 0)) return false;

                mOutfits.Clear();
                mOutfits.AddRange(choices);
            }

            CASParts.Key currentKey = new CASParts.Key(me.CreatedSim);

            // Remove them in reverse to ensure that removing earlier indices doesn't alter the index of later ones
            for(int i=mOutfits.Count-1; i>=0; i--)
            {
                Item item = mOutfits[i];

                if (item.Value == currentKey) continue;

                me.RemoveOutfit(item.Category, item.Index, true);
            }

            return true;
        }
    }
}
