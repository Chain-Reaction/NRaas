using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Propagation
{
    public class PropagateClanDelightScenario : PropagateBuffScenario
    {
        SimPersonality mClan;

        public PropagateClanDelightScenario(SimDescription center, StoryProgressionObject clan, Origin origin)
            : base(center, BuffNames.Delighted, origin)
        {
            mClan = clan as SimPersonality;
        }
        protected PropagateClanDelightScenario(PropagateClanDelightScenario scenario)
            : base (scenario)
        {
            mClan = scenario.mClan;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "ClanDelight";
        }

        public override void SetActors(SimDescription actor, SimDescription target)
        {
            Sim = actor;

            base.SetActors(actor, target);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (mClan == null) return null;

            return mClan.GetAlliesFor(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if ((Origin == Origin.FromFire) && (ManagerSim.HasTrait(Target, TraitNames.Pyromaniac)))
            {
                ManagerSim.AddBuff(Target, BuffNames.Fascinated, Origin);
            }

            if ((Origin == Origin.FromTheft) && (ManagerSim.HasTrait(Target, TraitNames.Kleptomaniac)))
            {
                ManagerSim.AddBuff(Target, BuffNames.Fascinated, Origin);
            }

            if ((Origin == Origin.FromWatchingSimSuffer) && (ManagerSim.HasTrait(Target, TraitNames.Evil)))
            {
                ManagerSim.AddBuff(Target, BuffNames.FiendishlyDelighted, Origin);
            }

            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new PropagateClanDelightScenario(this);
        }
    }
}
