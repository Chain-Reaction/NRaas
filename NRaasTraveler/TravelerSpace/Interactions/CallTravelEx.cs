using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace.Interactions
{
    public class CallTravelEx : Phone.CallTravel, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Phone, Phone.CallTravel.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.CallTravel.Definition>(Singleton);
        }

        public override Phone.Call.ConversationBehavior OnCallConnected()
        {
            try
            {
                if ((!IntroTutorial.IsRunning || IntroTutorial.AreYouExitingTutorial()) && Helpers.TravelUtilEx.ShowTravelToVacationWorldDialog(Actor, out mResult))
                {
                    return Phone.Call.ConversationBehavior.TalkBriefly;
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return Phone.Call.ConversationBehavior.JustHangUp;
        }

        public new class Definition : Phone.CallTravel.Definition
        {
            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CallTravelEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Phone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP1))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("EP1 Fail");
                    return false;
                }

                if (!target.IsUsableBy(a))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not Usable");
                    return false;
                }

                return Helpers.TravelUtilEx.CanSimTriggerTravelToVacationWorld(a, true, ref greyedOutTooltipCallback);
            }
        }
    }
}
