using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Fishing;
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
    public class DereferenceSimDescription : DereferenceController<SimDescription>
    {
        Dictionary<ulong, List<SimDescription>> mSims;

        public DereferenceSimDescription()
        { }

        public override void Clear()
        {
            mSims = null;
        }

        protected override void PreProcess(SimDescription obj, object parent, FieldInfo field)
        { }

        protected override void Perform(SimDescription obj, object parent, FieldInfo field)
        {
            //if (obj.IsValidDescription) return;

            if (mSims == null)
            {
                mSims = SimListing.AllSims<SimDescription>(null, true);
            }

            // All residents, ancestors, and urnstone sims are considered valid
            if (mSims.ContainsKey(obj.SimDescriptionId)) return;

            GameObjectReference reference = ObjectLookup.GetReference(new ReferenceWrapper(obj));

            if (!DereferenceManager.Perform(obj, reference, false, false))
            {
                return;
            }

            DereferenceManager.Perform(obj, reference, true, false);

            string fullName = obj.FullName;
            if (!string.IsNullOrEmpty(fullName))
            {
                fullName = fullName.Trim();
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                bool found = false;
                foreach (List<SimDescription> list in mSims.Values)
                {
                    foreach (SimDescription sim in list)
                    {
                        if (sim.FullName == fullName)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    if (reference.Count > 0)
                    {
                        ErrorTrap.LogCorrection("SimDescription Removed: " + fullName);
                    }
                    else
                    {
                        ErrorTrap.DebugLogCorrection("Zero Reference SimDescription Removed: " + fullName);
                    }
                }
            }
        }
    }
}
