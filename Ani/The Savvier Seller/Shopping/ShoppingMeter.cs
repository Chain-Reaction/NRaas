using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay;
using Sims3.SimIFace;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Utilities;
using ani_StoreSetRegister;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Autonomy;
using ani_StoreSetBase.Shopping;

namespace Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase.Shopping
{
    public class ShoppingMeter : GameObject
    {
        private ShoppingProgress mProgress;

        //  private ShoppingMeter.SecondBoneMove mBoneMoveHelper;

        #region Initial Stuff
        private enum GlowType
        {
            None,
            Weak,
            Strong
        }

        private const string kBlueBoneName = "_capsule_liquid_increases_";
        private const string kGreenBoneName = "_capsule_liquid_twoTone_";
        private const float kMaxFluidHeight = 0.3f;
        private Sim mSim;
        private StateMachineClient mSmc;
        private VisualEffect mGlowFx;
        private VisualEffect mSkillBarFx;
        private ShoppingMeter.GlowType mCurrentGlowType;
        private AlarmHandle mPlusPlusAlarm = AlarmHandle.kInvalidHandle;
        private AlarmHandle mChunkGrowthAlarm = AlarmHandle.kInvalidHandle;

        [Tunable]
        private static float kNoGlowRateModifierUpperLimit = 0.1f;
        [Tunable]
        private static float kStrongGlowRateModifierLowerLimit = 1f;

        // [Tunable]
        //  private static float kMinutesPerPlusPlusEffect = 5f;
        #endregion

        public override void OnStartup()
        {
            base.AddInteraction(TestInteraction.Singleton);
            base.OnStartup();
        }

        public class TestInteraction : Interaction<Sim, ShoppingMeter>
        {
            public class Definition : InteractionDefinition<Sim, ShoppingMeter, TestInteraction>
            {
                public override string GetInteractionName(Sim a, ShoppingMeter target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("TestInteraction", new object[0] { });
                }

                public override bool Test(Sim a, ShoppingMeter target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new TestInteraction.Definition();

            public override bool Run()
            {
                CMStoreSet.ShowDialogue("", "", this.Target.GetResourceKey().ToString());

                return true;
            }

        }

        private static ShoppingMeter CreateShoppingMeter()
        {
            //319e4f1d:00000000:5dfd7f143454e12c
            return GlobalFunctions.CreateObject(ResourceKey.FromString("319e4f1d:00000000:5dfd7f143454e12c"), Vector3.OutOfWorld, 1, Vector3.UnitZ) as ShoppingMeter;
        }

        public static ShoppingMeter StartShoppingGain(Sim sim, ShoppingProgress progress, float overallModifier)
        {
            if (Cheats.sHeadlineEffectsHidden)
            {
                return null;
            }

            ShoppingMeter shoppingMeter = ShoppingMeter.CreateShoppingMeter();

            if (shoppingMeter != null)
            {
                shoppingMeter.mProgress = progress;
                shoppingMeter.mSim = sim;
                shoppingMeter.UpdateGainRateGlow(overallModifier);
                shoppingMeter.ShowMeter();
                return shoppingMeter;
            }
            return null;
        }

        internal void UpdateGainRateGlow(float overallModifier)
        {

            float skillGainModifier = 100;// this.mSkillBeingGained.SkillOwner.SkillManager.GetSkillGainModifier(this.mSkillBeingGained.Guid);
            float num = skillGainModifier + overallModifier;

            if (num < ShoppingMeter.kNoGlowRateModifierUpperLimit)
            {
                this.SwitchGlow(ShoppingMeter.GlowType.None);
                return;
            }
            if (num > ShoppingMeter.kStrongGlowRateModifierLowerLimit)
            {
                this.SwitchGlow(ShoppingMeter.GlowType.Strong);
                return;
            }
            this.SwitchGlow(ShoppingMeter.GlowType.Weak);
        }

        private void SwitchGlow(ShoppingMeter.GlowType newType)
        {
            if (this.mCurrentGlowType != newType)
            {
                if (this.mGlowFx != null)
                {
                    this.mGlowFx.Stop();
                    this.mGlowFx.Dispose();
                    this.mGlowFx = null;
                }
                switch (newType)
                {
                    case ShoppingMeter.GlowType.Weak:
                        {
                            this.mGlowFx = VisualEffect.Create("skillMeterGlowWeak");
                            break;
                        }
                    case ShoppingMeter.GlowType.Strong:
                        {
                            this.mGlowFx = VisualEffect.Create("skillMeterGlowStrong");
                            break;
                        }
                }
                if (this.mGlowFx != null && this.mSmc != null)
                {
                    this.mGlowFx.ParentTo(this, Slot.FXJoint_0);
                    this.mGlowFx.Start();
                }
                this.mCurrentGlowType = newType;
            }
        }

