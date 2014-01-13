using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefAlarmManagerTimer : Dereference<AlarmManager.Timer>
    {
        protected override DereferenceResult Perform(AlarmManager.Timer reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "CallBack", field, objects))
            {
                RemoveDelegate<AlarmTimerCallback>(ref reference.CallBack, objects);

                RemoveAlarm(reference);
                return DereferenceResult.End;
            }

            if (Matches(reference, "ObjectRef", field, objects))
            {                
                Remove(ref reference.ObjectRef);

                RemoveAlarm(reference);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }

        protected bool RemoveAlarm(AlarmManager.Timer reference)
        {
            if (Performing)
            {
                reference.Repeating = false;

                if (RemoveAlarm(AlarmManager.Global, reference))
                {
                    return true;
                }

                foreach (Lot lot in LotManager.Lots)
                {
                    if (lot.AlarmManager == null) continue;

                    if (RemoveAlarm(lot.AlarmManager, reference))
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        protected static bool RemoveAlarm(AlarmManager manager, AlarmManager.Timer timer)
        {
            int count = manager.mTimers.Count;
            manager.RemoveAlarm(timer.Handle);
            if (count != manager.mTimers.Count)
            {
                return true;
            }

            foreach (KeyValuePair<AlarmHandle, List<AlarmManager.Timer>> timers in manager.mTimers)
            {
                if (timers.Value.Contains(timer))
                {
                    manager.RemoveAlarm(timers.Key);
                    if (count != manager.mTimers.Count)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
