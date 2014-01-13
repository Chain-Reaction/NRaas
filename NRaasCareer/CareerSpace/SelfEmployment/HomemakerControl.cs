using NRaas.CommonSpace.Helpers;
using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Careers;
using NRaas.CareerSpace.Metrics;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CareerSpace.SelfEmployment
{
    public class HomemakerControl : Common.IWorldLoadFinished
    {
        static Dictionary<ObjectGuid, Dictionary<ulong, bool>> sDirtyChecks = new Dictionary<ObjectGuid, Dictionary<ulong, bool>>();

        static Dictionary<BuffNames, Dictionary<ulong, bool>> sBuffChecks = new Dictionary<BuffNames, Dictionary<ulong, bool>>();

        static Dictionary<ulong, bool> sHygiene = new Dictionary<ulong, bool>();

        protected static List<Homemaker> GetCareerOfGuardian(Event e)
        {
            return GetCareerOfGuardian(e, true);
        }
        protected static List<Homemaker> GetCareerOfGuardian(Event e, bool mustBeChild)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null) return null;

            if (mustBeChild)
            {
                if (actor.SimDescription.YoungAdultOrAbove) return null;
            }

            if (actor.Occupation is Homemaker) return null;

            return GetCareerForHousehold(e);
        }

        protected static List<Homemaker> GetCareerForSim(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor != null)
            {
                Homemaker career = actor.Occupation as Homemaker;
                if (career != null)
                {
                    List<Homemaker> careers = new List<Homemaker>();
                    careers.Add(career);
                    return careers;
                }
            }

            return null;
        }

        protected static List<Homemaker> GetCareerForHousehold(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor != null)
            {
                return GetCareerForHousehold(actor.Household);
            }

            return null;
        }
        protected static List<Homemaker> GetCareerForHousehold(Household house)
        {
            if (house != null)
            {
                List<Homemaker> careers = new List<Homemaker>();

                foreach (SimDescription parent in Households.Humans(house))
                {
                    Homemaker career = parent.Occupation as Homemaker;
                    if (career == null) continue;

                    careers.Add(career);
                }

                return careers;
            }

            return null;
        }

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kSocialInteraction, OnSocialEvent);

            new Common.DelayedEventListener(EventTypeId.kReadBook, OnReadBook);

            new Common.DelayedEventListener(EventTypeId.kHasNegativeEnvironmentScore, OnNegativeEnvironment);

            new Common.DelayedEventListener(EventTypeId.kSawDirtyObject, OnSawDirtyObject);
            new Common.DelayedEventListener(EventTypeId.kSawPuddleOrBurntSpot, OnSawDirtyObject);
            new Common.DelayedEventListener(EventTypeId.kSawPetVomit, OnSawDirtyObject);
            new Common.DelayedEventListener(EventTypeId.kSawFullTrashCan, OnSawDirtyObject);
            new Common.DelayedEventListener(EventTypeId.kSawEmptyPetBowl, OnSawDirtyObject);
            new Common.DelayedEventListener(EventTypeId.kSawEmptyOrDirtyDishes, OnSawDirtyObject);
            new Common.DelayedEventListener(EventTypeId.kSawBrokenObject, OnSawDirtyObject);
            new Common.DelayedEventListener(EventTypeId.kSawTrashPile, OnSawDirtyObject);
            new Common.DelayedEventListener(EventTypeId.kSawUnmadeBed, OnSawDirtyObject);

            new Common.DelayedEventListener(EventTypeId.kHadFireAtHome, OnUnsafeConditions);
            new Common.DelayedEventListener(EventTypeId.kGotFleas, OnUnsafeConditions);
            new Common.DelayedEventListener(EventTypeId.kWasInvolvedInAFire, OnUnsafeConditions);
            new Common.DelayedEventListener(EventTypeId.kWasRobbed, OnUnsafeConditions);

            new Common.DelayedEventListener(EventTypeId.kFought, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kSkippedWork, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kSkippingWork, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kPrankSchool, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kPullPrank, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kStoleObject, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kSnubbedSim, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kThrowEggs, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kSetBoobyTrap, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kPunishmentSnuckOut, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kPlayedWithFire, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kPlayedInToilet, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kHacked, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kDetonatedObject, OnBeingBad);
            new Common.DelayedEventListener(EventTypeId.kCopiedHomework, OnBeingBad);

            new Common.DelayedEventListener(EventTypeId.kBladderFailure, OnMotiveFailure);
            new Common.DelayedEventListener(EventTypeId.kSimPassedOut, OnMotiveFailure);

            new Common.DelayedEventListener(EventTypeId.kAgedUpWell, OnAgedUpWell);

            new Common.DelayedEventListener(EventTypeId.kSignedUpForAfterschoolActivity, OnSignedUpForAfterschoolActivity);
            new Common.DelayedEventListener(EventTypeId.kSignUpChildForAfterschoolActivity, OnSignedUpForAfterschoolActivity);

            new Common.DelayedEventListener(EventTypeId.kFinishedHomework, OnFinishedHomework);

            new Common.DelayedEventListener(EventTypeId.kHelpedWithHomework, OnHelpedWithHomework);

            new Common.DelayedEventListener(EventTypeId.kGotBuff, OnGotBuff);

            new Common.DelayedEventListener(EventTypeId.kBoughtObject, OnBoughtObject);

            new Common.DelayedEventListener(EventTypeId.kBrushedTeeth, OnImprovedHygiene);
            new Common.DelayedEventListener(EventTypeId.kWashedHands, OnImprovedHygiene);

            new Common.DelayedEventListener(EventTypeId.kEventTakeBath, OnImprovedHygieneOneADay);
            new Common.DelayedEventListener(EventTypeId.kEventTakeShower, OnImprovedHygieneOneADay);

            new Common.DelayedEventListener(EventTypeId.kWashedOneDish, OnCleanedUp);
            new Common.DelayedEventListener(EventTypeId.kMopped, OnCleanedUp);
            new Common.DelayedEventListener(EventTypeId.kMadeBed, OnCleanedUp);
            new Common.DelayedEventListener(EventTypeId.kDidLaundry, OnCleanedUp);
            new Common.DelayedEventListener(EventTypeId.kCleanedObject, OnCleanedUp);
            new Common.DelayedEventListener(EventTypeId.kCleanedOrThrewAwayObject, OnCleanedUp);
            new Common.DelayedEventListener(EventTypeId.kCleanMinorPet, OnCleanedUp);
            new Common.DelayedEventListener(EventTypeId.kTookOutTrash, OnCleanedUp);

            new Common.DelayedEventListener(EventTypeId.kRepairedObject, OnRepaired);
            new Common.DelayedEventListener(EventTypeId.kUpgradedObject, OnRepaired);

            new Common.DelayedEventListener(EventTypeId.kCookedMeal, OnCookedMeal);

            new Common.DelayedEventListener(EventTypeId.kFilledPetBowl, OnMaintainedHome);
            new Common.DelayedEventListener(EventTypeId.kFillWaterTrough, OnMaintainedHome);
            new Common.DelayedEventListener(EventTypeId.kWateredPlant, OnMaintainedHome);
            new Common.DelayedEventListener(EventTypeId.kWeededPlant, OnMaintainedHome);
            new Common.DelayedEventListener(EventTypeId.kGrewPlantToMaturity, OnMaintainedHome);

            new Common.DelayedEventListener(EventTypeId.kSkillLevelUp, OnSkillLearned);
            new Common.DelayedEventListener(EventTypeId.kTookSkillClass, OnSkillLearned);

            new Common.DelayedEventListener(EventTypeId.kLearnedRecipe, OnRecipeLearned);
            new Common.DelayedEventListener(EventTypeId.kLearnTofuRecipe, OnRecipeLearned);

            new Common.DelayedEventListener(EventTypeId.kTutoredSim, OnTutored);

            new Common.DelayedEventListener(EventTypeId.kToddlerChat, OnPlayedWith);
            new Common.DelayedEventListener(EventTypeId.kToddlerTickle, OnPlayedWith);
            new Common.DelayedEventListener(EventTypeId.kToddlerAttackWithClaw, OnPlayedWith);
            new Common.DelayedEventListener(EventTypeId.kToddlerTossInAir, OnPlayedWith);
            new Common.DelayedEventListener(EventTypeId.kReadToSleep, OnPlayedWith);

            sBuffChecks.Clear();
            new Common.AlarmTask(6, TimeUnit.Hours, OnResetBuffChecks, 6f, TimeUnit.Hours);
        }

        public static void OnResetBuffChecks()
        {
            sBuffChecks.Clear();

            sHygiene.Clear();
        }

        public static void OnSocialEvent(Event e)
        {
            SocialEvent socialEvent = e as SocialEvent;
            if (socialEvent == null) return;

            if (socialEvent.WasRecipient) return;

            if (!socialEvent.WasAccepted) return;

            Sim actor = socialEvent.Actor as Sim;
            if (actor == null) return;

            Sim target = socialEvent.TargetObject as Sim;
            if (target == null) return;

            if (actor.Household != target.Household) return;

            if (target.SimDescription == null) return;

            List<Homemaker> careers = GetCareerForSim(e);
            if (careers == null)
            {
                careers = GetCareerOfGuardian(e);
            }

            if (careers == null) return;

            ActionData data = ActionData.Get(socialEvent.SocialName);
            if (data == null) return;

            switch (data.IntendedCommodityString)
            {
                case CommodityTypes.Friendly:
                case CommodityTypes.Funny:
                case CommodityTypes.Amorous:
                    Homemaker.AddMarks(careers, Homemaker.StipendValue.PositiveSocial, 1);
                    break;
                case CommodityTypes.Creepy:
                case CommodityTypes.Insulting:
                case CommodityTypes.Steamed:
                    Homemaker.AddMarks(careers, Homemaker.StipendValue.NegativeSocial, 1);
                    break;
            }
        }

        public static void OnSawDirtyObject(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null) return;

            if (actor.LotCurrent == null) return;

            Household house = actor.LotCurrent.Household;
            if (house == null) return;

            if (actor.Occupation is Homemaker) return;

            if (e.TargetObject != null)
            {
                List<Homemaker> careers = GetCareerForHousehold(house);
                if (careers != null)
                {
                    Dictionary<ulong, bool> observers;
                    if (!sDirtyChecks.TryGetValue(e.TargetObject.ObjectId, out observers))
                    {
                        observers = new Dictionary<ulong, bool>();
                        sDirtyChecks.Add(e.TargetObject.ObjectId, observers);
                    }

                    if (!observers.ContainsKey(e.Actor.SimDescription.SimDescriptionId))
                    {
                        observers.Add(e.Actor.SimDescription.SimDescriptionId, true);

                        Homemaker.AddMarks(careers, Homemaker.StipendValue.DirtyObject, 1);
                    }
                }
            }
            else
            {
                Homemaker.AddMarks(GetCareerForHousehold(house), Homemaker.StipendValue.NegativeEnvironment, 1);
            }
        }

        public static void OnReadBook(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null) return;

            Book book = e.TargetObject as Book;
            if (book == null) return;

            if (book is BookToddler)
            {
                Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.ReadBook, 1);
            }
            else if ((book.Data.ID == "PregnancyEricHW") || (book.Data.ID == "MorePregnancyEricHW"))
            {
                Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.ReadBook, 1);
            }
        }

        public static void OnNegativeEnvironment(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null) return;

            if (actor.LotCurrent == null) return;

            Household house = actor.LotCurrent.Household;
            if (house == null) return;

            if (actor.Occupation is Homemaker) return;

            Homemaker.AddMarks(GetCareerForHousehold(house), Homemaker.StipendValue.NegativeEnvironment, 1);
        }

        public static void OnUnsafeConditions(Event e)
        {
            Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.UnsafeConditions, 1);
        }

        public static void OnBeingBad(Event e)
        {
            Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.BeingBad, 1);
        }

        public static void OnMotiveFailure(Event e)
        {
            Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.MotiveFailure, 1);
        }

        public static void OnSignedUpForAfterschoolActivity(Event e)
        {
            Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.AfterschoolActivity, 1);
            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.AfterschoolActivity, 1);
        }

        public static void OnTutored(Event e)
        {
            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.Tutored, 1);
        }

        public static void OnPlayedWith(Event e)
        {
            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.PlayedWith, 1);
        }

        public static void OnRecipeLearned(Event e)
        {
            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.LearnedRecipe, 1);
        }

        public static void OnFinishedHomework(Event e)
        {
            Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.Homework, 1);
        }

        public static void OnHelpedWithHomework(Event e)
        {
            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.Homework, 1);
        }

        public static void OnAgedUpWell(Event e)
        {
            Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.AgedUpWell, 1);
        }

        public static void OnGotBuff(Event e)
        {
            HasGuidEvent<BuffNames> buffEvent = e as HasGuidEvent<BuffNames>;

            Dictionary<ulong, bool> sims;
            if (!sBuffChecks.TryGetValue(buffEvent.Guid, out sims))
            {
                sims = new Dictionary<ulong, bool>();
                sBuffChecks.Add(buffEvent.Guid, sims);
            }

            if (sims.ContainsKey(e.Actor.SimDescription.SimDescriptionId)) return;
            sims.Add(e.Actor.SimDescription.SimDescriptionId, true);

            List<Homemaker> careers = GetCareerForSim(e);
            if (careers != null)
            {
                switch (buffEvent.Guid)
                {
                    case BuffNames.StirCrazy:
                        if (careers[0].ImmuneToStirCrazy)
                        {
                            e.Actor.BuffManager.RemoveElement(buffEvent.Guid);
                        }
                        break;
                }

                if (HomemakerGoodParentMoodletBooter.IsBuff(buffEvent.Guid))
                {
                    Homemaker.AddMarks(careers, Homemaker.StipendValue.GoodMoodlet, 1);
                }
            }

            careers = GetCareerOfGuardian(e, false);
            if (careers != null)
            {
                if (HomemakerGoodChildMoodletBooter.IsBuff(buffEvent.Guid))
                {
                    Homemaker.AddMarks(careers, Homemaker.StipendValue.GoodMoodlet, 1);
                }
                else if (HomemakerBadMoodletBooter.IsBuff(buffEvent.Guid))
                {
                    Homemaker.AddMarks(careers, Homemaker.StipendValue.BadMoodlet, 1);
                }
            }

            if (HomemakerBadLotMoodletBooter.IsBuff(buffEvent.Guid))
            {
                Homemaker.AddMarks(GetCareerForHousehold(e.Actor.LotCurrent.Household), Homemaker.StipendValue.BadMoodlet, 1);
            }
            else if (HomemakerGoodLotMoodletBooter.IsBuff(buffEvent.Guid))
            {
                Homemaker.AddMarks(GetCareerForHousehold(e.Actor.LotCurrent.Household), Homemaker.StipendValue.GoodMoodlet, 1);
            }
        }

        public static void OnImprovedHygiene(Event e)
        {
            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.ImprovedHygiene, 1);
            Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.ImprovedHygiene, 1);
        }

        public static void OnImprovedHygieneOneADay(Event e)
        {
            if (sHygiene.ContainsKey(e.Actor.SimDescription.SimDescriptionId)) return;
            sHygiene.Add(e.Actor.SimDescription.SimDescriptionId, true);

            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.ImprovedHygiene, 1);
            Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.ImprovedHygiene, 1);
        }

        public static void OnCookedMeal(Event e)
        {
            IDish dish = e.TargetObject as IDish;
            if (dish == null) return;

            if (dish.GetNumServingsLeft() <= 1) return;

            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.CookedMeal, 1);
        }

        public static void OnRepaired(Event e)
        {
            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.RepairedObject, 1);
        }

        public static void OnMaintainedHome(Event e)
        {
            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.MantainedHome, 1);
        }

        public static void OnCleanedUp(Event e)
        {
            Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.ChildCleanedUp, 1);
            Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.ParentCleanedUp, 1);
        }

        public static void OnSkillLearned(Event e)
        {
            HasGuidEvent<SkillNames> skillEvent = e as HasGuidEvent<SkillNames>;

            if (HomemakerTaughtSkillBooter.IsSkill(skillEvent.Guid))
            {
                Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.TaughtChildSkill, 1);
            }

            if (HomemakerChildSkillBooter.IsSkill(skillEvent.Guid))
            {
                Homemaker.AddMarks(GetCareerOfGuardian(e), Homemaker.StipendValue.ChildLearnedSkill, 1);
            }

            if (HomemakerParentSkillBooter.IsSkill(skillEvent.Guid))
            {
                Homemaker.AddMarks(GetCareerForSim(e), Homemaker.StipendValue.ParentLearnedSkill, 1);
            }
        }

        public static void OnBoughtObject(Event e)
        {
            List<Homemaker> careers = GetCareerForHousehold(e);
            if (careers == null) return;

            bool discount = false;

            foreach (Homemaker homemaker in careers)
            {
                if (homemaker.HasDiscount)
                {
                    discount = true;
                    break;
                }
            }

            if (HomemakerDiscountObjectsBooter.IsObject(e.TargetObject.GetType()))
            {
                if (discount)
                {
                    float value = e.TargetObject.Value * (NRaas.Careers.Settings.mHomemakerDiscountRate / 100f);

                    GameObject obj = e.TargetObject as GameObject;
                    if (obj != null)
                    {
                        obj.ValueModifier -= (int)value;
                    }

                    careers[0].AddDiscount((int)value);
                }

                Homemaker.AddMarks(GetCareerForHousehold(e), Homemaker.StipendValue.BoughtObject, 1);
            }
        }
    }
}
