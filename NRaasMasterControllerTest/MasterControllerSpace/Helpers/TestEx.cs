using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public class TestEx : Common.IPreLoad, Common.IWorldLoadFinished
    {
        public void OnPreLoad()
        {
            //new Common.ImmediateEventListener(EventTypeId.kSimDescriptionDisposed, OnDisposed);

            //new Common.ImmediateEventListener(EventTypeId.kCareerDataChanged, OnChanged);
            //new Common.ImmediateEventListener(EventTypeId.kCareerQuitJob, OnQuit);

            //OpportunityTrackerModel.Singleton.OpportunitiesChanged += OnOppChanged;

            //InWorldState.InWorldSubStateChanging += OnInWorldSubStateChanging;
        }

        public void OnWorldLoadFinished()
        {
            //Household.ActiveHousehold.HouseholdSimsChanged += OnHouseholdSimChanged;

            //new Common.ImmediateEventListener(EventTypeId.kSimDescriptionDisposed, OnDisposed);

            //new Common.ImmediateEventListener(EventTypeId.kCareerDataChanged, OnChanged);
            //new Common.ImmediateEventListener(EventTypeId.kCareerQuitJob, OnQuit);

            //new Common.ImmediateEventListener(EventTypeId.kSimCompletedOccupationTask, OnSimCompletedOccupationTask);
            //new Common.ImmediateEventListener(EventTypeId.kFirefighterPutOutFire, OnFirefighterPutOutFire);

            //new Common.ImmediateEventListener(EventTypeId.kSimInstantiated, OnInstantiated);
        }

        public void OnInWorldSubStateChanging(InWorldState.SubState previousState, InWorldState.SubState newState)
        {
            Common.StackLog(new Common.StringBuilder("OnInWorldSubStateChanging\n" + previousState + "\n" + newState));
        }

        public void OnSimCompletedOccupationTask(Event e)
        {
            Common.StackLog(new Common.StringBuilder("OnSimCompletedOccupationTask"));
        }

        public void OnFirefighterPutOutFire(Event e)
        {
            Common.StackLog(new Common.StringBuilder("OnFirefighterPutOutFire"));
        }

        public void OnOppChanged()
        {
            Common.StackLog(new Common.StringBuilder("OnOppChanged"));
        }

        public void OnChanged(Event e)
        {
            Common.StackLog(new Common.StringBuilder("kCareerDataChanged"));
        }

        public void OnQuit(Event e)
        {
            Common.StackLog(new Common.StringBuilder("kCareerQuitJob"));
        }

        private void OnHouseholdSimChanged(Sims3.Gameplay.CAS.HouseholdEvent householdEvent, IActor actor, Household oldHousehold)
        {
            Common.StringBuilder msg = new Common.StringBuilder("OnHouseholdSimChanged");

            try
            {
                HudModel ths = Sims3.UI.Responder.Instance.HudModel as HudModel;

                if (ths.HouseholdChanged != null)
                {
                    msg += "A";

                    if (householdEvent != Sims3.Gameplay.CAS.HouseholdEvent.kNone)
                    {
                        msg += "B";

                        ths.ClearSimList();
                        if ((ths.mCurrentHousehold != null) && (ths.mCurrentHousehold.CurrentMembers != null))
                        {
                            msg += "C";

                            int count = ths.mCurrentHousehold.CurrentMembers.Count;
                            for (int i = 0x0; i < count; i++)
                            {
                                msg += "D";

                                Household.Member member = ths.mCurrentHousehold.CurrentMembers[i];
                                SimInfo item = ths.CreateSimInfo(member.mSimDescription);
                                if (item != null)
                                {
                                    msg += "E";

                                    ths.mSimList.Add(item);
                                }
                            }
                            ths.mSimList.Sort();
                        }
                    }

                    /*
                    switch (householdEvent)
                    {
                        case Sims3.Gameplay.CAS.HouseholdEvent.kSimAdded:
                            ths.HouseholdChanged(HouseholdEvent.SimAdded, actor.ObjectId);
                            break;

                        case Sims3.Gameplay.CAS.HouseholdEvent.kSimRemoved:
                            ths.HouseholdChanged(HouseholdEvent.SimRemoved, actor.ObjectId);
                            break;
                    }
                    */
                }
                ths.UpdateHouseholdThumb();
            }
            catch (Exception e)
            {
                Common.Exception(msg, e);
            }
            finally
            {
                Common.DebugNotify(msg);
            }
        }

        protected static void OnInstantiated(Event e)
        {
            Sim target = e.TargetObject as Sim;
            if (target != null)
            {
                //if (target.Name.Contains("Vu"))
                {
                    Common.DebugStackLog(target.Name);
                }
            }
        }

        protected static void OnDisposed(Event e)
        {
            SimDescriptionEvent dEvent = e as SimDescriptionEvent;
            if (dEvent != null)
            {
                Common.DebugStackLog(dEvent.SimDescription.FullName);
            }
        }
    }
}
