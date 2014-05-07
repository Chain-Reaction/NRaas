using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.PortraitPanelSpace.Interactions;
using NRaas.PortraitPanelSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.PortraitPanelSpace.Dialogs
{
    public class SkewerEx
    {
        public enum VisibilityType
        {
            ActiveHousehold,
            WholeTown,
            SelectedSims,
            ActiveSimLot,
            ActiveFamilyLot,
            ActiveHomeLot,
            ViewedLot,
            OnlyIdle,
            OnlySelectable,
            Relatives,
            ActiveHumans,
            ActiveAnimals,
            InactiveHumans,
            InactiveAnimals,
            SpeciesHumans,
            SpeciesDogs,
            SpeciesCats,
            SpeciesHorses,
            SpeciesOther,
        }

        public enum SortType
        {
            ByAge,
            ByMood,
            ByName,
            ByAutonomy,
            BySelectability,
            ByCustom,
            ActiveHousehold,
            BySpecies,
            ByRelationship,
        }

        Skewer.SkewerItem[] mHouseholdItems;

        List<SimInfo> mSimInfo = null;

        Dictionary<ObjectGuid, SimInfo> mSimLookup = null;

        Button mCyclePortraits;
        int[] mStartIterations = new int[4] { 0,0,0,0 };

        bool mUpdateNeeded = false;

        public static SkewerEx sInstance;

        SimInfo mPreviousInfo = null;

        public SkewerEx()
        {
            mHouseholdItems = new Skewer.SkewerItem[24];
        }

        public static void Unload()
        {
            if (sInstance != null)
            {
                sInstance.Dispose();
                sInstance = null;
            }
        }

        public void Dispose()
        {
            try
            {
                IHudModel hudModel = HudController.Instance.Model;

                hudModel.SimImageCache().ReleaseCacheReference();

                hudModel.SimChanged -= OnSimChanged;
                hudModel.SimAppearanceChanged -= OnSimAppearanceChanged;
                hudModel.HouseholdChanged -= OnHouseholdChanged;
                hudModel.SimMoodChanged -= OnMoodChanged;
                hudModel.SimMoodValueChanged -= OnMoodValueChanged;
                hudModel.SkewerNotificationChanged -= OnUpdateIconNotification;
                hudModel.SimLotChanged -= OnSimLotChanged;
                hudModel.SimAgeChanged -= OnSimAgeChanged;
                hudModel.SimNameChanged -= OnSimNameChanged;
                hudModel.SimRoomChanged -= OnSimRoomChanged;
                hudModel.RefreshCurrentSimInfoSkewer -= OnRefreshCurrentSimInfoSkewer;

                sInstance = null;
            }
            catch (Exception exception)
            {
                Common.Exception("Dispose", exception);
            }
        }

        public static void OnUpdate()
        {
            if (Instance == null) return;

            Instance.UpdateIfNeeded();
        }

        protected void UpdateIfNeeded()
        {
            if (!mUpdateNeeded) return;
            mUpdateNeeded = false;

            PopulateSkewers(false);
        }

        public static void Load()
        {
            if (sInstance == null)
            {
                sInstance = new SkewerEx();
                sInstance.Init();
            }
        }

        protected void Init()
        {
            try
            {
                sInstance = this;

                IHudModel hudModel = Skewer.Instance.mHudModel;

                hudModel.SimImageCache().AddCacheReference();

                for (uint i = 0x0; i < mHouseholdItems.Length; i++)
                {
                    mHouseholdItems[i].mContainer = Skewer.Instance.GetChildByID(0xf6fda571 + i, true) as Window;

                    if (i == 0)
                    {
                        mCyclePortraits = mHouseholdItems[i].mContainer.GetChildByID(0x10, false) as Button;
                        if (mCyclePortraits != null)
                        {
                            mCyclePortraits.MouseDown += OnCyclePortraits;
                        }
                    }

                    mHouseholdItems[i].mButton = mHouseholdItems[i].mContainer.GetChildByID(3, false) as Button;
                    mHouseholdItems[i].mButton.MouseDown += OnHouseholdSimButtonMouseDown;
                    mHouseholdItems[i].mButton.DragEnter += Skewer.Instance.OnHouseholdSimButtonDragEnter;
                    mHouseholdItems[i].mButton.DragDrop += Skewer.Instance.OnHouseholdSimButtonDragDrop;
                    mHouseholdItems[i].mGoButton = mHouseholdItems[i].mContainer.GetChildByID(4, false) as Button;
                    mHouseholdItems[i].mGoButton.Click += OnGoHome;
                    mHouseholdItems[i].mMood = mHouseholdItems[i].mContainer.GetChildByID(2, false) as Window;
                    mHouseholdItems[i].mThumb = mHouseholdItems[i].mContainer.GetChildByID(1, false) as Window;
                    mHouseholdItems[i].mGroupButton = mHouseholdItems[i].mContainer.GetChildByID(5, false) as Button;
                    mHouseholdItems[i].mDateButton = mHouseholdItems[i].mContainer.GetChildByID(6, false) as Button;
                    mHouseholdItems[i].mGroupButton.MouseUp += Skewer.Instance.OnGroupButtonMouseUpEvent;
                    mHouseholdItems[i].mDateButton.MouseUp += Skewer.Instance.OnGroupButtonMouseUpEvent;
                    mHouseholdItems[i].mFollowNotification = mHouseholdItems[i].mContainer.GetChildByID(7, false);
                    mHouseholdItems[i].mbShouldShowFollowNotify = false;
                    mHouseholdItems[i].mSimInfo = null;
                    mHouseholdItems[i].mEnsorcelButton = mHouseholdItems[i].mContainer.GetChildByID(10, false) as Button;
                }

                hudModel.SimChanged -= Skewer.Instance.OnSimChanged;
                hudModel.SimAppearanceChanged -= Skewer.Instance.OnSimAppearanceChanged;
                hudModel.HouseholdChanged -= Skewer.Instance.OnHouseholdChanged;
                hudModel.SimMoodChanged -= Skewer.Instance.OnMoodChanged;
                hudModel.SimMoodValueChanged -= Skewer.Instance.OnMoodValueChanged;
                hudModel.SkewerNotificationChanged -= Skewer.Instance.OnUpdateIconNotification;
                hudModel.SimLotChanged -= Skewer.Instance.OnSimLotChanged;
                hudModel.SimRoomChanged -= Skewer.Instance.OnSimRoomChanged;
                hudModel.SimAgeChanged -= Skewer.Instance.OnSimAgeChanged;
                hudModel.SimNameChanged -= Skewer.Instance.OnSimNameChanged;
                hudModel.RefreshCurrentSimInfoSkewer -= Skewer.Instance.OnRefreshCurrentSimInfoSkewer;

                hudModel.SimChanged += OnSimChanged;
                hudModel.SimAppearanceChanged += OnSimAppearanceChanged;
                hudModel.HouseholdChanged += OnHouseholdChanged;
                hudModel.SimMoodChanged += OnMoodChanged;
                hudModel.SimMoodValueChanged += OnMoodValueChanged;
                hudModel.SkewerNotificationChanged += OnUpdateIconNotification;
                hudModel.SimLotChanged += OnSimLotChanged;
                hudModel.SimRoomChanged += OnSimRoomChanged;
                hudModel.SimAgeChanged += OnSimAgeChanged;
                hudModel.SimNameChanged += OnSimNameChanged;
                hudModel.RefreshCurrentSimInfoSkewer += OnRefreshCurrentSimInfoSkewer;

                Common.FunctionTask.Perform(PopulateSkewers);
            }
            catch (Exception exception)
            {
                Common.Exception("Init", exception);
            }
        }

        protected static void Add(Dictionary<ulong, SimDescription> allSims, IEnumerable<SimDescription> sims)
        {
            foreach (SimDescription sim in sims)
            {
                if (allSims.ContainsKey(sim.SimDescriptionId)) continue;

                allSims.Add(sim.SimDescriptionId, sim);
            }
        }

        public static bool IsIdle(Sim createdSim)
        {
            if (createdSim == null) return true;

            if ((createdSim.InteractionQueue != null) &&
                (createdSim.InteractionQueue.GetCurrentInteraction() != null) &&
                (!createdSim.InteractionQueue.GetCurrentInteraction().Autonomous))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsVisible(SimDescription sim)
        {
            return IsVisible(sim, PortraitPanel.Settings.mVisibilityTypeV3, true, -1);
        }
        public static bool IsVisible(SimDescription sim, ICollection<VisibilityType> filters, bool trueOnEmpty, int column)
        {
            if (filters.Count == 0) return trueOnEmpty;

            Sim createdSim = sim.CreatedSim;

            if ((sim.IsNeverSelectable) || ((createdSim != null) && (createdSim.LotHome == null)))
            {
                if (filters.Contains(VisibilityType.OnlySelectable)) return false;
            }

            if (filters.Contains(VisibilityType.OnlyIdle))
            {
                if (!IsIdle(createdSim)) return false;

                trueOnEmpty = true;
            }

            bool found = false;
            foreach (VisibilityType type in filters)
            {
                switch (type)
                {
                    case VisibilityType.SpeciesHumans:
                        if (sim.IsHuman) return true;

                        found = true;
                        break;
                    case VisibilityType.SpeciesHorses:
                        if (sim.IsHorse) return true;

                        found = true;
                        break;
                    case VisibilityType.SpeciesCats:
                        if (sim.IsCat) return true;

                        found = true;
                        break;
                    case VisibilityType.SpeciesDogs:
                        if (sim.IsADogSpecies) return true;

                        found = true;
                        break;
                    case VisibilityType.SpeciesOther:
                        if ((sim.IsDeer) || (sim.IsRaccoon)) return true;

                        found = true;
                        break;
                    case VisibilityType.Relatives:
                        if (IsRelated(sim)) return true;

                        found = true;
                        break;
                    case VisibilityType.ActiveSimLot:
                        if ((Sim.ActiveActor == null) || (createdSim == null)) continue;

                        if (createdSim.LotCurrent == Sim.ActiveActor.LotCurrent) return true;

                        found = true;
                        break;
                    case VisibilityType.ActiveFamilyLot:
                        if ((Household.ActiveHousehold == null) || (createdSim == null)) continue;

                        foreach (Sim activeSim in Households.AllSims(Household.ActiveHousehold))
                        {
                            if (createdSim.LotCurrent == activeSim.LotCurrent) return true;
                        }

                        found = true;
                        break;
                    case VisibilityType.ActiveHomeLot:
                        if ((Sim.ActiveActor == null) || (createdSim == null)) continue;

                        if (createdSim.LotCurrent == null) continue;

                        if (createdSim.LotCurrent.CanSimTreatAsHome(Sim.ActiveActor)) return true;

                        found = true;
                        break;
                    case VisibilityType.ViewedLot:
                        Lot lot = LotManager.GetLotAtPoint(CameraController.GetLODInterestPosition());

                        if ((createdSim == null) || (lot == null) || (lot.IsWorldLot)) continue;

                        if (createdSim.LotCurrent == lot) return true;

                        found = true;
                        break;
                    case VisibilityType.ActiveAnimals:
                        if (sim.Household != Household.ActiveHousehold) continue;

                        if (!sim.IsHuman) return true;

                        found = true;
                        break;
                    case VisibilityType.ActiveHumans:
                        if (sim.Household != Household.ActiveHousehold) continue;

                        if (sim.IsHuman) return true;

                        found = true;
                        break;
                    case VisibilityType.InactiveAnimals:
                        if (sim.Household == Household.ActiveHousehold) continue;

                        if (!sim.IsHuman) return true;

                        found = true;
                        break;
                    case VisibilityType.InactiveHumans:
                        if (sim.Household == Household.ActiveHousehold) continue;

                        if (sim.IsHuman) return true;

                        found = true;
                        break;
                    case VisibilityType.SelectedSims:
                        int index = PortraitPanel.Settings.GetSimColumn(sim);

                        if ((index <= 0) || (column == index)) return true;

                        found = true;
                        break;
                    case VisibilityType.ActiveHousehold:
                        if (sim.Household == Household.ActiveHousehold)
                        {
                            return true;
                        }

                        found = true;
                        break;
                    case VisibilityType.WholeTown:
                        return true;
                }
            }

            if (trueOnEmpty)
            {
                return !found;
            }
            else
            {
                return false;
            }
        }

        public static bool IsRelated(SimDescription sim)
        {
            foreach (SimDescription active in Households.All(Household.ActiveHousehold))
            {
                if (Relationships.IsCloselyRelated(sim, active, false)) return true;
            }

            return false;
        }

        public static List<SimInfo> GetSimInfo()
        {
            Sims3.Gameplay.UI.HudModel hudModel = Skewer.Instance.mHudModel as Sims3.Gameplay.UI.HudModel;

            List<SimInfo> sims = new List<SimInfo>();

            Dictionary<ulong,SimDescription> choices = new Dictionary<ulong,SimDescription>();

            Dictionary<Lot,bool> lots = new Dictionary<Lot,bool>();

            foreach (VisibilityType type in PortraitPanel.Settings.mSetTypeV3)
            {
                switch (type)
                {
                    case VisibilityType.ActiveHousehold:
                        if (Household.ActiveHousehold != null)
                        {
                            Add(choices, Households.All(Household.ActiveHousehold));
                        }
                        break;
                    case VisibilityType.ActiveHumans:
                        if (Household.ActiveHousehold != null)
                        {
                            Add(choices, Households.Humans(Household.ActiveHousehold));
                        }
                        break;
                    case VisibilityType.ActiveAnimals:
                        if (Household.ActiveHousehold != null)
                        {
                            Add(choices, Households.Pets(Household.ActiveHousehold));
                        }
                        break;
                    case VisibilityType.SelectedSims:
                        Add(choices, PortraitPanel.Settings.SelectedSims);
                        break;
                    case VisibilityType.WholeTown:
                        Add(choices, Household.EverySimDescription());
                        break;
                    case VisibilityType.ActiveFamilyLot:
                        if (Household.ActiveHousehold != null)
                        {
                            foreach (Sim sim in Households.AllSims(Household.ActiveHousehold))
                            {
                                if (!lots.ContainsKey(sim.LotCurrent))
                                {
                                    lots.Add(sim.LotCurrent, true);
                                }
                            }
                        }
                        break;
                    case VisibilityType.ActiveSimLot:
                        if (Sim.ActiveActor != null)
                        {
                            Lot activeLot = Sim.ActiveActor.LotCurrent;

                            if (!lots.ContainsKey(activeLot))
                            {
                                lots.Add(activeLot, true);
                            }
                        }
                        break;
                    case VisibilityType.ActiveHomeLot:
                        if (Sim.ActiveActor != null)
                        {
                            Lot homeLot = Sim.ActiveActor.LotHome;

                            if (!lots.ContainsKey(homeLot))
                            {
                                lots.Add(homeLot, true);
                            }
                        }
                        break;
                    case VisibilityType.ViewedLot:
                        Lot viewedLot = LotManager.GetLotAtPoint(CameraController.GetLODInterestPosition());
                        if (viewedLot != null)
                        {
                            if (!lots.ContainsKey(viewedLot))
                            {
                                lots.Add(viewedLot, true);
                            }
                        }
                        break;
                }
            }

            foreach (Lot choiceLot in lots.Keys)
            {
                if (choiceLot.IsWorldLot) continue;

                foreach (Sim sim in choiceLot.GetAllActors())
                {
                    if (!choices.ContainsKey(sim.SimDescription.SimDescriptionId))
                    {
                        choices.Add(sim.SimDescription.SimDescriptionId, sim.SimDescription);
                    }
                }
            }

            SimDescription activeSim = null;
            if (Sim.ActiveActor != null)
            {
                activeSim = Sim.ActiveActor.SimDescription;

                if (!choices.ContainsKey(activeSim.SimDescriptionId))
                {
                    choices.Add(activeSim.SimDescriptionId, activeSim);
                }
            }

            foreach (SimDescription sim in choices.Values)
            {
                if (activeSim != sim)
                {
                    if (!IsVisible(sim)) continue;
                }

                try
                {
                    SimInfo info = hudModel.CreateSimInfo(sim);
                    if (info != null)
                    {
                        sims.Add(info);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }

            if ((sims.Count == 0) && (Sim.ActiveActor != null))
            {
                sims.Add(hudModel.CreateSimInfo(Sim.ActiveActor.SimDescription));
            }

            return sims;
        }

        private void OnGoHome(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                Sim sim = (Skewer.Instance.mHudModel as Sims3.Gameplay.UI.HudModel).mSavedCurrentSim;

                bool success = false;

                if (PortraitPanel.Settings.mGoHomeTeleport)
                {
                    if ((PortraitPanel.Settings.mGoHomeTeleportForAll) || (sim.SimDescription.IsVampire))
                    {
                        TerrainInteraction instance = new Terrain.TeleportMeHere.Definition(false).CreateInstance(Terrain.Singleton, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as TerrainInteraction;

                        Mailbox mailboxOnHomeLot = Mailbox.GetMailboxOnHomeLot(sim);
                        if (mailboxOnHomeLot != null)
                        {
                            Vector3 vector2;
                            World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(mailboxOnHomeLot.Position);
                            fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                            fglParams.InitialSearchDirection = RandomUtil.GetInt(0x0, 0x7);
                            if (GlobalFunctions.FindGoodLocation(sim, fglParams, out instance.Destination, out vector2))
                            {
                                success = sim.InteractionQueue.Add(instance);
                            }
                        }
                    }

                    if (!success)
                    {
                        Phone phone = sim.Inventory.Find<Phone>();
                        if (phone != null)
                        {
                            success = sim.InteractionQueue.Add(Phone.TeleportHome.Singleton.CreateInstance(phone, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
                        }
                    }
                }

                if (!success)
                {
                    if ((sim.LotHome == sim.LotCurrent) && 
                        (!sim.IsInPublicResidentialRoom) && 
                        (sim.InteractionQueue.Count > 0) && 
                        (Common.AssemblyCheck.IsInstalled("NRaasGoHere")))
                    {
                        sim.InteractionQueue.Add(GoHome.Singleton.CreateInstance(sim.LotHome, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
                    }
                    else
                    {
                        Skewer.Instance.mHudModel.PushGoHome();
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnGoHome", exception);
            }
        }

        public static void QueueChanged()
        {
            try
            {
                if (Instance == null) return;

                if (PortraitPanel.Settings.InUse(VisibilityType.OnlyIdle))
                {
                    Common.FunctionTask.Perform(Instance.PopulateSkewersNoRebuild);
                }
                else if (PortraitPanel.Settings.mSortType == SortType.ByAutonomy)
                {
                    Instance.mUpdateNeeded = true;
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnQueueChanged", e);
            }
        }

        public void OnCyclePortraits(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
                {
                    for (int i = 0; i < mStartIterations.Length; i++)
                    {
                        mStartIterations[i]++;
                    }

                    PopulateSkewers();
                }
                else
                {
                    Common.FunctionTask.Perform(OnShowListing);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCyclePortraits", e);
            }
        }

        public static void OnShowListing()
        {
            try
            {
                new InteractionOptionList<IPanelOption, Sim>.AllList(Common.Localize("Root:MenuName"), true).Perform(new GameHitParameters< Sim>(null, null, GameObjectHit.NoHit));
            }
            catch (Exception e)
            {
                Common.Exception("OnShowListing", e);
            }
        }

        public void OnHouseholdChanged(Sims3.UI.Hud.HouseholdEvent ev, ObjectGuid objectGuid)
        {
            Common.FunctionTask.Perform(PopulateSkewers);
        }

        public bool SelectSim(ObjectGuid objectGuid)
        {
            Sim actor = GameObject.GetObject<IActor>(objectGuid) as Sim;
            if (actor == null) return false;

            return DreamCatcher.Select(actor, PortraitPanel.Settings.mDreamCatcher, true);
        }

        protected static bool Matches(SimInfo left, SimInfo right)
        {
            if ((left == null) || (right == null)) return false;

            if (left.mGuid == right.mGuid) return true;

            if (left.UninstantiatedSimDescriptionId == 0) return false;

            return (left.UninstantiatedSimDescriptionId == right.UninstantiatedSimDescriptionId);
        }

        public void OnHouseholdSimButtonMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                if (Skewer.Instance == null) return;

                IHudModel hudModel = Skewer.Instance.mHudModel;
                if (hudModel == null) return;

                Button button = sender as Button;
                if ((button != null) && (button.Tag != null))
                {
                    Sims3.UI.Responder.Instance.TutorialModel.TriggerTutorial(TutorialTriggers.SimSkewerClicked);
                    SimInfo tag = (SimInfo)button.Tag;

                    if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
                    {
                        if ((eventArgs.Modifiers & (Modifiers.kModifierMaskControl|Modifiers.kModifierMaskShift)) != 0)
                        {
                            new ShowMenuTask(tag, false).AddToSimulator();
                        }
                        else
                        {
                            if ((mPreviousInfo != null) && (Matches(mPreviousInfo, tag)))
                            {
                                if (Skewer.Instance.mDoubleClickTimer.IsRunning() && (Skewer.Instance.mDoubleClickTimer.GetElapsedTimeFloat() < 0.5f))
                                {
                                    if (PortraitPanel.Settings.mMenuOnLeftClick)
                                    {
                                        new ShowMenuTask(tag, true).AddToSimulator();
                                    }
                                }
                            }

                            if ((Sim.ActiveActor != null) && (Sim.ActiveActor.ObjectId == tag.mGuid))
                            {
                                hudModel.MoveCameraToSim(tag.mGuid);
                            }
                            else if (!SelectSim(tag.mGuid))
                            {
                                if (hudModel.GetCurrentSimInfo() != null)
                                {
                                    if (Sims3.UI.Responder.Instance.PassportModel.IsSimRecallableFromPassport(tag))
                                    {
                                        hudModel.CheckPassportForRecall(tag);
                                    }

                                    OnSimChanged(hudModel.GetCurrentSimInfo().mGuid);
                                }
                            }

                            Skewer.Instance.mDoubleClickTimer.Restart();
                            mPreviousInfo = tag;
                        }

                        eventArgs.Handled = true;
                    }
                    else if (eventArgs.MouseKey == MouseKeys.kMouseRight)
                    {
                        bool following = false;

                        foreach(Skewer.SkewerItem item in mHouseholdItems)
                        {
                            if (Matches(item.mSimInfo, tag))
                            {
                                following = item.mbShouldShowFollowNotify;
                                break;
                            }
                        }

                        if ((following) || (PortraitPanel.Settings.mZoomInOnRightClick))
                        {
                            hudModel.AttachCameraToSim(tag.mGuid);
                        }
                        else
                        {
                            AttachCameraToSim(tag.mGuid);
                        }

                        if (!Matches(hudModel.GetCurrentSimInfo(), tag))
                        {
                            button.Selected = false;
                        }

                        eventArgs.Handled = true;
                    }
                    else if (eventArgs.MouseKey == MouseKeys.kMouseMiddle)
                    {
                        new ShowMenuTask(tag, false).AddToSimulator();
                        eventArgs.Handled = true;
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnHouseholdSimButtonMouseDown", exception);
            }
        }

        public void AttachCameraToSim(ObjectGuid objectGuid)
        {
            Sim actor = GameObject.GetObject<Sim>(objectGuid);
            if (actor != null)
            {
                Camera.FocusOnSim(actor);

                CameraController.EnableObjectFollow(actor.ObjectId.Value, Vector3.Empty);
            }
        }

        public void OnMoodChanged(SimInfo info)
        {
            try
            {
                if (PortraitPanel.Settings.mSortType == SortType.ByMood)
                {
                    mUpdateNeeded = true;
                }
                else
                {
                    UpdateForMoodChange(ref mHouseholdItems, info, true);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(info.mName, exception);
            }
        }

        public void OnMoodValueChanged(SimInfo info)
        {
            try
            {
                if (PortraitPanel.Settings.mSortType == SortType.ByMood)
                {
                    mUpdateNeeded = true;
                }
                else
                {
                    Skewer.Instance.UpdateForMoodValueChange(ref mHouseholdItems, info);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(info.mName, exception);
            }
        }

        private void OnSimRoomChanged()
        {
            try
            {
                UpdateForMoodChange(ref mHouseholdItems, Skewer.Instance.mHudModel.GetCurrentSimInfo(), false);
            }
            catch (Exception exception)
            {
                Common.Exception("OnSimRoomChanged", exception);
            }
        }

        public void OnSimAgeChanged(ObjectGuid objGuid)
        {
            Common.FunctionTask.Perform(PopulateSkewers);
        }

        public void OnSimAppearanceChanged(ObjectGuid objectGuid)
        {
            try
            {
                Skewer.SkewerItem item = new Skewer.SkewerItem();
                bool flag = false;
                foreach (Skewer.SkewerItem item2 in mHouseholdItems)
                {
                    if ((item2.mSimInfo != null) && (item2.mSimInfo.mGuid == objectGuid))
                    {
                        flag = true;
                        item = item2;
                        break;
                    }
                }
                if (flag)
                {
                    UpdateButton(ref item, item.mSimInfo, true, true);
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnSimAppearanceChanged", exception);
            }
        }

        public void OnRefreshCurrentSimInfoSkewer()
        {
            try
            {
                UpdateForMoodChange(ref mHouseholdItems, Skewer.Instance.mHudModel.GetCurrentSimInfo(), false);
            }
            catch (Exception exception)
            {
                Common.Exception("OnRefreshCurrentSimInfoSkewer", exception);
            }
        }

        public void OnSimNameChanged(ObjectGuid objGuid)
        {
            try
            {
                Skewer.SkewerItem item = new Skewer.SkewerItem();
                bool flag = false;
                foreach (Skewer.SkewerItem item2 in mHouseholdItems)
                {
                    if ((item2.mSimInfo != null) && (item2.mSimInfo.mGuid == objGuid))
                    {
                        flag = true;
                        item = item2;
                        break;
                    }
                }
                if (flag)
                {
                    UpdateButton(ref item, item.mSimInfo, false, true);
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnSimNameChanged", exception);
            }
        }

        public void UpdateButton(ref Skewer.SkewerItem item, SimInfo info, bool bUpdateThumbnail, bool bUseCurrentThumbAsPlaceholder)
        {
            try
            {
                Skewer.Instance.UpdateButton(ref item, info, bUpdateThumbnail, bUseCurrentThumbAsPlaceholder);

                if (item.mGoButton != null)
                {
                    item.mGoButton.Visible = ShouldShowGoHomeButtonForSim(item.mSimInfo.mGuid);
                }
            }
            catch (Exception e)
            {
                Common.Exception(info.mName, e);
            }
        }

        public static bool ShouldShowGoHomeButtonForSim(ObjectGuid simGuid)
        {
            Sims3.Gameplay.UI.HudModel model = HudController.Instance.Model as Sims3.Gameplay.UI.HudModel;

            Sim sim = GameObject.GetObject<Sim>(simGuid);
            if ((sim == null) || (sim != model.mSavedCurrentSim))
            {
                return false;
            }
            if (!sim.SimDescription.ChildOrAbove)
            {
                return false;
            }
            else if (sim.IsAtHome && !sim.IsInPublicResidentialRoom)
            {
                if (!Common.kDebugging) return false;

                return Common.AssemblyCheck.IsInstalled("NRaasGoHere");
            }
            else
            {
                return true;
            }
        }

        public void OnSimChanged(ObjectGuid objectGuid)
        {
            try
            {
                bool flag = false;
                foreach (Skewer.SkewerItem item in mHouseholdItems)
                {
                    if ((item.mSimInfo != null) && (item.mSimInfo.mGuid == objectGuid))
                    {
                        item.mButton.Selected = true;
                        if (item.mGoButton != null)
                        {
                            item.mGoButton.Visible = ShouldShowGoHomeButtonForSim(item.mSimInfo.mGuid);
                        }

                        Skewer.Instance.UpdateMoodWindow(item.mMood, item.mSimInfo);
                        item.mFollowNotification.Visible = item.mbShouldShowFollowNotify;
                        flag = true;
                    }
                    else
                    {
                        item.mButton.Selected = false;

                        if (item.mGoButton != null)
                        {
                            item.mGoButton.Visible = false;
                        }

                        item.mButton.Highlighted = false;
                        item.mFollowNotification.Visible = item.mbShouldShowFollowNotify;
                    }
                }

                if (!flag)
                {
                    Common.FunctionTask.Perform(PopulateSkewers);
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnSimChanged", exception);
            }
        }

        public void OnSimLotChanged(ulong lotID, bool isHome)
        {
            try
            {
                OnSimRoomChanged();
            }
            catch (Exception exception)
            {
                Common.Exception("OnSimLotChanged", exception);
            }
        }

        public void OnUpdateIconNotification(SkewerNotificationType type, ObjectGuid simGuid, bool bShow)
        {
            try
            {
                for (int i = 0x0; i < mHouseholdItems.Length; i++)
                {
                    if ((mHouseholdItems[i].mSimInfo != null) && (mHouseholdItems[i].mSimInfo.mGuid == simGuid))
                    {
                        switch (type)
                        {
                            case SkewerNotificationType.GroupingNotfication:
                                mHouseholdItems[i].mGroupButton.Visible = bShow;
                                mHouseholdItems[i].mDateButton.Visible = false;
                                break;

                            case SkewerNotificationType.DatingNotification:
                                mHouseholdItems[i].mDateButton.Visible = bShow;
                                mHouseholdItems[i].mGroupButton.Visible = false;
                                break;

                            case SkewerNotificationType.FollowNotification:
                                mHouseholdItems[i].mbShouldShowFollowNotify = bShow;
                                mHouseholdItems[i].mFollowNotification.Visible = bShow;
                                break;

                            case SkewerNotificationType.EnsorcelNotification:
                                mHouseholdItems[i].mEnsorcelButton.Visible = bShow;
                                mHouseholdItems[i].mGroupButton.Visible = bShow;
                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnUpdateIconNotification", exception);
            }
        }

        public static void HideSkewer(ref Skewer.SkewerItem[] skewerItems)
        {
            for (int i = 0x0; i < skewerItems.Length; i++)
            {
                skewerItems[i].mButton.Tag = null;
                skewerItems[i].mButton.CreateTooltipCallbackFunction = CreateSimTooltip;
                skewerItems[i].mSimInfo = null;
                skewerItems[i].mContainer.Visible = false;
                skewerItems[i].mbShouldShowFollowNotify = false;
                skewerItems[i].mFollowNotification.Visible = false;
                if (skewerItems[i].mGroupButton != null)
                {
                    skewerItems[i].mGroupButton.Visible = false;
                }

                if (skewerItems[i].mDateButton != null)
                {
                    skewerItems[i].mDateButton.Visible = false;
                }

                if (skewerItems[i].mGoButton != null)
                {
                    skewerItems[i].mGoButton.Visible = false;
                }
            }
        }

        private static Tooltip CreateSimTooltip(Vector2 mousePosition, WindowBase parent, ref Vector2 tooltipPosition)
        {
            try
            {
                if (Sims3.Gameplay.UI.Responder.Instance == null) return null;

                IHudModel hudModel = Sims3.Gameplay.UI.Responder.Instance.HudModel;
                if (hudModel == null) return null;

                SimInfo tag = parent.Tag as SimInfo;

                if (PortraitPanel.Settings.mShowKnownInfo)
                {
                    SimDescription sim = PortraitPanel.Settings.GetSim(tag);

                    if ((sim != null) && (Sim.ActiveActor != null))
                    {
                        Tooltip result = new KnownInfoTooltip(sim.FullName, hudModel.GetLTRRelationshipString(Sim.ActiveActor.SimDescription, sim), sim.HomeWorld, HudModelEx.GetKnownInfo(hudModel as HudModel, sim));

                        tooltipPosition.x = parent.WindowToScreen(new Vector2(0,0)).x + parent.Area.Width;

                        Window firstPortrait = Skewer.Instance.GetChildByID(0xf6fda571, true) as Window;

                        tooltipPosition.y = firstPortrait.WindowToScreen(new Vector2(0, 0)).y - result.TooltipWindow.Area.Height;

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }

                float offsetX = -5f;
                float offsetY = -10f;

                if (GameUtils.IsInstalled(ProductVersion.EP9))
                {
                    int nerdLevel = -1;
                    int rebelLevel = -1;
                    int socialiteLevel = -1;
                    hudModel.GetCurrentSimInfluence(tag, ref nerdLevel, ref rebelLevel, ref socialiteLevel);
                    return InfluentialCelebrityTooltip.CreateTooltipWithOffset(parent, ref tooltipPosition, tag, nerdLevel, rebelLevel, socialiteLevel, offsetX, offsetY);
                }

                return CelebrityTooltip.CreateTooltipWithOffset(parent, ref tooltipPosition, tag, offsetX, offsetY);
            }
            catch (Exception e)
            {
                Common.Exception("CreateKnownInfoToolTip", e);
            }
            return null;
        }

        public void PopulateSkewer(Window skewer, ref Skewer.SkewerItem[] skewerItems, float baseSize, float deltaSize)
        {
            Common.StringBuilder msg = new Common.StringBuilder("PopulateSkewer");

            try
            {
                if (mCyclePortraits != null)
                {
                    mCyclePortraits.Visible = ((PortraitPanel.Settings.mShowCycleButton) || (!PortraitPanel.Settings.mShowSimMenu) || (mSimInfo.Count > 24));
                }

                if (Skewer.Instance.mPetsSkewer != null)
                {
                    Skewer.Instance.mPetsSkewer.Visible = false;
                }

                HideSkewer(ref Skewer.Instance.mHouseholdItems);
                HideSkewer(ref Skewer.Instance.mPetItems);
                HideSkewer(ref skewerItems);

                float num = baseSize + (deltaSize * 8f);

                msg += Common.NewLine + num;

                skewer.Visible = false;
                if ((mSimInfo != null) && (mSimInfo.Count > 0))
                {
                    Sorter sorter = new Sorter(0);

                    Sorter.SorterFunc sort = sorter.GetSort();
                    if (sort != null)
                    {
                        mSimInfo.Sort(new Comparison<SimInfo>(sort));
                    }

                    int maximumPortraits = 24;
                    int maximumColumn3 = 8;
                    if (!PortraitPanel.Settings.mUsePortraitSeventeen)
                    {
                        maximumPortraits--;
                        maximumColumn3--;
                    }

                    int count = mSimInfo.Count;
                    if (count > 0x0)
                    {
                        msg += Common.NewLine + count;

                        bool exceeded = false;

                        List<SimInfo> column1 = new List<SimInfo>();
                        List<SimInfo> column2 = new List<SimInfo>();
                        List<SimInfo> column3 = new List<SimInfo>();

                        if ((mSimInfo.Count > maximumPortraits) && (PortraitPanel.Settings.mRevertToSingleListOnTooMany))
                        {
                            exceeded = true;
                        }
                        else
                        {
                            foreach (SimInfo info in mSimInfo)
                            {
                                SimDescription sim = PortraitPanel.Settings.GetSim(info);
                                if (sim == null) continue;

                                List<List<SimInfo>> choices = new List<List<SimInfo>>();
                                List<int> maximums = new List<int>();

                                if (IsVisible(sim, PortraitPanel.Settings.mColumnFilter1, false, 1))
                                {
                                    msg += Common.NewLine + "IsVisible 1";

                                    choices.Add(column1);
                                    maximums.Add(8);
                                }
                                
                                if (IsVisible(sim, PortraitPanel.Settings.mColumnFilter2, false, 2))
                                {
                                    msg += Common.NewLine + "IsVisible 2";

                                    choices.Add(column2);
                                    maximums.Add(8);
                                }
                                
                                if (IsVisible(sim, PortraitPanel.Settings.mColumnFilter3, false, 3))
                                {
                                    msg += Common.NewLine + "IsVisible 3";

                                    choices.Add(column3);
                                    maximums.Add(maximumColumn3);
                                }
                                
                                if ((choices.Count == 0) && (PortraitPanel.Settings.mRevertToSingleListOnFilterFail))
                                {
                                    msg += Common.NewLine + "Exceeded 1";

                                    exceeded = true;
                                }

                                bool placed = false;
                                for (int i = 0; i < choices.Count; i++)
                                {
                                    if (choices[i].Count < maximums[i])
                                    {
                                        choices[i].Add(info);
                                        placed = true;
                                        break;
                                    }
                                }

                                if (!placed)
                                {
                                    if (PortraitPanel.Settings.mRevertToSingleListOnFilterFail)
                                    {
                                        msg += Common.NewLine + "Exceeded 2";

                                        exceeded = true;
                                    }
                                    else if (choices.Count > 0)
                                    {
                                        choices[0].Add(info);
                                    }
                                }
                            }
                        }

                        if ((column1.Count + column2.Count + column3.Count) == 0)
                        {
                            msg += Common.NewLine + "Exceeded 3";

                            exceeded = true;
                        }

                        float num4 = baseSize;
                        if (exceeded)
                        {
                            Populate(ref skewerItems, 0, mSimInfo, 0, deltaSize, ref num4);
                        }
                        else
                        {
                            Populate(ref skewerItems, 0, column1, 1, deltaSize, ref num4);
                            Populate(ref skewerItems, 8, column2, 2, deltaSize, ref num4);
                            Populate(ref skewerItems, 16, column3, 3, deltaSize, ref num4);
                        }

                        Rect area = skewer.Area;
                        area.Set(area.TopLeft.x, num - num4, area.BottomRight.x, area.BottomRight.y);
                        skewer.Area = area;
                        skewer.Visible = true;
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(msg, exception);
            }
            finally
            {
                //Common.DebugWriteLog(msg);
            }
        }

        protected void Populate(ref Skewer.SkewerItem[] skewerItems, int skewerStart, List<SimInfo> simInfo, int iterationIndex, float deltaSize, ref float size)
        {
            int startIndex = mStartIterations[iterationIndex];

            int count = 0;

            if (iterationIndex == 0)
            {
                if (!PortraitPanel.Settings.mUsePortraitSeventeen)
                {
                    startIndex *= 23;
                    count = 23;
                }
                else
                {
                    startIndex *= 24;
                    count = 24;
                }
            }
            else if ((iterationIndex == 3) && (!PortraitPanel.Settings.mUsePortraitSeventeen))
            {
                startIndex *= 7;
                count = 7;
            }
            else
            {
                startIndex *= 8;
                count = 8;
            }

            if (startIndex >= simInfo.Count)
            {
                startIndex = 0;

                mStartIterations[iterationIndex] = 0;
            }

            if (count > (simInfo.Count - startIndex))
            {
                count = (simInfo.Count - startIndex);
            }

            for (int j = 0; j < count; j++)
            {
                int simIndex = j + startIndex;
                //if (simIndex >= simInfo.Count) break;

                int skewerIndex = j + skewerStart;
                //if (skewerIndex >= skewerItems.Length) break;

                if (skewerIndex == 16)
                {
                    if (!PortraitPanel.Settings.mUsePortraitSeventeen)
                    {
                        skewerStart++;
                        skewerIndex++;
                    }
                }

                if (simInfo[simIndex] == null) continue;

                if (skewerItems[skewerIndex].mButton != null)
                {
                    skewerItems[skewerIndex].mButton.Tag = simInfo[simIndex];
                }

                skewerItems[skewerIndex].mSimInfo = simInfo[simIndex];
                UpdateButton(ref skewerItems[skewerIndex], simInfo[simIndex], true, false);
                Skewer.Instance.UpdateMoodWindow(skewerItems[skewerIndex].mMood, simInfo[simIndex]);

                size += deltaSize;
            }
        }

        public void PopulateSkewers()
        {
            PopulateSkewers(true);
        }
        public void PopulateSkewersNoRebuild()
        {
            PopulateSkewers(false);
        }

        private void PopulateSkewers(bool rebuildList)
        {
            try
            {
                //using (Common.TestSpan span = new Common.TestSpan(PortraitPanel.Logger.sLogger, "PopulateSkewers"))
                {
                    if (Skewer.Instance == null) return;

                    if ((rebuildList) || (mSimInfo == null))
                    {
                        if (mSimInfo != null)
                        {
                            Sims3.Gameplay.UI.HudModel hudModel = Skewer.Instance.mHudModel as Sims3.Gameplay.UI.HudModel;

                            foreach (SimInfo info in mSimInfo)
                            {
                                if (info == null) continue;

                                hudModel.RemoveSimInfo(info);
                            }

                            mSimInfo.Clear();
                        }

                        mSimInfo = GetSimInfo();

                        mSimLookup = new Dictionary<ObjectGuid, SimInfo>();

                        foreach(SimInfo info in mSimInfo)
                        {
                            if (info == null) continue;

                            mSimLookup[info.mGuid] = info;
                        }
                    }

                    PopulateSkewer(Skewer.Instance.mHouseholdSkewer, ref mHouseholdItems, 118f, 60f);
                }
            }
            catch (Exception exception)
            {
                Common.Exception("PopulateSkewer", exception);
            }
        }

        public bool IsSimListed(SimDescription sim)
        {
            if (mSimLookup == null) return false;

            if (sim == null) return false;

            if (sim.CreatedSim == null) return false;

            if (sim.CreatedSim.ObjectId == null) return false;

            return mSimLookup.ContainsKey(sim.CreatedSim.ObjectId);
        }

        public bool UpdateForMoodChange(ref Skewer.SkewerItem[] skewerItems, SimInfo simInfo, bool bUpdateThumbnails)
        {
            if (simInfo != null)
            {
                for (int i = 0x0; i < skewerItems.Length; i++)
                {
                    if (Matches(skewerItems[i].mSimInfo, simInfo))
                    {
                        UpdateButton(ref skewerItems[i], simInfo, bUpdateThumbnails, true);

                        return true;
                    }
                }
            }
            return false;
        }

        public static SkewerEx Instance
        {
            get
            {
                return sInstance;
            }
        }

        public class ShowMenuTask : Common.FunctionTask
        {
            SimInfo mTag;

            bool mForceListing;

            public ShowMenuTask(SimInfo tag, bool forceListing)
            {
                mTag = tag;
                mForceListing = forceListing;
            }

            protected override void OnPerform()
            {
                Sims3.Gameplay.UI.PieMenu.ClearInteractions();
                Sims3.Gameplay.UI.PieMenu.HidePieMenuSimHead = true;

                Sim activeActor = Sim.ActiveActor;
                if (activeActor != null)
                {
                    Sim sim = Sim.GetObject(mTag.mGuid) as Sim;
                    if (sim != null)
                    {
                        bool popupStyle = false;

                        if (!mForceListing)
                        {
                            popupStyle = VersionStamp.sPopupMenuStyle;
                        }

                        if (activeActor.InteractionQueue.CanPlayerQueue())
                        {
                            bool success = false;
                            try
                            {
                                List<InteractionObjectPair> interactions = new List<InteractionObjectPair>();
                                interactions.Add(new InteractionObjectPair(CallOver.Singleton, sim));

                                List<InteractionObjectPair> others = sim.GetAllInteractionsForActor(activeActor);
                                if (others != null)
                                {
                                    interactions.AddRange(others);
                                }

                                InteractionDefinitionOptionList.Perform(activeActor, new GameObjectHit(GameObjectHitType.Object), interactions, popupStyle);
                                success = true;
                            }
                            catch (Exception e)
                            {
                                Common.Exception(activeActor, sim, e);
                            }

                            if (!success)
                            {
                                InteractionDefinitionOptionList.Perform(activeActor, new GameObjectHit(GameObjectHitType.Object), InteractionsEx.GetImmediateInteractions(sim), popupStyle);
                            }
                        }
                    }
                }
            }
        }

        public class Sorter
        {
            SortType mType;

            int mOrder;

            bool mReverse;

            public Sorter(int order)
            {
                mOrder = order;

                if (mOrder == 1)
                {
                    mType = PortraitPanel.Settings.mSecondSortType;
                    mReverse = PortraitPanel.Settings.mSecondReverse;
                }
                else if (mOrder == 2)
                {
                    mType = PortraitPanel.Settings.mThirdSortType;
                    mReverse = PortraitPanel.Settings.mThirdReverse;
                }
                else
                {
                    mType = PortraitPanel.Settings.mSortType;
                    mReverse = PortraitPanel.Settings.mReverse;
                }
            }

            public delegate int SorterFunc(SimInfo left, SimInfo right);

            public SorterFunc GetSort()
            {
                switch (mType)
                {
                    case SortType.ByMood:
                        return OnSortByMood;
                    case SortType.ByName:
                        return OnSortByName;
                    case SortType.ByAge:
                        return OnSortByAge;
                    case SortType.ByAutonomy:
                        return OnSortByAutonomy;
                    case SortType.BySelectability:
                        return OnSortBySelectability;
                    case SortType.ByCustom:
                        return OnSortByCustom;
                    case SortType.ActiveHousehold:
                        return OnSortActiveHousehold;
                    case SortType.BySpecies:
                        return OnSortBySpecies;
                    case SortType.ByRelationship:
                        return OnSortByRelationship;
                }

                return null;
            }

            public int HandleReverse(int result)
            {
                if (mReverse)
                {
                    return -result;
                }
                else
                {
                    return result;
                }
            }

            public static int GetSpeciesOrder(CASAgeGenderFlags species)
            {
                switch (species)
                {
                    case CASAgeGenderFlags.Human:
                        return 4;
                    case CASAgeGenderFlags.Horse:
                        return 3;
                    case CASAgeGenderFlags.Dog:
                    case CASAgeGenderFlags.LittleDog:
                        return 2;
                    case CASAgeGenderFlags.Cat:
                        return 1;
                    default:
                        return 0;
                }
            }

            public int OnSortBySpecies(SimInfo left, SimInfo right)
            {
                try
                {
                    SimDescription leftSim = PortraitPanel.Settings.GetSim(left);
                    if (leftSim == null) return -1;

                    SimDescription rightSim = PortraitPanel.Settings.GetSim(right);
                    if (rightSim == null) return 1;

                    int result = HandleReverse(GetSpeciesOrder(leftSim.Species).CompareTo(GetSpeciesOrder(rightSim.Species)));

                    if (result == 0)
                    {
                        return NextSort(left, right);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(left.mName + " - " + right.mName, e);
                    return 0;
                }
            }

            public int OnSortByRelationship(SimInfo left, SimInfo right)
            {
                try
                {
                    SimDescription active = Sim.ActiveActor.SimDescription;
                    if (Sim.ActiveActor == null) return 0;

                    SimDescription leftSim = PortraitPanel.Settings.GetSim(left);
                    if (leftSim == null) return -1;

                    SimDescription rightSim = PortraitPanel.Settings.GetSim(right);
                    if (rightSim == null) return 1;

                    float leftLiking = float.MinValue;

                    if (active == leftSim)
                    {
                        leftLiking = float.MaxValue;
                    }
                    else
                    {
                        Relationship leftRelation = Relationship.Get(active, leftSim, false);
                        if (leftRelation != null)
                        {
                            leftLiking = leftRelation.CurrentLTRLiking;
                        }
                    }

                    float rightLiking = float.MinValue;

                    if (active == rightSim)
                    {
                        rightLiking = float.MaxValue;
                    }
                    else
                    {
                        Relationship rightRelation = Relationship.Get(active, rightSim, false);
                        if (rightRelation != null)
                        {
                            rightLiking = rightRelation.CurrentLTRLiking;
                        }
                    }

                    int result = HandleReverse(leftLiking.CompareTo(rightLiking));

                    if (result == 0)
                    {
                        return NextSort(left, right);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(left.mName + " - " + right.mName, e);
                    return 0;
                }
            }

            public int OnSortActiveHousehold(SimInfo left, SimInfo right)
            {
                try
                {
                    SimDescription leftSim = PortraitPanel.Settings.GetSim(left);
                    if (leftSim == null) return -1;

                    SimDescription rightSim = PortraitPanel.Settings.GetSim(right);
                    if (rightSim == null) return 1;

                    // Intentionally negated to push active sims to the bottom of the list
                    int result = -HandleReverse((leftSim.Household == Household.ActiveHousehold).CompareTo(rightSim.Household == Household.ActiveHousehold));

                    if (result == 0)
                    {
                        return NextSort(left, right);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(left.mName + " - " + right.mName, e);
                    return 0;
                }
            }

            public int OnSortByMood(SimInfo left, SimInfo right)
            {
                try
                {
                    int result = HandleReverse(left.mMoodValue.CompareTo(right.mMoodValue));

                    if (result == 0)
                    {
                        return NextSort(left, right);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(left.mName + " - " + right.mName, e);
                    return 0;
                }
            }

            public int OnSortByCustom(SimInfo left, SimInfo right)
            {
                try
                {
                    SimDescription leftSim = PortraitPanel.Settings.GetSim(left);
                    if (leftSim == null) return -1;

                    SimDescription rightSim = PortraitPanel.Settings.GetSim(right);
                    if (rightSim == null) return 1;

                    int result = HandleReverse(PortraitPanel.Settings.GetSimSort(leftSim).CompareTo(PortraitPanel.Settings.GetSimSort(rightSim)));

                    if (result == 0)
                    {
                        return NextSort(left, right);
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Common.Exception(left.mName + " - " + right.mName, e);
                    return 0;
                }
            }

            protected static int CompareName(string left, string right)
            {
                if (left == null) return 1;

                if (right == null) return -1;

                return left.CompareTo(right);
            }

            public int OnSortByName(SimInfo left, SimInfo right)
            {
                try
                {
                    SimDescription leftSim = PortraitPanel.Settings.GetSim(left);
                    if (leftSim == null) return -1;

                    SimDescription rightSim = PortraitPanel.Settings.GetSim(right);
                    if (rightSim == null) return 1;

                    int result = HandleReverse(CompareName(leftSim.LastName, rightSim.LastName));

                    if (result == 0)
                    {
                        result = HandleReverse(CompareName(leftSim.FirstName, rightSim.FirstName));

                        if (result == 0)
                        {
                            return NextSort(left, right);
                        }
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Common.Exception(left.mName + " - " + right.mName, e);
                    return 0;
                }
            }

            public int OnSortByAge(SimInfo left, SimInfo right)
            {
                try
                {
                    SimDescription leftSim = PortraitPanel.Settings.GetSim(left);
                    if (leftSim == null) return -1;

                    SimDescription rightSim = PortraitPanel.Settings.GetSim(right);
                    if (rightSim == null) return 1;

                    int result = HandleReverse(leftSim.Age.CompareTo(rightSim.Age));

                    if (result == 0)
                    {
                        result = HandleReverse(leftSim.AgingYearsSinceLastAgeTransition.CompareTo(rightSim.AgingYearsSinceLastAgeTransition));

                        if (result == 0)
                        {
                            result = NextSort(left, right);
                            if (result == 0)
                            {
                                return leftSim.FullName.CompareTo(rightSim.FullName);
                            }
                        }
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Common.Exception(left.mName + " - " + right.mName, e);
                    return 0;
                }
            }

            public int OnSortByAutonomy(SimInfo left, SimInfo right)
            {
                try
                {
                    SimDescription leftSim = PortraitPanel.Settings.GetSim(left);
                    if (leftSim == null) return -1;

                    SimDescription rightSim = PortraitPanel.Settings.GetSim(right);
                    if (rightSim == null) return 1;

                    int result = HandleReverse(IsIdle(leftSim.CreatedSim).CompareTo(IsIdle(rightSim.CreatedSim)));

                    if (result == 0)
                    {
                        return NextSort(left, right);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(left.mName + " - " + right.mName, e);
                    return 0;
                }
            }

            public int OnSortBySelectability(SimInfo left, SimInfo right)
            {
                try
                {
                    SimDescription leftSim = PortraitPanel.Settings.GetSim(left);
                    if (leftSim == null) return -1;

                    SimDescription rightSim = PortraitPanel.Settings.GetSim(right);
                    if (rightSim == null) return 1;

                    int result = HandleReverse(leftSim.IsNeverSelectable.CompareTo(rightSim.IsNeverSelectable));

                    if (result == 0)
                    {
                        return NextSort(left, right);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(left.mName + " - " + right.mName, e);
                    return 0;
                }
            }

            protected int NextSort(SimInfo left, SimInfo right)
            {
                if (mOrder >= 2) return 0;

                Sorter sorter = new Sorter(mOrder + 1);

                SorterFunc sort = sorter.GetSort();
                if (sort == null) return 0;

                return sort(left, right);
            }
        }
    }
}
