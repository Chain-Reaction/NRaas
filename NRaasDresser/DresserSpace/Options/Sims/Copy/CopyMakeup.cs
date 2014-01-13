using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.DresserSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DresserSpace.Options.Sims.Copy
{
    public class CopyCurrentMakeup : OperationSettingOption<Sim>, ICopyOption
    {
        public override string GetTitlePrefix()
        {
            return "CopyCurrentMakeup";
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!parameters.mTarget.IsHuman) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", parameters.mTarget.IsFemale, new object[] { parameters.mTarget })))
            {
                return OptionResult.Failure;
            }

            List<OutfitCategories> ignore = new List<OutfitCategories>();
            ignore.Add(OutfitCategories.Naked);

            ProcessOutfitTask.Perform(parameters.mTarget, CASParts.sMakeup, new CASParts.Key(parameters.mTarget), ignore);

            return OptionResult.SuccessRetain;
        }
    }
}
