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
	public abstract class WooHooInRabbitHoleWithBaseEx<TVisitInteraction> : RabbitHole.WooHooInRabbitHoleWithBase<TVisitInteraction> where TVisitInteraction : RabbitHole.VisitRabbitHoleBase<TVisitInteraction>, new()
	{
		public abstract bool Makeout { get; }
		public abstract void SetWooHooImpregnateAndStyle (InteractionInstance currentInteraction, bool impregnate, CommonWoohoo.WoohooStyle style);

		public void ConfigureWooHooInteraction(TVisitInteraction currentInteraction, Sim selectedObject, RabbitHoleRomanticType romanticType, bool impregnate, CommonWoohoo.WoohooStyle style)
		{
			currentInteraction.IsGettingItOn = true;
			currentInteraction.WooHooer = Actor;
			currentInteraction.WooHooee = selectedObject;

			currentInteraction.RomanticType = romanticType;

			SetWooHooImpregnateAndStyle (currentInteraction, impregnate, style);
			currentInteraction.ActiveStage = currentInteraction.GetStages()[0x1];
		}

		public override bool Run()
		{
			Common.StringBuilder msg = new Common.StringBuilder(GetType().FullName + ":Run");

			try
			{
				msg += "A";

				IWooHooDefinition interactionDefinition = InteractionDefinition as IWooHooDefinition;

				Sim selectedObject = GetSelectedObject() as Sim; //interactionDefinition.GetTarget(Actor, Target, this);
				if (selectedObject == null)
				{
					return false;
				}

				msg += "B";

				bool impregnate = true;

				RabbitHoleRomanticType romanticType = RabbitHoleRomanticType.WooHoo;
				if (interactionDefinition.GetStyle(this) == CommonWoohoo.WoohooStyle.TryForBaby)
				{
					romanticType = RabbitHoleRomanticType.TryForBaby;
				}
				CommonWoohoo.WoohooStyle style = interactionDefinition.GetStyle(this);

				TVisitInteraction currentInteraction = selectedObject.CurrentInteraction as TVisitInteraction;
				if (currentInteraction != null)
				{
					msg += "C";

					ConfigureWooHooInteraction(currentInteraction, selectedObject, romanticType, impregnate, style);
					/*currentInteraction.IsGettingItOn = true;
					currentInteraction.WooHooer = Actor;
					currentInteraction.WooHooee = selectedObject;

					if (interactionDefinition.GetStyle(this) == CommonWoohoo.WoohooStyle.TryForBaby)
					{
						currentInteraction.RomanticType = RabbitHoleRomanticType.TryForBaby;
					}
					else
					{
						currentInteraction.RomanticType = RabbitHoleRomanticType.WooHoo;
					}

					currentInteraction.mImpregnate = impregnate;
					currentInteraction.mStyle = interactionDefinition.GetStyle(this);
					currentInteraction.ActiveStage = currentInteraction.GetStages()[0x1];*/

					impregnate = false;
				}

				currentInteraction = Actor.CurrentInteraction as TVisitInteraction;
				if (currentInteraction != null)
				{
					msg += "D";

					if (Makeout)
					{
						romanticType = RabbitHoleRomanticType.MakeOut;
					}
					ConfigureWooHooInteraction(currentInteraction, selectedObject, romanticType, impregnate, style);
					/*currentInteraction.IsGettingItOn = true;
					currentInteraction.WooHooer = Actor;
					currentInteraction.WooHooee = selectedObject;

					if (interactionDefinition.Makeout)
					{
						currentInteraction.RomanticType = RabbitHoleRomanticType.MakeOut;
					}
					else if (interactionDefinition.GetStyle(this) == CommonWoohoo.WoohooStyle.TryForBaby)
					{
						currentInteraction.RomanticType = RabbitHoleRomanticType.TryForBaby;
					}
					else
					{
						currentInteraction.RomanticType = RabbitHoleRomanticType.WooHoo;
					}
					currentInteraction.mImpregnate = impregnate;
					currentInteraction.mStyle = interactionDefinition.GetStyle(this);
					currentInteraction.ActiveStage = currentInteraction.GetStages()[0x1];*/
				}

				msg += "E";

				Target.RabbitHoleProxy.TurnOnWooHooEffect();

				CommonWoohoo.CheckForWitnessedCheating(Actor, selectedObject, true);

				if (Makeout)
				{
					EventTracker.SendEvent(new WooHooEvent(EventTypeId.kMadeOut, Actor, selectedObject, Target));
					EventTracker.SendEvent(new WooHooEvent(EventTypeId.kMadeOut, selectedObject, Actor, Target));

					EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, Actor, selectedObject, "Make Out", false, true, false, CommodityTypes.Undefined));
					EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, selectedObject, Actor, "Make Out", true, true, false, CommodityTypes.Undefined));
				}

				return true;
			}
			catch (ResetException)
			{
				throw;
			}
			catch (Exception e)
			{
				Common.Exception(Actor, Target, msg, e);
				return false;
			}
			finally
			{
				Common.DebugNotify(msg);
			}
		}

		public abstract class BaseDefinition<TInteraction> : CommonWoohoo.BaseDefinition<RabbitHole, TInteraction> where TInteraction : RabbitHole.WooHooInRabbitHoleWithBase<TVisitInteraction>, new()
		{
			protected string mPrefix;
			public Origin VisitBuffOrigin;
			public RabbitHole.VisitRabbitHoleTuningClass VisitTuning;

			public BaseDefinition()
			{ }
			protected BaseDefinition(VisitRabbitHoleEx.InteractionParameters parameters)
			{
				mPrefix = parameters.mPrefix;
				VisitBuffOrigin = parameters.mOrigin;
				VisitTuning = parameters.mTuning;
			}

			public override Sim GetTarget(Sim actor, RabbitHole target, InteractionInstance paramInteraction)
			{
				TInteraction interaction = paramInteraction as TInteraction;
				if (interaction == null) return null;

				return interaction.GetSelectedObject() as Sim;
			}

			public override int Attempts
			{
				set { }
			}

			public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
			{
				return CommonWoohoo.WoohooLocation.RabbitHole;
			}

			public virtual bool RomanticSimTest (Sim actor, Sim sim, bool isAutonomous)
			{
				GreyedOutTooltipCallback greyedOutTooltipCallback = null;
				switch (GetStyle(null))
				{
				case CommonWoohoo.WoohooStyle.Risky:
					return CommonPregnancy.SatisfiesRisky (actor, sim, "RabbitholeRisky", isAutonomous, true, ref greyedOutTooltipCallback);
				case CommonWoohoo.WoohooStyle.TryForBaby:
					return CommonPregnancy.SatisfiesTryForBaby (actor, sim, "RabbitholeTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
				}
				return CommonWoohoo.SatisfiesWoohoo (actor, sim, "RabbitholeWoohoo", isAutonomous, true, true, ref greyedOutTooltipCallback);
			}

			public List<Sim> GetRomanticSims(RabbitHole ths, Sim actor, bool isAutonomous, bool stopAtOne)
			{
				List<Sim> list = new List<Sim>();
				foreach (Sim sim in ths.RabbitHoleProxy.ActorsUsingMe)
				{
					if (sim != actor && RomanticSimTest (actor, sim, isAutonomous))
					{
						list.Add(sim);
						if (stopAtOne) break;
					}
				}
				return list;
			}

			public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
			{
				NumSelectableRows = 0x1;
				RabbitHole target = parameters.Target as RabbitHole;
				Sim actor = parameters.Actor as Sim;
				base.PopulateSimPicker(ref parameters, out listObjs, out headers, GetRomanticSims(target, actor, parameters.Autonomous, false), false);
			}

			public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
			{
				try
				{
					if (a.Posture.Container != target.RabbitHoleProxy)
					{
						greyedOutTooltipCallback = Common.DebugTooltip("Not Container");
						return false;

					}

					if (GetRomanticSims(target, a, isAutonomous, true).Count == 0x0)
					{
						greyedOutTooltipCallback = Common.DebugTooltip("No Other Sims");
						return false;
					}

					RabbitHole.RabbitHoleInteraction<Sim, RabbitHole> currentInteraction = a.CurrentInteraction as RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>;
					if (currentInteraction == null)
					{
						greyedOutTooltipCallback = Common.DebugTooltip("Not Rabbithole Interaction");
						return false;
					}

					if (!currentInteraction.CanWooHooDuringInteraction)
					{
						greyedOutTooltipCallback = Common.DebugTooltip("CanWooHooDuringInteraction Fail");
						return false;
					}

					return true;
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

			public override InteractionDefinition ProxyClone(Sim target)
			{
				throw new NotImplementedException();
			}
		}
	}

    public class WooHooInRabbitHoleWithEx : WooHooInRabbitHoleWithBaseEx<VisitRabbitHoleEx>, Common.IPreLoad, Common.IAddInteraction
    {
        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<RabbitHole, RabbitHole.WooHooInRabbitHoleWith.Definition, MakeoutDefinition>(true);
            Woohooer.InjectAndReset<RabbitHole, RabbitHole.WooHooInRabbitHoleWith.Definition, SafeDefinition>(true);
            Woohooer.InjectAndReset<RabbitHole, RabbitHole.WooHooInRabbitHoleWith.Definition, RiskyDefinition>(true);
            Woohooer.InjectAndReset<RabbitHole, RabbitHole.WooHooInRabbitHoleWith.Definition, TryForBabyDefinition>(true);
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
			VisitRabbitHoleEx visitRabbitHoleEx = currentInteraction as VisitRabbitHoleEx;
			visitRabbitHoleEx.mImpregnate = impregnate;
			visitRabbitHoleEx.mStyle = style;
		}

        public class SafeDefinition : BaseDefinition<WooHooInRabbitHoleWithEx>
        {
            public SafeDefinition()
            { }
            public SafeDefinition(VisitRabbitHoleEx.InteractionParameters parameters)
                : base(parameters)
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

		public class RiskyDefinition : BaseDefinition<WooHooInRabbitHoleWithEx>
        {
            public RiskyDefinition()
            { }
            public RiskyDefinition(VisitRabbitHoleEx.InteractionParameters parameters)
                : base(parameters)
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

		public class TryForBabyDefinition : BaseDefinition<WooHooInRabbitHoleWithEx>
        {
            public TryForBabyDefinition()
            { }
            public TryForBabyDefinition(VisitRabbitHoleEx.InteractionParameters parameters)
                : base(parameters)
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

		public class MakeoutDefinition : BaseDefinition<WooHooInRabbitHoleWithEx>
        {
            public MakeoutDefinition()
            { }
            public MakeoutDefinition(VisitRabbitHoleEx.InteractionParameters parameters)
                : base(parameters)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

			public override bool RomanticSimTest (Sim actor, Sim sim, bool isAutonomous)
			{
				GreyedOutTooltipCallback greyedOutTooltipCallback = null;
				return CommonSocials.SatisfiesRomance (actor, sim, "RabbitholeRomance ", isAutonomous, ref greyedOutTooltipCallback);
			}

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, mPrefix + "MakeOutWith", new object[0]);
            }
        }

        public class CustomInjector : Common.InteractionInjector<RabbitHole>
        {
            public CustomInjector()
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                RabbitHole hole = obj as RabbitHole;
				if (hole == null || hole is EiffelTower) return false;

                Common.RemoveInteraction<RabbitHole.WooHooInRabbitHoleWith.Definition>(obj);

                VisitRabbitHoleEx.InteractionParameters parameters;
                if (VisitRabbitHoleEx.Parameters.TryGetValue(hole.Guid, out parameters))
                {
                    base.Perform(obj, new MakeoutDefinition(parameters), existing);
                    base.Perform(obj, new SafeDefinition(parameters), existing);
                    base.Perform(obj, new RiskyDefinition(parameters), existing);
                    base.Perform(obj, new TryForBabyDefinition(parameters), existing);
                    return true;
                }

                return false;
            }
        }

        public class LocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.RabbitHole; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is RabbitHole;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return false;
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<RabbitHole>() > 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                return null;
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                return Woohooer.Settings.mAutonomousRabbithole;
            }

            public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
            {
                return null;
            }
        }
    }
}
