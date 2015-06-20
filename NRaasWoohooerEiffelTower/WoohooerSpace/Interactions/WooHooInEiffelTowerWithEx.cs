using NRaas.WoohooerSpace.Helpers;
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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
	public class WooHooInEiffelTowerWithEx : WooHooInRabbitHoleWithBaseEx<EiffelTower.TakeElevatorToTop>, Common.IPreLoad, Common.IAddInteraction
	{
		public void AddInteraction(Common.InteractionInjectorList interactions)
        {
			interactions.AddCustom(new CustomInjector());
        }

		public void OnPreLoad()
        {
			Woohooer.InjectAndReset<EiffelTower, EiffelTower.WooHooInEiffelTowerWith.Definition, MakeoutDefinition>(true);
			Woohooer.InjectAndReset<EiffelTower, EiffelTower.WooHooInEiffelTowerWith.Definition, SafeDefinition>(true);
			Woohooer.InjectAndReset<EiffelTower, EiffelTower.WooHooInEiffelTowerWith.Definition, RiskyDefinition>(true);
			Woohooer.InjectAndReset<EiffelTower, EiffelTower.WooHooInEiffelTowerWith.Definition, TryForBabyDefinition>(true);
        }

		public override bool Makeout
		{
			get
			{
				return InteractionDefinition is MakeoutDefinition;
			}
		}

		public override void SetWooHooImpregnateAndStyle (InteractionInstance currentInteraction, bool impregnate, CommonWoohoo.WoohooStyle style)
		{
			TakeElevatorToTopEx takeElevatorToTopEx = currentInteraction as TakeElevatorToTopEx;
			if (takeElevatorToTopEx != null)
			{
				takeElevatorToTopEx.mImpregnate = impregnate;
				takeElevatorToTopEx.mStyle = style;
			}
		}

		public new abstract class BaseDefinition : BaseDefinition<WooHooInEiffelTowerWithEx> //CommonWoohoo.BaseDefinition<RabbitHole, WooHooInEiffelTowerWithEx>
        {
			public BaseDefinition()
			{ }
			protected BaseDefinition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters)
            { }

			public override Sim GetTarget(Sim actor, RabbitHole target, InteractionInstance paramInteraction)
            {
				WooHooInEiffelTowerWithEx interaction = paramInteraction as WooHooInEiffelTowerWithEx;
                if (interaction == null) return null;

                return interaction.GetSelectedObject() as Sim;
            }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
				return CommonWoohoo.WoohooLocation.EiffelTower;
            }

			public override bool RomanticSimTest (Sim actor, Sim sim, bool isAutonomous)
			{
				return actor.Position.y == sim.Position.y && base.RomanticSimTest (actor, sim, isAutonomous);
			}

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    /*if (a.Posture.Container != target.RabbitHoleProxy)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not Container");
                        return false;
                    }*/

					if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                    /*if (GetRomanticSimsAtSameLevel(target, a, isAutonomous, GetStyle(null), Makeout, true).Count == 0x0)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("No Other Sims");
                        return false;
                    }*/

                    /*RabbitHole.RabbitHoleInteraction<Sim, RabbitHole> currentInteraction = a.CurrentInteraction as RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>;
                    if (currentInteraction == null)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not Rabbithole Interaction");
                        return false;
                    }

                    if (!currentInteraction.CanWooHooDuringInteraction)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("CanWooHooDuringInteraction Fail");
                        return false;
                    }*/
					TimedStage currentStage = a.CurrentInteraction.ActiveStage as TimedStage;
					return currentStage != null && currentStage.mbTimerActive;
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

            /*public override InteractionDefinition ProxyClone(Sim target)
            {
                throw new NotImplementedException();
            }*/
        }

        public class SafeDefinition : BaseDefinition
        {
			public SafeDefinition()
			{ }
            public SafeDefinition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }
        }

        public class RiskyDefinition : BaseDefinition
        {
			public RiskyDefinition()
			{ }
            public RiskyDefinition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.GetRiskyBabyMadeChance (actor) });
            }
        }

        public class TryForBabyDefinition : BaseDefinition
        {
			public TryForBabyDefinition()
			{ }
            public TryForBabyDefinition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }
        }

        public class MakeoutDefinition : BaseDefinition
        {
			public MakeoutDefinition()
			{ }
            public MakeoutDefinition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override bool RomanticSimTest (Sim actor, Sim sim, bool isAutonomous)
			{
				if (actor.Position.y != sim.Position.y) return false;

				GreyedOutTooltipCallback greyedOutTooltipCallback = null;
				return CommonSocials.SatisfiesRomance (actor, sim, "RabbitholeRomance ", isAutonomous, ref greyedOutTooltipCallback);
			}

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, mPrefix + "MakeOutWith", new object[0]);
            }
		}

		public class CustomInjector : Common.InteractionInjector<EiffelTower>
        {
            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
				EiffelTower tower = obj as EiffelTower;
				if (tower == null) return false;

				for (int i = obj.Interactions.Count - 1; i >= 0; i--)
				{
					if (obj.Interactions[i].InteractionDefinition is RabbitHole.WooHooInRabbitHoleWithBase<EiffelTower.TakeElevatorToTop>.BaseDefinition)
					{
						obj.Interactions.RemoveAt (i);
						break;
					}
				}

				VisitRabbitHoleEx.InteractionParameters parameters = new VisitRabbitHoleEx.InteractionParameters("Gameplay/Objects/RabbitHoles/EiffelTower:", "TakeElevatorToTop", EiffelTower.kVisitRabbitHoleTuning, Origin.FromEiffelTower);
				base.Perform(obj, new MakeoutDefinition(parameters), existing);
                base.Perform(obj, new SafeDefinition(parameters), existing);
                base.Perform(obj, new RiskyDefinition(parameters), existing);
                base.Perform(obj, new TryForBabyDefinition(parameters), existing);
                return true;
            }
		}

		public class LocationControl : WooHooInRabbitHoleWithEx.LocationControl
        {
			public override CommonWoohoo.WoohooLocation Location
            {
				get { return CommonWoohoo.WoohooLocation.EiffelTower; }
            }

            public override bool Matches(IGameObject obj)
            {
				return obj is EiffelTower;
            }

			/*public override bool HasWoohooableObject(Lot lot)
            {
                return false;
            }*/

            public override bool HasLocation(Lot lot)
            {
				return (lot.CountObjects<EiffelTower>() > 0);
            }

			/*public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                return null;
            }*/

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

				return Woohooer.Settings.mAutonomousEiffelTower;
            }

            /*public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
            {
                return null;
            }*/
        }
    }
}
