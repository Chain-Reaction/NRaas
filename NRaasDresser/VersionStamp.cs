using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class DresserPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.Dresser";

        public class Version : ProtoVersion<GameObject>
        { }

        public void OnPreLoad()
        {
            sPopupMenuStyle = DresserPopupTuning.kPopupMenuStyle;
        }

        /* TODO
         * 
        * Trait scoring to determine how often a sim switches outfits (higher for insane sims)
        * Option to add Age Detail parts when a sim ages up based on trait scoring
        * Option to apply random tattoos to sims, perhaps trait based ? How to control the location and type ?
        * Tuning to outright replace the Categories specified for a CASPart, allowing it to be used in more categories
        * 
        * Means of defining certain outfits to use when a specific object/interaction is used (Fishing outfit, Gardening outfit, Painting, etc)
        * Option to reroll the "Makeover" outfit randomly for sims, whenever they visit the Stylist lot
        * Option to disable rerolling of new hair on age-up based on whether the sim was originally wearing a hat or not
        * Ability to disallow certain content based on the other content the sim is already using (no facial hair if wearing a hat for example)
        * 
        * Ability to reorder the outfits in a particular category
        * 
        * How does ValidParts handle hats, if no hats are included in the tuning ?

        * Option to change sims into Everyday outfits at a certain time of day, if they are in their sleepwear.

        * Option to "Check Outfits" for parts that are not in the proper category as defined by the EA CAS part tuning
        ** This will conflict with the use of "Disable Clothing Filter" so cannot be an automatic system

        * Invalidating items in Career outfits

        * Option to invalidate parts for wild horses, versus resident ones

        * Ability to copy an outfit into another category

        * Option to specify general filters in-game rather than through tuning
        ** Option to remove all parts of a certain body type for a certain category/age/gender
        ** Option to disallow the use of FullBody vs. Upper/Lower Body parts

        * Invalidation based on Celebrity Level

        * Option to control what the Reaper wears, or any service sim

        * Option to have rotation perform in the order of the outfits, rather than random shuffle

        * Option to set a value for slider that all sims with no value will be assigned on check

        * [Dresser] performing the "Check Outfits" task twice in a row for some sims ?
        * 
         * Option to rotate Career outfits
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 38;
    }
}
