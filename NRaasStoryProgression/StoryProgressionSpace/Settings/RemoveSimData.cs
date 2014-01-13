using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class RemoveSimData : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "RemoveSimData";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!(parameters.mTarget is Sim)) return false;

            if (!StoryProgression.Main.DebuggingEnabled) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sim sim = parameters.mTarget as Sim;
            if (sim == null) return OptionResult.Failure;

            if (sim.LotHome == null) return OptionResult.Failure;

            StoryProgression.Main.Options.RemoveSim(sim);
            return OptionResult.SuccessRetain;
        }
    }
}
