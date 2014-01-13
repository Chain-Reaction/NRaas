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

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits.Makeup
{
    public class RerollMakeup : ProcessAllSimsOption, IMakeupOption
    {
        public override string GetTitlePrefix()
        {
            return "RerollAllMakeup";
        }

        protected override void Process(SimDescription sim)
        {
            RemovePartsOutfitTask.Perform(sim, CASParts.sMakeup, false, delegate
            {
                Dresser.Settings.RemoveTested(sim, CheckOutfitTask.ProcessOptions.Makeup);
                CheckOutfitTask.Controller.AddToQueue(sim, CheckOutfitTask.ProcessOptions.Makeup);
            });
        }
    }
}
