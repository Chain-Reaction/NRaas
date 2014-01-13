using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Flirts
{
    public class ImmigrantRequirementScenario : ImmigrantRequirementScenarioBase
    {
        public ImmigrantRequirementScenario(ManagerLot.ImmigrationRequirement requirement)
            : base(requirement)
        { }
        protected ImmigrantRequirementScenario(ImmigrantRequirementScenario scenario)
            : base(scenario)
        { }

        protected override bool Allow()
        {
            if (GetValue<PressureOption, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            mRequirement.mSingle = true;

            if (Flirts.PreviousLoveLoss != null)
            {
                mRequirement.mMate = Flirts.PreviousLoveLoss.SimDescription;
                Flirts.PreviousLoveLoss = null;
            }

            if (mRequirement.mMate != null)
            {
                if (mRequirement.MateGender == CASAgeGenderFlags.Male)
                {
                    mRequirement.mNeedMale = true;
                }
                else
                {
                    mRequirement.mNeedFemale = true;
                }
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new ImmigrantRequirementScenario(this);
        }

        public class PressureOption : Manager.ImmigrationPressureBaseOption<ManagerFlirt>
        {
            public PressureOption()
                : base(10)
            { }
        }
    }
}
