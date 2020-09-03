using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.HybridSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.MagicControls
{
    public class UnicornControl : MagicControl
    {
        public readonly static UnicornControl sControl = new UnicornControl();

        protected override OccultTypes Occult
        {
            get { return OccultTypes.Unicorn; }
        }

        public override int GetSkillLevel(SimDescription sim)
        {
            return 10;
        }

        public override MagicWand InitialPrep(Sim actor, IMagicalDefinition definition, out bool wandCreated)
        {
            // Unicorns cannot use the human animations, so cannot use wands
            wandCreated = false;
            return null;
        }

        public override bool IsFailure(Sim actor, IInteractionProxy proxy, IMagicalDefinition definition, out bool epicFailure)
        {
            epicFailure = false;

            return false;
        }

        protected override bool IsAvailable(Sim sim, IMagicalDefinition definition)
        {
            if (!base.IsAvailable(sim, definition)) return false;

            OccultUnicorn unicorn = sim.OccultManager.GetOccultType(OccultTypes.Unicorn) as OccultUnicorn;
            if (unicorn == null) return false;

            if (!unicorn.MagicPoints.HasPoints()) return false;

            if (GetMana(sim) - definition.SpellSettings.mMinMana <= 0) return false; 

            return true;
        }

        public override float GetMana(Sim sim)
        {
            OccultUnicorn unicorn = sim.OccultManager.GetOccultType(OccultTypes.Unicorn) as OccultUnicorn;
            if (unicorn == null) return 0;

            return unicorn.MagicPoints.mCurrentMagicPointValue;
        }

        public override void ConsumeMana(Sim actor, IInteractionProxy proxy, IMagicalDefinition definition)
        {
            OccultUnicorn unicorn = actor.OccultManager.GetOccultType(OccultTypes.Unicorn) as OccultUnicorn;
            if (unicorn != null)
            {
                unicorn.MagicPoints.UsePoints((int)definition.SpellSettings.mMinMana);
                (Sims3.UI.Responder.Instance.HudModel as HudModel).FlashMagicMotiveBar();
            }
        }
    }
}
