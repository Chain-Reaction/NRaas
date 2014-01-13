using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class ChangeSimOptions : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "SimOptions";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!base.Allow(parameters)) return false;

            return (parameters.mTarget is Sim);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sim sim = parameters.mTarget as Sim;
            if (sim != null)
            {
                StoryProgression.Main.GetData(sim).ShowOptions(StoryProgression.Main, Common.Localize("SimOptions:MenuName"));
                return OptionResult.SuccessRetain;
            }

            return OptionResult.Failure;
        }
    }
}

