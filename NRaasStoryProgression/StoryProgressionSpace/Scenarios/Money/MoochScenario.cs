using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public abstract class MoochScenario : MoneyTransferScenario
    {
        public MoochScenario(int delta)
            : base(delta)
        { }
        protected MoochScenario(MoochScenario scenario)
            : base (scenario)
        { }

        protected abstract float SkillIncrease
        { get; }

        public override bool IsFriendly
        {
            get { return false; }
        }

        protected override bool TestRelationship
        {
            get { return true; }
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (GetData<StoredNetWorthSimData>(Sim).NetWorth > GetData<StoredNetWorthSimData>(Target).NetWorth)
            {
                IncStat("Richer");
                return false;
            }

            return (base.TargetAllow(sim));
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            Skills.HandleMoochSkill(Sim, SkillIncrease);
            return true;
        }
    }
}
