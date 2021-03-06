﻿using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
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
    public class VisitLotEx : VisitLot, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Lot, VisitLot.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Lot, VisitLot.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (GoHereEx.Teleport.Perform(Actor, Target, true))
                {
                    return true;
                }

                return base.Run();
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

        private new class Definition : VisitLot.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new VisitLotEx();
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
