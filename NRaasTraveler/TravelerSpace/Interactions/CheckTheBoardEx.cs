using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace.Interactions
{
    public class CheckTheBoardEx : PostBoxJobBoard.CheckTheBoard, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<PostBoxJobBoard, PostBoxJobBoard.CheckTheBoard.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
                tuning.CodeVersion = new ProductVersion[] { ProductVersion.EP1, ProductVersion.EP9 };
                tuning.Availability.WorldRestrictionType = WorldRestrictionType.None;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition (true);

            PostBoxJobBoard.CheckTheBoardEP9.Singleton = new Definition(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<PostBoxJobBoard, PostBoxJobBoard.CheckTheBoard.Definition>(Singleton);
            interactions.Replace<PostBoxJobBoard, PostBoxJobBoard.CheckTheBoardEP9.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (!Target.RouteToCone(Actor))
                {
                    return false;
                }

                StandardEntry();

                try
                {
                    BeginCommodityUpdates();
                    if (((Target.mJobSelectionDay.Ticks == 0x0L) || (SimClock.CurrentDayOfWeek != Target.mJobSelectionDay.DayOfWeek)) || (SimClock.ElapsedTime(TimeUnit.Days, Target.mJobSelectionDay) >= 1f))
                    {
                        Target.mAvailableJobs.Clear();
                        Target.mTakenJobs.Clear();
                        Target.mJobSelectionDay = SimClock.CurrentTime();
                        Target.StartTimer();
                    }

                    if (GameUtils.IsUniversityWorld())
                    {
                        EnterStateMachine("JobBoard", "Enter", "x");
                        SetActor("jobBoard", Target);
                        AnimateSim("Exit");
                    }
                    else
                    {
                        Actor.PlaySoloAnimation("a2o_postBoxJobBoard_checkBoard_x", true);
                    }

                    EndCommodityUpdates(true);

                    Definition definition = InteractionDefinition as Definition;

                    Tutorialette.TriggerLesson(Lessons.QuestTracker, Actor);
                    if (Autonomous || !UIUtils.IsOkayToStartModalDialog())
                    {
                        Actor.Wander(kMinWanderDistance, kMaxWanderDistance, false, RouteDistancePreference.NoPreference, false);
                    }
                    else
                    {
                        OpportunityNames choice = OpportunityNames.Undefined;
                        if (Target.mTakenJobs.Count >= kOpportunitiesPerDay)
                        {
                            string msg = "JobBoardEmptyTNS";
                            if (GameUtils.IsUniversityWorld())
                            {
                                msg = "JobBoardEmptyTNSEP9";
                            }

                            Actor.ShowTNSIfSelectable(LocalizeString(Actor.IsFemale, msg, new object[0x0]), StyledNotification.NotificationStyle.kSimTalking);
                        }
                        else
                        {
                            if ((Target.mAvailableJobs.Count + Target.mTakenJobs.Count) < kOpportunitiesPerDay)
                            {
                                List<Opportunity> opportunities = OpportunityEx.GetAllOpportunities(Actor, definition.mOppName);

                                List<float> weights = new List<float>();
                                foreach (Opportunity opp in opportunities)
                                {
                                    float num = Actor.SimDescription.OpportunityHistory.HasRejectedOpportunity(opp.Guid) ? OpportunityHistory.kUserRejectedOpportunityWeightMultiplier : 1f;
                                    weights.Add(opp.ChanceToGetOnPhone * num);
                                }

                                Opportunity opportunity = null;
                                while (opportunities.Count > 0)
                                {
                                    int index = RandomUtil.GetWeightedIndex(weights.ToArray());

                                    opportunity = opportunities[index];

                                    weights.RemoveAt(index);
                                    opportunities.RemoveAt(index);

                                    if (!Target.mAvailableJobs.Contains(opportunity.Guid))
                                    {
                                        Target.mAvailableJobs.Add(opportunity.Guid);
                                        Target.mIndexOfLastChosen = Target.mAvailableJobs.Count - 0x1;

                                        if (!OpportunityEx.Perform(Actor.SimDescription, opportunity.Guid)) return false;

                                        choice = opportunity.Guid;
                                        break;
                                    }
                                }

                                if (choice == OpportunityNames.Undefined)
                                {
                                    Actor.ShowTNSIfSelectable(LocalizeString(Actor.IsFemale, "NoValidOppsTNS", new object[] { Actor }), StyledNotification.NotificationStyle.kGameMessagePositive, Target.ObjectId, Actor.ObjectId);
                                    return false;
                                }
                            }
                            else
                            {
                                choice = OpportunityNames.Undefined;
                                int count = Target.mAvailableJobs.Count;
                                if (count > 0x0)
                                {
                                    int num2 = (Target.mIndexOfLastChosen + 0x1) % count;
                                    for (int i = 0x0; i < Target.mAvailableJobs.Count; i++)
                                    {
                                        int num4 = (i + num2) % count;
                                        OpportunityNames names3 = Target.mAvailableJobs[num4];
                                        if ((Actor.OpportunityManager.IsOpportunityAvailable(names3)) && (Actor.OpportunityManager.IsOpportunityCategory(names3, definition.mOppName)))
                                        {
                                            choice = names3;
                                            Target.mIndexOfLastChosen = num4;
                                            break;
                                        }
                                    }
                                }

                                if (choice != OpportunityNames.Undefined)
                                {
                                    if (!OpportunityEx.Perform(Actor.SimDescription, choice)) return false;
                                }
                                else
                                {
                                    Actor.ShowTNSIfSelectable(LocalizeString(Actor.IsFemale, "NoValidOppsTNS", new object[] { Actor }), StyledNotification.NotificationStyle.kGameMessagePositive, Target.ObjectId, Actor.ObjectId);
                                }
                            }

                            if (choice != OpportunityNames.Undefined)
                            {
                                Target.mIndexOfLastChosen = 0x0;
                                Target.mAvailableJobs.Remove(choice);
                                Target.mTakenJobs.Add(choice);
                                if (Target.mTakenJobs.Count >= kOpportunitiesPerDay)
                                {
                                    Target.StopHelperEffect();
                                }
                            }
                        }
                    }
                }
                finally
                {
                    StandardExit();
                }

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : PostBoxJobBoard.CheckTheBoardEP9.Definition
        {
            bool mVisible;

            public Definition(bool visible)
                : base(null, null, OpportunityCategory.None)
            {
                mVisible = visible;
            }
            public Definition(string text, string path, OpportunityCategory oppName, bool visible)
                : base(text, path, oppName)
            {
                mVisible = visible;
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CheckTheBoardEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, PostBoxJobBoard target, List<InteractionObjectPair> results)
            {
                if (GameUtils.IsUniversityWorld())
                {
                    results.Add(new InteractionObjectPair(new Definition("Dare", "CheckBoards", OpportunityCategory.Dare, mVisible), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("SocialGroup", "CheckBoards", OpportunityCategory.SocialGroup, mVisible), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("DayJob", "CheckBoards", OpportunityCategory.DayJob, mVisible), iop.Target));
                }
                else
                {
                    switch (GameUtils.GetCurrentWorld())
                    {
                        case WorldName.Egypt:
                            mOppName = OpportunityCategory.AdventureEgypt;
                            break;
                        case WorldName.France:
                            mOppName = OpportunityCategory.AdventureFrance;
                            break;
                        case WorldName.China:
                            mOppName = OpportunityCategory.AdventureChina;
                            break;
                    }

                    results.Add(new InteractionObjectPair(new Definition(null, null, mOppName, mVisible), iop.Target));
                }
            }

            public override string[] GetPath(bool isFemale)
            {
                if (GameUtils.IsUniversityWorld())
                {
                    return new string[] { (Localization.LocalizeString("Gameplay/Abstracts/ScriptObject/CheckTheBoardEP9:InteractionName", new object[0]) + Localization.Ellipsis) };
                }
                else
                {
                    return new string[0];
                }
            }

            public override string GetInteractionName(Sim a, PostBoxJobBoard target, InteractionObjectPair interaction)
            {
                if (GameUtils.IsUniversityWorld())
                {
                    return Localization.LocalizeString("Gameplay/Objects/PostBoxJobBoard:" + MenuText, new object[0]);
                }
                else
                {
                    return Localization.LocalizeString("Gameplay/Core/PostBoxJobBoard/CheckTheBoard:InteractionName", new object[0]);
                }
            }

            public override bool Test(Sim a, PostBoxJobBoard target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!mVisible) return false;

                if (isAutonomous && a.IsSelectable)
                {
                    return false;
                }

                if (!isAutonomous && a.OpportunityManager.HasOpportunity(mOppName))
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return PostBoxJobBoard.CheckTheBoardEP9.LocalizeString(a.IsFemale, "HasOpportunityTooltip", new object[] { a });
                    };
                    return false;
                }

                return true;
            }
        }
    }
}
