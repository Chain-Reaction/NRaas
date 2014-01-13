using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.PorterSpace;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
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
    public class Porter : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        protected static Dictionary<ulong, int> sExportedSims = new Dictionary<ulong, int>();

        static Porter()
        {
            Bootstrap();
        }

        public static Lot GetLot(GameObject obj)
        {
            Lot lot = obj as Lot;
            if (lot == null)
            {
                BuildableShell shell = obj as BuildableShell;
                if (shell != null)
                {
                    lot = shell.LotCurrent;
                }
            }

            return lot;
        }

        public static int GetExportCount(SimDescription sim)
        {
            int count;
            if (sExportedSims.TryGetValue(sim.SimDescriptionId, out count))
            {
                return count;
            }
            else
            {
                return 0;
            }
        }

        public static void AddExport(SimDescription sim)
        {
            if (sExportedSims.ContainsKey(sim.SimDescriptionId))
            {
                sExportedSims[sim.SimDescriptionId]++;
            }
            else
            {
                sExportedSims.Add(sim.SimDescriptionId, 1);
            }
        }

        public void OnWorldLoadFinished()
        {
            sExportedSims.Clear();
        }

        public class PlaceGraveTask : Common.FunctionTask
        {
            SimDescription mSim;

            protected PlaceGraveTask(SimDescription sim)
            {
                mSim = sim;
            }

            public static void Perform(SimDescription sim)
            {
                new PlaceGraveTask(sim).AddToSimulator();
            }

            protected override void OnPerform()
            {
                Urnstones.CreateGrave(mSim, SimDescription.DeathType.OldAge, true, false);
            }
        }
    }
}
