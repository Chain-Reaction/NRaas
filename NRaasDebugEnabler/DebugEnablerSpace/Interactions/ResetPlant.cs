using NRaas.CommonSpace.Interactions;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class ResetPlant : DebugEnablerInteraction<IGameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is HarvestPlant)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            HarvestPlant plant = Target as HarvestPlant;
            if (plant == null) return false;

            plant.mBarren = false;
            plant.mLifetimeHarvestablesYielded = 0;

            return true;
        }

        [DoesntRequireTuning]
        private class Definition : DebugEnablerDefinition<ResetPlant>
        {
            public override string GetInteractionName(IActor a, IGameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("RestartPlantLife:MenuName");
            }

            public override bool Test(IActor a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                HarvestPlant plant = target as HarvestPlant;
                if (plant == null) return false;

                if (!plant.Barren) return false;

                if (plant.CanProduceMoreHarvestables()) return false;

                return true;
            }
        }
    }
}