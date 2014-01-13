using NRaas.MagicSpace.Skills;
using Sims3.Gameplay.Objects.KolipokiMod;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NRaas.MagicSpace.Interactions
{
    public class TakeOutTrash : Interaction<Sim, TrashcanIndoor>, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<TrashcanIndoor>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                Vector3 position = Target.Position;
                if (!Actor.RouteToDynamicObjectRadius(Target, 1.5f, null, new Route.RouteOption[0]))
                {
                    return false;
                }

                if (position == Target.Position)
                {
                    if (MagicWand.PerformAnimation(Actor, OnPerform))
                    {
                        Magic.EnsureSkill(Actor).IncreaseGoodSpellCount();
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

        protected bool OnPerform()
        {
            Target.TrashUnitsUsed = 0;
            return true;
        }

        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, TrashcanIndoor, TakeOutTrash>
        {
            public override string GetInteractionName(Sim a, TrashcanIndoor target, InteractionObjectPair interaction)
            {
                return Common.Localize("TakeOutTrash:MenuName", a.IsFemale);
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.Localize("Chores:MenuName", isFemale) };
            }

            public override bool Test(Sim a, TrashcanIndoor target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.InUse)
                {
                    if (!a.Inventory.ContainsType(typeof(MagicWand), 1))
                    {
                        return false;
                    }

                    if (a.SimDescription.ChildOrAbove && (target.TrashUnitsUsed > 0))
                    {
                        if (Magic.GetSkillLevel(a) >= Magic.Settings.mTakeOutTrashLevel)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}

