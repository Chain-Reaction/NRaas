using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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

namespace NRaas.HybridSpace.Helpers
{
    public class MagicPointControl
    {
        public readonly static OccultTypes[] sMagicOccult = new OccultTypes[] { OccultTypes.Witch, OccultTypes.Genie, OccultTypes.Fairy, OccultTypes.Unicorn };

        public static bool IsFailure(IMagicalInteraction interaction, OccultTypes preferredType)
        {
            bool epicFailure = false;
            return IsFailure(interaction, preferredType, out epicFailure);
        }
        public static bool IsFailure(IMagicalInteraction interaction, OccultTypes intended, out bool epicFailure)
        {
            bool fail = false;

            epicFailure = false;

            if (intended != OccultTypes.None)
            {
                if (PrivateIsFailure(interaction, intended, out fail, out epicFailure))
                {
                    return fail;
                }
            }

            foreach (OccultTypes type in sMagicOccult)
            {
                if (type == intended) continue;

                if (PrivateIsFailure(interaction, type, out fail, out epicFailure))
                {
                    return fail;
                }
            }

            return fail;
        }

        private static bool PrivateIsFailure(IMagicalInteraction interaction, OccultTypes testType, out bool fail, out bool epicFailure)
        {
            fail = false;
            epicFailure = false;

            if (!OccultTypeHelper.HasType(interaction.Actor, testType)) return false;

            switch (testType)
            {
                case OccultTypes.Witch:
                    SpellcastingSkill spellcasting = interaction.Actor.SkillManager.GetElement(SkillNames.Spellcasting) as SpellcastingSkill;
                    MagicWand wand = MagicWand.GetWandToUse(interaction.Actor, spellcasting);
                    if (wand != null)
                    {
                        fail = RandomUtil.RandomChance(wand.SuccessChance(interaction.Actor, interaction.SpellLevel, spellcasting.SkillLevel));
                        if (!fail)
                        {
                            epicFailure = RandomUtil.RandomChance(wand.EpicFailChance(interaction.Actor));
                        }
                    }
                    break;
                case OccultTypes.Fairy:
                    if (interaction.SpellLevel <= interaction.Actor.SkillManager.GetSkillLevel(SkillNames.FairyMagic))
                    {
                        fail = true;
                    }
                    break;
                case OccultTypes.Genie:
                case OccultTypes.Unicorn:
                    return false;
            }

            return fail;
        }

        private static bool PrivateUsePoints(Sim sim, MagicWand wand, int points, OccultTypes type)
        {
            if (!OccultTypeHelper.HasType(sim, type)) return false;

            switch (type)
            {
                case OccultTypes.Genie:
                    OccultGenie genie = sim.OccultManager.GetOccultType(OccultTypes.Genie) as OccultGenie;
                    if (genie != null)
                    {
                        genie.MagicPoints.UsePoints(points);
                        (Sims3.UI.Responder.Instance.HudModel as HudModel).FlashMagicMotiveBar();
                        return true;
                    }
                    break;
                case OccultTypes.Fairy:
                    sim.Motives.ChangeValue(CommodityKind.AuraPower, -points);
                    (Sims3.UI.Responder.Instance.HudModel as HudModel).FlashMagicMotiveBar();
                    return true;
                case OccultTypes.Witch:
                    if (wand != null)
                    {
                        wand.DrainMotive(sim, CommodityKind.MagicFatigue, -points);
                        return true;
                    }
                    else
                    {
                        float num = (points * DefaultWand.kMotiveDrainMultiplier) * MagicWand.kMoonPhaseMotiveDrainMultiplier[World.GetLunarPhase()];
                        if (sim.BuffManager.HasElement(BuffNames.AnimalFamiliar))
                        {
                            num *= MagicWand.kFamiliarMotiveMultiplier;
                        }
                        sim.Motives.ChangeValue(CommodityKind.MagicFatigue, -num);
                        (Sims3.UI.Responder.Instance.HudModel as HudModel).FlashMagicMotiveBar();
                    }
                    break;
                case OccultTypes.Unicorn:
                    OccultUnicorn unicorn = sim.OccultManager.GetOccultType(OccultTypes.Unicorn) as OccultUnicorn;
                    if (unicorn != null)
                    {
                        unicorn.MagicPoints.UsePoints(points);
                        (Sims3.UI.Responder.Instance.HudModel as HudModel).FlashMagicMotiveBar();
                        return true;
                    }
                    break;
            }

            return false;
        }

        public static void UsePoints(Sim sim, int points, OccultTypes intended)
        {
            SpellcastingSkill spellCasting = sim.SkillManager.GetElement(SkillNames.Spellcasting) as SpellcastingSkill;
            MagicWand wand = MagicWand.GetWandToUse(sim, spellCasting);

            if (intended != OccultTypes.None)
            {
                if (PrivateUsePoints(sim, wand, points, intended)) return;
            }

            foreach(OccultTypes type in sMagicOccult)
            {
                if (type == intended) continue;

                if (PrivateUsePoints(sim, wand, points, type)) return;
            }
        }

        private static bool PrivateHasPoints(Sim sim, OccultTypes type, bool allowFailure)
        {
            if (!OccultTypeHelper.HasType(sim, type)) return false;

            switch (type)
            {
                case OccultTypes.Genie:
                    OccultGenie genie = sim.OccultManager.GetOccultType(OccultTypes.Genie) as OccultGenie;
                    if (genie != null)
                    {
                        return genie.MagicPoints.HasPoints();
                    }
                    break;
                case OccultTypes.Fairy:
                    return (!sim.BuffManager.HasElement(BuffNames.FairyAuraFailure));
                case OccultTypes.Witch:
                    if (allowFailure) return true;

                    return (!sim.BuffManager.HasElement(BuffNames.DepletedMagic));
                case OccultTypes.Unicorn:
                    OccultUnicorn unicorn = sim.OccultManager.GetOccultType(OccultTypes.Unicorn) as OccultUnicorn;
                    if (unicorn != null)
                    {
                        return unicorn.MagicPoints.HasPoints();
                    }
                    break;
            }

            return false;
        }

        public static bool HasPoints(Sim sim, OccultTypes intended, bool allowFailure)
        {
            if (intended != OccultTypes.None)
            {
                if (PrivateHasPoints(sim, intended, allowFailure)) return true;
            }

            foreach(OccultTypes type in sMagicOccult)
            {
                if (type == intended) continue;

                if (PrivateHasPoints(sim, type, allowFailure)) return true;
            }

            return false;
        }
    }
}
