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
    public class FloatAlteration<TYPE> : AlterationBase<TYPE>
    {
        public FloatAlteration(string fieldName, float factor, float minimum)
            : base(fieldName, factor, minimum)
        { }

        protected override float GetValue(FieldInfo field)
        {
            if (field.FieldType == typeof(float))
            {
                return (float)field.GetValue(null);
            }
            else if (field.FieldType == typeof(int))
            {
                return (int)field.GetValue(null);
            }
            else
            {
                return 0;
            }
        }

        protected override void Alter(FieldInfo field, float value)
        {
            if (field.FieldType == typeof(float)) 
            {
                field.SetValue(null, value);
            }
            else if (field.FieldType == typeof(int))
            {
                field.SetValue(null, (int)value);
            }
            else
            {
                if (Common.kDebugging)
                {
                    Common.DebugNotify(ToString() + ": Improper Type");
                }
                return;
            }
        }
    }
}
