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
    public class ResurrectFromGrave : Interaction<Sim, Urnstone>, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Urnstone>(Singleton);
        }

        protected bool OnPerform()
        {
            SimDescription deadSimsDescription = Target.DeadSimsDescription;
            deadSimsDescription.DeathStyle = SimDescription.DeathType.None;
            deadSimsDescription.IsGhost = false;
            deadSimsDescription.IsNeverSelectable = false;
            deadSimsDescription.ShowSocialsOnSim = true;
            Vector3 vector2 = Target.Position;
            vector2.x++;
            if (deadSimsDescription.CreatedSim != null)
            {
                Target.GhostToSim(deadSimsDescription.CreatedSim, false, true);
            }
            else
            {
                Target.OriginalHousehold.Add(deadSimsDescription);
                deadSimsDescription.Instantiate(vector2);
                deadSimsDescription.AgingEnabled = true;
            }
            if (deadSimsDescription.CreatedSim != null)
            {
                Target.Destroy();
            }

            return true;
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

        public sealed class Definition : InteractionDefinition<Sim, Urnstone, ResurrectFromGrave>
        {
            public override string GetInteractionName(Sim a, Urnstone target, InteractionObjectPair interaction)
            {
                return Common.Localize("Resurrect:MenuName", a.IsFemale);
            }

            public override bool Test(Sim a, Urnstone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((a.SimDescription.ChildOrAbove && (target != null)) && a.Inventory.ContainsType(typeof(MagicWand), 1))
                {
                    Magic magic = a.SkillManager.GetSkill<Magic>(Magic.StaticGuid);
                    if ((magic != null) && (magic.IsMasterWizard()))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

