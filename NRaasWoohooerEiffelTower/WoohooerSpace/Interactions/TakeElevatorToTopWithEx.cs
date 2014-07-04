using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
//using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
	public class TakeElevatorToTopWithEx : RabbitHole.VisitRabbitHoleWithBase<TakeElevatorToTopWithEx>, Common.IPreLoad, Common.IAddInteraction
	{
		public void AddInteraction(Common.InteractionInjectorList interactions)
        {
			//interactions.Replace<EiffelTower, EiffelTower.TakeElevatorToTopWith.ElevatorDefinition>(new ElevatorDefinition (new VisitRabbitHoleEx.InteractionParameters ("Gameplay/Objects/RabbitHoles/EiffelTower:", "TakeElevatorToTop", EiffelTower.kVisitRabbitHoleTuning, Origin.FromEiffelTower)));
			//interactions.Replace<EiffelTower, EiffelTower.TakeElevatorToTopWith.StairsDefinition>(new StairsDefinition (new VisitRabbitHoleEx.InteractionParameters ("Gameplay/Objects/RabbitHoles/EiffelTower:", "TakeStairsToTop", EiffelTower.kVisitRabbitHoleTuning, Origin.FromEiffelTower)));
			interactions.AddCustom(new TakeElevatorToTopWithEx.CustomInjector());
        }

		public void OnPreLoad()
        {
			//Tunings.Inject<RabbitHole, RabbitHole.VisitRabbitHoleWith.Definition, Definition>(false);
			//Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTopWith.ElevatorDefinition, ElevatorDefinition>(false);
			Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTopWith.ElevatorDefinition, ElevatorDefinition>(false);
			Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTopWith.StairsDefinition, StairsDefinition>(false);
        }

        public override string GetInteractionName()
        {
			Definition definition = InteractionDefinition as Definition;
			if (definition != null)
			{
				if (definition is ElevatorDefinition)
				{
					return (definition as ElevatorDefinition).GetInteractionName(Actor, Target, InteractionObjectPair);
				}
				return (definition as StairsDefinition).GetInteractionName(Actor, Target, InteractionObjectPair);
			}
			return base.GetInteractionName();
        }

        public override InteractionInstance CreateVisitInteractionForSim(Sim sim, InteractionDefinition defToPush, List<Sim> alreadyAdded, ref Dictionary<Sim, bool> simArrivalStatus)
        {
			TakeElevatorToTopEx hole = defToPush.CreateInstance(Target, sim, mPriority, false, true) as TakeElevatorToTopEx;
            if (hole != null)
            {
                hole.TourGroup.AddRange(alreadyAdded);
				if (base.InteractionDefinition is ElevatorDefinition)
				{
					if (simArrivalStatus == null)
					{
						simArrivalStatus = new Dictionary<Sim, bool> ();
					}
					simArrivalStatus.Add (sim, false);
					hole.SimArrivalStatus = simArrivalStatus;
				}
            }
            return hole;
        }

        public override InteractionDefinition GetInteractionDefinition(string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin)
        {
			if (base.InteractionDefinition is TakeElevatorToTopWithEx.ElevatorDefinition)
			{
				TakeElevatorToTopEx.ElevatorDefinition elevatorDef = new TakeElevatorToTopEx.ElevatorDefinition(interactionName, visitTuning, visitBuffOrigin);
				elevatorDef.IsGroupAddition = true;
				return elevatorDef;
			}
			TakeElevatorToTopEx.StairsDefinition definition = new TakeElevatorToTopEx.StairsDefinition(interactionName, visitTuning, visitBuffOrigin);
            definition.IsGroupAddition = true;
            return definition;
        }

		public class Definition : RabbitHole.VisitRabbitHoleWithBase<TakeElevatorToTopWithEx>.BaseDefinition
		{
			public Definition()
			{
			}
			public Definition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters.mPrefix + parameters.mVisitName + "With", parameters.mPrefix + parameters.mVisitName, parameters.mTuning, parameters.mOrigin)
			{
			}
			/*public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
			{
				if (this.InteractionName.StartsWith("VisitWithInteraction"))
				{
					return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Core/VisitCommunityLot:VisitNamedLotWith", new object[]
						{
							target.CatalogName
						});
				}
				return base.GetInteractionName(actor, target, iop);
			}*/
			public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
			{
				return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback) && (a.Posture == null || a.Posture.Container != target);
			}
		}


		public class ElevatorDefinition : TakeElevatorToTopWithEx.Definition
        {
			public ElevatorDefinition()
            { }
			public ElevatorDefinition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters)
            { }

			public override MenuItem.ObjectPickerTitleDelegate GetPickerTitleDelegate ()
			{
				return new MenuItem.ObjectPickerTitleDelegate (this.GetPickerTitle);
			}

			public void GetPickerTitle (List<ObjectPicker.RowInfo> selectedRows, ref string caption, ref string toolTip)
			{
				int num = EiffelTower.TakeElevatorToTop.kElevatorPricePerPerson;
				if (selectedRows != null)
				{
					foreach (ObjectPicker.RowInfo current in selectedRows)
					{
						Sim sim = current.Item as Sim;
						if (sim != null && sim.Household == Household.ActiveHousehold)
						{
							num += EiffelTower.TakeElevatorToTop.kElevatorPricePerPerson;
						}
					}
				}
				caption = Localization.LocalizeString ("Gameplay/Objects/RabbitHoles/EiffelTower/TakeElevatorToTop:CostSoFar", new object[]
					{
						num
					});
				toolTip = Localization.LocalizeString ("Gameplay/Objects/RabbitHoles/EiffelTower/TakeElevatorToTop:TakeWithOthersTooltip", new object[0]);
			}

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
				string text = this.InteractionName;
				bool flag = false;
				bool flag2 = false;
				actor.IsInGroupOrPackSituation (out flag2);
				int groupCost = EiffelTower.TakeElevatorToTop.kElevatorPricePerPerson;
				GroupingSituation situationOfType = actor.GetSituationOfType<GroupingSituation> ();
				if (situationOfType != null)
				{
					IEnumerable<Sim> participants = situationOfType.Participants;
					foreach (Sim current in participants)
					{
						if (current != actor && actor.Household == current.Household)
						{
							groupCost += EiffelTower.TakeElevatorToTop.kElevatorPricePerPerson;
						}
					}
				}
				if (actor.IsInGroupOrDateSituation (out flag))
				{
					if (flag)
					{
						text += "_Date";
					}
					else
					{
						if (flag2)
						{
							text += "_Pack";
						}
						else
						{
							text += "_Group";
						}
					}
				}
				return Localization.LocalizeString (actor.IsFemale, text, new object[]{groupCost});
            }
        }

		public class StairsDefinition : TakeElevatorToTopWithEx.Definition
		{
			public StairsDefinition ()
			{ }
			public StairsDefinition (VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters)
			{ }
		}

		public class CustomInjector : Common.InteractionInjector<EiffelTower>
        {
            public CustomInjector()
            { }

			protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
			{
				EiffelTower tower = obj as EiffelTower;
				if (tower == null) return false;

				TakeElevatorToTopEx.CustomInjector.RemoveInteractionCustom<EiffelTower.TakeElevatorToTopWith.ElevatorDefinition> (obj);
				TakeElevatorToTopEx.CustomInjector.RemoveInteractionCustom<EiffelTower.TakeElevatorToTopWith.StairsDefinition> (obj);

				//Common.RemoveInteraction<VisitRabbitHoleWithEx.Definition>(obj);
				base.Perform (obj, new ElevatorDefinition (new VisitRabbitHoleEx.InteractionParameters ("Gameplay/Objects/RabbitHoles/EiffelTower:", "TakeElevatorToTop", EiffelTower.kVisitRabbitHoleTuning, Origin.FromEiffelTower)), existing);
				base.Perform (obj, new StairsDefinition (new VisitRabbitHoleEx.InteractionParameters("Gameplay/Objects/RabbitHoles/EiffelTower:", "TakeStairsToTop", EiffelTower.kVisitRabbitHoleTuning, Origin.FromEiffelTower)), existing);
				return true;
			}
        }
    }
}
