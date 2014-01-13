using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences.Controllers
{
    public class DereferenceDelegate : DereferenceController<Delegate>
    {
        public DereferenceDelegate()
        { }

        protected override void PreProcess(Delegate obj, object parent, FieldInfo field)
        {
            if (obj.Target != null)
            {
                ObjectLookup.Add(obj.Target, obj, null);
            }
        }

        protected override void SubPerform(Delegate obj, object parent, FieldInfo field, Dictionary<ReferenceWrapper, bool> performed, DereferenceController<Delegate>.OnProcess process)
        {
            process(obj, parent, field);
        }

        protected override void Perform(Delegate obj, object parent, FieldInfo field)
        {
            Delegate newDelegate = ProcessDuplicates(obj);
            if (!object.ReferenceEquals(newDelegate, obj))
            {
                field.SetValue(parent, newDelegate);
            }

            /*
            GameObject target = obj.Target as GameObject;
            if (target == null) return;

            DereferenceManager.Perform(obj, ObjectLookup.GetReference(new ReferenceWrapper(obj)), Dereferences, true);
            */
        }

        protected static Delegate ProcessDuplicates(Delegate del)
        {
            Delegate result = del;

            Delegate[] subs = del.GetInvocationList();

            if (subs.Length > 1)
            {
                List<Delegate> exists = new List<Delegate>();

                bool first = true;

                foreach (Delegate sub in subs)
                {
                    if (exists.Contains(sub))
                    {
                        del = Delegate.Remove(del, sub);

                        // return parameter
                        result = del;

                        bool record = Common.kDebugging;

                        if (record)
                        {
                            // Some objects are written incorrectly and continually add a new delegate each time the game is loaded
                            //   without removing the old one.  Fix these, but don't record them
                            if (sub.Target is ICanHasTombObjectComponent)
                            {
                                record = false;
                            }
                            else if (sub.Target is CollectionDisplaySingleItem)
                            {
                                record = false;
                            }
                            else if (sub.Target is CookingProcess)
                            {
                                record = false;
                            }
                        }

                        if (record)
                        {
                            if (first)
                            {
                                first = false;

                                ErrorTrap.LogCorrection("Delegate: " + del);
                            }

                            string text = sub.ToString();
                            text += ": " + sub.Method + " (" + sub.Target + ")";

                            ErrorTrap.LogCorrection("Duplicate Delegate Call Dropped: " + text);
                        }
                    }
                    else
                    {
                        exists.Add(sub);
                    }
                }
            }

            return result;
        }
    }
}
