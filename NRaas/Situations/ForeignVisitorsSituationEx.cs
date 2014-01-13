using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Situations
{
    public class ForeignVisitorsSituationEx : ForeignVisitorsSituation
    {
        public ForeignVisitorsSituationEx()
        { } 
        public ForeignVisitorsSituationEx(Lot lot, Sim host, List<MiniSimDescription> guests, DateAndTime startTime)
            : base(lot, host, guests, startTime)
        {
            SetState(new WaitForPreparationsEx(this));
        }

        public class WaitForPreparationsEx : WaitForPreparations
        {
            protected WaitForPreparationsEx()
            { }
            public WaitForPreparationsEx(ForeignVisitorsSituation parent)
                : base(parent)
            { }

            public override void Init(ForeignVisitorsSituation parent)
            {
                try
                {
                    base.Init(parent);

                    AlarmManager.RemoveAlarm(mAlarmHandle);

                    float time = SimClock.ElapsedTime(TimeUnit.Hours, SimClock.CurrentTime(), Parent.StartTime) - parent.GetParams().PreparationTime;
                    if (time <= 0)
                    {
                        Parent.SetState(new PrepareEx(Parent));
                    }
                    else
                    {
                        mAlarmHandle = AlarmManager.AddAlarm(time, TimeUnit.Hours, TimeToPrepareEx, "Prepare Foreign Visit", AlarmType.AlwaysPersisted, Parent.Host);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception("WaitForPreparationsEx:Init", e);
                }
            }

            private void TimeToPrepareEx()
            {
                Parent.SetState(new PrepareEx(Parent));
            }
        }

        public class PrepareEx : Prepare
        {
            protected PrepareEx()
            { }
            public PrepareEx(ForeignVisitorsSituation parent)
                : base(parent)
            { }

            public override void Init(ForeignVisitorsSituation parent)
            {
                try
                {
                    if (parent.Host.IsSelectable)
                    {
                        StyledNotification.Show(new StyledNotification.Format(Localization.LocalizeString("Gameplay/Situations/InviteForeignVisitors:GoPrepare", new object[] { parent.Host }), StyledNotification.NotificationStyle.kGameMessagePositive));
                    }

                    parent.Host.Motives.CreateMotive(parent.PreparationMotive());
                    parent.OnPreparation();

                    float time = SimClock.ElapsedTime(TimeUnit.Hours, SimClock.CurrentTime(), Parent.StartTime) - parent.GetParams().HoursBeforePartyToInvite;
                    if (time <= 0f)
                    {
                        Parent.SetState(new TryInviteEx(Parent));
                    }
                    else
                    {
                        mAlarmGuestInvite = AlarmManager.AddAlarm(time, TimeUnit.Hours, TimeToInviteGuestsEx, "Invite Guests To Foreign Visitors Situation", AlarmType.AlwaysPersisted, Parent.Host);
                        AlarmManager.AlarmWillYield(mAlarmGuestInvite);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception("PrepareEx:Init", e);
                }
            }

            private void TimeToInviteGuestsEx()
            {
                Parent.SetState(new TryInviteEx(Parent));
            }
        }

        public class TryInviteEx : TryInvite
        {
            protected TryInviteEx()
            { }
            public TryInviteEx(ForeignVisitorsSituation parent)
                : base(parent)
            { }

            public override void Init(ForeignVisitorsSituation parent)
            {
                mAlarmRetryInviteGuests = AlarmManager.AddAlarm(0f, TimeUnit.Seconds, InviteGuestsIfHostAtHomeEx, "Invite foreign visitors", AlarmType.AlwaysPersisted, Parent.Host);
                AlarmManager.AlarmWillYield(mAlarmRetryInviteGuests);
            }

            private bool UnpackForeignVisitors(ForeignVisitorsSituation ths)
            {
                foreach (MiniSimDescription description in ths.GuestMiniDescriptions)
                {
                    SimDescription simDescription = MiniSims.ImportWithCheck(description);
                    if (simDescription == null) continue;

                    ths.GuestDescriptions.Add(simDescription);
                }

                return (ths.GuestDescriptions.Count > 0x0);
            }

            private bool InviteGuestsEx()
            {
                if (!Parent.Host.IsSelectable)
                {
                    Common.DebugNotify("Not IsSelectable");
                    return false;
                }
                if (!UnpackForeignVisitors(Parent))
                {
                    Common.DebugNotify("Not UnpackForeignVisitors");
                    return false;
                }

                Sim host = Parent.Host;
                foreach (SimDescription description in Parent.GuestDescriptions)
                {
                    if (host != null)
                    {
                        Sim createdSim = description.CreatedSim;
                        if (createdSim == null)
                        {
                            createdSim = Instantiation.PerformOffLot(description, host.LotHome, null);
                        }

                        if (createdSim != null)
                        {
                            Parent.Guests.Add(createdSim);
                            createdSim.AssignRole(Parent);
                        }
                    }
                }

                foreach (Sim sim in Parent.Guests)
                {
                    if (sim.LotCurrent != Parent.Lot)
                    {
                        Parent.PushComeToLot(sim);
                    }
                    else
                    {
                        Parent.OnRouteToLotSucceeded(sim, 0f);
                    }
                }

                return true;
            }

            private void InviteGuestsIfHostAtHomeEx()
            {
                try
                {
                    bool failure = false;

                    if ((!Common.kDebugging) && (!IsAnyHostAtHome()))
                    {
                        failure = true;
                    }
                    else if (!InviteGuestsEx())
                    {
                        failure = true;
                    }

                    if ((failure) && (SimClock.HoursPassedOfDay < 23f))
                    {
                        mAlarmRetryInviteGuests = AlarmManager.AddAlarm(ForeignVisitorsSituation.kRetryInviteGuestsNoHostsHomeFrequency, TimeUnit.Minutes, InviteGuestsIfHostAtHomeEx, "Retry invite foreign visitors", AlarmType.AlwaysPersisted, Parent.Host);
                        AlarmManager.AlarmWillYield(mAlarmRetryInviteGuests);
                    }
                    else
                    {
                        Parent.SetState(new FailedToStart(Parent));
                    }
                }
                catch (Exception e)
                {
                    Common.Exception("InviteGuestsIfHostAtHomeEx", e);
                }
            }
        }
    }
}
