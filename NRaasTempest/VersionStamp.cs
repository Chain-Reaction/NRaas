using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas
{
    public class VersionStamp : Common.ProtoVersionStamp
    {
        public static readonly string sNamespace = "NRaas.Tempest";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Tempest.ResetSettings();
        }

        /* TODO
         * 
        * Have playing in sprinklers reduce sim temperature
        * Drinking specific drinks should increase or decrease a sim's temperature (how to specify which drinks do what ?)
        
         * 
         * There are no holidays while at University ?
         * 
         
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 14;
    }
}
