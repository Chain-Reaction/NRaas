using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Alterations
{
    public abstract class AlterationBase<TYPE> : Alteration
    {
        protected readonly string mFieldName;

        public AlterationBase(string fieldName, float factor, float minimum)
            : base(factor, minimum)
        {
            mFieldName = fieldName;
        }

        public override float GetValue()
        {
            FieldInfo field = typeof(TYPE).GetField(mFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null) return 0;

            return GetValue(field);
        }

        protected abstract float GetValue(FieldInfo field);

        protected override void Alter(float value)
        {
            FieldInfo field = typeof(TYPE).GetField(mFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
            {
                if (Common.kDebugging)
                {
                    Common.DebugNotify(ToString() + ": Missing");
                }
                return;
            }

            Alter(field, value);
        }
        protected abstract void Alter(FieldInfo field, float value);

        public override string ToString()
        {
            return typeof(TYPE) + "." + mFieldName;
        }
    }
}
