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
    public class AskToBehaveEx : Sim.AskToBehave, Common.IPreLoad//, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.AskToBehave.Definition,Definition>(false);

            sOldSingleton = Sim.AskToBehave.Singleton;
            Sim.AskToBehave.Singleton = new Definition();
        }

        public static bool IsAlone(Sim target)
        {
            if ((target.LotCurrent == null) || (target.LotCurrent.Household == null)) return true;

            foreach (Sim sim in Households.AllHumans(target.LotCurrent.Household))
            {
                if (sim == target) continue;

                if (sim.RoomId == target.RoomId) return false;
            }

            return true;
        }

        public static bool SatisfiesLikingGate(Sim sim, Sim target)
        {
            if ((target.IsSelectable) && (GoHere.Settings.mDisallowActiveGoHome)) return true;

            if (IsAlone(target)) return true;
                    
            foreach (SimDescription member in Households.All(sim.Household))
            {
                if (target.SimDescription.TeenOrAbove)
                {
                    if (member.ChildOrBelow) continue;
                }

                float liking = 0;

                Relationship relation = Relationship.Get(member, target.SimDescription, false);
                if (relation != null)
                {
                    liking = relation.CurrentLTRLiking;
                }

                if (liking >= GoHere.Settings.mRudeGuestLikingGate)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Run()
        {
            try
            {
                if (SatisfiesLikingGate(Actor, Target))
                {
                    VisitSituation situation = Target.GetSituationOfType<VisitSituation>();
                    if (situation != null)
                    {
                        VisitSituation.GuestBehavingInappropriately child = situation.Child as VisitSituation.GuestBehavingInappropriately;
                        if (child != null)
                        {
                            child.mOnlyOneHostResponds = Actor;

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

        private new class Definition : Sim.AskToBehave.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new AskToBehaveEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return base.GetInteractionName(a, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.RoomId != target.RoomId) return false;

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
