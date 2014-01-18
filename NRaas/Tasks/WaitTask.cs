using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Stores;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Tasks
{
    public abstract class WaitTask : Common.FunctionTask
    {
        bool mCompleted = false;

        protected WaitTask()
        { }

        protected abstract void OnWaitPerform();

        protected sealed override void OnPerform()
        {
            mCompleted = false;
            OnWaitPerform();
            mCompleted = true;
        }

        protected static T Wait<T>(T task)
            where T : WaitTask
        {
            if (Simulator.CheckYieldingContext(false))
            {
                task.AddToSimulator();

                while (!task.mCompleted)
                {
                    try
                    {
                        Common.Sleep();
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            else
            {
                // Run immediately
                task.OnPerform();
            }

            return task;
        }
    }
}

