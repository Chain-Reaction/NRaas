using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.DresserSpace.Options.Sims
{
    public abstract class OutfitBase : OperationSettingOption<Sim>
    {
        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!parameters.mTarget.IsHuman) return false;

            return base.Allow(parameters);
        }

        protected abstract IEnumerable<OutfitCategories> GetCategories(SimDescription sim);

        protected abstract bool Allow(SimDescription sim, ref CASParts.Key outfitKey, ref bool alternate, ref CASParts.Key displayKey);

        protected abstract void Perform(SimDescription sim, CASParts.Key outfitKey, CASParts.Key displayKey);

        protected virtual int GetOutfitCount(SimDescription sim, OutfitCategories category)
        {
            return sim.GetOutfitCount(category);
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            List<Item> allOptions = new List<Item>();

            foreach (OutfitCategories category in GetCategories(parameters.mTarget.SimDescription))
            {
                int count = GetOutfitCount(parameters.mTarget.SimDescription, category);
                for (int i = 0; i < count; i++)
                {
                    CASParts.Key outfitKey = new CASParts.Key(category, i);
                    CASParts.Key displayKey = new CASParts.Key(category, i);
                    bool alternate = false;
                    if (!Allow(parameters.mTarget.SimDescription, ref outfitKey, ref alternate, ref displayKey)) continue;

                    allOptions.Add(new Item(outfitKey, alternate, parameters.mTarget.SimDescription, displayKey));
                }
            }

            if (allOptions.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":NoChoices", parameters.mTarget.IsFemale, new object[] { parameters.mTarget }));
                return OptionResult.Failure;
            }

            CommonSelection<Item>.Results choices = new CommonSelection<Item>(Name, parameters.mTarget.FullName, allOptions).SelectMultiple();
            if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

            List<Item> selection = new List<Item>(choices);

            // Remove them in reverse to ensure that removing earlier indices doesn't alter the index of later ones
            for (int i = selection.Count - 1; i >= 0; i--)
            {
                Perform(parameters.mTarget.SimDescription, selection[i].Value, selection[i].mDisplayKey);
            }

            return OptionResult.SuccessClose;
        }

        public class Item : ValueSettingOption<CASParts.Key>
        {
            public readonly CASParts.Key mDisplayKey;

            public Item(CASParts.Key outfitKey, bool alternate, SimDescription sim, CASParts.Key displayKey)
                : base(outfitKey, displayKey.GetLocalizedName(sim, false), 0)
            {
                mDisplayKey = displayKey;

                if (mValue.mCategory == OutfitCategories.None)
                {
                    SetThumbnail("shop_all_r2", ProductVersion.BaseGame);
                }
                else
                {
                    SimOutfit outfit = CASParts.GetOutfit(sim, mValue, alternate);
                    if (outfit != null)
                    {
                        mThumbnail = new ThumbnailKey(outfit, 0x0, ThumbnailSize.Medium, ThumbnailCamera.Body);
                    }
                }
            }
        }
    }
}


