using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.SimDataElement;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.ScoringMethods;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerCareer : Manager
    {
        List<SimDescription> mSchoolChildren = new List<SimDescription>();
        List<SimDescription> mEmployed = new List<SimDescription>();

        bool mRewardsDisabled = false;

        static CareerLocation sRetiredLocation = null;

        public ManagerCareer(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Careers";
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerCareer>(this).Perform(initial);
        }

        public static event Scenario.UpdateDelegate sStylistHelp;

        public static void PerformStylistHelp(Scenario scenario, ScenarioFrame frame)
        {
            if (sStylistHelp == null) return;

            sStylistHelp(scenario, frame);
        }

        // Externalized to Register
        public static bool ApplyRetiredCareer(SimDescription sim)
        {
            try
            {
                if (NRaas.StoryProgression.Main != null)
                {
                    NRaas.StoryProgression.Main.GetData<CareerSimData>(sim).Retire();
                    return true;
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }

            return false;
        }

        public static CareerLocation RetiredLocation
        {
            get 
            {
                if (sRetiredLocation == null) return null;

                if (sRetiredLocation.Career == null) return null;

                return sRetiredLocation; 
            }
            set 
            { 
                sRetiredLocation = value; 
            }
        }

        public List<SimDescription> SchoolChildren
        {
            get { return mSchoolChildren; }
        }

        public List<SimDescription> Employed
        {
            get { return mEmployed; }
        }

        public override void Startup(PersistentOptionBase options)
        {
            base.Startup(options);

            Career.kMinimumBossLevel = 1;
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if ((!mRewardsDisabled) && (DisableRewardCeremonies.Perform()))
            {
                mRewardsDisabled = true;
            }

            if (initialPass)
            {
                if (ProgressionEnabled)
                {
                    foreach (SimDescription sim in Sims.All)
                    {
                        if (sim.CareerManager == null) continue;

                        if (sim.Occupation != null) continue;

                        if (sim.CareerManager.RetiredCareer == null) continue;

                        CareerSimData data = GetData<CareerSimData>(sim);
                        
                        data.Retire();
                        data.Reset();
                    }
                }
            }

            if ((ProgressionEnabled) && (fullUpdate))
            {
                mSchoolChildren.Clear();
                mEmployed.Clear();

                foreach (SimDescription sim in Sims.All)
                {
                    if (sim.Household == null) continue;

                    if ((sim.CareerManager != null) && (sim.CareerManager.School != null))
                    {
                        if ((sim.Child) || (sim.Teen))
                        {
                            mSchoolChildren.Add(sim);

                            AddScoring("School Perf", (int)sim.CareerManager.School.Performance);
                        }
                        else
                        {
                            sim.CareerManager.School.LeaveJob(false, Career.LeaveJobReason.kDebug);
                            sim.CareerManager.mSchool = null;

                            IncStat("Bad School Dropped");
                        }
                    }

                    if (ValidCareer(sim.Occupation))
                    {
                        if (!IsPlaceholderCareer(sim.Occupation as Career))
                        {
                            AddScoring("Work Perf", (int)sim.Occupation.Performance);

                            if (sim.Occupation.Coworkers != null)
                            {
                                AddStat("Coworkers", sim.Occupation.Coworkers.Count);
                            }
                            else
                            {
                                AddStat("Coworkers", 0);
                            }

                            ResetBoss(this, sim.Occupation);

                            AddStat("Boss", (sim.Occupation.Boss != null) ? 1 : 0);

                            mEmployed.Add(sim);
                        }
                    }
                    else
                    {
                        if (sim.Occupation != null)
                        {
                            try
                            {
                                sim.Occupation.LeaveJob(Career.LeaveJobReason.kDebug);
                            }
                            catch (Exception e)
                            {
                                Common.DebugException(sim, e);

                                sim.CareerManager.mJob = null;
                            }
                        }
                    }
                }
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public static bool IsRegisterInstalled()
        {
            return Common.AssemblyCheck.IsInstalled("NRaasRegister");
        }

        public List<SimDescription> GetCareerSims(OccupationNames name)
        {
            List<SimDescription> results = new List<SimDescription>();

            foreach (SimDescription sim in Employed)
            {
                if (sim.Occupation == null) continue;

                if (sim.Occupation.Guid == name)
                {
                    results.Add(sim);
                }
            }

            return results;
        }

        public bool IsValidBoss(Common.IStatGenerator stats, Occupation job, SimDescription boss)
        {
            if (boss == null)
            {
                stats.IncStat("Valid: Null");
                return false;
            }
            else if (job.OwnerDescription == boss)
            {
                stats.IncStat("Valid: Me");
                return false;
            }
            else if (!boss.IsValidDescription)
            {
                stats.IncStat("Valid: Invalid");
                return false;
            }
            else if (SimTypes.IsDead(boss))
            {
                stats.IncStat("Valid: Dead");
                return false;
            }
            else
            {
                try
                {
                    LawEnforcement lawCareer = job as LawEnforcement;
                    if ((lawCareer != null) && (lawCareer.Partner == boss))
                    {
                        stats.IncStat("Valid: Partner");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    // Partner 
                    Common.DebugException(job.OwnerDescription, e);
                }

                if (boss.CareerManager == null)
                {
                    stats.IncStat("Valid: No Manager");
                    return false;
                }

                Occupation bossJob = boss.Occupation;
                if ((bossJob != null) && (bossJob.Boss == job.OwnerDescription) && ((!SimTypes.IsSelectable(job.OwnerDescription)) || (bossJob.CareerLevel != job.CareerLevel)))
                {
                    stats.IncStat("Valid: Peon");
                    return false;
                }

                Career career = job as Career;
                if ((career != null) && (bossJob != null) && (!(bossJob is Career)))
                {
                    stats.IncStat("Valid: Job Type Mismatch");
                    return false;
                }

                if ((job.CareerLoc == null) || (!Money.GetDeedOwner(job.CareerLoc.Owner).Contains(boss)))
                {
                    if (bossJob == null)
                    {
                        stats.IncStat("Valid: No Job");
                        return false;
                    }
                    else if (((career == null) || (!career.IsPartTime)) && (bossJob.Guid != job.Guid))
                    {
                        stats.IncStat("Valid: Wrong Job");
                        return false;
                    }
                    else if (bossJob.CareerLoc != job.CareerLoc)
                    {
                        stats.IncStat("Valid: Wrong Location");
                        return false;
                    }
                    else if ((bossJob.CareerLevel <= job.CareerLevel) &&
                             ((!SimTypes.IsSelectable(job.OwnerDescription)) || (bossJob.CareerLevel != job.CareerLevel)))
                    {
                        stats.IncStat("Valid: Wrong Level");
                        return false;
                    }
                }
                else
                {
                    if (job.OwnerDescription.YoungAdultOrAbove != boss.YoungAdultOrAbove)
                    {
                        stats.IncStat("Valid: Too Young");
                        return false;
                    }

                    if (boss.IsEP11Bot)
                    {
                        if (boss.Household.IsActive)
                        {
                            stats.IncStat("Valid: EP11 Bot In Active Household");
                            return false;
                        }

                        if (boss.TraitManager != null)
                        {                         
                            if(!boss.TraitManager.HasElement(TraitNames.ProfessionalChip))
                            {
                                stats.IncStat("Valid: EP11 Bot doesn't have ProfessionalChip");
                                return false;
                            }
                        }
                        else
                        {
                            stats.IncStat("Valid: EP11 Bot TraitManager Null");
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool ResetBoss(Common.IStatGenerator stats, Occupation job)
        {
            if (job == null) return false;

            Career career = job as Career;
            if (career != null)
            {
                if ((job.FormerBoss != null) && (job.FormerBoss.Occupation != null) && (!(job.FormerBoss.Occupation is Career)))
                {
                    job.FormerBoss = null;

                    IncStat("Reset Former Boss");
                }
            }

            if (((career == null) || (career.CurLevel.HasBoss)) && (!IsValidBoss(stats, job, job.Boss)) && (IsValidBoss(stats, job, job.FormerBoss)))
            {
                job.Boss = job.FormerBoss;
                return true;
            }
            return false;
        }

        public override void Shutdown()
        {
            base.Shutdown();

            sRetiredLocation = null;
        }

        protected override string IsOnActiveLot(SimDescription sim, bool testViewLot)
        {
            return base.IsOnActiveLot(sim, false);
        }

        public bool Allow(IScoringGenerator stats, Sim sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, Sim sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (sim.Household != null)
            {
                if (SimTypes.IsTourist(sim))
                {
                    stats.IncStat("Allow: Tourist");
                    return false;
                }
            }

            if (!settings.GetValue<AllowCareerProgressionOption, bool>())
            {
                stats.IncStat("Allow: Career Denied");
                return false;
            }

            return true;
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            check &= ~AllowCheck.Active;

            return base.PrivateAllow(stats, sim, check);
        }

        public bool AllowFindJob(IScoringGenerator stats, SimDescription sim)
        {
            if (!Allow(stats, sim)) return false;

            if (!GetValue<AllowFindJobOption, bool>(sim))
            {
                stats.IncStat("Allow: Find Job Denied");
                return false;
            }

            if (sim.Household.IsTravelHousehold)
            {
                stats.IncStat("Allow: Travel Household");
                return false;
            }

            return true;
        }

        public override Scenario GetImmigrantRequirement(Manager.ImmigrationRequirement requirement)
        {
            return new Scenarios.Careers.ImmigrantRequirementScenario(requirement);
        }

        public static bool ValidCareer(Occupation career)
        {
            return ValidCareer(career, false);
        }
        protected static bool ValidCareer(Occupation job, bool nullValid)
        {
            if (job == null) return nullValid;

            Career career = job as Career;
            if (career == null) return true;

            if (career.SharedData == null) return false;

            if (career.CurLevel == null) return false;

            if (job.CareerLoc == null) return false;

            if (job.CareerLoc.Owner == null) return false;

            if (job.CareerLoc.Owner.CareerLocations == null) return false;

            return true;
        }

        public static bool IsCoworkerOrBoss(Occupation career, SimDescription sim)
        {
            if (career == null) return false;

            if (career.Boss == sim) return true;

            if (career.Coworkers != null)
            {
                if (career.Coworkers.Contains(sim)) return true;
            }

            return false;
        }

        public static bool IsCoworkerTone(Tone tone)
        {
            return ((tone is MeetCoworkersTone) ||
                    (tone is HangWithCoworkersTone) ||
                    (tone is SuckUpToBossTone));
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);

            RemoveSim(mSchoolChildren, sim);
            RemoveSim(mEmployed, sim);

            if (Sims != null)
            {
                // Removal of a coworker can causes a HangWithCoworkers bounces, reset all tones now
                foreach (SimDescription coworker in Sims.All)
                {
                    try
                    {
                        VerifyTone(coworker);
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(coworker, e);
                    }
                }
            }
        }

        public static bool HasSkillCareer(Household house, SkillNames skill)
        {
            if (house == null) return false;

            foreach (SimDescription sim in house.AllSimDescriptions)
            {
                if (HasSkillCareer(sim, skill))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool HasSkillCareer(SimDescription sim, SkillNames skill)
        {
            SkillBasedCareer career = sim.Occupation as SkillBasedCareer;
            if (career == null) return false;

            SkillBasedCareerStaticData skillData = career.GetOccupationStaticDataForSkillBasedCareer();
            if (skillData == null) return false;

            return (skill == skillData.CorrespondingSkillName);
        }

        public void VerifyTone(SimDescription sim)
        {
            if (sim == null) return;

            Career career = sim.Occupation as Career;
            if (career != null)
            {
                if (!VerifyTone(career.LastTone))
                {
                    IncStat("Bad Tone Dropped");

                    career.LastTone = null;
                }
            }

            if ((sim.CareerManager != null) && (sim.CareerManager.School != null))
            {
                if (!VerifyTone(sim.CareerManager.School.LastTone))
                {
                    IncStat("Bad Tone Dropped");

                    sim.CareerManager.School.LastTone = null;
                }
            }

            if ((sim.CreatedSim != null) && (sim.CreatedSim.InteractionQueue != null))
            {
                RabbitHole.RabbitHoleInteraction<Sim, RabbitHole> work = sim.CreatedSim.InteractionQueue.GetCurrentInteraction() as RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>;
                if (work != null)
                {
                    if (!VerifyTone(work.CurrentTone as CareerTone))
                    {
                        IncStat("Bad Tone Dropped");

                        work.CurrentTone = null;
                    }
                }
            }
        }
        public bool VerifyTone(Career career)
        {
            if (career == null) return true;

            if (VerifyTone( career.LastTone)) return true;

            IncStat("Bad Tone Dropped");

            career.LastTone = null;
            return false;
        }
        public static bool VerifyTone(CareerTone tone)
        {
            if (tone == null) return true;

            if (!ValidCareer(tone.Career)) return false;

            if (SimTypes.IsSpecial(tone.Career.OwnerDescription))
            {
                if (IsCoworkerTone(tone)) return false;
            }

            MeetCoworkersTone meetTone = tone as MeetCoworkersTone;
            if (meetTone != null)
            {
                if ((meetTone.Career.Coworkers == null) ||
                    (meetTone.Career.Coworkers.Count == 0) ||
                    (!meetTone.AllCoworkersNotKnown()))
                {
                    return false;
                }
            }
            else 
            {
                HangWithCoworkersTone hangTone = tone as HangWithCoworkersTone;
                if (hangTone != null)
                {
                    if ((hangTone.GetCoworkersKnown() == null) ||
                        (hangTone.GetCoworkersKnown().Count == 0))
                    {
                        return false;
                    }

                    int index=0;
                    while (index < hangTone.mSimsInConv.Count)
                    {
                        SimDescription other = hangTone.mSimsInConv[index];
                        if ((other == null) || (other == tone.Career.OwnerDescription))
                        {
                            Common.DebugNotify("Empty Sim Dropped: " + hangTone.Career.OwnerDescription.FullName);

                            hangTone.mSimsInConv.RemoveAt(index);
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
                else
                {
                    TakeARiskTone riskTone = tone as TakeARiskTone;
                    if (riskTone != null)
                    {
                        if (riskTone.Career.Boss == null) return false;
                    }
                    else
                    {
                        SuckUpToBossTone bossTone = tone as SuckUpToBossTone;
                        if (bossTone != null)
                        {
                            if (bossTone.Career.Boss == null) return false;
                        }
                    }
                }
            }

            return tone.ShouldAddTone(tone.Career);
        }

        public static DreamJob GetDreamJob(OccupationNames guid)
        {
            if (guid == OccupationNames.Undefined) return null;

            SkillBasedCareer career = CareerManager.GetStaticOccupation(guid) as SkillBasedCareer;
            if (career != null)
            {
                SkillBasedCareerStaticData occupationStaticDataForSkillBasedCareer = career.GetOccupationStaticDataForSkillBasedCareer();
                if (occupationStaticDataForSkillBasedCareer == null) return null;

                return SkillDreamJob.Get(occupationStaticDataForSkillBasedCareer.CorrespondingSkillName);
            }
            else
            {
                switch (guid)
                {
                    case OccupationNames.Firefighter:
                        return new FirefighterDreamJob();
                    case OccupationNames.GhostHunter:
                        return new GhostHunterDreamJob();
                    case OccupationNames.Stylist:
                        return new StylistDreamJob();
                    case OccupationNames.InteriorDesigner:
                        return new InteriorDesignerDreamJob();
                    case OccupationNames.PrivateEye:
                        return new PrivateEyeDreamJob();
                    case OccupationNames.AcademicCareer:
                        return new AcademicDreamJob();
                    default:
                        return new CareerDreamJob(guid);
                }
            }
        }
        public static List<DreamJob> GetDreamJob(SimDescription sim)
        {
            return GetDreamJob(sim, false);
        }
        public static List<DreamJob> GetDreamJob(SimDescription sim, bool onlySelfEmployed)
        {
            List<DreamJob> dreamJobs = new List<DreamJob>();

            if (sim != null)
            {
                DreamNodeInstance instance = null;
                DreamsAndPromisesManager.sMajorWishes.TryGetValue(sim.LifetimeWish, out instance);
                if (((instance != null) && (instance.InputSubject != null)) && (instance.InputSubject.mType == DreamNodePrimitive.InputSubjectType.Career))
                {
                    dreamJobs.Add(new CareerDreamJob ((OccupationNames)instance.InputSubject.EnumValue));
                }
                else
                {
                    switch ((LifetimeWant)sim.LifetimeWish)
                    {
                        case LifetimeWant.PerfectMindPerfectBody:
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.ProfessionalSports));
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.Criminal));
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.Science));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Athletic));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Logic));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Chess));
                            break;
                        case LifetimeWant.TheCulinaryLibrarian:
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.Culinary));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Cooking));
                            break;
                        case LifetimeWant.MartialArtsMaster:
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.ProfessionalSports));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.MartialArts));
                            break;
                        case LifetimeWant.ChessLegend:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Chess));
                            break;
                        case LifetimeWant.TheTinkerer:
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.Science));
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.Military));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Handiness));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Logic));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Chess));
                            break;
                        case LifetimeWant.GoldenTongueGoldenFingers:
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.Music));
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.Business));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Guitar));
                            break;
                        case LifetimeWant.PhysicalPerfection:
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.ProfessionalSports));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Athletic));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.MartialArts));
                            break;
                        case LifetimeWant.ThePerfectGarden:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Gardening));
                            break;
                        case LifetimeWant.WorldClassGallery:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Photography));
                            break;
                        case LifetimeWant.PresentingThePerfectPrivateAquarium:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Fishing));
                            break;
                        case LifetimeWant.BottomlessNectarCellar:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Nectar));
                            break;
                        case LifetimeWant.Visionary:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Painting));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Photography));
                            break;
                        case LifetimeWant.DescendantOfDaVinci:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Painting));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Sculpting));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Inventing));
                            break;
                        case LifetimeWant.MasterOfTheArts:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Painting));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Guitar));
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.Music));
                            break;
                        case LifetimeWant.IllustriousAuthor:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Writing));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Painting));
                            break;
                        case LifetimeWant.ProfessionalAuthor:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Writing));
                            break;
                        case LifetimeWant.MonsterMaker:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Inventing));
                            break;
                        case LifetimeWant.ParanormalProfiteer:
                            dreamJobs.Add(new GhostHunterDreamJob());
                            break;
                        case LifetimeWant.FashionPhenomenon:
                            dreamJobs.Add(new StylistDreamJob());
                            break;
                        case LifetimeWant.PervasivePrivateEye:
                            dreamJobs.Add(new PrivateEyeDreamJob());
                            break;
                        case LifetimeWant.HomeDesignHotshot:
                            dreamJobs.Add(new InteriorDesignerDreamJob());
                            break;
                        case LifetimeWant.FirefighterSuperHero:
                            dreamJobs.Add(new FirefighterDreamJob());
                            break;
                        case LifetimeWant.MasterOfAllInstruments:
                            dreamJobs.Add(new CareerDreamJob(OccupationNames.Music));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Guitar));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Drums));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Piano));
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.BassGuitar));
                            break;
                        case LifetimeWant.MasterBartender:
                            //dreamJobs.Add(new DreamJob(SkillNames.Bartending, new Type[] { typeof(BarProfessional)));
                            break;
                        case LifetimeWant.Jockey:
                            dreamJobs.Add(SkillDreamJob.Get(SkillNames.Riding));
                            break;
                    }
                }
            }

            if (onlySelfEmployed)
            {
                List<DreamJob> oldChoices = dreamJobs;
                
                dreamJobs = new List<DreamJob>();

                foreach (DreamJob job in oldChoices)
                {
                    if (job is SkillDreamJob)
                    {
                        dreamJobs.Add(job);
                    }
                }
            }

            return dreamJobs;
        }

        public static bool IsPlaceholderCareer(Career career)
        {
            if (career == null) return false;

            if (career.SharedData == null) return true;

            if (career.Level1 == null) return true;

            if (career.Level1.DayLength == 0) return true;

            return false;
        }

        public bool Test(Common.IStatGenerator stats, DreamJob job, SimDescription sim, Lot lot, Dictionary<OccupationNames, int> scores, bool inspecting)
        {
            if (job == null) return false;

            int score = 0;
            if (scores.TryGetValue(job.mCareer, out score))
            {
                if (score < 0)
                {
                    stats.IncStat(job.mCareer + " Score Fail");
                    return false;
                }
            }

            return TestCareer(stats, sim, job.mCareer, job);
        }

        public bool TestCareer(Common.IStatGenerator stats, SimDescription sim, OccupationNames career)
        {
            return TestCareer(stats, sim, career, GetDreamJob(career));
        }
        public bool TestCareer(Common.IStatGenerator stats, SimDescription sim, OccupationNames guid, DreamJob job)
        {
            if (HasAnyValue<AllowCareerOption, OccupationNames>(sim))
            {
                if (!HasValue<AllowCareerOption, OccupationNames>(sim, guid))
                {
                    IncStat(guid + " Not Allowed");
                    return false;
                }
            }

            if (HasValue<DisallowCareerOption, OccupationNames>(sim, guid))
            {
                stats.IncStat(guid + " Disallowed");
                return false;
            }

            if (job != null)
            {
                if (!job.Satisfies(this, sim, sim.LotHome, false))
                {
                    stats.IncStat(job.mCareer + " Not Satisfied");
                    return false;
                }
            }

            return true;
        }

        public bool TestAndAdd(Common.IStatGenerator stats, List<DreamJob> jobs, DreamJob job, SimDescription sim, Lot lot, Dictionary<OccupationNames,int> scores, bool inspecting, bool checkQuit)
        {
            if (job == null) return false;

            if (checkQuit)
            {
                if ((sim.CareerManager.QuitCareers == null) || (sim.CareerManager.QuitCareers.ContainsKey(job.mCareer)))
                {
                    return false;
                }
            }

            if (!Test(stats, job, sim, lot, scores, inspecting)) return false;

            jobs.Add(job);
            return true;
        }

        public bool GetPotentialCareers(Common.IStatGenerator stats, SimDescription sim, List<OccupationNames> results, bool checkQuit)
        {
            if (sim.LifetimeWish == (uint)LifetimeWant.JackOfAllTrades)
            {
                stats.IncStat("Jack of All Trades");
                return true;
            }

            bool retired = (sim.Occupation is Retired);

            bool dream = false;

            CareerScoringParameters parameters = new CareerScoringParameters(sim);

            List<DreamJob> careers = new List<DreamJob>();

            List<DreamJob> dreamJobs = GetDreamJob(sim, retired);
            if (dreamJobs.Count > 0)
            {
                foreach (DreamJob job in dreamJobs)
                {
                    TestAndAdd(stats, careers, job, sim, sim.LotHome, parameters.Scores, false, checkQuit);
                }

                if (careers.Count > 0)
                {
                    stats.IncStat("Adult: Dream Job");
                    dream = true;
                }
            }

            IListedScoringMethod scoring = ScoringLookup.GetScoring("JobScore");
            if (scoring != null)
            {
                scoring.IScore(parameters);
            }

            if ((careers.Count == 0) && (retired))
            {
                foreach (Skill skill in sim.SkillManager.List)
                {
                    if (skill.SkillLevel >= 8)
                    {
                        TestAndAdd(stats, careers, SkillDreamJob.Get(skill.Guid), sim, sim.LotHome, parameters.Scores, false, checkQuit);
                    }
                }

                if (careers.Count > 0)
                {
                    stats.IncStat("Retired: Skill Career");
                }

                return false;
            }

            if ((careers.Count == 0) && (GetValue<FamilyBusinessOption, bool>()))
            {
                foreach (SimDescription parent in Relationships.GetParents(sim))
                {
                    if (parent.CareerManager == null) continue;

                    if (parent.CareerManager.QuitCareers == null) continue;

                    List<Occupation> parentList = new List<Occupation>(parent.CareerManager.QuitCareers.Values);
                    if (parent.Occupation != null)
                    {
                        parentList.Add(parent.Occupation);
                    }

                    Career choice = null;

                    foreach (Occupation occupation in parentList)
                    {
                        if (occupation is School) continue;

                        if (!Test(stats, GetDreamJob(occupation.Guid), sim, sim.LotHome, parameters.Scores, false)) continue;

                        Career career = occupation as Career;
                        if (career != null)
                        {
                            if (IsPlaceholderCareer(career)) continue;

                            if (career.IsPartTime) continue;

                            if (career.CareerLevel == 1) continue;

                            if ((choice == null) || (choice.CareerLevel < career.CareerLevel))
                            {
                                choice = career as Career;
                            }
                        }
                    }

                    if (choice != null)
                    {
                        if ((sim.CareerManager.QuitCareers == null) || (!sim.CareerManager.QuitCareers.ContainsKey(choice.Guid)))
                        {
                            careers.Add(GetDreamJob(choice.Guid));
                        }
                    }
                }

                if (careers.Count > 0)
                {
                    stats.AddStat("Family Business", careers.Count);
                }
            }

#if _NEXTPHASE        
            if ((careers.Count == 0) && (GetValue<BalancedCareersOption, bool>()))
            {
                Dictionary<OccupationNames, int> counts = new Dictionary<OccupationNames, int>();

                foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
                {
                    if (hole.CareerLocations == null) continue;

                    foreach (CareerLocation loc in hole.CareerLocations.Values)
                    {
                        if (loc == null) continue;

                        if (loc.Career == null) continue;

                        if (loc.Career is School) continue;

                        if (IsPlaceholderCareer(loc.Career)) continue;

                        if (loc.Career.IsPartTime) continue;

                        if (loc.Workers == null) continue;

                        if (!Test(GetDreamJob(loc.Career), sim, sim.LotHome, false)) continue;
                        
                        int count = 0;
                        if (counts.TryGetValue(loc.Career.Guid, out count))
                        {
                            count += loc.Workers.Count;
                        }

                        counts[loc.Career.Guid] = count;
                    }
                }

                if (counts.Count > 0)
                {
                    List<KeyValuePair<OccupationNames, int>> sorted = new List<KeyValuePair<OccupationNames, int>>(counts);
                    sorted.Sort(new Comparison<KeyValuePair<OccupationNames, int>>(OnSort));

                    OccupationNames guid = sorted[0].Key;

                    if ((sim.CareerManager.QuitCareers == null) || (!sim.CareerManager.QuitCareers.ContainsKey(guid)))
                    {
                        Occupation career = CareerManager.GetStaticOccupation(guid);
                        if (career != null)
                        {
                            careers.Add(GetDreamJob(career));

                            stats.IncStat("Balanced Career");
                        }
                    }
                }
            }
#endif
            if ((careers.Count == 0) && (sim.TraitManager != null))
            {
                List<OccupationNames> choices = new List<OccupationNames>();

                stats.AddStat("Trait Choices", parameters.Scores.Count);

                foreach (KeyValuePair<OccupationNames, int> scores in parameters.Scores)
                {
                    stats.AddScoring("Trait Scores", scores.Value);

                    if (!RandomUtil.RandomChance(scores.Value)) continue;

                    choices.Add(scores.Key);
                }

                if (choices.Count == 0)
                {
                    foreach (KeyValuePair<OccupationNames, int> scores in parameters.Scores)
                    {
                        if (scores.Value <= 0) continue;

                        choices.Add(scores.Key);
                    }
                }

                foreach (OccupationNames career in choices)
                {
                    TestAndAdd(stats, careers, GetDreamJob(career), sim, sim.LotHome, parameters.Scores, false, true);
                }

                stats.AddStat("Trait Job", careers.Count);
            }

            foreach (DreamJob career in careers)
            {
                if (career == null) continue;

                results.Add(career.mCareer);
            }

            return dream;
        }

        public static int OnSort(KeyValuePair<OccupationNames, int> left, KeyValuePair<OccupationNames, int> right)
        {
            if (left.Value > right.Value)
            {
                return 1;
            }
            else if (left.Value < right.Value)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public class Updates : AlertLevelOption<ManagerCareer>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        public class DebugOption : DebugLevelOption<ManagerCareer>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerCareer>
        {
            public DumpScoringOption()
            { }
        }

        public class SpeedOption : SpeedBaseOption<ManagerCareer>
        {
            public SpeedOption()
                : base(500, false)
            { }
        }

        public class TicksPassedOption : TicksPassedBaseOption<ManagerCareer>
        {
            public TicksPassedOption()
            { }
        }

#if _NEXTPHASE        
        public class BalancedCareersOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public BalancedCareersOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "BalancedCareers";
            }
        }
#endif

        public class FamilyBusinessOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public FamilyBusinessOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "FamilyBusiness";
            }
        }

        public class AssignSelfEmployedOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public AssignSelfEmployedOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AssignSelfEmployed";
            }
        }

        public interface IPerformanceOption : INotRootLevelOption
        { }

        public class PerformanceListingOption : NestingManagerOptionItem<ManagerCareer, IPerformanceOption>
        {
            public PerformanceListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "PerformanceListing";
            }
        }

        public interface ISchoolOption : INotRootLevelOption
        { }

        public class SchoolListingOption : NestingManagerOptionItem<ManagerCareer, ISchoolOption>
        {
            public SchoolListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "SchoolListing";
            }
        }

        public interface IRetirementOption : INotRootLevelOption
        { }

        public class RetirementListingOption : NestingManagerOptionItem<ManagerCareer, IRetirementOption>
        {
            public RetirementListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "RetirementListing";
            }
        }
    }
}
