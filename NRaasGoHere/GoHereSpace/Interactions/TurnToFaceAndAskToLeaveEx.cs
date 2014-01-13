using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class TurnToFaceAndAskToLeaveEx : Sim.TurnToFaceAndAskToLeave, Common.IPreLoad//, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.TurnToFaceAndAskToLeave.Definition,Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                if (AskToBehaveEx.SatisfiesLikingGate(Actor, Target))
                {
                    VisitSituation situation = Target.GetSituationOfType<VisitSituation>();
                    if (situation != null)
                    {
                        VisitSituation.TimeForGuestToLeave child = situation.Child as VisitSituation.TimeForGuestToLeave;
                        if (child != null)
                        {
                            situation.SetState(new VisitSituation.Socializing(situation));
                        }
                    }

                    return true;
                }
                else
                {
                    return base.Run();
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        private new class Definition : Sim.TurnToFaceAndAskToLeave.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TurnToFaceAndAskToLeaveEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return base.GetInteractionName(a, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.SimDescription.ChildOrBelow) return false;

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
