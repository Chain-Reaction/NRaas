using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using Sims3.UI.HUD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Proxies
{
    public class HudModelProxy : IHudModel
    {
        IHudModel mHudModel;

        public HudModelProxy(IHudModel hudModel)
        {
            mHudModel = hudModel;

            mHudModel.AutonomousInteractionCancelledFromUI += OnAutonomousInteractionCancelledFromUIProxy;
            mHudModel.CareerPerformanceChanged += OnCareerPerformanceChangedProxy;
            mHudModel.CareerUpdated += OnCareerUpdatedProxy;
            mHudModel.CompetitionStandingEvent += OnCompetitionStandingEventProxy;
            mHudModel.CurrentSimInventoryOwnerChanged += OnCurrentSimInventoryOwnerChangedProxy;
            mHudModel.HouseholdAncientCoinCountChanged += OnHouseholdAncientCoinCountChangedProxy;
            mHudModel.HideCompetitionPanelEvent += OnHideCompetitionPanelEventProxy;
            mHudModel.HouseholdChanged += OnHouseholdChangedProxy;
            mHudModel.HouseholdFundsChanged += OnHouseholdFundsChangedProxy;
            mHudModel.InteractionQueueDirtied += OnInteractionQueueDirtiedProxy;
            mHudModel.JobTrackingUpdated += OnJobTrackingUpdatedProxy;
            mHudModel.LifeEventUpdate += OnLifeEventUpdateProxy;
            mHudModel.LifetimePointsChanged += OnLifetimePointsChangedProxy;
            mHudModel.LotChanged += OnLotChangedProxy;
            mHudModel.MinuteChanged += OnMinuteChangedProxy;
            mHudModel.OccultUpdated += OnOccultUpdatedProxy;
            mHudModel.PostureChanged += OnPostureChangedProxy;
            mHudModel.RelationshipsChanged += OnRelationshipsChangedProxy;
            mHudModel.RewardTraitsChanged += OnRewardTraitsChangedProxy;
            mHudModel.SecondaryInventoryOwnerChanged += OnSecondaryInventoryOwnerChangedProxy;
            mHudModel.ShowCompetitionPanelEvent += OnShowCompetitionPanelEventProxy;
            mHudModel.SimAgeChanged += OnSimAgeChangedProxy;
            mHudModel.SimAppearanceChanged += OnSimAppearanceChangedProxy;
            mHudModel.SimBuffsChanged += OnSimBuffsChangedProxy;
            mHudModel.SimCelebrityInfoChanged += OnSimCelebrityInfoChangedProxy;
            mHudModel.SimChanged += OnSimChangedProxy;
            mHudModel.SimCurrentWorldChanged += OnSimCurrentWorldChangedProxy;
            mHudModel.SimDaysPerAgingYearChanged += OnSimDaysPerAgingYearChangedProxy;
            mHudModel.SimFavoritesChanged += OnSimFavoritesChangedProxy;
            mHudModel.SimLotChanged += OnSimLotChangedProxy;
            mHudModel.SimMoodChanged += OnSimMoodChangedProxy;
            mHudModel.SimMoodValueChanged += OnSimMoodValueChangedProxy;
            mHudModel.SimMotivesChanged += OnSimMotivesChangedProxy;
            mHudModel.SimNameChanged += OnSimNameChangedProxy;
            mHudModel.SimRoomChanged += OnSimRoomChangedProxy;
            mHudModel.SkewerNotificationChanged += OnSkewerNotificationChangedProxy;
            mHudModel.SkillChanged += OnSkillChangedProxy;
            mHudModel.TraitsChanged += OnTraitsChangedProxy;
            mHudModel.TraitUseUpdated += OnTraitUseUpdatedProxy;
            mHudModel.TriggerCompetitionProgressBarGlowEvent += OnTriggerCompetitionProgressBarGlowEventProxy;
            mHudModel.TwelveHourClockSettingChanged += OnTwelveHourClockSettingChangedProxy;
            mHudModel.VisitorsChanged += OnVisitorsChangedProxy;
            mHudModel.WallModeChanged += OnWallModeChangedProxy;
            mHudModel.LunarUpdate += OnLunarUpdateProxy;
            mHudModel.MagicMotiveChanged += OnMagicMotiveChangedProxy;
            mHudModel.SeasonTransitioned += OnSeasonTransitionedProxy;
            mHudModel.TemperatureWaveChanged += OnTemperatureWaveChangedProxy;
            mHudModel.UpdatePhoneIcon += OnUpdatePhoneIconProxy;
            mHudModel.RefreshCurrentSimInfoSkewer += OnRefreshCurrentSimInfoSkewerProxy;
            mHudModel.BotCompetitionStandingEvent += OnBotCompetitionStandingEvent;
            mHudModel.HideBotCompetitionPanelEvent += OnHideBotCompetitionPanelEvent;
            mHudModel.ShowBotCompetitionPanelEvent += OnShowBotCompetitionPanelEvent;
            mHudModel.UpdateTimeAlmanac += OnUpdateTimeAlmanac;
            mHudModel.TriggerBotCompetitionProgressBarGlowEvent += OnTriggerBotCompetitionProgressBarGlowEvent;
       }

        // Events
        public event AutonomousInteractionCancelledFromUICallback AutonomousInteractionCancelledFromUI;
        public event CareerPerformanceCallback CareerPerformanceChanged;
        public event CareerUpdatedCallback CareerUpdated;
        public event CompetitionStandingsChanged CompetitionStandingEvent;
        public event InventoryOwnerChangedCallback CurrentSimInventoryOwnerChanged;
        public event VoidEventHandler HideCompetitionPanelEvent;
        public event HouseholdFundsChangedCallback HouseholdAncientCoinCountChanged;
        public event HouseholdChangedCallback HouseholdChanged;
        public event HouseholdFundsChangedCallback HouseholdFundsChanged;
        public event InteractionQueueDirtyCallback InteractionQueueDirtied;
        public event CareerUpdatedCallback JobTrackingUpdated;
        public event SimChangedCallback LifeEventUpdate;
        public event LifetimePointsChangedCallback LifetimePointsChanged;
        public event LotChangedCallback LotChanged;
        public event MinuteChangedCallback MinuteChanged;
        public event OccultUpdatedCallback OccultUpdated;
        public event PostureChangeCallback PostureChanged;
        public event SimRelationshipsChangedCallback RelationshipsChanged;
        public event RewardTraitsChangedCallback RewardTraitsChanged;
        public event InventoryOwnerChangedCallback SecondaryInventoryOwnerChanged;
        public event VoidEventHandler ShowCompetitionPanelEvent;
        public event SimChangedCallback SimAgeChanged;
        public event SimChangedCallback SimAppearanceChanged;
        public event SimStateChangedCallback SimBuffsChanged;
        public event SimStateChangedCallback SimCelebrityInfoChanged;
        public event SimChangedCallback SimChanged;
        public event SimWorldChangedCallback SimCurrentWorldChanged;
        public event VoidEventHandler SimDaysPerAgingYearChanged;
        public event SimChangedCallback SimFavoritesChanged;
        public event SimLotChangedCallback SimLotChanged;
        public event SimStateChangedCallback SimMoodChanged;
        public event SimStateChangedCallback SimMoodValueChanged;
        public event SimStateChangedCallback SimMotivesChanged;
        public event SimChangedCallback SimNameChanged;
        public event SimRoomChangedCallback SimRoomChanged;
        public event SkewerEventChangedCallback SkewerNotificationChanged;
        public event SkillChangedCallback SkillChanged;
        public event TraitsChangedCallback TraitsChanged;
        public event TraitUseUpdatedCallback TraitUseUpdated;
        public event CompetitionStandingsChanged TriggerCompetitionProgressBarGlowEvent;
        public event VoidEventHandler TwelveHourClockSettingChanged;
        public event VisitorsChangedCallback VisitorsChanged;
        public event WallModeChangedCallback WallModeChanged;
        public event LunarCycleUpdateCallback LunarUpdate;
        public event OccultUpdatedCallback MagicMotiveChanged;
        public event UpdatePhoneIconCallback UpdatePhoneIcon;
        public event VoidEventHandler SeasonTransitioned;
        public event TemperatureWaveChanged TemperatureWaveChanged;
        public event RefreshCurrentSimInfoSkewerCallback RefreshCurrentSimInfoSkewer;
        public event BotCompetitionStandingsChanged BotCompetitionStandingEvent;
        public event VoidEventHandler HideBotCompetitionPanelEvent;
        public event VoidEventHandler ShowBotCompetitionPanelEvent;
        public event UpdateTimeAlmanacCallback UpdateTimeAlmanac;
        public event BotCompetitionStandingsChanged TriggerBotCompetitionProgressBarGlowEvent;

        public void OnTriggerBotCompetitionProgressBarGlowEvent(IBotCompetitionInteractionInstance competitionInstance)
        {
            try
            {
                if (TriggerBotCompetitionProgressBarGlowEvent != null)
                {
                    TriggerBotCompetitionProgressBarGlowEvent(competitionInstance);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTriggerBotCompetitionProgressBarGlowEvent", e);
            }
        }

        public void OnUpdateTimeAlmanac()
        {
            try
            {
                if (UpdateTimeAlmanac != null)
                {
                    UpdateTimeAlmanac();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnUpdateTimeAlmanac", e);
            }
        }

        public void OnShowBotCompetitionPanelEvent()
        {
            try
            {
                if (ShowBotCompetitionPanelEvent != null)
                {
                    ShowBotCompetitionPanelEvent();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnShowBotCompetitionPanelEvent", e);
            }
        }

        public void OnHideBotCompetitionPanelEvent()
        {
            try
            {
                if (HideBotCompetitionPanelEvent != null)
                {
                    HideBotCompetitionPanelEvent();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnHideBotCompetitionPanelEvent", e);
            }
        }

        public void OnBotCompetitionStandingEvent(IBotCompetitionInteractionInstance competitionInstance)
        {
            try
            {
                if (BotCompetitionStandingEvent != null)
                {
                    BotCompetitionStandingEvent(competitionInstance);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnBotCompetitionStandingEvent", e);
            }
        }

        public void OnRefreshCurrentSimInfoSkewerProxy()
        {
            try
            {
                if (RefreshCurrentSimInfoSkewer != null)
                {
                    RefreshCurrentSimInfoSkewer();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnRefreshCurrentSimInfoSkewer", e);
            }
        }

        public void OnUpdatePhoneIconProxy(PhoneType phoneType, int colorNum, bool isBroken)
        {
            try
            {
                if (UpdatePhoneIcon != null)
                {
                    UpdatePhoneIcon(phoneType, colorNum, isBroken);

                }
            }
            catch (Exception e)
            {
                Common.Exception("OnUpdatePhoneIconPProxy", e);
            }
        }

        public void OnSeasonTransitionedProxy()
        {
            try
            {
                if (SeasonTransitioned != null)
                {
                    SeasonTransitioned();

                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSeasonTransitioned", e);
            }
        }

        public void OnTemperatureWaveChangedProxy(SimDisplay.WaveTypes waveType)
        {
            try
            {
                if (TemperatureWaveChanged != null)
                {
                    TemperatureWaveChanged(waveType);

                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTemperatureWaveChangedProxy", e);
            }
        }

        public void OnAutonomousInteractionCancelledFromUIProxy()
        {
            try
            {
                if (AutonomousInteractionCancelledFromUI != null)
                {
                    AutonomousInteractionCancelledFromUI();

                }
            }
            catch (Exception e)
            {
                Common.Exception("OnAutonomousInteractionCancelledFromUIProxy", e);
            }
        }

        public void OnCareerPerformanceChangedProxy(IOccupationEntry career, float performaceChange, float metricAv)
        {
            try
            {
                if (CareerPerformanceChanged != null)
                {
                    CareerPerformanceChanged(career, CareerLastPerformanceChange, metricAv);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCareerPerformanceChangedProxy", e);
            }
        }

        public void OnCareerUpdatedProxy()
        {
            try
            {
                if (CareerUpdated != null)
                {
                    CareerUpdated();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCareerUpdatedProxy", e);
            }
        }

        public void OnCompetitionStandingEventProxy(ICompetitionInteractionInstance competitionInstance)
        {
            try
            {
                if (CompetitionStandingEvent != null)
                {
                    CompetitionStandingEvent(competitionInstance);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCompetitionStandingEventProxy", e);
            }
        }

        public void OnCurrentSimInventoryOwnerChangedProxy(IInventory newInventory)
        {
            try
            {
                if (CurrentSimInventoryOwnerChanged != null)
                {
                    CurrentSimInventoryOwnerChanged(newInventory);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCurrentSimInventoryOwnerChangedProxy", e);
            }
        }

        public void OnHideCompetitionPanelEventProxy()
        {
            try
            {
                if (HideCompetitionPanelEvent != null)
                {
                    HideCompetitionPanelEvent();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnHideCompetitionPanelEventProxy", e);
            }
        }

        public void OnHouseholdAncientCoinCountChangedProxy(int oldValue, int newValue)
        {
            try
            {
                if (HouseholdAncientCoinCountChanged != null)
                {
                    HouseholdAncientCoinCountChanged(oldValue, newValue);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnHouseholdAncientCoinCountChangedProxy", e);
            }
        }

        public void OnHouseholdChangedProxy(Sims3.UI.Hud.HouseholdEvent ev, ObjectGuid objectGuid)
        {
            try
            {
                if (HouseholdChanged != null)
                {
                    HouseholdChanged(ev, objectGuid);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Simulator.GetProxy(objectGuid), e);
            }
        }

        public void OnHouseholdFundsChangedProxy(int oldValue, int newValue)
        {
            try
            {
                if (HouseholdFundsChanged != null)
                {
                    HouseholdFundsChanged(oldValue, newValue);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnHouseholdFundsChangedProxy", e);
            }
        }

        public void OnInteractionQueueDirtiedProxy()
        {
            try
            {
                if (InteractionQueueDirtied != null)
                {
                    InteractionQueueDirtied();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnInteractionQueueDirtiedProxy", e);
            }
        }

        public void OnJobTrackingUpdatedProxy()
        {
            try
            {
                if (JobTrackingUpdated != null)
                {
                    JobTrackingUpdated();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnJobTrackingUpdatedProxy", e);
            }
        }

        public void OnLifeEventUpdateProxy(ObjectGuid objectGuid)
        {
            try
            {
                if (LifeEventUpdate != null)
                {
                    LifeEventUpdate(objectGuid);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Simulator.GetProxy(objectGuid), e);
            }
        }

        public void OnLifetimePointsChangedProxy()
        {
            try
            {
                if (LifetimePointsChanged != null)
                {
                    LifetimePointsChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnLifetimePointsChangedProxy", e);
            }
        }

        public void OnLotChangedProxy(ulong newLotId)
        {
            try
            {
                if (LotChanged != null)
                {
                    LotChanged(newLotId);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnLotChangedProxy", e);
            }
        }

        public void OnMinuteChangedProxy()
        {
            try
            {
                if (MinuteChanged != null)
                {
                    MinuteChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMinuteChangedProxy", e);
            }
        }

        public void OnOccultUpdatedProxy()
        {
            try
            {
                if (OccultUpdated != null)
                {
                    OccultUpdated();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnOccultUpdatedProxy", e);
            }
        }

        public void OnPostureChangedProxy()
        {
            try
            {
                if (PostureChanged != null)
                {
                    PostureChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnPostureChangedProxy", e);
            }
        }

        public void OnRelationshipsChangedProxy()
        {
            try
            {
                if (RelationshipsChanged != null)
                {
                    RelationshipsChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnRelationshipsChangedProxy", e);
            }
        }

        public void OnRewardTraitsChangedProxy()
        {
            try
            {
                if (RewardTraitsChanged != null)
                {
                    RewardTraitsChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnRewardTraitsChangedProxy", e);
            }
        }

        public void OnSecondaryInventoryOwnerChangedProxy(IInventory newInventory)
        {
            try
            {
                if (SecondaryInventoryOwnerChanged != null)
                {
                    SecondaryInventoryOwnerChanged(newInventory);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSecondaryInventoryOwnerChangedProxy", e);
            }
        }

        public void OnShowCompetitionPanelEventProxy()
        {
            try
            {
                if (ShowCompetitionPanelEvent != null)
                {
                    ShowCompetitionPanelEvent();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnShowCompetitionPanelEventProxy", e);
            }
        }

        public void OnSimAgeChangedProxy(ObjectGuid objectGuid)
        {
            try
            {
                if (SimAgeChanged != null)
                {
                    SimAgeChanged(objectGuid);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Simulator.GetProxy(objectGuid), e);
            }
        }

        public void OnSimAppearanceChangedProxy(ObjectGuid objectGuid)
        {
            try
            {
                if (SimAppearanceChanged != null)
                {
                    SimAppearanceChanged(objectGuid);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Simulator.GetProxy(objectGuid), e);
            }
        }

        public void OnSimBuffsChangedProxy(SimInfo info)
        {
            try
            {
                if (SimBuffsChanged != null)
                {
                    SimBuffsChanged(info);
                }
            }
            catch (Exception e)
            {
                Common.Exception(info.mName, e);
            }
        }

        public void OnSimCelebrityInfoChangedProxy(SimInfo info)
        {
            try
            {
                if (SimCelebrityInfoChanged != null)
                {
                    SimCelebrityInfoChanged(info);
                }
            }
            catch (Exception e)
            {
                Common.Exception(info.mName, e);
            }
        }

        public void OnSimChangedProxy(ObjectGuid objectGuid)
        {
            try
            {
                if (SimChanged != null)
                {
                    SimChanged(objectGuid);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Simulator.GetProxy(objectGuid), e);
            }
        }

        public void OnSimCurrentWorldChangedProxy(bool toCurrentWorld, IMiniSimDescription simDesc)
        {
            try
            {
                if (SimCurrentWorldChanged != null)
                {
                    SimCurrentWorldChanged(toCurrentWorld, simDesc);
                }
            }
            catch (Exception e)
            {
                Common.Exception(simDesc.FullName, e);
            }
        }

        public void OnSimDaysPerAgingYearChangedProxy()
        {
            try
            {
                if (SimDaysPerAgingYearChanged != null)
                {
                    SimDaysPerAgingYearChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimDaysPerAgingYearChangedProxy", e);
            }
        }

        public void OnSimFavoritesChangedProxy(ObjectGuid objectGuid)
        {
            try
            {
                if (SimFavoritesChanged != null)
                {
                    SimFavoritesChanged(objectGuid);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Simulator.GetProxy(objectGuid), e);
            }
        }

        public void OnSimLotChangedProxy(ulong lotID, bool isHome)
        {
            try
            {
                if (SimLotChanged != null)
                {
                    SimLotChanged(lotID, isHome);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimLotChangedProxy", e);
            }
        }

        public void OnSimMoodChangedProxy(SimInfo info)
        {
            try
            {
                if (SimMoodChanged != null)
                {
                    SimMoodChanged(info);
                }
            }
            catch (Exception e)
            {
                Common.Exception(info.mName, e);
            }
        }

        public void OnSimMoodValueChangedProxy(SimInfo info)
        {
            try
            {
                if (SimMoodValueChanged != null)
                {
                    SimMoodValueChanged(info);
                }
            }
            catch (Exception e)
            {
                Common.Exception(info.mName, e);
            }
        }

        public void OnSimMotivesChangedProxy(SimInfo info)
        {
            try
            {
                if (SimMotivesChanged != null)
                {
                    SimMotivesChanged(info);
                }
            }
            catch (Exception e)
            {
                Common.Exception(info.mName, e);
            }
        }

        public void OnSimNameChangedProxy(ObjectGuid objectGuid)
        {
            try
            {
                if (SimNameChanged != null)
                {
                    SimNameChanged(objectGuid);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Simulator.GetProxy(objectGuid), e);
            }
        }

        public void OnSimRoomChangedProxy()
        {
            try
            {
                if (SimRoomChanged != null)
                {
                    SimRoomChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("", e);
            }
        }

        public void OnSkewerNotificationChangedProxy(SkewerNotificationType type, ObjectGuid simGuid, bool bShow)
        {
            try
            {
                if (SkewerNotificationChanged != null)
                {
                    SkewerNotificationChanged(type, simGuid, bShow);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Simulator.GetProxy(simGuid), e);
            }
        }

        public void OnSkillChangedProxy()
        {
            try
            {
                if (SkillChanged != null)
                {
                    SkillChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSkillChangedProxy", e);
            }
        }

        public void OnTraitsChangedProxy(SimInfo info)
        {
            try
            {
                if (TraitsChanged != null)
                {
                    TraitsChanged(info);
                }
            }
            catch (Exception e)
            {
                Common.Exception(info.mName, e);
            }
        }

        public void OnTraitUseUpdatedProxy(ulong traitName)
        {
            try
            {
                if (TraitUseUpdated != null)
                {
                    TraitUseUpdated(traitName);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTraitUseUpdatedProxy", e);
            }
        }

        public void OnTriggerCompetitionProgressBarGlowEventProxy(ICompetitionInteractionInstance competitionInstance)
        {
            try
            {
                if (TriggerCompetitionProgressBarGlowEvent != null)
                {
                    TriggerCompetitionProgressBarGlowEvent(competitionInstance);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTriggerCompetitionProgressBarGlowEventProxy", e);
            }
        }

        public void OnTwelveHourClockSettingChangedProxy()
        {
            try
            {
                if (TwelveHourClockSettingChanged != null)
                {
                    TwelveHourClockSettingChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTwelveHourClockSettingChangedProxy", e);
            }
        }

        public void OnVisitorsChangedProxy()
        {
            try
            {
                if (VisitorsChanged != null)
                {
                    VisitorsChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnVisitorsChangedProxy", e);
            }
        }

        public void OnWallModeChangedProxy()
        {
            try
            {
                if (WallModeChanged != null)
                {
                    WallModeChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnWallModeChangedProxy", e);
            }
        }

        public void OnLunarUpdateProxy(uint moonPhase)
        {
            try
            {
                if (LunarUpdate != null)
                {
                    LunarUpdate(moonPhase);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnLunarUpdateProxy", e);
            }
        }

        public void OnMagicMotiveChangedProxy()
        {
            try
            {
                if (MagicMotiveChanged != null)
                {
                    MagicMotiveChanged();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMagicMotiveChangedProxy", e);
            }
        }

        // Methods
        public void AcceptOpportunity(ObjectGuid simID, ulong oppID)
        {
            mHudModel.AcceptOpportunity(simID, oppID);
        }

        public RewardTraitAdditionResult AddRewardTraits(IUIRewardTrait traitsToAdd)
        {
            return mHudModel.AddRewardTraits(traitsToAdd);
        }

        public RewardTraitAdditionResult AddRewardTraits(IUIRewardTrait traitsToAdd, int traitCost)
        {
            return mHudModel.AddRewardTraits(traitsToAdd, traitCost);
        }

        public void AttachCameraToSim(ObjectGuid objectGuid)
        {
            mHudModel.AttachCameraToSim(objectGuid);
        }

        public bool CancelInteraction(ulong interactionID)
        {
            return mHudModel.CancelInteraction(interactionID);
        }

        public void CancelPosture()
        {
            mHudModel.CancelPosture();
        }

        public void CancelSocializing()
        {
            mHudModel.CancelSocializing();
        }

        public bool CanChangeWallMode()
        {
            return mHudModel.CanChangeWallMode();
        }

        public bool CanLevelDown(bool fogEnabled)
        {
            return mHudModel.CanLevelDown(fogEnabled);
        }

        public bool CanLevelUp(bool fogEnabled)
        {
            return mHudModel.CanLevelUp(fogEnabled);
        }

        public bool CanSellObjs(List<ObjectGuid> objGuids, out int price)
        {
            return mHudModel.CanSellObjs(objGuids, out price);
        }

        public void CaptureLifeEvent()
        {
            mHudModel.CaptureLifeEvent();
        }

        public void CenterCameraOnSim(ObjectGuid objectGuid)
        {
            mHudModel.CenterCameraOnSim(objectGuid);
        }

        public void ChangeRelationship(ISimDescription simDesc, float value)
        {
            mHudModel.ChangeRelationship(simDesc, value);
        }

        public void ChangeVisibilityToSimsLevel(ObjectGuid objectGuid)
        {
            mHudModel.ChangeVisibilityToSimsLevel(objectGuid);
        }

        public bool CheckAndMoveCameraToSim(ObjectGuid objectGuid)
        {
            return mHudModel.CheckAndMoveCameraToSim(objectGuid);
        }

        public bool CheckForEmergency(ref string emergencyWarning)
        {
            return mHudModel.CheckForEmergency(ref emergencyWarning);
        }

        public void CheckPassportForRecall(SimInfo info)
        {
            mHudModel.CheckPassportForRecall(info);
        }

        public void CloseSecondaryInventory()
        {
            mHudModel.CloseSecondaryInventory();
        }

        public Route CreateEmptyRouteForCurrentSim()
        {
            return mHudModel.CreateEmptyRouteForCurrentSim();
        }

        public IBuff CurrentMotiveBuff(IBuff inBuff)
        {
            return mHudModel.CurrentMotiveBuff(inBuff);
        }

        public bool CurrentSimHasValidInteractionsOn(ObjectGuid objGuid, ScenePickArgs eventArgs)
        {
            return mHudModel.CurrentSimHasValidInteractionsOn(objGuid, eventArgs);
        }

        public bool CurrentSimHasValidInventoryInteractionsOn(ObjectGuid objGuid, bool isStack)
        {
            return mHudModel.CurrentSimHasValidInventoryInteractionsOn(objGuid, isStack);
        }

        public void DecreaseMotiveState(uint motive)
        {
            mHudModel.DecreaseMotiveState(motive);
        }

        public void DelayNextStateStartup()
        {
            mHudModel.DelayNextStateStartup();
        }

        public void DisableQuit()
        {
            mHudModel.DisableQuit();
        }

        public void EnableQuit()
        {
            mHudModel.EnableQuit();
        }

        public void ForwardGameplayClickEvent(ScenePickArgs eventArgs)
        {
            mHudModel.ForwardGameplayClickEvent(eventArgs);
        }

        public string GetBuffDescription(IBuff inBuff)
        {
            return mHudModel.GetBuffDescription(inBuff);
        }

        public string GetBuffName(IBuff inBuff)
        {
            return mHudModel.GetBuffName(inBuff);
        }

        public IInteractionInstance GetCurrentInteraction()
        {
            return mHudModel.GetCurrentInteraction();
        }

        public string GetCurrentMotiveDescription(uint motive)
        {
            return mHudModel.GetCurrentMotiveDescription(motive);
        }

        public string GetCurrentMotiveName(uint motive)
        {
            return mHudModel.GetCurrentMotiveName(motive);
        }

        public SimInfo GetCurrentSimInfo()
        {
            return mHudModel.GetCurrentSimInfo();
        }

        public List<SimInfo> GetHouseholdHumanSims()
        {
            return mHudModel.GetHouseholdHumanSims();
        }

        public List<SimInfo> GetHouseholdPets()
        {
            return mHudModel.GetHouseholdPets();
        }

        public List<SimInfo> GetHouseholdSims()
        {
            return mHudModel.GetHouseholdSims();
        }

        public List<SimInfo> GetHouseholdVisitors()
        {
            return mHudModel.GetHouseholdVisitors();
        }

        public int GetBuyHorseCost(IMiniSimDescription petDesc)
        {
            return mHudModel.GetBuyHorseCost(petDesc);
        }

        public List<IInteractionInstance> GetInteractionList()
        {
            return mHudModel.GetInteractionList();
        }

        public IInventory GetInventoryContainingObject(ObjectGuid objGuid)
        {
            return mHudModel.GetInventoryContainingObject(objGuid);
        }

        public IInventory GetInventoryForObject(ObjectGuid objGuid)
        {
            return mHudModel.GetInventoryForObject(objGuid);
        }

        public List<KnownInfo> GetKnownInfo(IMiniSimDescription simDesc)
        {
            return HudModelEx.GetKnownInfo(mHudModel as Sims3.Gameplay.UI.HudModel, simDesc);
        }

        public List<KnownInfo> GetKnownInfo(IMiniSimDescription curSimDesc, IMiniSimDescription otherSimDesc)
        {
            return mHudModel.GetKnownInfo(curSimDesc, otherSimDesc);
        }

        public List<KnownInfo> GetKnownInfoAboutSelf(IMiniSimDescription simDescription)
        {
            return HudModelEx.GetKnownInfoAboutSelf(mHudModel as Sims3.Gameplay.UI.HudModel, simDescription);
        }

        public string GetLocalizedSkillName(ObjectGuid simGuid, ulong SkillGuid)
        {
            return mHudModel.GetLocalizedSkillName(simGuid, SkillGuid);
        }

        public string GetLTRRelationshipString(IMiniSimDescription curSimDesc, IMiniSimDescription otherSimDesc)
        {
            return mHudModel.GetLTRRelationshipString(curSimDesc, otherSimDesc);
        }

        public IRelationship GetRelationshipWith(ISimDescription simDesc)
        {
            return mHudModel.GetRelationshipWith(simDesc);
        }

        public float GetSimCurrentAgeInDays(ObjectGuid simGuid)
        {
            return mHudModel.GetSimCurrentAgeInDays(simGuid);
        }

        public ISimDescription GetSimDescription()
        {
            return mHudModel.GetSimDescription();
        }

        public string GetPetBreedName(IMiniSimDescription petDesc)
        {
            return mHudModel.GetPetBreedName(petDesc);
        }

        public SimInfo GetSimInfo(ObjectGuid objectGuid)
        {
            return mHudModel.GetSimInfo(objectGuid);
        }

        public int GetSimSkillLevel(ObjectGuid simGuid, ulong SkillGuid)
        {
            return mHudModel.GetSimSkillLevel(simGuid, SkillGuid);
        }

        public ThumbnailKey GetThumbnailForGameObject(ObjectGuid objGuid)
        {
            return mHudModel.GetThumbnailForGameObject(objGuid);
        }

        public ThumbnailKey GetThumbnailForGameObject(ObjectGuid objGuid, bool bForceUseSimDescription)
        {
            return mHudModel.GetThumbnailForGameObject(objGuid, bForceUseSimDescription);
        }

        public ThumbnailKey GetThumbnailForGameObject(ObjectGuid objGuid, ThumbnailSize size, int index)
        {
            return mHudModel.GetThumbnailForGameObject(objGuid, size, index);
        }

        public ThumbnailKey GetThumbnailForGameObject(ObjectGuid objGuid, ThumbnailSize size, int index, bool bForceUseSimDescription)
        {
            return mHudModel.GetThumbnailForGameObject(objGuid, size, index, bForceUseSimDescription);
        }

        public ThumbnailKey GetThumbnailForSimDescriptionId(ulong simDescriptionId, ThumbnailSize size, int index)
        {
            return mHudModel.GetThumbnailForSimDescriptionId(simDescriptionId, size, index);
        }

        public Tooltip GetTooltipForInventoryItem(Vector2 mousePosition, WindowBase currMousedOverWindow, ObjectGuid obj, ref Vector2 tooltipPosition)
        {
            return mHudModel.GetTooltipForInventoryItem(mousePosition, currMousedOverWindow, obj, ref tooltipPosition);
        }

        public int GetVisaLevel(WorldName world)
        {
            return mHudModel.GetVisaLevel(world);
        }

        public void GlowClicked()
        {
            mHudModel.GlowClicked();
        }

        public void GroupingButtonClicked(ObjectGuid simGuid, UIMouseEventArgs eventArgs)
        {
            mHudModel.GroupingButtonClicked(simGuid, eventArgs);
        }

        public void IncreaseLifetimeHappiness()
        {
            mHudModel.IncreaseLifetimeHappiness();
        }

        public void IncreaseMotiveState(uint motive)
        {
            mHudModel.IncreaseMotiveState(motive);
        }

        public void Initialize()
        {
            mHudModel.Initialize();
        }

        public void InitializeSimInfo()
        {
            mHudModel.InitializeSimInfo();
        }

        public bool IsBuildBuyMode()
        {
            return mHudModel.IsBuildBuyMode();
        }

        public bool IsCasState()
        {
            return mHudModel.IsCasState();
        }

        public bool IsCelebrityLot(ulong lotID)
        {
            return mHudModel.IsCelebrityLot(lotID);
        }

        public bool IsGameEntryState()
        {
            return mHudModel.IsGameEntryState();
        }

        public bool IsGameSpeedLocked()
        {
            return mHudModel.IsGameSpeedLocked();
        }

        public bool IsInteractionCancellableByPlayer(ulong interactionID)
        {
            return mHudModel.IsInteractionCancellableByPlayer(interactionID);
        }

        public bool IsLotUnoccupied(ulong lotID)
        {
            return mHudModel.IsLotUnoccupied(lotID);
        }

        public bool IsNestedInteractionForCurrentPosture(IInteractionInstance instance)
        {
            return mHudModel.IsNestedInteractionForCurrentPosture(instance);
        }

        public bool IsObjectTerrainMimic(ObjectGuid objectGuid)
        {
            return mHudModel.IsObjectTerrainMimic(objectGuid);
        }

        public bool IsPlayFlowState()
        {
            return mHudModel.IsPlayFlowState();
        }

        public bool IsResidentialLotID(ulong lotID)
        {
            return mHudModel.IsResidentialLotID(lotID);
        }

        public bool IsTraitReinforced(ITraitEntryInfo traitInfo)
        {
            return mHudModel.IsTraitReinforced(traitInfo);
        }

        public string LocationIconName(WorldName world)
        {
            return mHudModel.LocationIconName(world);
        }

        public string LocationName(WorldName world)
        {
            return mHudModel.LocationName(world);
        }

        public string LocationName(WorldName world, bool fullName)
        {
            return mHudModel.LocationName(world, fullName);
        }

        public bool LotHasValidInteractions(ulong lotID, ScenePickArgs eventArgs)
        {
            return mHudModel.LotHasValidInteractions(lotID, eventArgs);
        }

        public void LotLevelDown(bool all)
        {
            mHudModel.LotLevelDown(all);
        }

        public void LotLevelUp(bool all, bool noRoof)
        {
            mHudModel.LotLevelUp(all, noRoof);
        }

        public bool HasSimsAvailableForPassport()
        {
            return mHudModel.HasSimsAvailableForPassport();
        }

        public void LoadingScreenDisposed()
        {
            mHudModel.LoadingScreenDisposed();
        }

        public void MaxMotiveStates()
        {
            mHudModel.MaxMotiveStates();
        }

        public void MoveCameraToSim(ObjectGuid objectGuid)
        {
            mHudModel.MoveCameraToSim(objectGuid);
        }

        public void NextMoodState()
        {
            mHudModel.NextMoodState();
        }

        public void SendTNSComment(long nucleusId, long postId, string commentText)
        {
            mHudModel.SendTNSComment(nucleusId, postId, commentText);
        }

        public void SendTNSPassportMsg(long msgId, string postText)
        {
            mHudModel.SendTNSPassportMsg(msgId, postText);
        }

        public void SendTNSPost(long nucleusId, string commentText)
        {
            mHudModel.SendTNSPost(nucleusId, commentText);
        }

        public void ShowGigScheduleButtonWindow()
        {
            mHudModel.ShowGigScheduleButtonWindow();
        }

        public void ShowIGMTutorial()
        {
            mHudModel.ShowIGMTutorial();
        }

        public void NotifyTooltipHidden(ObjectGuid objGuid)
        {
            mHudModel.NotifyTooltipHidden(objGuid);
        }

        public void OnAutonomousInteractionCancelledFromUI()
        {
            mHudModel.OnAutonomousInteractionCancelledFromUI();
        }

        public void OnPostureChanged()
        {
            mHudModel.OnPostureChanged();
        }

        public bool OpenInventoryContainingObject(ObjectGuid objGuid)
        {
            return mHudModel.OpenInventoryContainingObject(objGuid);
        }

        public void PickGameObject(UIMouseEventArgs eventArgs, ObjectGuid objectGuid)
        {
            mHudModel.PickGameObject(eventArgs, objectGuid);
        }

        public void PickObjectFromInventory(UIMouseEventArgs eventArgs, ObjectGuid objGuid, bool isStackOfItems)
        {
            mHudModel.PickObjectFromInventory(eventArgs, objGuid, isStackOfItems);
        }

        public void PlayLoadLoopAudio(WorldName world)
        {
            mHudModel.PlayLoadLoopAudio(world);
        }

        public bool PurchaseAdventureReward(IUIAdventureReward reward, ref ObjectGuid guid)
        {
            return mHudModel.PurchaseAdventureReward(reward, ref guid);
        }

        public PushInteractionErrorCode PushGoHome()
        {
            return mHudModel.PushGoHome();
        }

        public bool PushGoSchool(Vector2 position)
        {
            return mHudModel.PushGoSchool(position);
        }

        public bool PushGoToActiveJob(Vector2 position)
        {
            return mHudModel.PushGoToActiveJob(position);
        }

        public bool PushGoWork(Vector2 position)
        {
            return mHudModel.PushGoWork(position);
        }

        public void RefreshMapTags()
        {
            mHudModel.RefreshMapTags();
        }

        public void RejectOpportunity(ObjectGuid simID, ulong oppID)
        {
            mHudModel.RejectOpportunity(simID, oppID);
        }

        public void RemoveBuff(IBuff inBuff)
        {
            mHudModel.RemoveBuff(inBuff);
        }

        public void RestoreUIVisibility()
        {
            mHudModel.RestoreUIVisibility();
        }

        public bool SelectSim(ObjectGuid objectGuid)
        {
            return mHudModel.SelectSim(objectGuid);
        }

        public void SendSkewerNotfication(SkewerNotificationType type, ObjectGuid simGuid, bool bShow)
        {
            mHudModel.SendSkewerNotfication(type, simGuid, bShow);
        }

        public void SetBestDisplayLevelForSelectedSim()
        {
            mHudModel.SetBestDisplayLevelForSelectedSim();
        }

        public void SetMotiveValue(MotiveID motive, float value)
        {
            mHudModel.SetMotiveValue(motive, value);
        }

        public bool ShouldShowDateButton(ObjectGuid simGuid)
        {
            return mHudModel.ShouldShowDateButton(simGuid);
        }

        public bool ShouldShowFollowNotificationForSim(ObjectGuid sim)
        {
            return mHudModel.ShouldShowFollowNotificationForSim(sim);
        }

        public bool ShouldShowGoHomeButtonForSim(ObjectGuid sim)
        {
            return mHudModel.ShouldShowGoHomeButtonForSim(sim);
        }

        public bool ShouldShowGroupButton(ObjectGuid simGuid)
        {
            return mHudModel.ShouldShowGroupButton(simGuid);
        }

        public void ShowCellPhoneInteractions(UIMouseEventArgs eventArgs)
        {
            mHudModel.ShowCellPhoneInteractions(eventArgs);
        }

        public void ShowGreyedOutTooltip(string textKey, Vector2 position)
        {
            mHudModel.ShowGreyedOutTooltip(textKey, position);
        }

        public void ShowGreyedOutTooltip(string textKey, Vector2 position, bool overridePieMenuLayerVisibility)
        {
            mHudModel.ShowGreyedOutTooltip(textKey, position, overridePieMenuLayerVisibility);
        }

        public void Shutdown()
        {
            mHudModel.Shutdown();
        }

        public HUDSimImageCache SimImageCache()
        {
            return mHudModel.SimImageCache();
        }

        public void SimTraitsChanged(ObjectGuid simGuid)
        {
            mHudModel.SimTraitsChanged(simGuid);
        }

        public string SmallLocationIconName(WorldName world)
        {
            return mHudModel.SmallLocationIconName(world);
        }

        public int StartInGameStoreMusicLoopAudio()
        {
            return mHudModel.StartInGameStoreMusicLoopAudio();
        }

        public void StopInGameStoreMusicLoopAudio(int prevMusicMode)
        {
            mHudModel.StopInGameStoreMusicLoopAudio(prevMusicMode);
        }

        public void StopLoadLoopAudio()
        {
            mHudModel.StopLoadLoopAudio();
        }

        public void TransitionToBuildMode()
        {
            mHudModel.TransitionToBuildMode();
        }

        public void TransitionToBuyMode()
        {
            mHudModel.TransitionToBuyMode();
        }

        public void TransitionToCASMode()
        {
            mHudModel.TransitionToCASMode();
        }

        public void TransitionToLiveMode()
        {
            mHudModel.TransitionToLiveMode();
        }

        public bool TrySolveBuff(IBuff buff)
        {
            return mHudModel.TrySolveBuff(buff);
        }

        public void UnDelayNextStateStartup()
        {
            mHudModel.UnDelayNextStateStartup();
        }

        public void UpdateSimInfo(SimInfo info, SimUpdateContext context)
        {
            mHudModel.UpdateSimInfo(info, context);
        }

        public bool TryParseProductVersion(string name, out ProductVersion value, ProductVersion defaultValue)
        {
            return mHudModel.TryParseProductVersion(name, out value, defaultValue);
        }

        public void ValidateThumbnail(ThumbnailKey thumbKey)
        {
            mHudModel.ValidateThumbnail(thumbKey);
        }

        public string GetPassportGivenGiftText(int gift)
        {
            return mHudModel.GetPassportGivenGiftText(gift);
        }

        public string GetPassportPerformTrickText(int trick)
        {
            return mHudModel.GetPassportPerformTrickText(trick);
        }

        public string GetPassportRespondToShowText(int respondToShowAction)
        {
            return mHudModel.GetPassportRespondToShowText(respondToShowAction);
        }

        public List<UiGigInfo> GetScheduledGigsForActiveSim()
        {
            return mHudModel.GetScheduledGigsForActiveSim();
        }

        public void TriggerProgressBarGlow(ICompetitionInteractionInstance competitionInstance)
        {
            mHudModel.TriggerProgressBarGlow(competitionInstance);
        }

        public void UpdateCompetitionStanding(ICompetitionInteractionInstance competitionInstance)
        {
            mHudModel.UpdateCompetitionStanding(competitionInstance);
        }

        public void ShowCompetitionPanel()
        {
            mHudModel.ShowCompetitionPanel();
        }

        public void HideCompetitionPanel()
        {
            mHudModel.HideCompetitionPanel();
        }

        public ICompetitionInteractionInstance GetCurrentSimCompetitionInteraction()
        {
            return mHudModel.GetCurrentSimCompetitionInteraction();
        }

        public List<ITraitEntryInfo> GetAdoptPetTraits(IMiniSimDescription petDesc)
        {
            return mHudModel.GetAdoptPetTraits(petDesc);
        }

        public int GetBreedCost(IMiniSimDescription petDesc)
        {
            return mHudModel.GetBreedCost(petDesc);
        }

        public Dictionary<IMiniSimDescription, IRelationship> CurrentRelationships 
        {
            get 
            {
                try
                {
                    //Common.DebugStackLog("CurrentRelationships");

                    Dictionary<IMiniSimDescription, IRelationship> allRelationships = mHudModel.CurrentRelationships;
                    if (allRelationships == null) return allRelationships;

                    Dictionary<IMiniSimDescription, IRelationship> relationships = new Dictionary<IMiniSimDescription, IRelationship>();

                    foreach (KeyValuePair<IMiniSimDescription, IRelationship> relation in allRelationships)
                    {
                        if (relation.Key is ISimDescription)
                        {
                            relationships.Add(relation.Key, relation.Value);
                        }
                        else
                        {
                            relationships.Add(new MiniSimDescriptionProxy(relation.Key), relation.Value);
                        }
                    }

                    return relationships;
                }
                catch (Exception e)
                {
                    Common.Exception("CurrentRelationships", e);
                    return new Dictionary<IMiniSimDescription, IRelationship>();
                }
            } 
        }

        // Properties
        public bool ActiveCareerInstalled { get { return mHudModel.ActiveCareerInstalled; } }
        public List<AdventureJournalDialog.IAdventureJournalAboutPageInfo> AdventureJournalAboutPages { get { return mHudModel.AdventureJournalAboutPages; } }
        public AdventureJournalDialog.IAdventureJournalRelicCollectionPageInfo AdventureJournalRelicCollectionPageInfo { get { return mHudModel.AdventureJournalRelicCollectionPageInfo; } }
        public AdventureJournalDialog.IAdventureJournalRelicPageInfo AdventureJournalRelicPageInfo { get { return mHudModel.AdventureJournalRelicPageInfo; } }
        public AdventureJournalDialog.IAdventureJournalTombStatsPageInfo AdventureJournalTombStatsPageInfo { get { return mHudModel.AdventureJournalTombStatsPageInfo; } }
        public List<IUIAdventureReward> AllAvailableAdventureRewards { get { return mHudModel.AllAvailableAdventureRewards; } }
        public Dictionary<string, List<IUIRewardTrait>> AllAvailableRewardTraits { get { return mHudModel.AllAvailableRewardTraits; } }
        public BuildBuyDayNightMode BuildBuyDayNightMode { get { return mHudModel.BuildBuyDayNightMode; } set { mHudModel.BuildBuyDayNightMode = value; } }
        public bool CanEnableBuildBuy { get { return mHudModel.CanEnableBuildBuy; } }
        public List<IOccupationEntry> CareerHistoryEntries { get { return mHudModel.CareerHistoryEntries; } }
        public float CareerHoursTillWork { get { return mHudModel.CareerHoursTillWork; } }
        public float CareerLastPerformanceChange { get { return mHudModel.CareerLastPerformanceChange; } }
        public ICelebrityJournalEntry CelebrityJournalEntry { get { return mHudModel.CelebrityJournalEntry; } }
        public bool CheatsEnabled { get { return mHudModel.CheatsEnabled; } }
        public int CurrentAncientCoins { get { return mHudModel.CurrentAncientCoins; } set { mHudModel.CurrentAncientCoins = value; } }
        public int CurrentDay { get { return mHudModel.CurrentDay; } }
        public Gameflow.GameSpeed CurrentGameSpeed { get { return mHudModel.CurrentGameSpeed; } set { mHudModel.CurrentGameSpeed = value; } }
        public int CurrentLifetimePoints { get { return mHudModel.CurrentLifetimePoints; } set { mHudModel.CurrentLifetimePoints = value; } }
        public IOccupationEntry CurrentOccupation { get { return mHudModel.CurrentOccupation; } }
        public IOccupationEntry CurrentSchool { get { return mHudModel.CurrentSchool; } }
        public IInventory CurrentSimInventory { get { return mHudModel.CurrentSimInventory; } }
        public List<IUIRewardTrait> CurrentSimsRewardTraits { get { return mHudModel.CurrentSimsRewardTraits; } }
        public float CurrentTime { get { return mHudModel.CurrentTime; } }
        public int CurrentTripDay { get { return mHudModel.CurrentTripDay; } }
        public string DaysTillAgingUp { get { return mHudModel.DaysTillAgingUp; } }
        public IInventory FamilyInventory { get { return mHudModel.FamilyInventory; } }
        public FloorGridMode FloorGridMode { get { return mHudModel.FloorGridMode; } set { mHudModel.FloorGridMode = value; } }
        public string GoHomeButtonTooltip { get { return mHudModel.GoHomeButtonTooltip; } }
        public GraduationType GraduationType { get { return mHudModel.GraduationType; } }
        public bool HasPostureBeenCanceled { get { return mHudModel.HasPostureBeenCanceled; } }
        public int HouseholdFunds { get { return mHudModel.HouseholdFunds; } set { mHudModel.HouseholdFunds = value; } }
        public bool IsPostureSocializing { get { return mHudModel.IsPostureSocializing; } }
        public bool IsSocializing { get { return mHudModel.IsSocializing; } }
        public int kAdditionalInteractionsDisplayed { get { return mHudModel.kAdditionalInteractionsDisplayed; } }
        public ILifeEventManager LifeEventManager { get { return mHudModel.LifeEventManager; } }
        public int LotLevel { get { return mHudModel.LotLevel; } }
        public uint LowestVisibleCelebrityLevel { get { return mHudModel.LowestVisibleCelebrityLevel; } }
        public int MaxTripDays { get { return mHudModel.MaxTripDays; } }
        public int MoodValueLifetimeRewards { get { return mHudModel.MoodValueLifetimeRewards; } }
        public int MoodValueMax { get { return mHudModel.MoodValueMax; } }
        public int MoodValueMin { get { return mHudModel.MoodValueMin; } }
        public float[] MotiveValueMaxs { get { return mHudModel.MotiveValueMaxs; } }
        public float[] MotiveValueMins { get { return mHudModel.MotiveValueMins; } }
        public UIImage PostureIcon { get { return mHudModel.PostureIcon; } }
        public bool PostureImplicit { get { return mHudModel.PostureImplicit; } }
        public string PostureName { get { return mHudModel.PostureName; } }
        public int RoofLevel { get { return mHudModel.RoofLevel; } }
        public IInventory SecondaryInventory { get { return mHudModel.SecondaryInventory; } }
        public bool ShowCelebrityButtonGlow { get { return mHudModel.ShowCelebrityButtonGlow; } }
        public CASAgeGenderFlags SimAge { get { return mHudModel.SimAge; } }
        public ulong SimCurrentActiveTrait { get { return mHudModel.SimCurrentActiveTrait; } }
        public float SimDaysRemainingInAge { get { return mHudModel.SimDaysRemainingInAge; } }
        public float SimDaysTillAgeUp { get { return mHudModel.SimDaysTillAgeUp; } }
        public List<string> SimFavorites { get { return mHudModel.SimFavorites; } }
        public List<UIImage> SimFavoritesImages { get { return mHudModel.SimFavoritesImages; } }
        public bool SimOnJobLot { get { return mHudModel.SimOnJobLot; } }
        public List<ITraitEntryInfo> SimTraits { get { return mHudModel.SimTraits; } }
        public Zodiac SimZodiacSign { get { return mHudModel.SimZodiacSign; } }
        public List<ISkillJournalEntry> SkillJournalEntries { get { return mHudModel.SkillJournalEntries; } }
        public List<ISkillJournalEntry> SkillPanelEntries { get { return mHudModel.SkillPanelEntries; } }
        public ISimDescription TalkedToSimDesc { get { return mHudModel.TalkedToSimDesc; } }
        public ThumbnailKey TalkedToSimThumbnail { get { return mHudModel.TalkedToSimThumbnail; } }
        public ObjectGuid TerrainObjectGuid { get { return mHudModel.TerrainObjectGuid; } }
        public string TimeString { get { return mHudModel.TimeString; } }
        public int TimeStringChangeCounter { get { return mHudModel.TimeStringChangeCounter; } }
        public string TimeTooltipString { get { return mHudModel.TimeTooltipString; } }
        public int TotalLifetimePoints { get { return mHudModel.TotalLifetimePoints; } }
        public string ViewHomeButtonTooltip { get { return mHudModel.ViewHomeButtonTooltip; } }
        public WallMode WallMode { get { return mHudModel.WallMode; } set { mHudModel.WallMode = value; } }
        public bool CurrentHouseholdExists { get { return mHudModel.CurrentHouseholdExists; } }
        public string RandomStageName { get { return mHudModel.RandomStageName; } }

        public List<CollectionRowInfo> GetCollectingInfo(CollectionType type)
        {
            return mHudModel.GetCollectingInfo(type);
        }

        public int GetCollectionSkillGuid()
        {
            return mHudModel.GetCollectionSkillGuid();
        }

        public bool GetIsShareableFromObjectGuid(ObjectGuid guid)
        {
            return mHudModel.GetIsShareableFromObjectGuid(guid);
        }

        public bool HasActiveSimStartedCollections()
        {
            return mHudModel.HasActiveSimStartedCollections();
        }

        public void OnLunarUpdate()
        {
            mHudModel.OnLunarUpdate();
        }

        public List<IUIFestivalTicketReward> AllAvailableFestivalTicketRewards
        {
            get { return mHudModel.AllAvailableFestivalTicketRewards; }
        }

        public int CurrentFestivalTickets
        {
            get
            {
                return mHudModel.CurrentFestivalTickets;
            }
            set
            {
                mHudModel.CurrentFestivalTickets = value;
            }
        }

        public Season GetCurrentSeason()
        {
            return mHudModel.GetCurrentSeason();
        }

        public string GetSeasonDaysLeftString()
        {
            return mHudModel.GetSeasonDaysLeftString();
        }

        public string GetSeasonName(Season season, bool isLower)
        {
            return mHudModel.GetSeasonName(season, isLower);
        }

        public string GetTemperatureString()
        {
            return mHudModel.GetTemperatureString();
        }

        public SimDisplay.WaveTypes GetTemperatureWaveType()
        {
            return mHudModel.GetTemperatureWaveType();
        }

        public bool PurchaseFestivalTicketReward(IUIFestivalTicketReward reward, ref ObjectGuid guid)
        {
            return mHudModel.PurchaseFestivalTicketReward(reward, ref guid);
        }

        public Dictionary<int, Season> ThisWeeksHolidays
        {
            get { return mHudModel.ThisWeeksHolidays; }
        }


        public List<IDegreeEntry> DegreeHistoryEntries
        {
            get { return mHudModel.DegreeHistoryEntries; }
        }

        public List<Sims3.UI.Controller.HUD.UIAcademicCourseInfo> GetClassScheduleForActiveSim()
        {
            return mHudModel.GetClassScheduleForActiveSim();
        }

        public void GetCurrentSimInfluence(SimInfo info, ref int nerdLevel, ref int rebelLevel, ref int socialiteLevel)
        {
            mHudModel.GetCurrentSimInfluence(info, ref nerdLevel, ref rebelLevel, ref socialiteLevel);
        }

        public float GradeAPercent()
        {
            return mHudModel.GradeAPercent();
        }

        public float GradeBPercent()
        {
            return mHudModel.GradeBPercent();
        }

        public float GradeCPercent()
        {
            return mHudModel.GradeCPercent();
        }

        public float GradeDPercent()
        {
            return mHudModel.GradeDPercent();
        }

        public bool IsSocialGroupTrait(ITraitEntryInfo traitInfo)
        {
            return mHudModel.IsSocialGroupTrait(traitInfo);
        }

        public bool IsUniversityGraduateTrait(ITraitEntryInfo traitInfo)
        {
            return mHudModel.IsUniversityGraduateTrait(traitInfo);
        }

        public void ShowClassScheduleWindow()
        {
            mHudModel.ShowClassScheduleWindow();
        }

        public ITraitEntryInfo SimSocialGroupTrait
        {
            get { return mHudModel.SimSocialGroupTrait; }
        }

        public ITraitEntryInfo SimUniversityGraduateTrait
        {
            get { return mHudModel.SimUniversityGraduateTrait; }
        }

        public void UpdateCellPhoneIcon(PhoneType phoneType, int colorNum, bool isBroken)
        {
            mHudModel.UpdateCellPhoneIcon(phoneType, colorNum, isBroken);
        }

        public void CheckPassportForRecallFromBook(SimInfo info, PASSPORTSLOT slot, bool wasPaused)
        {
            mHudModel.CheckPassportForRecallFromBook(info, slot, wasPaused);
        }

        public bool CurrentSimScubaLotTest(Vector3 worldPos)
        {
            return mHudModel.CurrentSimScubaLotTest(worldPos);
        }

        public void RequestRefreshCurrentSimInfoSkewer()
        {
            mHudModel.RequestRefreshCurrentSimInfoSkewer();
        }

        public IBotCompetitionInteractionInstance GetCurrentBotCompetitionInteraction()
        {
            return mHudModel.GetCurrentBotCompetitionInteraction();
        }

        public string[,] GetPacesText(string location)
        {
            return mHudModel.GetPacesText(location);
        }

        public int GetRobotCurrentQualityLevelInt(ObjectGuid simGuid)
        {
            return mHudModel.GetRobotCurrentQualityLevelInt(simGuid);
        }

        public string GetRobotCurrentQualityLevelString(ObjectGuid simGuid)
        {
            return mHudModel.GetRobotCurrentQualityLevelString(simGuid);
        }

        public string GetRobotCurrentQualityLevelString(int botLevel)
        {
            return mHudModel.GetRobotCurrentQualityLevelString(botLevel);
        }

        public void HideBotCompetitionPanel()
        {
            mHudModel.HideBotCompetitionPanel();
        }

        public bool IsRobotMoodEnabled(ObjectGuid guid)
        {
            return mHudModel.IsRobotMoodEnabled(guid);
        }

        public bool IsRobotMoodEnabled(ulong SimID)
        {
            return mHudModel.IsRobotMoodEnabled(SimID);
        }

        public bool IsValidFutureWorldCareer()
        {
            return mHudModel.IsValidFutureWorldCareer();
        }

        public void MoveCameraToLot(ulong lotId)
        {
            mHudModel.MoveCameraToLot(lotId);
        }

        public void MoveCameraToLot(ObjectGuid objectGuid)
        {
            mHudModel.MoveCameraToLot(objectGuid);
        }

        public void PauseMusic()
        {
            mHudModel.PauseMusic();
        }

        public void RestartMusic()
        {
            mHudModel.RestartMusic();
        }

        public bool RobotCareerEnabled
        {
            get { return mHudModel.RobotCareerEnabled; }
        }

        public List<ITraitEntryInfo> RobotChips
        {
            get { return mHudModel.RobotChips; }
        }

        public bool RobotFunMotiveEnabled
        {
            get { return mHudModel.RobotFunMotiveEnabled; }
        }

        public bool RobotOpportunitiesEnabled
        {
            get { return mHudModel.RobotOpportunitiesEnabled; }
        }

        public bool RobotSkillsEnabled
        {
            get { return mHudModel.RobotSkillsEnabled; }
        }

        public bool RobotSocialMotiveEnabled
        {
            get { return mHudModel.RobotSocialMotiveEnabled; }
        }

        public bool ShouldShowTimeAlmanacButton()
        {
            return mHudModel.ShouldShowTimeAlmanacButton();
        }

        public void ShowBotCompetitionPanel()
        {
            mHudModel.ShowBotCompetitionPanel();
        }

        public void ShowTimeAlmanacDialog()
        {
            mHudModel.ShowTimeAlmanacDialog();
        }

        public void TriggerBotProgressBarGlow(IBotCompetitionInteractionInstance competitionInstance)
        {
            mHudModel.TriggerBotProgressBarGlow(competitionInstance);
        }

        public void UpdateBotCompetitionStanding(IBotCompetitionInteractionInstance competitionInstance)
        {
            mHudModel.UpdateBotCompetitionStanding(competitionInstance);
        }

        public bool isSelectedSimAnEP11Robot()
        {
            return mHudModel.isSelectedSimAnEP11Robot();
        }
    }
}
