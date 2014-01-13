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
    public abstract class GenericAlteration<TYPE,FIELDTYPE> : AlterationBase<TYPE>
        where FIELDTYPE : class
    {
        public GenericAlteration(string fieldName, float factor, float minimum)
            : base(fieldName, factor, minimum)
        { }

        protected override float GetValue(FieldInfo field)
        {
            if (field.FieldType != typeof(FIELDTYPE)) return 0;

            return GetValue(field.GetValue(null) as FIELDTYPE);
        }

        protected abstract float GetValue(FIELDTYPE field);

        protected override void Alter(FieldInfo field, float value)
        {
            if (field.FieldType != typeof(FIELDTYPE))
            {
                if (Common.kDebugging)
                {
                    Common.DebugNotify(ToString() + ": Improper Type");
                }
                return;
            }

            Alter(field.GetValue(null) as FIELDTYPE, value);
        }

        protected abstract void Alter(FIELDTYPE field, float value);
    }
}
