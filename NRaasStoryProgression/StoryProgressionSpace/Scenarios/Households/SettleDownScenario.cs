using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class SettleDownScenario : MoveScenario 
    {
        public SettleDownScenario (SimDescription sim, SimDescription target)
            : base (sim, target)
        { }
        protected SettleDownScenario(SettleDownScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "SettleDown";
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
            bool homeless = false;
            foreach (SimDescription sim in going)
            {
                if (sim.LotHome == null)
                {
                    homeless = true;
                    break;
                }
            }

            return new SettleDownMoveInScenario(going, moveInWith, homeless);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if ((Sim.Partner != null) && (Sim.Partner != Target))
            {
                IncStat("Not Partner");
                return false;
            }
            else if ((Target.Partner != null) && (Target.Partner != Sim))
            {
                IncStat("Not Partner");
                return false;
            }
            else if ((!Sim.IsMarried) && (!GetValue<AllowStrandedPartnerOption, bool>()))
            {
                IncStat("Not Married");
                return false;
            }
            else if (Target.Household == Sim.Household)
            {
                IncStat("Together");
                return false;
            }
            else if (AddScoring("SettleDown", Sim, Target) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }
            else if (AddScoring("SettleDown", Target, Sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.TargetAllow(sim);
        }

        public override Scenario Clone()
        {
            return new SettleDownScenario(this);
        }

        public class SettleDownMoveInScenario : MoveInScenario
        {
            bool mHomeless;

            public SettleDownMoveInScenario(List<SimDescription> going, SimDescription moveInWith, bool homeless)
                : base(going, moveInWith)
            {
                mHomeless = homeless;
            }
            protected SettleDownMoveInScenario(SettleDownMoveInScenario scenario)
                : base(scenario)
            {
                mHomeless = scenario.mHomeless;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "SettleDownMoveIn";
                }
                else
                {
                    return "MoveIn";
                }
            }

            protected override ManagerLot.FindLotFlags Inspect
            {
                get 
                {
                    return ManagerLot.FindLotFlags.Inspect | ManagerLot.FindLotFlags.InspectPets | ManagerLot.FindLotFlags.AllowExistingInfractions;
                }
            }

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (manager == null)
                {
                    if (mHomeless)
                    {
                        manager = Pregnancies;
                    }
                    else
                    {
                        manager = Flirts;
                    }
                }

                return base.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new SettleDownMoveInScenario(this);
            }
        }

        public class AllowStrandedPartnerOption : BooleanManagerOptionItem<ManagerRomance>
        {
            public AllowStrandedPartnerOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowStrandedPartner";
            }
        }
    }
}
