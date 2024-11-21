using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Objects.TombObjects;
using Sims3.SimIFace;
using Sims3.UI.Hud;
using static Sims3.Gameplay.Objects.TombObjects.Sarcophagus;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class SummonPlayableMummyEx : Sarcophagus.SummonPlayableMummy, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sarcophagus, Sarcophagus.SummonPlayableMummy.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sarcophagus, Sarcophagus.SummonPlayableMummy.Definition>(Singleton);
        }

        public override bool Run()
        {
            if (!this.Target.RouteToSarcaphagus(this.Actor, (Sim)null))
                return false;
            this.mJars = this.Target.GetMummyIngredients(this.Actor, true);
            if (this.mJars.Count != 5)
                return false;
            this.StandardEntry();
            Sim household = this.Target.AddPlayableMummyToHousehold(this.Actor.Household);
            if (household == null)
            {
                this.StandardExit();
                return false;
            }
            household.SetHiddenFlags(HiddenFlags.Everything);
            household.OccultManager.AddOccultType(OccultTypes.Mummy, true, false, false);
            this.EnterStateMachine(nameof(Sarcophagus), "Enter", "x", "sarcophagus");
            this.BeginCommodityUpdates();
            this.AnimateSim(nameof(SummonPlayableMummy));
            this.AnimateSim("Exit");
            if (!this.Target.TombMummyPushExitSarcophagus(household))
            {
                household.Destroy();
            }
            else
            {
                this.mDestroyItems = true;
                EventTracker.SendEvent(EventTypeId.kCreatedMonster, (IActor)this.Actor, (IGameObject)household);
            }
            this.StandardExit();
            return true;
        }

        public override void Cleanup()
        {
            if (this.mJars.Count == 5)
            {
                foreach (CollectableRelic mJar in this.mJars)
                {
                    if (this.mDestroyItems)
                    {
                        this.Actor.Inventory.RemoveByForce((IGameObject)mJar);
                        mJar.Destroy();
                    }
                    else
                        this.Actor.Inventory.SetNotInUse((IGameObject)mJar);
                }
            }
            base.Cleanup();
        }
        public new class Definition : Sarcophagus.SummonPlayableMummy.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SummonPlayableMummyEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sarcophagus target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sarcophagus target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.LotCurrent.IsResidentialLot || TombRoomManager.IsObjectInATombRoom((IGameObject)target) || !target.DoesSimHaveAllMummyIngredients(a))
                    return false;
                /*
                if (a.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.Human))
                    return true;
                greyedOutTooltipCallback = new GreyedOutTooltipCallback(Sarcophagus.SummonPlayableMummy.HouseholdTooBigGreyedOutTooltip);
                return false;
                */
                return true;
            }
        }
    }
}
