using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class RegisterPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.Register";

        public class Reapply : ProtoReapply<GameObject>
        { }

        public class Version : ProtoVersion<GameObject>
        { }

        public void OnPreLoad()
        {
            sPopupMenuStyle = RegisterPopupTuning.kPopupMenuStyle;
        }

        /* TODO
         *
* Ability to assign two or more sims to a register, who then work alternating sim-days

* More flexible Register mod, with ability to set hours, provide multiple sims to be cashier at different times of the day.
** Ability to set hours based on lot type
* Ability to specify the MetaAutonomyType on a bar-by-bar basis, for use in determining the career outfit for the sim.
** Custom outfits using SIMO ?
* How to stop active sims from receiving free skills when registered for a role ?

* Removing the static needs forced on the cashier roles, possible ?

* Replacement of the Paparazzi SimulateRoles code
** Option to change which level of celebrity a paparazzi will choose to follow (such as ignore Level 1 sims)
** Option to at least make the interaction cancellable
** Ability to escalate harassment by celebrity level (photos for low level, window peeping and trash rummaging for high levels)

* Age control for roles

* Custom Role for DJ objects on community lots
* Custom Role for Sauna objects
* Custom role for the Premium Content Industrial Oven object

* Don't assign role sims to the temporary pianos that are placed by other sims on the community lots

* Option to force newly assigned role sims to dehibernate right next to their role objects and forgo the whole commute time

* Lunar Cycle zombie control
** Option to suppress the "UI/LunarCycle/TNS:FullMoonAlert" notice

* Sim-level option to protect a sim from auto-selection

* Replacement of the Service objects with custom copies, so as to gain access to the virtual functions for possible alterations
** Such as having the service sims spawn on specific lots based on their service type

* Option to retire resident role sims once they reach Elder

* Ability to rotate the Commercial Lot Type for venues so a user only needs a couple of bars in town, rather than all of them
** http://nraas.wikispaces.com/message/view/Chatterbox/57356160

* Option to only disallow homeless to be chosen as tourists/explorers

* Paparazzi alterations : http://nraas.wikispaces.com/message/view/Register+Issues/59367228
** Ability to better interrupt paparazzi and tell them to buzz off, die by fire, or what not

* Interaction to "Summon Attendant" when the role sim has wondered off

* Direct hook to [[StoryProgression]] to create new role sims using "Replace Service With Immigrants"

* Hook to [[StoryProgresion]] as to whether a resident sim can be chosen for a role position or not

* Roommate Management ? Possible ?

* Tuning flag that pauses mod operation on loadup so the user can alter other settings

* Automation of University Professors.  Making them available at the Student Union blackboard for classes and such
         * 
----
         * 
         * Issue with role assigned sims not appearing in proper uniform in CAW worlds ?
         * Issue with Special Merchant interaction missing until a reload is performed ?
         * What is dropping the role on a resident sim being "fired" from their mixology job ?
         * 
         * Issue with Baristas not doing their job properly
         * 
         */

        /* Changes
         *          
         */
        public static readonly int sVersion = 80;
    }
}
