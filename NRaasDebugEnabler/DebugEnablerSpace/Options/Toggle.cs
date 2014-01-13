using NRaas.CommonSpace.Options;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Options
{
    public class Toggle : OperationSettingOption<GameObject>, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            if (DebugEnabler.Settings.mEnabled)
            {
                return "ToggleOff";
            }
            else
            {
                return "ToggleOn";
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return (parameters.mTarget is Sim);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            DebugEnabler.Settings.mEnabled = !DebugEnabler.Settings.mEnabled;
            return OptionResult.SuccessClose;
        }
    }
}