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
	public class TakeElevatorToTopWithEx : EiffelTower.TakeElevatorToTopWith, Common.IPreLoad, Common.IAddInteraction
	{
		public void AddInteraction(Common.InteractionInjectorList interactions)
        {
			interactions.AddCustom(new CustomInjector());
        }

		public void OnPreLoad()
        {
			Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTopWith.ElevatorDefinition, ElevatorDefinition>(false);
			Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTopWith.StairsDefinition, StairsDefinition>(false);
        }

		public override string GetInteractionName ()
		{
			return (InteractionDefinition as RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition).InteractionName ?? base.GetInteractionName();
		}

		public override InteractionDefinition GetInteractionDefinition(string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin)
        {
			if (InteractionDefinition is StairsDefinition)
			{
				return new TakeElevatorToTopEx.StairsDefinition (GetInteractionName(), null, true, interactionName, visitTuning, visitBuffOrigin);
			}
			return new TakeElevatorToTopEx.ElevatorDefinition (GetInteractionName(), null, true, interactionName, visitTuning, visitBuffOrigin);
        }

		public new class StairsDefinition : RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition
		{
			public RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition mBaseDefinition;
			public string mDisplayName;
			public string[][] mPaths;

			public StairsDefinition()
			{
			}
			public StairsDefinition(RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition baseDef) : base(baseDef.InteractionName, baseDef.VisitName, baseDef.VisitTuniing, baseDef.VisitBuffOrigin)
			{
				mBaseDefinition = baseDef;
			}
			public StairsDefinition(RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition baseDef, string displayName) : this(baseDef)
			{
				mDisplayName = displayName;
				mPaths = new string[][]{ baseDef.GetPath(false), baseDef.GetPath(true) };
				mBaseDefinition = null;
			}
			public virtual InteractionDefinition CreateDefinition (RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition baseDef, string displayName)
			{
				return new StairsDefinition (baseDef, displayName);
			}
			public override void AddInteractions (InteractionObjectPair iop, Sim actor, RabbitHole target, List<InteractionObjectPair> results)
			{
				List<InteractionObjectPair> iops = new List<InteractionObjectPair> ();
				mBaseDefinition.AddInteractions (iop, actor, target, iops);
				foreach (InteractionObjectPair current in iops)
				{
					RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition baseDef = current.InteractionDefinition as RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition;
					results.Add(new InteractionObjectPair(CreateDefinition(baseDef, baseDef.GetInteractionName(actor, target, iop)), iop.Target));
				}
			}
			public override InteractionInstance CreateInstance (ref InteractionInstanceParameters parameters)
			{
				InteractionInstance takeElevatorToTopWithEx = new TakeElevatorToTopWithEx();
				takeElevatorToTopWithEx.Init (ref parameters);
				return takeElevatorToTopWithEx;
			}
			public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
			{
				return mDisplayName;
			}
			public override string[] GetPath (bool isFemale)
			{
				return mPaths[isFemale ? 1 : 0];
			}
			public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
			{
				return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback) && (a.Posture == null || a.Posture.Container != target);
			}
		}


		public new class ElevatorDefinition : StairsDefinition
        {
			public MenuItem.ObjectPickerTitleDelegate mTitleDelegate;

			public ElevatorDefinition()
			{
			}
			public ElevatorDefinition(RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition baseDef) : base(baseDef)
			{
			}
			public ElevatorDefinition(RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition baseDef, string displayName) : base(baseDef, displayName)
			{
				mTitleDelegate = baseDef.GetPickerTitleDelegate ();
			}
			public override InteractionDefinition CreateDefinition (RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition baseDef, string displayName)
			{
				return new ElevatorDefinition (baseDef, displayName);
			}
			public override InteractionInstance CreateInstance (ref InteractionInstanceParameters parameters)
			{
				parameters.mInteractionObjectPair.mInteraction = new EiffelTower.TakeElevatorToTopWith.ElevatorDefinition (mDisplayName, VisitName, VisitTuniing, VisitBuffOrigin);
				return base.CreateInstance(ref parameters);
			}
			public override MenuItem.ObjectPickerTitleDelegate GetPickerTitleDelegate ()
			{
				return mTitleDelegate;
			}
        }

		public class CustomInjector : TakeElevatorToTopEx.CustomInjector<RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition>
        {
			protected override InteractionDefinition CreateElevatorDefinition (RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition baseDef)
			{
				return new ElevatorDefinition (baseDef);
			}
			protected override InteractionDefinition CreateStairsDefinition (RabbitHole.VisitRabbitHoleWithBase<EiffelTower.TakeElevatorToTopWith>.BaseDefinition baseDef)
			{
				return new StairsDefinition(baseDef);
			}
        }
    }
}
