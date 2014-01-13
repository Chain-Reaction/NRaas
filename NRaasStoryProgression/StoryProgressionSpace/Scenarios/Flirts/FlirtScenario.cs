using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
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
    public abstract class FlirtScenario : RelationshipScenario
    {
        public FlirtScenario(int delta)
            : base (delta)
        { }
        public FlirtScenario(SimDescription sim)
            : base (sim, 20)
        { }
        protected FlirtScenario(FlirtScenario scenario)
            : base (scenario)
        { }

        protected abstract ManagerRomance.AffairStory AffairStory
        {
            get;
        }

        protected virtual int ReportSegregatedChance
        {
            get { return ReportChance; }
        }

        protected virtual bool TestAffair(SimDescription sim, SimDescription target)
        {
            return Romances.AllowAffair(this, sim, target, Managers.Manager.AllowCheck.None);
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        public override bool IsRomantic
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected abstract int PregnancyChance
        {
            get;
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected abstract bool TestFlirtCooldown
        {
            get;
        }

        protected virtual string EarlyFlirtStory
        {
            get { return "EarlyFlirt"; }
        }

        protected virtual string NewFlirtStory
        {
            get { return "NewFlirt"; }
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Flirts.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (AddScoring("Partner To Flirt Cooldown", TestElapsedTime<DayOfLastPartnerOption, MinTimeFromPartnerToFlirtOption>(sim)) < 0)
            {
                AddStat("Too Early", GetElapsedTime<DayOfLastPartnerOption>(sim));
                return false;
            }
            else if ((TestFlirtCooldown) && (AddScoring("Flirt To Flirt Cooldown", TestElapsedTime<DayOfLastRomanceOption, MinTimeBetweenFlirtsOption>(sim)) < 0))
            {
                AddStat("Too Early", GetElapsedTime<DayOfLastRomanceOption>(sim));
                return false;
            }
            else
            {
                int maximum = GetValue<ScheduledFlirtScenario.MaximumFlirtsOption, int>();
                if (maximum > 0)
                {
                    int totalFlirts = 0;
                    foreach (Relationship relation in Relationship.GetRelationships(sim))
                    {
                        if (relation.AreRomantic())
                        {
                            totalFlirts++;

                            if (totalFlirts >= maximum)
                            {
                                IncStat("Maximum Exceeded");
                                return false;
                            }
                        }
                    }
                }
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (ManagerRomance.IsAffair(Sim, Target))
            {
                if (!TestAffair(Sim, Target))
                {
                    IncStat("Affair Denied");
                    return false;
                }
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            bool reportSegregated = RandomUtil.RandomChance(ReportSegregatedChance);

            ManagerRomance.AffairStory affairStory = AffairStory;
            bool reportAffair = (affairStory != ManagerRomance.AffairStory.None);

            Add(frame, new EarlyFlirtScenario(Sim, Target, EarlyFlirtStory, reportSegregated && reportAffair), ScenarioResult.Failure);
            Add(frame, new NewFlirtScenario(Sim, Target, NewFlirtStory, reportSegregated && reportAffair), ScenarioResult.Failure);
            Add(frame, new OldFlirtScenario(Sim, Target, reportSegregated, AffairStory, PregnancyChance), ScenarioResult.Failure);
            return true;
        }

        protected override bool Push()
        {
            // The Push will be provided by one of the sub scenarios
            return true;
        }

        public class MinTimeFromPartnerToFlirtOption : Manager.CooldownOptionItem<ManagerFlirt>
        {
            public MinTimeFromPartnerToFlirtOption()
                : base(3)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownBetweenPartners";
            }
        }

        public class MinTimeBetweenFlirtsOption : Manager.CooldownOptionItem<ManagerFlirt>
        {
            public MinTimeBetweenFlirtsOption()
                : base(1)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownBetweenFlirts";
            }
        }
    }
}
