using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Flirts
{
    public class AgeUpScenario : AgeUpBaseScenario
    {
        public AgeUpScenario()
        { }
        protected AgeUpScenario(AgeUpScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "AgeUp";
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            if (Sim.Teen)
            {
                float gaySims = 0;
                float straightSims = 0;
                Flirts.CalculateGayRatio(ref gaySims, ref straightSims);

                bool allowGay = ((gaySims / straightSims) * 100) < GetValue<ManagerFlirt.MaximumGayRatioOption, int>();

                Flirts.SetGenderPreference(Sim, allowGay);
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new AgeUpScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerFlirt, AgeUpScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "FlirtAgeUp";
            }
        }
    }
}
