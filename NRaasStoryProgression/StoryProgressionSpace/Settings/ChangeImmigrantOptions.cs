using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class ChangeImmigrantOptions : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "ImmigrantOptions";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!base.Allow(parameters)) return false;

            Sim sim = parameters.mTarget as Sim;
            if (sim != null)
            {
                return (sim == Sim.ActiveActor);
            }

            return Common.IsRootMenuObject(parameters.mTarget);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            StoryProgression.Main.Options.ShowImmigrantOptions(StoryProgression.Main, Common.Localize("ImmigrantOptions:MenuName"));
            return OptionResult.SuccessRetain;
        }
    }
}

