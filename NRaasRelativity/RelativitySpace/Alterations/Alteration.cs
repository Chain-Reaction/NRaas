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
    public abstract class Alteration : IAlteration
    {
        public static readonly float sDefaultMinimum = 1;

        protected float mOriginal;

        protected float mFactor;

        protected float mMinimum;

        public Alteration(float factor, float minimum)
        {
            mFactor = factor;
            mMinimum = minimum;
        }

        public void Store()
        {
            mOriginal = GetValue();

            Alter(AdjustToMinimum(mOriginal * mFactor, mMinimum));
        }

        public abstract float GetValue();

        protected abstract void Alter(float value);

        public void Revert()
        {
            Alter(mOriginal);
        }

        public static float AdjustToMinimum(float value, float minimum)
        {
            if (value < minimum)
            {
                value = minimum;
            }

            return value;
        }
    }
}
