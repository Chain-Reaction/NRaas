using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
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
    public class SinkAlteration : AlterationList
    {
        public SinkAlteration(Sink.Tuning tuning, float factor)
        {
            Add(new WashHandsAlteration(tuning, factor));
            Add(new BrushTeethAlteration(tuning, factor));
            Add(new WashDishesAlteration(tuning, factor));
        }

        public abstract class TuningAlteration : Alteration
        {
            protected Sink.Tuning mTuning;

            public TuningAlteration(Sink.Tuning tuning, float factor, float minimum)
                : base(factor, minimum)
            {
                mTuning = tuning;
            }
        }

        public class WashHandsAlteration : TuningAlteration
        {
            public WashHandsAlteration(Sink.Tuning tuning, float factor)
                : base(tuning, factor, sDefaultMinimum)
            { }

            public override float GetValue()
            {
                return mTuning.WashHandsLenthInMinutes;
            }

            protected override void Alter(float value)
            {
                mTuning.WashHandsLenthInMinutes = (int)value;
            }
        }

        public class BrushTeethAlteration : TuningAlteration
        {
            public BrushTeethAlteration(Sink.Tuning tuning, float factor)
                : base(tuning, factor, sDefaultMinimum)
            { }

            public override float GetValue()
            {
                return mTuning.BrushTeethLenthInMinutes;
            }

            protected override void Alter(float value)
            {
                mTuning.BrushTeethLenthInMinutes = (int)value;
            }
        }

        public class WashDishesAlteration : TuningAlteration
        {
            public WashDishesAlteration(Sink.Tuning tuning, float factor)
                : base(tuning, factor, sDefaultMinimum)
            { }

            public override float GetValue()
            {
                return mTuning.WashDishesLengthInMinutes;
            }

            protected override void Alter(float value)
            {
                mTuning.WashDishesLengthInMinutes = (int)value;
            }
        }
    }
}
