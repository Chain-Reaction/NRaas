using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class VersionStamp : Common.ProtoVersionStamp
    {
        public static readonly string sNamespace = "NRaas.Hybrid";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Hybrid.ResetSettings();
        }

        /*
         * 
         * 0 : Fairy Soothing Aura
         * 0 : Fairy Chattering Teeth
         * 0 : Fairy Hot Head
         * 
         * 1 : Witch GoodLuckCharm
         * 1 : Witch Convert
         * 1 : Fairy Creative Aura
         * 1 : Fairy Flight of Felicity
         * 
         * 2 : Witch FireBlast
         * 2 : Witch IceBlast
         * 2 : Genie SummonFood
         * 
         * 3 : Witch SpellDuel
         * 3 : Witch Upgrade
         * 3 : Fairy Repair
         * 3 : Fairy Body/Mind Aura
         * 
         * 4 : Witch LoveCharm
         * 4 : Witch Toadify
         * 4 : Fairy Tummy Twister
         * 
         * 5 : Witch HauntingCurse
         * 5 : Genie RemoveSim
         * 5 : Fairy Bloom
         * 
         * 6 : Witch Bladder Charm
         * 6 : Witch Hunger Charm
         * 6 : Witch Hygiene Charm
         * 6 : Fairy Skivvies
         * 6 : Genie CleanSim
         * 
         * 7 : Witch Bladder Curse
         * 7 : Witch Hunger Curse
         * 7 : Witch Hygiene Curse
         * 7 : Fairy Golden Toad
         * 7 : Genie EnchantHouse
         * 
         * 8 : Witch Energy Charm
         * 8 : Witch Fun Charm
         * 8 : Witch Social Charm
         * 8 : Witch SunlightCharm
         * 8 : Witch PestilenceCurse
         * 
         * 9 : Witch Energy Curse
         * 9 : Witch Fun Curse
         * 9 : Witch Social Curse
         * 9 : Witch Restoration
         * 9 : Genie EnsorcelInteraction/ReleaseSim
         * 
         * 10: Witch Reanimate
         * 10: Fairy Inner Beauty
         * 
         */

        /* TODO
         * 
         * SkinWalker : Higher skill level would allow for longer periods of time in animal form (with less recharge), eventually unlocking of change anytime
         * 
         * Ability for the Vampire "Bite" sim interaction to create hybrids
         * Designating which occult grants the walk-style (PostProcessWalkStyle)
         * 
         * Unlock Energy, Fun, Social Charms (kudos to Shimrod101)
         * 
         * Weather Stone interactions
         * 
         * Ability to dig dog sites as Werewolf
         * 
         * BuffEnsorcelled:OnRemoval() performs a ForceSelectActor that can mess with academic careers
         * 
         * Altered: Sims3.Gameplay.Objects.Decorations.FishTank+ScoopOut+MermaidDefinition - Sims3.Gameplay.Objects.Decorations.FishTank (Ignore)
         * 
         * TestShouldDemandFairyGift
         * TestShouldRequestFairyGift
         * TestFairySkillLevelForFlightOfFelicity
         * TestFairySkillLevelForGoldenToad
         * TestFairySkillLevelForInnerBeauty
         * OnAskForFairyGiftDemandFailure
         * FairySetBoobyTrap
         * kRequiredFairySkillLevelRepair
         * 
         */

        /* Changes
         *  
         * 
         * Added "Use Special Werewolf Outfit" default: True
         * Added "Allow Occult Skating" default: True
         * 
         */
        public static readonly int sVersion = 11;
    }
}
