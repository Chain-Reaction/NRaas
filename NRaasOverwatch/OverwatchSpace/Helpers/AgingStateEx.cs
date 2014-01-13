using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OverwatchSpace.Helpers
{
    public class AgingStateEx
    {
        public static bool IsInactiveActive(AgingState ths)
        {
            if ((!SimTypes.IsSelectable(ths.SimDescription)) && (ths.SimDescription.Household == Household.ActiveHousehold))
            {
                return true;
            }

            return false;
        }

        public static void AgeTransitionWithoutCakeCallback(AgingState ths)
        {
            bool useInteraction = IsInactiveActive(ths);

            if (AgingManager.Singleton.Enabled)
            {
                if (ths.SimDescription.Elder)
                {
                    AgingManager.Singleton.AgeTransitionWithoutCake(ths, useInteraction);
                }
                else if (!AgeUp.ActiveAgingInteraction)
                {
                    AgingState.AgeTransitionWithoutCakeFailureReason reason;
                    Sim createdSim = ths.SimDescription.CreatedSim;
                    if (ths.IsSimAllowedToAgeUpWithoutCake(createdSim, out reason))
                    {
                        AgingManager.Singleton.AgeTransitionWithoutCake(ths, useInteraction);
                    }
                }
            }
        }

        public class AgeTransitionWithoutCakeTask : Common.FunctionTask
        {
            AgingState mState;

            protected AgeTransitionWithoutCakeTask(AgingState state)
            {
                mState = state;
            }

            public static void Perform(AgingState state)
            {
                new AgeTransitionWithoutCakeTask(state).AddToSimulator();
            }

            protected override void OnPerform()
            {
                AgingStateEx.AgeTransitionWithoutCakeCallback(mState);
            }
        }
    }
}
