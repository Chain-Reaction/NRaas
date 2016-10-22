using NRaas.CommonSpace;
using NRaas.CommonSpace.Booters;
using NRaas.TravelerSpace.CareerMergers;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Helpers
{
    public class AgingCheckTask : Common.FunctionTask, Common.IWorldQuit
    {
        static List<Check> sChecks = new List<Check>();

        static AgingCheckTask sTask = null;

        protected AgingCheckTask()
        { }

        public static void Add(SimDescription sim, MiniSimDescription miniSim)
        {
            sChecks.Add(new Check(sim, miniSim));

            if (sTask == null)
            {
                sTask = new AgingCheckTask();            
                sTask.AddToSimulator();
            }
        }

        protected override void OnPerform()
        {
            while (sChecks.Count > 0)
            {
                Check check = sChecks[0];
                sChecks.RemoveAt(0);

                SpeedTrap.Sleep();

                try
                {
                    while (AgingManager.Singleton == null)
                    {                       
                        SpeedTrap.Sleep(0);
                    }

                    if (check.mSim.AgingState != null)
                    {
                        check.mSim.AgingState.MergeTravelInformation(check.mMiniSim);
                    }                    
                }
                catch (Exception e)
                {
                    Common.Exception(check.mSim, e);
                }
            }
        }

        public void OnWorldQuit()
        {
            sTask = null;
            sChecks.Clear();
        }

        public class Check
        {
            public readonly SimDescription mSim;

            public readonly MiniSimDescription mMiniSim;

            public Check(SimDescription sim, MiniSimDescription miniSim)
            {
                mSim = sim;
                mMiniSim = miniSim;
            }
        }
    }
}