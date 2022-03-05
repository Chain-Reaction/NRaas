using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Helpers;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class RabbitholeSignChildUpForAfterschoolClassEx : SchoolRabbitHole.SignChildUpForAfterschoolClass, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, SchoolRabbitHole.SignChildUpForAfterschoolClass.Definition, Definition>(false);

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
                bool succeeded = false;
                if ((SelectedObjects != null) && (SelectedObjects.Count > 0x0))
                {
                    AfterschoolActivityType chosenActivity = (InteractionDefinition as Definition).ChosenActivity;
                    if (AfterschoolActivityEx.IsChildActivity(chosenActivity) && AfterschoolActivityEx.AlreadyHasChildActivity(Actor.SimDescription))
                    {
                        return false;
                    }

                    StartStages();
                    BeginCommodityUpdates();
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    EndCommodityUpdates(succeeded);
                    if (!succeeded)
                    {
                        return succeeded;
                    }

                    foreach (object obj2 in SelectedObjects)
                    {
                        Sim actor = (Sim)obj2;
                        if (!AfterschoolActivityEx.HasAfterschoolActivityOnDays(actor.SimDescription, AfterschoolActivityEx.GetDaysForActivityType(chosenActivity)) && AfterschoolActivityEx.AddNewActivity(actor.SimDescription, chosenActivity))
                        {
                            EventTracker.SendEvent(new AfterschoolActivityEvent(EventTypeId.kSignUpChildForAfterschoolActivity, Actor, actor, chosenActivity));
                            EventTracker.SendEvent(new AfterschoolActivityEvent(EventTypeId.kSignedUpForAfterschoolActivity, actor, Target, chosenActivity));
                        }
                    }
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

        public new class Definition : SchoolRabbitHole.SignChildUpForAfterschoolClass.Definition
        {
            public Definition()
            { }
            public Definition(AfterschoolActivityType type)
                : base(type)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new RabbitholeSignChildUpForAfterschoolClassEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, SchoolRabbitHole target, List<InteractionObjectPair> results)
            {
                foreach (AfterschoolActivityData data in AfterschoolActivityBooter.Activities.Values)
                {
                    results.Add(new InteractionObjectPair(new Definition(data.mActivity.CurrentActivityType), iop.Target));
                }
            }

            public override string GetInteractionName(Sim a, SchoolRabbitHole target, InteractionObjectPair interaction)
            {
                return base.GetInteractionName(a, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                List<Sim> validSims = GetValidSims(parameters.Actor, false);
                NumSelectableRows = validSims.Count;
                base.PopulateSimPickerAndHideTabs(ref parameters, out listObjs, out headers, validSims, false);
            }

            protected new bool IsValidSim(Sim s)
            {
                if (!AfterschoolActivityEx.IsValidSimForSimPicker(s.SimDescription, ChosenActivity))
                {
                    return false;
                }
                return true;
            }

            public new List<Sim> GetValidSims(IActor actor, bool justFindOne)
            {
                List<Sim> sims = actor.Household.Sims;
                List<Sim> list2 = new List<Sim>();
                foreach (Sim sim in sims)
                {
                    if (IsValidSim(sim))
                    {
                        list2.Add(sim);
                        if (justFindOne)
                        {
                            return list2;
                        }
                    }
                }
                return list2;
            }

            public override bool Test(Sim a, SchoolRabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    return (GetValidSims(a, true).Count > 0x0);
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
