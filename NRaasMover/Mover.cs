using NRaas.CommonSpace.Tasks;
using NRaas.MoverSpace;
using NRaas.MoverSpace.Helpers;
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
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Mover : Common, Common.IPreLoad, Common.IWorldLoadFinished
    {
        static Common.MethodStore sStoryProgressionGetLotCost = new Common.MethodStore("NRaasStoryProgressionMoney", "NRaas.StoryProgressionModule", "GetLotCost", new Type[] { typeof(Lot) });
        static Common.MethodStore sStoryProgressionPresetLotHome = new Common.MethodStore("NRaasStoryProgressionMoney", "NRaas.StoryProgressionModule", "PresetRentalLotHome", new Type[] { typeof(Lot), typeof(Household) });

        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Mover()
        {
            Bootstrap();
        }
        public Mover()
        { }

        public void OnPreLoad()
        {
            MoverTask.Create<MoverTask>();
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;

            //new Common.ImmediateEventListener(EventTypeId.kSimDescriptionDisposed, OnDisposed);
        }
/*
        protected static void OnDisposed(Event e)
        {
            SimDescriptionEvent dEvent = e as SimDescriptionEvent;
            if (dEvent != null)
            {
                Common.StackLog(new Common.StringBuilder(dEvent.SimDescription.FullName));
            }
        }
*/
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

        public static int GetLotCost(Lot lot)
        {
            return GetLotCost(lot, true);
        }
        public static int GetLotCost(Lot lot, bool buyFurnished)
        {
            if (Settings.mFreeRealEstate) return 0;

            if ((BinModel.Singleton != null) && (BinModel.Singleton.FreeRealEstate)) return 0;

            if (lot.IsApartmentLot) return 0;

            if (sStoryProgressionGetLotCost.Valid)
            {
                int rentalCost = sStoryProgressionGetLotCost.Invoke<int>(new object[] { lot });

                if (rentalCost != lot.Cost)
                {
                    if (buyFurnished)
                    {
                        return rentalCost;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            
            if (buyFurnished)
            {
                return lot.Cost;
            }
            else
            {
                return lot.GetUnfurnishedCost();
            }
        }

        public static void PresetStoryProgressionLotHome(Lot lot, Household house)
        {
            sStoryProgressionPresetLotHome.Invoke<bool>(new object[] { lot, house });
        }

        protected class MoverTask : RepeatingTask
        {
            protected override bool OnPerform()
            {
                if (EditTownInfoPanel.sInstance != null)
                {
                    if (EditTownInfoPanel.sInstance.mActionButtons[0x0] != null)
                    {
                        EditTownInfoPanel.sInstance.mActionButtons[0x0].Click -= EditTownInfoPanel.sInstance.OnPlaceClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0x0].Click -= EditTownInfoPanelEx.OnPlaceClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0x0].Click += EditTownInfoPanelEx.OnPlaceClick;

                        EditTownInfoPanel.sInstance.mActionButtons[0x0].Enabled = true;
                    }

                    if (EditTownInfoPanel.sInstance.mActionButtons[0x2] != null)
                    {
                        EditTownInfoPanel.sInstance.mActionButtons[0x2].Click -= EditTownInfoPanel.sInstance.OnSplitClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0x2].Click -= EditTownInfoPanelEx.OnSplitClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0x2].Click += EditTownInfoPanelEx.OnSplitClick;
                    }

                    if (EditTownInfoPanel.sInstance.mActionButtons[0x5] != null)
                    {
                        EditTownInfoPanel.sInstance.mActionButtons[0x5].Click -= EditTownInfoPanel.sInstance.OnMergeClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0x5].Click -= EditTownInfoPanelEx.OnMergeClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0x5].Click += EditTownInfoPanelEx.OnMergeClick;
                    }

                    if (EditTownInfoPanel.sInstance.mActionButtons[0xa] != null)
                    {
                        EditTownInfoPanel.sInstance.mActionButtons[0xa].Click -= EditTownInfoPanel.sInstance.OnPlaceClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0xa].Click -= EditTownInfoPanelEx.OnPlaceClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0xa].Click += EditTownInfoPanelEx.OnPlaceClick;
                    }

                    if (EditTownInfoPanel.sInstance.mActionButtons[0xd] != null)
                    {
                        EditTownInfoPanel.sInstance.mActionButtons[0xd].Click -= EditTownInfoPanel.sInstance.OnSwitchToClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0xd].Click -= EditTownInfoPanelEx.OnSwitchToClick;
                        EditTownInfoPanel.sInstance.mActionButtons[0xd].Click += EditTownInfoPanelEx.OnSwitchToClick;
                    }
                }

                return true;
            }
        }
    }
}
