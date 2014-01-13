using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DreamerSpace.Helpers
{
    public class DreamsAndPromisesManagerEx
    {
        public static void PartialUpdate(DreamsAndPromisesManager ths)
        {
            List<ActiveDreamNode> activeNodes = new List<ActiveDreamNode>(ths.mCompletedNodes);

            Dictionary<Event, List<DreamsAndPromisesManager.ScoredNode>> completedNodes = new Dictionary<Event, List<DreamsAndPromisesManager.ScoredNode>>();
            foreach (ActiveDreamNode node in activeNodes)
            {
                if ((node.CompletionEvent != null) && !completedNodes.ContainsKey(node.CompletionEvent))
                {
                    completedNodes.Add(node.CompletionEvent, new List<DreamsAndPromisesManager.ScoredNode>());
                }
            }

            foreach (ActiveDreamNode node2 in activeNodes)
            {
                List<DreamsAndPromisesManager.ScoredNode> list;
                if ((node2.PromiseParent != null) && (node2.PromiseParent.WasPromisedAndBroken || node2.PromiseParent.IsComplete))
                {
                    continue;
                }

                if ((node2.CompletionEvent == null) || !completedNodes.TryGetValue(node2.CompletionEvent, out list))
                {
                    list = new List<DreamsAndPromisesManager.ScoredNode>();
                    completedNodes.Add(new Event(EventTypeId.kEventNone), list);
                }

                try
                {
                    list.AddRange(ths.ScoreChildren(node2, completedNodes, false, node2.GetNonPromiseLinkChildren()));
                }
                catch(Exception e)
                {
                    Common.DebugException(ths.Actor, e);
                }
            }

            bool flag = ths.SelectDreamsToDisplay(completedNodes);
            foreach (ActiveDreamNode node3 in activeNodes)
            {
                bool flag2 = true;
                if (!ActiveDreamNodeEx.OnCompletion(node3))
                {
                    flag2 = false;
                }
                else if ((node3.IsRepeatable) && (ths.IsNotSpecialCaseNode(node3.NodeInstance.PrimitiveId) && ((node3.DebugCompleteFlags & ActiveDreamNode.DebugCompleteReasons.kStartNode) == ActiveDreamNode.DebugCompleteReasons.kNone)))
                {
                    if (node3.NodeInstance.TimeBetweenRepeats <= 0f)
                    {
                        ths.CancelDream(node3);
                        node3.Reset(true);
                        flag2 = false;
                    }
                    else
                    {
                        node3.Reset(false);
                        ths.mSleepingNodes.Add(node3);
                    }
                }
                if (flag2)
                {
                    ths.RemoveActiveNode(node3);
                }
            }

            if ((flag && (ths == DreamsAndPromisesManager.ActiveDreamsAndPromisesManager)) && (DreamsAndPromisesModel.Singleton != null))
            {
                ths.RefreshDisplayedDreams();
            }

            if (ths.mShouldRefreshOnUpdate)
            {
                ths.mShouldRefreshOnUpdate = false;

                try
                {
                    ths.Refresh();
                }
                catch (Exception e)
                {
                    Common.Exception(ths.Actor, e);
                }
            }
            ths.mCompletedNodes.Clear();
        }

        public static void Update(DreamsAndPromisesManager ths)
        {
            if (!ths.mFullUpdate)
            {
                PartialUpdate(ths);
            }
            else
            {
                ths.mFullUpdate = false;
                float num = SimClock.ElapsedTime(TimeUnit.Minutes);
                float simMinutesPassed = num - ths.mLastUpdateTime;
                ths.mLastUpdateTime = num;
                Dictionary<Event, List<DreamsAndPromisesManager.ScoredNode>> completedNodes = new Dictionary<Event, List<DreamsAndPromisesManager.ScoredNode>>();
                List<ActiveNodeBase> timeoutList = new List<ActiveNodeBase>();
                List<ActiveNodeBase> completedList = new List<ActiveNodeBase>();

                foreach (ActiveNodeBase base2 in new List<ActiveNodeBase>(ths.mActiveNodes))
                {
                    if (DreamerTuning.kNoTimeOut)
                    {
                        ActiveDreamNode node = base2 as ActiveDreamNode;
                        if (node != null)
                        {
                            node.mTimeActive = 0;
                        }
                    }

                    try
                    {
                        switch (base2.Update(simMinutesPassed))
                        {
                            case ActiveNodeBase.UpdateStatus.TimedOut:
                                timeoutList.Add(base2);
                                break;

                            case ActiveNodeBase.UpdateStatus.Completed:
                                {
                                    ActiveDreamNode node = base2 as ActiveDreamNode;
                                    if (((node != null) && (node.CompletionEvent != null)) && !completedNodes.ContainsKey(node.CompletionEvent))
                                    {
                                        completedNodes.Add(node.CompletionEvent, new List<DreamsAndPromisesManager.ScoredNode>());
                                    }
                                    completedList.Add(base2);
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(ths.Actor, e);

                        ths.RemoveActiveNode(base2);
                    }
                }

                ths.RetestVisibleNodes(timeoutList, completedList);
                foreach (ActiveNodeBase base3 in completedList)
                {
                    List<DreamsAndPromisesManager.ScoredNode> list3;
                    if ((base3.PromiseParent != null) && (base3.PromiseParent.WasPromisedAndBroken || base3.PromiseParent.IsComplete))
                    {
                        continue;
                    }
                    ActiveDreamNode node2 = base3 as ActiveDreamNode;
                    if (((node2 == null) || (node2.CompletionEvent == null)) || !completedNodes.TryGetValue(node2.CompletionEvent, out list3))
                    {
                        list3 = new List<DreamsAndPromisesManager.ScoredNode>();
                        completedNodes.Add(new Event(EventTypeId.kEventNone), list3);
                    }
                    if (node2 != null)
                    {
                        ths.RemoveCompletedNode(node2);
                    }

                    try
                    {
                        list3.AddRange(ths.ScoreChildren(base3, completedNodes, false, base3.GetNonPromiseLinkChildren()));
                    }
                    catch(Exception e)
                    {
                        Common.DebugException(ths.Actor, e);
                    }
                }

                bool flag = ths.SelectDreamsToDisplay(completedNodes);
                foreach (ActiveNodeBase base4 in timeoutList)
                {
                    ths.RemoveActiveNode(base4);
                    flag = true;
                }

                List<ActiveDreamNode> list4 = new List<ActiveDreamNode>();
                foreach (ActiveDreamNode node3 in ths.mSleepingNodes)
                {
                    if (node3.UpdateSleeping(simMinutesPassed))
                    {
                        node3.Reset(true);
                        ths.mActiveNodes.Add(node3);
                        ths.AddToReferenceList(node3);
                        list4.Add(node3);
                    }
                }

                foreach (ActiveDreamNode node4 in list4)
                {
                    ths.mSleepingNodes.Remove(node4);
                }

                foreach (ActiveNodeBase base5 in completedList)
                {
                    ActiveDreamNode node5 = base5 as ActiveDreamNode;
                    ActiveTimerNode node6 = base5 as ActiveTimerNode;
                    bool flag2 = true;
                    if (node5 != null)
                    {
                        if (!ActiveDreamNodeEx.OnCompletion(node5))
                        {
                            flag2 = false;
                        }
                        else if ((node5.IsRepeatable) && (ths.IsNotSpecialCaseNode(node5.NodeInstance.PrimitiveId) && ((node5.DebugCompleteFlags & ActiveDreamNode.DebugCompleteReasons.kStartNode) == ActiveDreamNode.DebugCompleteReasons.kNone)))
                        {
                            if (node5.NodeInstance.TimeBetweenRepeats <= 0f)
                            {
                                ths.CancelDream(node5);
                                node5.Reset(true);
                                flag2 = false;
                            }
                            else
                            {
                                node5.Reset(false);
                                ths.mSleepingNodes.Add(node5);
                            }
                        }
                    }
                    if ((node6 != null) && !node6.IsComplete)
                    {
                        flag2 = false;
                    }
                    if (flag2)
                    {
                        ths.RemoveActiveNode(base5);
                    }
                }

                List<ActiveDreamNode> list5 = new List<ActiveDreamNode>();
                foreach (ActiveDreamNode node7 in ths.mDreamNodes)
                {
                    if (node7.TimeActive > node7.NodeInstance.DisplayTime)
                    {
                        list5.Add(node7);
                    }
                }

                foreach (ActiveDreamNode node8 in list5)
                {
                    if ((ths.mDisplayedDreamNodes.Remove(node8) && (ths == DreamsAndPromisesManager.ActiveDreamsAndPromisesManager)) && (DreamsAndPromisesModel.Singleton != null))
                    {
                        DreamsAndPromisesModel.Singleton.FireRemoveDream(node8);
                    }
                    ths.mDreamNodes.Remove(node8);
                }

                if ((flag && (ths == DreamsAndPromisesManager.ActiveDreamsAndPromisesManager)) && (DreamsAndPromisesModel.Singleton != null))
                {
                    ths.RefreshDisplayedDreams();
                }

                if (ths.mShouldRefreshOnUpdate)
                {
                    ths.mShouldRefreshOnUpdate = false;

                    try
                    {
                        ths.Refresh();
                    }
                    catch (Exception e)
                    {
                        Common.Exception(ths.Actor, e);
                    }
                }
            }
        }
    }
}
