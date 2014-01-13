using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.MapTags;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class HandleMapTagsScenario : SimEventScenario<Event>
    {
        public static ulong sCount = 1;

        MapTagManager mMTM = null;

        public HandleMapTagsScenario()
        { }
        public HandleMapTagsScenario(SimDescription sim)
            : base(sim)
        { }
        protected HandleMapTagsScenario(HandleMapTagsScenario scenario)
            : base (scenario)
        {
            mMTM = scenario.mMTM;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HandleSimMapTags";
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimInstantiated);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            List<SimDescription> sims = new List<SimDescription>();
            foreach (Sim sim in LotManager.Actors)
            {
                sims.Add(sim.SimDescription);
            }
            return sims;
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

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }

            return base.Allow(sim);
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

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return Perform(Main, mMTM, Sim.CreatedSim, true);
        }

        protected static bool ImmediateUpdate(Main manager)
        {
            Sim active = Sims3.Gameplay.Actors.Sim.ActiveActor;
            if (active == null) return false;

            MapTagManager mtm = active.MapTagManager;
            if (mtm == null) return false;

            HandleMapTagsScenario scenario = new HandleMapTagsScenario();
            scenario.Manager = manager;

            foreach (Sim sim in LotManager.Actors)
            {
                if (!scenario.Allow(sim.SimDescription)) continue;

                Perform(manager, mtm, sim, true);
            }

            return true;
        }

        protected static bool Perform(Main manager, MapTagManager mtm, Sim sim, bool force)
        {
            try
            {
                if (sim == null) return false;

                MapTag tag = mtm.GetTag(sim);

                if ((sim.Household == Household.ActiveHousehold) || 
                    (sim.SimDescription.AssignedRole is RoleSpecialMerchant) ||
                    (sim.SimDescription.AssignedRole is Proprietor) ||
                    (manager.GetOption<TagLevelOption>().Matches(sim.SimDescription)))
                {
                    if (((tag is NPCSimMapTag) || (tag is SelectedSimMapTag)) || (tag is FamilySimMapTag))
                    {
                        mtm.RemoveTag(tag);
                    }
                    else if ((force) && (tag is TrackedSim))
                    {
                        mtm.RemoveTag(tag);
                    }

                    if (!mtm.HasTag(sim))
                    {
                        mtm.AddTag(new TrackedSim(sim, mtm.Actor));
                        return true;
                    }
                }
                else if (tag is TrackedSim)
                {
                    mtm.RemoveTag(tag);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Common.DebugException(sim, exception);
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new HandleMapTagsScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim, HandleMapTagsScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerSim main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                MapTagController.kMaxNumMaptagsToDisplay = 5000;

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

        public class EventOption : BooleanEventOptionItem<ManagerSim, HandleMapTagsScenario>, IDebuggingOption
        {
            public EventOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HandleMapTagsEvent";
            }
        }

        public class PersonalityTagOption : BooleanManagerOptionItem<ManagerPersonality>
        {
            public PersonalityTagOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowMapTags";
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

        public class ColorOption : BooleanManagerOptionItem<ManagerSim>
        {
            public ColorOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ColoredMapTags";
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

        public class ColorByAgeOption : BooleanManagerOptionItem<ManagerSim>
        {
            public ColorByAgeOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ColorByAgeTags";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<TagLevelOption, Managers.Manager.AlertLevel>() == Managers.Manager.AlertLevel.None) return false;

                if (!Manager.GetValue<ColorOption, bool>()) return false;

                return base.ShouldDisplay();
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                ImmediateUpdate(Manager.Main);
                return true;
            }
        }

        public class TagLevelOption : Manager.AlertLevelOption<ManagerSim>, IAfterImportOptionItem
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
                
                return "SimSelected";
            }

            protected override bool Progressed
            {
                get { return false; }
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                ImmediateUpdate(Manager.Main);
                return true;
            }

            public override Scenario Clone()
            {
                return new SimSelectedScenario(this);
            }
        }
    }
}
