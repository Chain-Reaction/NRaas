using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences.Controllers
{
    public class DereferenceEventListener : DereferenceController<EventListener>
    {
        protected override void PreProcess(EventListener obj, object parent, FieldInfo field)
        { }

        protected override void Perform(EventListener obj, object parent, FieldInfo field)
        {
            if (!DereferenceManager.HasBeenDestroyed(obj.TargetObject)) return;

            if (DereferenceManager.Perform(obj, ObjectLookup.GetReference(new ReferenceWrapper(obj)), false, false))
            {
                EventTracker.RemoveListener(obj);

                obj.CompletionDelegate = null;
                obj.mScriptObject = null;
                obj.mTargetObject = null;

                DelegateListener delListener = obj as DelegateListener;
                if (delListener != null)
                {
                    delListener.mProcessEvent = null;
                }

                DereferenceManager.Perform(obj, ObjectLookup.GetReference(new ReferenceWrapper(obj)), true, false);
            }
        }
    }
}
