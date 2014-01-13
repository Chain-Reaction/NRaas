using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Collections;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class DumpAlarms : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if ((obj is CityHall) || (obj is Sim))
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public static void Dump(List<Pair<AlarmManager.Timer, int>> timers)
        {
            StringBuilder msg = new StringBuilder();

            foreach (Pair<AlarmManager.Timer, int> pair in timers)
            {
                msg.Append(Common.NewLine);

                msg.Append(Common.NewLine + "Order: " + pair.Second);

                AlarmManager.Timer timer = pair.First;

                msg.Append(Common.NewLine + "Time: " + timer.AlarmDateAndTime);
                msg.Append(Common.NewLine + "Repeating: " + timer.Repeating);

                if (timer.Repeating)
                {
                    msg.Append(Common.NewLine + "Repeat Length: " + timer.RepeatLength);
                    msg.Append(Common.NewLine + "Time Unit: " + timer.UnitOfTime);
                }

                msg.Append(Common.NewLine + "Owner: " + timer.ObjectRef);

                if (timer.ObjectRef is Lot)
                {
                    msg.Append(Common.NewLine + "Owner Name: " + (timer.ObjectRef as Lot).Name);
                    msg.Append(Common.NewLine + "Owner Address: " + (timer.ObjectRef as Lot).Address);
                }
                else if (timer.ObjectRef is SimDescription)
                {
                    msg.Append(Common.NewLine + "Owner Name: " + (timer.ObjectRef as SimDescription).FullName);
                }
                else if (timer.ObjectRef is Sim)
                {
                    msg.Append(Common.NewLine + "Owner Name: " + (timer.ObjectRef as Sim).FullName);
                }

                if (timer.CallBack != null)
                {
                    msg.Append(Common.NewLine + "Function: " + timer.CallBack.Method.ToString() + " - " + timer.CallBack.Method.DeclaringType.GetType().AssemblyQualifiedName);
                    if (timer.CallBack.Target != null)
                    {
                        msg.Append(Common.NewLine + "Target: " + timer.CallBack.Target);
                        msg.Append(Common.NewLine + "Target Type: " + timer.CallBack.Target.GetType() + "," + timer.CallBack.Target.GetType().Assembly.FullName);
                    }
                }
                else
                {
                    msg.Append(Common.NewLine + "<No Callback>");
                }
            }

            Common.WriteLog(msg.ToString());
        }

        public override bool Run()
        {
            try
            {
                List<AlarmManager> managers = new List<AlarmManager>();

                managers.Add (AlarmManager.Global);

                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lot.mSavedData.mAlarmManager == null) continue;

                    managers.Add(lot.AlarmManager);
                }

                Dictionary<ReferenceWrapper, bool> lookup = new Dictionary<ReferenceWrapper, bool>();

                List<Pair<AlarmManager.Timer,int>> timers = new List<Pair<AlarmManager.Timer,int>>();

                foreach (AlarmManager manager in managers)
                {
                    int count = 0;
                    foreach(KeyValuePair<AlarmHandle,List<AlarmManager.Timer>> handles in manager.mTimers)
                    {
                        foreach (AlarmManager.Timer timer in handles.Value)
                        {
                            if (lookup.ContainsKey(new ReferenceWrapper(timer))) continue;
                            lookup.Add(new ReferenceWrapper(timer), true);

                            timers.Add(new Pair<AlarmManager.Timer,int> (timer, count));
                            count++;
                        }
                    }
                }

                timers.Sort(OnSortTimer);

                Dump(timers);
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
            return true;
        }

        public static int OnSortTimer(Pair<AlarmManager.Timer, int> left, Pair<AlarmManager.Timer, int> right)
        {
            return left.First.AlarmDateAndTime.CompareTo (right.First.AlarmDateAndTime);
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<DumpAlarms>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("DumpAlarms:MenuName");
            }
        }
    }
}
