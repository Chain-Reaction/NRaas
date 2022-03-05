using NRaas.CareerSpace.Interactions;
using NRaas.CareerSpace.Situations;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Skills;
using NRaas.Gameplay.Opportunities;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Skills
{
    [Persistable]
    public class Assassination : CommonSkill<Assassination, Assassination.AssassinationMajorStat, Assassination.AssassinationMinorStat, Assassination.AssassinationOpportunity>, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Level at which the Poison interaction becomes available")]
        public static uint kStarvationInteractionLevel = 1;
        [Tunable, TunableComment("Level at which the Liquify interaction becomes available")]
        public static uint kDrowningInteractionLevel = 2;
        [Tunable, TunableComment("Level at which the Electrocute interaction becomes available")]
        public static uint kElectrocuteInteractionLevel = 3;
        [Tunable, TunableComment("Level at which the Burn interaction becomes available")]
        public static uint kBurnInteractionLevel = 4;
        [Tunable, TunableComment("Level at which the Curse interaction becomes available")]
        public static uint kCurseInteractionLevel = 5;
        [Tunable, TunableComment("Level at which the Thirst interaction becomes available")]
        public static uint kThirstInteractionLevel = 6;
        [Tunable, TunableComment("Level at which the Meteor interaction becomes available")]
        public static uint kMeteorInteractionLevel = 7;
        [Tunable, TunableComment("Level at which the Erase interaction becomes available")]
        public static uint kEraseInteractionLevel = 8;
        
        [Tunable, TunableComment("Total Random Kills to be considered a Psychopath")]
        private static int kPsychopathMinKills = 25;
        [Tunable, TunableComment("Total Contract Kills to be considered a Hitman")]
        private static int kHitmanMinKills = 25;
        [Tunable, TunableComment("Total Confirmed Kills to be considered a Reaper")]
        private static int kReaperMinKills = 25;
        [Tunable, TunableComment("Total Unconfirmed Kills to be considered a Ghost")]
        private static int kGhostMinKills = 25;
        [Tunable, TunableComment("Total Occult Kills to be considered a Helsing")]
        private static int kOccultMinKills = 25;
        [Tunable, TunableComment("Total Cop Kills to be considered a Cop Killer")]
        private static int kCopMinKills = 25;
        [Tunable, TunableComment("Total Criminal Kills to be considered a Vigilante")]
        private static int kVigilanteMinKills = 25;
        [Tunable, TunableComment("Total Unwitnessed Kills to be considered a Ninja")]
        private static int kNinjaMinKills = 25;
        [Tunable, TunableComment("Total Paparazzi Kills to be considered a Bodyguard")]
        private static int kBodyguardMinKills = 20;
        [Tunable, TunableComment("Total Witnessed Kills to be considered Infamous")]
        private static int kInfamousMinKills = 25;
        [Tunable, TunableComment("Average Celebrity Kill Level required to be considered a Fanatic")]
        private static int kFanaticMinLevel = 4;
        [Tunable, TunableComment("Minimum number of celebrities required to be considered for Fanatic")]
        private static int kFanaticMinKills = 20;
        [Tunable, TunableComment("Total Elder kills to be consiered an Angel of Mercy")]
        private static int kAngelOfMercyMinKills = 20;
        [Tunable, TunableComment("Number of celebrity points to grant on each witnessed kill")]
        private static int kCelebrityGainPerKill = 100;
        [Tunable, TunableComment("Increase in police aggression per confirmed kill")]
        private static int kAggressionPerConfirmed = 1000;
        [Tunable, TunableComment("Increase in police aggression per unconfirmed kill")]
        private static int kAggressionPerUnconfirmed = 100;
        [Tunable, TunableComment("Increase in police aggression per witnessed kill")]
        private static int kAggressionPerWitnessed = 9000;
        [Tunable, TunableComment("Increase in police aggression per cop kill")]
        private static int kAggressionPerCop = 19000;
        [Tunable, TunableComment("Increase in police aggression per political kill")]
        private static int kAggressionPerPolitico = 29000;
        [Tunable, TunableComment("Multiple applied to aggression based on the number of goals completed")]
        private static float kAggressionMultiplePerGoal = 1.20f;
        [Tunable, TunableComment("Number of aggression points for each 1% chance of intimidation")]
        private static int kChancePerAggression = 1000;
        [Tunable, TunableComment("Reduction in police aggression after each stay in jail")]
        private static int kReducedAggressionPerArrest = 50000;
        [Tunable, TunableComment("Aggression depreciation for each day not suspected of murder")]
        private static int kReducedAggressionPerDay = 1000;
        [Tunable, TunableComment("Reduction in police aggression for each criminal kill")]
        private static int kReducedAggressionPerVigilante = 5000;
        [Tunable, TunableComment("Bribe Inflation per day")]
        private static float kBribeInflation = 0.003f;
        [Tunable, TunableComment("Rabbithole Bribe Processing Factor")]
        private static float kRabbitHoleCorruption = 0.2f;      
        [Tunable, TunableComment("Change in relationship per day once certain goals are acheived")]
        private static int kDailyRelationshipChange = 5;
        [Tunable, TunableComment("Whether the kill interactions are autonomous")]
        private static bool kAutonomous = false;
        [Tunable, TunableComment("Whether to enable the ability to kill and be killed by teenagers")]
        private static bool kAllowTeen = false;
        [Tunable, TunableComment("Whether to allow aggression repossessions")]
        private static bool kAllowRepo = true;
        [Tunable, TunableComment("Whether to allow any form of aggression")]
        private static bool kAllowAggression = true;
        [Tunable, TunableComment("Whether to allow arrests performed by assassination")]
        private static bool kAllowArrest = true;

        [Tunable, TunableComment("The maximum long-term relationship to allow an autonomous assassination")]
        private static int kLikingGate = 0;
        [Tunable, TunableComment("Whether to allow autonomous blood relation kills")]
        private static bool kAllowAutonomousBlood = false;
        [Tunable, TunableComment("Whether to allow autonomous blood or step relation kills")]
        private static bool kAllowAutonomousClose = false;
        [Tunable, TunableComment("Whether to allow autonomous kills of active sims")]
        private static bool kAllowAutonomousActive = false;
        [Tunable, TunableComment("Whether to display notices when autonomous kills occur")]
        private static bool kShowAutonomousNotice = true;
        [Tunable, TunableComment("Whether to allow user-directed kills of the last guardian in a family")]
        private static bool kAllowLastGuardian = false;
        [Tunable, TunableComment("Whether to allow user-directed kills normally denied by StoryProgression")]
        private static bool kUserDirectedProgressionAllow = false;

        [Tunable, TunableComment("The cost of hiring an assassin to kill a sim")]
        private static int kHiringCost = 1000;
        
        [Persistable]
        public class AssassinationSettings
        {
            public int mPsychopathMinKills = kPsychopathMinKills;
            public int mHitmanMinKills = kHitmanMinKills;
            public int mReaperMinKills = kReaperMinKills;
            public int mGhostMinKills = kGhostMinKills;
            public int mOccultMinKills = kOccultMinKills;
            public int mCopMinKills = kCopMinKills;
            public int mVigilanteMinKills = kVigilanteMinKills;
            public int mNinjaMinKills = kNinjaMinKills;
            public int mBodyguardMinKills = kBodyguardMinKills;
            public int mInfamousMinKills = kInfamousMinKills;
            public int mFanaticMinLevel = kFanaticMinLevel;
            public int mFanaticMinKills = kFanaticMinKills;
            public int mAngelOfMercyMinKills = kAngelOfMercyMinKills;
            
            public int mAggressionPerConfirmed = kAggressionPerConfirmed;
            public int mAggressionPerUnconfirmed = kAggressionPerUnconfirmed;
            public int mAggressionPerWitnessed = kAggressionPerWitnessed;
            public int mAggressionPerCop = kAggressionPerCop;
            public int mAggressionPerPolitico = kAggressionPerPolitico;
            public float mAggressionMultiplePerGoal = kAggressionMultiplePerGoal;
            public int mChancePerAggression = kChancePerAggression;
            public int mReducedAggressionPerArrest = kReducedAggressionPerArrest;
            public int mReducedAggressionPerDay = kReducedAggressionPerDay;
            public int mReducedAggressionPerVigilante = kReducedAggressionPerVigilante;
            public float mBribeInflation = kBribeInflation;
            public float mRabbitHoleCorruption = kRabbitHoleCorruption;

            public int mCelebrityGainPerKill = kCelebrityGainPerKill;
            public int mDailyRelationshipChange = kDailyRelationshipChange;
            public bool mAutonomous = kAutonomous;
            public bool mAllowTeen = kAllowTeen;
            public bool mAllowRepo = kAllowRepo;
            public bool mAllowAggression = kAllowAggression;
            public bool mAllowArrest = kAllowArrest;

            public int mLikingGate = kLikingGate;
            public bool mAllowAutonomousBlood = kAllowAutonomousBlood;
            public bool mAllowAutonomousClose = kAllowAutonomousClose;
            public bool mAllowAutonomousActive = kAllowAutonomousActive;
            public bool mShowAutonomousNotice = kShowAutonomousNotice;
            public bool mAllowLastGuardian = kAllowLastGuardian;
            public bool mUserDirectedProgressionAllow = kUserDirectedProgressionAllow;

            public int mHiringCost = kHiringCost;
        }

        public class SimData
        {
            public ServiceType mService;
            public Role.RoleType mRole;

            public SimData()
            { }

            public void Update(SimDescription sim)
            {
                mService = ServiceType.None;
                if (SimTypes.InServicePool(sim))
                {
                    mService = sim.CreatedByService.ServiceType;
                }

                mRole = Role.RoleType.None;
                if (sim.AssignedRole != null)
                {
                    mRole = sim.AssignedRole.Type;
                }
            }
        }

        public enum AggressionType
        {
            Repossession,
            Taxes,
            Arrest,
            JobPerformance,
            Shakedown
        }

        static Common.MethodStore sStoryProgressionAllowDeath = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AllowDeath", new Type[] { typeof(SimDescription) });

        [PersistableStatic]
        static AssassinationSettings sSettings = null;

        static Dictionary<SimDescription.DeathType, uint> sTypes;

        static List<AggressionType> sAggressionTypes = new List<AggressionType>();

        static Dictionary<ulong, SimData> sData = new Dictionary<ulong, SimData>();

        static Dictionary<ulong, ulong> sWanted = new Dictionary<ulong, ulong>();

        Dictionary<SimDescription.DeathType,int> mKillTypes = new Dictionary<SimDescription.DeathType,int>();

        Dictionary<uint, int> mCelebrities = new Dictionary<uint, int>();

        Dictionary<ulong, ulong> mJobs = new Dictionary<ulong, ulong>();

        int mConfirmedKills;
        int mUnconfirmedKills;

        int mContractKills;
        int mRandomKills;

        int mWitnessedKills;
        int mCurrentNinjaKills;
        int mTotalNinjaKills;
        int mArrests;
        int mBribes;
        int mInflation;
        int mDaysSinceLastKill;

        int mCloseRelationKills;
        int mFriendKills;
        int mEnemyKills;
        int mLoverKills;
        int mServiceKills;
        int mPaparazziKills;
        int mOccultKills;
        int mPoliticalKills;
        int mCopKills;
        int mVigilanteKills;
        int mAssassinKills;
        int mAssassinLevelKills;
        int mElderKills;

        [Persistable(false)]
        Dictionary<ulong, bool> mPotentialConfirmed = new Dictionary<ulong, bool>();

        [Persistable(false)]
        Dictionary<ulong, bool> mNotedKills = new Dictionary<ulong, bool>();

        static Assassination()
        {
            foreach (AggressionType type in Enum.GetValues(typeof(AggressionType)))
            {
                sAggressionTypes.Add(type);
            }
        }

        public Assassination()
        { }
        public Assassination(SkillNames guid)
            : base(guid)
        { }

        protected override string LocalizationKey
        {
            get { return "NRaasAssassination"; }
        }

        public override bool ExportContent(IPropertyStreamWriter writer)
        {
            try
            {
                uint skillHash = GetSkillHash() + 1;

                int[] killTypeKeys = new int[mKillTypes.Count];
                int[] killTypeValues = new int[mKillTypes.Count];

                int index = 0;
                foreach (KeyValuePair<SimDescription.DeathType, int> type in mKillTypes)
                {
                    killTypeKeys[index] = (int)type.Key;
                    killTypeValues[index] = type.Value;
                    index++;
                }

                writer.WriteInt32(skillHash, killTypeKeys);
                skillHash++;

                writer.WriteInt32(skillHash, killTypeValues);
                skillHash++;

                uint[] celebrityKeys = new uint[mCelebrities.Count];
                int[] celebrityValues = new int[mCelebrities.Count];

                index = 0;
                foreach (KeyValuePair<uint, int> type in mCelebrities)
                {
                    celebrityKeys[index] = type.Key;
                    celebrityValues[index] = type.Value;
                    index++;
                }

                writer.WriteUint32(skillHash, celebrityKeys);
                skillHash++;

                writer.WriteInt32(skillHash, celebrityValues);
                skillHash++;

                writer.WriteInt32(skillHash, mConfirmedKills);
                skillHash++;

                writer.WriteInt32(skillHash, mUnconfirmedKills);
                skillHash++;

                writer.WriteInt32(skillHash, mContractKills);
                skillHash++;

                writer.WriteInt32(skillHash, mRandomKills);
                skillHash++;

                writer.WriteInt32(skillHash, mWitnessedKills);
                skillHash++;

                writer.WriteInt32(skillHash, mCurrentNinjaKills);
                skillHash++;

                writer.WriteInt32(skillHash, mTotalNinjaKills);
                skillHash++;

                writer.WriteInt32(skillHash, mArrests);
                skillHash++;

                writer.WriteInt32(skillHash, mBribes);
                skillHash++;

                writer.WriteInt32(skillHash, mInflation);
                skillHash++;

                writer.WriteInt32(skillHash, mDaysSinceLastKill);
                skillHash++;

                writer.WriteInt32(skillHash, mCloseRelationKills);
                skillHash++;

                writer.WriteInt32(skillHash, mFriendKills);
                skillHash++;

                writer.WriteInt32(skillHash, mEnemyKills);
                skillHash++;

                writer.WriteInt32(skillHash, mLoverKills);
                skillHash++;

                writer.WriteInt32(skillHash, mServiceKills);
                skillHash++;

                writer.WriteInt32(skillHash, mPaparazziKills);
                skillHash++;

                writer.WriteInt32(skillHash, mOccultKills);
                skillHash++;

                writer.WriteInt32(skillHash, mPoliticalKills);
                skillHash++;

                writer.WriteInt32(skillHash, mCopKills);
                skillHash++;

                writer.WriteInt32(skillHash, mVigilanteKills);
                skillHash++;

                writer.WriteInt32(skillHash, mAssassinKills);
                skillHash++;

                writer.WriteInt32(skillHash, mAssassinLevelKills);
                skillHash++;

                writer.WriteInt32(skillHash, mElderKills);
                skillHash++;

                ulong[] jobKeys = new ulong[mJobs.Count];
                ulong[] jobEmployers = new ulong[mJobs.Count];

                index = 0;
                foreach (KeyValuePair<ulong, ulong> type in mJobs)
                {
                    jobKeys[index] = type.Key;
                    jobEmployers[index] = type.Value;
                    index++;
                }

                writer.WriteUint64(skillHash, jobKeys);
                skillHash++;

                writer.WriteUint64(skillHash, jobEmployers);
                skillHash++;

                return base.ExportContent(writer);
            }
            catch (Exception e)
            {
                Common.Exception("ExportContent", e);
                return false;
            }
        }

        public override bool ImportContent(IPropertyStreamReader reader)
        {
            try
            {
                uint skillHash = GetSkillHash() + 1;

                int[] killTypeKeys;
                int[] killTypeValues;

                bool found = reader.ReadInt32(skillHash, out killTypeKeys);
                skillHash++;

                found = found & reader.ReadInt32(skillHash, out killTypeValues);
                skillHash++;

                mKillTypes.Clear();

                if (found)
                {
                    for (int i = 0; i < killTypeKeys.Length; i++)
                    {
                        mKillTypes.Add((SimDescription.DeathType)killTypeKeys[i], killTypeValues[i]);
                    }
                }

                uint[] celebrityKeys;
                int[] celebrityValues;

                found = reader.ReadUint32(skillHash, out celebrityKeys);
                skillHash++;

                found = found & reader.ReadInt32(skillHash, out celebrityValues);
                skillHash++;

                mCelebrities.Clear();

                if (found)
                {
                    for (int i = 0; i < celebrityKeys.Length; i++)
                    {
                        mCelebrities.Add(celebrityKeys[i], celebrityValues[i]);
                    }
                }

                reader.ReadInt32(skillHash, out mConfirmedKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mUnconfirmedKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mContractKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mRandomKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out  mWitnessedKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mCurrentNinjaKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mTotalNinjaKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mArrests, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mBribes, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mInflation, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mDaysSinceLastKill, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mCloseRelationKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mFriendKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mEnemyKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mLoverKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mServiceKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mPaparazziKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mOccultKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mPoliticalKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mCopKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mVigilanteKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mAssassinKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mAssassinLevelKills, 0);
                skillHash++;

                reader.ReadInt32(skillHash, out mElderKills, 0);
                skillHash++;

                ulong[] jobKeys;
                ulong[] jobEmployers;

                found = reader.ReadUint64(skillHash, out jobKeys);
                skillHash++;

                found = found & reader.ReadUint64(skillHash, out jobEmployers);
                skillHash++;

                mJobs.Clear();

                if (found)
                {
                    for (int i = 0; i < jobKeys.Length; i++)
                    {
                        mJobs.Add(jobKeys[i], jobEmployers[i]);
                    }
                }

                return base.ImportContent(reader);
            }
            catch (Exception e)
            {
                Common.Exception("ImportContent", e);
                return false;
            }
        }

        public static Dictionary<SimDescription.DeathType, uint> Types
        {
            get
            {
                if (sTypes == null)
                {
                    sTypes = new Dictionary<SimDescription.DeathType, uint>();
                    sTypes.Add(SimDescription.DeathType.Burn, Assassination.kBurnInteractionLevel);
                    sTypes.Add(SimDescription.DeathType.Electrocution, Assassination.kElectrocuteInteractionLevel);
                    sTypes.Add(SimDescription.DeathType.Starve, Assassination.kStarvationInteractionLevel);
                    sTypes.Add(SimDescription.DeathType.Drown, Assassination.kDrowningInteractionLevel);

                    if (GameUtils.IsInstalled(ProductVersion.EP1))
                    {
                        sTypes.Add(SimDescription.DeathType.MummyCurse, Assassination.kCurseInteractionLevel);
                    }

                    if (GameUtils.IsInstalled(ProductVersion.EP2))
                    {
                        sTypes.Add(SimDescription.DeathType.Meteor, Assassination.kMeteorInteractionLevel);
                    }

                    if (GameUtils.IsInstalled(ProductVersion.EP3))
                    {
                        sTypes.Add(SimDescription.DeathType.Thirst, Assassination.kThirstInteractionLevel);
                    }

                    sTypes.Add(SimDescription.DeathType.None, Assassination.kEraseInteractionLevel);
                }
                return sTypes;
            }
        }

        public void AddJob(Sim employer, Sim target)
        {
            mJobs[target.SimDescription.SimDescriptionId] = employer.SimDescription.SimDescriptionId;
        }

        protected static bool LastGuardianStanding(Sim target)
        {
            if (target.LotHome == null) return false;

            bool children = false;

            foreach (SimDescription member in Households.Humans(target.Household))
            {
                if (target.SimDescription == member) continue;

                if (member.TeenOrAbove)
                {
                    return false;
                }
                else
                {
                    children = true;
                }
            }

            return children;
        }

        public static bool Allow(Sim actor, Sim target, SimDescription.DeathType deathType, bool isAutonomous, bool direct, bool massDeath, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (actor.InteractionQueue == null)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Corrupt Actor");
                return false;
            }

            if (target.IsPet)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Pet Target");
                return false;
            }

            if (target.SimDescription.ChildOrBelow)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Child Target");
                return false;
            }

            if ((actor.SimDescription.Teen) || (target.SimDescription.Teen))
            {
                if (!Assassination.Settings.mAllowTeen)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Teen Denied");
                    return false;
                }
            }

            if (!actor.IsSelectable)
            {
                if ((target.IsSelectable) || (Assassination.Settings.mAllowAutonomousActive))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Inactive vs. Active");
                    return false;
                }
            }

            if (isAutonomous)
            {
                if (LastGuardianStanding(target))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Last Guardian");
                    return false;
                }

                if (!Assassination.Settings.mAutonomous)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Autonomous Denied");
                    return false;
                }

                float liking = 0;

                Relationship relation = Relationship.Get(actor, target, false);
                if (relation != null)
                {
                    liking = relation.CurrentLTRLiking;
                }

                if (liking > Assassination.Settings.mLikingGate)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Liking Gate Denied");
                    return false;
                }

                if (!Assassination.Settings.mAllowAutonomousBlood)
                {
                    if (Relationships.IsBloodRelated(actor.Genealogy, target.Genealogy, false))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Blood Denied");
                        return false;
                    }
                }

                if (!Assassination.Settings.mAllowAutonomousClose)
                {
                    if (Relationships.IsCloselyRelated(actor.SimDescription, target.SimDescription, false))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Closely Denied");
                        return false;
                    }
                }
            }

            Assassination skill;
            if (!direct)
            {                
                skill = actor.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
            }
            else
            {
                // for hiring someone to do the kill for you
                skill = target.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
            }

            if (skill == null)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("No Skill");
                return false;
            }

            if (deathType == SimDescription.DeathType.Thirst)
            {
                if ((actor.IsSimBot) || (target.IsSimBot))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Simbot Thirst");
                    return false;
                }
            }

            if ((deathType != SimDescription.DeathType.Thirst) || (!actor.SimDescription.IsVampire))
            {
                if (skill.SkillLevel < Types[deathType])
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Skill Too Low");
                    return false;
                }
            }

            if ((target.LotCurrent != actor.LotCurrent) /*|| (direct)*/)
            {
                if (!skill.IsReaper())
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not Reaper");
                    return false;
                }
            }

            if (actor == target)
            {
                if (massDeath) return false;

                if (deathType == SimDescription.DeathType.None)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Erase Denied");
                    return false;
                }
                else
                {
                    if (!skill.IsGhost())
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not Ghost");
                        return false;
                    }
                }
            }
            else
            {
                if (massDeath)
                {
                    if (!skill.IsPsychopath())
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not Psychopath");
                        return false;
                    }

                    if (target.LotCurrent == null)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Lot Fail");
                        return false;
                    }
                }
            }

            if (!CanBeKilled(target, ref greyedOutTooltipCallback))
            {
                return false;
            }

            if ((isAutonomous) || ((target.LotHome != null) && (!Assassination.Settings.mUserDirectedProgressionAllow)))
            {
                if (sStoryProgressionAllowDeath.Valid)
                {
                    if (!sStoryProgressionAllowDeath.Invoke<bool>(new object[] { target.SimDescription }))
                    {
                        greyedOutTooltipCallback = delegate { return Common.Localize("Assassination:ProgressionDenied", target.IsFemale); };
                        return false;
                    }
                }
            }

            if (!isAutonomous)
            {
                if (!Assassination.Settings.mAllowLastGuardian)
                {
                    if (LastGuardianStanding(target))
                    {
                        greyedOutTooltipCallback = delegate { return Common.Localize("AllowLastGuardian:Tooltip", target.IsFemale); };
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool CanBeKilled(Sim sim, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (SimTypes.InServicePool(sim.SimDescription, ServiceType.GrimReaper))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Is Reaper");
                return false;
            }

            if (HolographicProjectionSituation.IsSimHolographicallyProjected(sim))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Hologram");
                return false;
            }

            if (sim.SimDescription.IsDead)
            {
                greyedOutTooltipCallback = delegate
                {
                    return Common.Localize("Kill:IsAlreadyDead", sim.IsFemale, new object[0]);
                };
                return false;
            }

            if (sim.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton))
            {
                greyedOutTooltipCallback = delegate
                {
                    return Common.Localize("Kill:IsDying", sim.IsFemale, new object[0]);
                };
                return false;
            }

            if ((sim.LotCurrent == null) || (sim.LotCurrent.IsWorldLot))
            {
                greyedOutTooltipCallback = delegate
                {
                    return Common.Localize("Kill:OnWorldLot", sim.IsFemale, new object[0]);
                };
                return false;
            }

            return true;
        }

        public static AssassinationSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new AssassinationSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        public override Skill Clone(SimDescription owner)
        {
            Assassination skill = base.Clone(owner) as Assassination;

            skill.MergeTravelData(this);

            return skill;
        }

        public void IncArrest()
        {
            mArrests++;
        }

        // Externalized to StoryProgression
        public static void SetPotential(SimDescription sim, SimDescription target, bool erased)
        {
            try
            {
                if (Assassination.Settings.mAutonomous)
                {
                    Assassination skill = sim.SkillManager.AddElement(StaticGuid) as Assassination;
                    if (skill != null)
                    {
                        skill.AddPotentialKill(target, erased);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, target, e);
            }
        }

        public bool IsPotential(SimDescription sim)
        {
            return mPotentialConfirmed.ContainsKey(sim.SimDescriptionId);
        }

        public int GetPyschopathKills()
        {
            return mRandomKills;
        }

        public bool IsPsychopath()
        {
            return (GetPyschopathKills() >= Settings.mPsychopathMinKills);
        }

        public int GetHitmanKills()
        {
            return mContractKills;
        }

        public bool IsHitman()
        {
            return (GetHitmanKills() >= Settings.mHitmanMinKills);
        }

        public int GetGhostKills()
        {
            if (IsInfamous()) return 0;

            return mUnconfirmedKills;
        }

        public bool IsGhost()
        {
            return (GetGhostKills() >= Settings.mGhostMinKills);
        }

        public int GetReaperKills()
        {
            return mConfirmedKills;
        }

        public bool IsReaper()
        {
            return (GetReaperKills() >= Settings.mReaperMinKills);
        }

        public int GetHelsingKills()
        {
            return mOccultKills;
        }

        public int GetCopKills()
        {
            return (mCopKills + mPoliticalKills);
        }

        public bool IsCopKiller()
        {
            return (GetCopKills() >= Settings.mCopMinKills);
        }

        public int GetNinjaKills()
        {
            if (IsInfamous()) return 0;

            return mCurrentNinjaKills;
        }

        public bool IsNinja()
        {
            return (GetNinjaKills() >= Settings.mNinjaMinKills);
        }

        public int GetInfamousKills()
        {
            return mWitnessedKills;
        }

        public bool IsInfamous()
        {
            return (GetInfamousKills() >= Settings.mInfamousMinKills);
        }

        public int GetBodyGuardKills()
        {
            if (IsFanatic()) return 0;

            return mPaparazziKills;
        }

        public bool IsBodyGuard()
        {
            return (GetBodyGuardKills() >= Settings.mBodyguardMinKills);
        }

        public int GetFanaticKills()
        {
            int count = 0;

            foreach (KeyValuePair<uint, int> value in mCelebrities)
            {
                if (value.Key < Settings.mFanaticMinLevel) continue;

                count += value.Value;
            }

            return count;
        }

        public bool IsFanatic()
        {
            return (GetFanaticKills() >= Settings.mFanaticMinKills);
        }

        public int GetVigilanteKills()
        {
            if (IsCopKiller()) return 0;

            return mVigilanteKills + mAssassinKills;
        }

        public int GetBribes()
        {
            return mBribes;
        }

        public int GetMercyKills()
        {
            return mElderKills;
        }

        public void Bribe(int value)
        {
            mBribes += value;
        }

        public void ReduceAggression(int value)
        {
            mInflation -= value;
        }

        public int GetNetAggression()
        {
            int aggression = GetPoliceAggression();

            int net = aggression - GetBribes();

            net -= mArrests * Settings.mReducedAggressionPerArrest;

            net -= mVigilanteKills * Settings.mReducedAggressionPerVigilante;

            if (net < 0)
            {
                net = 0;
            }

            return net;
        }

        public int GetChanceOfIntimidation()
        {
            int net = GetNetAggression();

            int chance = (int)(net / (float)Settings.mChancePerAggression);

            if (chance < 0) return 0;

            if (chance > 100) return 100;

            return chance;
        }

        public int GetPoliceAggression()
        {
            int value = 0;

            int confirmedKills = mConfirmedKills;
            int unconfirmedKills = mUnconfirmedKills;
            int currentNinjaKills = mCurrentNinjaKills;

            confirmedKills -= currentNinjaKills;

            if (confirmedKills < 0)
            {
                currentNinjaKills = -confirmedKills;
                confirmedKills = 0;
            }
            else
            {
                currentNinjaKills = 0;
            }

            unconfirmedKills -= currentNinjaKills;

            if (unconfirmedKills < 0)
            {
                unconfirmedKills = 0;
            }

            value += (confirmedKills * Settings.mAggressionPerConfirmed);
            value += (unconfirmedKills * Settings.mAggressionPerUnconfirmed);

            value += (mWitnessedKills * Settings.mAggressionPerWitnessed);
            value += (mCopKills * Settings.mAggressionPerCop);
            value += (mPoliticalKills * Settings.mAggressionPerPolitico);

            float factor = 1;
            foreach (AssassinationOpportunity stat in AllOpportunities)
            {
                if (!stat.Completed) continue;

                int count = Math.Abs(stat.Aggression);

                for (int i = 0; i < count; i++)
                {
                    if (stat.Aggression < 0)
                    {
                        factor /= Settings.mAggressionMultiplePerGoal;
                    }
                    else
                    {
                        factor *= Settings.mAggressionMultiplePerGoal;
                    }
                }
            }

            value = (int)(value * factor);

            value -= (mDaysSinceLastKill * Settings.mReducedAggressionPerDay);

            value += mInflation;

            if (value < 0)
            {
                value = 0;
            }

            return value;
        }

        public int GetCelebrityLevelRatio()
        {
            int count = 0, total = 0;

            foreach (KeyValuePair<uint, int> value in mCelebrities)
            {
                count += value.Value;

                total += (int)(value.Key * value.Value);
            }

            if (count == 0) return 0;

            return (total / count);
        }

        protected void GetKillMethods(List<ObjectPicker.TabInfo> tabInfo)
        {
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            foreach (AssassinationMinorStat stat in MinorStats)
            {
                if (stat.Count == 0) continue;

                if (stat.RowType != AssassinationMinorStat.StatType.Method) continue;

                rowInfo.Add(CreateRow(stat));
            }

            if (rowInfo.Count == 0) return;

            tabInfo.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("KillMethodsTitle"), rowInfo));
        }

        protected void GetSpecialtyKills(List<ObjectPicker.TabInfo> tabInfo)
        {
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            foreach (AssassinationMinorStat stat in MinorStats)
            {
                if (stat.Count == 0) continue;

                if (stat.RowType != AssassinationMinorStat.StatType.Specialty) continue;

                rowInfo.Add(CreateRow(stat));
            }

            if (rowInfo.Count == 0) return;

            tabInfo.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("SpecialtyKillsTitle"), rowInfo));
        }

        public override List<ObjectPicker.TabInfo> SecondaryTabs
        {
            get
            {
                List<ObjectPicker.TabInfo> tabInfo = base.SecondaryTabs;

                GetKillMethods(tabInfo);
                GetSpecialtyKills(tabInfo);

                return tabInfo;
            }
        }

        public override void MergeTravelData(Skill paramSkill)
        {
            base.MergeTravelData(paramSkill);

            Assassination skill = paramSkill as Assassination;

            mKillTypes = new Dictionary<SimDescription.DeathType, int>();

            foreach (KeyValuePair<SimDescription.DeathType, int> type in skill.mKillTypes)
            {
                mKillTypes.Add(type.Key, type.Value);
            }

            mCelebrities = new Dictionary<uint, int>();

            foreach (KeyValuePair<uint, int> type in skill.mCelebrities)
            {
                mCelebrities.Add(type.Key, type.Value);
            }

            mConfirmedKills = skill.mConfirmedKills;
            mUnconfirmedKills = skill.mUnconfirmedKills;

            mContractKills = skill.mContractKills;
            mRandomKills = skill.mRandomKills;

            mWitnessedKills = skill.mWitnessedKills;
            mCurrentNinjaKills = skill.mCurrentNinjaKills;
            mTotalNinjaKills = skill.mTotalNinjaKills;
            mArrests = skill.mArrests;
            mBribes = skill.mBribes;
            mInflation = skill.mInflation;
            mDaysSinceLastKill = skill.mDaysSinceLastKill;

            mCloseRelationKills = skill.mCloseRelationKills;
            mFriendKills = skill.mFriendKills;
            mEnemyKills = skill.mEnemyKills;
            mLoverKills = skill.mLoverKills;
            mServiceKills = skill.mServiceKills;
            mPaparazziKills = skill.mPaparazziKills;
            mOccultKills = skill.mOccultKills;
            mPoliticalKills = skill.mPoliticalKills;
            mCopKills = skill.mCopKills;
            mVigilanteKills = skill.mVigilanteKills;
            mAssassinKills = skill.mAssassinKills;
            mAssassinLevelKills = skill.mAssassinLevelKills;
            mElderKills = skill.mElderKills;

            mPotentialConfirmed.Clear();
            mNotedKills.Clear();
        }

        public override bool OwnerCanAcquireSkill()
        {
            return base.SkillOwner.TeenOrAbove;
        }

        public void OnWorldLoadFinished()
        {
            try
            {
                sWanted.Clear();

                new Common.AlarmTask(2, DaysOfTheWeek.All, OnDailyAlarm);

                sData.Clear();
                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.SimDescription == null) continue;

                    if (sData.ContainsKey(sim.SimDescription.SimDescriptionId)) continue;

                    SimData data = new SimData();
                    sData.Add(sim.SimDescription.SimDescriptionId, data);
                    data.Update(sim.SimDescription);
                }

                new Common.DelayedEventListener(EventTypeId.kSimCommittedDisgracefulAction, OnDisgracefulAct);
                new Common.DelayedEventListener(EventTypeId.kSimInstantiated, OnInstantiated);
                new Common.DelayedEventListener(EventTypeId.kRoomChanged, OnRoomChanged);

                new Common.DelayedEventListener(EventTypeId.kSimDied, OnSimDied);
            }
            catch (Exception e)
            {
                Common.Exception("OnWorldLoadFinished", e);
            }
        }

        protected static void OnRoomChanged(Event e)
        {
            Sim sim = e.Actor as Sim;
            if ((sim != null) && (sim.LotCurrent != null) && (!sim.LotCurrent.IsWorldLot))
            {
                if (sim.Service is Police)
                {
                    foreach (Sim wanted in sim.LotCurrent.GetSims())
                    {
                        ulong lotId;
                        if (sWanted.TryGetValue(sim.SimDescription.SimDescriptionId, out lotId))
                        {
                            if (wanted.LotCurrent.LotId == lotId)
                            {
                                Common.DebugNotify("Police Arrived For " + wanted.FullName);

                                SimArrestSituationEx.Create(wanted);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    ulong lotId;
                    if (sWanted.TryGetValue(sim.SimDescription.SimDescriptionId, out lotId))
                    {
                        if (sim.LotCurrent.LotId != lotId)
                        {
                            sWanted.Remove(sim.SimDescription.SimDescriptionId);
                        }

                        foreach (Sim police in sim.LotCurrent.GetSims())
                        {
                            if (police.Service is Police)
                            {
                                Common.DebugNotify("Police On Scene For " + sim.FullName);

                                SimArrestSituationEx.Create(sim);
                                break;
                            }
                        }
                    }
                }
            }
        }

        protected static void OnInstantiated(Event e)
        {
            Sim sim = e.TargetObject as Sim;
            if (sim != null)
            {
                SimData data;
                if (!sData.TryGetValue(sim.SimDescription.SimDescriptionId, out data))
                {
                    data = new SimData();
                    sData.Add(sim.SimDescription.SimDescriptionId, data);
                }

                data.Update(sim.SimDescription);
            }
        }

        protected static void OnDailyAlarm()
        {
            try
            {
                List<SimDescription> sims = Household.AllSimsLivingInWorld();
                foreach (SimDescription sim in sims)
                {
                    if (sim.SkillManager == null) continue;

                    try
                    {
                        Assassination skill = sim.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                        if (skill == null) continue;

                        skill.TestPoliceAggression();

                        skill.InflateBribes();

                        if (Settings.mDailyRelationshipChange != 0)
                        {
                            foreach (Relationship relation in Relationship.Get(sim))
                            {
                                SimDescription other = relation.GetOtherSimDescription(sim);
                                if (other.ChildOrBelow) continue;

                                try
                                {
                                    int like = 0;

                                    foreach (AssassinationOpportunity opportunity in skill.AllOpportunities)
                                    {
                                        if (!opportunity.Completed) continue;

                                        like += opportunity.GetLikingChange(other);
                                    }

                                    if (like != 0)
                                    {
                                        relation.LTR.UpdateLiking(like * Settings.mDailyRelationshipChange);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(sim, other, e);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnDailyAlarm", e);
            }
        }

        protected static void OnDisgracefulAct(Event e)
        {
            DisgracefulActionEvent actionEvent = e as DisgracefulActionEvent;
            if (actionEvent != null)
            {
                if (actionEvent.DisgracefulActionType == DisgracefulActionType.Arrested)
                {
                    Assassination skill = e.Actor.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                    if (skill != null)
                    {
                        skill.IncArrest();
                    }
                }
            }
        }

        public static bool WasWitnessed(Lot lot, int roomId, SimDescription killer, SimDescription target, List<Sim> witnesses)
        {
            if (lot == null) return false;

            foreach (Sim witness in lot.GetAllActors())
            {
                if (witness.SimDescription == target) continue;

                if (killer != null)
                {
                    if (killer == witness.SimDescription) continue;

                    if (killer.Household == witness.Household) continue;
                }

                if ((witness.SimDescription.IsDead) && (!witness.SimDescription.IsPlayableGhost)) continue;

                if (witness.SimDescription.ToddlerOrBelow) continue;

                if (SimTypes.InServicePool(witness.SimDescription, ServiceType.GrimReaper)) continue;

                if (witness.RoomId != roomId) continue;

                if (witness.SkillManager.GetSkillLevel(Assassination.StaticGuid) > 0) continue;

                witnesses.Add(witness);
            }

            return (witnesses.Count > 0);
        }

        public static void AnimateWitnesses(Sim killer, List<Sim> witnesses)
        {
            TraitNames[] traits = new TraitNames[] { TraitNames.Evil, TraitNames.MeanSpirited, TraitNames.Coward };

            foreach (Sim witness in witnesses)
            {
                if (witness.IsSelectable) continue;

                if (witness.TraitManager.HasAnyElement(traits)) continue;

                if (killer != null)
                {
                    if (witness.LotHome == killer.LotHome) continue;

                    if (witness.Service is Police)
                    {
                        SimArrestSituationEx.Create(killer);
                    }
                }

                PhoneCell phone = witness.Inventory.Find<PhoneCell>();
                if (phone != null)
                {
                    witness.InteractionQueue.Add(new WitnessedMurderCall.Definition(killer).CreateInstance(phone, witness, new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true));
                }
                
                if (witness.LotCurrent != witness.LotHome)
                {
                    witness.RequestWalkStyle(Sim.WalkStyle.Run);

                    Sim.MakeSimGoHome(witness, false, new InteractionPriority(InteractionPriorityLevel.UserDirected));
                }
            }
        }

        protected static void OnSimDied(Event e)
        {
            SimDescription sim = null;

            SimDescriptionEvent simDescEvent = e as SimDescriptionEvent;
            if (simDescEvent != null)
            {
                sim = simDescEvent.SimDescription;
            }
            else
            {
                MiniSimDescriptionEvent miniSimEvent = e as MiniSimDescriptionEvent;
                if (miniSimEvent != null)
                {
                    sim = miniSimEvent.MiniSimDescription as SimDescription;
                }
                else
                {
                    Sim actor = e.Actor as Sim;
                    if (actor != null)
                    {
                        sim = actor.SimDescription;
                    }
                }
            }

            if (!sim.IsHuman) return;

            Urnstone grave = Urnstone.FindGhostsGrave(sim);

            if ((grave != null) && 
                (grave.LotCurrent != null) && 
                (sim.DeathStyle != SimDescription.DeathType.OldAge))
            {
                SimDescription primary = null;

                List<Sim> onLot = new List<Sim>(grave.LotCurrent.GetSims());
                List<Sim> killers = new List<Sim>(onLot);
                if ((Sim.ActiveActor != null) && (Sim.ActiveActor.SimDescription.TeenOrAbove) && (!killers.Contains(Sim.ActiveActor)))
                {
                    killers.Add(Sim.ActiveActor);
                }

                foreach (Sim killer in killers)
                {
                    Assassination skill = killer.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                    if (skill == null) continue;

                    if (skill.IsPotential(sim))
                    {
                        killers.Clear();
                        killers.Add(killer);

                        primary = killer.SimDescription;
                        break;
                    }
                }

                List<Sim> witnesses = new List<Sim>();
                bool witnessed = WasWitnessed(grave.LotCurrent, grave.RoomId, primary, grave.DeadSimsDescription, witnesses);

                Sim confirmed = null;

                foreach (Sim killer in killers)
                {
                    Assassination skill = null;

                    if (!Assassination.Settings.mAllowTeen)
                    {
                        if (killer.SimDescription.Teen) continue;
                    }

                    if (killer == Sim.ActiveActor)
                    {
                        skill = EnsureSkill(killer);
                    }
                    else
                    {
                        skill = killer.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                    }
                    if (skill == null) continue;

                    if (killer.LotCurrent == grave.LotCurrent)
                    {
                        bool allow = true;
                        if (skill.IsPotential(sim))
                        {
                            confirmed = killer;

                            sWanted[killer.SimDescription.SimDescriptionId] = grave.LotCurrent.LotId;
                        }
                        else if ((killer == Sim.ActiveActor) && (skill.SkillLevel == 0))
                        {
                            if (!AcceptCancelDialog.Show(Common.Localize("AssassinationAdd:Prompt", killer.IsFemale, new object[] { killer, sim })))
                            {
                                allow = false;
                            }
                            else
                            {
                                skill.AddPotentialKill(sim, false);
                            }
                        }

                        if (allow)
                        {
                            skill.AddActualKill(sim, skill.IsPotential(sim), witnessed);
                        }
                    }
                    else if (skill.IsPotential(sim))
                    {
                        skill.AddActualKill(sim, false, witnessed);
                    }
                }

                AnimateWitnesses(confirmed, witnesses);
            }
        }

        protected void InflateBribes()
        {
            mDaysSinceLastKill++;

            mInflation += (int)(GetNetAggression() * Settings.mBribeInflation);
        }

        protected void TestPoliceAggression()
        {
            if (Common.IsOnTrueVacation()) return;

            if (!Settings.mAllowAggression) return;

            int chance = GetChanceOfIntimidation();
            if (chance == 0) return;

            bool success = RandomUtil.RandomChance(GetChanceOfIntimidation());

            if (!success) return;

            int netAggression = GetNetAggression();
            if (netAggression < 1000) return;

            int funds = RandomUtil.GetInt(1000, 10000);
            if (funds > netAggression)
            {
                funds = netAggression;
            }

            AggressionType type = RandomUtil.GetRandomObjectFromList(sAggressionTypes);
 
            string story = null;

            switch(type)
            {
                case AggressionType.Arrest:
                    if (SkillOwner.CreatedSim != null)
                    {
                        SimArrestSituationEx.Create(SkillOwner.CreatedSim);

                        funds = 0;
                    }
                    break;
                case AggressionType.JobPerformance:
                    if (SkillOwner.Occupation is Career)
                    {
                        funds /= 10;

                        int performance = (int)SkillOwner.Occupation.Performance + 100;
                        if (funds > performance)
                        {
                            funds = performance;
                        }

                        (SkillOwner.Occupation as Career).mPerformance -= funds;

                        story = Common.Localize("Aggression:Career", SkillOwner.IsFemale, new object[] { SkillOwner, funds });

                        funds *= 10;
                    }
                    else if (SkillOwner.Occupation is XpBasedCareer)
                    {
                        XpBasedCareer xpCareer = SkillOwner.Occupation as XpBasedCareer;
                        if (xpCareer != null)
                        {
                            if (funds > xpCareer.Experience)
                            {
                                funds = (int)xpCareer.Experience;
                            }

                            xpCareer.mXp -= funds;

                            story = Common.Localize("Aggression:Xp", SkillOwner.IsFemale, new object[] { SkillOwner, funds });
                        }
                    }
                    else if ((SkillOwner.CareerManager != null) && (SkillOwner.CareerManager.School != null))
                    {
                        funds /= 10;

                        int performance = (int)SkillOwner.CareerManager.School.Performance + 100;
                        if (funds > performance)
                        {
                            funds = performance;
                        }

                        SkillOwner.CareerManager.School.mPerformance -= funds;

                        story = Common.Localize("Aggression:School", SkillOwner.IsFemale, new object[] { SkillOwner, funds });

                        funds *= 10;
                    }
                    break;
                case AggressionType.Repossession:
                    if (!Settings.mAllowRepo) return;

                    if ((SkillOwner.LotHome != null) && (SkillOwner.Household == Household.ActiveHousehold))
                    {
                        SkillOwner.Household.DelinquentFunds += (int)funds;

                        Repoman instance = Repoman.Instance;
                        if (instance != null)
                        {
                            instance.MakeServiceRequest(SkillOwner.LotHome, true, ObjectGuid.InvalidObjectGuid);

                            story = Common.Localize("Aggression:Repo", SkillOwner.IsFemale, new object[] { SkillOwner, funds });
                        }
                    }
                    break;
                case AggressionType.Shakedown:
                    if (SkillOwner.Household != null)
                    {
                        if (funds > SkillOwner.FamilyFunds)
                        {
                            funds = SkillOwner.FamilyFunds;
                        }

                        story = Common.Localize("Aggression:Shakedown", SkillOwner.IsFemale, new object[] { SkillOwner, funds });
                    }
                    break;
                case AggressionType.Taxes:
                    if (SkillOwner.Household == Household.ActiveHousehold)
                    {
                        Bill bill = GlobalFunctions.CreateObjectOutOfWorld("Bill") as Bill;
                        if (bill != null)
                        {
                            bill.Amount = (uint)funds;
                            Mailbox mailboxOnLot = Mailbox.GetMailboxOnLot(SkillOwner.LotHome);
                            if (mailboxOnLot != null)
                            {
                                mailboxOnLot.AddMail(bill, false);
                                bill.OriginatingHousehold = SkillOwner.Household;

                                story = Common.Localize("Aggression:Taxes", SkillOwner.IsFemale, new object[] { SkillOwner, funds });
                            }
                            else
                            {
                                bill.Destroy();

                                funds = 0;
                            }
                        }
                    }
                    break;
            }

            if (funds > 0)
            {
                ReduceAggression(funds);

                if ((story != null) && (SkillOwner.Household == Household.ActiveHousehold))
                {
                    Common.Notify(story, SkillOwner.CreatedSim.ObjectId);
                }
            }
        }

        public void AddPotentialKill(SimDescription target, bool erased)
        {
            if (mPotentialConfirmed.ContainsKey(target.SimDescriptionId)) return;

            mPotentialConfirmed.Add(target.SimDescriptionId, erased);
        }

        protected static bool HadCareer(SimDescription sim, OccupationNames career)
        {
            if (sim.CareerManager == null) return false;
           
            if ((sim.Occupation != null) && (sim.Occupation.Guid == career))
            {
                return true;
            }
            else if ((sim.CareerManager.RetiredCareer != null) && (sim.CareerManager.RetiredCareer.Guid == career))
            {
                return true;
            }
            else
            {
                foreach (Occupation quit in sim.CareerManager.QuitCareers.Values)
                {
                    if (quit.Guid == career)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AddActualKill(SimDescription target, bool confirmed, bool witnessed)
        {
            if (target == SkillOwner) return;

            if (mNotedKills.ContainsKey(target.SimDescriptionId)) return;
            mNotedKills.Add(target.SimDescriptionId, true);

            ulong employerID;
            if (mJobs.TryGetValue(target.SimDescriptionId, out employerID))
            {
                SimDescription employer = SimDescription.Find(employerID);
                if ((employer != null) && (employer.Household != null))
                {
                    int cost = Assassination.Settings.mHiringCost;

                    if (cost > employer.FamilyFunds)
                    {
                        cost = employer.FamilyFunds;
                    }

                    employer.ModifyFunds(-cost);

                    SkillOwner.ModifyFunds(cost);
                }
            }

            mDaysSinceLastKill = 0;

            SimDescription.DeathType type = target.DeathStyle;
            if ((mPotentialConfirmed.ContainsKey(target.SimDescriptionId)) && (mPotentialConfirmed[target.SimDescriptionId]))
            {
                type = SimDescription.DeathType.None;  // Erased
            }

            if (type != SimDescription.DeathType.OldAge)
            {
                if (!mKillTypes.ContainsKey(type))
                {
                    mKillTypes.Add(type, 1);
                }
                else
                {
                    mKillTypes[type]++;
                }
            }

            SimData data;
            if (!sData.TryGetValue(target.SimDescriptionId, out data))
            {
                data = new SimData();
            }

            if (data.mService != ServiceType.None)
            {
                mServiceKills++;
            }
            else if (data.mRole != Role.RoleType.None)
            {
                if (data.mRole == Sims3.Gameplay.Roles.Role.RoleType.Paparazzi)
                {
                    mPaparazziKills++;
                }
                else
                {
                    mServiceKills++;
                }
            }

            if (confirmed)
            {
                mConfirmedKills++;

                if (witnessed)
                {
                    mWitnessedKills++;

                    mCurrentNinjaKills = 0;

                    if (GameUtils.IsInstalled(ProductVersion.EP3))
                    {
                        if (Settings.mCelebrityGainPerKill > 0)
                        {
                            SkillOwner.CelebrityManager.AddPoints((uint)Settings.mCelebrityGainPerKill);
                        }
                    }
                }
                else
                {
                    mTotalNinjaKills++;

                    mCurrentNinjaKills++;
                }
            }
            else
            {
                mUnconfirmedKills++;
            }

            uint celebLevel = 0;
            try
            {
                celebLevel = target.CelebrityLevel;
            }
            catch
            { }

            if (celebLevel > 0)
            {
                if (mCelebrities.ContainsKey(celebLevel))
                {
                    mCelebrities[celebLevel]++;
                }
                else
                {
                    mCelebrities.Add(celebLevel, 1);
                }
            }

            if (HadCareer(target, OccupationNames.Political))
            {
                mPoliticalKills++;
            }

            bool cop = HadCareer(target, OccupationNames.LawEnforcement);

            bool criminal = HadCareer(target, OccupationNames.Criminal);

            if ((cop) && (!criminal))
            {
                mCopKills++;
            }
            else if ((!cop) && (criminal))
            {
                mVigilanteKills++;
            }

            if (target.Elder)
            {
                mElderKills++;
            }

            if ((target.OccultManager != null) &&
                (target.OccultManager.HasAnyOccultType()))
            {
                mOccultKills++;
            }

            bool contracted = false;

            if ((SkillOwner.CreatedSim != null) && (SkillOwner.CreatedSim.OpportunityManager != null))
            {
                KillOpportunity opp = SkillOwner.CreatedSim.OpportunityManager.GetActiveOpportunity(Sims3.Gameplay.Opportunities.OpportunityCategory.Career) as KillOpportunity;
                if (opp != null)
                {
                    if (target == opp.TargetEx)
                    {
                        contracted = true;
                    }
                }
            }

            if (contracted)
            {
                mContractKills++;
            }
            else
            {
                mRandomKills++;
            }

            if ((SkillOwner.Genealogy != null) && (target.Genealogy != null))
            {
                if (Relationships.IsCloselyRelated(SkillOwner, target, false))
                {
                    mCloseRelationKills++;
                }
            }

            Relationship relation = Relationship.Get(SkillOwner, target, false);
            if (relation != null)
            {
                if (relation.AreRomantic())
                {
                    mLoverKills++;
                }
                else if (relation.AreFriends())
                {
                    mFriendKills++;
                }
                else if (relation.AreEnemies())
                {
                    mEnemyKills++;
                }
            }

            float previous = SkillOwner.SkillManager.OverallModifier;
            SkillOwner.SkillManager.mOverallModifier = 0;

            try
            {
                AddPoints(1, false);
            }
            finally
            {
                SkillOwner.SkillManager.mOverallModifier = previous;
            }

            int skillLevel = target.SkillManager.GetSkillLevel(StaticGuid);
            if (skillLevel > 0)
            {
                mAssassinKills++;

                mAssassinLevelKills += skillLevel;
            }

            TestForNewLifetimeOpp();
        }

        public int GetKillTypes(SimDescription.DeathType type)
        {
            if (!mKillTypes.ContainsKey(type)) return 0;

            return mKillTypes[type];
        }

        public abstract class AssassinationMajorStat : MajorStat
        {
            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }
        }

        public abstract class AssassinationMinorStat : MinorStat
        {
            public abstract StatType RowType
            {
                get;
            }

            public enum StatType
            {
                Specialty,
                Method,
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }
        }

        public class UnconfirmedKills : AssassinationMajorStat
        {
            public UnconfirmedKills()
            { }

            protected override string LocalizationKey
            {
                get { return "UnconfirmedKills"; }
            }

            public override int Count
            {
                get { return mSkill.mUnconfirmedKills; }
            }

            public override AssassinationMajorStat Clone()
            {
                return new UnconfirmedKills();
            }

            public override int Order
            {
                get { return 10; }
            }
        }

        public class ConfirmedKills : AssassinationMajorStat
        {
            public ConfirmedKills()
            { }

            protected override string LocalizationKey
            {
                get { return "ConfirmedKills"; }
            }

            public override int Count
            {
                get { return mSkill.mConfirmedKills; }
            }

            public override AssassinationMajorStat Clone()
            {
                return new ConfirmedKills();
            }

            public override int Order
            {
                get { return 20; }
            }
        }

        public class ContractKills : AssassinationMajorStat
        {
            public ContractKills()
            { }

            protected override string LocalizationKey
            {
                get { return "ContractKills"; }
            }

            public override int Count
            {
                get { return mSkill.mContractKills; }
            }

            public override AssassinationMajorStat Clone()
            {
                return new ContractKills();
            }

            public override int Order
            {
                get { return 30; }
            }
        }

        public class RandomKills : AssassinationMajorStat
        {
            public RandomKills()
            { }

            protected override string LocalizationKey
            {
                get { return "RandomKills"; }
            }

            public override int Count
            {
                get { return mSkill.mRandomKills; }
            }

            public override AssassinationMajorStat Clone()
            {
                return new RandomKills();
            }

            public override int Order
            {
                get { return 40; }
            }
        }

        public class CelebrityKills : AssassinationMinorStat
        {
            public CelebrityKills()
            { }

            protected override string LocalizationKey
            {
                get { return "CelebrityKills"; }
            }

            public override int Count
            {
                get 
                {
                    int count = 0;

                    foreach (int value in mSkill.mCelebrities.Values)
                    {
                        count += value;
                    }

                    return count;
                }
            }

            public override string Description
            {
                get { return mSkill.LocalizeString(LocalizationKey + "Description", new object[] { mSkill.GetCelebrityLevelRatio() }); }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP3)) return null;

                return new CelebrityKills();
            }
        }

        public class WitnessedKills : AssassinationMinorStat
        {
            public WitnessedKills()
            { }

            protected override string LocalizationKey
            {
                get { return "WitnessedKills"; }
            }

            public override int Count
            {
                get { return mSkill.mWitnessedKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new WitnessedKills();
            }
        }

        public class CurrentNinjaKills : AssassinationMinorStat
        {
            public CurrentNinjaKills()
            { }

            protected override string LocalizationKey
            {
                get { return "CurrentNinjaKills"; }
            }

            public override int Count
            {
                get { return mSkill.mCurrentNinjaKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new CurrentNinjaKills();
            }
        }

        public class TotalNinjaKills : AssassinationMinorStat
        {
            public TotalNinjaKills()
            { }

            protected override string LocalizationKey
            {
                get { return "TotalNinjaKills"; }
            }

            public override int Count
            {
                get { return mSkill.mTotalNinjaKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new TotalNinjaKills();
            }
        }

        public class CloseRelationKills : AssassinationMinorStat
        {
            public CloseRelationKills()
            { }

            protected override string LocalizationKey
            {
                get { return "CloseRelationKills"; }
            }

            public override int Count
            {
                get { return mSkill.mCloseRelationKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new CloseRelationKills();
            }
        }

        public class LoverKills : AssassinationMinorStat
        {
            public LoverKills()
            { }

            protected override string LocalizationKey
            {
                get { return "LoverKills"; }
            }

            public override int Count
            {
                get { return mSkill.mLoverKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new LoverKills();
            }
        }

        public class FriendKills : AssassinationMinorStat
        {
            public FriendKills()
            { }

            protected override string LocalizationKey
            {
                get { return "FriendKills"; }
            }

            public override int Count
            {
                get { return mSkill.mFriendKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new FriendKills();
            }
        }

        public class EnemyKills : AssassinationMinorStat
        {
            public EnemyKills()
            { }

            protected override string LocalizationKey
            {
                get { return "EnemyKills"; }
            }

            public override int Count
            {
                get { return mSkill.mEnemyKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new EnemyKills();
            }
        }

        public class BurnKills : AssassinationMinorStat
        {
            public BurnKills()
            { }

            protected override string LocalizationKey
            {
                get { return "BurnKills"; }
            }

            public override int Count
            {
                get { return mSkill.GetKillTypes(SimDescription.DeathType.Burn); }
            }

            public override StatType RowType
            {
                get { return StatType.Method; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new BurnKills();
            }
        }

        public class ThirstKills : AssassinationMinorStat
        {
            public ThirstKills()
            { }

            protected override string LocalizationKey
            {
                get { return "ThirstKills"; }
            }

            public override int Count
            {
                get { return mSkill.GetKillTypes(SimDescription.DeathType.Thirst); }
            }

            public override StatType RowType
            {
                get { return StatType.Method; }
            }

            public override AssassinationMinorStat Clone()
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP3)) return null;

                return new ThirstKills();
            }
        }

        public class MeteorKills : AssassinationMinorStat
        {
            public MeteorKills()
            { }

            protected override string LocalizationKey
            {
                get { return "MeteorKills"; }
            }

            public override int Count
            {
                get { return mSkill.GetKillTypes(SimDescription.DeathType.Meteor); }
            }

            public override StatType RowType
            {
                get { return StatType.Method; }
            }

            public override AssassinationMinorStat Clone()
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP2)) return null;

                return new MeteorKills();
            }
        }

        public class CopKills : AssassinationMinorStat
        {
            public CopKills()
            { }

            protected override string LocalizationKey
            {
                get { return "CopKills"; }
            }

            public override int Count
            {
                get { return mSkill.mCopKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new CopKills();
            }
        }

        public class PoliticalKills : AssassinationMinorStat
        {
            public PoliticalKills()
            { }

            protected override string LocalizationKey
            {
                get { return "PoliticalKills"; }
            }

            public override int Count
            {
                get { return mSkill.mPoliticalKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new PoliticalKills();
            }
        }

        public class VigilanteKills : AssassinationMinorStat
        {
            public VigilanteKills()
            { }

            protected override string LocalizationKey
            {
                get { return "VigilanteKills"; }
            }

            public override int Count
            {
                get { return mSkill.mVigilanteKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new VigilanteKills();
            }
        }

        public class ElectricKills : AssassinationMinorStat
        {
            public ElectricKills()
            { }

            protected override string LocalizationKey
            {
                get { return "ElectricKills"; }
            }

            public override int Count
            {
                get { return mSkill.GetKillTypes(SimDescription.DeathType.Electrocution); }
            }

            public override StatType RowType
            {
                get { return StatType.Method; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new ElectricKills();
            }
        }

        public class StarveKills : AssassinationMinorStat
        {
            public StarveKills()
            { }

            protected override string LocalizationKey
            {
                get { return "StarveKills"; }
            }

            public override int Count
            {
                get { return mSkill.GetKillTypes(SimDescription.DeathType.Starve); }
            }

            public override StatType RowType
            {
                get { return StatType.Method; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new StarveKills();
            }
        }

        public class DrownKills : AssassinationMinorStat
        {
            public DrownKills()
            { }

            protected override string LocalizationKey
            {
                get { return "DrownKills"; }
            }

            public override int Count
            {
                get { return mSkill.GetKillTypes(SimDescription.DeathType.Drown); }
            }

            public override StatType RowType
            {
                get { return StatType.Method; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new DrownKills();
            }
        }

        public class CurseKills : AssassinationMinorStat
        {
            public CurseKills()
            { }

            protected override string LocalizationKey
            {
                get { return "CurseKills"; }
            }

            public override int Count
            {
                get { return mSkill.GetKillTypes(SimDescription.DeathType.MummyCurse); }
            }

            public override StatType RowType
            {
                get { return StatType.Method; }
            }

            public override AssassinationMinorStat Clone()
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP1)) return null;

                return new CurseKills();
            }
        }

        public class EraseKills : AssassinationMinorStat
        {
            public EraseKills()
            { }

            protected override string LocalizationKey
            {
                get { return "EraseKills"; }
            }

            public override int Count
            {
                get { return mSkill.GetKillTypes(SimDescription.DeathType.None); }
            }

            public override StatType RowType
            {
                get { return StatType.Method; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new EraseKills();
            }
        }

        public class ServiceKills : AssassinationMinorStat
        {
            public ServiceKills()
            { }

            protected override string LocalizationKey
            {
                get { return "ServiceKills"; }
            }

            public override int Count
            {
                get { return mSkill.mServiceKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new ServiceKills();
            }
        }

        public class OccultKills : AssassinationMinorStat
        {
            public OccultKills()
            { }

            protected override string LocalizationKey
            {
                get { return "OccultKills"; }
            }

            public override int Count
            {
                get { return mSkill.mOccultKills; }
            }

            public override StatType RowType
            {
                get { return StatType.Specialty; }
            }

            public override AssassinationMinorStat Clone()
            {
                return new OccultKills();
            }
        }

        public class Arrests : AssassinationMajorStat
        {
            public Arrests()
            { }

            protected override string LocalizationKey
            {
                get { return "Arrests"; }
            }

            public override int Count
            {
                get { return mSkill.mArrests; }
            }

            public override AssassinationMajorStat Clone()
            {
                return new Arrests();
            }

            public override int Order
            {
                get { return 120; }
            }
        }

        public class PoliceAggression : AssassinationMajorStat
        {
            public PoliceAggression()
            { }

            protected override string LocalizationKey
            {
                get { return "PoliceAggression"; }
            }

            public override int Count
            {
                get { return mSkill.GetNetAggression(); }
            }

            public override AssassinationMajorStat Clone()
            {
                return new PoliceAggression();
            }

            public override int Order
            {
                get { return 100; }
            }
        }

        public class Bribes : AssassinationMajorStat
        {
            public Bribes()
            { }

            protected override string LocalizationKey
            {
                get { return "Bribes"; }
            }

            public override int Count
            {
                get { return mSkill.GetBribes(); }
            }

            public override AssassinationMajorStat Clone()
            {
                return new Bribes();
            }

            public override int Order
            {
                get { return 130; }
            }
        }

        public class ChanceOfIntimidation : AssassinationMajorStat
        {
            public ChanceOfIntimidation()
            { }

            protected override string LocalizationKey
            {
                get { return "ChanceOfIntimidation"; }
            }

            public override int Count
            {
                get { return mSkill.GetChanceOfIntimidation(); }
            }

            public override AssassinationMajorStat Clone()
            {
                return new ChanceOfIntimidation();
            }

            public override int Order
            {
                get { return 140; }
            }
        }

        public abstract class AssassinationOpportunity : CommonOpportunity
        {
            public AssassinationOpportunity()
            { }

            public abstract int Aggression
            {
                get;
            }

            public virtual int GetLikingChange(SimDescription sim)
            {
                return 0;
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }
        }

        public class Psychopath : AssassinationOpportunity
        {
            public Psychopath()
            { }

            protected override string LocalizationKey
            {
                get { return "Psychopath"; }
            }

            public override int Aggression
            {
                get { return 3; }
            }

            public override int MinValue
            {
                get { return Settings.mPsychopathMinKills; }
            }

            public override bool IsMajor
            {
                get { return true; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetPyschopathKills(); }
            }

            public override AssassinationOpportunity Clone()
            {
                return new Psychopath();
            }
        }

        public class Hitman : AssassinationOpportunity
        {
            public Hitman()
            { }

            protected override string LocalizationKey
            {
                get { return "Hitman"; }
            }

            public override int Aggression
            {
                get { return 2; }
            }

            public override int MinValue
            {
                get { return Settings.mHitmanMinKills; }
            }

            public override bool IsMajor
            {
                get { return true; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetHitmanKills(); }
            }

            public override AssassinationOpportunity Clone()
            {
                return new Hitman();
            }
        }

        public class Ghost : AssassinationOpportunity
        {
            public Ghost()
            { }

            protected override string LocalizationKey
            {
                get { return "Ghost"; }
            }

            public override int Aggression
            {
                get { return 1; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mGhostMinKills; }
            }

            public override bool IsMajor
            {
                get { return true; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetGhostKills(); }
            }

            public override AssassinationOpportunity Clone()
            {
                return new Ghost();
            }
        }

        public class Reaper : AssassinationOpportunity
        {
            public Reaper()
            { }

            protected override string LocalizationKey
            {
                get { return "Reaper"; }
            }

            public override int Aggression
            {
                get { return 2; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mReaperMinKills; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetReaperKills(); }
            }

            public override bool IsMajor
            {
                get { return true; }
            }

            public override AssassinationOpportunity Clone()
            {
                return new Reaper();
            }
        }

        public class Helsing : AssassinationOpportunity
        {
            public Helsing()
            { }

            protected override string LocalizationKey
            {
                get { return "Helsing"; }
            }

            public override int Aggression
            {
                get { return 1; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mOccultMinKills; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetHelsingKills(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if ((sim.OccultManager != null) &&
                    (sim.OccultManager.HasAnyOccultType()))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }

            public override AssassinationOpportunity Clone()
            {
                return new Helsing();
            }
        }

        public class CopKiller : AssassinationOpportunity
        {
            public CopKiller()
            { }

            protected override string LocalizationKey
            {
                get { return "CopKiller"; }
            }

            public override int Aggression
            {
                get { return 5; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mCopMinKills; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetCopKills(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                int like = 0;

                if (HadCareer(sim, OccupationNames.LawEnforcement))
                {
                    like--;
                }

                if (HadCareer(sim, OccupationNames.Political))
                {
                    like--;
                }

                if (HadCareer(sim, OccupationNames.Criminal))
                {
                    like++;
                }

                return like;
            }

            public override AssassinationOpportunity Clone()
            {
                return new CopKiller();
            }
        }

        public class Vigilante : AssassinationOpportunity
        {
            public Vigilante()
            { }

            protected override string LocalizationKey
            {
                get { return "Vigilante"; }
            }

            public override int Aggression
            {
                get { return -2; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mVigilanteMinKills; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetVigilanteKills(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                int like = 0;

                if (HadCareer(sim, OccupationNames.LawEnforcement))
                {
                    like++;
                }

                if (HadCareer(sim, OccupationNames.Political))
                {
                    like--;
                }

                if (HadCareer(sim, OccupationNames.Criminal))
                {
                    like--;
                }

                return like;
            }

            public override AssassinationOpportunity Clone()
            {
                return new Vigilante();
            }
        }

        public class Ninja : AssassinationOpportunity
        {
            public Ninja()
            { }

            protected override string LocalizationKey
            {
                get { return "Ninja"; }
            }

            public override bool IsMajor
            {
                get { return true; }
            }

            public override int Aggression
            {
                get { return -10; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mNinjaMinKills; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetNinjaKills(); }
            }

            public override AssassinationOpportunity Clone()
            {
                return new Ninja();
            }
        }

        public class Infamous : AssassinationOpportunity
        {
            public Infamous()
            { }

            protected override string LocalizationKey
            {
                get { return "Infamous"; }
            }

            public override int Aggression
            {
                get { return 3; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mInfamousMinKills; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetInfamousKills(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                int like = 0;

                if (HadCareer(sim, OccupationNames.LawEnforcement))
                {
                    like--;
                }

                if (HadCareer(sim, OccupationNames.Political))
                {
                    like--;
                }

                if (HadCareer(sim, OccupationNames.Criminal))
                {
                    like++;
                }

                return like;
            }

            public override AssassinationOpportunity Clone()
            {
                return new Infamous();
            }
        }

        public class Fanatic : AssassinationOpportunity
        {
            public Fanatic()
            { }

            protected override string LocalizationKey
            {
                get { return "Fanatic"; }
            }

            public override int Aggression
            {
                get { return 2; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mFanaticMinKills; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetFanaticKills(); }
            }

            public override bool Completed
            {
                get { return mSkill.IsFanatic(); }
            }

            public override string RewardDescription
            {
                get
                {
                    if (Completed)
                    {
                        return base.RewardDescription;
                    }
                    else
                    {
                        return mSkill.LocalizeString("Description" + LocalizationKey, new object[] { MinValue, Assassination.Settings.mFanaticMinLevel, mSkill.SkillOwner });
                    }
                }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.CelebrityLevel > 0)
                {
                    return -1;
                }

                return 0;
            }

            public override AssassinationOpportunity Clone()
            {
                return new Fanatic();
            }
        }
        
        public class BodyGuard : AssassinationOpportunity
        {
            public BodyGuard()
            { }

            protected override string LocalizationKey
            {
                get { return "BodyGuard"; }
            }

            public override int Aggression
            {
                get { return 1; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mBodyguardMinKills; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetBodyGuardKills(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.CelebrityLevel > 0)
                {
                    return 1;
                }

                return 0;
            }

            public override AssassinationOpportunity Clone()
            {
                return new BodyGuard();
            }
        }

        public class AngelOfMercy : AssassinationOpportunity
        {
            public AngelOfMercy()
            { }

            protected override string LocalizationKey
            {
                get { return "AngelOfMercy"; }
            }

            public override int Aggression
            {
                get { return 3; }
            }

            public override int MinValue
            {
                get { return Assassination.Settings.mAngelOfMercyMinKills; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetMercyKills(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.Elder)
                {
                    return -1;
                }
                else
                {
                    foreach (SimDescription parent in Relationships.GetParents(sim))
                    {
                        if (!parent.IsDead)
                        {
                            return -1;
                        }
                    }

                    return 1;
                }
            }

            public override AssassinationOpportunity Clone()
            {
                return new AngelOfMercy();
            }
        }
    }
}
