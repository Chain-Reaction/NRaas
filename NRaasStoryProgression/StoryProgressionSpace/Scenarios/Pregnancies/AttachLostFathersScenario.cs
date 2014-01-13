using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
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
    public class AttachLostFathersScenario : SimScenario
    {
        ICollection<SimDescription> mSims = null;

        public AttachLostFathersScenario()
        { }
        public AttachLostFathersScenario(ICollection<SimDescription> sims)
        {
            mSims = sims;
        }
        protected AttachLostFathersScenario(AttachLostFathersScenario scenario)
            : base (scenario)
        {
            mSims = scenario.mSims;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "AttachLostFathers";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AlwaysReschedule
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 60; }
        }

        protected override int MaximumReschedules
        {
            get { return int.MaxValue; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            if (mSims != null)
            {
                return mSims;
            }
            else
            {
                return Pregnancies.PregnantSims;
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!sim.IsPregnant)
            {
                IncStat("Not Pregnant");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Pregnancy pregnancy = Sim.Pregnancy;
            if (pregnancy == null) return false;

            SimDescription dad = ManagerSim.Find(pregnancy.DadDescriptionId);

            if (Sim.LotHome == null)
            {
                if (dad != null)
                {
                    Add(frame, new ForcedMoveScenario(Sim, dad), ScenarioResult.Start);
                    Add(frame, new NoHomeScenario(Sim, dad), ScenarioResult.Failure);
                }

                Add(frame, new GetOffStreetsScenario(Sim), ScenarioResult.Start);
            }

            if (dad != null)
            {
                if (dad.LotHome == null)
                {
                    Add(frame, new GetOffStreetsScenario(dad), ScenarioResult.Start);
                }

                if (dad.CreatedSim != null)
                {
                    if (pregnancy.mDad != dad.CreatedSim)
                    {
                        pregnancy.mDad = dad.CreatedSim;

                        IncStat("Attached");
                        return true;
                    }
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new AttachLostFathersScenario(this);
        }

        public class ForcedMoveScenario : MoveScenario
        {
            public ForcedMoveScenario(SimDescription sim, SimDescription target)
                : base(sim, target)
            { }
            protected ForcedMoveScenario(ForcedMoveScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;
                
                return "ForcedMove";
            }

            protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
            {
                return new StandardMoveInLotScenario(going, 0);
            }

            protected override MoveInLotScenario GetMoveInLotScenario(HouseholdBreakdown breakdown)
            {
                return new StandardMoveInLotScenario(breakdown, 0);
            }

            protected override MoveInScenario GetMoveInScenario(List<SimDescription> going, SimDescription moveInWith)
            {
                return new StrandedCoupleScenario.StrandedMoveInScenario(going, moveInWith);
            }

            public override Scenario Clone()
            {
                return new ForcedMoveScenario(this);
            }
        }

        public class Option : BooleanScenarioOptionItem<ManagerPregnancy, AttachLostFathersScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AttachLostFathers";
            }
        }
    }
}
