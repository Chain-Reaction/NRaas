using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class MasterControllerPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.MasterController";

        public class Version : ProtoVersion<GameObject>
        { }

        public void OnPreLoad()
        {
            sPopupMenuStyle = MasterControllerPopupTuning.kPopupMenuStyle;
        }

        /* TODO
         *         
        * Automated Module:
        ** "Clean All" and "Repair All" from MasterController as an nightly alarm process
        ** "Purge Memories" for inactives sims. Purge only "Duplicate" memories, purge them all, purge ones without photos
        ** "Delete All Gnomes"

        * Possible to restart the audio loop used in CAS when it ends ?

        * Option to run [[Porter]]'s Doppelganger merging system on a sim in town

        * Issues with changing accessories in Integration's "Plan Outfit".  One gets removed when another is added ?

        * The "Demographics \ Career Summary" can become too long for some user's screens
        * Issue with "Export Genealogy" not dumping the tree for a spouse ?
        * Need some way to stop creation of bosses, coworkers, skills, everything during "Choose Job"
        * Private Investigator opportunities listed while on vacation ?
        * Issue with "Add Sim" on NPC apartment doors not assigning sims ?
        * Issue with choosing a Boss removing all existing coworkers ?

        * Test "Get Tattoo" on the regular Tattoo chair
        * Issue with Eyebrow and Body Hair color retention

        * Issue with "Reset Sim" placing sims in improper locations in households
        * After using "Edit in CAS" on a sim, all their Martial Arts outfits were doubled ?
        **    Issues with entering CAS while Naked, producing multiples of other outfit categories ?
        * Issue with the Naked Outfit switching to use the Everyday wear after a sim is brought into CAS.

        * Better handling of CAP windows in MasterController ?
        * Get slider multipliers working for CAPFBDAdvanced    

        * Check whether boarding school sims can be sent back to school after a Reset

        * Issues with the autonomy level for adoption pets added via "Add Sim" ?

        * Blacklisting for Facial Hair and Eyebrow panels
        * Does the [MasterController] blacklist handle maternity vs. non-maternity properly ?
        * Prompt when blacklisting that provides the option to blacklist for all categories, or "young adult and adult", or both.
        * Ability to differentiate between blacklisting for maternity wear versus non-maternity
        * Blacklisting of Tattoo Parts via CAS ?
        * Make blacklisting work properly when displaying the Naked or Martial Arts outfits
        * Implement new "hide the window" blacklisting approach for the Hair panel
        * Option to blacklist Young Adult items for Adults automatically, and vice versa
        * Prompt to specify whether the CAS part should be blacklisted for all categories, or just the current one.
        * 
        * Running "Pollinate" with multiple mother choices does not handle chosing multiple donors correctly
        *  
        * Issue where "Edit Town CAS" reverts outfits that are "Disable Clothing Filter" enabled
        * 
        * Ability to disable the option to post memories on the "Wall" ?
        * 
        * Build/Buy Mode ability to retrieve an Instance of an object
        * 
        * Remove the hour restrictions in the "Party" interaction
        * 
        * Drop the species restriction in "Add Moodlet"
        * 
        * Still issues with the "Days Off" interaction giving out odd results ?
        * 
        * Issue where the relationship panel does not update after a "Relationship By Category" change
        * Test "Edit in CAS" on an imaginary friend
        * 
        * Weight and Fitness while transformed ?
        * 
        * Does "Blood and Step" filter in [MasterController] include a sim's spouse and extended family ?
        * 
        * Need to split the "Demographics \ Personalities" window if it gets too long.
        * 
        * Adding beyond 8 in CAS not working if [Pets] is not installed ?
        * 
        * Test whether "Show Custom Content Only" is handled properly when switching categories
        * 
        * Check storage of eyecolor and eyebrow genetics in MasterController's CAS for consistency
        * 
        * Ability to reset Sim references after a reset using the Instantiated event and a lookup table ?
        * 
        * Issues with the "Autonomy" interaction in [MasterController]
        * 
        * Issues using blood relations filter during the "Party" interaction ?
        * 
        * Copy Genetics does not work between adult and child animals
        * 
        * Issue changing hair-style, makeup for naked outfit bleeding into Everyday ?
        * 
        * Multi Layering lipstick not retained after CAS ?
        * The Fun Motive getting stuck after a "Reset" ?
        * "Sort Inventory" and shelves with comicbooks ?
        * Verify Naked Tattoo in "Edit In CAS"
        * Test "Collect Investments"
        * 
        * Option to reset the "collected sets" values for a sim, in case they get messed up for some reason.
        * Ability to select a specific wish
        * Option to move a family to another home automatically based on home inspection criteria
        * Option to transfer clothing/genetics from a library sim
        * Option to "Form Group", which includes children
        * Option to change a sim's voice
        * Option to view the Household menu in "Edit Town" (Integration)
        * 
        * Ability to define the value of any slider without having to enter "Edit in CAS", including custom ones
        * Ability to mass set the value for a specific slider
        * Option to change the skin tone slider outside of CAS

        * Option to designate which rocks a sim has already collected (as a workaround regarding the EA export error)

        * Tuning to allow for Teen-YA Spouse or GBFriend relationships
        * Ability to create a household where the children are step-siblings ?
        * Ability to set the parents of a sim to roommates, and still retain the child link to both

        * Ability to display the "Known Info" tooltip during the Sim Listing Window

        * Additional sorting/filtering options for the Library listing in "Edit Town"
        ** Option to list only those households containing only humans, or a specific species
        ** Option to list all household containing at least one of a specific species
        ** Perhaps options for age filters, gender filters.

        * Map Tag information option that displays the current interaction for the sim
        * Progression Map Tag option that displays the information displayed by [[StoryProgression]]
        * Map Tag stamps that define a custom filter to use
        * Ability to define the color of the map tag applied during the process

        * Interaction to transfer the pattern style/color of an object to any number of other objects on the lot using an "Object Stats" listing

        * Tooltip for the CAS library sims that displays the sim's name

        * Ability to filter the Family Tree window to not display step-siblings

        * Option to define the value assigned to gender preferences during "Town \ Randomize Gender Preference"

        * Option to cancel non-steady gigs and steady gigs?

        * "Attraction: Both" filter
        * Ability to filter by the matchmaking results from [[Woohooer]]
        * Ability to manually set the Attraction value between two sims (how to stop the value from being recalculated by [[Woohooer]] later ?)

        * "Find Grave" option that does not move graves if they are already located in the mausoleum inventory

        * Option to use a "Hot Keys" nesting menu for the hot key interactions

        * A "Number of Generations separate from living sim" filter

        * Option to run "Drop Dreams" on a specific household

        * Option to randomize voices on sims who already have one set
        * Option to manually select a voice variation and pitch for a sim
        * Option to randomize weight

        * Setting to define a default sort method for "Sort Inventory"

        * A prompt to ask how many objects you want "Object Stats" to remove

        * Unlock ability to add more than one accessory for pets ?

        * Display the numeric value for skin tone sliders, and the values for the Basis sliders

        * Add mLifetimeEarningsForPensionCalculation to "Status \ Career".
        ** Some method of changing the value manually ?

        * Separate the current and original body shape settings for Weight and Fitness

        * Listing during "Transfer Genetics" that determines which facial regions to copy (or simply all of them)

        * Ability to add multiple body hair to the same location
        ** Ability to add multiple facial hair

        * Add random Unicorn Horn, Beard when the occult is applied to a horse

        * Alter "Go To Work" to operate against role sims. Possible ?

        * Eyelash length slider and setting it to no-length at all ?

        * Option to transfer human-form genetics to the werewolf outfit, so the user can tweek it from there

        * Enabling the CAS tooltips for the Facial blend panel sets (was part of the debugging tooltips)

        * Retaining the previous CAS outfit filter between CAS sessions
        ** One for the Buy/Build mode as well

        * "Weed All", "Water All" lot options

        * Numeric values for CAP Sliders

        * Possible to add two hairs to the same sim without the engine gakking ?

        * Option to change gender of sim
        * Option to change age-stage of sim

        * Option to turn off the Known Info tooltip in the family tree window

        * Option to set traits as "Trained"

        * Export for all the Status entries at one time

        * Ability to start an inactive party ?
        * Option to start a party situation on a neighbors lot
        * Issue throwing destination parties while on vacation ?

        * Tuning file which dictates the minimum and maximum ranges allowable for each slider during the "Randomize Genetics" operation

        * Means of seeing the Instance ID for a custom skin ?

        * Better category sorting for "Sort Inventory". One that sorts all ingredients (including custom ingredients, and non-plantable ones) together.

        * Ability to see the "Master Controller" menu on the map tags in "Edit Town" ?

        * Removal of nectar recipes

        * "ValidParts" package for CAS
        * 
        * Lot Interaction that pushes a "Teleport Me Here" onto a set of sims.

        * Different Facial hair per outfit (possibly based on Hair lock)

        * Blacklisting of skin-tones, makeup, costume makeup

        * Slider multiplier for pet panels

        * Ability to transfer the color/textures for one part to all other parts in the current outfit in CAS
        * 
        * Muscle Definition slider not working ?
        * 
        * "Status \ Household" includes the service household even when "Type of Sim" - "Resident" is used ?
        * 
        * Method of removing all accessories from an outfit in CAS
        * Option to transfer accessories to all outfits in CAS
        * 
        * InvalidParts files and <WorldTypes>Base, Downtown
        * 
        * Issue where sims who are the root of several branches of a family tree are only listed once in the custom family tree window
        * 
        * Issue changing occult while in CAS not removing occult effects properly
        * 
        * List Roommates separately on Household Status
        * 
        * Sell Inventory \ Everything But Perfect
        * 
        * Bed Assignment interaction
        * 
        * Options to alter [Generations] Reputation system
        * 
        * Fine-tuning the map tag information, allowing for mix and matching of data from multiple "Status" windows
        ** Hook for [[StoryProgression]] to access the tooltip information
        *
----
         * 
         */

        /*
         * Fixed the "Basic \ Tattoo" interaction so it appears for the correct sims
         * 
         */
        public static readonly int sVersion = 128;
    }
}
