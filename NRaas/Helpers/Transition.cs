using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public interface ITransition
    {
        void Store(SimDescription sim);

        void Restore(SimDescription sim);
    }

    public class Transition
    {
        // Exported to Traveler
        public static void Store(SimDescription sim)
        {
            foreach (ITransition setting in Common.DerivativeSearch.Find<ITransition>())
            {
                setting.Store(sim);
            }
        }

        // Exported to Traveler
        public static void Restore(SimDescription sim)
        {
            foreach (ITransition setting in Common.DerivativeSearch.Find<ITransition>())
            {
                setting.Restore(sim);
            }
        }
    }
}

