using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Rewards;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.Gameplay.Opportunities
{
    [Persistable]
    public class OpportunityEx : Opportunity
    {
        SimDescription mTargetEx;

        public OpportunityEx()
        { }
        public OpportunityEx(Opportunity.OpportunitySharedData sharedData)
            : base(sharedData)
        { }
        public OpportunityEx(OpportunityEx obj)
            : base(obj.SharedData)
        { }

        public List<RequirementInfoEx> GetTargetRequirements()
        {
            List<RequirementInfoEx> list = new List<RequirementInfoEx>();

            foreach (Opportunity.OpportunitySharedData.RequirementInfo info in mSharedData.mRequirementList)
            {
                if (info.mType == RequirementType.Undefined)
                {
                    List<string> entry = new List<string>(info.mData.Split(new char[] { ',' }));

                    List<RequirementTypeEx> types = new List<RequirementTypeEx>();

                    if (entry.Count > 0)
                    {
                        RequirementTypeEx type;
                        ParserFunctions.TryParseEnum<RequirementTypeEx>(entry[0], out type, RequirementTypeEx.Undefined);

                        RequirementInfoEx data = null;

                        if (type != RequirementTypeEx.Undefined)
                        {
                            switch (type)
                            {
                                case RequirementTypeEx.TargetCareer:
                                    data = new CareerRequirementInfo(entry);
                                    break;
                                case RequirementTypeEx.TargetAge:
                                    data = new AgeRequirementInfo(entry);
                                    break;
                                case RequirementTypeEx.TargetGender:
                                    data = new GenderRequirementInfo(entry);
                                    break;
                                case RequirementTypeEx.TargetService:
                                    data = new ServiceRequirementInfo(entry);
                                    break;
                                default:
                                    data = new GenericRequirementInfo(type);
                                    break;
                            }
                        }

                        if (data != null)
                        {
                            list.Add(data);
                        }
                    }
                }
            }

            return list;
        }

        public SimDescription TargetEx
        {
            get { return mTargetEx; }
        }

        public static SimDescription GetMatchingSim(SimDescription actor, List<RequirementInfoEx> requirements)
        {
            List<SimDescription> sims = new List<SimDescription>();
            foreach (SimDescription sim in Household.EverySimDescription())
            {
                if (sim == actor) continue;

                if (SimTypes.InServicePool(sim, ServiceType.GrimReaper)) continue;

                bool success = true;
                foreach (RequirementInfoEx requirement in requirements)
                {
                    if (!requirement.Matches(actor, sim))
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    sims.Add(sim);
                }
            }

            if (sims.Count > 0)
            {
                return RandomUtil.GetRandomObjectFromList(sims);
            }
            else
            {
                return null;
            }
        }

        public override bool IsAvailable(Sim s)
        {
            if (s == null)
            {
                return false;
            }

            if (!s.OpportunityManager.CheckRepeatOpportunity(RepeatLevel, Guid))
            {
                return false;
            }

            if (this.IsChildOfOrEqualTo(s.OpportunityManager.GetLastOpportunity(OpportunityCategory)) && !s.SimDescription.OpportunityHistory.HasCurrentOpportunity(OpportunityCategory, this.Guid))
            {
                return false;
            }

            if (GetMatchingSim(s.SimDescription, GetTargetRequirements()) == null)
            {
                return false;
            }

            foreach (OpportunitySharedData.RequirementInfo info in mSharedData.mRequirementList)
            {
                if (info.mType == RequirementType.Undefined) continue;

                if (!CheckRequirement(info, s, mSharedData))
                {
                    return false;
                }
            }

            if (mSharedData.mRequirementDelegate != null)
            {
                return mSharedData.mRequirementDelegate(s, this);
            }
            return true;
        }

        public ListenerAction SimCreatedEx(Event e)
        {
            try
            {
                if (TryResumeOpportunity(e.TargetObject as Sim))
                {
                    ResetListeners();

                    return ListenerAction.Remove;
                }
            }
            catch (Exception exception)
            {
                Common.Exception(e.Actor, e.TargetObject, exception);
            }

            return ListenerAction.Keep;
        }

        public virtual void ResetListeners()
        {
            if (TargetObject == null)
            {
                EventTracker.RemoveListener(mSimCreatedListener);
                mSimCreatedListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, SimCreatedEx);
            }
        }

        public override void PersistPostLoad()
        {
            base.PersistPostLoad();

            ResetListeners();
        }

        public override void OnAdded()
        {
            base.OnAdded();

            mTargetEx = GetMatchingSim(Actor.SimDescription, GetTargetRequirements());
            if (mTargetEx == null) return;

            if (mTargetEx.CreatedSim == null)
            {
                Instantiation.PerformOffLot(mTargetEx, Actor.LotHome, null);
                SpeedTrap.Sleep();
            }

            if (mTargetEx.CreatedSim != null)
            {
                TargetObject = mTargetEx.CreatedSim;

                AddTargetDeletedListener();

                EventTracker.RemoveListener(mCompletionListener);

                if (CompletionEvent != null)
                {
                    EventListener e = Actor.OpportunityManager.SetupOneListener(Actor, this, CompletionEvent, null);
                    if (e != null)
                    {
                        AddCompletionListener(e);
                        EventTracker.AddListener(e);
                    }
                }

                foreach (EventListener listener in mListenerList)
                {
                    EventTracker.RemoveListener(listener);
                }

                foreach (Opportunity.OpportunitySharedData.EventInfo info in EventList)
                {
                    EventListener listener2 = Actor.OpportunityManager.SetupOneListener(Actor, this, info, null);
                    if (listener2 != null)
                    {
                        listener2.CompletionDelegate = new OnCompletion(OpportunityTrackerModel.FireOpportunitiesChanged);
                        AddListener(listener2);
                        EventTracker.AddListener(listener2);
                    }
                }
            }

            ResetListeners();

            try
            {
                if (Actor != null)
                {
                    Relationship.Get(Actor.SimDescription, mTargetEx, true);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Actor.SimDescription, mTargetEx, e);
            }
        }

        public override void OnCompletion(out OpportunityNames triggerOpportunity)
        {
            bool opportunityWin = RewardsManager.CheckForWin(mSharedData.mCompletionWinChance, mSharedData.mModifierList, Actor);
            ArrayList rewardsList = opportunityWin ? mSharedData.mWinRewardsList : mSharedData.mLossRewardsList;

            RewardInfoEx.GiveRewards(Actor, TargetObject, rewardsList);

	        base.OnCompletion(out triggerOpportunity);
        }

        public override void OnFailed()
        {
            RewardInfoEx.GiveRewards(Actor, TargetObject, mSharedData.mFailureRewardsList);

         	base.OnFailed();
        }

        public override void OnLoss()
        {
            RewardInfoEx.GiveRewards(Actor, TargetObject, mSharedData.mLossRewardsList);

            base.OnLoss();
        }

        public abstract class RequirementInfoEx
        {
            public RequirementTypeEx mType;

            public RequirementInfoEx(RequirementTypeEx type)
            {
                mType = type;
            }

            public abstract bool Matches(SimDescription actor, SimDescription target);
        }

        public class AgeRequirementInfo : RequirementInfoEx
        {
            List<CASAgeGenderFlags> mAges = new List<CASAgeGenderFlags>();

            public AgeRequirementInfo(List<string> list)
                :base(RequirementTypeEx.TargetAge)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    CASAgeGenderFlags age;
                    ParserFunctions.TryParseEnum<CASAgeGenderFlags>(list[i], out age, CASAgeGenderFlags.None);
                    
                    if (age != CASAgeGenderFlags.None)
                    {
                        mAges.Add(age);
                    }
                }
            }

            public override bool Matches(SimDescription actor, SimDescription target)
            {
                return mAges.Contains(target.Age);
            }
        }

        public class GenderRequirementInfo : RequirementInfoEx
        {
            List<CASAgeGenderFlags> mGenders = new List<CASAgeGenderFlags>();

            public GenderRequirementInfo(List<string> list)
                :base(RequirementTypeEx.TargetGender)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    CASAgeGenderFlags gender;
                    ParserFunctions.TryParseEnum<CASAgeGenderFlags>(list[i], out gender, CASAgeGenderFlags.None);

                    if (gender != CASAgeGenderFlags.None)
                    {
                        mGenders.Add(gender);
                    }
                }
            }

            public override bool Matches(SimDescription actor, SimDescription target)
            {
                return mGenders.Contains(target.Age);
            }
        }

        public class CareerRequirementInfo : RequirementInfoEx
        {
            protected class CareerReq
            {
                public OccupationNames mName;
                public string mBranch;
            }

            List<CareerReq> mCareers = new List<CareerReq>();

            public CareerRequirementInfo(List<string> list)
                : base(RequirementTypeEx.TargetCareer)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    CareerReq career = new CareerReq();

                    List<string> data = new List<string> (list[i].Split(new char[] { ':' }));
                    
                    ParserFunctions.TryParseEnum<OccupationNames>(data[0], out career.mName, OccupationNames.Undefined);

                    if (career.mName == OccupationNames.Undefined)
                    {
                        career.mName = unchecked((OccupationNames)ResourceUtils.HashString64(data[0]));
                    }

                    if (data.Count == 2)
                    {
                        career.mBranch = data[1];
                    }

                    mCareers.Add(career);
                }
            }

            public override bool Matches(SimDescription actor, SimDescription target)
            {
                if (target.Occupation == null)
                {
                    return (mCareers.Count == 0);
                }

                foreach (CareerReq career in mCareers)
                {
                    if (career.mName == target.Occupation.Guid)
                    {
                        if (career.mBranch != null)
                        {
                            if (target.Occupation.CurLevelBranchName == career.mBranch) return true;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public class ServiceRequirementInfo : RequirementInfoEx
        {
            List<ServiceType> mServices = new List<ServiceType>();

            public ServiceRequirementInfo(List<string> list)
                : base(RequirementTypeEx.TargetCareer)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    ServiceType serviceType;
                    ParserFunctions.TryParseEnum<ServiceType>(list[0], out serviceType, ServiceType.None);

                    mServices.Add(serviceType);
                }
            }

            public override bool Matches(SimDescription actor, SimDescription target)
            {
                if (target.CreatedByService == null) return false;

                if (mServices.Count == 0) return true;

                foreach (ServiceType type in mServices)
                {
                    if (target.CreatedByService.ServiceType == type) return true;
                }

                return false;
            }
        }

        public class GenericRequirementInfo : RequirementInfoEx
        {
            public GenericRequirementInfo(RequirementTypeEx type)
                : base(type)
            { }

            public override bool Matches(SimDescription actor, SimDescription target)
            {
                switch (mType)
                {
                    case RequirementTypeEx.DifferentCareer:
                        if (actor.Occupation == null)
                        {
                            return false;
                        }
                        else if ((target.Occupation != null) && (target.Occupation.Guid == actor.Occupation.Guid))
                        {
                            return false;
                        }
                        break;
                    case RequirementTypeEx.DifferentBranch:
                        if (actor.Occupation == null)
                        {
                            return false;
                        }
                        else if ((target.Occupation != null) && (target.Occupation.CurLevelBranchName == actor.Occupation.CurLevelBranchName))
                        {
                            return false;
                        }
                        break;
                    case RequirementTypeEx.SameBranch:
                        if (actor.Occupation == null)
                        {
                            return false;
                        }
                        else if ((target.Occupation == null) || (target.Occupation.CurLevelBranchName != actor.Occupation.CurLevelBranchName))
                        {
                            return false;
                        }
                        break;
                    case RequirementTypeEx.SameCareer:
                        if (actor.Occupation == null)
                        {
                            return false;
                        }
                        else if ((target.Occupation == null) || (target.Occupation.Guid != actor.Occupation.Guid))
                        {
                            return false;
                        }
                        break;
                    case RequirementTypeEx.Coworker:
                        if (actor.Occupation == null)
                        {
                            return false;
                        }
                        else if (!actor.Occupation.Coworkers.Contains(target))
                        {
                            return false;
                        }
                        break;
                    case RequirementTypeEx.Boss:
                        if (actor.Occupation == null)
                        {
                            return false;
                        }
                        else if (actor.Occupation.Boss != target)
                        {
                            return false;
                        }
                        break;
                    case RequirementTypeEx.Alive:
                        if (target.IsDead)
                        {
                            return false;
                        }
                        break;
                    case RequirementTypeEx.Undead:
                        if ((!target.IsPlayableGhost) && (!target.IsMummy))
                        {
                            return false;
                        }
                        break;
                }

                return true;
            }
        }

        public enum RequirementTypeEx
        {
            Undefined,
            TargetAge,
            TargetCareer,
            TargetGender,
            SameCareer,
            DifferentCareer,
            SameBranch,
            DifferentBranch,
            TargetService,
            Alive,
            Undead,
            Coworker,
            Boss
        }
    }
}
