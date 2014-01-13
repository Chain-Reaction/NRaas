using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class TrainerPushScenario : DualSimSingleProcessScenario, IHasSkill
    {
        bool mPushed;
        
        public TrainerPushScenario()
        { }
        protected TrainerPushScenario(TrainerPushScenario scenario)
            : base (scenario)
        {
            mPushed = scenario.mPushed;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "TrainerPush";
        }

        protected override int ContinueChance
        {
            get { return 25; }
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Athletic };
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return new SimScoringList(this, "Trainer", Sims.TeensAndAdults, false).GetBestByMinScore(1);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            List<SimDescription> sims = new List<SimDescription>();

            if ((sim.CreatedSim != null) && (sim.CreatedSim.LotCurrent != null) && (sim.CreatedSim.LotCurrent.CanSimTreatAsHome(sim.CreatedSim)))
            {
                foreach (AthleticGameObject obj in sim.CreatedSim.LotCurrent.GetObjects<AthleticGameObject>())
                {
                    if (!obj.InUse) continue;

                    if (obj.ActorsUsingMe.Count == 1)
                    {
                        sims.Add(obj.ActorsUsingMe[0].SimDescription);
                    }
                }
            }

            if (sims.Count == 0)
            {
                foreach (AthleticGameObject obj in Sims3.Gameplay.Queries.GetObjects<AthleticGameObject>())
                {
                    if (!obj.InUse) continue;

                    if (obj.ActorsUsingMe.Count == 1)
                    {
                        sims.Add(obj.ActorsUsingMe[0].SimDescription);
                    }
                }
            }

            if (sims.Count > 0)
            {
                return sims;
            }

            return new SimScoringList(this, "Athletic", Sims.TeensAndAdults, false).GetBestByMinScore(1);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.Household == Sim.Household)
            {
                IncStat("Same Family");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CreatedSim.BuffManager.HasElement(BuffNames.Fatigued))
            {
                IncStat("Fatigued");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skill Denied");
                return false;
            }
            else if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        public static bool TestObject(AthleticGameObject obj)
        {
            if (obj.InUse) return false;

            if (!obj.CanObjectTrainSim()) return false;

            if (obj.LotCurrent == null) return false;

            if (!obj.InWorld) return false;

            return true;
        }

        protected bool FirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                List<AthleticGameObject> choices = new List<AthleticGameObject>(parent.mLot.GetObjects<AthleticGameObject>(TestObject));
                if (choices.Count == 0)
                {
                    return false;
                }

                InteractionInstance interaction = parent.mSimB.InteractionQueue.GetCurrentInteraction();
                if ((interaction != null) && (interaction.Target is AthleticGameObject))
                {
                    OnSecondAction(parent.mSimA, parent.mSimB, meetUp, parent.mSimB, 0);
                }
                else
                {
                    AthleticGameObject choice = RandomUtil.GetRandomObjectFromList(choices);

                    GoToLotSituation.Action startUp = new GoToLotSituation.Action(meetUp, OnSecondAction);

                    parent.mSimB.InteractionQueue.CancelAllInteractions();

                    if (choice.Cardio)
                    {
                        meetUp.ForceSituationSpecificInteraction(choice, parent.mSimB, AthleticGameObject.WorkOut.CardioSingleton, startUp.OnPerform, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed, new InteractionPriority(InteractionPriorityLevel.UserDirected));
                    }
                    else
                    {
                        meetUp.ForceSituationSpecificInteraction(choice, parent.mSimB, AthleticGameObject.WorkOut.StrengthSingleton, startUp.OnPerform, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed, new InteractionPriority(InteractionPriorityLevel.UserDirected));
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        protected bool OnSecondAction(Sim a, Sim b, GoToLotSituation.MeetUp meetUp, Sim actor, float x)
        {
            if (mPushed) return false;
            mPushed = true;

            InteractionInstance interactionB = b.InteractionQueue.GetCurrentInteraction();
            if (interactionB == null)
            {
                return false;
            }

            AthleticGameObject choice = interactionB.Target as AthleticGameObject;
            if (choice == null)
            {
                return false;
            }

            if ((a.InteractionQueue.HasInteractionOfType(QueueTrainSimEx.Singleton)) ||
                (a.InteractionQueue.HasInteractionOfType(AthleticGameObject.TrainSim.Singleton)))
            {
                return true;
            }

            a.InteractionQueue.CancelAllInteractions();

            QueueTrainSimEx train = meetUp.ForceSituationSpecificInteraction(choice, a, QueueTrainSimEx.Singleton, null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed, new InteractionPriority(InteractionPriorityLevel.UserDirected)) as QueueTrainSimEx;
            if (train == null)
            {
                return false;
            }

            StoryProgression.Main.Skills.Post(new ReportScenario(a.SimDescription, b.SimDescription));

            train.Trainee = b;
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            List<AthleticGameObject> objs = new List<AthleticGameObject>();

            InteractionInstance interaction = Target.CreatedSim.InteractionQueue.GetCurrentInteraction();
            if ((interaction != null) && (interaction.Target is AthleticGameObject))
            {
                IncStat("Trainee Ready");

                objs.Add(interaction.Target as AthleticGameObject);
            }
            else
            {
                foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
                {
                    objs.AddRange(lot.GetObjects<AthleticGameObject>(TestObject));
                }

                if (objs.Count == 0)
                {
                    foreach (Lot lot in ManagerLot.GetOwnedLots(Target))
                    {
                        objs.AddRange(lot.GetObjects<AthleticGameObject>(TestObject));
                    }
                    if (objs.Count == 0)
                    {
                        foreach (AthleticGameObject obj in Sims3.Gameplay.Queries.GetObjects<AthleticGameObject>())
                        {
                            if (!TestObject(obj)) continue;

                            if (!obj.LotCurrent.IsCommunityLot) continue;

                            objs.Add(obj);
                        }

                        if (objs.Count == 0)
                        {
                            IncStat("No Training Object");
                            return false;
                        }
                    }
                }
            }

            return Situations.PushMeetUp(this, Sim, Target, RandomUtil.GetRandomObjectFromList(objs).LotCurrent, FirstAction);
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new TrainerPushScenario(this);
        }

        public class ReportScenario : DualSimScenario
        {
            public ReportScenario(SimDescription sim, SimDescription target)
                : base (sim, target)
            { }
            public ReportScenario(ReportScenario scenario)
                : base (scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                return "Trainer";
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
                return null;
            }

            protected override ICollection<SimDescription> GetTargets(SimDescription sim)
            {
                return null;
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                return true;
            }

            public override Scenario Clone()
            {
                return new ReportScenario(this);
            }
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, TrainerPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "TrainerPush";
            }
        }
    }
}
