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
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
	public class TakeElevatorToTopEx : EiffelTower.TakeElevatorToTop, Common.IPreLoad, Common.IAddInteraction
	{
		public bool mImpregnate;
		public CommonWoohoo.WoohooStyle mStyle;

		public void AddInteraction(Common.InteractionInjectorList interactions)
        {
			interactions.AddCustom(new CustomInjector());
        }

		public void OnPreLoad()
        {
			Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTop.ElevatorDefinition, ElevatorDefinition>(false);
			Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTop.StairsDefinition, StairsDefinition>(false);
        }

		public void ResetShapeDeltaAndStopSkillGain()
		{
			if (InteractionDefinition is StairsDefinition)
			{
				Actor.ResetShapeDelta();
				Actor.SkillManager.StopSkillGain(SkillNames.Athletic);
			}
		}

		public override void BeforeExitRabbitHoleAndRouteAway(Sim actor)
		{
			ResetShapeDeltaAndStopSkillGain ();
			base.BeforeExitRabbitHoleAndRouteAway(actor);
		}

		public override void Cleanup()
		{
			ResetShapeDeltaAndStopSkillGain ();
			base.Cleanup();
		}

		public override void ConfigureInteraction()
		{
		}

		public override string GetInteractionName()
        {
			if (IsGettingItOn)
			{
				switch (mStyle)
				{
				case CommonWoohoo.WoohooStyle.Safe:
					return Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
				case CommonWoohoo.WoohooStyle.Risky:
					return Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.mRiskyBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(Actor)] });
				case CommonWoohoo.WoohooStyle.TryForBaby:
					return Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
				}
			}

			return base.GetInteractionName ();
		}

        public override bool InRabbitHole()
        {
			try
            {
                StartStages();
                BeginCommodityUpdates();

                bool succeeded = false;
                try
                {
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, RabbitHole>.InsideLoopFunction(LoopDel), null);
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

				RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition definition = InteractionDefinition as RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition;

                if (IsGettingItOn)
                {
					VisitRabbitHoleEx.GettingItOnInRabbitHole(this, WooHooer, WooHooee, RomanticType, definition.VisitTuning, mStyle, CommonWoohoo.WoohooLocation.EiffelTower, mImpregnate);
                    /*if (Actor == WooHooer)
                    {
                        switch (RomanticType)
                        {
                            case RabbitHoleRomanticType.TryForBaby:
                            case RabbitHoleRomanticType.WooHoo:
							CommonWoohoo.RunPostWoohoo(WooHooer, WooHooee, Target, mStyle, CommonWoohoo.WoohooLocation.EiffelTower, true);
                                break;
                        }

                        Relationship relationship = Relationship.Get(WooHooer, WooHooee, true);
                        if (relationship != null)
                        {
                            relationship.UpdateSTCFromOutsideConversation(WooHooer, WooHooee, definition.VisitTuning.WooHooingCommodity, definition.VisitTuning.WooHooingSTCIncrement);
                        }
                    }

                    if ((mImpregnate) && (CommonPregnancy.IsSuccess(WooHooer, WooHooee, Autonomous, mStyle)))
                    {
                        CommonPregnancy.Impregnate(WooHooer, WooHooee, Autonomous, mStyle);
                    }

                    Target.RabbitHoleProxy.TurnOffWooHooEffect();*/
                }

                if (Actor.HasExitReason(ExitReason.StageComplete) || (Actor.HasExitReason(ExitReason.Finished) && !IsGettingItOn))
                {
					VisitRabbitHoleEx.GiveVisitingBuffs(Actor, Target, definition.VisitTuning, Origin.FromEiffelTower);
                    /*Career occupationAsCareer = Actor.OccupationAsCareer;
                    if ((occupationAsCareer != null) && (occupationAsCareer.CareerLoc.Owner == Target))
                    {
                        Actor.BuffManager.AddElement(BuffNames.Bored, definition.VisitBuffOrigin);
                        return succeeded;
                    }

                    float visitBoredomChanceAdult = definition.VisitTuning.VisitBoredomChanceAdult;
                    if (Actor.SimDescription.Teen)
                    {
                        visitBoredomChanceAdult = definition.VisitTuning.VisitBoredomChanceTeen;
                    }
                    else if (Actor.SimDescription.Child)
                    {
                        visitBoredomChanceAdult = definition.VisitTuning.VisitBoredomChanceChild;
                    }

                    if (RandomUtil.RandomChance(visitBoredomChanceAdult))
                    {
                        Actor.BuffManager.AddElement(BuffNames.Bored, definition.VisitBuffOrigin);
                        return succeeded;
                    }

                    BuffNames[] namesArray = new BuffNames[] { BuffNames.Excited, BuffNames.Fascinated, BuffNames.Intrigued, BuffNames.Impressed, BuffNames.Educated };
                    Actor.BuffManager.AddElement(namesArray[RandomUtil.GetInt(0x4)], definition.VisitBuffOrigin);*/
                }
                return succeeded;
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

		public new class ElevatorDefinition : VisitRabbitHoleEx.BaseDefinition<EiffelTower.TakeElevatorToTop> //RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition, IWooHooDefinition
		{
			//public bool IsGroupAddition;
			public RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition mBaseDefinition;
			/*public int Attempts
			{
				set
				{
				}
			}*/


			public ElevatorDefinition(string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin) : base(interactionName, visitTuning, visitBuffOrigin)
			{
				IsGroupAddition = true;
			}
			public ElevatorDefinition(RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition baseDef) : base(baseDef.InteractionName, baseDef.VisitTuning, baseDef.VisitBuffOrigin)
            {
				mBaseDefinition = baseDef;
			}

			/*public Sim ITarget(InteractionInstance paramInteraction)
			{
				TakeElevatorToTopEx takeElevatorToTopEx = paramInteraction as TakeElevatorToTopEx;
				if (takeElevatorToTopEx == null)
				{
					return null;
				}
				return takeElevatorToTopEx.WooHooee;
			}*/
			public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
			{
				return CommonWoohoo.WoohooLocation.EiffelTower;
			}
			public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
			{
				TakeElevatorToTopEx takeElevatorToTopEx = paramInteraction as TakeElevatorToTopEx;
				if (takeElevatorToTopEx == null)
				{
					return CommonWoohoo.WoohooStyle.Safe;
				}
				return takeElevatorToTopEx.mStyle;
			}
			/*public InteractionDefinition ProxyClone(Sim target)
			{
				throw new NotImplementedException();
			}*/

			public virtual InteractionDefinition CreateDefinition (RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition baseDef)
			{
				return new ElevatorDefinition (baseDef);
			}

			public override void AddInteractions (InteractionObjectPair iop, Sim actor, RabbitHole target, List<InteractionObjectPair> results)
			{
				List<InteractionObjectPair> iops = new List<InteractionObjectPair> ();
				mBaseDefinition.AddInteractions (iop, actor, target, iops);
				foreach (InteractionObjectPair current in iops)
				{
					results.Add(new InteractionObjectPair(CreateDefinition(current.InteractionDefinition as RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition), iop.Target));
				}
			}
			public override InteractionInstance CreateInstance (ref InteractionInstanceParameters parameters)
			{
				TakeElevatorToTopEx takeElevatorToTopEx = new TakeElevatorToTopEx();
				takeElevatorToTopEx.Init (ref parameters);
				return takeElevatorToTopEx;
			}
            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
				return mBaseDefinition != null ? mBaseDefinition.GetInteractionName (actor, target, iop) : Common.LocalizeEAString(actor.IsFemale, "Gameplay/Core/VisitCommunityLot:VisitNamedLot", new object[]
					{
						target.GetLocalizedName()
					});
			}
			public override string[] GetPath (bool isFemale)
			{
				return mBaseDefinition.GetPath (isFemale);
			}
			/*public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
			{
				if (isAutonomous && a.IsInGroupingSituation())
				{
					return false;
				}
				if (a.GetSituationOfType<GroupingSituation>() != null)
				{
					return IsGroupAddition;
				}
				return a.Posture == null || a.Posture.Container != target;
			}*/
        }

		public new class StairsDefinition : ElevatorDefinition
		{
			public StairsDefinition(string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin) : base(interactionName, visitTuning, visitBuffOrigin)
			{
			}
			public StairsDefinition(RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition baseDef) : base(baseDef)
			{
			}
			public override InteractionDefinition CreateDefinition (RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition baseDef)
			{
				return new StairsDefinition (baseDef);
			}
		}

		public class CustomInjector : CustomInjector<RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition>
		{
			protected override InteractionDefinition CreateElevatorDefinition (RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition baseDef)
			{
				return new ElevatorDefinition (baseDef);
			}
			protected override InteractionDefinition CreateStairsDefinition (RabbitHole.VisitRabbitHoleBase<EiffelTower.TakeElevatorToTop>.BaseDefinition baseDef)
			{
				return new StairsDefinition(baseDef);
			}
		}

		public abstract class CustomInjector<BASEDEF> : Common.InteractionInjector<EiffelTower> where BASEDEF : class, InteractionDefinition
        {
            protected abstract InteractionDefinition CreateElevatorDefinition (BASEDEF baseDef);
			protected abstract InteractionDefinition CreateStairsDefinition(BASEDEF baseDef);
            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
				EiffelTower tower = obj as EiffelTower;
				if (tower == null || tower.Interactions == null) return false;

				for (int i = obj.Interactions.Count - 1; i >= 0; i--)
				{
					BASEDEF current = obj.Interactions[i].InteractionDefinition as BASEDEF;
					if (current != null)
					{
						string fullName = current.GetType ().FullName;
						base.Perform (obj, fullName.Contains("Stairs") ? CreateStairsDefinition(current) : CreateElevatorDefinition(current), existing);
						obj.Interactions.RemoveAt (i);
					}
				}
				return true;
            }
		}
    }
}
