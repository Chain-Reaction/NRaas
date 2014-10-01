using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Options.Romance;
using NRaas.WoohooerSpace.Scoring;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Skills
{
    [Persistable]
    public class KamaSimtraSettings
    {
        [Tunable, TunableComment("Number of Renown points received per level")]
        public static int kRenownPerLevel = 2;
        [Tunable, TunableComment("Increase in chance of a positive experience per level")]
        public static int kSatisfactionPerLevel = 5;
        [Tunable, TunableComment("Number of Renown points received per completed opportunity")]
        public static int kRenownPerOpportunity = 1;
        [Tunable, TunableComment("Ratio of Renown to Woohoo buff mood value")]
        public static float kRenownToMoodMultiple = 2.5f;
        [Tunable, TunableComment("Ratio of Renown to Woohoo payment")]
        public static int kRenownToPaymentMultiple = 20;
        [Tunable, TunableComment("Time when the daily alarm for this skill fires, in fractional 24 hour time")]
        public static float kDailyAlarmTime = 2.5f;
        [Tunable, TunableComment("Change in relationship per day once certain goals are acheived")]
        public static int kDailyRelationshipChange = 1;
        [Tunable, TunableComment("Whether to allow skill gain amongst the inactive population")]
        public static bool kInactiveGain = true;
        [Tunable, TunableComment("Whether to only count a sim once in the statistics, rather than for each style and location used")]
        public static bool kDistinctSimStats = false;

        [Tunable, TunableComment("Minimum number of different women that must be shagged to be considered a Womanizer")]
        public static int kWomanizerMinNotches = 25;
        [Tunable, TunableComment("Minimum number of younger men that must be shagged to be considered a Cougar")]
        public static int kCougarMinNotches = 25;
        [Tunable, TunableComment("Minimum number of elder sims that must be shagged to be considered a Gold Digger")]
        public static int kGoldDiggerMinNotches = 25;
        [Tunable, TunableComment("Minimum number of sims that must be pay-shagged to be considered a Gigolo")]
        public static int kGigoloMinNotches = 25;
        [Tunable, TunableComment("Minimum number of total different sims shagged to be considered Promiscuous")]
        public static int kPromiscuousMinNotches = 25;
        [Tunable, TunableComment("Minimum number of witnessed shags to be considered Exhibitionist")]
        public static int kExhibitionistMinNotches = 25;
        [Tunable, TunableComment("Minimum number of woohoos per world to be considered Worldly")]
        public static int kWorldlyMinPerWorld = 10;
        [Tunable, TunableComment("Minimum number of woohoos per location to be considered Experienced")]
        public static int kExperiencedMinPerLocation = 5;
        [Tunable, TunableComment("Minimum number of satisying shags to be considered Casanova")]
        public static int kCasanovaMinNotches = 25;
        [Tunable, TunableComment("Minimum number of celebrity shags to be considered Starry Eyed")]
        public static int kStarryEyedMinNotches = 25;
        [Tunable, TunableComment("Average level of celebrity shags to be considered Starry Eyed")]
        public static int kStarryEyedAvgLevel = 3;
        [Tunable, TunableComment("Minimum number of occult shags to be considered Occultist")]
        public static int kOccultistMinNotches = 25;
        [Tunable, TunableComment("Minimum number of woohoos with someone other than your partner to be considered Cheater")]
        public static int kCheaterMinNotches = 10;
        [Tunable, TunableComment("Minimum number of woohoos with another sim's partner to be considered Bike")]
        public static int kBikeMinNotches = 10;
        [Tunable, TunableComment("Minimum number of casual woohoos to be considered an Easy Rider")]
        public static int kEasyRiderMinNotches = 25;
        [Tunable, TunableComment("Minimum number of service woohoos to be considered an Journeyman")]
        public static int kJourneymanMinNotches = 25;
        [Tunable, TunableComment("Minimum number of any type of woohoos to be considered an Prolific")]
        public static int kProlificMinNotches = 50;
        [Tunable, TunableComment("Minimum number of woohoos with your partner without cheating")]
        public static int kFidelityMinNotches = 25;
        [Tunable, TunableComment("Minimum number of woohoos with sims older than you")]
        public static int kPrecociousMinNotches = 25;
        [Tunable, TunableComment("Minimum number of woohoos with sims younger than you")]
        public static int kCradleRobberMinNotches = 25;
        [Tunable, TunableComment("Minimum number of woohoos with any type of non-occult shagged by an occult")]
        public static int kFreshMeatMinNotches = 25;
        //[Tunable, TunableComment("Minimum number of woohoos with different sims to acheive the Century club")]
        //public static int kCenturyMinNotches = 100;
        //[Tunable, TunableComment("Minimum number of woohoos to acheive the Millenium club")]
        //public static int kMilleniumMinNotches = 1000;
        [Tunable, TunableComment("Minimum number of stars required to acheive a Galaxy of Stars")]
        public static int kGalaxyOfStarsMinNotches = 500;
        [Tunable, TunableComment("Minimum number of each occult that must be mashed to count towards Monster Masher")]
        public static int kMonsterMashMinNotchPerOccult = 5;
        [Tunable, TunableComment("Minimum number of each occult that must be put to rest to count towards Grave Robber")]
        public static int kGraveRobberMinNotchPerDeathType = 5;

        [Tunable, TunableComment("Minimum number of risky woohoos engaged in to achieve Risque status")]
        public static int kRisqueMinNotches = 50;
        [Tunable, TunableComment("Minimum number of different women that must be shagged to be considered a Man Eater")]
        public static int kManEaterMinNotches = 25;
        [Tunable, TunableComment("Minimum number of risky woohoos engaged in to achieve Cyber Junkie status")]
        public static int kCyberJunkieMinNotches = 25;
        [Tunable, TunableComment("Whether to show the Register As Professional Interaction on the phone")]
        public static bool kShowRegisterInteraction = true;
        [Tunable, TunableComment("How much the services of a professional cost at the Spa")]
        public static int kRendezvousCostPerLevel = 200;
        [Tunable, TunableComment("How much time is spent waiting for the other sim to arrive for a Rendevous")]
        public static int kRendezvousWaitPeriod = 60;
        [Tunable, TunableComment("How much time is spent during a Rendevous")]
        public static int kRendezvousDuration = 60;
        [Tunable, TunableComment("Whether to apply a random moodlet after a Rendezvous")]
        public static bool kRandomRendezvousMoodlet = true;
        [Tunable, TunableComment("Whether to show the Rendezvous Interactions")]
        public static bool kShowRendezvousInteraction = true;

        [Tunable, TunableComment("Base chance of a positive experience")]
        public static int kBaseSatisfaction = 0;
        [Tunable, TunableComment("Base chance of a negative experience")]
        public static int kBaseDisappointment = 0;
        [Tunable, TunableComment("Base number of minutes that a positive buff lasts")]
        public static int kBasePositiveBuffLength = 60;
        [Tunable, TunableComment("The minimum level for registering as a Professional")]
        public static int kRegisterSkillLevel = 1;
        [Tunable, TunableComment("The minimum level for Tantraport")]
        public static int kMinLevelTantraport = 8;

        [Tunable, TunableComment("Whether to display the cyber woohoo interactions")]
        public static bool kShowCyberWoohooInteraction = true;
        [Tunable, TunableComment("The duration in sim-minutes to wait before testing for a cyber climax")]
        public static int kCyberWoohooChanceToClimaxEveryXMinutes = 20;
        [Tunable, TunableComment("The chance a Cyber climax will occur")]
        public static int kCyberWoohooChanceToClimax = 10;
        [Tunable, TunableComment("The chance a Cyber woohoo may not be the proper gender")]
        public static int kCyberWoohooChanceOfMisunderstanding = 10;
        [Tunable, TunableComment("The base scoring chance of a sim liking cyber woohoo")]
        public static int kCyberWoohooBaseChanceScoring = 0;

        [Tunable, TunableComment("The number of skill points to achieve the next level in skill in a comma separated format")]
        public static int[] kSkillPoints = new int[0];

        [Tunable, TunableComment("Whether to display a notice when a sim receives a new notch")]
        public static bool kShowNotices = false;

        [Tunable, TunableComment("Whether to seed the service pool with professionals")]
        public static bool kSeedServicePool = false;

        public int mRenownPerLevel = kRenownPerLevel;
        public int mRenownPerOpportunity = kRenownPerOpportunity;
        public float mRenownToMoodMultiple = kRenownToMoodMultiple;
        public int mRenownToPaymentMultiple = kRenownToPaymentMultiple;
        public int mDailyRelationshipChange = kDailyRelationshipChange;
        public int mSatisfactionPerLevel = kSatisfactionPerLevel;
        public bool mInactiveGain = kInactiveGain;
        public bool mDistinctSimStats = kDistinctSimStats;

        public int mBaseSatisfaction = kBaseSatisfaction;
        public int mBaseDisappointment = kBaseDisappointment;
        public int mBasePositiveBuffLength = kBasePositiveBuffLength;
        public bool mShowRegisterInteraction = kShowRegisterInteraction;
        public int mRendezvousCostPerLevel = kRendezvousCostPerLevel;
        public int mRendezvousDuration = kRendezvousDuration;
        public int mRendezvousWaitPeriod = kRendezvousWaitPeriod;
        public bool mRandomRendezvousMoodlet = kRandomRendezvousMoodlet;
        public bool mShowRendezvousInteraction = kShowRendezvousInteraction;

        public bool mShowCyberWoohooInteraction = kShowCyberWoohooInteraction;
        public int mCyberWoohooChanceToClimaxEveryXMinutes = kCyberWoohooChanceToClimaxEveryXMinutes;
        public int mCyberWoohooChanceToClimax = kCyberWoohooChanceToClimax;
        public int mCyberWoohooChanceOfMisunderstanding = kCyberWoohooChanceOfMisunderstanding;
        public int mCyberWoohooBaseChanceScoring = kCyberWoohooBaseChanceScoring;
        public int mRegisterSkillLevel = kRegisterSkillLevel;
        public int mMinLevelTantraport = kMinLevelTantraport;

        public int mWomanizerMinNotches = kWomanizerMinNotches;
        public int mCougarMinNotches = kCougarMinNotches;
        public int mGoldDiggerMinNotches = kGoldDiggerMinNotches;
        public int mGigoloMinNotches = kGigoloMinNotches;
        public int mPromiscuousMinNotches = kPromiscuousMinNotches;
        public int mExhibitionistMinNotches = kExhibitionistMinNotches;
        public int mWorldlyMinPerWorld = kWorldlyMinPerWorld;
        public int mExperiencedMinPerLocation = kExperiencedMinPerLocation;
        public int mCasanovaMinNotches = kCasanovaMinNotches;
        public int mStarryEyedMinNotches = kStarryEyedMinNotches;
        public int mStarryEyedAverageLevel = kStarryEyedAvgLevel;
        public int mOccultistMinNotches = kOccultistMinNotches;
        public int mCheaterMinNotches = kCheaterMinNotches;
        public int mBikeMinNotches = kBikeMinNotches;
        public int mEasyRiderMinNotches = kEasyRiderMinNotches;
        public int mJourneymanMinNotches = kJourneymanMinNotches;
        public int mProlificMinNotches = kProlificMinNotches;
        public int mFidelityMinNotches = kFidelityMinNotches;
        public int mPrecociousMinNotches = kPrecociousMinNotches;
        public int mCradleRobberMinNotches = kCradleRobberMinNotches;
        public int mFreshMeatMinNotches = kFreshMeatMinNotches;
        //public int mCenturyMinNotches = kCenturyMinNotches;
        //public int mMilleniumMinNotches = kMilleniumMinNotches;
        public int mGalaxyOfStarsMinNotches = kGalaxyOfStarsMinNotches;
        public int mRisqueMinNotches = kRisqueMinNotches;
        public int mManEaterMinNotches = kManEaterMinNotches;
        public int mCyberJunkieMinNotches = kCyberJunkieMinNotches;
        public int mMonsterMashMinNotchPerOccult = kMonsterMashMinNotchPerOccult;
        public int mGraveRobberMinNotchPerDeathType = kGraveRobberMinNotchPerDeathType;

        private List<int> mSkillPoints = new List<int>(kSkillPoints);

        public bool mShowNotices = kShowNotices;

        public bool mSeedServicePool = kSeedServicePool;

        Dictionary<ulong, string> mAlias = new Dictionary<ulong, string>();

        public Dictionary<ulong, ServiceData> sRequests = new Dictionary<ulong, ServiceData>();        

        public int GetSkillPoints(int level)
        {
            Skill skill = SkillManager.GetStaticSkill(Skills.KamaSimtra.StaticGuid);
            if (skill == null)
            {
                return 1;
            }

            if (mSkillPoints == null)
            {
                mSkillPoints = new List<int>(kSkillPoints);
            }

            if (mSkillPoints.Count < skill.MaxSkillLevel)
            {
                int points = 0;

                for(int i=0; i<skill.MaxSkillLevel; i++)
                {
                    if (i >= mSkillPoints.Count)
                    {
                        mSkillPoints.Add(0);
                    }

                    int levelPoints = mSkillPoints[i];
                    if (levelPoints == 0)
                    {
                        levelPoints = skill.PointsForNextLevel[i] - points;
                    }

                    points += levelPoints;

                    mSkillPoints[i] = levelPoints;
                }
            }

            if (level < mSkillPoints.Count)
            {
                return mSkillPoints[level];
            }
            else
            {
                return 1;
            }
        }

        public void SetSkillPoints(int level, int points)
        {
            mSkillPoints[level] = points;

            ApplySkillPoints();
        }

        public void ApplySkillPoints()
        {
            Skill skill = SkillManager.GetStaticSkill(Skills.KamaSimtra.StaticGuid);
            if (skill == null) return;

            int points = 0;

            for (int i = 0; i < 10; i++)
            {
                points += GetSkillPoints(i);

                skill.PointsForNextLevel[i] = points;
            }
        }

        public string GetAlias(SimDescription sim)
        {
            string result;
            if (!mAlias.TryGetValue(sim.SimDescriptionId, out result))
            {
                if (GameUtils.IsInstalled(ProductVersion.EP5))
                {
                    if (sim.IsFemale)
                    {
                        result = SimUtils.GetRandomPetName(false, CASAgeGenderFlags.Cat, true);
                    }
                    else
                    {
                        result = SimUtils.GetRandomPetName(true, CASAgeGenderFlags.Dog, true);
                    }
                }
                else
                {
                    result = SimUtils.GetRandomGivenName(!sim.IsFemale, GameUtils.GetCurrentWorld());
                }

                mAlias.Add(sim.SimDescriptionId, result);
            }

            return result;
        }

        public ServiceData GetServiceData(ulong simDescID, bool createIfNotExists)
        {
            ServiceData data;
            if (sRequests.TryGetValue(simDescID, out data))
            {
                return data;
            }

            if (createIfNotExists)
            {
                return new ServiceData();
            }
            else
            {
                return null;
            }
        }

        public ServiceData FindServiceDataInvolvingProfessionalAndLot(ulong professionalID, ulong lotID)
        {
            foreach (KeyValuePair<ulong, ServiceData> data in sRequests)
            {
                if (data.Value.mProfessional == professionalID)
                {
                    SimDescription desc = SimDescription.Find(data.Key);
                    if (desc != null && desc.LotHomeId == lotID)
                    {
                        return data.Value;
                    }
                }
            }

            return null;
        }

        public void SetServiceData(ulong simDescId, ServiceData data)
        {
            sRequests[simDescId] = data;
        }
        
        public class ServiceData
        {
            public bool mWasRandom;
            public Common.AlarmTask mPush;
            public Common.AlarmTask mLotPush;
            public ulong mProfessional;
            public ulong mRequester;
            public int mAttempts;
            public int mLotPushAttempts;
            public bool autonomyWasDisabled;
            public CommonWoohoo.WoohooStyle mStyle;

            public ServiceData()
            {
                mWasRandom = false;
                mPush = null;
                mLotPush = null;
                mProfessional = 0;
                mRequester = 0;
                autonomyWasDisabled = false;
                mStyle = CommonWoohoo.WoohooStyle.Safe;
            }            

            public void DisableAutonomy()
            {
                if (this.mProfessional != 0)
                {                    
                    SimDescription desc = SimDescription.Find(this.mProfessional);
                    if (desc != null && desc.CreatedSim != null && desc.CreatedSim.Autonomy != null)
                    {
                        if (desc.CreatedSim.Autonomy.mAutonomyDisabledCount > 0)
                        {
                            // already disabled by the user
                            this.autonomyWasDisabled = true;
                            return;
                        }

                        desc.CreatedSim.Autonomy.mAutonomyDisabledCount = int.MaxValue / 2;
                    }
                }
            }

            public void CheckWoohoo()
            {                
                if (this.mAttempts == 20 && this.mPush != null && this.mPush.Valid)
                {
                    this.Dispose();
                    return;
                }

                this.mAttempts++;

                if (this.mProfessional != 0)
                {
                    SimDescription desc = SimDescription.Find(this.mProfessional);
                    SimDescription descClient = SimDescription.Find(this.mRequester);
                    if ((desc != null && desc.CreatedSim != null) && (descClient != null && descClient.CreatedSim != null))
                    {
                        if (desc.CreatedSim.LotCurrent != descClient.CreatedSim.LotHome)
                        {
                            this.Dispose();
                            return;
                        }

                        IWooHooDefinition def = null;
                        if (desc.CreatedSim.CurrentInteraction != null)
                        {
                            def = desc.CreatedSim.CurrentInteraction.InteractionDefinition as IWooHooDefinition;
                        }

                        if (def == null)
                        {
                            new CommonWoohoo.PushWoohoo(desc.CreatedSim, descClient.CreatedSim, false, mStyle);
                        }
                    }
                    else
                    {
                        this.Dispose();
                        return;
                    }
                }
            }

            public void CheckArrival()
            {
                if (this.mLotPushAttempts >= 5)
                {
                    this.Dispose();
                    return;
                }

                if (this.mProfessional != 0)
                {
                    SimDescription desc = SimDescription.Find(this.mProfessional);
                    if (desc != null)
                    {
                        if (desc.CreatedSim == null)
                        {
                            Instantiation.Perform(desc, null);
                        }

                        if (desc.CreatedSim != null)
                        {
                            SimDescription descClient = SimDescription.Find(this.mRequester);
                            if (descClient != null && descClient.LotHome != null && desc.CreatedSim.LotCurrent != descClient.LotHome)
                            {
                                SimEx.GetSimToSimHome(desc, descClient.CreatedSim, new Callback(GoToLotSuccessEx));
                                this.mLotPushAttempts++;
                            }
                        }
                    }
                    else
                    {
                        this.Dispose();
                        return;
                    }
                }
            }

            public static void GoToLotSuccessEx(Sim sim, float f)
            {
                if (sim.LotCurrent == null)
                {
                    return;
                }

                ServiceData data = KamaSimtra.Settings.FindServiceDataInvolvingProfessionalAndLot(sim.SimDescription.SimDescriptionId, sim.LotCurrent.LotId);
                SimDescription client = SimDescription.Find(data.mRequester);
                if (data != null && client != null && client.CreatedSim != null)
                {
                    Relationship relationship = Relationship.Get(client, sim.SimDescription, true);
                    if (relationship != null)
                    {
                        relationship.STC.Set(client.CreatedSim, sim, CommodityTypes.Amorous, 500f);
                        client.CreatedSim.InteractionQueue.CancelAllInteractions();                        
                        while (client.CreatedSim.CurrentInteraction != null)
                        {
                            Common.Sleep(0);
                        }                              
                        
                        data.SetupAlarm();
                        data.DisableAutonomy();                        

                        client.CreatedSim.GreetSimOnMyLotIfPossible(sim);
                        CommonWoohoo.WoohooStyle style = CommonWoohoo.WoohooStyle.Safe;
                        if (!Woohooer.Settings.ReplaceWithRisky && TwoButtonDialog.Show(Woohooer.Localize("FriskyConfirm:Prompt", sim.IsFemale, new object[] { sim, client.CreatedSim }), Woohooer.Localize("FriskyConfirm:Yes", sim.IsFemale, new object[] { sim, client.CreatedSim }), Woohooer.Localize("FriskyConfirm:No", sim.IsFemale, new object[] { sim, client.CreatedSim })))
                        {
                            style = CommonWoohoo.WoohooStyle.Risky;
                        }
                        data.mStyle = style;
                        KamaSimtra.Settings.SetServiceData(data.mRequester, data);

                        new CommonWoohoo.PushWoohoo(sim, client.CreatedSim, false, style);

                        StyledNotification.Format format = new StyledNotification.Format(Common.Localize("OrderServices:Arrived", sim.IsFemale), sim.ObjectId, client.CreatedSim.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
                        StyledNotification.Show(format);
                    }
                }
            }        

            public void SetupAlarm()
            {
                this.mPush = new Common.AlarmTask(5, TimeUnit.Minutes, CheckWoohoo, 10, TimeUnit.Minutes);
            }

            public void SetupPushAlarm()
            {
                this.mLotPush = new Common.AlarmTask(10, TimeUnit.Minutes, CheckArrival, 10, TimeUnit.Minutes);
            }

            public void SetupDisposeAlarm()
            {
                new Common.AlarmTask(5, TimeUnit.Minutes, this.Cleanup);
            }

            public void Cleanup()
            {                
                if (this.mProfessional != 0)
                {
                    SimDescription desc = SimDescription.Find(this.mProfessional);
                    if (desc != null && desc.CreatedSim != null)
                    {                        
                        VisitSituation.MakeSimExitToYard(desc.CreatedSim.LotCurrent, desc.CreatedSim);
                        Sim.MakeSimGoHome(desc.CreatedSim, false, new InteractionPriority(InteractionPriorityLevel.UserDirected));
                    }
                }

                if (this.mRequester != 0)
                {
                    KamaSimtra.Settings.sRequests.Remove(this.mRequester);
                }
            }

            public void Dispose()
            {                
                if (this.mPush != null && this.mPush.Valid)
                {
                    this.mPush.Dispose();
                }

                if (this.mLotPush != null && this.mLotPush.Valid)
                {
                    this.mLotPush.Dispose();
                }

                if (this.mProfessional != 0)
                {
                    SimDescription desc = SimDescription.Find(this.mProfessional);
                    if (desc != null && desc.CreatedSim != null && desc.CreatedSim.Autonomy != null)
                    {                        
                        if (!this.autonomyWasDisabled && desc.CreatedSim.Autonomy.mAutonomyDisabledCount > 0)
                        {
                            desc.CreatedSim.Autonomy.mAutonomyDisabledCount = 0;
                        }

                        SetupDisposeAlarm();                        
                    }                    
                }               
            }
        }
    }
}
