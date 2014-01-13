using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ComputerSpace.Town
{
    public class BootAllGnomes : TownInteraction
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();

        // Nested Types
        [DoesntRequireTuning]
        private sealed class Definition : TownInteraction.Definition<BootAllGnomes>
        {
            public override string GetInteractionName(Sim a, GameObject target, InteractionObjectPair interaction)
            {
                return Localize("BootAllGnomes:MenuName");
            }
        }

        public override bool Run()
        {
            Perform();
            return true;
        }

        public static void PopulateActions()
        {
            if (MagicGnome.sActionChances == null)
            {
                List<MagicGnome.MagicGnomeWeightedPlan> list = new List<MagicGnome.MagicGnomeWeightedPlan>();
                foreach (MagicGnome.MagicGnomePlan plan in Enum.GetValues(typeof(MagicGnome.MagicGnomePlan)))
                {
                    list.Add(new MagicGnome.MagicGnomeWeightedPlan(plan));
                }
                MagicGnome.sActionChances = list.ToArray();
            }

            if (MagicGnome.sMagicGnomePoses == null)
            {
                MagicGnome.sMagicGnomePoses = new List<MagicGnome.MagicGnomePose>();
                foreach (MagicGnome.MagicGnomePose pose in Enum.GetValues(typeof(MagicGnome.MagicGnomePose)))
                {
                    MagicGnome.sMagicGnomePoses.Add(pose);
                }
            }
        }

        public static void Perform()
        {
            try
            {
                int count = 0;

                List<MagicGnome> televisions = new List<MagicGnome>(Sims3.Gameplay.Queries.GetObjects<MagicGnome>());
                foreach (MagicGnome obj in televisions)
                {
                    if (obj.mTrickeryAlarm != AlarmHandle.kInvalidHandle)
                    {
                        AlarmManager.Global.RemoveAlarm(obj.mTrickeryAlarm);
                    }

                    obj.mTrickeryAlarm = AlarmManager.Global.AddAlarmDay(0f, ~DaysOfTheWeek.None, new AlarmTimerCallback(obj.EnactTrickery), "Magic gnome trickery alarm", AlarmType.AlwaysPersisted, obj);

                    obj.mPreviousTrickeryDayOfTheWeek = DaysOfTheWeek.None;

                    obj.EnactTrickery ();

                    count++;
                }

                if (count > 0)
                {
                    Notify(Localize("BootAllGnomes:Success", new object[] { count }));
                }
            }
            catch (Exception exception)
            {
                Exception(exception);
            }
        }
    }
}
