using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupSituationEvents : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupSituationEvents");

            EventTracker instance = EventTracker.Instance;

            List<ulong> removes2 = new List<ulong>();

            foreach (KeyValuePair<ulong, Dictionary<ulong, List<EventListener>>> lookup in instance.mListeners)
            {
                List<ulong> removes = new List<ulong>();

                foreach (KeyValuePair<ulong, List<EventListener>> lookup2 in lookup.Value)
                {
                    int index = 0;
                    while (index < lookup2.Value.Count)
                    {
                        DelegateListener dListener = lookup2.Value[index] as DelegateListener;
                        index++;

                        if (dListener == null) continue;

                        if (dListener.mProcessEvent == null) continue;

                        if (CleanupSituationInteractions.TestCallback(dListener.mProcessEvent, "ProcessEvent"))
                        {
                            string name = dListener.mProcessEvent.Method.ToString();

                            if (dListener.mProcessEvent.Target != null)
                            {
                                name = dListener.mProcessEvent.Target.GetType().ToString() + " " + name;
                            }

                            Overwatch.Log(name + ": Bogus Situation Event Dropped");

                            index--;
                            lookup2.Value.RemoveAt(index);
                        }
                    }

                    if (lookup2.Value.Count == 0)
                    {
                        removes.Add(lookup2.Key);
                    }
                }

                foreach (ulong remove in removes)
                {
                    lookup.Value.Remove(remove);
                }

                if (lookup.Value.Count == 0)
                {
                    removes2.Add(lookup.Key);
                }
            }

            foreach (ulong remove in removes2)
            {
                instance.mListeners.Remove(remove);
            }
        }
    }
}
