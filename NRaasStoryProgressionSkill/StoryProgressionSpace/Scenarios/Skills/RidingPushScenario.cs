using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Pets;
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public class RidingPushScenario : SimSingleProcessScenario, IHasSkill
    {
        static CompetitionType[] sTypes = new CompetitionType[] { CompetitionType.Racing, CompetitionType.Jumping, CompetitionType.CrossCountry };

        static CompetitionLevel[] sLevels = new CompetitionLevel[] { CompetitionLevel.Beginner, CompetitionLevel.Advanced, CompetitionLevel.International };

        public RidingPushScenario()
        { }
        protected RidingPushScenario(RidingPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Riding";
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Riding };
            }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return new SimScoringList(this, "Riding", Sims.TeensAndAdults, false).GetBestByMinScore(1);
        }

        protected override bool Allow(SimDescription sim)
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
            else if (sim.SkillManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skill Denied");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("Careers Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new MountScenario(Sim, 0), ScenarioResult.Start);
            return false;
        }

        public class MountScenario : SimScenario, ICallbackScenario
        {
            int mPushes = 0;

            public MountScenario(SimDescription sim, int pushes)
                : base(sim)
            {
                mPushes = pushes;
            }
            protected MountScenario(MountScenario scenario)
                : base(scenario)
            {
                mPushes = scenario.mPushes;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "RidingMount";
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
                get { return true; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected override bool Allow()
            {
                if (mPushes > 5) return false;

                return base.Allow();
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                if (Sim.CreatedSim == null) return false;

                if (Sim.CreatedSim.Posture == null) return false;

                Sim horseSim = Sim.CreatedSim.Posture.Container as Sim;
                if ((horseSim == null) || (!horseSim.IsHorse))
                {
                    bool horseFound = false;

                    List<SimDescription> lotHorses = new List<SimDescription>();
                    List<SimDescription> horses = new List<SimDescription>();
                    foreach (SimDescription sim in HouseholdsEx.Pets(Sim.Household))
                    {
                        if (!sim.IsHorse) continue;

                        if (sim.SkillManager == null) continue;

                        horseFound = true;

                        if (sim.CreatedSim == null) continue;

                        if (sim.ChildOrBelow) continue;

                        if (sim.IsPregnant) continue;

                        if (ManagerSim.GetLTR(sim, Sim) < 0) continue;

                        if (sim.CreatedSim.LotCurrent == Sim.CreatedSim.LotCurrent)
                        {
                            lotHorses.Add(sim);
                        }

                        horses.Add(sim);
                    }

                    if (horses.Count == 0)
                    {
                        IncStat("No Horse");

                        if (!horseFound)
                        {
                            bool found = false;
                            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
                            {
                                if (lot.GetObjects<IBoxStall>().Length > 0)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                IncStat("No Stalls");
                                return false;
                            }

                            Add(frame, new PetAdoptionScenario(CASAgeGenderFlags.Horse, Sim), ScenarioResult.Start);
                        }

                        return false;
                    }

                    if (lotHorses.Count > 0)
                    {
                        horses = lotHorses;
                    }

                    AddStat("Horses", horses.Count);

                    while (horses.Count > 0)
                    {
                        SimDescription horseChoice = RandomUtil.GetRandomObjectFromList(horses);
                        horses.Remove(horseChoice);

                        float num = horseChoice.HasTrait(TraitNames.UntrainedHorse) ? TraitTuning.UntrainedHorseSaddleLtrThreshold : RidingSkill.kMinLtrForMountAccept;

                        if (ManagerSim.GetLTR(horseChoice, Sim) < num)
                        {
                            Add(frame, new ExistingFriendScenario(horseChoice, Sim, false), ScenarioResult.Start);

                            IncStat("Friend Building");
                            return false;
                        }

                        horseSim = horseChoice.CreatedSim;

                        if (RemountScenario.Perform(Manager, this, Sim, horseSim, new RemountScenario(Sim, horseSim)))
                        {
                            break;
                        }

                        horseSim = null;
                    }

                    if (horseSim == null)
                    {
                        IncStat("Mount Fail");
                        return false;
                    }

                    Skills.AddListener(new HorseMountedScenario(Sim, horseSim, mPushes));

                    IncStat("AddListener");
                }
                else
                {
                    Add(frame, new HorseMountedScenario(Sim, horseSim, mPushes), ScenarioResult.Start);
                }

                return true;
            }

            public override Scenario Clone()
            {
                return new MountScenario(this);
            }
        }

        public class RemountScenario : SimScenario, ICallbackScenario
        {
            Sim mHorse;

            int mMountAttempts = 0;

            public RemountScenario(SimDescription sim, Sim horse)
                : base(sim)
            {
                mHorse = horse;
            }
            protected RemountScenario(RemountScenario scenario)
                : base(scenario)
            {
                mHorse = scenario.mHorse;
                mMountAttempts = scenario.mMountAttempts;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "RidingRemount";
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Progressed
            {
                get { return true; }
            }

            protected override bool UsesSim(ulong sim)
            {
                if ((mHorse != null) && (mHorse.SimDescription != null))
                {
                    if (mHorse.SimDescription.SimDescriptionId == sim) return true;
                }

                return base.UsesSim(sim);
            }


            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            public static bool Perform(StoryProgressionObject manager, IScoringGenerator stats, SimDescription sim, Sim horse, RemountScenario remount)
            {
                if (ManagerSituation.HasInteraction(sim, Sims3.Gameplay.Actors.Sim.Mount.Singleton)) return false;

                return manager.Situations.PushInteraction<Sim>(stats, sim, horse, new ManagerSituation.WithCallbackPush(manager, Sims3.Gameplay.Actors.Sim.Mount.Singleton, null, null, remount));
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                mMountAttempts++;
                if (mMountAttempts > 10) return false;

                IncStat("Try Mount Again");

                return Perform(Manager, this, Sim, mHorse, this);
            }

            public override Scenario Clone()
            {
                return new RemountScenario(this);
            }
        }

        public class HorseMountedScenario : SimEventScenario<Event>
        {
            Sim mHorse;

            int mPushes = 0;

            public HorseMountedScenario(SimDescription sim, Sim horse, int pushes)
                : base(sim)
            {
                mHorse = horse;
                mPushes = pushes;
            }
            protected HorseMountedScenario(HorseMountedScenario scenario)
                : base(scenario)
            {
                mHorse = scenario.mHorse;
                mPushes = scenario.mPushes;
            }

            public override bool SetupListener(IEventHandler events)
            {
                events.AddListener(this, EventTypeId.kSimMountsHorse);
                return true;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "RidingHorseMounted";
                }
                else
                {
                    return "Riding";
                }
            }

            protected override bool AllowActive
            {
                get { return true; }
            }

            protected override bool Progressed
            {
                get { return true; }
            }

            protected override ListenHandleType HandleImmediately
            {
                get { return ListenHandleType.Task; }
            }

            protected override bool ShouldReport
            {
                get
                {
                    if (mPushes > 1) return false;

                    return RandomUtil.RandomChance(25);
                }
            }

            protected override Scenario Handle(Event e, ref ListenerAction result)
            {
                if (Sim != GetEventSim(e)) return null;

                Scenario scenario = base.Handle(e, ref result);

                result = ListenerAction.Remove;

                return scenario;
            }

            protected override bool UsesSim(ulong sim)
            {
                if ((mHorse != null) && (mHorse.SimDescription != null))
                {
                    if (mHorse.SimDescription.SimDescriptionId == sim) return true;
                }

                return base.UsesSim(sim);
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                mPushes++;

                if (Sim.CreatedSim == null)
                {
                    IncStat("Hibernating");
                    return false;
                }
                else if (Sim.CreatedSim.InteractionQueue == null)
                {
                    IncStat("No Queue");
                    return false;
                }

                if (mPushes <= 1)
                {
                    Sim.CreatedSim.InteractionQueue.CancelAllInteractions();
                }

                bool maxSkill = true;

                List<SkillNames> practiceSkills = new List<SkillNames>();

                RidingSkill riding = Sim.SkillManager.GetSkill<RidingSkill>(SkillNames.Riding);
                if ((riding == null) || (!riding.ReachedMaxLevel()))
                {
                    maxSkill = false;

                    practiceSkills.Add(SkillNames.Riding);
                }

                Racing racing = mHorse.SkillManager.GetSkill<Racing>(SkillNames.Racing);
                if ((racing == null) || (!racing.ReachedMaxLevel()))
                {
                    maxSkill = false;

                    practiceSkills.Add(SkillNames.Racing);
                }

                Jumping jumping = mHorse.SkillManager.GetSkill<Jumping>(SkillNames.Jumping);
                if ((jumping == null) || (!jumping.ReachedMaxLevel()))
                {
                    maxSkill = false;

                    practiceSkills.Add(SkillNames.Jumping);
                }

                List<EquestrianCenter> lotCenters = new List<EquestrianCenter>();
                List<EquestrianCenter> centers = new List<EquestrianCenter>();
                foreach (EquestrianCenter hole in Sims3.Gameplay.Queries.GetObjects<EquestrianCenter>())
                {
                    if (Sim.CreatedSim.LotCurrent == hole.LotCurrent)
                    {
                        lotCenters.Add(hole);
                    }

                    centers.Add(hole);
                }

                if (lotCenters.Count > 0)
                {
                    centers = lotCenters;
                }

                EquestrianCenter center = null;

                if (centers.Count > 0)
                {
                    center = RandomUtil.GetRandomObjectFromList(centers);
                }
                else
                {
                    IncStat("No Centers");
                }

                if (center != null)
                {
                    CompetitionType type = CompetitionType.Racing;

                    bool canCompete = (maxSkill) || (RandomUtil.CoinFlip());

                    if (canCompete)
                    {
                        List<CompetitionType> types = new List<CompetitionType>(sTypes);

                        while (types.Count > 0)
                        {
                            type = RandomUtil.GetRandomObjectFromList(types);
                            types.Remove(type);

                            canCompete = false;

                            switch (type)
                            {
                                case CompetitionType.CrossCountry:
                                    if (RidingSkill.CanEnterCrossCountryCompetition(Sim))
                                    {
                                        canCompete = true;
                                    }
                                    break;
                                case CompetitionType.Jumping:
                                    if (!RidingSkill.CanEnterJumpingCompetition(Sim))
                                    {
                                        canCompete = true;
                                    }
                                    break;
                                case CompetitionType.Racing:
                                    if (!RidingSkill.CanEnterRacingCompetition(Sim))
                                    {
                                        canCompete = true;
                                    }
                                    break;
                            }

                            if (canCompete)
                            {
                                break;
                            }
                        }
                    }

                    if (canCompete)
                    {
                        List<CompetitionLevel> levels = new List<CompetitionLevel>();

                        foreach (CompetitionLevel level in sLevels)
                        {
                            GreyedOutTooltipCallback callback = null;
                            if (EquestrianCenter.EnterEquestrianCompetition.CanEnterCompetition(type, level, Sim.CreatedSim, false, ref callback))
                            {
                                levels.Add(level);
                            }
                        }

                        if (levels.Count == 0)
                        {
                            IncStat("No Levels");
                        }
                        else
                        {
                            CompetitionLevel level = RandomUtil.GetRandomObjectFromList(levels);

                            if (Common.kDebugging)
                            {
                                Common.DebugNotify("Compete " + type + " " + level, Sim.CreatedSim);
                            }

                            if (!Situations.PushVisit(this, Sim, center.LotCurrent))
                            {
                                IncStat("Visit Fail");
                                return false;
                            }
                            else if (Situations.PushInteraction<EquestrianCenter>(this, Sim, center, new EnterEquestrianCompetitionEx.Definition(type, level)))
                            {
                                return true;
                            }
                            else
                            {
                                IncStat("Competition Fail");
                            }
                        }
                    }
                }

                if (practiceSkills.Count == 0)
                {
                    IncStat("No Practice Skills");
                    return false;
                }

                bool practiceJumping = true;

                if ((riding != null) & (riding.SkillLevel >= RidingSkill.kLevelForTrainForRacing))
                {
                    switch (RandomUtil.GetRandomObjectFromList(practiceSkills))
                    {
                        case SkillNames.Riding:
                            practiceJumping = RandomUtil.CoinFlip();
                            break;
                        case SkillNames.Racing:
                            practiceJumping = false;
                            break;
                    }
                }

                if (!practiceJumping)
                {
                    List<TrainingPosts> lotPosts = new List<TrainingPosts>();
                    List<TrainingPosts> posts = new List<TrainingPosts>();
                    foreach(TrainingPosts post in Sims3.Gameplay.Queries.GetObjects<TrainingPosts>())
                    {
                        if (Sim.CreatedSim.LotCurrent == post.LotCurrent)
                        {
                            lotPosts.Add(post);
                        }

                        posts.Add(post);
                    }

                    if (lotPosts.Count > 0)
                    {
                        posts = lotPosts;
                    }

                    if (posts.Count > 0)
                    {
                        TrainingPosts choice = RandomUtil.GetRandomObjectFromList(posts);

                        if (!Situations.PushVisit(this, Sim, choice.LotCurrent))
                        {
                            IncStat("Visit Fail");
                            return false;
                        }
                        else if (Situations.PushInteraction<TrainingPosts>(this, Sim, choice, new ManagerSituation.WithCallbackPush(Manager, TrainingPosts.RiderTrainForRacing.Singleton, null, null, new MountScenario(Sim, mPushes))))
                        {
                            return true;
                        }
                        else
                        {
                            IncStat("Traning Fail");
                        }
                    }
                }

                int ridingSkill = 0;
                if (riding != null)
                {
                    ridingSkill = riding.SkillLevel;
                }

                List<HorseJump> lotJumps = new List<HorseJump>();
                List<HorseJump> jumps = new List<HorseJump>();
                foreach (HorseJump jump in Sims3.Gameplay.Queries.GetObjects<HorseJump>())
                {
                    if (jump.LotCurrent == null) continue;

                    if (!jump.LotCurrent.IsCommunityLot)
                    {
                        if (!jump.LotCurrent.CanSimTreatAsHome(Sim.CreatedSim)) continue;
                    }

                    if (jump.JumpTuning == null) continue;

                    if (jump.JumpTuning.HighSuccessChances[ridingSkill] < 0.75) continue;

                    if (Sim.CreatedSim.LotCurrent == jump.LotCurrent)
                    {
                        lotJumps.Add(jump);
                    }

                    jumps.Add(jump);
                }

                if (lotJumps.Count > 0)
                {
                    jumps = lotJumps;
                }

                if (jumps.Count > 0)
                {
                    HorseJump choice = RandomUtil.GetRandomObjectFromList(jumps);

                    if (!Situations.PushVisit(this, Sim, choice.LotCurrent))
                    {
                        IncStat("Visit Fail");
                        return false;
                    }
                    else if (Situations.PushInteraction<HorseJump>(this, Sim, choice, new ManagerSituation.WithCallbackPush(Manager, HorseJump.Jump.Singleton, null, null, new MountScenario(Sim, mPushes))))
                    {
                        return true;
                    }
                    else
                    {
                        IncStat("Jump Fail");
                        return false;
                    }
                }
                else
                {
                    IncStat("No Jump Choices");
                    return false;
                }
            }

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (parameters == null)
                {
                    parameters = new object[] { Sim, mHorse };
                }

                return base.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new HorseMountedScenario(this);
            }
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new RidingPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, RidingPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RiderPush";
            }
        }
    }
}
