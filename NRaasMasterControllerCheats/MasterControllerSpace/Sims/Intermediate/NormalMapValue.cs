using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public abstract class NormalMapValue : SimFromList
    {
        float mValue = 0;

        protected abstract float GetValue(SimBuilder builder);

        protected static string GetOutfitName(SimDescription me, OutfitCategories category, int outfitIndex)
        {
            return string.Concat(new object[] { me.FirstName, me.LastName, category.ToString(), outfitIndex });
        }

        protected abstract void ApplyValue(SimBuilder builder, float value);

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.IsUsingMaternityOutfits) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(me, CASParts.sPrimary))
            {
                if (!builder.OutfitValid) return true;

                if (!ApplyAll)
                {
                    float oldValue = GetValue(builder.Builder);

                    string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me }), oldValue.ToString(), 256, StringInputDialog.Validation.None);
                    if (string.IsNullOrEmpty(text)) return false;

                    mValue = 0;
                    if (!float.TryParse(text, out mValue))
                    {
                        SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                        return false;
                    }
                }

                ApplyValue(builder.Builder, mValue);
            }

            new SavedOutfit.Cache(me).PropagateGenetics(me, CASParts.sPrimary);

            return true;
        }
    }
}
