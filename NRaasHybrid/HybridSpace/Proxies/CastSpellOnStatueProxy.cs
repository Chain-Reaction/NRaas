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
    public class CastSpellOnStatueProxy<T> : FireIceBlastProxy<T, ISculptureFrozenSim>
        where T : MagicWand.CastSpellOnStatue, IMagicalInteraction
    {
        protected override bool InitialPrep(T ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
        {
            if (!ths.Actor.RouteToObjectRadius(ths.Target.GetFrozenSim(), MagicWand.CastSpellOnStatue.kRangeToCastSpellOn))
            {
                return false;
            }

            return true;
        }

        public static bool CommonSpellOnTest(Sim a, ISculptureFrozenSim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (a == target.GetFrozenSim())
            {
                return false;
            }
            /*
            if (!a.HasTrait(TraitNames.WitchHiddenTrait))
            {
                return false;
            }*/
            if ((target.GetFrozenSim() == null) || !target.GetFrozenSim().BuffManager.HasElement(BuffNames.FrozenSolid))
            {
                return false;
            }
            if (target.GetFrozenSim().SimDescription.ChildOrBelow || target.GetFrozenSim().SimDescription.IsPet)
            {
                return false;
            }
            /*if (!MagicWand.HasWand(a))
            {
                return false;
            }*/
            return true;
        }
    }
}
