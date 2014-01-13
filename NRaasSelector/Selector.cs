using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.SelectorSpace;
using NRaas.SelectorSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tasks;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Selector : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Selector()
        {
            Bootstrap();
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        public void OnWorldLoadFinished()
        {
            DreamCatcher.OnWorldLoadFinishedDreams();

            kDebugging = Settings.Debugging;

            ReplacePickTask.Create<ReplacePickTask>();
            ReplaceSelectTask.Create<ReplaceSelectTask>();
        }
        
        public class ReplacePickTask : RepeatingTask
        {
            protected override bool OnPerform()
            {
                if (PickObjectTask.sPickObjectTask == null) return true;

                PickObjectTask.Shutdown();

                PickObjectTask.sPickObjectTask = new PickTask();
                Simulator.AddObject(PickObjectTask.sPickObjectTask);
                return false;
            }
        }
        
        public class ReplaceSelectTask : RepeatingTask
        {
            protected override bool OnPerform()
            {
                if (SelectObjectTask.sSelectObjectTask == null) return true;

                SelectObjectTask.Shutdown();

                SelectObjectTask.sSelectObjectTask = new SelectTask();
                Simulator.AddObject(SelectObjectTask.sSelectObjectTask);
                return false;
            }
        }
    }
}
