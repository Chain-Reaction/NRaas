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
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DresserSpace.Options.Sims.Reroll
{
    public class RerollCurrentOutfit : OperationSettingOption<Sim>, IRerollOption
    {
        public override string GetTitlePrefix()
        {
            return "RerollCurrentOutfit";
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            if (!parameters.mTarget.IsHuman) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", parameters.mTarget.IsFemale, new object[] { parameters.mTarget })))
            {
                return OptionResult.Failure;
            }

            CheckOutfitTask.Controller.AddToQueue(parameters.mTarget.SimDescription, CheckOutfitTask.ProcessOptions.Reroll | CheckOutfitTask.ProcessOptions.CurrentOutfit);
            return OptionResult.SuccessRetain;
        }
    }
}
