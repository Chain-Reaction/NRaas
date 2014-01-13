using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class WorkOutfitsScenario : SimEventScenario<Event>, IAlarmScenario
    {
        public WorkOutfitsScenario()
            : base ()
        { }
        protected WorkOutfitsScenario(WorkOutfitsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "WorkOutfits";
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kRoomChanged);
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDelayed(this, 1f, TimeUnit.Hours);
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            if (!GetValue<ForceChangeOption, bool>()) return null;

            return base.Handle(e, ref result);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            if (initialPass) return false;

            return true;
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.Employed;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.CreatedByService != null)
            {
                IncStat("Service");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.AssignedRole != null)
            {
                IncStat("Role");
                return false;
            }
            else if (!(sim.Occupation is Career))
            {
                IncStat("Not Career");
                return false;
            }

            try
            {
                if (sim.CreatedSim.CurrentOutfitCategory != Sims3.SimIFace.CAS.OutfitCategories.Career)
                {
                    IncStat("Not Career");
                    return false;
                }
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);

                IncStat("Exception");
                return false;
            }

            DateAndTime queryTime = SimClock.CurrentTime();
            queryTime.Ticks += SimClock.ConvertToTicks(2f, TimeUnit.Hours);
            if (sim.Occupation.IsWorkHour(queryTime))
            {
                IncStat("Too Close");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            try
            {
                if ((GetValue<ForceChangeOption, bool>()) || (AddScoring("ChangeWorkClothes", Sim) > 0))
                {
                    ManagerSim.ChangeOutfit(Manager, Sim, OutfitCategories.Everyday);
                    return true;
                }
            }
            catch (Exception e)
            {
                Common.DebugException(Sim, e);
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new WorkOutfitsScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerCareer, WorkOutfitsScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ChangeWorkClothes";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class ForceChangeOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public ForceChangeOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ForceChangeWorkClothes";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
