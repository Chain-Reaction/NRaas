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
    public class FairyControl : MagicControl
    {
        public readonly static FairyControl sControl = new FairyControl();

        protected override OccultTypes Occult
        {
            get { return OccultTypes.Fairy; }
        }

        public override int GetSkillLevel(SimDescription sim)
        {
            return sim.SkillManager.GetSkillLevel(SkillNames.FairyMagic);
        }

        public override float GetMana(Sim sim)
        {
            return sim.Motives.GetValue(CommodityKind.AuraPower);
        }

        public override bool IsFailure(Sim actor, IInteractionProxy proxy, IMagicalDefinition definition, out bool epicFailure)
        {
            epicFailure = false;

            return (GetMinSkillLevel(definition) < actor.SkillManager.GetSkillLevel(SkillNames.FairyMagic));
        }

        protected override bool IsAvailable(Sim sim, IMagicalDefinition definition)
        {
            if (!base.IsAvailable(sim, definition)) return false;

            return (!sim.BuffManager.HasElement(BuffNames.FairyAuraFailure));
        }

        public override void ConsumeMana(Sim actor, IInteractionProxy proxy, IMagicalDefinition definition)
        {
            actor.Motives.ChangeValue(CommodityKind.AuraPower, -definition.SpellSettings.mMinMana);
            (Sims3.UI.Responder.Instance.HudModel as HudModel).FlashMagicMotiveBar();
        }
    }
}
