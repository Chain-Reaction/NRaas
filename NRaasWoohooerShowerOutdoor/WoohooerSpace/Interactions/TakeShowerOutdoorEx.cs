using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;

namespace NRaas.WoohooerSpace.Interactions
{
    public class TakeShowerOutdoorEx : TakeShowerEx
    {
        static InteractionDefinition sOldSingleton;

        public override void OnPreLoad()
        {
            Tunings.Inject<ShowerOutdoor, ShowerOutdoor.TakeShower.Definition, Definition>(false);

            sOldSingleton = ShowerOutdoor.TakeShower.Singleton;
            ShowerOutdoor.TakeShower.Singleton = new Definition();
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ShowerOutdoor, ShowerOutdoor.TakeShower.Definition>(ShowerOutdoor.TakeShower.Singleton);

            // The other interactions are added by ShowerWoohoo itself
            interactions.Add<IShowerable>(ShowerWoohoo.SafeSingleton);
        }

        protected override Sim.ClothesChangeReason OutfitChoice
        {
            get
            {
                if (HavingWooHoo)
                {
                    return Sim.ClothesChangeReason.GoingToBathe;
                }
                else
                {
                    return Sim.ClothesChangeReason.GoingToSwim;
                }
            }
        }

        protected override bool EnforcePrivacy
        {
            get { return false; }
        }

		public new void DuringShower(StateMachineClient smc, LoopData loopData)
		{
			if (Actor.SimDescription.IsFrankenstein)
			{
				OccultFrankenstein.PushFrankensteinShortOut(Actor);
			}

			if (HavingWooHoo)
			{
				return;
			}

			if (Actor.SimDescription.IsMatureMermaid)
			{
				if (Actor.Motives.IsMax(CommodityKind.MermaidDermalHydration))
				{
					Actor.AddExitReason(ExitReason.Finished);
				}
			}

			else if (Actor.Motives.IsMax(CommodityKind.Hygiene))
			{
				Actor.AddExitReason(ExitReason.Finished);
			}

			if (RandomUtil.RandomChance((float)Target.TuningShower.ChanceOfSinging))
			{
				Actor.TrySinging();
			}

			EventTracker.SendEvent(EventTypeId.kEventTakeBath, Actor, Target);
			EventTracker.SendEvent(EventTypeId.kEventTakeShower, Actor, Target);
		}

		public override bool Run()
        {
			if (!Target.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.DefaultEvict, ExitReason.Default, Shower.kTimeToWaitToEvict))
			{
				return false;
			}

			try
			{
				mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GoingToSwim);
			} catch
            {
				return false;
            }

			mSwitchOutfitHelper.Start();

			if (Actor.HasTrait(TraitNames.Hydrophobic))
			{
				Actor.PlayReaction(ReactionTypes.WhyMe, Target as GameObject, ReactionSpeed.ImmediateWithoutOverlay);
			}

			if (Actor.HasTrait(TraitNames.Daredevil))
			{
				TraitTipsManager.ShowTraitTip(0xb82d0015b9294260L, Actor, TraitTipsManager.TraitTipCounterIndex.Daredevil, TraitTipsManager.kDaredevilCountOfShowersTaken);
			}

			if (!Actor.RouteToSlotAndCheckInUse(Target, Slot.RoutingSlot_0))
			{
				return false;
			}

			StandardEntry();
			if (!Actor.RouteToSlot(Target, Slot.RoutingSlot_0))
			{
				StandardExit();
				return false;
			}

			if (base.Autonomous)
			{
				mPriority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
			}

			mSwitchOutfitHelper.Wait(true);
			bool daredevilPerforming = Actor.DaredevilPerforming;
			bool flag = Actor.GetCurrentOutfitCategoryFromOutfitInGameObject() == OutfitCategories.Singed;
			EnterStateMachine("Shower", "Enter", "x");
			SetActor("Shower", Target);
			SetParameter("IsShowerTub", Target.IsShowerTub);
			SetParameter("SimShouldCloseDoor", true);
			SetParameter("SimShouldClothesChange", !daredevilPerforming && !flag && !Actor.OccultManager.DisallowClothesChange() && mSwitchOutfitHelper.WillChange);
			bool paramValue = false;
			SetParameter("isBoobyTrapped", paramValue);
			mSwitchOutfitHelper.AddScriptEventHandler(this);
			AddOneShotScriptEventHandler(1001u, EventCallbackStartShoweringSound);