        private void ShowMeter()
        {
            Animation.CreateTransformController(base.ObjectId);
            this.mSmc = StateMachineClient.Acquire(this.mSim.Proxy.ObjectId, "SkillMeter", AnimationPriority.kAPDefault, false);
            this.mSmc.SetActor("x", this.mSim);
            this.mSmc.SetActor("SkillMeter", this);
            this.mSmc.EnterState("SkillMeter", "Enter");
            this.mSmc.AddOneShotScriptEventHandler(100u, new SacsEventHandler(this.StartGlow));
            this.mSmc.RequestState(false, "SkillMeter", "Open");
            this.UpdateProgress(true);
            this.mPlusPlusAlarm = base.AddAlarmRepeating(this.mProgress.kMinutesPerPlusPlusEffect, TimeUnit.Minutes, new AlarmTimerCallback(this.ShowPlusPlus), this.mProgress.kMinutesPerPlusPlusEffect, TimeUnit.Minutes, "Plus Plus Alarm", AlarmType.AlwaysPersisted);
        }

        public void HideMeter()
        {
            this.mChunkGrowthAlarm = AlarmHandle.kInvalidHandle;
            base.RemoveAlarm(this.mPlusPlusAlarm);
            if (this.mSmc != null && this.mSmc.IsValid)
            {
                this.mSmc.RequestState(false, "SkillMeter", "Exit");
            }
            this.StopGlow();
            this.Destroy();
        }

        private void ShowPlusPlus()
        {
            this.mProgress.ProgressPoints += 0.1f;

            this.UpdateProgress(true);

            if (this.mProgress.ProgressPoints >= 1f)
                HideMeter();

            switch (this.mCurrentGlowType)
            {
                case ShoppingMeter.GlowType.Weak:
                    {
                        VisualEffect.FireOneShotEffect("skillMeterPlusFx", this, Slot.FXJoint_0, VisualEffect.TransitionType.SoftTransition);
                        return;
                    }
                case ShoppingMeter.GlowType.Strong:
                    {
                        VisualEffect.FireOneShotEffect("skillMeterPlusPlusFx", this, Slot.FXJoint_0, VisualEffect.TransitionType.SoftTransition);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }
               
        public void UpdateProgress(bool fromShowMeter)
        {
            if (!fromShowMeter)
            {
                if (this.mSkillBarFx != null)
                {
                    this.mSkillBarFx.Stop();
                    this.mSkillBarFx.Dispose();
                    this.mSkillBarFx = null;
                }
            }
            this.MoveBones(this.mProgress.Progress);
        }

        private void MoveBones(float barY)
        {
            ShoppingMeter.NormalizeProgress(ref barY, 0.3f);
            Vector3 pos = new Vector3(0f, barY, 0f);
            Animation.SetBonePosition(base.ObjectId, "_capsule_liquid_increases_", pos, 0f);
            Animation.SetBonePosition(base.ObjectId, "_capsule_liquid_twoTone_", pos, 0f);
        }

        public static void NormalizeProgress(ref float currentProgress, float maxFluidHeight)
        {
            if (currentProgress < 0f)
            {
                currentProgress = 0f;
            }
            else
            {
                if (currentProgress > 1f)
                {
                    currentProgress = 1f;
                }
            }
            currentProgress *= maxFluidHeight;
        }

        private void StartGlow(StateMachineClient sender, IEvent evt)
        {
            if (this.mGlowFx != null)
            {
                this.mGlowFx.ParentTo(this, Slot.FXJoint_0);
                this.mGlowFx.Start();
            }
        }

        private void StopGlow()
        {
            if (this.mGlowFx != null)
            {
                this.mGlowFx.Stop();
                this.mGlowFx.Dispose();
                this.mGlowFx = null;
            }
            if (this.mSkillBarFx != null)
            {
                this.mSkillBarFx.Stop();
                this.mSkillBarFx.Dispose();
                this.mSkillBarFx = null;
            }
        }





    }

}
