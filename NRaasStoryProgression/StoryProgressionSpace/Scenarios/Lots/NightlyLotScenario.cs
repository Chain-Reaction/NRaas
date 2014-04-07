using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class NightlyLotScenario : LotScenario, IAlarmScenario
    {
        public NightlyLotScenario()
        { }
        protected NightlyLotScenario(NightlyLotScenario scenario)
            : base (scenario)
        { }

        public static Common.MethodStore sGetStuckCheckEnabled = new Common.MethodStore("NRaasOverwatch", "NRaas.Overwatch", "GetStuckCheckEnable", new Type[] { typeof(bool) });

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "NightlyLot";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, LotManager.kVenueCleanupTime, ~DaysOfTheWeek.None);
        }

        public static event UpdateDelegate OnAdditionalScenario;

        protected override Scenario.GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            if (Lot == null)
            {
                MetaAutonomyManager.DecideTodaysHotSpotsAndDeadZones();

                if (GetValue<StuckCheckOption, bool>() && ((!sGetStuckCheckEnabled.Valid) || (sGetStuckCheckEnabled.Valid && !sGetStuckCheckEnabled.Invoke<bool>(new object[] { false }))))
                {
                    foreach (Sim sim in LotManager.Actors)
                    {
                        LotManager.RunStuckCheckHelper.Init(sim);
                    }
                }
            }

            return base.Gather(list, ref continueChance, ref maximum, ref random);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new CleanupLotScenario(Lot), ScenarioResult.Start);
            Add(frame, new ReplaceItemsScenario(Lot), ScenarioResult.Start);

            if (OnAdditionalScenario != null)
            {
                OnAdditionalScenario(this, frame);
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new NightlyLotScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerLot, NightlyLotScenario>, IDebuggingOption
        {
            bool mCleanupInventoryEnabled = false;

            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CleanupLots";
            }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                if ((!mCleanupInventoryEnabled) && (LotManager.sVenueMaintenanceAlarm != AlarmHandle.kInvalidHandle))
                {
                    mCleanupInventoryEnabled = true;

                    AlarmManager.Global.RemoveAlarm(LotManager.sVenueMaintenanceAlarm);
                    LotManager.sVenueMaintenanceAlarm = AlarmHandle.kInvalidHandle;
                }

                base.PrivateUpdate(fullUpdate, initialPass);
            }
        }

        // Note this is overwritten if Overwatch is installed and StuckCheck is enabled in that mod
        public class StuckCheckOption : BooleanManagerOptionItem<ManagerLot>, IDebuggingOption
        {
            public StuckCheckOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CleanupLotStuckCheck";
            }
        }
    }
}
