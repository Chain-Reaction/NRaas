using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.MapTags;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.SimDataElement;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class HandleMapTagsScenario : LotScenario
    {
        public static ulong sCount = 1;

        MapTagManager mMTM = null;

        public HandleMapTagsScenario()
        { }
        protected HandleMapTagsScenario(HandleMapTagsScenario scenario)
            : base (scenario)
        {
            mMTM = scenario.mMTM;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HandleLotMapTags";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            if (initialPass) return true;

            return base.Allow(fullUpdate, initialPass);
        }

        protected override bool Allow()
        {
            if (GetValue<TagLevelOption, Managers.Manager.AlertLevel>() == Managers.Manager.AlertLevel.None) return false;

            Sim active = Sims3.Gameplay.Actors.Sim.ActiveActor;
            if (active == null) return false;

            if (active.MapTagManager == null) return false;

            if (!CameraController.IsMapViewModeEnabled()) return false;

            return base.Allow();
        }

        protected override bool Allow(Lot lot)
        {
            if (lot.Household == null)
            {
                IncStat("Empty");
                return false;
            }
            else if (lot.ObjectId == ObjectGuid.InvalidObjectGuid)
            {
                IncStat("No GUID");
                return false;
            }

            return base.Allow(lot);
        }

        protected override Scenario.GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            sCount++;

            Sim active = Sims3.Gameplay.Actors.Sim.ActiveActor;
            if (active == null) return GatherResult.Failure;

            mMTM = active.MapTagManager;
            if (mMTM == null) return GatherResult.Failure;

            return base.Gather(list, ref continueChance, ref maximum, ref random);
        }

        protected static bool ImmediateUpdate(Main manager)
        {
            Sim active = Sims3.Gameplay.Actors.Sim.ActiveActor;
            if (active == null) return false;

            MapTagManager mtm = active.MapTagManager;
            if (mtm == null) return false;

            HandleMapTagsScenario scenario = new HandleMapTagsScenario();
            scenario.Manager = manager;

            foreach (Lot lot in LotManager.AllLots)
            {
                if (!scenario.Allow(lot)) continue;

                Perform(manager, mtm, lot);
            }

            return true;
        }

        protected static bool Perform(Main manager, MapTagManager mtm, Lot lot)
        {
            try
            {
                MapTag tag = mtm.GetTag(lot);

                if (manager.GetOption<TagLevelOption>().Matches(lot.Household.AllSimDescriptions))
                {
                    if ((tag != null) && (!(tag is TrackedLot)) && (!(tag is HomeLotMapTag)))
                    {
                        mtm.RemoveTag(tag);
                    }
                    if (!mtm.HasTag(lot))
                    {
                        mtm.AddTag(new TrackedLot(lot, mtm.Actor));
                        return true;
                    }
                }
                else if (tag is TrackedLot)
                {
                    mtm.RemoveTag(tag);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Common.DebugException(lot, exception);
            }

            return false;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return Perform(Main, mMTM, Lot);
        }

        public override Scenario Clone()
        {
            return new HandleMapTagsScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerLot, HandleMapTagsScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerLot main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                Manager.AddListener(new SimSelectedScenario());

                if (initial)
                {
                    CameraController.OnCameraMapViewEnabledCallback += OnMapView;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "HandleMapTags";
            }

            protected static void OnMapView(bool enabled)
            {
                if (enabled)
                {
                    if (StoryProgression.Main == null) return;

                    ImmediateUpdate(StoryProgression.Main);
                }
            }
        }

        public class ColorOption : BooleanManagerOptionItem<ManagerLot>
        {
            public ColorOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ColoredMapTags";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<TagLevelOption, Managers.Manager.AlertLevel>() == Managers.Manager.AlertLevel.None) return false;

                return base.ShouldDisplay();
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                ImmediateUpdate(Manager.Main);
                return true;
            }
        }

        public class TagLevelOption : Manager.AlertLevelOption<ManagerLot>, IAfterImportOptionItem
        {
            public TagLevelOption()
                : base(Managers.Manager.AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowMapTags";
            }

            public override bool UpdatesOnAll()
            {
                return false;
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                if ((Value != Managers.Manager.AlertLevel.None) && (Common.AssemblyCheck.IsInstalled("Awesome")))
                {
                    SimpleMessageDialog.Show(StoryProgression.Localize("Conflict:Title"), Localize("Warning"));
                }

                ImmediateUpdate(Manager.Main);
                return true;
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public void PerformAfterImport()
            {
                ImmediateUpdate(StoryProgression.Main);
            }
        }

        public class FamilyMovedScenario : ScheduledSoloScenario, IEventScenario
        {
            public FamilyMovedScenario()
            { }
            protected FamilyMovedScenario(FamilyMovedScenario scenario)
                : base(scenario)
            { }

            public bool SetupListener(IEventHandler events)
            {
                return events.AddListener(this, EventTypeId.kLotChosenForActiveHousehold);
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "MapTagsFamilyMoved";
            }

            protected override bool Progressed
            {
                get { return false; }
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                ImmediateUpdate(Main);
                return true;
            }

            public override Scenario Clone()
            {
                return new FamilyMovedScenario(this);
            }
        }

        public class SimSelectedScenario : ScheduledSoloScenario, IEventScenario
        {
            public SimSelectedScenario()
            { }
            protected SimSelectedScenario(SimSelectedScenario scenario)
                : base(scenario)
            { }

            public bool SetupListener(IEventHandler events)
            {
                return events.AddListener(this, EventTypeId.kEventSimSelected);
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "MapTagsSimSelected";
            }

            protected override bool Progressed
            {
                get { return false; }
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                ImmediateUpdate(Main);
                return true;
            }

            public override Scenario Clone()
            {
                return new SimSelectedScenario(this);
            }
        }
    }
}
