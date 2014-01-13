using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Helpers;
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
    public class VisitLotWithEx : VisitLotWith, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldVisitWithSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Lot, VisitLotWith.VisitWithDefinition, Definition>(false);

            sOldVisitWithSingleton = VisitWithSingleton;
            VisitWithSingleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Lot, VisitLotWith.VisitWithDefinition>(VisitWithSingleton);
        }

        private new class Definition : VisitLotWith.VisitWithDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new VisitLotWithEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (isAutonomous)
                    {
                        if (!GoHere.Settings.AllowPush(a, target))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Allow Push Fail");
                            return false;
                        }
                    }

                    bool wasGreeted = false;

                    if (a.Household != null)
                    {
                        wasGreeted = a.Household.mGreetedLots.Contains(target);
                    }

                    try
                    {
                        if ((wasGreeted) && (a.Household != null))
                        {
                            a.Household.mGreetedLots.Remove(target);
                        }

                        return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
                    }
                    finally
                    {
                        if ((wasGreeted) && (a.Household != null))
                        {
                            a.Household.mGreetedLots.Add(target);
                        }
                    }
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
