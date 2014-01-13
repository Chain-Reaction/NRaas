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
    public class SetFire : Interaction<Sim, GameObject>, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<GameObject>(Singleton);
        }

        protected bool OnPerform()
        {
            Vector3 positionOnFloor = Target.PositionOnFloor;
            LotLocation invalid = LotLocation.Invalid;
            ulong lotLocation = World.GetLotLocation(positionOnFloor, ref invalid);
            if ((lotLocation != 0L) && (Fire.CreateFire(lotLocation, invalid) != null))
            {
                return true;
            }

            return false;
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
                        Magic.EnsureSkill(Actor).IncreaseEvilSpellCount();
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

        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, GameObject, SetFire>
        {
            public override string GetInteractionName(Sim a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("SetOnFire:MenuName", a.IsFemale);
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.Localize("Mischief:MenuName", isFemale) };
            }

            public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((((target != a) && !(target is Sim)) && (!(target is Lot) && !(target is Sims3.Gameplay.Abstracts.RabbitHole))) && ((a.Inventory.ContainsType(typeof(MagicWand), 1) && a.SimDescription.ChildOrAbove) && (!target.InUse && (target.LotCurrent != null))))
                {
                    if (Magic.GetSkillLevel(a) >= Magic.Settings.mSetFireLevel)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

