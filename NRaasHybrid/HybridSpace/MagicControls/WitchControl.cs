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
    public class WitchControl : MagicControl
    {
        public readonly static WitchControl sControl = new WitchControl();

        protected override OccultTypes Occult
        {
            get { return OccultTypes.Witch; }
        }

        public override int GetSkillLevel(SimDescription sim)
        {
            return sim.SkillManager.GetSkillLevel(SkillNames.Spellcasting);
        }

        public override MagicWand InitialPrep(Sim actor, IMagicalDefinition definition, out bool wandCreated)
        {
            actor.SkillManager.AddElement(SkillNames.Spellcasting);
            SpellcastingSkill element = actor.SkillManager.GetElement(SkillNames.Spellcasting) as SpellcastingSkill;

            wandCreated = false;
            return MagicWand.GetWandToUse(actor, element);
        }

        public override bool IsFailure(Sim actor, IInteractionProxy proxy, IMagicalDefinition definition, out bool epicFailure)
        {
            bool fail = true;

            epicFailure = false;

            SpellcastingSkill spellcasting = actor.SkillManager.GetElement(SkillNames.Spellcasting) as SpellcastingSkill;
            MagicWand wand = MagicWand.GetWandToUse(actor, spellcasting);
            if (wand != null)
            {
                fail = RandomUtil.RandomChance(wand.SuccessChance(actor, GetMinSkillLevel(definition), spellcasting.SkillLevel));
                if (!fail)
                {
                    epicFailure = RandomUtil.RandomChance(wand.EpicFailChance(actor));
                }
            }

            return fail;
        }

        protected override bool IsAvailable(Sim sim, IMagicalDefinition definition)
        {
            if (!base.IsAvailable(sim, definition)) return false;

            return (!sim.BuffManager.HasElement(BuffNames.DepletedMagic));
        }

        public override float GetMana(Sim sim)
        {
            return sim.Motives.GetValue(CommodityKind.MagicFatigue);
        }

        public override void ConsumeMana(Sim actor, IInteractionProxy proxy, IMagicalDefinition definition)
        {
            MagicWand wand = proxy.Wand;
            if (wand != null)
            {
                wand.DrainMotive(actor, CommodityKind.MagicFatigue, -definition.SpellSettings.mMinMana);
            }
            else
            {
                float num = (definition.SpellSettings.mMinMana * DefaultWand.kMotiveDrainMultiplier) * MagicWand.kMoonPhaseMotiveDrainMultiplier[World.GetLunarPhase()];
                if (actor.BuffManager.HasElement(BuffNames.AnimalFamiliar))
                {
                    num *= MagicWand.kFamiliarMotiveMultiplier;
                }
                actor.Motives.ChangeValue(CommodityKind.MagicFatigue, -num);
                (Sims3.UI.Responder.Instance.HudModel as HudModel).FlashMagicMotiveBar();
            }
        }
    }
}
