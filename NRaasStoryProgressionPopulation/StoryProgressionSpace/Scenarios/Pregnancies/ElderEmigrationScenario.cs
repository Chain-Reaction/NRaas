using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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
    public class ElderEmigrationScenario : SimScenario
    {
        SimDescription mPartner;

        public ElderEmigrationScenario()
        { }
        protected ElderEmigrationScenario(ElderEmigrationScenario scenario)
            : base (scenario)
        {
            mPartner = scenario.mPartner;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "ElderEmigration";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.Adults;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!sim.Elder)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("No Home");
                return false;
            }
            else if (!Households.Allow(this, sim))
            {
                IncStat("Move Denied");
                return false;
            }
            else if ((sim.Partner != null) && (!SimTypes.IsDead(sim.Partner)) && (Pregnancies.Allow(this, sim, sim.Partner, Managers.Manager.AllowCheck.Active)))
            {
                IncStat("Fertile Partnered");
                return false;
            }
            else if (!GetValue<EmigrationOption, bool>(sim))
            {
                IncStat("Emigrate Denied");
                return false;
            }
            else if (HouseholdsEx.NumHumansIncludingPregnancy(sim.Household) < GetValue<MaximumSizeOption,int>(sim.Household))
            {
                IncStat("House Empty");
                return false;
            }
            else
            {
                bool found = false;
                foreach (SimDescription other in HouseholdsEx.Humans(sim.Household))
                {
                    if (other == sim) continue;

                    if (other == sim.Partner) continue;

                    if (other.Partner == null) continue;

                    if (!Pregnancies.Allow(this, other)) continue;

                    if (ExpectedPregnancyBaseScenario.GetNumLiveChildren(other) > 0) continue;

                    if (AddScoring("PreferredBabyCount", other) <= 0) continue;

                    if ((!Relationships.IsCloselyRelated(other, sim, false)) ||
                        (!Relationships.IsCloselyRelated(other, sim.Partner, false)))
                    {
                        continue;
                    }

                    found = true;
                }

                if (!found)
                {
                    IncStat("Not Necessary");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Deaths.CleansingKill(Sim, SimDescription.DeathType.OldAge, false))
            {
                mPartner = Sim.Partner;

                if (Sim.Partner != null)
                {
                    Deaths.CleansingKill(Sim.Partner, SimDescription.DeathType.OldAge, false);
                }
                return true;
            }
            return false;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if ((mPartner != null) && (parameters == null))
            {
                parameters = new object[] { Sim, mPartner };

                name += "Duo";
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new ElderEmigrationScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim, ElderEmigrationScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ElderEmigration";
            }

            public override bool Value
            {
                get
                {
                    if (!ShouldDisplay()) return false;

                    return base.Value;
                }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<EmigrationScenario.Option, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
