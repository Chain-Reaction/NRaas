using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.DecensorSpace;
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
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.Pets;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Decensor : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Decensor()
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
            SimTask.Create();
            LotTask.Create();
        }

        protected static void VerifyCensor(Sim sim)
        {
            if (sim.mCensorGrid == null) return;

            if (sim.IsFemale)
            {
                if (Decensor.Settings.mCensorForFemales) return;
            }
            else
            {
                if (Decensor.Settings.mCensorForMales) return;
            }

            if (Decensor.Settings.mCensorByAge.Contains(sim.SimDescription.Age)) return;

            if (Decensor.Settings.mCensorBySpecies.Contains(sim.SimDescription.Species)) return;

            InteractionInstance interaction = sim.CurrentInteraction;
            if (interaction != null)
            {
                if (interaction.Target is IToiletOrUrinal)
                {
                    if (Decensor.Settings.mCensorOnToilet) return;
                }
                else if (interaction is Shower.WooHoo)
                {
                    if (Decensor.Settings.mCensorForShowerWoohoo) return;
                }
                else if ((interaction is PottyChair.PottyTrain) || (interaction is PottyChair.UsePottyChair))
                {
                    if (Decensor.Settings.mCensorOnPotty) return;
                }
                else if (interaction is BoxStall.WooHooInBoxStall)
                {
                    if (Decensor.Settings.mCensorForHorseWoohoo) return;
                }
            }

            sim.mCensorGrid.Stop(VisualEffect.TransitionType.HardTransition);
            sim.mCensorGrid.Dispose();
            sim.mCensorGrid = null;
        }

        public class SimTask : RepeatingTask
        {
            static SimTask sTask = null;

            static readonly int sDelay = 10;

            List<Sim> mSims;
            int mIndex;

            int mCount;

            protected SimTask()
            { }

            public static bool Running
            {
                get { return (sTask != null); }
            }

            protected override int Delay
            {
                get
                {
                    return sDelay;// Decensor.Settings.mDelay;
                }
            }

            public override void Dispose()
            {
                base.Dispose();

                sTask = null;
            }

            public static void Create()
            {
                if (sTask == null)
                {
                    sTask = new SimTask();
                    sTask.AddToSimulator();
                }
            }

            protected override bool OnPerform()
            {
                if (Decensor.Settings.mDisable) return true;

                if (mSims == null)
                {
                    Lot worldLot = LotManager.GetWorldLot();
                    if (worldLot == null) return true;

                    mSims = new List<Sim>(worldLot.GetAllActors());
                    mIndex = -1;

                    mCount = 0;
                }

                mCount++;
                mIndex++;
                if (mIndex >= mSims.Count)
                {
                    if (mCount > (Decensor.Settings.mDelay / sDelay))
                    {
                        mSims.Clear();
                        mSims = null;
                    }
                }
                else
                {
                    VerifyCensor(mSims[mIndex]);
                }

                return true;
            }
        }

        public class LotTask : RepeatingTask
        {
            static LotTask sTask = null;

            protected LotTask()
            { }

            public static bool Running
            {
                get { return (sTask != null); }
            }

            protected override int Delay
            {
                get
                {
                    return Decensor.Settings.mDelay;
                }
            }

            public override void Dispose()
            {
                base.Dispose();

                sTask = null;
            }

            public static void Create()
            {
                if (sTask == null)
                {
                    sTask = new LotTask();
                    sTask.AddToSimulator();
                }
            }

            protected override bool OnPerform()
            {
                if (Decensor.Settings.mDisable) return true;

                List<Lot> lots = new List<Lot>();

                Lot choice = LotManager.ActiveLot;
                if (choice != null)
                {
                    lots.Add(choice);
                }

                choice = LotManager.GetLotAtPoint(CameraController.GetLODInterestPosition());
                if ((choice != null) && (!lots.Contains(choice)))
                {
                    lots.Add(choice);
                }
                
                if (Household.ActiveHousehold != null)
                {
                    foreach (Sim sim in Households.AllSims(Household.ActiveHousehold))
                    {
                        VerifyCensor(sim);
                    }
                }
                
                foreach(Lot lot in lots)
                {
                    if (lot.IsWorldLot) continue;

                    foreach (Sim sim in lot.GetAllActors())
                    {
                        VerifyCensor(sim);
                    }
                }

                return true;
            }
        }
    }
}
