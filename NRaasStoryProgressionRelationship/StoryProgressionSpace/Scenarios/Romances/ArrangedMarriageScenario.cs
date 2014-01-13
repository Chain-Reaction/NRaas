using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public class ArrangedMarriageScenario : ScheduledMarriageBaseScenario, IEventScenario
    {
        public ArrangedMarriageScenario()
        { }
        protected ArrangedMarriageScenario(ArrangedMarriageScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ArrangedMarriage";
        }

        protected override bool TestScoring
        {
            get { return false; }
        }

        protected override bool TestPartnered
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 25; }
        }

        protected override int Rescheduling
        {
            get { return 0; }
        }

        public bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimAgeTransition);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (GetValue<ArrangedMarriageOption, ulong>(sim) == 0)
            {
                IncStat("No Match");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (SimTypes.IsDead(sim))
            {
                IncStat("Dead");
                return false;
            }

            return base.Allow(sim);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.TeensAndAdults;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            ulong match = GetValue<ArrangedMarriageOption, ulong>(sim);
            if (match == 0)
            {
                IncStat("No Match");
                return null;
            }

            SimDescription partner = ManagerSim.Find(match);
            if (partner == null)
            {
                SetValue<ArrangedMarriageOption, ulong>(sim, 0);

                IncStat("Missing Dropped");
                return null;
            }
            else if (SimTypes.IsDead(partner))
            {
                SetValue<ArrangedMarriageOption, ulong>(sim, 0);

                IncStat("Target Dead");
                return null;
            }
            else if (GetValue<ArrangedMarriageOption, ulong>(partner) != sim.SimDescriptionId)
            {
                SetValue<ArrangedMarriageOption, ulong>(sim, 0);

                IncStat("Mismatch Dropped");
                return null;
            }

            List<SimDescription> results = new List<SimDescription>();
            results.Add(partner);

            return results;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            // Doing so stops the Allow() from stopping the engagement
            SetValue<ArrangedMarriageOption, ulong>(Sim, 0);
            SetValue<ArrangedMarriageOption, ulong>(Target, 0);

            if ((Sim.IsMarried) || (Target.IsMarried))
            {
                IncStat("Married Flagged");
                return false;
            }

            Relationship relationship = Relationship.Get(Sim, Target, true);
            if (relationship != null)
            {
                relationship.MakeAcquaintances();

                if (!relationship.AreRomantic())
                {
                    // Bump to Romantic Interest
                    Romances.BumpToHigherState(this, Sim, Target);
                }
            }

            Add(frame, new PartneringScenario(Sim, Target, false, true), ScenarioResult.Start);
            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new ArrangedMarriageScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerRomance,ArrangedMarriageScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ArrangedMarriage";
            }

            public override bool Install(ManagerRomance main, bool initial)
            {
                main.OnDualAllow += OnDualAllow;

                return base.Install(main, initial);
            }

            protected static bool OnDualAllow(Common.IStatGenerator stats, SimData actorData, SimData targetData, Managers.Manager.AllowCheck check)
            {
                ulong id = actorData.GetValue<ArrangedMarriageOption, ulong>();
                if (id != 0)
                {
                    if (targetData.SimDescription.SimDescriptionId != id)
                    {
                        stats.IncStat("Arranged");
                        return false;
                    }
                }

                id = targetData.GetValue<ArrangedMarriageOption, ulong>();
                if (id != 0)
                {
                    if (actorData.SimDescription.SimDescriptionId != id)
                    {
                        stats.IncStat("Arranged");
                        return false;
                    }
                }

                return true;
            }
        }

        public class ScheduledOption : BooleanScenarioOptionItem<ManagerRomance, ArrangedMarriageScenario>, IDebuggingOption
        {
            public ScheduledOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ScheduledArrangedMarriage";
            }
        }
    }
}
