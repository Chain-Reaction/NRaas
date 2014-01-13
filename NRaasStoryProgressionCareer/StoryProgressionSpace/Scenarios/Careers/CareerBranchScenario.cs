using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class CareerBranchScenario : SimScenario
    {
        static List<ulong> sSuppressed = new List<ulong>();

        public CareerBranchScenario(SimDescription sim)
            : base (sim)
        { }
        protected CareerBranchScenario(CareerBranchScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CareerBranch";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            Career career = sim.Occupation as Career;
            if (career == null)
            {
                IncStat("No Job");
                return false;
            }
            else if (career.CurLevel == null)
            {
                IncStat("No Level");
                return false;
            }
            else if (career.CurLevel.LastLevel == null)
            {
                IncStat("First Level");
                return false;
            }
            else if (career.CurLevel.LastLevel.NextLevels.Count <= 1)
            {
                IncStat("Not Branch");
                return false;
            }

            if ((!GetValue<PromptOption, bool>()) || (!Careers.MatchesAlertLevel(Sim)))
            {
                List<DreamJob> dreams = ManagerCareer.GetDreamJob(Sim);
                if (dreams.Count == 0)
                {
                    IncStat("No Dream");
                    return false;
                }
                else if (!DreamJob.Contains(dreams, career.Guid))
                {
                    IncStat("Wrong Job");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            LifetimeWant lifetimeWant = (LifetimeWant)Sim.LifetimeWish;

            bool prompt = (GetValue<PromptOption, bool>()) && (Careers.MatchesAlertLevel(Sim));

            DreamNodeInstance instance = null;
            DreamsAndPromisesManager.sMajorWishes.TryGetValue(Sim.LifetimeWish, out instance);
            if (instance == null)
            {
                if (!prompt)
                {
                    IncStat("No LTW");
                    return false;
                }
            }
            else if (instance.InputSubject == null)
            {
                IncStat("No InputSubject");
                return false;
            }

            Career career = Sim.Occupation as Career;

            CareerLevel prior = career.CurLevel.LastLevel;
            if (prior == null)
            {
                IncStat("No Prior");
                return false;
            }

            if (prior.NextLevels.Count != 2)
            {
                IncStat("Not Branch");
                return false;
            }

            CareerLevel newLevel = null;

            if (prompt)
            {
                bool flag = TwoButtonDialog.Show(
                    ManagerSim.GetPersonalInfo(Sim, Common.LocalizeEAString(Sim.IsFemale, career.SharedData.Text_BranchOffer, new object[] { Sim })),
                    Common.LocalizeEAString(Sim.IsFemale, career.SharedData.Text_Branch1, new object[0]),
                    Common.LocalizeEAString(Sim.IsFemale, career.SharedData.Text_Branch2, new object[0])
                );

                if (flag)
                {
                    if (prior.NextLevels[0] == career.CurLevel) return false;
                    newLevel = prior.NextLevels[0];
                }
                else
                {
                    if (prior.NextLevels[1] == career.CurLevel) return false;
                    newLevel = prior.NextLevels[1];
                }
            }
            else if (instance.InputSubject != null)
            {
                if (instance.InputSubject.mType != DreamNodePrimitive.InputSubjectType.Career)
                {
                    IncStat("Not Career LTW");
                    return false;
                }

                switch (career.Guid)
                {
                    case OccupationNames.Music:
                        if (lifetimeWant == LifetimeWant.RockStar)
                        {
                            if (career.CurLevelBranchName == "ElectricRock")
                            {
                                IncStat("ElectricRock");
                                return false;
                            }
                        }
                        else
                        {
                            if (career.CurLevelBranchName != "ElectricRock")
                            {
                                IncStat("Not ElectricRock");
                                return false;
                            }
                        }
                        break;
                    case OccupationNames.Criminal:
                        if (lifetimeWant == LifetimeWant.TheEmperorOfEvil)
                        {
                            if (career.CurLevelBranchName == "Evil")
                            {
                                IncStat("Evil");
                                return false;
                            }
                        }
                        else
                        {
                            if (career.CurLevelBranchName != "Evil")
                            {
                                IncStat("Not Evil");
                                return false;
                            }
                        }
                        break;
                    case OccupationNames.LawEnforcement:
                        if (lifetimeWant == LifetimeWant.ForensicSpecialistDynamicDNAProfiler)
                        {
                            if (career.CurLevelBranchName == "ForensicAnalyst")
                            {
                                IncStat("Forensic");
                                return false;
                            }
                        }
                        else
                        {
                            if (career.CurLevelBranchName != "ForensicAnalyst")
                            {
                                IncStat("Not Forensic");
                                return false;
                            }
                        }
                        break;
                    case OccupationNames.Film:
                        if (lifetimeWant == LifetimeWant.FileActor)
                        {
                            if (career.CurLevelBranchName == "Acting")
                            {
                                IncStat("Acting");
                                return false;
                            }
                        }
                        else
                        {
                            if (career.CurLevelBranchName != "Acting")
                            {
                                IncStat("Not Acting");
                                return false;
                            }
                        }
                        break;
                    default:
                        return false;
                }

                foreach (CareerLevel level in prior.NextLevels)
                {
                    if (career.CurLevel == level) continue;

                    newLevel = level;
                    break;
                }
            }

            if (newLevel == null) return false;

            CareerLevel curLevel = career.CurLevel.LastLevel;

            career.GivePromotionRewardObjectsIfShould(newLevel);
            career.SetLevel(newLevel);

            try
            {
                PromotedScenario.AddSuppressed(Sim);

                career.OnPromoteDemote(curLevel, newLevel);
            }
            finally
            {
                PromotedScenario.RemoveSuppressed(Sim);
            }

            if (Sim.CreatedSim != null)
            {
                career.SetTones(Sim.CreatedSim.CurrentInteraction);
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new CareerBranchScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerCareer>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerCareer main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    PromotedScenario.OnCareerBranchScenario += OnPerform;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "AutoChooseBranch";
            }

            public static void OnPerform(Scenario scenario, ScenarioFrame frame)
            {
                PromotedScenario promoted = scenario as PromotedScenario;
                if (promoted != null)
                {
                    if (promoted.IsSuppressed()) return;
                }

                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new CareerBranchScenario(s.Sim), ScenarioResult.Start);
            }
        }

        public class PromptOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public PromptOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "PromptOnBranch";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
