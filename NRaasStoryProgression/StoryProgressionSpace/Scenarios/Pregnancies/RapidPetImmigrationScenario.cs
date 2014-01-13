using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class RapidPetImmigrationScenario : ScheduledSoloScenario
    {
        public RapidPetImmigrationScenario()
        { }
        protected RapidPetImmigrationScenario(RapidPetImmigrationScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RapidPetImmigration";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            return true;
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            GetOption<Option>().SetValue(GetValue<Option,int>() - 1);

            Add(frame, new PetAdoptionScenario(), ScenarioResult.Start);
            return true;
        }

        public override Scenario Clone()
        {
            return new RapidPetImmigrationScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerPregnancy, RapidPetImmigrationScenario>, ManagerPregnancy.IAdoptionOption
        {
            public Option()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "RapidPetImmigration";
            }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                if (Value <= 0) return;

                if (!ShouldDisplay()) return;

                base.PrivateUpdate(fullUpdate, initialPass);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<PetAdoptionScenario.Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
