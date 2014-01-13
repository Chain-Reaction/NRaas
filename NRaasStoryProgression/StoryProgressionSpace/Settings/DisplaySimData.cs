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
    public class DisplaySimData : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "DisplaySimData";
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

            Common.StringBuilder result = new Common.StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            result += Common.NewLine + "<Settings>";

            result += Common.NewLine + StoryProgression.Main.GetData(sim).ToString();

            result += Common.NewLine + "</Settings>";

            Common.WriteLog(result, false);

            Common.Notify(result);
            return OptionResult.SuccessRetain;
        }
    }
}
