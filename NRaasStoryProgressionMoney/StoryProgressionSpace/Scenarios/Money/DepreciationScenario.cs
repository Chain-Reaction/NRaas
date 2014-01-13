using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Locations;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class DepreciationScenario : LotScenario, IAlarmScenario
    {
        public DepreciationScenario()
            : base ()
        { }
        protected DepreciationScenario(DepreciationScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Depreciation";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 100; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, 0f, ~DaysOfTheWeek.None);
        }

        protected override bool Allow(Lot lot)
        {
            if (lot.LotType != LotType.Residential)
            {
                IncStat("Not Residential");
                return false;
            }
            else if ((!GetValue<Option, bool>()) && ((Sim.ActiveActor == null) || (lot != Sim.ActiveActor.LotHome)))
            {
                IncStat("Inactive Denied");
                return false;
            }

            Household house = lot.Household;
            if (house != null)
            {
                if (SimTypes.IsSpecial(house))
                {
                    IncStat("Special");
                    return false;
                }
                else if (!Money.Allow(this, SimTypes.HeadOfFamily(house)))
                {
                    IncStat("Money Denied");
                    return false;
                }
            }

            return base.Allow(lot);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int count = 0;

            foreach (GameObject obj in Lot.GetObjects<GameObject>())
            {
                try
                {
                    if (obj.HasBeenDestroyed) continue;

                    int value = obj.Value;

                    obj.AppreciateDepreciateDailyUpdate();

                    AddScoring("", obj.Value - value);

                    count++;

                    if (count > 250)
                    {
                        SpeedTrap.Sleep();
                        count = 0;
                    }
                }
                catch (Exception e)
                {
                    Common.DebugException(obj, e);
                }
            }

            Household house = Lot.Household;
            if (house != null)
            {
                foreach (Sim sim in HouseholdsEx.AllSims(house))
                {
                    if (!Inventories.VerifyInventory(sim.SimDescription)) continue;

                    sim.Inventory.AppreciateDepreciateInventoryItems();
                }
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new DepreciationScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerMoney, DepreciationScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                if (GameObject.sAppreciationDepreciationAlarm != AlarmHandle.kInvalidHandle)
                {
                    AlarmManager.Global.RemoveAlarm(GameObject.sAppreciationDepreciationAlarm);
                    GameObject.sAppreciationDepreciationAlarm = AlarmHandle.kInvalidHandle;

                    Manager.IncStat("EA Depreciation Disabled");
                }

                base.PrivateUpdate(fullUpdate, initialPass);
            }

            public override string GetTitlePrefix()
            {
                return "Depreciation";
            }
        }
    }
}
