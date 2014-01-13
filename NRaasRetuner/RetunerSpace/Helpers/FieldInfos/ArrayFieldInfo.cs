using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RetunerSpace.Helpers.Stores;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Helpers.FieldInfos
{
    public class ArrayFieldInfo : TunableFieldInfo
    {
        int mIndex;

        public ArrayFieldInfo()
        { }
        public ArrayFieldInfo(TunableFieldInfo field, int index)
            : this(field.Field, field.ParentInfo, index)
        { }
        public ArrayFieldInfo(FieldInfo field, int index)
            : this(field, null, index)
        { }
        public ArrayFieldInfo(FieldInfo field, TunableFieldInfo parentInfo, int index)
            : base(field, parentInfo)
        {
            mIndex = index;
        }

        public override string Name
        {
            get { return EAText.GetNumberString(mIndex); }
        }

        public override Type ElementalType
        {
            get { return base.ElementalType.GetElementType(); }
        }

        protected override object PrivateGetValue()
        {
            Array array = base.PrivateGetValue() as Array;
            if (array == null) return null;

            if (mIndex >= array.Length) return null;

            return array.GetValue(mIndex);
        }

        public override TunableStore GetParentStore(TunableStore child)
        {
            ValueStore store = child as ValueStore;

            return base.GetParentStore(new ArrayValueStore(Field, mIndex, store.Value));
        }
    }
}
