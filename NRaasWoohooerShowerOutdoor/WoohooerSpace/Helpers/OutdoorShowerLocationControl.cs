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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Helpers
{
    public class OutdoorShowerLocationControl : WoohooLocationControl
    {
        public override CommonWoohoo.WoohooLocation Location
        {
            get { return CommonWoohoo.WoohooLocation.OutdoorShower; }
        }

        public override bool Matches(IGameObject obj)
        {
            return obj is ShowerOutdoor;
        }

        public override bool HasWoohooableObject(Lot lot)
        {
            return (lot.GetObjects<ShowerOutdoor>(new Predicate<ShowerOutdoor>(TestUse)).Count > 0);
        }

        public override bool HasLocation(Lot lot)
        {
            return (lot.CountObjects<ShowerOutdoor>() > 0);
        }

        public override bool AllowLocation(SimDescription sim, bool testVersion)
        {
            if (!sim.IsHuman) return false;

            //if (testVersion)
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP4)) return false;
            }

            return Woohooer.Settings.mAutonomousShower;
        }

        public bool TestUse(ShowerOutdoor obj)
        {
            if (!TestRepaired(obj)) return false;

            return (obj.UseCount == 0);
        }

        public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
        {
            List<GameObject> results = new List<GameObject>();

            if (GameUtils.IsInstalled(ProductVersion.EP4))
            {
                foreach (ShowerOutdoor obj in actor.LotCurrent.GetObjects<ShowerOutdoor>(new Predicate<ShowerOutdoor>(TestUse)))
                {
                    if ((testFunc != null) && (!testFunc(obj, null))) continue;

                    results.Add(obj);
                }
            }

            return results;
        }

        public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
        {
            switch (style)
            {
                case CommonWoohoo.WoohooStyle.Safe:
                    return new ShowerWoohoo.SafeDefinition(target);
                case CommonWoohoo.WoohooStyle.Risky:
                    return new ShowerWoohoo.RiskyDefinition(target);
                case CommonWoohoo.WoohooStyle.TryForBaby:
                    return new ShowerWoohoo.TryForBabyDefinition(target);
            }

            return null;
        }
    }
}


