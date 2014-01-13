using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Skills;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.Settings;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class SkillControl
    {
        static Tracer sTracer = new Tracer();

        protected static bool OnPrivateAllow(Common.IStatGenerator stats, SimData settings, Managers.Manager.AllowCheck check)
        {
            if (!settings.GetValue<AllowSkillOption, bool>())
            {
                stats.IncStat("Allow: Skills Denied");
                return false;
            }
            else if (!settings.GetValue<AllowPushSkillOption, bool>())
            {
                stats.IncStat("Allow: Push Denied");
                return false;
            }

            return true;
        }

        protected static bool OnAllowSkill(Common.IStatGenerator stats, SimData settings, SkillNames skill)
        {
            if (settings.HasValue<DisallowSkillOption, SkillNames>(skill))
            {
                stats.IncStat("Allow: " + skill + " Denied");
                return false;
            }
            else if (settings.GetValue<AllowOnlyExistingSkillOption, bool>())
            {
                if (settings.SimDescription.SkillManager.GetSkillLevel(skill) < 1)
                {
                    stats.IncStat("Allow: " + skill + " Only Existing Denied");
                    return false;
                }
            }

            return true;
        }

        protected static void OnPerform(Scenario paramScenario, ScenarioFrame frame)
        {
            LeveledScenario scenario = paramScenario as LeveledScenario;

            if (!scenario.Main.SecondCycle)
            {
                scenario.IncStat("First Cycle");
                return;
            }

            sTracer.Initialize(scenario, scenario.Sim);

            sTracer.Perform();

            if ((!sTracer.mIgnore) || (scenario.DebuggingLevel >= Common.DebugLevel.High))
            {
                Common.DebugNotify("Skillup Allow" + Common.NewLine + scenario.Sim.FullName + Common.NewLine + scenario.Event.Guid, scenario.Sim.CreatedSim);

                Common.DebugWriteLog("Sim: " + scenario.Sim.FullName + Common.NewLine + "Skill: " + scenario.Event.Guid + Common.NewLine + sTracer.ToString());
            }

            // In order to bypass a script error involving Skill:DisplayTextForLevel() we must delay resetting
            //   until after the calling routine is complete
            new DelayedResetTask(scenario, sTracer.mAllow, sTracer.mManual);
        }

        public class Option : BooleanManagerOptionItem<ManagerSkill>, IDebuggingOption
        {
            DefaultSkillTuning sDefaultSkillTuning = null;

            public Option()
                : base(true)
            { }

            public override bool Install(ManagerSkill main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                main.OnPrivateAllow += OnPrivateAllow;
                main.OnAllowSkill += OnAllowSkill;

                if (initial)
                {
                    sDefaultSkillTuning = new DefaultSkillTuning();

                    CelebrityManager.kSkillLevelForForceAddSkill = new int[] { 0, 0 };

                    SimDescription.kTeenHomeworkSkillStartLevel = 0;

                    LeveledScenario.OnPerform += OnPerform;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "SkillControl";
            }
        }

        protected class DelayedResetTask : Common.AlarmTask
        {
            LeveledScenario mScenario;

            bool mTracerAllow;

            bool mTracerManual;

            public DelayedResetTask(LeveledScenario scenario, bool tracerAllow, bool tracerManual)
                : base(1, TimeUnit.Seconds)
            {
                mScenario = scenario;
                mTracerAllow = tracerAllow;
                mTracerManual = tracerManual;
            }

            protected override void OnPerform()
            {
                bool allow = mTracerAllow;
                if (allow)
                {
                    if (!mTracerManual)
                    {
                        if (!mScenario.GetValue<AllowSkillOption,bool>(mScenario.Sim))
                        {
                            mScenario.IncStat("Allow Skill Denied");

                            allow = false;
                        }
                        else if ((mScenario.GetValue<AllowOnlyExistingSkillOption, bool>(mScenario.Sim)) && (mScenario.Sim.SkillManager != null) && (mScenario.Sim.SkillManager.GetSkillLevel(mScenario.Event.Guid) <= 1))
                        {
                            mScenario.IncStat("Existing Skill Denied");

                            allow = false;
                        }
                    }
                }

                SkillLevelSimData data = mScenario.GetData<SkillLevelSimData>(mScenario.Sim);

                if (!allow)
                {
                    mScenario.IncStat("Not Allowed");

                    data.Adjust(mScenario, mScenario.Event.Guid);
                }
                else
                {
                    mScenario.IncStat("Allowed");

                    data.UpdateSkill(mScenario.Event.Guid);

                    ManagerSim.TestInactiveLTWTask.Perform(mScenario, mScenario.Sim, mScenario.Event);

                    if (mScenario.Sim.SkillManager != null)
                    {
                        Skill skill = mScenario.Sim.SkillManager.GetElement(mScenario.Event.Guid);
                        if (skill != null)
                        {
                            if (skill.IsHiddenSkill())
                            {
                                mScenario.IncStat("Hidden Skill");
                            }
                            else
                            {
                                int celebrity = mScenario.GetValue<CelebrityBySkillOption, int>();
                                if (celebrity > 0)
                                {
                                    mScenario.Friends.AccumulateCelebrity(mScenario.Sim, celebrity * mScenario.SkillLevel);
                                }

                                mScenario.Scenarios.Post(new LeveledScenario.StoryScenario(mScenario.Sim, mScenario.Event.Guid, mScenario.SkillLevel), mScenario.Skills, false);
                            }
                        }
                    }
                }
            }
        }

        protected class DefaultSkillTuning
        {
            int[] kTeenNewSkills = null;
            int[] kTeenExistingSkills = null;
            int[] kYoungAdultNewSkills = null;
            int[] kYoungAdultExistingSkills = null;
            int[] kAdultNewSkills = null;
            int[] kAdultExistingSkills = null;
            int[] kElderNewSkills = null;
            int[] kElderExistingSkills = null;

            SkillNames[] kNPCCatGrantedSkills = null;
            int[] kAdultCatNewSkills = null;
            int[] kAdultCatExistingSkills = null;

            SkillNames[] kNPCDogGrantedSkills = null;
            int[] kAdultDogNewSkills = null;
            int[] kAdultDogExistingSkills = null;

            SkillNames[] kNPCHorseGrantedSkills = null;
            int[] kAdultHorseNewSkills = null;
            int[] kAdultHorseExistingSkills = null;

            int[] kSkillLevelForForceAddSkill = null;

            public DefaultSkillTuning()
            {
                kTeenNewSkills = AgingManager.kTeenNewSkills.Clone() as int[];
                kTeenExistingSkills = AgingManager.kTeenExistingSkills.Clone() as int[];
                kYoungAdultExistingSkills = AgingManager.kYoungAdultExistingSkills.Clone() as int[];
                kYoungAdultNewSkills = AgingManager.kYoungAdultNewSkills.Clone() as int[];
                kAdultExistingSkills = AgingManager.kAdultExistingSkills.Clone() as int[];
                kAdultNewSkills = AgingManager.kAdultNewSkills.Clone() as int[];
                kElderNewSkills = AgingManager.kElderNewSkills.Clone() as int[];
                kElderExistingSkills = AgingManager.kElderExistingSkills.Clone() as int[];

                kNPCDogGrantedSkills = AgingManager.kNPCDogGrantedSkills.Clone() as SkillNames[];
                kAdultDogNewSkills = AgingManager.kAdultDogNewSkills.Clone() as int[];
                kAdultDogExistingSkills = AgingManager.kAdultDogExistingSkills.Clone() as int[];

                kNPCCatGrantedSkills = AgingManager.kNPCCatGrantedSkills.Clone() as SkillNames[];
                kAdultCatNewSkills = AgingManager.kAdultCatNewSkills.Clone() as int[];
                kAdultCatExistingSkills = AgingManager.kAdultCatExistingSkills.Clone() as int[];

                kNPCHorseGrantedSkills = AgingManager.kNPCHorseGrantedSkills.Clone() as SkillNames[];
                kAdultHorseNewSkills = AgingManager.kAdultHorseNewSkills.Clone() as int[];
                kAdultHorseExistingSkills = AgingManager.kAdultHorseExistingSkills.Clone() as int[];

                kSkillLevelForForceAddSkill = CelebrityManager.kSkillLevelForForceAddSkill;

                Wipe();
            }

            protected static void Wipe()
            {
                AgingManager.kTeenNewSkills = new int[0x0];
                AgingManager.kTeenExistingSkills = new int[0x0];
                AgingManager.kYoungAdultExistingSkills = new int[0x0];
                AgingManager.kYoungAdultNewSkills = new int[0x0];
                AgingManager.kAdultExistingSkills = new int[0x0];
                AgingManager.kAdultNewSkills = new int[0x0];
                AgingManager.kElderNewSkills = new int[0x0];
                AgingManager.kElderExistingSkills = new int[0x0];

                AgingManager.kNPCCatGrantedSkills = new SkillNames[0x0];
                AgingManager.kAdultCatNewSkills = new int[0x0];
                AgingManager.kAdultCatExistingSkills = new int[0x0];

                AgingManager.kNPCDogGrantedSkills = new SkillNames[0x0];
                AgingManager.kAdultDogNewSkills = new int[0x0];
                AgingManager.kAdultDogExistingSkills = new int[0x0];

                AgingManager.kNPCHorseGrantedSkills = new SkillNames[0x0];
                AgingManager.kAdultHorseNewSkills = new int[0x0];
                AgingManager.kAdultHorseExistingSkills = new int[0x0];

                CelebrityManager.kSkillLevelForForceAddSkill = new int[2] { 0, 0 };
            }

            public void Restore()
            {
                AgingManager.kTeenNewSkills = kTeenNewSkills.Clone() as int[];
                AgingManager.kTeenExistingSkills = kTeenExistingSkills.Clone() as int[];
                AgingManager.kYoungAdultExistingSkills = kYoungAdultExistingSkills.Clone() as int[];
                AgingManager.kYoungAdultNewSkills = kYoungAdultNewSkills.Clone() as int[];
                AgingManager.kAdultExistingSkills = kAdultExistingSkills.Clone() as int[];
                AgingManager.kAdultNewSkills = kAdultNewSkills.Clone() as int[];
                AgingManager.kElderNewSkills = kElderNewSkills.Clone() as int[];
                AgingManager.kElderExistingSkills = kElderExistingSkills.Clone() as int[];

                AgingManager.kNPCCatGrantedSkills = kNPCCatGrantedSkills.Clone() as SkillNames[];
                AgingManager.kAdultCatNewSkills = kAdultCatNewSkills.Clone() as int[];
                AgingManager.kAdultCatExistingSkills = kAdultCatExistingSkills.Clone() as int[];

                AgingManager.kNPCDogGrantedSkills = kNPCDogGrantedSkills.Clone() as SkillNames[];
                AgingManager.kAdultDogNewSkills = kAdultDogNewSkills.Clone() as int[];
                AgingManager.kAdultDogExistingSkills = kAdultDogExistingSkills.Clone() as int[];

                AgingManager.kNPCHorseGrantedSkills = kNPCHorseGrantedSkills.Clone() as SkillNames[];
                AgingManager.kAdultHorseNewSkills = kAdultHorseNewSkills.Clone() as int[];
                AgingManager.kAdultHorseExistingSkills = kAdultHorseExistingSkills.Clone() as int[];

                CelebrityManager.kSkillLevelForForceAddSkill = kSkillLevelForForceAddSkill;
            }
        }

        public class Tracer : StackTracer
        {
            Common.IStatGenerator mStats;

            SimDescription mSim;

            public bool mAllow;

            public bool mManual;

            public bool mIgnore;

            public Tracer()
            {
                // Must be ahead of "ForceSkillLevelUp"
                AddTest(new MasterControllerFrame(OnAllowManual));
                AddTest(typeof(Sims3.Gameplay.Objects.HobbiesSkills.BrainEnhancingMachine.BrainEnhancingMachine.BrainEnhancement), "Boolean Run", OnIgnore);

                AddTest("NRaas.StoryProgressionSpace.Scenarios.Lots.ImmigrantCareerScenario", "Boolean PrivateUpdate", OnImmigrant);

                AddTest(typeof(SkillManager), "Void SetTakenClass", OnIgnore); // Must be ahead of "AddAutomaticSkill"
                AddTest(typeof(SkillManager), "Void AddAutomaticSkill", OnResidentOrHomeless);

                AddTest(typeof(Sims3.Gameplay.Roles.Role), "Role CreateRole", OnResidentOrHomeless);

                AddTest(typeof(Skill), "Boolean ForceSkillLevelUp", OnDisallow);

                AddTest(typeof(Sims3.Gameplay.ObjectComponents.RepairableComponent), "Void StopRepair", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.ObjectComponents.RepairableComponent), "Boolean StartRepair", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Socializing.Conversation), "Void UpdateSimBeforeInteraction", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Objects.Gardening.HarvestPlant), "Boolean DoHarvest", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Objects.Gardening.Plant.WeedPlant), "Boolean DoWeed", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Objects.Gardening.Plant.WaterPlant), "Boolean DoWater", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Skills.Fishing), "String CaughtFish", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Skills.Cooking), "Void SimMadeRecipe", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Interactions.InteractionInstance), "Void EndSkillUpdate", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Situations.FieldTripSituation.RabbitHoleFieldTrip), "Boolean InRabbitHole", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Skills.Fishing), "System.String CaughtFish", OnIgnore);
                AddTest(typeof(SkillManager), "Void UpdateSkills", OnIgnore);
                AddTest(typeof(Computer.WriteNovel), "Boolean Run", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Objects.HobbiesSkills.Shuffleboard.Play), "Void FinalizePuckPlacement", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.ActorSystems.OccultManager), "Boolean AddOccultType", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Objects.Vehicles.MagicBroom), "Void OnRouteFinished", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Skills.Photography), "Void OnPhotoTakenPreInstantiate", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Objects.Vehicles.MagicBroom), "Void OnRouteFinished", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.ObjectComponents.MaintenanceComponent.MaintainObject), "Boolean Run()", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Objects.SkatingRink.Skate), "Void SkatingRouteCallback", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Objects.SkatingRink.Skate), "Void UpdateSkillAndMotivesDuringSpin", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Abstracts.AthleticGameObject.WorkOut), "Void ConfigureInteraction", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.ActiveCareer.ActiveCareers.PerformanceCareer.PerformerPerformForTips), "Void PerformForTipsXP", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Careers.CareerTone), "Void BeginTone", OnIgnore);

                AddTest("NRaas.WoohooerSpace.Skills.KamaSimtra", "Void AddNotch", OnIgnore);
                AddTest("NRaas.StoryProgressionSpace.Scenarios.Skills.TeachScenario", "Boolean PrivateUpdate", OnIgnore);
                AddTest("NRaas.StoryProgressionSpace.Interactions.WriteNovelEx", "Boolean Run", OnIgnore);
                AddTest("NRaas.StoryProgressionSpace.Interactions.EnterEquestrianCompetitionEx", "Void ShowCompetitionEndScoreEx", OnIgnore);
                AddTest("NRaas.StoryProgressionSpace.Scenarios.Skills.PhotogPushScenario", "Boolean PrivateUpdate", OnIgnore);
                AddTest("NRaas.StoryProgressionSpace.Interactions.HarvestEx", "Boolean Run", OnIgnore);
            }

            public void Initialize(Common.IStatGenerator stats, SimDescription sim)
            {
                mStats = stats;
                mSim = sim;
            }

            public override void Reset()
            {
                mAllow = true;
                mManual = false;
                mIgnore = false;

                base.Reset();
            }

            private bool OnIgnore(StackTrace trace, StackFrame frame)
            {
                mStats.IncStat("Other Allowed");

                mAllow = true;
                mIgnore = true;
                return true;
            }

            private bool OnDisallow(StackTrace trace, StackFrame frame)
            {
                mStats.IncStat("ForceSkillLevelUp Denied");

                mAllow = false;
                mIgnore = true;
                return true;
            }

            private bool OnResidentOrHomeless(StackTrace trace, StackFrame frame)
            {
                if (mSim.LotHome != null)
                {
                    mStats.IncStat("Resident Denied");

                    mAllow = false;
                }
                else
                {
                    mStats.IncStat("Homeless Allowed");

                    mAllow = true;
                }

                mIgnore = true;
                return true;
            }

            private bool OnAllowManual(StackTrace trace, StackFrame frame)
            {
                mStats.IncStat("MasterController Allowed");

                mAllow = true;
                mManual = true;
                mIgnore = true;
                return true;
            }

            private bool OnImmigrant(StackTrace trace, StackFrame frame)
            {
                mStats.IncStat("Immigrant Allowed");

                mAllow = true;
                mIgnore = true;
                return true;
            }

            public class MasterControllerFrame : StackFrameBase
            {
                public MasterControllerFrame(OnMatch func)
                    : base("", func)
                { }

                public override string TypeName
                {
                    get { return ""; }
                }

                public override bool Test(StackFrame frame)
                {
                    return frame.GetMethod().DeclaringType.FullName.Contains("MasterControllerSpace");
                }
            }
        }

        public class CelebrityBySkillOption : IntegerManagerOptionItem<ManagerSkill>
        {
            public CelebrityBySkillOption()
                : base(50)
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityBySkill";
            }

            public override bool Install(ManagerSkill manager, bool initial)
            {
                TestSim.OnPerform += OnPerform;

                return base.Install(manager, initial);
            }

            protected static void OnPerform(Sim target)
            {
                Common.Notify("Allow Push: " + StoryProgression.Main.GetValue<AllowPushSkillOption, bool>(target));
            }
        }
    }
}
