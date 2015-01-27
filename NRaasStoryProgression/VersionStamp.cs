using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class StoryProgressionPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.StoryProgression";

        public class Version : ProtoVersion<GameObject>
        { }

        public void OnPreLoad()
        {
            sPopupMenuStyle = StoryProgressionPopupTuning.kPopupMenuStyle;
        }

        /* Todo
         * 
         * Town Cougar ?
         ** Canoodle with younger sims
         ** Allow Elder : False
         * Town "Gold Digger" ?  Black Widow style ?
         * Town Hippie
         * Scenario where a town personality resigns their position (for instance a Casanova getting married, or bully being upsurped by a gang member)
         * Thug Scenario to sick their dog on someone
         * Casanova, Kingpin Refund scenarios for objects they purchase
         * Kingpin Scenario to steal deeds
         * Active Particpation in Personalities using interactions
         ** Unlock Bribe for Kingpin, Cat
         ** Unlock Recruit for Kingpin, Loon, Nerd, etc.
         ** Unlock Woo for Casanova, Bike, Lestat
         ** Change [[Woohooer]] to use rules for clans to determine "Woohoo!" availability when not totally unlocked
         ** Have long-term relationships change automatically based on clan
         * Meteor Strike as the ultimate nerd vengeance ?
         * Scenario to steal deeds for Baron
         * "Ponzi Scheme" scenario for Baron
        * Check the scoring for "Gigolo Leave 'Em" and ensure that the sim does not score high in using gigolo services
         * Add Scenario where Nosferatu attacks Werewolves
----
        * Situations involving the Town personalities and sims in the active family ?
        * Ability to initiate a situation involving one of the personalities and an active sim, via the phone.  Nerd tutoring, or Gigolo services.
        * 
        * Saving the Household Options in BioText and reading them out on Import into a new town?
        * Ability to limit sims with wild hairstyles to non-uniform careers (ie: not military or police)
        * 
        * Add scenario for REALLY rich sims to hire a Butler
        * Some way to actually handle hiring maids without the service population exploding in the process ?
        * 
        * Progression memories of prior scenarios, that influence the chance of a future scenario occurring between sims
        * 
        * Cooldown between start of "romantic interest" and "partnering"
        ** Requires a value for each relationship, messy
        *
        * Option to manually choose the genetic parents for immigrants
        * 
        * System of Goals and Anti-Goals that govern what activities an individual sim is allowed to do
        * 
        * Option to use a horse as a commute option, rather than a car ?
        * 
        * Ability for children to skip school by "forging permission", "hacking the computer", things like that
        ** Turns off the "Go To School" push for the day, with the chance of getting caught via the curfew system
        *
        * Personal Bank Accounts that don't actual hold the money itself, but define the amount that is owed this sim from Family Funds
        * 
        * “Balanced Careers” overriding the trait-scoring system.  It should be moved to *after* trait selection.
        * 
        * Interaction to call police and remove all uninvited sims from the lot
        * 
        * Ability to specify certain CAS Parts as genetically inheritable. For custom content for instance.
        * 
        * Option to specify the maximum number of sims allowed to attend at any one rabbithole
        ** Not sure what to do with the excess though, perhaps only available if there is another rabbithole to use
        * 
        * A situation where inactive children of an active sim are added to the active family temporarily as part of a split custody scenario
        * 
        * Ability to gain a "Retired" career pension increase via Workaholic computer interaction ?
        * 
        * Issue with the Facial blend inheritance, where all custom sliders in the "Mouth" region are inherited from one parent of the other
        **    Can the facial blends be inherited individually without producing freaks ?
         *
        * A "On Vacation" rabbithole interaction that simulates an inactive sim being on vacation, during which WA LTWs could be satisfied in some manner.
         * 
        * Reasons for moving to a new home:
        ** Athletic Sims would prefer a house with athletic stuff
        ** A Sim with an LTW of "Bottomless Nectar Cellar" would have a high wish to move to a house with a Nectar maker
        ** Elder Sims would like to move out to a smaller house
        ** Just married YA's would heavily prefer houses with cribs and kids stuff
        ** Love the outdoors would prefer lots of outdoor stuff (BBQ's, pool etc)
        ** A Green Thumb would be looking for gardening plants
        ** A Natural Cook would prefer a house with high quality kitchen stuff
        ** A Snob would want lots of decorations
        ** a Frugal would like a cheaper house
         *
        * Can Career immigration be disabled during loadup ?
        *
        * Computer interaction that points out the current [[Late Night]] Hot Spot
         * 
        * Option to force every couple in town to have at least one child
         * 
        * Ratio of "Good" to "Bad" traits for use by the trait reselection scenario
         * 
        * Option to immigrate relatives of existing families.
        ** Apply a family tree connection with similiarly named sims already in town ?
         *
        * Increase relationship decay rate when two sims are romantic interests
         * 
        * Add startup prompt that asks the user to switch to "Blood and Friends" for their "Stories" settings
        ** Prompt to adjust "Marriage Name"
        ** Prompt to adjust "Manage Homeless"
         * 
         * Skill Decay
         * 
         * Mailbox interaction for transferring funds to another family
         * 
        * Pushing service sims around town, to community lots which match their job positions
        ** Repairmen to junkyards
        ** Policeman to police station
        ** Firefighters to the fire station
        ** etc.
        ** Will this interfere with the processing of their regular service duties ?
         *
        * Option to define the maximum number of sims that can be employed in a specific career
         * 
        * Option to restrict the number of sims added to a particular rabbithole career, to reduce routing issues
         * 
        * When an active has a baby with someone other than their partner ? Can the inactive be scripted to "ask to marry" or "scoff off the event" scoring-wise

        * Add StoryProgression component to stop the mod from picking the same couple for different scenarios within the same simday

        * Holding the Deed for the Police or City Hall as an "Evil" kick back/bribe element
        *          
        * Option to do active homework on a rotating schedule.  Say every-other day.

        * Pushes for Toddler training.

        * Purchasing of Lifetime rewards for inactives, based on trait-scoring ?

        * Add scenario to move into a smaller home, maybe if the sims are elderly and have no more children ?

        * Option to limit marriage to sims close in daily age

        * Review moving in general, look for reason a household would move back to their original home after moving out.
        * Sims can move into homes where there are insufficient beds to support them, and are then moved out again by the "Out of Nest" scenario.

        * Scenario to change the body-shape of sims around town based on trait-scoring

        * Option to disable use of the fire truck in the FirefighterSituation

        * Active career job difficulty scaling

        * Ability to limit the number of notices fired for a scenario each cycle

        * Look into means of resetting sims that have objects stuck in their hands (such as the chisel from sculpting)

        * Method of controlling the host of an NPC Party and ensuring they don't call a party while they are at work

        * Scenario that relates music/food favorites to traits, rerolled on teen and young adult age-up

        * Issue where "Overcrowded" moves a single sim, when it should try to move everyone to a new home first
        * One of the move scenarios moved a husband out of the lot without their wife and child

        * Add the "Band" career to the Dream Job system, and setup progression
        * Handling of inactive bands (perhaps pushing gigs and getting paid)
        * Progression of the DayCare
        * Allow assignment of day care if [[GoHere]] is installed
        * Add progression scenarios for new Lifetime wishes

        * Implement MidlifeCrisisManager in appropriate locations

        * Possible to run an Interloper or other move-out scenario against inactive sims in the active household ?

        * Add a cleanup routine to remove dead remains from in front of the hospital

        * Scenario like "Wealth Move" that moves celebrity families into Celebrity designated houses

        * Issue with grounding not understanding that sims are simply coming home from school or work

        * Consider reducing the LTR delta changes produced by the various scenarios
        * Option to define the amount of relationship granted per friendship scenario
    
        * How much impact does current LTR have on who flirts with who in StoryProgression ?

        * Option to limit celebrity gain to only those sims that have specific careers

        * Option to specify a "daily chance of retirement" that is checked each day, rather than a straight "retire on age-up"

        * Scenario to have an inventor create simbots

        * Explicit push to work out at gym for sims requiring athletic skill

        * Option to alter StyledNotification.kDefaultTimeout parameter with an in-game option
        * 
        * "Chance of Occult" control of the Ghost Inheritance ?
        * 
        * Pet Emigration if no household can be found in town
        * 
        * Option to always force skill building school tones
        * 
        * Replacement of the Late Night Celebrity system to make it so the guests and hosts actually show up
        * 
        * Option to choose the type of vehicles sims are allowed to purchase
        * 
        * Progression of [[Showtime]] active careers
        * 
        * Option to add a copy of newly written books to random residential bookshelves around town
        * 
        * Add code to handle royalty alarms for hibernating sims (such as those on free vacations or at boarding school)
        * 
        * Add "Magically Complete" into the Homework scenario
        * 
        * NameList Tuning for different town-files
        * 
        * Sim.GoInside and the HeatingUpScenario ?
        * 
        * Integration of EA Attraction scoring system
        * 
        * Inheriting [Werewolf] genetics in [StoryProgression], possible ?
        * 
        * [StoryProgression] versus the homeless showtime career sims
        * 
        * Turn and Slay need to handle Zombies
        * 
        * Alchemy Station automation
        * 
        * Inactive aliens and UFO vehicle purchases
        * 
        * Using the "Retired" career and a countdown to simulate unemployment after being fired
        * 
        * Check Visitor scenarios that run against the active household
        * 
        * "Breakup" scenario occurred between two sims with friendly rel ?  Why ?
        * 
        * Does having no Hospital in town cause problems with Death Pushes ?
        * 
        * How to restart carpooling push after switching households near the time the process starts
        * 
        * Issue with instruments piling up in sim inventories
        * 
        * Issue with genetics of second-born pets not being genetically inherited ?
        * 
        * Add Cleanup for existing Active Career jobs if user turns off the "Allow Career" on a sim
        * 
        * Check on how inactive active firefighters are paid in relation to the time they spend at work
        * 
        * Change color of "Friends" map tag so it looks different than the Proprietor Gig map tags
        * 

        * Friend Building scenario between sims with similar pet species
        * Option to define a house as being haunted, and then running Ghost Hunter jobs against it to produce the effect

        * Upper limit to the ratio of partnered sims to singles

        * Any way to stop "Wealth" movers from taking households that would be better suited to satisfying home inspection failure families ?

        * Tally of number of arrests, and then an override of the "Go To Jail" interaction that increases the time spent based on tally

        * Option to open an unopened lot that was the target of a scenario push
        ** An alarm set for some point in the future would then close the lot (provided the user is not actively looking at it)

        * Pets stories:
        ** Friend/Enemy
        ** Mating (aka marriage)

        * Pet scenarios
        ** Destruction scenarios (destroying furniture, digging holes)
        ** Scenarios where a pet guards a sim against attack from a personality
        ** Overly protective pets or Lassie style helpers
        ** Mass breeders to improve the pet pregnancy chances
        ** "Wild" domestic horses which ignore are allowed to be pushed by the mod to wander around town

        * Personality option that defines an upper limit to the number of members
        * Option for an upper limit on the number of leadership positions a specific sim can hold at once
        * Option for an upper limit on the number of different memberships a specific sim can hold at once

        * Way to make the age-up prompts appear in a particular order:
        ## Trait
        ## Lifetime Wish
        ## School
        ## Part-time Job

        * Prompt to purchase lifetime rewards for inactives when they exceed a certain amount of points

        * Option to define Cooldown for unexpected pregnancies, or apply the expected pregnancy Baby-To-Baby cooldown to those conceptions as well

        * Option to define how quickly the "Work Outfits" scenario cycles, or change the scenario to fire on "Lot Changed" when the sim gets home

        * Option for "Wealth Move" to stop sims from moving into households which have more beds than they require

        * 
        * Option to push sims to the Martial Arts Academy to work on skill

        * Option to determine the valid times for npc parties (whether day or night)
        * Curfew options should stop sims from attending parties, or pull them out of them when curfew starts
        * 
        * Scenario to purchase laptops for sims

        * Use of the newspaper to broadcast stories

        * "Replace Service with Immigrants" and the pet pools

        * Gender balancing on pet adoptions is necessary

        * Inheritance of werewolf genetics

        * Push Sims to festival lots on the festival day
        * 
        * Can arranged marriages be fired against active sims ?
        * 
        * A cooldown mechanism between giving sims new jobs, using the Retired career as a placeholder
         * 
         * Sim-Level Inheritance options
         * 
         * Pet Naming option that defines a newborns last name as the combination of the parent's first names
         * 
         * Does the mod assign Homemaker to sims ?
         * 
         * Replacing the Velvet Ropes interactions with ones that check Caste association
         * 
         * Cancel Field Trip situations for snow days and holidays
         * 
         * Option to define the chance for "Push Collecting"
         * 
         * Replace "Private School Prompt" with a general one for both school types
         * 
----
         * 
         * Move "Child Support Payment" to "Sim Options" and drop "Child Support Rich Multiple"
         * Move "Elder Support Payment" to "Sim Options"
         * 
         * Break Extra into independent mods, and roll relevant scenario into the other modules
         * 
         * Give "Push Fishing" a translation and make it visible
         * 
         * Option to hide the Caste listing options on the options window
         * 
         * Trait scoring for new rabbithole careers and new career tones
         ** GameDeveloperCareer.WorkResearch
         ** GameDeveloperCareer.OnTheJobTraining
         ** ArtAppraiserCareer.PracticePaintingSkill
         *
         * Occupation.CanAcceptCareer()
         * 
         * Add a "Sim Options \ Allow Enroll In University"
         ** Change system to allow teens to enroll
         * 
         * Career selection based on academic degree completion
         * 
         * Purchasing of boats if a mooringpost is available
         * 
         * Include roommates in "Replace Service With Immigrants"
         * 
         * Better handling of Service Imaginary Friends in the [[Woohooer]] hook
         * 
         */

        /* Issues
         * 
         * "LynchMob" scenario is extremely slow
         * Check whether inactive book royalties go towards career experience
         * Still possible for a red-haired baby scenario to leave the child with the father
         * 
         * Issue with two inactive sims constantly cycling through promotions due to “Only One May Lead”
         * Issue with the "Death" notices not appearing, though they appear in the story log
         * Disgraced moodlet not being applied during a disgrace event ?
         * Resident sims vanishing from town (possibly role sim related)
         * “Assign Dream Job to Employed” is not stopping reassignment of sims to new jobs
         * 
         * Are promotions occurring too often ?  Issue with job performance calculation ?
         * Are inactives gaining job performance too quickly ?
         * 
         * Are Headhunter immigration properly gender balanced ?
         * 
         * Issue with the "Go To School" interaction being canceled and replaced by a new one ?
         ** Caused by sim being picked up by the buspool spawned by a sibling who does not have a personal vehicle
         * Test the buspooling system in StoryProgression
         * Inability to cancel the carpool/buspool interaction ?
         * Issue with carpool not appearing, and sims not being pushed to work, related to not having Expanded module installed ?
         * Issue with carpooling message not appearing for a 7am pickup, but appearing for sims in the family that leave at 8am.
         * Carpool notice progressively gets closer to the actual start time.
         * Issue with the carpools for sims that need to be at work at 12am not working properly
         * 
         * LTW coding in immigration scenario not working properly, test it.
         * Possible issue with pushing sims involved in an active firefighter job
         * Issue with active sims being woken up from sleep to visit a lot
         * Can StoryProgression render a sim homeless if you turn off certain types of move scenarios ?  "Broken Home Move" for instance ?
         * Issue with emigrated sims remaining on the relationship panel for sims ?
         * Are self-employed repairmen not getting the priority during repair pushes in StoryProgression ?
         * Weird issue where active sims gain relationship with new immigrants they have never actually met ?
         * Playable ghosts vanishing after being moved to an inactive home
         * Break and Repair scenarios take unusually long to run ?
         * Issue with "Manage Bosses" not working ?
         * Issue with emigration leaving invalid coworkers assigned to actives ?
         * Test performing a Vaccination Clinic during work hours with StoryProgression
         * Issue with slain vampires reverting to vampires within a sim-day
         * Check the "Break Up" notice and ensure that it fires properly for everyone
         * Issue with the "Not So Routine Machine" and carpooling ?
         * A self employed sculptor moved to a home lacking a sculpting station ?
         * Unlock carpooling for use in [[Traveler]] worlds if necessary
         * Days off for Prom Situation not working ?
         * Unable to turn off proms in [StoryProgression] ?
         * Issue with inactive Interior Designers not progressing ?
         * Does "Estranged Visitor" create stories properly ?
         * Chess Push not working ?
         * Check the "Film" career for proper trait scoring
         * Verify the use of "Set Booby Trap" for adults
         * Test Propagate Mourn
         * Test the "Heating Up" scenario
         * Issue with Heating Up push not handling fenced outdoor rooms properly
         * Deaths by Assassination are not noted in the [StoryProgression] log ?
         * Issue with being unable to make friends, flirt, etc with service sims ?
         * Issue with not receiving stories about service sims flirting ?
         * "Allow Inventory Management" not stopping all sales ?
         * Issue with the "Steady Visitor" scenario firing constantly for the same sim.  Need reduction in chance of story ?
         * Crash when returning to full-screen after minimizing
         * Missing Story: BirthdayGatheringSelfHost
         * Arranged marriage option being reset ?
         * Animals appearing in the "Went into Debt" story
         * Issue where "Disallow Trait" does not stop EA trait selection from simply reselecting one of the disabled traits
         * IBotBed requirements for home inspection
         * 
         * Issue where "CasteDisallow" and "DisallowCaste" have the same translation, so cannot use both options in same listing
         * 
         * See if "Tourists \ Allow Aging" being applied to tourists properly
         * 
         * 
         * Test curfew in "AllowPushToLot"
----
         * 
         */

        /* Changes
         *          
         * 
         */
        public static readonly int sVersion = 267;

        void TraitTest()
        {
            /*
            TraitNames.BetterBartender
            TraitNames.SupernaturalSkeptic;
            TraitNames.SupernaturalFanTrait;
            TraitNames.SpellcastingTalentTrait;
            TraitNames.FestivalFrequenter;
            TraitNames.AvanteGarde
            TraitNames.HotelMogul
            TraitNames.LovesToSwim
            TraitNames.LungsOfSteel
            TraitNames.Sailor
            TraitNames.StrongStomach

            TraitNames.FairyQueen;
            TraitNames.SuperVampire;
            TraitNames.WerewolfAlphaDog;
            TraitNames.WitchMagicHands;

            TraitNames.BotFan;
            TraitNames.MaintenanceMaster;
            TraitNames.WonderlandSim;
             * 
            TraitNames.ArtisticAlgorithmsChip; Enables painting interactions
            TraitNames.ChefChip; Enables cooking interactions
            TraitNames.CleanerChip; Enables cleaning interactions
            TraitNames.FisherBotChip; Enables fishing interactions
            TraitNames.GardenerChip; Enables gardening interactions
            TraitNames.MusicianChip;  Enables music interactions
            TraitNames.HandiBotChip; Enables Repair interactions

            TraitNames.AbilityToLearnChip; Enables Skill building
            TraitNames.CapacityToLoveChip; Enables Social
            TraitNames.EfficientChip; Better at work and skills
            TraitNames.EvilChip; Enables Social
            TraitNames.FearOfHumansChip; Dislike humans
            TraitNames.FriendlyChip; Enables Social
            TraitNames.HoloProjectorChip;
            TraitNames.HumanEmotionChip; Enables Social, Mood
            TraitNames.HumorChip; Enables Social
            TraitNames.MoodAdjusterChip; Enables Social
            TraitNames.ProfessionalChip; Allow for Careers
            TraitNames.RoboNannyChip; Better with children
            TraitNames.SentienceChip; Enables Fun, Social, Mood
            TraitNames.SolarPoweredChip; Hunger while outside

            */
        }
    }
}