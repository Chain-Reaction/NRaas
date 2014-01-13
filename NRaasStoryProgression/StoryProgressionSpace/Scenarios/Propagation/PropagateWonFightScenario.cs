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
    public class PropagateWonFightScenario : PropagateBuffScenario
    {
        SimDescription mLoser;

        public PropagateWonFightScenario(SimDescription winner, SimDescription loser)
            : base(winner, BuffNames.Delighted, Origin.FromWatchingSimSuffer)
        { 
            mLoser = loser;
        }
        protected PropagateWonFightScenario(PropagateWonFightScenario scenario)
            : base (scenario)
        { 
            mLoser = scenario.mLoser;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;
            
            return "PropagateWonFight";
        }

        public override void SetActors(SimDescription actor, SimDescription target)
        {
            Sim = actor;
            mLoser = target;

            base.SetActors(actor, target);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            List<SimDescription> list = Personalities.GetClanAlliesFor(sim);

            if (!list.Contains(mLoser))
            {
                list.Add(mLoser);
            }

            return list;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Target == mLoser)
            {
                ManagerSim.AddBuff(Manager, Target, BuffNames.Sore, Origin.FromLosingFight);
            }
            else
            {
                if (Target == Sim)
                {
                    ManagerSim.AddBuff(Target, BuffNames.Winner, Origin.FromWinningFight);
                }
                else
                {
                    base.PrivateUpdate(frame);
                }

                if (ManagerSim.HasTrait(Target, TraitNames.Evil))
                {
                    ManagerSim.AddBuff(Target, BuffNames.FiendishlyDelighted, Origin);
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new PropagateWonFightScenario(this);
        }
    }
}
