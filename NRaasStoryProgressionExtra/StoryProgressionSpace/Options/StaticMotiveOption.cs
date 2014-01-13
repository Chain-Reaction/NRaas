using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public abstract class StaticMotiveOption : SimBooleanOption
    {
        public StaticMotiveOption()
            : base(false)
        { }

        public override bool Value
        {
            get
            {
                // Return True, rather than False
                if (!ShouldDisplay()) return true;

                return PureValue;
            }
        }

        public override void SetValue(bool value, bool persist)
        {
            UpdateMotivesScenario.SetToImmediateUpdate();

            base.SetValue(value, persist);
        }
    }
}

