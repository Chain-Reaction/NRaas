using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
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
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Telemetry;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class ChangeCareerLevel : CareerOption
    {
        [Persistable]
        protected class Item : CommonOptionItem
        {
            public CareerLevel mLevel;

            public int mXPLevel;

            public Item(SimDescription sim, CareerLevel level)
                : base(level.GetLocalizedName(sim), level.Level)
            {
                mLevel = level;
            }
            public Item(string title, int level)
                : base(title, level)
            {
                mXPLevel = level;
            }

            public override string DisplayValue
            {
                get { return null; }
            }
        }

        public override string GetTitlePrefix()
        {
            return "CareerLevel";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return (me.Occupation != null);
        }

        public static List<CareerLevel> AllLevels(Sims3.Gameplay.Careers.Career job)
        {
            List<CareerLevel> list = new List<CareerLevel>();

            if (job != null)
            {
                CareerLevel level1 = job.Level1;
                if (level1 != null)
                {
                    list.Add(level1);

                    int index = 0;
                    while(index < list.Count)
                    {
                        CareerLevel level = list[index];
                        index++;

                        foreach (CareerLevel next in level.NextLevels)
                        {
                            list.Add(next);
                        }
                    }
                }
            }

            return list;
        }

        public static List<CareerLevel> AllBranches(CareerLevel level)
        {
            List<CareerLevel> branches = new List<CareerLevel>();

            while (level != null)
            {
                if (level.NextLevels.Count > 1)
                {
                    branches.Add(level);
                }

                level = level.LastLevel;
            }

            return branches;
        }

        public static List<CareerLevel> LevelsBetween(CareerLevel lowLevel, CareerLevel highLevel, bool reverse)
        {
            List<CareerLevel> order = new List<CareerLevel>();

            if ((lowLevel != null) && (highLevel != null))
            {
                order.Add(highLevel);

                CareerLevel curLevel = highLevel.LastLevel;
                while (curLevel != null)
                {
                    if (reverse)
                    {
                        order.Add(curLevel);
                    }
                    else
                    {
                        order.Insert(0, curLevel);
                    }

                    if (curLevel == lowLevel)
                    {
                        return order;
                    }

                    curLevel = curLevel.LastLevel;
                }
            }

            order.Clear();
            return order;
        }

        public static void Promote(Sims3.Gameplay.Careers.Career job, CareerLevel newLevel)
        {
            CareerLevel curLevel = job.CurLevel;

            int bonusAmount = job.GivePromotionBonus();
            job.GivePromotionRewardObjectsIfShould(newLevel);
            job.SetLevel(newLevel);
            job.OnPromoteDemote(curLevel, newLevel);
            if (job.OwnerDescription.CreatedSim != null)
            {
                job.SetTones(job.OwnerDescription.CreatedSim.CurrentInteraction);
            }

            job.ShowOccupationTNS(job.GeneratePromotionText(bonusAmount));
        }

        protected static ListenerAction OnStub(Event e)
        {
            return ListenerAction.Keep;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Sims3.Gameplay.Careers.Career career = me.Occupation as Sims3.Gameplay.Careers.Career;
            if (career != null)
            {
                List<CareerLevel> levels = AllLevels(career);
                if ((levels == null) || (levels.Count == 0)) return false;

                List<Item> allOptions = new List<Item>();
                foreach (CareerLevel level in levels)
                {
                    allOptions.Add(new Item(me, level));
                }

                Item choice = new CommonSelection<Item>(Name, me.FullName, allOptions).SelectSingle();
                if (choice == null) return false;

                List<CareerLevel> order = null;
                if (choice.mLevel.Level < me.Occupation.CareerLevel)
                {
                    order = LevelsBetween(choice.mLevel, career.CurLevel, true);
                }
                else
                {
                    order = LevelsBetween(career.CurLevel, choice.mLevel, false);
                }

                if (order.Count == 0)
                {
                    List<CareerLevel> oldBranches = AllBranches(career.CurLevel);
                    List<CareerLevel> newBranches = AllBranches(choice.mLevel);

                    CareerLevel common = null;
                    foreach (CareerLevel branch in newBranches)
                    {
                        if (oldBranches.Contains(branch))
                        {
                            common = branch;
                            break;
                        }
                    }

                    if (common != null)
                    {
                        order.AddRange(LevelsBetween(common, career.CurLevel, true));
                        order.AddRange(LevelsBetween(common, choice.mLevel, false));
                    }
                }

                foreach (CareerLevel level in order)
                {
                    if (career.CurLevel == level) continue;

                    if (career.CurLevel.LastLevel == level)
                    {
                        career.DemoteSim();
                    }
                    else
                    {
                        Promote(career, level);
                    }
                }
                return true;
            }
            else
            {
                XpBasedCareer xpCareer = me.Occupation as XpBasedCareer;
                if (xpCareer != null)
                {
                    List<Item> allOptions = new List<Item>();
                    for (int level = 1; level <= xpCareer.HighestLevel; level++)
                    {
                        allOptions.Add(new Item(xpCareer.LevelJobTitle(level), level));
                    }

                    Item choice = new CommonSelection<Item>(Name, me.FullName, allOptions).SelectSingle();
                    if (choice == null) return false;

                    if (choice.mXPLevel < me.Occupation.CareerLevel)
                    {
                        xpCareer.mLevel = choice.mXPLevel;
                        xpCareer.mXp = 0;
                    }
                    else
                    {
                        Dictionary<DelegateListener, ProcessEventDelegate> retain = new Dictionary<DelegateListener, ProcessEventDelegate>();

                        Dictionary<ulong, List<EventListener>> dictionary;
                        EventTracker.Instance.mListeners.TryGetValue((ulong)EventTypeId.kActiveCareerAdvanceLevel, out dictionary);

                        // Required to stop a hang when an inactive firefighter is promoted to level 10

                        if (me.Household != Household.ActiveHousehold)
                        {
                            foreach (List<EventListener> list in dictionary.Values)
                            {
                                foreach (EventListener listener in list)
                                {
                                    DelegateListener delListener = listener as DelegateListener;
                                    if (delListener != null)
                                    {
                                        RewardsManager.OccupationRewardInfo target = delListener.mProcessEvent.Target as RewardsManager.OccupationRewardInfo;
                                        if (target != null)
                                        {
                                            retain.Add(delListener, delListener.mProcessEvent);

                                            delListener.mProcessEvent = OnStub;
                                        }
                                    }
                                }
                            }
                        }

                        xpCareer.ForcePromoteToLevel(choice.mXPLevel);

                        if (me.Household != Household.ActiveHousehold)
                        {
                            foreach (List<EventListener> list in dictionary.Values)
                            {
                                foreach (EventListener listener in list)
                                {
                                    DelegateListener delListener = listener as DelegateListener;
                                    if (delListener != null)
                                    {
                                        RewardsManager.OccupationRewardInfo target = delListener.mProcessEvent.Target as RewardsManager.OccupationRewardInfo;
                                        if (target != null)
                                        {
                                            delListener.mProcessEvent = retain[delListener];
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return true;
                }
            }
            return false;
        }
    }
}
