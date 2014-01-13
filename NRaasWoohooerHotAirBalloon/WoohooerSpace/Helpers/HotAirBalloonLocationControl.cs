using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Store.Objects;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Helpers
{
    public class HotAirBalloonLocationControl : WoohooLocationControl
    {
        public override CommonWoohoo.WoohooLocation Location
        {
            get { return CommonWoohoo.WoohooLocation.HotAirBalloon; }
        }

        public override bool Matches(IGameObject obj)
        {
            return obj is HotairBalloon;
        }

        public override bool HasWoohooableObject(Lot lot)
        {
            return (lot.GetObjects<HotairBalloon>(new Predicate<HotairBalloon>(TestUse)).Count > 0);
        }

        public override bool HasLocation(Lot lot)
        {
            return (lot.CountObjects<ShowerOutdoor>() > 0);
        }

        public override bool AllowLocation(SimDescription sim, bool testVersion)
        {
            if (!sim.IsHuman) return false;

            return Woohooer.Settings.mAutonomousHotAirBalloon;
        }

        public bool TestUse(HotairBalloon obj)
        {
            if (!TestRepaired(obj)) return false;

            if (obj.mCurrentHeight != HotairBalloon.BalloonHeight.OnGround) return false;

            if (obj.mTargetHeight != HotairBalloon.BalloonHeight.OnGround) return false;

            return (obj.UseCount == 0);
        }

        public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
        {
            List<GameObject> results = new List<GameObject>();

            foreach (HotairBalloon obj in actor.LotCurrent.GetObjects<HotairBalloon>(new Predicate<HotairBalloon>(TestUse)))
            {
                if ((testFunc != null) && (!testFunc(obj, null))) continue;

                results.Add(obj);
            }

            return results;
        }

        public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
        {
            switch (style)
            {
                case CommonWoohoo.WoohooStyle.Safe:
                    return new HotAirBalloonWoohoo.SafeHotAirBalloonDefinition(target);
                case CommonWoohoo.WoohooStyle.Risky:
                    return new HotAirBalloonWoohoo.RiskyHotAirBalloonDefinition(target);
                case CommonWoohoo.WoohooStyle.TryForBaby:
                    return new HotAirBalloonWoohoo.TryForBabyHotAirBalloonDefinition(target);
            }

            return null;
        }
    }
}


