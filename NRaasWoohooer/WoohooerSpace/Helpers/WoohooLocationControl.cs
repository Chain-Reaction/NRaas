using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Helpers
{
    public abstract class WoohooLocationControl
    {
        static Dictionary<CommonWoohoo.WoohooLocation, WoohooLocationControl> sControls;

        public WoohooLocationControl()
        { }

        public static WoohooLocationControl GetControl(CommonWoohoo.WoohooLocation location)
        {
            if (sControls == null)
            {
                sControls = new Dictionary<CommonWoohoo.WoohooLocation, WoohooLocationControl>();

                foreach (WoohooLocationControl control in Common.DerivativeSearch.Find<WoohooLocationControl>())
                {
                    sControls.Add(control.Location, control);
                }
            }

            WoohooLocationControl result;
            if (sControls.TryGetValue(location, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public abstract CommonWoohoo.WoohooLocation Location
        {
            get;
        }

        public abstract bool Matches(IGameObject obj);

        public abstract bool HasWoohooableObject(Lot lot);

        public abstract bool HasLocation(Lot lot);

        public abstract bool AllowLocation(SimDescription sim, bool testVersion);

        public abstract List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc);

        public abstract InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style);

        protected static bool TestRepaired(IGameObject obj)
        {
            if (obj != null)
            {
                RepairableComponent repairable = obj.Repairable;
                if (repairable != null)
                {
                    if (repairable.Broken) return false;
                }
            }

            return true;
        }
    }
}
