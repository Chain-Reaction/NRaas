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
    public class DereferenceForce : DereferenceController<object>
    {
        public DereferenceForce()
        {
            mType = null;

            if (!string.IsNullOrEmpty(ErrorTrapTuning4.kForceReferenceLog))
            {
                DereferenceManager.Logger.Append("ForceReferenceLog: " + ErrorTrapTuning4.kForceReferenceLog);

                mType = Type.GetType(ErrorTrapTuning4.kForceReferenceLog);
                if (mType == null)
                {
                    DereferenceManager.Logger.Append(" Type Not Found");
                }
            }
        }

        protected override void PreProcess(object obj, object parent, FieldInfo field)
        { }

        protected override void Perform(object obj, object parent, FieldInfo field)
        {
            DereferenceManager.Perform(obj, ObjectLookup.GetReference(new ReferenceWrapper(obj)), false, true);
        }
    }
}
