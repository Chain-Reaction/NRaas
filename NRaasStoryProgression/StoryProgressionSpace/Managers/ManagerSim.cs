using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Tasks;
using NRaas.StoryProgressionSpace.Booters;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Options.Immigration;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerSim : Manager, ISimFromBinManager
    {
        protected static SimDemographics sDemographics = null;

        protected int mMiniSimCount = 0;

        public ManagerSim(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Sims";
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerSim>(this).Perform(initial);
        }

        public int GetResidentCount(CASAgeGenderFlags species)
        {
            int value;
            if (Demographics.mResidentCount.TryGetValue(species, out value))
            {
                return value;
            }
            else
            {
                return -1;
            }
        }

        public static void ForceRecount()
        {
            sDemographics = null;
        }

        public ICollection<SimDescription> All
        {
            get
            {
                return Demographics.mAll.Values;
            }
        }

        public ICollection<SimDescription> Humans
        {
            get
            {
                return Demographics.mHumans;
            }
        }

        public ICollection<SimDescription> Pets
        {
            get
            {
                return Demographics.mPets;
            }
        }

        public ICollection<SimDescription> GetSpecies(bool isPet)
        {
            if (isPet)
            {
                return Pets;
            }
            else
            {
                return Humans;
            }
        }

        protected static SimDemographics Demographics
        {
            get
            {
                if (sDemographics == null)
                {
                    sDemographics = new SimDemographics();
                }

                return sDemographics;
            }
        }

        public int GetCelebrityCount(uint level)
        {
            int celebCount;
            if (!Demographics.mCelebrityCount.TryGetValue(level, out celebCount)) return 0;

            return celebCount;
        }
        
        public ICollection<SimDescription> Occults
        {
            get { return Demographics.mOccults; }
        }
        
        public ICollection<SimDescription> Adults
        {
            get { return Demographics.mAdults; }
        }

        public ICollection<SimDescription> AdultMales
        {
            get { return Demographics.mAdultMales; }
        }

        public ICollection<SimDescription> AdultFemales
        {
            get { return Demographics.mAdultFemales; }
        }

        public ICollection<SimDescription> TeensAndAdults
        {
            get { return Demographics.mTeensAndAdults; }
        }

        public ICollection<SimDescription> Children
        {
            get { return Demographics.mChildren; }
        }

        public ICollection<SimDescription> Teens
        {
            get { return Demographics.mTeens; }
        }

        public ICollection<SimDescription> TeenMales
        {
            get { return Demographics.mTeenMales; }
        }

        public ICollection<SimDescription> TeenFemales
        {
            get { return Demographics.mTeenFemales; }
        }

        public ICollection<SimDescription> ResidentMales
        {
            get { return Demographics.mResidentMales; }
        }

        public ICollection<SimDescription> ResidentFemales
        {
            get { return Demographics.mResidentFemales; }
        }

        public float AdultToChildRatio
        {
            get
            {
                return (((float)Demographics.mAdults.Count) / All.Count);
            }
        }

        public float NextGenerationRatio
        {
            get
            {
                return (((float)Demographics.mNextGeneration.Count) / All.Count);
            }
        }

        public static bool AddBuff(SimDescription sim, BuffNames buff, Origin origin)
        {
            return AddBuff(sim, buff, float.MaxValue, origin);
        }
        public static bool AddBuff(SimDescription sim, BuffNames buff, float timeoutLength, Origin origin)
        {
            try
            {
                sim.CreatedSim.BuffManager.RemoveElement(buff);
                sim.CreatedSim.BuffManager.AddElement(buff, timeoutLength, origin);
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);
                return false;
            }
        }

        public bool InStasis(SimDescription sim)
        {
            return GetValue<InStasisOption, bool>(sim);
        }

        public class TestInactiveLTWTask : Common.FunctionTask
        {
            Common.IStatGenerator mStats;
            SimDescription mSim;
            Event mEvent;

            protected TestInactiveLTWTask(Common.IStatGenerator stats, SimDescription sim, Event e)
            {
                mStats = stats;
                mSim = sim;
                mEvent = e;
            }

            public static void Perform(Common.IStatGenerator stats, SimDescription sim, Event e)
            {
                new TestInactiveLTWTask(stats, sim, e).AddToSimulator();
            }

            protected override void OnPerform()
            {
                if ((mSim.CreatedSim != null) && 
                    (!mSim.HasCompletedLifetimeWish) && 
                    (mSim.CreatedSim.DreamsAndPromisesManager == null) && 
                    (mSim.LotHome != null)) // Must check residency to stop several dreams errors from occurring
                {
                    try
                    {
                        DreamsAndPromisesManager.CreateAndIntitForSim(mSim.CreatedSim);

                        if (mSim.LifetimeWishNode != null)
                        {
                            ActiveDreamNode ltw = mSim.LifetimeWishNode as ActiveDreamNode;
                            if (ltw != null)
                            {
                                if (ltw.mEventListener == null)
                                {
                                    ltw.ResetListener();
                                }

                                ltw.RunFeedbackFunction(mEvent);

                                // Stops the actual dream system from checking the node as well
                                bool completed = mSim.CreatedSim.DreamsAndPromisesManager.mCompletedNodes.Remove(ltw);

                                mSim.CreatedSim.NullDnPManager();

                                if (completed)
                                {

                                    mSim.HasCompletedLifetimeWish = true;

                                    mSim.mLifetimeHappiness += ltw.AchievementPoints;
                                    mSim.mSpendableHappiness += ltw.AchievementPoints;

                                    try
                                    {
                                        mSim.CreatedSim.BuffManager.RemoveElement(BuffNames.Fulfilled);
                                        mSim.CreatedSim.BuffManager.AddElement(BuffNames.Fulfilled, DreamsAndPromisesManager.kVeryFulfilledMoodValue, DreamsAndPromisesManager.kVeryFulfilledDuration, Origin.FromLifetimeWish);
                                        (mSim.CreatedSim.BuffManager.GetElement(BuffNames.Fulfilled) as BuffFulfilled.BuffInstanceFulfilled).SetVeryFulfilledText();
                                    }
                                    catch (Exception ex)
                                    {
                                        Common.DebugException(mSim, ex);
                                    }

                                    mStats.IncStat("LifetimeWish Fulfilled");
                                }
                                else
                                {
                                    mStats.IncStat("Lifetime Wish Not Satisfied");
                                }
                            }
                            else
                            {
                                mStats.IncStat("Invalid Lifetime Wish");
                            }
                        }
                        else
                        {
                            mStats.IncStat("No Lifetime Wish");
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.DebugException(mSim, ex);
                    }
                    finally
                    {
                        mSim.CreatedSim.NullDnPManager();
                    }
                }
            }
        }

        public string HandleName(INameTakeOption option, SimDescription simA, SimDescription simB, out bool wasEither)
        {
            return HandleName(option.Value, option.GetNameTakeLocalizationValueKey(), simA, simB, out wasEither);
        }
        public string HandleName(NameTakeType value, string prefix, SimDescription simA, SimDescription simB, out bool wasEither)
        {
            wasEither = (value == NameTakeType.Either);

            if (value == NameTakeType.None)
            {
                return null;
            }

            string lastName = null;

            NameTakeType choice = value;

            bool legacyA = false, legacyB = false;
            if (simA != null)
            {
                lastName = simA.LastName;

                if (GetValue<LegacyMarriageName, bool>(simA))
                {
                    legacyA = true;
                }
            }

            if (simB != null)
            {
                if (GetValue<LegacyMarriageName, bool>(simB))
                {
                    if (!legacyA)
                    {
                        lastName = simB.LastName;

                        legacyB = true;
                    }
                }
            }

            if (choice == NameTakeType.User)
            {
                return StringInputDialog.Show(Common.Localize(prefix + ":Title"), Common.Localize(prefix + ":Prompt", false, new object[] { simA, simB }), lastName);
            }
            else if ((legacyA) || (legacyB))
            {
                return lastName;
            }

            if (simA == null)
            {
                if (simB == null)
                {
                    return null;
                }
                else
                {
                    return simB.LastName;
                }
            }
            else if (simB == null)
            {
                return simA.LastName;
            }

            if (choice == NameTakeType.Either)
            {
                List<NameTakeType> choices = new List<NameTakeType>();
                choices.Add(NameTakeType.Husband);
                choices.Add(NameTakeType.Wife);
                choices.Add(NameTakeType.FemMale);
                choices.Add(NameTakeType.MaleFem);
                choices.Add(NameTakeType.None);

                choice = RandomUtil.GetRandomObjectFromList(choices);
            }

            if (simA.Gender == simB.Gender)
            {
                if (SimTypes.IsOlderThan(simB, simA))
                {
                    SimDescription sim = simA;
                    simA = simB;
                    simB = sim;
                }
            }

            switch (choice)
            {
                case NameTakeType.Husband:
                    if ((simA.IsMale) || (simA.Gender == simB.Gender))
                    {
                        lastName = simA.LastName;
                    }
                    else
                    {
                        lastName = simB.LastName;
                    }
                    break;
                case NameTakeType.Wife:
                    if ((simA.IsMale) || (simA.Gender == simB.Gender))
                    {
                        lastName = simB.LastName;
                    }
                    else
                    {
                        lastName = simA.LastName;
                    }
                    break;
                case NameTakeType.MaleFem:
                case NameTakeType.FemMale:
                    List<string> simANames = new List<string>(simA.LastName.Split(new char[] { '-' }));
                    List<string> simBNames = new List<string>(simB.LastName.Split(new char[] { '-' }));

                    string first = null;
                    string last = null;

                    if ((choice == NameTakeType.FemMale) == (simA.IsMale))
                    {
                        first = simBNames[0];
                        last = simANames[simANames.Count - 1];
                    }
                    else
                    {
                        first = simANames[0];
                        last = simBNames[simBNames.Count - 1];
                    }

                    if (first == last)
                    {
                        lastName = last;
                    }
                    else
                    {
                        lastName = first + "-" + last;
                    }
                    break;
            }
            return lastName;
        }

        public static string GetRoleHours(SimDescription sim)
        {
            DateAndTime start = DateAndTime.Invalid;
            DateAndTime end = DateAndTime.Invalid;
            if (Roles.GetRoleHours(sim, ref start, ref end))
            {
                return Common.LocalizeEAString(false, "Gameplay/MapTags/MapTag:ProprietorHours", new object[] { start.ToString(), end.ToString() });
            }
            else
            {
                return null;
            }
        }

        public string GetStatus(SimDescription sim)
        {
            string str = sim.FullName;

            if (sim.AssignedRole != null)
            {
                ShoppingRegister register = sim.AssignedRole.RoleGivingObject as ShoppingRegister;
                if (register != null)
                {
                    str += ", " + register.RegisterRoleName(sim.IsFemale);
                }
                else
                {
                    string roleName;
                    if (Localization.GetLocalizedString(sim.AssignedRole.CareerTitleKey, out roleName))
                    {
                        str += ", " + roleName;
                    }
                }

                string hours = GetRoleHours(sim);
                if (!string.IsNullOrEmpty(hours))
                {
                    str += Common.NewLine + hours;
                }
            }
            else if (SimTypes.InServicePool(sim))
            {
                string serviceName;
                if (Localization.GetLocalizedString("Ui/Caption/Services/Service:" + sim.CreatedByService.ServiceType.ToString(), out serviceName))
                {
                    str += ", " + serviceName;
                }
            }

            Sim createdSim = sim.CreatedSim;

            if (Main.DebuggingEnabled)
            {
                if (createdSim != null)
                {
                    str += Common.NewLine + "Autonomy: ";
                    if (createdSim.Autonomy == null)
                    {
                        str += "None";
                    }
                    else
                    {
                        if (createdSim.Autonomy.AutonomyDisabled)
                        {
                            str += "Disabled";
                        }
                        else if (!AutonomyRestrictions.IsAnyAutonomyEnabled(createdSim))
                        {
                            str += "User Disabled";
                        }
                        else if (createdSim.Autonomy.IsRunningHighLODSimulation)
                        {
                            str += "High";
                        }
                        else
                        {
                            str += "Low";
                        }

                        if (createdSim.Autonomy.ShouldRunLocalAutonomy)
                        {
                            str += " Local";
                        }

                        if (createdSim.CanRunAutonomyImmediately())
                        {
                            str += " Ready";
                        }
                        else if (!createdSim.mLastInteractionWasAutonomous)
                        {
                            str += " Push";
                        }
                        else if (!createdSim.mLastInteractionSucceeded)
                        {
                            str += " Fail";
                        }

                        if (createdSim.Autonomy.InAutonomyManagerQueue)
                        {
                            str += " Queued";
                        }
                    }
                }
            }

            if (createdSim != null)
            {
                int flavour = (int)createdSim.MoodManager.MoodFlavor;

                str += Common.NewLine + Common.LocalizeEAString(false, "Ui/Tooltip/HUD/SimDisplay:MoodFlavor" + flavour.ToString());

                if (createdSim.CurrentInteraction != null)
                {
                    str += Common.NewLine;

                    try
                    {
                        str += createdSim.CurrentInteraction.ToString();
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(createdSim, e);

                        str += createdSim.CurrentInteraction.GetType();
                    }

                    Tone tone = createdSim.CurrentInteraction.CurrentTone;
                    if (tone != null)
                    {
                        str += Common.NewLine + tone.ToString();
                    }

                    SocialInteractionBase social = createdSim.CurrentInteraction as SocialInteractionBase;
                    if ((social != null) && (social.Target != null))
                    {
                        str += Common.Localize("Status:With", sim.IsFemale, new object[] { social.Target.Name });
                    }
                }
            }

            if (!SimTypes.IsSpecial(sim))
            {
                str += Common.Localize("Status:Cash", sim.IsFemale, new object[] { sim.FamilyFunds });

                int debt = NRaas.StoryProgression.Main.GetValue<DebtOption, int>(sim.Household);
                if (debt > 0)
                {
                    str += Common.Localize("Status:Debt", sim.IsFemale, new object[] { debt });
                }

                str += Common.Localize("Status:NetWorth", sim.IsFemale, new object[] { GetValue<NetWorthOption, int>(sim.Household) });
            }

            List<SimPersonality> clans = NRaas.StoryProgression.Main.Personalities.GetClanLeadership(sim);
            if (clans.Count > 0)
            {
                foreach (SimPersonality personality in clans)
                {
                    str += Common.Localize("Status:Leader", personality.IsFemaleLocalization(), new object[] { personality.GetLocalizedName() });
                }
            }

            clans = NRaas.StoryProgression.Main.Personalities.GetClanMembership(sim, false);
            if (clans.Count > 0)
            {
                foreach (SimPersonality personality in clans)
                {
                    str += Common.Localize("Status:Member", personality.IsFemaleLocalization(), new object[] { personality.GetLocalizedName() });
                }
            }

            return str;
        }

        public static Relationship GetRelationship(SimDescription a, SimDescription b)
        {
            try
            {
                return Relationship.Get(a, b, true);
            }
            catch (Exception e)
            {
                Common.DebugException(a, b, e);
            }
            return null;
        }

        public static void Union(List<SimDescription> results, List<SimDescription> adds)
        {
            Dictionary<SimDescription, bool> lookup = new Dictionary<SimDescription, bool>();

            foreach (SimDescription sim in results)
            {
                lookup.Add(sim, true);
            }

            foreach (SimDescription sim in adds)
            {
                if (lookup.ContainsKey(sim)) continue;

                results.Add(sim);
            }
        }

        public static bool HasTrait(SimDescription sim, TraitNames trait)
        {
            if (sim == null) return false;

            if (sim.TraitManager == null) return false;

            return sim.TraitManager.HasElement(trait);
        }

        public static bool ChangeOutfit(Common.IStatGenerator stats, SimDescription sim, OutfitCategories category)
        {
            if (sim.CreatedSim == null) return false;

            try
            {
                if (category == OutfitCategories.Singed)
                {
                    BuffSinged.SetupSingedOutfit(sim.CreatedSim);
                }

                stats.IncStat("Outfit " + category);

                sim.CreatedSim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.Force, category);
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);
                return false;
            }
        }

        public static bool AddBuff(Common.IStatGenerator stats, SimDescription sim, BuffNames buff, Origin origin)
        {
            if (sim == null) return false;

            if (sim.CreatedSim == null) return false;

            if (sim.CreatedSim.BuffManager == null) return false;

            stats.IncStat("Buffed " + buff);

            try
            {
                sim.CreatedSim.BuffManager.AddElement(buff, origin);
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);
            }

            if ((buff == BuffNames.Singed) || (buff == BuffNames.SingedElectricity))
            {
                ChangeOutfit(stats, sim, OutfitCategories.Singed);
            }

            return true;
        }

        public static int GetLTR(SimDescription a, SimDescription b)
        {
            Relationship relation = Relationship.Get(a, b, false);
            if (relation == null) return 0;

            return (int)relation.LTR.Liking;
        }

        public void ApplyOccultChance(Common.IStatGenerator stats, SimDescription sim, List<OccultTypes> validTypes, int occultChance, int maximum)
        {
            stats.AddStat("Occult Choices", validTypes.Count);

            bool activeSim = SimTypes.IsSelectable(sim);

            int occultsAdded = -1;
            if (OccultTypeHelper.CreateList(sim).Count > 0)
            {
                occultsAdded = 0;
            }

            foreach (OccultTypes type in validTypes)
            {
                if (occultsAdded >= maximum)
                {
                    break;
                }

                if (!RandomUtil.RandomChance(occultChance)) continue;

                if ((!activeSim) && (!SatisfiesTownOccultRatio(type))) continue;

                if (OccultTypeHelper.Add(sim, type, false, false))
                {
                    stats.IncStat("Occult Chance " + type);
                    occultsAdded++;
                }
            }

            if (!activeSim)
            {
                foreach (OccultTypes type in OccultTypeHelper.CreateList(sim))
                {
                    if (!SatisfiesTownOccultRatio(type))
                    {
                        stats.IncStat("Occult Ratio Fail");

                        OccultTypeHelper.Remove(sim, type, true);
                    }
                }
            }

            OccultTypeHelper.ValidateOccult(sim, stats.IncStat);
        }

        public bool SatisfiesTownOccultRatio(OccultTypes type)
        {
            float residents = (Demographics.mResidentMales.Count + Demographics.mResidentFemales.Count);

            float ratio = 0;
            if (residents > 0) 
            {
                ratio = (Demographics.GetOccultCount (type) / residents) * 100;
            }

            return (ratio < GetValue<OccultRatioOptionV2, int>(type));
        }

        public bool Select(Sim sim)
        {
            if (sim == null) return false;

            if (sim.Household == null) return false;

            bool bPopulateFridge = false;
            if ((sim.Household.SharedFridgeInventory != null) && (sim.Household.SharedFridgeInventory.Inventory != null))
            {
                if (sim.Household.SharedFridgeInventory.Inventory.Find<IGameObject>() == null)
                {
                    bPopulateFridge = true;
                }
            }

            if (bPopulateFridge)
            {
                Household.kNumServingsOnStartup = Lots.NumServingsOnStartup;
            }

            bool result = false;
            try
            {
                bool catchDreams = false;

                Household previousHouse = Household.ActiveHousehold;
                if (previousHouse != null)
                {
                    catchDreams = GetValue<DreamCatcherOption, bool>(previousHouse);
                }

                result = DreamCatcher.Select(sim, true, catchDreams);
            }
            finally
            {
                Household.kNumServingsOnStartup = 0;
            }

            return result;
        }

        public int GetDepopulationDanger (bool forPressure)
        {
            int num = 0;
            if (Lots.FreeLotRatio > 0.5f)
            {
                num++;
            }

            if (!forPressure)
            {
                if (NextGenerationRatio < 0.5f)
                {
                    num++;
                }
                if (AdultToChildRatio >= 0.8f)
                {
                    num++;
                }
            }

            if (Households.AverageHouseholdHumanSize <= 2.5f)
            {
                num++;
            }
            return num;
        }

        public override void Startup(PersistentOptionBase options)
        {
            base.Startup(options);

            if (MiniSimDescription.sMiniSims != null)
            {
                mMiniSimCount = MiniSimDescription.sMiniSims.Count;
            }
            else
            {
                mMiniSimCount = 0;
            }
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if ((fullUpdate) && (Main.ProgressionEnabled))
            {
                sDemographics = null;

                if (!initialPass)
                {
                    Lots.IncreaseImmigrationPressure(this, GetDepopulationDanger(true) * GetValue<ImmigrantRequirementScenario.PressureOption, int>());
                }

                AddStat("Danger Level", GetDepopulationDanger(false));
                AddStat("Free Lot Ratio", Lots.FreeLotRatio);
                AddStat("Adult to Child", AdultToChildRatio);
                AddStat("Average Housesize", Households.AverageHouseholdHumanSize);
                AddStat("Next Generation Ratio", NextGenerationRatio);
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public override void Shutdown()
        {
            sDemographics = null;

            base.Shutdown();
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
            if (settings.GetValue<InStasisOption,bool>())
            {
                stats.IncStat("Allow: Stasis Denied");
                return false;
            }

            return true;
        }

        public bool AllowInventory(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (!Allow(stats, sim, check)) return false;

            if (!GetValue<AllowInventoryOption,bool>(sim))
            {
                stats.IncStat("Allow: Inventory Denied");
                return false;
            }

            return true;
        }

        public bool AllowAging(IScoringGenerator stats, SimDescription sim)
        {
            if (!Allow(stats, sim, AllowCheck.None)) return false;

            // Don't use "Enabled" it checks world type and will set the aging flag for those sims
            if (!AgingManager.Singleton.mEnabled)
            {
                stats.IncStat("Allow: Aging Disabled");
                return false;
            }
            else if (SimTypes.IsDead(sim))
            {
                stats.IncStat("Allow: Dead");
                return false;
            }
            else if (sim.HasTrait(TraitNames.ForeverYoung))
            {
                stats.IncStat("Allow: Forever Young");
                return false;
            }
            else if (sim.HasTrait(TraitNames.SuperVampire))
            {
                stats.IncStat("Allow: Super Vampire");
                return false;
            }
            else if (!GetValue<AllowAgingOption,bool>(sim))
            {
                stats.IncStat("Allow: Aging Denied");
                return false;
            }

            return true;
        }

        public static Dictionary<ulong, SimDescription> GetSimDescriptionsInWorld(bool includeDead)
        {
            Dictionary<ulong, SimDescription> sims = new Dictionary<ulong, SimDescription>();

            foreach (SimDescription sim in Household.EverySimDescription())
            {
                if (sims.ContainsKey(sim.SimDescriptionId)) continue;

                sims.Add(sim.SimDescriptionId, sim);
            }

            if (includeDead)
            {
                foreach (SimDescription sim in SimDescription.GetHomelessSimDescriptionsFromUrnstones())
                {
                    if (sims.ContainsKey(sim.SimDescriptionId)) continue;

                    sims.Add(sim.SimDescriptionId, sim);
                }
            }

            return sims;
        }

        public static SimDescription Find(ulong id)
        {
            SimDescription sim;
            if (Demographics.mAll.TryGetValue(id, out sim))
            {
                return sim;
            }
            else
            {
                return null;
            }
        }

        public static void IncrementLifetimeHappiness(SimDescription sim, int delta)
        {
            if (sim == null) return;

            if ((!Sims3.SimIFace.Environment.HasEditInGameModeSwitch) && (delta > 0))
            {
                sim.mLifetimeHappiness += delta;
                sim.mSpendableHappiness += delta;
                /*
                if (sim.OnLifetimeHappinessChanged != null)
                {
                    sim.OnLifetimeHappinessChanged();
                }
                */ 
            }
        }

        public override Scenario GetImmigrantRequirement(ImmigrationRequirement requirement)
        {
            return new ImmigrantRequirementScenario(requirement);
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);

            sDemographics = null;
        }

        public bool Instantiate (SimDescription sim, Lot lot, bool moveIn)
        {
            if (sim.CreatedSim != null) return true;

            if (Instantiation.PerformOffLot(sim, lot, null) == null)
            {
                return false;
            }

            try
            {
                if (sim.CreatedSim != null)
                {
                    if ((moveIn) && (sim.CreatedSim.Service != null))
                    {
                        sim.CreatedSim.Service.EndService(sim);
                    }

                    if (sim.CreatedSim.Autonomy != null)
                    {
                        sim.CreatedSim.Autonomy.AllowedToRunMetaAutonomy = true;
                        sim.CreatedSim.Motives.RestoreDecays();
                    }
                }

                sim.MotivesDontDecay = false;
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);

                IncStat("Instantiate Failure");
                return false;
            }
        }

        public static bool HasYoungChildrenWith(SimDescription a, SimDescription b)
        {
            if ((b.IsPregnant) || (a.IsPregnant))
            {
                return true;
            }

            foreach (SimDescription child in Relationships.GetSims (a.Genealogy.Children))
            {
                if ((child.Genealogy.Parents.Contains(b.Genealogy)) && (!child.Baby) && (child.TeenOrBelow))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ValidSim(SimDescription sim, out string reason)
        {
            if (!sim.IsValidDescription)
            {
                reason = "Invalid Desc";
                return false;
            }

            if (sim.IsHuman)
            {
                if (sim.TraitManager != null)
                {
                    if (sim.TraitManager.List.Count == 0)
                    {
                        reason = "No Traits";
                        return false;
                    }
                }

                if (sim.Genealogy != null)
                {
                    if (sim.Genealogy.Children.Count > 0)
                    {
                        reason = null;
                        return true;
                    }
                }

                if (sim.FavoriteMusic == FavoriteMusicType.None)
                {
                    reason = "No Music";
                    return false;
                }

                if (sim.FavoriteFood == FavoriteFoodType.None)
                {
                    reason = "No Food";
                    return false;
                }
            }

            reason = null;
            return true;
        }

        public static List<SimDescription> Matching(ICollection<SimDescription> sims, CASAgeGenderFlags match)
        {
            List<SimDescription> results = new List<SimDescription> ();

            if (sims != null)
            {
                foreach (SimDescription sim in sims)
                {
                    if (((match & CASAgeGenderFlags.SpeciesMask) != CASAgeGenderFlags.None) && ((sim.Species & match) != sim.Species))
                    {
                        continue;
                    }

                    if ((sim.Age & match) == sim.Age)
                    {
                        results.Add(sim);
                    }
                    else if ((sim.Gender & match) == sim.Gender)
                    {
                        results.Add(sim);
                    }
                }
            }

            return results;
        }

        public bool Reset(SimDescription sim)
        {
            if (!GameStates.IsLiveState) return false;

            return (ResetSimTask.Perform(sim.CreatedSim, true) != null);
        }

        public bool HasEnough(IEnumerable<SimDescription> sims)
        {
            foreach (SimDescription sim in sims)
            {
                if (HasEnough(this, sim.Species))
                {
                    return false;
                }
            }

            return true;
        }
        public bool HasEnough(Common.IStatGenerator stats, SimDescription sim)
        {
            return HasEnough(stats, sim.Species);
        }
        public bool HasEnough(Common.IStatGenerator stats, CASAgeGenderFlags species)
        {
            int value = GetValue<ManagerSim.MaximumResidentsOption, int>(species);
            if (value <= 0) return false;

            // Has not been initialized
            if (GetResidentCount(species) < 0)
            {
                stats.IncStat("HasEnough Not Set");
                return true;
            }

            if (GetResidentCount(species) >= value)
            {
                stats.AddStat("HasEnough Too Many", GetResidentCount(species));
                return true;
            }

            return false;
        }
         
        public string EnsureUniqueName(SimDescription newSim)
        {
            bool fullList = !GetValue<CustomNamesOnlyOption<ManagerLot>, bool>();

            if (!fullList)
            {
                // The default name will be EA generated, change it now
                string newName = FirstNameListBooter.GetRandomName(fullList, newSim.Species, newSim.IsFemale);
                if (!string.IsNullOrEmpty(newName))
                {
                    newSim.FirstName = newName;
                }
            }

            Dictionary<string, bool> names = new Dictionary<string, bool>();
            names.Add("", true);

            foreach (SimDescription sim in SimListing.GetResidents(true).Values)
            {
                if (newSim == sim) continue;

                if (!SimTypes.IsEquivalentSpecies(newSim, sim)) continue;

                if (!string.IsNullOrEmpty(sim.FirstName))
                {
                    names[sim.FirstName] = true;
                }
            }

            string name = newSim.FirstName;
            if (string.IsNullOrEmpty(name))
            {
                name = "";
            }

            for (int i = 0; i < 10; i++)
            {
                if (!names.ContainsKey(name)) return name;

                string newName = FirstNameListBooter.GetRandomName(fullList, newSim.Species, newSim.IsFemale);
                if (!string.IsNullOrEmpty(newName))
                {
                    name = newName;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                if (!names.ContainsKey(name)) return name;

                string newName = FirstNameListBooter.GetRandomName(fullList, newSim.Species, newSim.IsFemale) + "-" + FirstNameListBooter.GetRandomName(fullList, newSim.Species, newSim.IsFemale);
                if (!string.IsNullOrEmpty(newName))
                {
                    name = newName;
                }
            }

            return name;
        }

        public static string GetPersonalInfo(SimDescription sim, string reason)
        {
            string traits = null;

            foreach (Trait trait in sim.TraitManager.List)
            {
                if (trait.IsReward) continue;

                if (!string.IsNullOrEmpty(traits))
                {
                    traits += ", ";
                }

                traits += trait.TraitName(sim.IsFemale);
            }

            return Common.Localize("PersonalInfo:Prompt", sim.IsFemale, new object[] { sim, Common.LocalizeEAString("UI/Feedback/CAS:" + sim.Age), Common.Localize("Species:" + sim.Species), LifetimeWants.GetName(sim), traits, reason });
        }

        public class Updates : AlertLevelOption<ManagerSim>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        public class DebugOption : DebugLevelOption<ManagerSim>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerSim>
        {
            public DumpScoringOption()
            { }
        }

        public class ProgressActiveNPCOption : BooleanManagerOptionItem<ManagerSim>
        {
            public ProgressActiveNPCOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ProgressNPCsinActiveHousehold";
            }
        }

        public class SpeedOption : SpeedBaseOption<ManagerSim>
        {
            public SpeedOption()
                : base(500, false)
            { }
        }

        public class TicksPassedOption : TicksPassedBaseOption<ManagerSim>
        {
            public TicksPassedOption()
            { }
        }

        public class MaximumResidentsOption : IntegerBySpeciesManagerOptionItem<ManagerSim, MaximumResidentsOption.SubOption>
        {
            public class SubOption : SubBaseOption, IAdjustForVacationOption
            {
                public SubOption()
                    : base(150)
                { }

                public override string GetTitlePrefix()
                {
                    return "MaximumResidents";
                }

                protected override int GetDefaultValue()
                {
                    switch (Species)
                    {
                        case CASAgeGenderFlags.Human:
                            return 150;
                        default:
                            return 25;
                    }
                }

                public bool AdjustForVacationTown()
                {
                    switch (Species)
                    {
                        case CASAgeGenderFlags.Human:
                            SetValue(40);
                            break;
                        default:
                            SetValue(10);
                            break;
                    }
                    return true;
                }
            }
        }

        public class OccultRatioOptionV2 : IntegerByOccultManagerOptionItem<ManagerSim, OccultRatioOptionV2.SubOption>
        {
            public class SubOption : SubBaseOption
            {
                public SubOption()
                    : base(50)
                { }

                public override string GetTitlePrefix()
                {
                    return "TownOccultRatio";
                }

                protected override string GetPrompt()
                {
                    return Localize("Prompt", IsFemaleLocalization(), new object[] { OccultTypeHelper.GetLocalizedName(Occult) });
                }

                public override bool Progressed
                {
                    get { return false; }
                }

                protected override int GetDefaultValue()
                {
                    return 50;
                }
            }
        }

        public interface IImmigrationEmigrationOption : INotRootLevelOption
        { }

        public class ImmigrationEmigrationListingOption : NestingManagerOptionItem<ManagerSim, IImmigrationEmigrationOption>
        {
            public ImmigrationEmigrationListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "ImmigrationEmigrationListing";
            }
        }

        protected class SimDemographics
        {
            public readonly Dictionary<ulong, SimDescription> mAll = null;

            public readonly List<SimDescription> mHumans = new List<SimDescription>();
            public readonly List<SimDescription> mPets = new List<SimDescription>();
            public readonly List<SimDescription> mOccults = new List<SimDescription>();
            
            readonly Dictionary<OccultTypes, int> mOccultByType = new Dictionary<OccultTypes, int>();

            public readonly List<SimDescription> mAdults = new List<SimDescription>();
            public readonly List<SimDescription> mAdultMales = new List<SimDescription>();
            public readonly List<SimDescription> mAdultFemales = new List<SimDescription>();

            public readonly List<SimDescription> mResidentMales = new List<SimDescription>();
            public readonly List<SimDescription> mResidentFemales = new List<SimDescription>();

            public readonly List<SimDescription> mNextGeneration = new List<SimDescription>();

            public readonly List<SimDescription> mChildren = new List<SimDescription>();

            public readonly List<SimDescription> mTeens = new List<SimDescription>();
            public readonly List<SimDescription> mTeenMales = new List<SimDescription>();
            public readonly List<SimDescription> mTeenFemales = new List<SimDescription>();

            public readonly List<SimDescription> mTeensAndAdults = new List<SimDescription>();

            public readonly Dictionary<CASAgeGenderFlags,int> mResidentCount = new Dictionary<CASAgeGenderFlags,int>();

            public readonly Dictionary<uint, int> mCelebrityCount = new Dictionary<uint, int>();

            public SimDemographics()
            {
                mAll = SimListing.GetResidents(false);

                mHumans.Clear();
                mPets.Clear();
                mOccultByType.Clear();

                mAdults.Clear();
                mAdultMales.Clear();
                mAdultFemales.Clear();

                mTeensAndAdults.Clear();
                mTeens.Clear();
                mTeenMales.Clear();
                mTeenFemales.Clear();
                mNextGeneration.Clear();

                mChildren.Clear();
                mCelebrityCount.Clear();

                foreach (CASAgeGenderFlags species in Enum.GetValues(typeof(CASAgeGenderFlags)))
                {
                    if ((species & CASAgeGenderFlags.SpeciesMask) == CASAgeGenderFlags.None) continue;

                    mResidentCount[species] = 0;
                }

                List<ISimScoringCache> caches = Common.DerivativeSearch.Find<ISimScoringCache>();

                foreach (SimDescription sim in mAll.Values)
                {
                    foreach (ISimScoringCache cache in caches)
                    {
                        cache.UnloadCaches(sim);
                    }

                    if ((sim.LotHome != null) && (sim.Species != CASAgeGenderFlags.None))
                    {
                        mResidentCount[sim.Species]++;

                        if (sim.IsPregnant)
                        {
                            mResidentCount[sim.Species]++;
                        }
                    }

                    if (sim.CelebrityManager != null)
                    {
                        uint celebLevel = sim.CelebrityLevel;
                        if (celebLevel > CelebrityManager.HighestLevel)
                        {
                            celebLevel = CelebrityManager.HighestLevel;
                        }

                        int celebCount;
                        if (!mCelebrityCount.TryGetValue(celebLevel, out celebCount))
                        {
                            celebCount = 1;
                        }
                        else
                        {
                            celebCount++;
                        }

                        mCelebrityCount[celebLevel] = celebCount;
                    }

                    if (sim.IsHuman)
                    {
                        mHumans.Add(sim);

                        if (sim.LotHome != null)
                        {
                            bool found = false;
                            foreach(OccultTypes type in OccultTypeHelper.CreateList(sim))
                            {
                                int count = 0;
                                if (!mOccultByType.TryGetValue(type, out count))
                                {
                                    count = 0;
                                }

                                mOccultByType[type] = (count + 1);
                                found = true;
                            }

                            if (found)
                            {
                                mOccults.Add(sim);
                            }

                            if (sim.IsMale)
                            {
                                mResidentMales.Add(sim);
                            }
                            else
                            {
                                mResidentFemales.Add(sim);
                            }
                        }

                        if (sim.Child)
                        {
                            mChildren.Add(sim);
                        }

                        if (sim.Teen)
                        {
                            mTeens.Add(sim);

                            mTeensAndAdults.Add(sim);

                            if (sim.IsMale)
                            {
                                mTeenMales.Add(sim);
                            }
                            else
                            {
                                mTeenFemales.Add(sim);
                            }
                        }

                        if (sim.YoungAdultOrBelow)
                        {
                            mNextGeneration.Add(sim);
                        }

                        if (sim.YoungAdultOrAbove)
                        {
                            mTeensAndAdults.Add(sim);

                            mAdults.Add(sim);

                            if (sim.IsFemale)
                            {
                                mAdultFemales.Add(sim);
                            }
                            else
                            {
                                mAdultMales.Add(sim);
                            }
                        }
                    }
                    else
                    {
                        mPets.Add(sim);
                    }
                }
            }

            public int GetOccultCount(OccultTypes type)
            {
                int count;
                if (mOccultByType.TryGetValue(type, out count)) return count;

                return 0;
            }

            public override string ToString()
            {
                Common.StringBuilder text = new Common.StringBuilder();

                text += "\nAll = " + mAll.Count;
                text += "\nHumans = " + mHumans.Count;
                text += "\nPets = " + mPets.Count;
                text += "\nOccult = " + mOccultByType.Count;

                text += "\nAdults = " + mAdults.Count;
                text += "\nAdult Males = " + mAdultMales.Count;
                text += "\nAdult Females = " + mAdultFemales.Count;

                text += "\nResident Males = " + mResidentMales.Count;
                text += "\nResident Females = " + mResidentFemales.Count;

                text += "\nNext Generation = " + mNextGeneration.Count;

                text += "\nTeens = " + mTeens.Count;
                text += "\nTeen Males = " + mTeenMales.Count;
                text += "\nTeen Females = " + mTeenFemales.Count;
                text += "\nTeen And Adults = " + mTeensAndAdults.Count;

                return text.ToString();
            }
        }

        public class AddInteractionsOption : BooleanManagerOptionItem<ManagerSim>
        {
            public static bool sQuickCheck = true;

            public AddInteractionsOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AddInteractions";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override void SetValue(bool value, bool persist)
            {
                base.SetValue(value, persist);

                sQuickCheck = value;
            }
        }

        public interface ITraitsLTWOption : INotRootLevelOption
        { }

        public class TraitsLTWOption : NestingManagerOptionItem<ManagerSim, ITraitsLTWOption>
        {
            public TraitsLTWOption()
            { }

            public override string GetTitlePrefix()
            {
                return "TraitsLTWListing";
            }
        }
    }
}

