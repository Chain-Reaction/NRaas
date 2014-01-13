using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Selection;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public class FindJobScenario : SimScenario
    {
        bool mForce = false;

        bool mCheckQuit = false;

        public FindJobScenario()
        { }
        public FindJobScenario(SimDescription sim, bool force, bool checkQuit)
            : base(sim)
        {
            mForce = force;
            mCheckQuit = checkQuit;
        }
        protected FindJobScenario(FindJobScenario scenario)
            : base (scenario)
        { 
            mForce = scenario.mForce;
            mCheckQuit = scenario.mCheckQuit;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "FindJob" + (mForce ? " Forced" : " Natural");
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            if ((Common.IsOnTrueVacation()) && (!GameUtils.IsUniversityWorld())) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        public override bool ManualSetup(StoryProgressionObject manager)
        {
            //mForce = true;

            return base.ManualSetup(manager);
        }

        protected bool HasProperJob(SimDescription sim)
        {
            if (sim.Occupation == null) return false;

            if (sim.Occupation is AcademicCareer) return true;

            if (!Careers.TestCareer(this, sim, sim.Occupation.Guid))
            {
                return false;
            }

            if (ManagerCareer.IsPlaceholderCareer(sim.Occupation as Career))
            {
                IncStat("Proper Job: Placeholder");
                return true;
            }

            if (sim.Teen)
            {
                IncStat("Proper Job: Teen");
                return true;
            }

            if (!GetValue<AssignDreamJobToEmployedOption, bool>())
            {
                IncStat("Proper Job: Employed Denied");
                return true;
            }

            bool retired = false;
            if (sim.Elder)
            {
                if (sim.Occupation is Retired)
                {
                    retired = true;
                    if (!GetValue<AllowSelfEmployedRetirementOption,bool>())
                    {
                        IncStat("Proper Job: Retired Denied");
                        return true;
                    }
                }
                else
                {
                    IncStat("Proper Job: Elder Not Retired");
                    return true;
                }
            }

            if (sim.HasCompletedLifetimeWish)
            {
                IncStat("Proper Job: Wish Completed");
                return true;
            }
            else
            {
                List<DreamJob> jobs = ManagerCareer.GetDreamJob(sim, retired);
                if (jobs.Count == 0)
                {
                    if (!retired)
                    {
                        if (sim.YoungAdultOrAbove)
                        {
                            Career career = sim.Occupation as Career;
                            if ((career != null) && (career.IsPartTime))
                            {
                                IncStat("Proper Job: No Dream Part-Time");
                                return false;
                            }
                        }

                        IncStat("Proper Job: No Dream Job");
                        return true;
                    }
                    else
                    {
                        IncStat("Proper Job: No Dream Retired");
                        return false;
                    }
                }

                foreach (DreamJob job in jobs)
                {
                    if (job == null) continue;

                    if (job.mCareer == sim.Occupation.Guid)
                    {
                        if (job.Satisfies(Careers, sim, sim.LotHome, false))
                        {
                            IncStat("Proper Job: Already Dream");
                            return true;
                        }
                        else
                        {
                            IncStat("Proper Job: Dream No Satisfy");
                            return false;
                        }
                    }
                }

                AddStat("Proper Job: Default", jobs.Count);
                return false;
            }
        }

        protected override bool Allow(SimDescription sim)
        {
            if ((Common.IsOnTrueVacation()) && (!GameUtils.IsUniversityWorld ()))
            {
                IncStat("Vacation");
                return false;
            }
            else if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.IsEnrolledInBoardingSchool())
            {
                IncStat("Boarding School");
                return false;
            }
            else if (SimTypes.InServicePool(sim))
            {
                IncStat("Service");
                return false;
            }
            else if ((!mForce) && (HasProperJob(sim)))
            {
                IncStat("Has Job");
                return false;
            }
            else if (!Careers.AllowFindJob(this, sim))
            {
                IncStat("Careers Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected bool AllowStandalone(CareerLocation location)
        {
            if (GetValue<AllowStandAloneOption, bool>()) return true;

            RabbitHole owner = location.Owner;
            if ((owner == null) || (owner.CareerLocations == null)) return true;

            foreach (CareerLocation loc in owner.CareerLocations.Values)
            {
                if (loc.Career == null) continue;

                if (!loc.Career.IsPartTime)
                {
                    return true;
                }
            }

            return false;
        }

        protected List<Occupation> GetChoices(bool checkQuit)
        {
            List<Occupation> allCareers = new List<Occupation>();

            foreach (Occupation occupation in CareerManager.OccupationList)
            {
                if (occupation is School) continue;

                if (occupation.IsAcademicCareer) continue;

                if (!Careers.TestCareer(this, Sim, occupation.Guid))
                {
                    continue;
                }

                Career career = occupation as Career;
                if (career != null)
                {
                    if (!career.CareerAgeTest(Sim)) continue;

                    if (career.IsPartTime != Sim.Teen) continue;

                    if (ManagerCareer.IsPlaceholderCareer(career)) continue;

                    if (career.Level1.PayPerHourBase <= 0) continue;

                    CareerLocation location = Career.FindClosestCareerLocation(Sim, career.Guid);
                    if (location == null) continue;

                    if (!AllowStandalone(location)) continue;
                }

                if (checkQuit)
                {
                    Occupation oldCareer;
                    if (Sim.CareerManager.QuitCareers.TryGetValue(occupation.Guid, out oldCareer))
                    {
                        if (oldCareer.CareerLevel >= 5) continue;
                    }
                }

                allCareers.Add(occupation);
            }

            return allCareers;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Sim.AssignedRole != null)
            {
                if (ManagerCareer.IsRegisterInstalled())
                {
                    if ((Sim.LotHome == null) && (Sim.Occupation == null))
                    {
                        IncStat("Registered Retiremenet");

                        GetData<CareerSimData>(Sim).Retire();
                    }
                }

                return true;
            }
            else if (SimTypes.IsSpecial(Sim))
            {
                IncStat("Special");
                return false;
            }

            bool enroll = false;

            if (GameUtils.IsUniversityWorld())
            {
                enroll = true;
            }
            else if (Careers.TestCareer(this, Sim, OccupationNames.AcademicCareer))
            {
                if (Common.AssemblyCheck.IsInstalled("NRaasCareer"))
                {
                    bool success = false;
                    foreach (AdminstrationCenter center in Sims3.Gameplay.Queries.GetObjects<AdminstrationCenter>())
                    {
                        if (GetLotOptions(center.LotCurrent).AllowCastes(this, Sim))
                        {
                            success = true;
                            break;
                        }
                    }

                    if (success)
                    {
                        AcademicDegreeManager degreeManager = Sim.CareerManager.DegreeManager;
                        if (degreeManager != null)
                        {
                            if (!degreeManager.HasCompletedAnyDegree())
                            {
                                enroll = true;
                            }
                        }
                    }
                }
            }

            if (!mForce)
            {
                if (GetValue<ManualCareerOption, bool>(Sim))
                {
                    IncStat("Manual");
                    return false;
                }
            }

            List<OccupationNames> careers = new List<OccupationNames>();
            bool dream = Careers.GetPotentialCareers(this, Sim, careers, mCheckQuit);

            bool partTime = false;
            if (Sim.Teen)
            {
                bool scoreSuccess = true;
                if (mForce)
                {
                    if (AddScoring("FindJob", Sim) < 0)
                    {
                        scoreSuccess = false;
                    }
                }
                else
                {
                    if (AddScoring("ChanceFindJob", Sim) < 0)
                    {
                        scoreSuccess = false;
                    }
                }

                if ((!scoreSuccess) && (!GetValue<ForceTeensOption,bool>()))
                {
                    IncStat("Score Fail");
                    return false;
                }
                partTime = true;
            }

            if (partTime)
            {
                List<OccupationNames> partTimeList = new List<OccupationNames>();

                AddStat(Sim.Age + ": Part-time Choices", careers.Count);

                foreach (OccupationNames career in careers)
                {
                    Career staticCareer = CareerManager.GetStaticCareer(career);
                    if (staticCareer == null) continue;

                    if (staticCareer is School) continue;

                    CareerLocation location = Career.FindClosestCareerLocation(Sim, staticCareer.Guid);
                    if (location == null) continue;

                    if (!AllowStandalone(location)) continue;

                    foreach (CareerLocation loc in location.Owner.CareerLocations.Values)
                    {
                        Career possible = loc.Career;

                        if (!possible.IsPartTime) continue;

                        if (ManagerCareer.IsPlaceholderCareer(possible)) continue;

                        partTimeList.Add(possible.Guid);
                    }
                }

                careers = partTimeList;

                AddStat(Sim.Age + ": Part-time Final", careers.Count);
            }
            else
            {
                AddStat(Sim.Age + ": Full-time Final", careers.Count);
            }

            if ((!mForce) && (!dream) && (Sim.Occupation != null) && (!(Sim.Occupation is Retired)))
            {
                IncStat("Non-Dream Employed");
                return false;
            }

            if (enroll)
            {
                AcademicDegreeNames degreeName = AcademicDegreeNames.Undefined;

                foreach (DreamJob job in ManagerCareer.GetDreamJob(Sim))
                {
                    if (job == null) continue;

                    foreach (AcademicDegreeStaticData data in AcademicDegreeManager.sDictionary.Values)
                    {
                        if (data.AssociatedOccupations.Contains(job.mCareer))
                        {
                            degreeName = data.AcademicDegreeName;
                            break;
                        }
                    }
                }

                if (degreeName == AcademicDegreeNames.Undefined)
                {
                    degreeName = AcademicDegreeManager.ChooseWeightRandomSuitableDegree(Sim);
                }

                if (degreeName != AcademicDegreeNames.Undefined)
                {
                    if (AcademicCareer.GlobalTermLength == AcademicCareer.TermLength.kInvalid)
                    {
                        AcademicCareer.GlobalTermLength = AcademicCareer.TermLength.kOneWeek;
                    }

                    AcademicCareer.EnrollSimInAcademicCareer(Sim, degreeName, AcademicCareer.ChooseRandomCoursesPerDay());
                    return true;
                }
            }

            bool promptForJob = GetValue<ChooseCareerOption, bool>();

            if ((promptForJob) && (!Careers.MatchesAlertLevel(Sim)))
            {
                promptForJob = false;
            }

            if (careers.Count > 0)
            {
                if ((Sim.Occupation != null) && (careers.Contains(Sim.Occupation.Guid)))
                {
                    IncStat("Already Has Choice");
                    return false;
                }

                if (!promptForJob)
                {
                    if (AskForJob(Sim, RandomUtil.GetRandomObjectFromList(careers)))
                    {
                        return true;
                    }
                }
            }

            if ((!mForce) && (Sim.Occupation != null))
            {
                IncStat("Already Employed");
                return false;
            }

            List<Occupation> allCareers = null;

            if (careers.Count > 0)
            {
                allCareers = new List<Occupation>();

                foreach(Career career in CareerManager.CareerList)
                {
                    if (careers.Contains(career.Guid))
                    {
                        allCareers.Add(career);
                    }
                }
            }

            if ((allCareers == null) || (allCareers.Count == 0))
            {
                if (Sim.LifetimeWish == (uint)LifetimeWant.JackOfAllTrades)
                {
                    allCareers = GetChoices(true);
                }
            }

            if ((allCareers == null) || (allCareers.Count == 0))
            {
                allCareers = GetChoices(false);
            }

            if (allCareers.Count > 0)
            {
                AddStat("Random Choices", allCareers.Count);

                if ((promptForJob) && (AcceptCancelDialog.Show(ManagerSim.GetPersonalInfo(Sim, Common.Localize("ChooseCareer:Prompt", Sim.IsFemale)))))
                {
                    List<JobItem> jobs = new List<JobItem>();

                    foreach (Occupation career in GetChoices(false))
                    {
                        jobs.Add(new JobItem(career, allCareers.Contains(career)));
                    }

                    bool okayed;
                    JobItem choice = new CommonSelection<JobItem>(Common.Localize("ChooseCareer:Header", Sim.IsFemale), Sim.FullName, jobs, new JobPreferenceColumn()).SelectSingle(out okayed);
                    if (!okayed) return false;

                    if (choice != null)
                    {
                        allCareers.Clear();
                        allCareers.Add(choice.Value);

                        SetValue<ManualCareerOption, bool>(Sim, true);
                    }
                }

                while (allCareers.Count > 0)
                {
                    Occupation choice = RandomUtil.GetRandomObjectFromList(allCareers);
                    allCareers.Remove(choice);

                    if (choice != null)
                    {
                        if (AskForJob(Sim, choice))
                        {
                            return true;
                        }
                    }
                }

                IncStat("Ask Failure");
                return false;
            }
            else
            {
                if (promptForJob)
                {
                    Common.Notify(Common.Localize("ChooseCareer:PromptFailure", Sim.IsFemale, new object[] { Sim }));
                }

                IncStat("No Applicable");
                return false;
            }
        }

        protected override bool Push()
        {
            if (Sim.Occupation is Retired) return true;

            if ((Sim.Occupation != null) && (Sim.Occupation.CareerLoc != null))
            {
                return Situations.PushToRabbitHole(this, Sim, Sim.Occupation.CareerLoc.Owner, false, false);
            }

            return true;
        }

        protected bool AcquireOccupation(CareerManager ths, AcquireOccupationParameters occupationParameters)
        {
            Occupation occupation;

            Sim createdSim = ths.mSimDescription.CreatedSim;

            CareerLocation location = occupationParameters.Location;
            OccupationNames newJobGuid = occupationParameters.TargetJob;

            if ((ths.mJob != null) && (location != null) && (ths.mJob.Guid == location.Career.Guid) && (ths.mJob.CareerLoc != location))
            {
                Career mJob = ths.mJob as Career;
                if ((mJob != null) && (mJob.TransferBetweenCareerLocations(location, false)))
                {
                    IncStat("Transferred");
                    return true;
                }
            }

            if (!ths.TryGetNewCareer(newJobGuid, out occupation))
            {
                IncStat("TryGetNewCareer Fail");
                return false;
            }

            if (ths.mJob != null)
            {
                if (createdSim != null)
                {
                    EventTracker.SendEvent(new TransferCareerEvent(createdSim, ths.mJob, occupation));
                }
                ths.mJob.LeaveJob(false, Career.LeaveJobReason.kTransfered);
            }

            EventTracker.SendEvent(EventTypeId.kCareerNewJob, createdSim);
            occupation.OwnerDescription = ths.mSimDescription;
            occupation.mDateHired = SimClock.CurrentTime();
            occupation.mAgeWhenJobFirstStarted = ths.mSimDescription.Age;
            occupation.SetAttributesForNewJob(location, occupationParameters.LotId, occupationParameters.CharacterImportRequest);
            EventTracker.SendEvent(new CareerEvent(EventTypeId.kEventCareerHired, occupation));
            EventTracker.SendEvent(new CareerEvent(EventTypeId.kEventCareerChanged, occupation));
            EventTracker.SendEvent(new CareerEvent(EventTypeId.kCareerDataChanged, occupation));
            occupation.RefreshMapTagForOccupation();
            ths.UpdateCareerUI();

            if ((createdSim != null) && createdSim.IsActiveSim)
            {
                HudController.SetInfoState(InfoState.Career);
            }

            IncStat("Job Acquired " + occupation.Guid);
            return true;
        }

        protected bool AskForJob(SimDescription sim, OccupationNames careerName)
        {
            Occupation occupation = CareerManager.GetStaticOccupation(careerName);
            if (occupation == null)
            {
                IncStat("Bad Occupation");
                return false;
            }

            return AskForJob(sim, occupation);
        }
        protected bool AskForJob(SimDescription sim, Occupation occupation)
        {
            if ((sim.Occupation != null) && (sim.Occupation.Guid == occupation.Guid))
            {
                IncStat("Same Job");
                return false;
            }

            Career career = occupation as Career;
            if (career != null)
            {
                if (!career.CareerAgeTest(sim))
                {
                    IncStat("Wrong Age");
                    return false;
                }
            }

            if (!Careers.TestCareer(this, Sim, occupation.Guid))
            {
                IncStat("User Disallow " + occupation.CareerName);
                return false;
            }

            AcquireOccupationParameters parameters;

            if (occupation is Career)
            {
                CareerLocation location = Career.FindClosestCareerLocation(sim, occupation.Guid);
                if (location == null)
                {
                    IncStat("No Location " + occupation.CareerName);
                    return false;
                }
                else if (!AllowStandalone(location))
                {
                    IncStat("Standalone " + occupation.CareerName);
                    return false;
                }
                else
                {
                    parameters = new AcquireOccupationParameters(location, false, false);
                }
            }
            else
            {
                parameters = new AcquireOccupationParameters(occupation.Guid, false, false);
            }

            try
            {
                if (sim.Occupation != null)
                {
                    if (sim.Occupation.Guid == occupation.Guid)
                    {
                        IncStat("Already In Job");
                        return true;
                    }

                    if (sim.Occupation.CareerLoc != null)
                    {
                        foreach (SimDescription worker in sim.Occupation.CareerLoc.Workers)
                        {
                            if (worker.Occupation == null) continue;

                            if (worker.Occupation.Coworkers == null)
                            {
                                worker.Occupation.Coworkers = new List<SimDescription>();
                            }
                        }
                    }

                    sim.Occupation.LeaveJobNow(Career.LeaveJobReason.kQuit);
                }

                Occupation retiredCareer = sim.CareerManager.mRetiredCareer;
                sim.CareerManager.mRetiredCareer = null;

                int originaHighest = 0;
                try
                {
                    if (occupation is ActiveFireFighter)
                    {
                        ActiveCareerStaticData activeCareerStaticData = ActiveCareer.GetActiveCareerStaticData(OccupationNames.Firefighter);

                        originaHighest = activeCareerStaticData.HighestLevel;

                        // Required to bypass auto promotion in SetAttributesForNewJob
                        activeCareerStaticData.HighestLevel = 1;
                    }

                    if (occupation is XpBasedCareer)
                    {
                        // Required by Stylist.GetOccupationJoiningTnsTextPrefix()
                        if (!Sims.Instantiate(sim, sim.LotHome, false))
                        {
                            IncStat("Hibernating");
                            return false;
                        }
                    }

                    if (AcquireOccupation(sim.CareerManager, parameters))
                    {
                        IncStat(occupation.Guid.ToString());
                        return true;
                    }
                }
                finally
                {
                    sim.CareerManager.mRetiredCareer = retiredCareer;

                    if (occupation is ActiveFireFighter)
                    {
                        ActiveCareerStaticData activeCareerStaticData = ActiveCareer.GetActiveCareerStaticData(OccupationNames.Firefighter);
                        activeCareerStaticData.HighestLevel = originaHighest;
                    }
                }
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);
            }

            IncStat("Core Failure");
            return false;
        }

        public override Scenario Clone()
        {
            return new FindJobScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerCareer, FindJobScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AutoFindJob";
            }
        }

        public class AllowSelfEmployedRetirementOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.IRetirementOption
        {
            public AllowSelfEmployedRetirementOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowSelfEmployedRetirement";
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
                if (!Manager.GetValue<ManagerCareer.AssignSelfEmployedOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class AssignDreamJobToEmployedOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public AssignDreamJobToEmployedOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AssignDreamJobToEmployed";
            }
        }

        public class AllowStandAloneOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public AllowStandAloneOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowStandAlone";
            }
        }

        public class ChooseCareerOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public ChooseCareerOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ChooseCareer";
            }
        }

        public class ForceTeensOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public ForceTeensOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ForceTeenJobs";
            }
        }

        public class JobItem : ValueSettingOption<Occupation>
        {
            public readonly bool mPreferred;

            public JobItem(Occupation career, bool preferred)
            {
                mValue = career;

                mPreferred = preferred;

                SetThumbnail(career.CareerIconColored, ProductVersion.BaseGame);
            }

            public override string Name
            {
                get { return mValue.CareerName; }
            }
        }

        public class JobPreferenceColumn : ObjectPickerDialogEx.CommonHeaderInfo<JobItem>
        {
            public JobPreferenceColumn()
                : base("NRaas.StoryProgression.ChooseCareer:PreferenceTitle", "NRaas.StoryProgression.ChooseCareer:PreferenceToolTip", 20)
            { }

            public override ObjectPicker.ColumnInfo GetValue(JobItem item)
            {
                if (item.mPreferred)
                {
                    return new ObjectPicker.TextColumn(Common.Localize("Boolean:True"));
                }
                else
                {
                    return new ObjectPicker.TextColumn("");
                }
            }
        }
    }
}
