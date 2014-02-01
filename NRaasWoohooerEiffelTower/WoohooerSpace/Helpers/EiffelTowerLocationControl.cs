using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Helpers
{
	public class EiffelTowerLocationControl : WoohooLocationControl
    {
        public override CommonWoohoo.WoohooLocation Location
        {
			get { return CommonWoohoo.WoohooLocation.RabbitHole; }
        }

        public override bool Matches(IGameObject obj)
        {
			return obj is EiffelTower;
        }

        public override bool HasWoohooableObject(Lot lot)
        {
			return false;
        }

        public override bool HasLocation(Lot lot)
        {
			return (lot.CountObjects<EiffelTower>() > 0);
        }

		public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
		{
			return null;
		}

        public override bool AllowLocation(SimDescription sim, bool testVersion)
        {
			return sim.IsHuman && Woohooer.Settings.mAutonomousRabbithole;
        }

        public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
        {
            return null;
        }
    }
}


