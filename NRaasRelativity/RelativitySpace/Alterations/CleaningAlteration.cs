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
    public class CleaningAlteration : AlterationList
    {
        public CleaningAlteration(CleanableComponent.Tuning tuning, float factor)
        {
            // Cleaning gets faster as speed goes down
            Add(new DirtyAlteration(tuning, 1f / factor));
            Add(new UpgradeAlteration(tuning.UpgradeSelfCleaning, factor));
        }

        public abstract class TuningAlteration : Alteration
        {
            protected CleanableComponent.Tuning mTuning;

            public TuningAlteration(CleanableComponent.Tuning tuning, float factor, float minimum)
                : base(factor, minimum)
            {
                mTuning = tuning;
            }
        }

        public class DirtyAlteration : TuningAlteration
        {
            public DirtyAlteration(CleanableComponent.Tuning tuning, float factor)
                : base(tuning, factor, sDefaultMinimum)
            { }

            public override float GetValue()
            {
                return mTuning.CleanDirtyDecPerMinute;
            }

            protected override void Alter(float value)
            {
                mTuning.CleanDirtyDecPerMinute = value;
            }
        }
    }
}
