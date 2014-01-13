using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CAS.Locale;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Tasks
{
    public class AgeTransitionTask : Common.FunctionTask
    {
        Sim mSim;

        protected AgeTransitionTask(Sim sim)
        {
            mSim = sim;
        }

        public static void Perform(Sim sim)
        {
            new AgeTransitionTask(sim).AddToSimulator();
        }

        protected override void OnPerform()
        {
            if (mSim.Posture != null)
            {
                mSim.Posture.PreviousPosture = null;
            }

            if (mSim.SimDescription.AgingState == null)
            {
                if (mSim.SimDescription.AgingEnabled)
                {
                    AgingManager.Singleton.AddSimDescription(mSim.SimDescription);
                }
                else
                {
                    mSim.SimDescription.AgingState = new AgingState(mSim.SimDescription);
                }
            }

            if (mSim.InteractionQueue != null)
            {
                mSim.InteractionQueue.CancelAllInteractions();
            }

            if ((mSim.BuffManager != null) && (mSim.BuffManager.HasTransformBuff()))
            {
                mSim.BuffManager.RemoveAllElements();
            }

            using (DreamCatcher.HouseholdStore store = new DreamCatcher.HouseholdStore(mSim.Household, false))
            {
                AgingManager.Singleton.AgeTransitionWithoutCake(mSim);
            }
        }
    }
}