using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class DisableRewardCeremonies
    {
        class Middleman
        {
            ProcessEventDelegate mDelegate;

            public Middleman(ProcessEventDelegate eventDelegate)
            {
                mDelegate = eventDelegate;
            }

            public ListenerAction OnEvent(Event e)
            {
                try
                {
                    if ((mDelegate == null) || ((e.Actor != null) && (!e.Actor.IsSelectable)))
                    {
                        return ListenerAction.Keep;
                    }

                    return mDelegate(e);
                }
                catch (Exception exception)
                {
                    Common.Exception(e.Actor, e.TargetObject, exception);
                    return ListenerAction.Remove;
                }
            }
        }

        public static bool Perform()
        {
            try
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP2)) return true;

                if (EventTracker.Instance == null) return false;

                if (EventTracker.Instance.mListeners == null) return false;

                // Required to stop a hang when an inactive firefighter reaches level 10

                Dictionary<ulong, List<EventListener>> dictionary;
                if (!EventTracker.Instance.mListeners.TryGetValue((ulong)EventTypeId.kActiveCareerAdvanceLevel, out dictionary))
                {
                    return false;
                }

                foreach (List<EventListener> list in dictionary.Values)
                {
                    if (list == null) continue;

                    foreach (EventListener listener in list)
                    {
                        if (listener == null) continue;

                        DelegateListener delListener = listener as DelegateListener;
                        if (delListener != null)
                        {
                            RewardsManager.OccupationRewardInfo target = delListener.mProcessEvent.Target as RewardsManager.OccupationRewardInfo;
                            if ((target != null) && (delListener.mProcessEvent != null))
                            {
                                delListener.mProcessEvent = new Middleman(delListener.mProcessEvent).OnEvent;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("DisableRewardCeremonies", e);
            }

            return true;
        }
    }
}