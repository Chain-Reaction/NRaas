using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Appliances;
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
    public class UpgradeAlteration : AlterationList
    {
        public UpgradeAlteration(Handiness.UpgradeTuning tuning, float factor)
        {
            Add(new TimeAlteration(tuning, factor));
            Add(new SkillAlteration(tuning));
        }

        public abstract class TuningAlteration : Alteration
        {
            protected Handiness.UpgradeTuning mTuning;

            public TuningAlteration(Handiness.UpgradeTuning tuning, float factor, float minimum)
                : base(factor, minimum)
            {
                mTuning = tuning;
            }
        }

        public class TimeAlteration : TuningAlteration
        {
            public TimeAlteration(Handiness.UpgradeTuning tuning, float factor)
                : base(tuning, factor, sDefaultMinimum)
            {}

            public override float GetValue()
            {
                return mTuning.UpgradeTime;
            }

            protected override void Alter(float value)
            {
                mTuning.UpgradeTime = value;
            }
        }

        public class SkillAlteration : TuningAlteration
        {
            public SkillAlteration(Handiness.UpgradeTuning tuning)
                : base(tuning, Relativity.Settings.GetSkillFactor(SkillNames.Handiness), 0)
            {}

            public override float GetValue()
            {
                return mTuning.SkillGainRate;
            }

            protected override void Alter(float value)
            {
                mTuning.SkillGainRate = value;
            }
        }
    }
}
