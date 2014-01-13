using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class SelectSim : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            if ((Household.ActiveHousehold != null) &&
                (StoryProgression.Main.GetValue<DreamCatcherOption, bool>(Household.ActiveHousehold)))
            {
                return "MakeActiveDreamCatcher";
            }
            return "MakeActive";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            Sim sim = parameters.mTarget as Sim;
            if (sim == null) return false;

            if (sim.SimDescription.IsNeverSelectable) return false;

            if (sim.LotHome == null) return false;

            if (sim.Household == null) return false;

            if (sim.Household.IsActive) return false;

            if (SimTypes.IsSpecial(sim.SimDescription)) return false;

            return true;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sim sim = parameters.mTarget as Sim;
            if (sim == null) return OptionResult.Failure;

            StoryProgression.Main.Sims.Select(sim);
            return OptionResult.SuccessClose;
        }
    }
}

