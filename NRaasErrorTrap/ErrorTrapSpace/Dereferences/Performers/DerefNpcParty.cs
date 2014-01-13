using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefNpcParty : Dereference<NpcParty>
    {
        protected override DereferenceResult Perform(NpcParty reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mNotifyPartyCrashSuccess", field, objects))
            {
                Remove(reference.mNotifyPartyCrashSuccess, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mNotifyPartyCrashFail", field, objects))
            {
                Remove(reference.mNotifyPartyCrashFail, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mSimsReceivedLikingChange", field, objects))
            {
                Remove(reference.mSimsReceivedLikingChange, objects);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mHandleCrashersBroadCasters", field, objects))
            {
                try
                {
                    foreach (KeyValuePair<Sim,ReactionBroadcaster> broadcaster in reference.mHandleCrashersBroadCasters)
                    {
                        if (broadcaster.Key.HasBeenDestroyed)
                        {
                            broadcaster.Value.Dispose();
                        }
                    }
                }
                catch
                { }

                RemoveKeys(reference.mHandleCrashersBroadCasters, objects);
                return DereferenceResult.End;
            }
           
            return DereferenceResult.Failure;
        }
    }
}
