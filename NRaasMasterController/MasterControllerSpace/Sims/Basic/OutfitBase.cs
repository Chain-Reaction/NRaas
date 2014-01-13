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
    public abstract class OutfitBase : SimFromList, IBasicOption
    {
        protected List<Item> mOutfits = new List<Item>();

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        public override void Reset()
        {
            mOutfits.Clear();

            base.Reset();
        }

        protected static List<Item> GetOptions(SimDescription me, IEnumerable<OutfitCategories> categories)
        {
            List<Item> allOptions = new List<Item>();

            foreach (OutfitCategories category in categories)
            {
                if (me.GetOutfitCount(category) == 0)
                {
                    if ((me.IsHuman) && (category == OutfitCategories.Singed))
                    {
                        allOptions.Add(new Item(new CASParts.Key(category, 0), null));
                    }
                }
                else
                {
                    for (int i = 0; i < me.GetOutfitCount(category); i++)
                    {
                        allOptions.Add(new Item(new CASParts.Key(category, i), me));
                    }
                }
            }

            return allOptions;
        }

        public class Item : ValueSettingOption<CASParts.Key>
        {
            public Item(CASParts.Key key)
                : base(key, GetCategoryName(key.mCategory) + " " + EAText.GetNumberString(key.GetIndex()+1), -1)
            {
                if (mValue.mCategory == OutfitCategories.None)
                {
                    SetThumbnail("shop_all_r2", ProductVersion.BaseGame);
                }
            }
            public Item(CASParts.Key key, SimDescriptionCore sim)
                : this(key)
            {
                if (sim != null)
                {
                    SimOutfit outfit = CASParts.GetOutfit(sim, key, false);
                    if (outfit != null)
                    {
                        mThumbnail = new ThumbnailKey(outfit, 0x0, ThumbnailSize.Medium, ThumbnailCamera.Body);
                    }
                }
            }

            public OutfitCategories Category
            {
                get { return Value.mCategory; }
            }

            public int Index
            {
                get { return Value.GetIndex(); }
            }

            public static string GetCategoryName(OutfitCategories category)
            {
                switch (category)
                {
                    case OutfitCategories.None:
                        return "(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")";
                    case OutfitCategories.Bridle:
                        return Common.LocalizeEAString("Ui/Caption/ObjectPicker:BridleOnly");
                    default:
                        return Common.LocalizeEAString("Ui/Caption/ObjectPicker:" + category);
                }
            }
        }
    }
}
