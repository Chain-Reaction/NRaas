using System;
using System.Collections.Generic;
using System.Text;
using Sims3.SimIFace;
using Sims3.Gameplay.EventSystem;
using Sims3.UI;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Abstracts;

[assembly: Tunable]
namespace NoFreeWorkout
{
    public class WorkOutCostsMoney
    {
        [Tunable]
        protected static bool liikuntaNuuttis;

        [Tunable]
        protected static bool ShowNotifications;

        static WorkOutCostsMoney()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(World_OnWorldLoadFinishedEventHandler);
        }

        static void World_OnWorldLoadFinishedEventHandler(object sender, EventArgs e)
        {
            EventTracker.AddListener(EventTypeId.kSwimming, new ProcessEventDelegate(WorkOutCostsMoney.Swimming));
            EventTracker.AddListener(EventTypeId.kStoppedWorkingOut, new ProcessEventDelegate(WorkOutCostsMoney.StoppedWorkingOut));
        }

        protected static ListenerAction Swimming(Event e)
        {
            Sim sim = e.Actor as Sim;
            if (sim != null && sim.LotCurrent != null)
            {
                //Only pay if you are at gym or pool lot type
                if (sim.LotCurrent.CommercialLotSubType == CommercialLotSubType.kGym || sim.LotCurrent.CommercialLotSubType == CommercialLotSubType.kPool)
                    CommonMethods.PayForWorkOut(sim, sim.LotCurrent, CommonMethods.ReturnFee(CommonMethods.WorkOut.Swim));
            }
            return ListenerAction.Keep;
        }

        protected static ListenerAction StoppedWorkingOut(Event e)
        {
            Sim sim = e.Actor as Sim;
            if (sim != null)
            {
                //Treadmill tm = e.TargetObject as Treadmill;
                //WorkoutBench wb = e.TargetObject as WorkoutBench;
                //AthleticGameObject s = e.TargetObject as AthleticGameObject;

                //if (tm != null || wb != null || s != null)
                AthleticGameObject ao = e.TargetObject as AthleticGameObject;

                if (ao != null && sim.LotCurrent != null)
                {
                    //Only pay if you are at gym or pool lot type
                    if (sim.LotCurrent.CommercialLotSubType == CommercialLotSubType.kGym || sim.LotCurrent.CommercialLotSubType == CommercialLotSubType.kPool)
                        CommonMethods.PayForWorkOut(sim, sim.LotCurrent, CommonMethods.ReturnFee(CommonMethods.WorkOut.WorkOut));
                }
            }

            return ListenerAction.Keep;
        }
    }
}
