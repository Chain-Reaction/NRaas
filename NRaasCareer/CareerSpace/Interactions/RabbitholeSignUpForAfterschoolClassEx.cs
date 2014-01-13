using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Helpers;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class RabbitholeSignUpForAfterschoolClassEx : SchoolRabbitHole.SignUpForAfterschoolClass, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, SchoolRabbitHole.SignUpForAfterschoolClass.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<SchoolRabbitHole>(Singleton);
        }

        public override bool InRabbitHole()
        {
            try
            {
                AfterschoolActivityType chosenActivity = (InteractionDefinition as Definition).ChosenActivity;
                if (AfterschoolActivityEx.IsChildActivity(chosenActivity) && AfterschoolActivityEx.AlreadyHasChildActivity(Actor.SimDescription))
                {
                    return false;
                }

                StartStages();
                BeginCommodityUpdates();
                bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                EndCommodityUpdates(succeeded);

                if (succeeded && AfterschoolActivityEx.AddNewActivity(Actor.SimDescription, chosenActivity))
                {
                    EventTracker.SendEvent(new AfterschoolActivityEvent(EventTypeId.kSignedUpForAfterschoolActivity, Actor, Target, chosenActivity));
                }
                return succeeded;
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

        public new class Definition : SchoolRabbitHole.SignUpForAfterschoolClass.Definition
        {
            public Definition()
            { }
            public Definition(AfterschoolActivityType type)
                : base(type)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new RabbitholeSignUpForAfterschoolClassEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, SchoolRabbitHole target, List<InteractionObjectPair> results)
            {
                if (AfterschoolActivityEx.AlreadyHasChildActivity(actor.SimDescription)) return;

                foreach (AfterschoolActivityData data in AfterschoolActivityBooter.Activities.Values)
                {
                    if (!data.IsValidFor(actor.SimDescription)) continue;

                    results.Add(new InteractionObjectPair(new Definition(data.mActivity.CurrentActivityType), iop.Target));
                }
            }

            public override string GetInteractionName(Sim a, SchoolRabbitHole target, InteractionObjectPair interaction)
            {
                return base.GetInteractionName(a, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, SchoolRabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!AfterschoolActivityEx.MeetsCommonAfterschoolActivityRequirements(a.SimDescription, ChosenActivity, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}
