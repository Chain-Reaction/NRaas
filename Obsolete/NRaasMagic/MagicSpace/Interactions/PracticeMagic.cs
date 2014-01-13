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
    public class PracticeMagic : Interaction<Sim, Mirror>, ISkillCallbackUser, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Mirror>(Singleton);
        }

        public Skill.SkillLevelUpCallback GetSkillCallback()
        {
            return new Skill.SkillLevelUpCallback(OnLeveledUp);
        }

        protected void OnLeveledUp(int skillLevel)
        {
        }

        public bool RouteToMirror(Sim s, float radius, Mirror obj)
        {
            RadialRangeDestination destination = new RadialRangeDestination();
            destination.mTargetObject = obj;
            destination.mCenterPoint = obj.Position;
            destination.mfMinRadius = radius;
            destination.mfMaxRadius = radius;
            destination.mConeVector = obj.ForwardVector;
            destination.mfConeAngle = MathUtils.Degree2Radian(0.5f);
            destination.mFacingPreference = RouteOrientationPreference.TowardsObject;
            Route r = s.CreateRoute();
            r.SetValidRooms(obj.LotCurrent.LotId, new int[] { obj.RoomId });
            r.SetOption(Route.RouteOption.DoLineOfSightCheckUserOverride, true);
            r.AddDestination(destination);
            r.Plan();
            return s.DoRoute(r);
        }

        public override bool Run()
        {
            try
            {
                Vector3 position = Target.Position;
                if (!RouteToMirror(Actor, 1.5f, Target))
                {
                    return false;
                }
                if (position == Target.Position)
                {
                    BeginCommodityUpdates();

                    IGameObject obj2 = Actor.Inventory.SetInUse(typeof(MagicWand), MagicWand.TestFunction, typeof(MagicWand));

                    bool flag = false;
                    try
                    {
                        EnterStateMachine("wand", "Enter", "x");
                        EnterState("x", "Cast Spell - Loop");

                        Magic magic = Magic.EnsureSkill(Actor);

                        magic.RegisterForSkillLevelUpEvent(GetSkillCallback(), this);
                        Actor.SkillManager.StartSkillGain(magic.Guid, 15f);
                        flag = DoLoop(~(ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle));
                        Actor.SkillManager.StopSkillGain(magic.Guid);
                    }
                    finally
                    {
                        Actor.Inventory.SetNotInUse(obj2);
                        EndCommodityUpdates(true);
                    }

                    EnterState("x", "Exit");
                    return flag;
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

        public void SkillLevelUpCallback(int skillLevel)
        {
            if (skillLevel >= 10)
            {
                base.Actor.AddExitReason(ExitReason.Finished);
            }
        }

        public sealed class Definition : InteractionDefinition<Sim, Mirror, PracticeMagic>
        {
            public override string GetInteractionName(Sim a, Mirror target, InteractionObjectPair interaction)
            {
                return Common.Localize("PracticeMagic:MenuName", a.IsFemale);
            }

            public override bool Test(Sim a, Mirror target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.SimDescription.ChildOrAbove && a.Inventory.ContainsType(typeof(MagicWand), 1))
                {
                    if (Magic.GetSkillLevel(a) < 10)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

