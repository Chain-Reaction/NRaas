using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class EnsorcelInteractionEx : Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, OccultGenie.EnsorcelInteraction.Definition, Definition>(false);

            sOldSingleton = OccultGenie.EnsorcelInteraction.Singleton;
            OccultGenie.EnsorcelInteraction.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, OccultGenie.EnsorcelInteraction.Definition>(OccultGenie.EnsorcelInteraction.Singleton);
        }

        public class Definition : OccultGenie.EnsorcelInteraction.Definition
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!OccultTypeHelper.HasType(a, OccultTypes.Genie)) return false;

                if (target.LotHome != null)
                {
                    if (Households.All(target.Household).Count <= 1)
                    {
                        return false;
                    }
                }

                OccultGenie occultType = a.SimDescription.OccultManager.GetOccultType(OccultTypes.Genie) as OccultGenie;
                if (target.SimDescription.IsBonehilda)
                {
                    return false;
                }

                if (((target.Service != null) && (target.Service.ServiceType == ServiceType.GrimReaper)) || (target.Household == a.Household))
                {
                    return false;
                }

                if (DaycareSituation.IsInDaycareSituationWith(target, a))
                {
                    return false;
                }

                if ((target.BuffManager.HasElement(BuffNames.WeddingDay) || target.IsEngaged) || target.SimDescription.IsPregnant)
                {
                    return false;
                }

                if (GameUtils.IsOnVacation())
                {
                    return false;
                }

                if (((isAutonomous || (occultType == null)) || (target.OccultManager.HasOccultType(OccultTypes.Genie) || target.BuffManager.HasElement(BuffNames.Ensorcelled))) || !a.SimDescription.ChildOrAbove)
                {
                    return false;
                }

                if (!occultType.MagicPoints.HasPoints())
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return Localization.LocalizeString(a.IsFemale, "Gameplay/Actors/Sim/GenieOutOfPoints:OutOfPoints", new object[] { a });
                    };
                    return false;
                }

                /*
                if (!a.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.Human))
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return Localization.LocalizeString(a.IsFemale, "Gameplay/Actors/Sim/EnsorcelFail:HouseholdFull", new object[] { a });
                    };
                    return false;
                }

                if (!a.Household.CanAddSpeciesToHousehold(target.SimDescription.Species, 0x1, true))
                {
                    greyedOutTooltipCallback = delegate {
                        return Localization.LocalizeString(a.IsFemale, "Gameplay/Actors/Sim/EnsorcelFail:UnableToJoinHousehold", new object[] { a });
                    };
                    return false;
                }
                */
                if (target.SimDescription.ToddlerOrBelow)
                {
                    return false;
                }

                if (target.Household.IsServiceNpcHousehold && (target.SimDescription.CreatedByService != null))
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return Localization.LocalizeString(a.IsFemale, "Gameplay/Actors/Sim/EnsorcelFail:UnableToJoinHousehold", new object[] { a });
                    };
                    return false;
                }

                if (target.SimDescription.IsDead || ((target.SimDescription.Service != null) && (target.SimDescription.Service.ServiceType == ServiceType.GrimReaper)))
                {
                    greyedOutTooltipCallback = delegate {
                        return Localization.LocalizeString(a.IsFemale, "Gameplay/Actors/Sim/EnsorcelFail:UnableToJoinHousehold", new object[] { a });
                    };
                    return false;
                }

                /*
                if (target.SimDescription.AssignedRole != null) 
                {
                    RoleData data = target.SimDescription.AssignedRole.Data;
                    if ((data != null) && !data.CanBeEnsorceled)
                    {
                        greyedOutTooltipCallback = delegate
                        {
                            return Localization.LocalizeString(a.IsFemale, "Gameplay/Actors/Sim/EnsorcelFail:UnableToJoinHousehold", new object[] { a });
                        };
                        return false;
                    }
                }
                */
                return true;
            }
        }
    }
}
