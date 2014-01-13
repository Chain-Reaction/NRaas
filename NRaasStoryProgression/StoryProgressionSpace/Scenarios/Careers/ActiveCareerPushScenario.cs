using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class ActiveCareerPushScenario : OccupationPushScenario
    {
        public ActiveCareerPushScenario(SimDescription sim)
            : base(sim)
        { }
        protected ActiveCareerPushScenario(ActiveCareerPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ActiveCareerPush";
        }

        public override Occupation Occupation
        {
            get 
            { 
                return Sim.OccupationAsActiveCareer; 
            }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<ActiveCareerOption, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("Careers Denied");
                return false;
            }
            else if (AddScoring("ActiveAmbition", sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PostSlackerWarning()
        {
            return Situations.AddWorkSlacker(Sim);
        }

        protected bool IsValidJobId(JobId id)
        {
            switch (id)
            {
                case JobId.SpiritInvasion:
                //case JobId.GenericRabbitholeRescue:
                //case JobId.EpicCriminalRescue:
                //case JobId.EpicHospitalRescue:
                //case JobId.EpicScienceRescue:
                case JobId.SmallFires:
                    return true;
                case JobId.RabbitHoleInvestigation:
                    return GetValue<AllowGhostRabbitholeOption,bool>();
            }

            return false;
        }

        protected Job GetExistingJob(Occupation occupation)
        {
            if (occupation.Jobs == null) return null;

            Job currentJob = occupation.CurrentJob;
            if (currentJob != null)
            {
                if (IsValidJobId(currentJob.Id))
                {
                    return currentJob;
                }
            }

            foreach (Job job in occupation.Jobs)
            {
                if (IsValidJobId(job.Id))
                {
                    return job;
                }
            }

            return null;
        }

        public bool TryCreateJobForCustomer(Occupation ths, JobCreationSpec jobCreationSpec, OccupationLevelStaticData levelStaticData, out Job job)
        {
            job = null;
            jobCreationSpec.mJobDestinationType = jobCreationSpec.JobStaticData.DestinationType;
            jobCreationSpec.mCustomerType = JobCustomerType.Random;
            if ((jobCreationSpec.mLot == null) || !ths.TryCreateJob(jobCreationSpec, levelStaticData, out job))
            {
                IncStat("CreateJob Fail");
                return false;
            }

            DateAndTime date = SimClock.Add(SimClock.CurrentTime(), TimeUnit.Hours, 0);

            Occupation.MarkJobForCooldown(job, date);
            return true;
        }

        protected bool ProgressPrivateEye(Occupation occupation)
        {
            if (!Test(Sim.CreatedSim, PoliceStation.LowLevelPoliceWork.Singleton))
            {
                PoliceStation.LowLevelPoliceWork interaction = Sim.CreatedSim.InteractionQueue.GetCurrentInteraction() as PoliceStation.LowLevelPoliceWork;
                if (interaction != null)
                {
                    if (interaction.CurrentTone == null)
                    {
                        List<ITone> allTones = new List<ITone>();

                        foreach (InteractionToneDisplay tone in interaction.AvailableTonesForDisplay())
                        {
                            allTones.Add(tone.InteractionTone);
                        }

                        string name = null;
                        if (CareerToneScenario.SetTone(interaction, allTones, ref name))
                        {
                            IncStat("Private Eye Tone");
                        }
                    }
                }

                return true;
            };

            RabbitHole hole = ManagerSituation.FindRabbitHole(RabbitHoleType.PoliceStation);
            if (hole == null)
            {
                IncStat("No Police Station");
                return false;
            }

            return Situations.PushInteraction<RabbitHole>(this, Sim, hole, PoliceStation.LowLevelPoliceWork.Singleton);
        }

        protected bool ProgressJobBased(ActiveCareer occupation)
        {
            List<InteractionDefinition> interactions = new List<InteractionDefinition>();

            Job job = GetExistingJob(occupation);
            if (job != null)
            {
                InteractionDefinition definition;
                GameObject target;
                job.GetInteractionDefinitionAndTargetToGoToWork(out definition, out target);

                interactions.Add(definition);
            }

            if (!Test(Sim.CreatedSim, interactions))
            {
                return false;
            }

            if (occupation.HasJobAtLot(Sim.CreatedSim.LotCurrent))
            {
                IncStat("Already On Site");
                return false;
            }

            if (job == null)
            {
                List<JobId> choices = new List<JobId>();

                foreach (JobId id in occupation.GetCurrentLevelStaticData().RandomCustomerJobIds)
                {
                    if (IsValidJobId(id))
                    {
                        choices.Add(id);
                    }
                }

                if (choices.Count == 0)
                {
                    IncStat("No Choices");
                    return false;
                }

                JobId jobId = RandomUtil.GetRandomObjectFromList(choices);

                JobCreationSpec jobCreationSpec;
                if (!occupation.TryCreateJobCreationSpec(jobId, out jobCreationSpec))
                {
                    IncStat("Creation Fail");
                    return false;
                }

                if (!GetValue<AllowGhostHunterOnActiveOptionV2, bool>())
                {
                    if ((Household.ActiveHousehold == null) || (jobCreationSpec.mLot.CanSimTreatAsHome(Sims3.Gameplay.Actors.Sim.ActiveActor)))
                    {
                        IncStat("Active Lot Fail");
                        return false;
                    }
                }

                jobCreationSpec.mStartTime = SimClock.CurrentTime();

                if (!TryCreateJobForCustomer(occupation, jobCreationSpec, occupation.GetCurrentLevelStaticData(), out job))
                {
                    IncStat("CreateJob Fail");
                    return false;
                }

                if (job == null)
                {
                    IncStat("No Job");
                    return false;
                }

                job.SetTimeoutAlarm((float)job.Specification.mHoursAvailable);
            }

            job.MapTagEnabled = true;

            job.SetCurrentJobLocation();

            if (!Situations.PushInteraction(this, Sim, Managers.Manager.AllowCheck.None, occupation.CreateGoToActiveCareerJobInteraction(job)))
            {
                IncStat("Push Fail");
                return false;
            }

            IncStat("Pushed");
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Occupation occupation = Occupation;

            if (occupation is PrivateEye)
            {
                return ProgressPrivateEye(occupation);
            }
            else if (occupation is Stylist)
            {
                if (!Test(Sim.CreatedSim, DraftingTable.Research.SingletonStylist))
                {
                    return false;
                }

                Add(frame, new DraftingPushScenario(Sim), ScenarioResult.Start);
                return true;
            }
            else if (occupation is InteriorDesigner)
            {
                if (!Test(Sim.CreatedSim, DraftingTable.Research.SingletonStylist))
                {
                    return false;
                }

                Add(frame, new DraftingPushScenario(Sim), ScenarioResult.Start);
                return true;
            }
            else if (occupation is GhostHunter) 
            {
                return ProgressJobBased(occupation as ActiveCareer);
            }
            else if (occupation is ActiveFireFighter)
            {
                if (Sim.CreatedSim.GetSituationOfType<FirefighterSituation>() != null)
                {
                    IncStat("In Situation");
                    return true;
                }

                if ((Household.ActiveHousehold == null) || (Household.ActiveHousehold.LotHome == null))
                {
                    IncStat("No Active Household");
                    return true;
                }

                if (occupation.ActiveCareerLotID != Sim.CreatedSim.LotCurrent.LotId)
                {
                    return Situations.PushInteraction(this, Sim, Managers.Manager.AllowCheck.None, occupation.GetInteractionToTakeSimToWork());
                }

                if (Sim.CreatedSim.InteractionQueue.HasInteractionOfType(AthleticGameObject.WorkOut.Singleton))
                {
                    IncStat("Relax");

                    Sim.CreatedSim.InteractionQueue.CancelAllInteractions();
                    return true;
                }

                bool activeFirefighter = false;

                if (!GetValue<AllowInactiveFireRepairOption, bool>())
                {
                    foreach (Sim sim in HouseholdsEx.AllSims(Household.ActiveHousehold))
                    {
                        if (sim.Occupation is ActiveFireFighter)
                        {
                            activeFirefighter = true;
                            break;
                        }
                    }
                }

                if ((!activeFirefighter) && (RandomUtil.CoinFlip()))
                {
                    List<GameObject> objs = new List<GameObject>();

                    foreach (GameObject obj in Sim.CreatedSim.LotCurrent.GetObjects<GameObject>())
                    {
                        if (obj.MaintenenceComponent == null) continue;

                        objs.Add(obj);
                    }

                    if (objs.Count == 0)
                    {
                        IncStat("No Maintain Objects");
                        return false;
                    }

                    IncStat("Maintain");

                    GameObject choice = RandomUtil.GetRandomObjectFromList(objs);

                    choice.MaintenenceComponent.MaintenanceLevel = 0;

                    return Situations.PushInteraction<GameObject>(this, Sim, choice, MaintenanceComponent.MaintainObject.Singleton);
                }
                else
                {
                    List<AthleticGameObject> objs = new List<AthleticGameObject>();

                    foreach (AthleticGameObject obj in Sim.CreatedSim.LotCurrent.GetObjects<AthleticGameObject>())
                    {
                        objs.Add(obj);
                    }

                    if (objs.Count == 0)
                    {
                        IncStat("No Gym Objects");
                        return false;
                    }

                    IncStat("Workout");

                    AthleticGameObject choice = RandomUtil.GetRandomObjectFromList(objs);

                    return Situations.PushInteraction<AthleticGameObject>(this, Sim, choice, AthleticGameObject.WorkOut.Singleton);
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new ActiveCareerPushScenario(this);
        }

        public class ActiveCareerOption : BooleanManagerOptionItem<ManagerSituation>
        {
            public ActiveCareerOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "WorkPushActiveCareers";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP2);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<ScheduledWorkPushScenario.Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class AllowInactiveFireRepairOption : BooleanManagerOptionItem<ManagerSituation>
        {
            public AllowInactiveFireRepairOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowInactiveFireRepair";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP2);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<ActiveCareerOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class AllowGhostRabbitholeOption : BooleanManagerOptionItem<ManagerSituation>
        {
            public AllowGhostRabbitholeOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowGhostRabbithole";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP2);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<ActiveCareerOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class AllowGhostHunterOnActiveOptionV2 : BooleanManagerOptionItem<ManagerSituation>
        {
            public AllowGhostHunterOnActiveOptionV2()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowGhostHunterOnActive";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP2);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<ActiveCareerOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
