using NRaas.CommonSpace.Replacers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Preload
{
    public class FixTestBarBrawl : PreloadOption
    {
        public override string GetTitlePrefix()
        {
            return "FixTestBarBrawl";
        }

        public static bool CommonTest(Sim actor, Sim target)
        {
            if ((actor == null) || (actor.SimDescription == null)) return false;

            if ((target == null) || (target.SimDescription == null)) return false;

            if ((actor.Genealogy == null) || (target.Genealogy == null)) return false;

            if (target.LotCurrent == null) return false;

            return true;
        }

        public static bool TestFight(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!CommonTest(actor, target)) return false;

                if (TestBarBrawl(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback))
                {
                    return false;
                }
                else if ((actor.SimDescription.ChildOrBelow) || (target.SimDescription.ChildOrBelow))
                {
                    if ((actor.SimDescription.YoungAdultOrAbove) || (target.SimDescription.YoungAdultOrAbove))
                    {
                        return false;
                    }
                }

                if (target.Service is Burglar)
                {
                    BurglarSituation situation = ServiceSituation.FindServiceSituationInvolving(target) as BurglarSituation;
                    if (((situation != null) && (situation.HasBeenApprehended || situation.HasBeenDefeated)) || (target.LotCurrent != Household.ActiveHousehold.LotHome))
                    {
                        return false;
                    }
                    else if ((!actor.IsBrave && !actor.BuffManager.HasElement(BuffNames.OddlyPowerful)) && !actor.TraitManager.HasElement(TraitNames.CanApprehendBurglar))
                    {
                        return false;
                    }
                    return true;
                }

                if ((actor.Genealogy == null) || (target.Genealogy == null))
                {
                    return false;
                }

                Relationship relationship = Relationship.Get(actor, target, false);
                if ((actor.HasTrait(TraitNames.MeanSpirited) && (relationship != null)) && ((relationship.AreFriendsOrRomantic() || (actor.Household.Contains(target.SimDescription) && (relationship.LTR.Liking >= 0f))) || target.Genealogy.IsParentOrStepParent(actor.Genealogy)))
                {
                    return false;
                }

                return (((relationship == null) || (relationship.LTR.Liking < SocialComponent.kFightLikingValue)) && !target.Genealogy.IsParentOrStepParent(actor.Genealogy));
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestBarBrawl(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!CommonTest(actor, target)) return false;

                if ((actor.SimDescription.TeenOrBelow != target.SimDescription.TeenOrBelow) || target.Genealogy.IsParentOrStepParent(actor.Genealogy))
                {
                    return false;
                }

                if (!Bartending.IsVenueNightlifeVenue(target.LotCurrent.GetMetaAutonomyType))
                {
                    return false;
                }

                if (isAutonomous)
                {
                    return Bartending.IsVenueDiveBar(target.LotCurrent.GetMetaAutonomyType);
                }
                return true;
            }
            catch (Exception e)
            {
                Common.Exception("TestBarBrawl", e);
                return false;
            }
        }

        public override void OnPreLoad()
        {
            try
            {
                Overwatch.Log("Try FixTestBarBrawl");

                Overwatch.Log(ActionDataReplacer.Perform<FixTestBarBrawl>("TestBarBrawl"));
                Overwatch.Log(ActionDataReplacer.Perform<FixTestBarBrawl>("TestFight"));
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
