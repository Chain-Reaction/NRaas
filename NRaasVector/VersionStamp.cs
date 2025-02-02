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
        public static readonly string sNamespace = "NRaas.Vector";

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Vector.ResetSettings();
        }

        /* TODO
         * 
         * Skill that only appears on Stats Panel to store statistics
         ** Typhoid Mary :  Has infected a certain number of other sims
         ** Immuno Suppressed : Has been infected multiple times
         * The number of other vectors in a sim as a scoring method
         * Virologist house calls for sims too distressed to make it to the hospital
         * Diagnose social interaction for Doctor sims
         * Add translation for "AllowRelationshipDelta"
         * Add translation for "MotiveAdjustmentRatio"
         * Crabs
         * Tranqs
         * Lazarus Plaque
         ** Changes sims into ghosts, and then back again
         * Elderidious
         ** Disease for elders that are long past their time for dying
         * Custom moodlets to denote each disease, that are always active for identified ones
         * A symptom that gives other sims moodlets
         * A symptom that makes sims flee the lot/room
         * Change the "Visit Doctor" interaction to display a tooltip if insufficient funds are available
         * 
All packs installed
Found: NRaas.Vector.SocializingActionAvailability
 NRaasAskContagious Added
 NRaasVaccinate Added
[E]Vaccinate: Action Not Found
[E]Diagnose: Action Not Found
         * 
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 20;
    }
}
