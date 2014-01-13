using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class Swipe : Interaction<Sim,GameObject>, Common.IAddInteraction
    {
        public static Definition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<GameObject>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (Target.Parent != null)
                {
                    Target.UnParent();
                }

                Household household = Target.LotCurrent.Household;

                Target.FadeOut(true);
                Target.RemoveFromUseList(base.Actor);
                Target.RemoveFromWorld();

                if (Actor.Inventory.CanAdd(Target))
                {
                    Actor.Inventory.TryToAdd(Target, false);
                }
                else
                {
                    Actor.Household.SharedFamilyInventory.Inventory.TryToAdd(Target, false);

                    Common.Notify(Common.Localize("Swipe:FamilyInventory", Actor.IsFemale, new object[] { Actor, Target }));
                }

                if ((household != null) && (household.Sims.Count > 0x0))
                {
                    TraitFunctions.ItemStolenCallback(household, Origin.FromTheft);
                    Target.SetStealActors(Actor, RandomUtil.GetRandomObjectFromList<Sim>(household.Sims));
                }
                else
                {
                    Target.SetStealActors(Actor, null);
                }

                foreach (Situation situation in Actor.Autonomy.SituationComponent.Situations)
                {
                    VisitSituation situation2 = situation as VisitSituation;
                    if (situation2 != null)
                    {
                        situation2.GuestStartingInappropriateAction(base.Actor, TraitTuning.KleptomaniacStealingInnapropriateness);
                    }
                }

                return true;
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

        public class Definition : InteractionDefinition<Sim, GameObject, Swipe>
        {
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return Common.Localize("Swipe:MenuName", actor.IsFemale);
            }

            public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a.Household == null) return false;

                    if (target.LotCurrent == a.LotHome) return false;

                    if (!target.IsStealable()) return false;

                    OmniCareer omni = a.Occupation as OmniCareer;
                    if (omni == null) return false;

                    return omni.CanSwipe();
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
