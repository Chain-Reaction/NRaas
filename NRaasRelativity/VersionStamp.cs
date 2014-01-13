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
        public static readonly string sNamespace = "NRaas.Relativity";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Relativity.ResetSettings();
        }

        /* TODO
         * 
         * Relativity that only affects certain interactions
         * Quick presets that can switch between various pre-defined speeds
         * Issue where running the game at ultra-low speeds (aka "1") the game clock will bounce up by a large amount on load-up
         * Option to specify a specific sim-time to automatically pause the game
         * Issue with plant decay speeding up as time slows, but the sprinklers still running on clock time.
         * Issue with Active Career jobs vanishing after a couple of sim-minutes when the speed is set to slower
         * 
         * Ability to specify the time for an alarm clock wake up call.
         * 
         * Ability for sims to get bored over time doing the same "Fun" based interaction (reduced fun the longer one uses the object non-stop)
         * 
         * Issue with the progress meter filling to full immediately upon starting a repair job, though the interaction continues as normal
         * 
         * Thought balloons are relative to the game-clock ?  Should be adjusted by relativity
         * 
         * Issue with "Energy" motive acting as if it is Relative, even though it is not set such
         * 
         * Relativity interfering with Showtime songs ?
         * 
         * Test the persistence of the Speed setting when changing worlds
         * 
         */
        /* Changes
         *
         * "Skill Gain Factors" approach adjusted to better handle certain types of interactions
         * 
         */
        public static readonly int sVersion = 24;
    }
}
