using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Flirts
{
    public class CannotFindLoveScenario : SimScenario
    {
        public CannotFindLoveScenario(SimDescription sim)
            : base (sim)
        { }
        protected CannotFindLoveScenario(CannotFindLoveScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "CantFindLove";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected int ImmigrationPressure
        {
            get { return GetValue<ImmigrantRequirementScenario.PressureOption,int>(); }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Lots.IncreaseImmigrationPressure(Flirts, ImmigrationPressure);

            if (Flirts.PreviousLoveLoss == null)
            {
                Flirts.PreviousLoveLoss = new SimID(Sim);
                return true;
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new CannotFindLoveScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerFlirt>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CantFindLove";
            }
        }
    }
}
