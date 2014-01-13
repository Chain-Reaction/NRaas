using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.HybridSpace.Interfaces;
using NRaas.HybridSpace.MagicControls;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Proxies
{
    public abstract class FireIceBlastProxy<T, TARGET> : InteractionProxy<T, TARGET>
        where T : InteractionInstance<Sim, TARGET>, IMagicalSubInteraction
        where TARGET : class, IGameObject
    {
        protected override bool SetupAnimation(T ths, MagicControl control, bool twoPerson)
        {
            ths.EnterStateMachine("FireIceBlast", "Enter", "x");

            return base.SetupAnimation(ths, control, twoPerson);
        }
    }
}
