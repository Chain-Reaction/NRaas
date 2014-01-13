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
using Sims3.Gameplay.Objects.Environment;
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
    public class JungleGymAlteration : AlterationList
    {
        public JungleGymAlteration(JungleGym.Tuning tuning, float factor)
        {
            Add(new MinTimeAlteration(tuning, factor));
            Add(new MaxTimeAlteration(tuning, factor));
        }

        public abstract class TuningAlteration : Alteration
        {
            protected JungleGym.Tuning mTuning;

            public TuningAlteration(JungleGym.Tuning tuning, float factor, float minimum)
                : base(factor, minimum)
            {
                mTuning = tuning;
            }
        }

        public class MinTimeAlteration : TuningAlteration
        {
            public MinTimeAlteration(JungleGym.Tuning tuning, float factor)
                : base(tuning, factor, sDefaultMinimum)
            { }

            public override float GetValue()
            {
                return mTuning.HangOutTimeMin;
            }

            protected override void Alter(float value)
            {
                mTuning.HangOutTimeMin = (int)value;
            }
        }

        public class MaxTimeAlteration : TuningAlteration
        {
            public MaxTimeAlteration(JungleGym.Tuning tuning, float factor)
                : base(tuning, factor, sDefaultMinimum)
            { }

            public override float GetValue()
            {
                return mTuning.HangOutTimeMax;
            }

            protected override void Alter(float value)
            {
                mTuning.HangOutTimeMax = (int)value;
            }
        }
    }
}
