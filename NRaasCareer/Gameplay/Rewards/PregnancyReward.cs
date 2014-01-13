using NRaas.Gameplay.Opportunities;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.Rewards
{
    public class PregnancyReward : RewardInfoEx
    {
        public PregnancyReward()
        { }

        public override void Grant(Sim actor, object target)
        {
            if (actor.SimDescription.IsPregnant) return;

            Sim targetSim = target as Sim;
            if (targetSim == null)
            {
                List<SimDescription> choices = new List<SimDescription>();

                foreach (SimDescription sim in Household.AllSimsLivingInWorld())
                {
                    if (sim.CreatedSim == null) continue;

                    if (sim.TeenOrBelow) continue;

                    if (sim.Gender == actor.SimDescription.Gender) continue;

                    if (sim.TraitManager.HasElement(TraitNames.Good)) continue;

                    if (sim.TraitManager.HasElement(TraitNames.Friendly)) continue;

                    choices.Add(sim);
                }

                if (choices.Count == 0) return;

                targetSim = RandomUtil.GetRandomObjectFromList(choices).CreatedSim;
            }

            Pregnancy.Start(actor, targetSim);
        }
    }
}
