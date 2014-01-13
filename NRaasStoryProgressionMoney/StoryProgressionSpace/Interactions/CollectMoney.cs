using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class CollectMoney : Computer.ComputerInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Computer, Computer.BuyStockMarket.Definition, Definition>(true);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                StandardEntry();

                try
                {
                    if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                    {
                        return false;
                    }

                    AnimateSim("GenericTyping");

                    int totalFunds = CollectDeedsScenario.Perform(Actor);

                    if (Actor.IsSelectable)
                    {
                        if (totalFunds > 0)
                        {
                            Common.Notify(Actor, Common.Localize("CollectDeeds:Success", Actor.IsFemale, new object[] { Actor, totalFunds }));
                        }
                        else
                        {
                            Common.Notify(Actor, Common.Localize("CollectDeeds:Failure", Actor.IsFemale, new object[] { Actor }));
                        }
                    }

                    Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                    return true;
                }
                finally
                {
                    StandardExit();
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

        private class Definition : InteractionDefinition<Sim, Computer, CollectMoney>
        {
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return Common.Localize("CollectDeeds:MenuName", actor.IsFemale);
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a.Household.RealEstateManager.AllProperties.Count == 0)
                    {
                        return false;
                    }

                    return target.IsComputerUsable(a, true, false, isAutonomous);
                }
                catch (ResetException)
                {
                    throw;
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

