using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ActorSystems;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class WoohooerPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.Woohooer";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Woohooer.ResetSettings();
        }

        public void OnPreLoad()
        {
            sPopupMenuStyle = WoohooerPopupTuning.kPopupMenuStyle;
        }

        /* TODO
         * 
         * Skill Opportunities to seduce sims
         * Prizes for completing opportunities.  Heart Shaped Bed, Woohooium
         * Consider bonuses when certain challenges are completed, making it easier to romance certain types of sims
         * Add the "Change with Spin" interaction when switching out of Naked woohoo
         * Option to make all the "WrongGender" socials a success result, so the gender preference system will still adjust the target
         * Option to designate woohoo object restrictions by room
         * Uninterruptable Woohoo interaction ?  One that can't be pulled off the queue by another sim.
         * 
         * Sims with high Renown should receive cheers or jeers from sims in the vicinity (reaction broadcaster)
         * Option to add Moodlet to pregnant teen sims that broadcasts a negative reaction amongst family and friends
         * Should woohoo interactions pushed on the target not force UserDirected priority ?
         * Ability to call for the services of a Professional in town via the Phone or Computer
         * Trait Scoring on which woohoo location to use during a standing social. "Outdoors" using treehouse, or objects outdoors.
         * 
         * Can the skinny dipping changes be broken out to their own mod ?
         * Option for skinny dippers to stay naked after having clothes stolen by pranksters
         * Option to disable switching into the Towel after skinny dipping in the pool (Override of Pool.GetOutOfPool and OnExitPosture ?)
         * 
         * Rolling romantic dreams for teenagers (sDreamTrees, how to restrict to teen-teen ?)
         * DreamNodeInstance.mAgeFlags
         * 
         * Scoring for : A friends with B, B likes but not romantic with C, A steers clear of C
         * 
         * Town-Founder-  Have at least 25 babies with other partners using risky or try for baby option.
         * 
         * A "Find Room" interaction that brings up a prompt of available woohoo-able locations, and directs a group to head there
         ** If the item is in a abandoned home, greet the sims onto the lot
         * 
         * Closer interconnect between the Town Personalities and the Kama Simtra "Professional" status
         * 
         * The [[Generations]] Reputation system, expansion possibilities ?
         * Using the ReputationEvent to hook changes to reputation ?
         * 
         * "Make Sim Think about Me" as a means of seduction, pushing the Short-Term Context up to a higher level
         * 
         * Unlock the "Professional" status for animals as a "Stud Fee" for males
         * Rewrite of "Breed Mare" to use a list of Stallions most recently offered as Studs via the Equestrian center.
         * 
         * Change Active Service sims back into uniforms after woohoo
         * 
         * Rewrite the "Risque" description so it is more obvious that it refers to Risky Woohoo
         * 
         * Elder autonomous pregnancy.  Why does the block not work properly ?
         * 
         * Option to set the base attraction for each sim pair
         * Sim-level options ?

        * Option to gauge satisfaction for both sims based on a combination of both sim's renown

        * "Mail Order Bride" immigration of a sim that matches your sim's attraction level

        * Trait-Scoring to determine whether a sim wants to switch into clothes after naked woohoo
        ** Daredevil only has risky which is great, and it got me thinking that Great Kisser should only have kissing interactions, but always available (pass or fail) similar to the inappropriate trait kissing option.
        ** Eye Candy should only have options available when the buff is active, and should probably be more about high percentage chance of successful flirt, regardless of gender preferences - This would give a way to train a target sim's preference to a different gender.
        ** Master of Seduction could be enable going straight for asking a Sim to be a partner/spouse without going through the motions.
        ** No jealousy could become similar to the "Convince to.." party interactions, and about encouraging romantic interactions in others.
        ** Family Orientated could have only the "Try For Baby" interaction always active for partners, and the "Propose Marriage" for non-partners.
        ** Atractive could be more about getting something for nothing like free night club bouncer bribes, or asking another sim to buy you a drink.
        * Woohoo Interaction Level that dictates visibility based on traits. Flirty (always), Hopeless Romantic (with partner only, if partnered. Or anyone if unpartnered)

        * Option to check trait-scoring during user-directed on the Target, when testing "Try For Baby" and "Risky"

         * Option to disable autonomous flirting between in-laws
         ** How to do this efficiently?

        * Phone request by Rendezvous partons for an encounter with an active sim

        * Near Relation Polyamorous Jealousy Option

        * Replacement of the "Chat with Someone" on the computer to better match age-range
        ** Less children chatting with creepy adults sort of thing.

        * Option to define the number of days off given after marriage (WeddingCeremony.sDaysOffAfterWedding)

        * Link to [[Dresser]] to check that mod's settings for switching outfits during room change (for Naked Woohoo purposes)

        * Override of the "can I Sleep Over?" social interactions to remove the LTR restrictions, or replace them with a "Closely Related" check.

        * Different animation when initiating woohoo, perhaps the "Alluring Greet" animation
        * Option to have the sims make out in bed prior to woohoo
         * 
         * Privacy should be checked prior to getting naked for bed
         * 
         * Online Dating, anything to do here ?  Perhaps limit it by monogamy scoring ?
         * 
         * Add option for a sim to sleep naked (StoryProgression or Woohooer ?)
         * 
         * Double check coding for "Positively Orgasmic"
         * 
         * Butlers and autonomous hot tub woohoo ?
         * 
         * Re-evaluate the use of asymmetric attraction scoring
         * 
         * Unlock "YouShouldEx" for any type of occult
         * 
         * Juiced interactions and the issue with the "Juiced" motive auto-maxing for inactives
         * 
         * Add tooltip when a sim is not in the mood due to user-directed trait-scoring
         * 
         * Naked sun-bathing ?
         * 
         * Double check PetHouseWoohoo cannot impregnate using "Woohoo!"
----
         *
         * Issue with being unable to queue multiple hot tub woohoos
         * Issue with teen skinny dippers immediately cancelling posture CheckIfTeenOrBelowOnLot
         * issue with skinny dippers getting redressed by "Switch On Outside" if their outfits are stolen
         * Issue with ClearPartner in the PetHouse Woohoo interaction
         * Check "Get Frisky" and the Actor Trailer
         ** Issue with sims leaving the actor trailer immediately after the deed. They should be staying in there. 
         * Issue with Hot Tub interactions for teens appearing as canceled interactions on queue ?
         * Issue with interruption of Shower woohoo causes both sims to get stuck permanently waiting to sync (a shoo from a sim using the toilet for instance)
        
         * Issue with elders autonomously "Try For Baby" ?
         * Test Polyamorous Woohoo Jealousy
         * Block the use of "Woohoo" while in labor
         * Woohoo Dropping Queue Issue still exists ?
         * Verify Prom fix in Woohooer
         * Test Rendezvous with sims that are already in-town, rather than hibernating
         * 
         * SingSongRomanticInteraction has CanHaveRomanceWith check
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 121;
    }
}