			if (Actor.HasTrait(TraitNames.Virtuoso) || RandomUtil.RandomChance(Target.TuningShower.ChanceOfSinging))
			{
				AddOneShotScriptEventHandler(200u, EventCallbackStartSinging);
			}

			PetStartleBehavior.CheckForStartle(Target as GameObject, StartleType.ShowerOn);
			AnimateSim("Loop Shower");
			Actor.BuffManager.AddElement(BuffNames.SavingWater, Origin.FromShower, ProductVersion.EP2, TraitNames.EnvironmentallyConscious);
			mShowerStage.ResetCompletionTime(GetShowerTime());
			StartStages();

			if (Actor.HasTrait(TraitNames.EnvironmentallyConscious))
			{
				BeginCommodityUpdate(CommodityKind.Hygiene, Shower.kEnvironmentallyConsciousShowerSpeedMultiplier);
			}

			if (Actor.SimDescription.IsPlantSim)
			{
				ModifyCommodityUpdate(CommodityKind.Hygiene, Shower.kPlantSimHygieneModifier);
			}

			BeginCommodityUpdates();
			bool succeeded = false;
			try
			{
				Target.SimInShower = Actor;
				succeeded = DoLoop(ExitReason.Default, DuringShower, null);
			}
			finally
			{
				Target.SimInShower = null;
			}

			EndCommodityUpdates(succeeded);
			Shower.WaitToLeaveShower(Actor, Target);

			if (succeeded)
			{
				ShowerOutdoor.ApplyPostShowerEffects(Actor, Target);
			}

			if (flag && succeeded)
			{
				mSwitchOutfitHelper.Dispose();
				mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GoingToSwim);
				mSwitchOutfitHelper.Start();
				mSwitchOutfitHelper.Wait(false);
				mSwitchOutfitHelper.ChangeOutfit();
			}

			bool flag3 = false;
			if ((flag && succeeded) || (!flag && !daredevilPerforming))
			{
				SetParameter("SimShouldClothesChange", !Actor.OccultManager.DisallowClothesChange());
				mSwitchOutfitHelper.Dispose();
				mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(Actor, Sim.ClothesChangeReason.GettingOutOfBath);
				mSwitchOutfitHelper.Start();
				mSwitchOutfitHelper.AddScriptEventHandler(this);
				mSwitchOutfitHelper.Wait(false);
				flag3 = true;
			}

			AddOneShotScriptEventHandler(201u, EventCallbackStopSinging);
			AddOneShotScriptEventHandler(1002u, EventCallbackStopShoweringSound);
			if (flag3 && InventingSkill.IsBeingDetonated(Target as GameObject))
			{
				SetParameter("SimShouldClothesChange", false);
				mSwitchOutfitHelper.Abort();
				mSwitchOutfitHelper.Dispose();
			}

			AnimateSim("Exit Working");
			if (Actor.SimDescription.IsMummy || Actor.DaredevilPerforming || (Actor.TraitManager.HasElement(TraitNames.Slob) && RandomUtil.RandomChance01(TraitTuning.SlobTraitChanceToLeavePuddle)))
			{
				PuddleManager.AddPuddle(Actor.Position);
			}

			if (succeeded)
			{
				Actor.BuffManager.RemoveElement(BuffNames.GotFleasHuman);
			}

			StandardExit();
			return succeeded;
		}

        public new class Definition : TakeShowerEx.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TakeShowerOutdoorEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, IShowerable target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, IShowerable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
				if (isAutonomous && ((a.Autonomy.Motives.GetValue(CommodityKind.Hygiene) >= 100f && a.SimDescription != null && !a.SimDescription.IsMermaid) || (a.SimDescription != null && a.SimDescription.IsMermaid && a.Autonomy.Motives.GetValue(CommodityKind.MermaidDermalHydration) >= 100f) || a.SimDescription.IsRobot))
				{
					return false;
				}

				return true;
			}
        }
    }
}


