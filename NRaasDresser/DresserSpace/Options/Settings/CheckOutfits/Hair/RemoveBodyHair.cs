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

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits.Hair
{
    public class RemoveBodyHair : ProcessAllSimsOption, IHairOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveAllBodyHair";
        }

        protected override void Process(SimDescription sim)
        {
            List<BodyTypes> types = new List<BodyTypes>(CASParts.BodyHairTypes);
            types.Add(BodyTypes.Beard);

            RemovePartsOutfitTask.Perform(sim, types, false, null);
        }
    }
}
