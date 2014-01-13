using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
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
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class HeatingUpScenario : SimBuffScenario, IAlarmScenario
    {
        bool mForceHome;

        public HeatingUpScenario()
        { }
        public HeatingUpScenario(SimDescription sim, bool forceHome)
            : base(sim)
        {
            mForceHome = forceHome;
        }
        protected HeatingUpScenario(HeatingUpScenario scenario)
            : base (scenario)
        {
            mForceHome = scenario.mForceHome;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HeatingUp";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarm(this, 1);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(BuffNames buff)
        {
            return (buff == BuffNames.HeatingUp);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!sim.CreatedSim.IsOutside)
            {
                IncStat("Inside");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situation Denied");
                return false;
            }

            return base.Allow(sim);
        }

        public static Vector3 GetDestination(Lot lot, Vector3 source)
        {
            if (lot != null)
            {
                Door door = lot.FindOutsideDoor(source);
                if (door != null)
                {
                    int roomId = door.GetRoomIdOfDoorSide(CommonDoor.tSide.Front);
                    if (roomId == 0)
                    {
                        roomId = door.GetRoomIdOfDoorSide(CommonDoor.tSide.Back);
                    }

                    if (roomId != 0)
                    {
                        List<GameObject> objects = lot.GetObjectsInRoom<GameObject>(roomId);
                        if (objects.Count > 0)
                        {
                            return RandomUtil.GetRandomObjectFromList(objects).Position;
                        }
                    }
                }
            }

            return Vector3.Empty;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mForceHome)
            {
                Lot current = Sim.CreatedSim.LotCurrent;

                if ((!current.IsResidentialLot) || (Sim.CreatedSim.IsGreetedOnLot(current)))
                {
                    if (Situations.PushGoHere(this, Sim, GetDestination(current, Sim.CreatedSim.Position)))
                    {
                        Friends.AddAlarm(new HeatingUpScenario(Sim, true));
                        return true;
                    }
                }
            }

            Situations.PushGoHome(this, Sim);

            if (Situations.PushGoHere(this, Sim, GetDestination(Sim.LotHome, Sim.CreatedSim.Position)))
            {
                return true;
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new HeatingUpScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSituation, HeatingUpScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HeatingUp";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }
        }
    }
}
