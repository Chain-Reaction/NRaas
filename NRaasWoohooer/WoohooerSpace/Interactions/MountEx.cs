using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class MountEx : Common.IPreLoad
    {
        public static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.Mount.Definition, Definition>(false);

            Sim.Mount.Singleton = Singleton;
        }

        public class Definition : Sim.Mount.Definition
        {
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!target.IsHorse || !target.SimDescription.AdultOrAbove)
                    {
                        return false;
                    }
                    else if (target.BeingRiddenPosture != null)
                    {
                        return false;
                    }
                    /*
                    if (target.SimDescription.IsPregnant)
                    {
                        return false;
                    }
                    */
                    if (!BorrowHorseSituation.IsPermittedToRide(actor, target))
                    {
                        return false;
                    }
                    else if (target.IsSleeping)
                    {
                        return false;
                    }

                    if (actor.Household == target.Household)
                    {
                        Relationship relationship = Relationship.Get(actor, target, false);
                        if ((relationship == null) || (relationship.CurrentLTRLiking < RidingSkill.kMinLtrForMountAvailability))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Common.Exception(actor, target, e);
                    return false;
                }
            }
        }
    }
}
