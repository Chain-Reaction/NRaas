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

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits
{
    public class ImmediateCheckServiceOutfits : ProcessAllSimsOption, ICheckOutfitsOption
    {
        public override string GetTitlePrefix()
        {
            return "ImmediateCheckServiceOutfits";
        }

        protected override void Process(SimDescription sim)
        {
            if (!SimTypes.IsService(sim)) return;

            CheckOutfitTask.Controller.AddToQueue(sim, CheckOutfitTask.ProcessOptions.Invalid | CheckOutfitTask.ProcessOptions.Accessories | CheckOutfitTask.ProcessOptions.Makeup | CheckOutfitTask.ProcessOptions.BodyHair | CheckOutfitTask.ProcessOptions.Beard);
        }
    }
}
