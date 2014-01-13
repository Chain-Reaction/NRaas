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
    public class RepairAlteration : AlterationList
    {
        public RepairAlteration(RepairableComponent.Tuning tuning, float factor)
        {
            Add(new MinTimeAlteration(tuning, factor));
            Add(new MaxTimeAlteration(tuning, factor));
            Add(new SkillAlteration(tuning, Relativity.Settings.GetSkillFactor(SkillNames.Handiness)));
        }

        public abstract class TuningAlteration : Alteration
        {
            protected RepairableComponent.Tuning mTuning;

            public TuningAlteration(RepairableComponent.Tuning tuning, float factor, float minimum)
                : base(factor, minimum)
            {
                mTuning = tuning;
            }
        }

        public class MinTimeAlteration : TuningAlteration
        {
            public MinTimeAlteration(RepairableComponent.Tuning tuning, float factor)
                : base(tuning, factor, sDefaultMinimum)
            { }

            public override float GetValue()
            {
                return mTuning.MinRepairTime;
            }

            protected override void Alter(float value)
            {
                mTuning.MinRepairTime = value;
            }
        }

        public class MaxTimeAlteration : TuningAlteration
        {
            public MaxTimeAlteration(RepairableComponent.Tuning tuning, float factor)
                : base(tuning, factor, sDefaultMinimum)
            { }

            public override float GetValue()
            {
                return mTuning.MaxRepairTime;
            }

            protected override void Alter(float value)
            {
                mTuning.MaxRepairTime = value;
            }
        }

        public class SkillAlteration : TuningAlteration
        {
            public SkillAlteration(RepairableComponent.Tuning tuning, float factor)
                : base(tuning, factor, 0)
            { }

            public override float GetValue()
            {
                return mTuning.RepairSkillGain;
            }

            protected override void Alter(float value)
            {
                mTuning.RepairSkillGain = value;
            }
        }
    }
}
