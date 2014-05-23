// NOTE THIS MOD WILL NOT COMPILE WITHOUT THE DLL'S FROM THE STORE CONTENT IT ALTERS IN THE SIMS3 / COMPILER DIRECTORY
using NRaas.CupcakeSpace;
using NRaas.CupcakeSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Cupcake : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        public static GameObject activeDisplay = null;

        static Cupcake()
        {
            Bootstrap();
        }

        protected static void GenerateGoodies()
        {
            BakeryController.RefillDisplays();
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
            kDebugging = Settings.Debugging;

            Settings.ValidateObjects();

            BakeryController.UnlockRecipes();

            new Common.AlarmTask(5, DaysOfTheWeek.All, GenerateGoodies);
        }        
    }
}