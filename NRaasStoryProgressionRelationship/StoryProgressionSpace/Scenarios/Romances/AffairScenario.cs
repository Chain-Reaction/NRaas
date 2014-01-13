using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public class AffairScenario : DualSimScenario
    {
        ManagerRomance.AffairStory mAffairStory = ManagerRomance.AffairStory.All;

        public AffairScenario()
        { }
        public AffairScenario(SimDescription sim, SimDescription target, ManagerRomance.AffairStory affairStory)
            : base (sim, target)
        {
            mAffairStory = affairStory;
        }
        protected AffairScenario(AffairScenario scenario)
            : base (scenario)
        {
            mAffairStory = scenario.mAffairStory;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Tryst";
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
            return Romances.Partnered;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Flirts.FindExistingFor(this, sim, true);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!ManagerRomance.IsAffair(Sim, Target))
            {
                IncStat("Not Affair");
                return false;
            }

            int scoreA = AddScoring("CaughtCheating", GetValue<CheatDiscoveryChanceOption, int>(Sim), ScoringLookup.OptionType.Chance, Sim);
            int scoreB = AddScoring("CaughtCheating", GetValue<CheatDiscoveryChanceOption, int>(Target), ScoringLookup.OptionType.Chance, Target);

            if ((scoreA <= 0) && (scoreB <= 0))
            {
                scoreA = 0;
                if ((Sim.Partner != null) && (AddScoring("PartnerAffairAcceptance", Sim.Partner) < 0))
                {
                    scoreA = AddScoring("CatchCheating", GetValue<CheatDiscoveryChanceOption, int>(Sim.Partner), ScoringLookup.OptionType.Chance, Sim.Partner);
                }

                scoreB = 0;
                if ((Target.Partner != null) && (AddScoring("PartnerAffairAcceptance", Target.Partner) < 0))
                {
                    scoreB = AddScoring("CatchCheating", GetValue<CheatDiscoveryChanceOption, int>(Target.Partner), ScoringLookup.OptionType.Chance, Target.Partner);
                }

                if ((scoreA <= 0) && (scoreB <= 0))
                {
                    IncStat("Score Fail");
                    return false;
                }
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new CaughtCheatingScenario(Sim, Target), ScenarioResult.Start);
            Add(frame, new CaughtCheatingScenario(Target, Sim), ScenarioResult.Start);
            Add(frame, new SuccessScenario(), ScenarioResult.Start);
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if ((mAffairStory & ManagerRomance.AffairStory.Partner) == ManagerRomance.AffairStory.Partner)
            {
                if ((Sim.IsMarried) && (Target.IsMarried))
                {
                    name = "Married" + name;
                }
                else
                {
                    name = "Steady" + name;
                }
            }

            if ((Sim.Partner != null) && (Target.Partner != null))
            {
                if ((mAffairStory & ManagerRomance.AffairStory.Duo) == ManagerRomance.AffairStory.Duo)
                {
                    name += "Duo";
                }
                else if ((mAffairStory & ManagerRomance.AffairStory.Target) == ManagerRomance.AffairStory.Target)
                {
                    name += "Second";
                }
                else if ((mAffairStory & ManagerRomance.AffairStory.Actor) != ManagerRomance.AffairStory.Actor)
                {
                    // No story
                    return null;
                }
            }
            else if (Target.Partner != null)
            {
                if ((mAffairStory & ManagerRomance.AffairStory.Target) == ManagerRomance.AffairStory.Target)
                {
                    name += "Second";
                }
                else if ((mAffairStory & ManagerRomance.AffairStory.Actor) != ManagerRomance.AffairStory.Actor)
                {
                    // No story
                    return null;
                }
            }
            else
            {
                if ((mAffairStory & ManagerRomance.AffairStory.Actor) != ManagerRomance.AffairStory.Actor)
                {
                    // No story
                    return null;
                }
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new AffairScenario(this);
        }

        public class Installer : ExpansionInstaller<ManagerRomance>
        {
            protected override bool PrivateInstall(ManagerRomance main, bool initial)
            {
                if (initial)
                {
                    OldFlirtScenario.OnRomanceAffairScenario += OnPerform;
                    UnexpectedPregnancyScenario.PhaseTwoScenario.OnRomanceAffairScenario += OnPerform;

                    ManagerRomance.OnAllowAffair += OnAllowAffair;
                    ManagerRomance.OnAllowAdultery += OnAllowAdultery;
                    ManagerRomance.OnAllowLiaison += OnAllowLiaison;
                }

                return true;
            }

            protected static bool OnAllowAdultery(IScoringGenerator stats, SimData simData, Managers.Manager.AllowCheck check)
            {
                return (simData.GetValue<ChanceOfAdulteryOption, int>() != 0);
            }

            protected static bool OnAllowLiaison(IScoringGenerator stats, SimData simData, Managers.Manager.AllowCheck check)
            {
                return (simData.GetValue<ChanceOfLiaisonOption, int>() != 0);
            }

            protected static bool Test(IScoringGenerator stats, SimData actorData, SimData targetData, Managers.Manager.AllowCheck check)
            {
                SimDescription actor = actorData.SimDescription;
                SimDescription target = targetData.SimDescription;

                if (actor.Partner != null)
                {
                    int chance = actorData.GetValue<ChanceOfAdulteryOption, int>();
                    if (chance == 0)
                    {
                        stats.IncStat("Adultery Denied");
                        return false;
                    }
                    else if (stats.AddScoring("FlirtyPartner", chance, ScoringLookup.OptionType.Chance, actor) <= 0)
                    {
                        stats.IncStat("Adultery Scoring Fail");
                        return false;
                    }

                    chance = targetData.GetValue<ChanceOfLiaisonOption, int>();
                    if (chance == 0)
                    {
                        stats.IncStat("Liaison Denied");
                        return false;
                    }
                    if (stats.AddScoring("FlirtyPartner", chance, ScoringLookup.OptionType.Chance, target) <= 0)
                    {
                        stats.IncStat("Liaison Scoring Fail");
                        return false;
                    }
                }

                return true;
            }

            protected static bool OnAllowAffair(IScoringGenerator stats, SimData actorData, SimData targetData, Managers.Manager.AllowCheck check)
            {
                if (!Test(stats, actorData, targetData, check)) return false;

                if (!Test(stats, targetData, actorData, check)) return false;

                return true;
            }

            protected static void OnPerform(Scenario scenario, ScenarioFrame frame)
            {
                DualSimScenario s = scenario as DualSimScenario;

                ManagerRomance.AffairStory affairStory = ManagerRomance.AffairStory.All;

                OldFlirtScenario flirtScenario = scenario as OldFlirtScenario;
                if (flirtScenario != null)
                {
                    affairStory = flirtScenario.AffairStory;
                }

                scenario.Add(frame, new AffairScenario(s.Sim, s.Target, affairStory), ScenarioResult.Failure);
            }
        }
    }
}
