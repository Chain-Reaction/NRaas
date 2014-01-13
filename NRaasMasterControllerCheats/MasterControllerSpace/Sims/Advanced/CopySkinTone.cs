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
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class CopySkinTone : DualSimFromList, IAdvancedOption
    {
        public override string GetTitlePrefix()
        {
            return "CopySkinTone";
        }

        protected override int GetMaxSelectionB(IMiniSimDescription sim)
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override string GetTitleA()
        {
            return Common.Localize("Copy:Source");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("Copy:Destination");
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.GetOutfit(OutfitCategories.Everyday, 0) == null) return false;

            //if (SimTypes.IsSkinJob(me)) return false;

            if (me.Household == null) return false;

            if (me.Household == Household.ActiveHousehold) return true;

            return (!SimTypes.IsTourist(me));
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", a.IsFemale, b.IsFemale, new object[] { a, b })))
                {
                    return false;
                }
            }

            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(b, CASParts.sPrimary))
            {
                builder.Builder.SkinTone = a.SkinToneKey;
                builder.Builder.SkinToneIndex = a.SkinToneIndex;
            }

            new SavedOutfit.Cache(b).PropagateGenetics(b, CASParts.sPrimary);

            return true;
        }
    }
}
