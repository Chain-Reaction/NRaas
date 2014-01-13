using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public struct ReferenceWrapper
    {
        public static readonly ReferenceWrapper Empty;

        public readonly object mObject;

        public ReferenceWrapper(object obj)
        {
            mObject = obj;
        }

        public bool Valid
        {
            get { return (mObject != null); }
        }

        public static bool operator ==(ReferenceWrapper left, ReferenceWrapper right)
        {
            return object.ReferenceEquals(left.mObject, right.mObject);
        }
        public static bool operator !=(ReferenceWrapper left, ReferenceWrapper right)
        {
            return !object.ReferenceEquals(left.mObject, right.mObject);
        }

        public override bool Equals(object o)
        {
            if (o is ReferenceWrapper)
            {
                ReferenceWrapper obj = (ReferenceWrapper)o;

                return object.ReferenceEquals(mObject, obj.mObject);
            }
            else
            {
                return object.ReferenceEquals(mObject, o);
            }
        }

        public bool ReferenceEquals(object o)
        {
            return Equals(o);
        }

        public override int GetHashCode()
        {
            if (mObject == null) return 0;

            return mObject.GetHashCode();
        }

        public override string ToString()
        {
            if (mObject == null) return null;

            return mObject.ToString();
        }
    }
}

