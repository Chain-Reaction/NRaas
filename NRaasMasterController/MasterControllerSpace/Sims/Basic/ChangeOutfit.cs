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
    public class ChangeOutfit : OutfitBase, IBasicOption
    {
        static OutfitCategories[] sCategories = new OutfitCategories[] { OutfitCategories.Everyday, OutfitCategories.Formalwear, OutfitCategories.Sleepwear, OutfitCategories.Swimwear, OutfitCategories.Athletic, OutfitCategories.MartialArts, OutfitCategories.Career, OutfitCategories.Naked, OutfitCategories.Singed, OutfitCategories.Special, OutfitCategories.Jumping, OutfitCategories.Racing, OutfitCategories.Bridle, OutfitCategories.Outerwear };

        public override string GetTitlePrefix()
        {
            return "ChangeOutfit";
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            try
            {
                if (me.CreatedSim.CurrentOutfitCategory == OutfitCategories.None) return false;
            }
            catch
            {}

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<OutfitCategories> categories = new List<OutfitCategories>();
                if (!Common.kDebugging)
                {
                    categories.AddRange(sCategories);
                }
                else
                {
                    foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                    {
                        categories.Add(category);
                    }
                }

                List<Item> allOptions = GetOptions(me, categories);

                Item choice = new CommonSelection<Item>(Name, me.FullName, allOptions).SelectSingle();
                if (choice == null) return false;

                mOutfits.Clear();
                mOutfits.Add(choice);
            }

            if (mOutfits.Count == 0) return false;

            Item item = mOutfits[0];

            if (item.Category == OutfitCategories.Singed)
            {
                BuffSinged.SetupSingedOutfit(me.CreatedSim);
            }

            int index = item.Index;
            if ((me.IsUsingMaternityOutfits) && (me.mMaternityOutfits != null))
            {
                ArrayList list = me.mMaternityOutfits[item.Category] as ArrayList;

                if ((list != null) && (list.Count > index))
                {
                    object a = list[0];
                    object b = list[index];

                    list[0] = b;
                    list[index] = a;

                    index = 0;
                }
            }

            if (item.Category == OutfitCategories.Career)
            {
                me.CareerOutfitIndex = index;
            }

            SwitchOutfits.SwitchNoSpin(me.CreatedSim, new CASParts.Key(item.Category, index));

            if (me.HorseManager != null)
            {
                switch (item.Category)
                {
                    case OutfitCategories.Naked:
                    case OutfitCategories.Bridle:
                        me.HorseManager.PostUnsaddleHorseAction();
                        break;
                    default:
                        me.HorseManager.PostSaddleHorseAction();
                        break;
                }
            }

            return true;
        }
    }
}
