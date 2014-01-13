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
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DresserSpace.Options.Sims.Reroll
{
    public class RerollAccessories : OperationSettingOption<Sim>, IRerollOption
    {
        public override string GetTitlePrefix()
        {
            return "RerollAccessories";
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", parameters.mTarget.IsFemale, new object[] { parameters.mTarget })))
            {
                return OptionResult.Failure;
            }

            RemovePartsOutfitTask.Perform(parameters.mTarget, CASParts.sAccessories, false, delegate 
            {
                Dresser.Settings.RemoveTested(parameters.mTarget.SimDescription, CheckOutfitTask.ProcessOptions.Accessories);
                CheckOutfitTask.Controller.AddToQueue(parameters.mTarget.SimDescription, CheckOutfitTask.ProcessOptions.Accessories); 
            });

            return OptionResult.SuccessRetain;
        }
    }
}
