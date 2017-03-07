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
        public static readonly string sNamespace = "NRaas.Careers";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Careers.ResetSettings();
        }

        /* TODO
         * 
         * New opportunity that asks a teen to enroll in a specialty school
         * Figure way to explain the "Download" and submitting of homework better for Home Schooling
         * Ability to transfer performance from school to a career
         * 
         * Apply "Tinker" interaction to vehicles ? Possible ?
         * 
         * Ability to specify a generic "Visit Rabbithole" interaction that can be assigned to various holes with the intent of increasing a generic metric.
         * 
         * Reaper outfit needs sim to switch skintone to the "Invisible Skin" to look proper.  Warning prompt when doing so.
         * 
         * Ability to hold two jobs at the same time ?
         * 
         * Additional Skill Based careers:
         * Logic - Tutoring ?
         * Charisma - Matchmaking ? Party planning ?
         * Cooking - Catering ?
         * Gaming - "online tournies" or playing against another sim for bets ?
         * Mooch - Need to replace the mooch socials to grant access to the mooch value
         * 
         * Opportunity to train sims at their home for bonus pay ?
         * Opportunities to fix or upgrade an object for extra pay.
         * 
         * "Socialite" Trait that allows sims to create social circles.  Or perhaps a future component of the Charisma career ?
         * 
         * Review the buspooling and see if there is another way to re-enable the buspool for specialty schools
         * Ability to Designate the Lot for an EA career.
         * 
         * Increase assassin skill to full 10 levels
         * Ability for a sim to survive an initial attack, and evade assassination ?
         * Pure skill based opportunities for Assassination.
         * Evil Aura for assassinations, perhaps attached to a moodlet
         * Overhaul of the kill interactions for assassination. Make them more challenging to perform by attaching them to existing systems in game (electrocution requires setting up a trap on an item).
         * 
         * Police Academy school
         * Sports Academy school
         * Child level Trade schools
         * Medical School ?
         * Beauty School ?  Not sure what this one would entail.  Stylist tone performance ?
         * Political school ?
         * 
         * Unique rabbithole script distinguished by an ID ?  Possible to devise without too much hassle ?
         * 
         * Creation of custom Active careers ?
         * 
         * Ability to import custom dreams .  Possible ?
         * Lifetime Wish to be the Top of the Education career.
         * 
         * Ability to specify a custom audio key for each Tone
         * 
         * Option to define a "Celebrity Only" trade school
         * 
         * Add a "Work Hard" tone to the Pro Sports career
         * 
         * Ability to specify multiple careers via <TransferCareer> 
         * Ability for <TransferCareer> to handle non-rabbithole careers
         * 
         * Add fields for multiple branch prompts
         * Ability to call an inactive self-employed repairman over to fix the items in the actives house.
         * Have sims call active when a repair job is available
         * 
         * Generic interactions added to various objects in town that increase custom skills, increase custom metric counts, or job performance. Possible to add multiple, and gate by career level.
         * 
         * "Best in Show" style cat and dog competitions ? New skill required for such.
         * 
         * Field to create an object in the sim's inventory, such as the medical beeper. Remember to delete it when the career is removed
         * 
         * Should the Traits : Ambitious, Professional Simoleon Booster, and Entrepreneurial Mindeset be used in self-employed career performance ?
         * 
         * Ability to set career work hours for any rabbithole career, EA or otherwise
         * 
         * Ability to perform on stage using a band
         * 
         * Homemaker Opportunities: PTA meetings, parent's night, school play, teacher conferences
         * 
         * Partier
         * Combination of DJ, Dancer, Mixologist career
         * Money made dancing at venues, working the DJ booth, and moonlighting as bartender
         * 
         * Field Trips for homeschooling ?
         * 
         * Store a kills name and type of death in Assassination
----
         * Issue with the "Train" interaction appearing for children while training butlers, but for no one else ?
         * 
         * Issue with standing social interaction "Train" not working for Martial Artists
         * Not getting paid for training on the martial training dummy ?
         * Issue with "Reroll Sparring Opponent" not working properly
         * 
         * TransferCareer career event, and the ability to transfer to non-rabbithole careers
         * 
         * Issue with grounded teenagers getting in trouble for attending a custom school
         * Issue with "Recruit" not appearing in menu ?
         * Issue with Handyman crashing the game ?
         * 
         * Rabbithole Careers for animals ?
         * 
         * Issue with "Spar" interaction in Careers mod not working, dropping from queue ?
         * Issue with sculptor's recording twice as much in self-employed exp than actually made ?
         * 
         * 
         * Store a kills name and type of death in Assassination
         * 
         * "The Family" sending sims to jail whenever they try to go to work ?
         * 
         * Change the Assassination standing socials to inject as "Mean \ Assassination"
         * Different icon for Assassination standing socials.  Possible ?
         * 
         * Martial Artist career sims not getting paid for training ?
         * 
         * Issue with receiving a lot of money [Careers] busking at a bar ?
         * 
         * The "Chilling Before Work" error
         * 
         * Find out why actives can't "Ask for Promotion" if boss is part of household
         * 
         * Custom Academic Degrees
         ** Whiteboard.BaseUseWhiteboard
         ** UniversityWelcomeKit.TakeAptitudeTest
         * 
         */

        /* Changes:
         * 
----
         *          
         * 
    SpellCrafter = 0x494013358591e72L,
         * 
         */
        public static readonly int sVersion = 88;
    }
}
