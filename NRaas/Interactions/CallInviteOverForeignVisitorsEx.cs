using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Situations;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Interactions
{
    public class CallInviteOverForeignVisitorsEx : Phone.CallInviteOverForeignVisitors, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Phone, Phone.CallInviteOverForeignVisitors.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.CallInviteOverForeignVisitors.Definition>(Singleton);
        }

        public override Phone.Call.DialBehavior GetDialBehavior()
        {
            try
            {
                MiniSimDescription receiver = (InteractionDefinition as Definition).Receiver as MiniSimDescription;
                if (receiver != null)
                {
                    if (mSimsToCall == null)
                    {
                        mSimsToCall = new List<MiniSimDescription>();
                    }
                    else
                    {
                        mSimsToCall.Clear();
                    }
                    mSimsToCall.Add(receiver);
                }

                float threshold = Phone.kLTRThresholdForInviteOverForeignVisitorMin;
                if (Common.kDebugging)
                {
                    threshold = -101;
                }

                return ChooseForeignSimsForCall("InviteOverForeignVisitors", threshold, ref mSimsToCall);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return DialBehavior.DoNotPick;
        }

        public override Phone.Call.ConversationBehavior OnCallConnected()
        {
            try
            {
                if ((Actor.LotHome == null) || !Actor.IsSelectable)
                {
                    return Phone.Call.ConversationBehavior.JustHangUp;
                }

                float arrivalTime = Phone.kForeignVisitorArrivalTime;

                DateAndTime startTime = SimClock.CurrentTime();

                if (!Common.kDebugging)
                {
                    float hoursPassedOfDay = SimClock.HoursPassedOfDay;
                    float num2 = 24f - (hoursPassedOfDay - arrivalTime);
                    if (num2 < 1f)
                    {
                        num2 += 24f;
                    }

                    long ticks = SimClock.ConvertToTicks(num2, TimeUnit.Hours);
                    startTime += new DateAndTime(ticks);
                }

                SimpleMessageDialog.Show(Common.LocalizeEAString("Gameplay/Situations/Party:InviteForeignVisitorsTitle"), Common.LocalizeEAString(false, "Gameplay/Situations/Party:InviteForeignVisitorsStart", new object[] { arrivalTime }), ModalDialog.PauseMode.PauseSimulator);

                new ForeignVisitorsSituationEx(Actor.LotHome, Actor, new List<MiniSimDescription>(mSimsToCall), startTime);

                return Phone.Call.ConversationBehavior.ExpressSatisfaction;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return ConversationBehavior.JustHangUp;
        }

        public new class Definition : Phone.CallInviteOverForeignVisitors.Definition
        {
            public Definition()
            { }
            public Definition(IMiniSimDescription receiver)
                : base(receiver)
            { }

            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CallInviteOverForeignVisitorsEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
