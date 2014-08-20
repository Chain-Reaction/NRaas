using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.CAS;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class RandomizeGenetics : SimFromList, IAdvancedOption
    {
        int mRange = 0;

        bool mAdd = false;

        public override string GetTitlePrefix()
        {
            return "RandomizeGenetics";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.GetOutfit(OutfitCategories.Everyday, 0) == null) return false;

            if (me.Household == null) return false;

            if (me.Household == Household.ActiveHousehold) return true;

            return (!SimTypes.IsTourist(me));
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me }), "0");
                if (string.IsNullOrEmpty(text)) return false;

                if (!int.TryParse(text, out mRange))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }

                if (mRange <= 0) return false;

                mAdd = false;

                if (TwoButtonDialog.Show(
                    Common.Localize(GetTitlePrefix() + ":AddPrompt", me.IsFemale, new object[] { me, mRange }),
                    Common.Localize(GetTitlePrefix() + ":Add"),
                    Common.Localize(GetTitlePrefix() + ":Reroll")
                ))
                {
                    mAdd = true;
                }
            }

            float maximum = MasterController.Settings.mSliderMultiple;

            Vector2 range = new Vector2(-mRange / 256f, mRange / 256f);

            FacialBlends.RandomizeBlends(null, me, range, mAdd, range, true, false);

            return true;
        }
    }
}
