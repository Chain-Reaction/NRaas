using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class ImmigrantMailOrderScenario : MoveScenario
    {
        List<SimDescription> mImmigrants = null;

        ManagerLot.ImmigrationRequirement mRequirement;

        public ImmigrantMailOrderScenario(ManagerLot.ImmigrationRequirement requirement, List<SimDescription> immigrants)
            : base(requirement.mMate)
        {
            mImmigrants = immigrants;
            mRequirement = requirement;
        }
        protected ImmigrantMailOrderScenario(ImmigrantMailOrderScenario scenario)
            : base (scenario)
        {
            mImmigrants = scenario.mImmigrants;
            mRequirement = scenario.mRequirement;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "MailOrder";
        }

        protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
        {
            return new StandardMoveInLotScenario(going, 0);
        }

        protected override MoveInLotScenario GetMoveInLotScenario(HouseholdBreakdown breakdown)
        {
            return new StandardMoveInLotScenario(breakdown, 0);
        }

        protected override MoveInScenario GetMoveInScenario(List<SimDescription> going, SimDescription moveInWith)
        {
            return new UninspectedMoveInScenario(going, moveInWith);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            if (mRequirement.mMate == null) return null;

            List<SimDescription> list = new List<SimDescription>();
            list.Add(mRequirement.mMate);
            return list;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return mImmigrants;
        }

        protected override bool Allow()
        {
            if (mRequirement.mMate == null) return false;

            return base.Allow();
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (mRequirement.MateGender != sim.Gender)
            {
                IncStat("Wrong Gender");
                return false;
            }
            
            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (mRequirement.mMate.IsMale)
            {
                Target.IncreaseGenderPreferenceMale();
            }
            else
            {
                Target.IncreaseGenderPreferenceFemale();
            }

            Relationship relationship = Relationship.Get(Sim, Target, true);
            relationship.LTR.SetLiking(75f);

            Romances.BumpToHigherState(this, Sim, Target);

            if (!base.PrivateUpdate(frame)) return false;

            mImmigrants.Remove(Target);

            Add(frame, new ImmigrantCareerScenario(Target, mRequirement), ScenarioResult.Start);            
            return false;
        }

        public override Scenario Clone()
        {
            return new ImmigrantMailOrderScenario(this);
        }
    }
}
