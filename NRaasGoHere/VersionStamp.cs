using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class VersionStamp : Common.ProtoVersionStamp
    {
        public static readonly string sNamespace = "NRaas.GoHere";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            GoHere.ResetSettings();
        }

        /*
         * 
         * Route.AboutToPlanCallback
         * Route.PostPlanCallback
         * RoutingComponent.OnRoutingEvent
         * RoutingComponent.OnRouteActionsFinished
         * RoutingComponent.OnRoutingObstaclesEncounteredEvent
         * World.OnRoutingEvent_ObstaclesEncountered_Callback
         * World.OnRoutingEventCallback
         * 
         */

        /* TODO
         * 
         * "Jog with" pet or fellow sim in GoHere ?
         ** Expansion of "GoForWalkWithDog"
         *
         * Can service sims be imprisoned ?  Unknown forced reset occurring at night that is causing issues.
         ** Caused by the Visit Situation exiting ?
         * 
         * Replacement of the EatHeldFood Singleton, that pushes sims to eat at a specific table or room, depending on the time of day (breakfast eat at A, lunch/dinner eat at B)
         * 
         * Replacements of meta-autonomy interactions "VisitLot", "GoToCommunityLot" that add a scoring value based on age or gender, with the ability to deny the action for same criteria
         ** Override of CalculateScore()
         * 
         * Replace the "Exit" interaction on the tent, and force the sim to move away from the entrance to allow the other occupant the chance to leave
         * 
         * "Go Here And " \ "Change Outfit" does not work because the outfit selection is run through the menu
         * 
         * Force caregivers to pick up new day care sims when they arrive
         * 
         * Notice when a new toddler appears or returns home for an active day care
         * 
         * PostRouteCallback hook to teleport sims if destination is on different lot
         * 
         * Issue with babies assigned to daycare not getting proper relationship with caregiver
         * 
         * Option to disable AncientPortral and Subway use for general routing
         * Turn off use of LLama and subway when sim has alien spacecraft or hot air balloon available
         * 
         * Queue dropping on loadup ?  Custom injection issue ?
         */

        /* Changes
         * 
         * 
         * Added "Allow Car Routing" default: True
         * 
         */
        public static readonly int sVersion = 42;
    }
}
