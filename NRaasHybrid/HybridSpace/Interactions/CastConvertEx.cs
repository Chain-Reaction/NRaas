using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using NRaas.HybridSpace.Interfaces;
using NRaas.HybridSpace.MagicControls;
using NRaas.HybridSpace.Proxies;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class CastConvertEx : MagicWand.CastConvert, IMagicalInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        Proxy mProxy = new Proxy();

        public void OnPreLoad()
        {
            Tunings.Inject<GameObject, MagicWand.CastConvert.Definition, Definition>(false);

            sOldSingleton = MagicWand.CastConvert.Singleton;
            MagicWand.CastConvert.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GameObject, MagicWand.CastConvert.Definition>(MagicWand.CastConvert.Singleton);
        }

        public MagicWand Wand
        {
            set { mWand = value; }
        }

        public void OnSpellSuccess()
        { }

        public override bool Run()
        {
            try
            {
                return mProxy.Run(this);
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

        public override void Cleanup()
        {
            base.Cleanup();

            mProxy.Cleanup();
        }

        public class Proxy : InteractionProxy<CastConvertEx, GameObject>
        {
            protected override bool InitialPrep(CastConvertEx ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                if (ths.Target.InInventory)
                {
                    if (ths.Target is Fish)
                    {
                        Fish target = ths.Target as Fish;
                        ths.Target = CreateFishBowlAndAddFish(target.Type);
                        if (ths.Target == null)
                        {
                            return false;
                        }
                        ths.Actor.Inventory.TryToRemove(target);
                    }
                    mSucceeded = PutDown.DisallowCarrySingleton.CreateInstance(ths.Target, ths.Actor, ths.GetPriority(), ths.Autonomous, ths.CancellableByPlayer).RunInteraction();
                }
                else if (!ths.Actor.RouteToPointRadius(ths.Target.Position, kRoutingDistance))
                {
                    return false;
                }

                if (!mSucceeded || ths.Actor.HasExitReason())
                {
                    return false;
                }

                return true;
            }

            protected override bool SetupAnimation(CastConvertEx ths, MagicControl control, bool twoPerson)
            {
                ths.EnterStateMachine("FireIceBlast", "Enter", "x");

                return base.SetupAnimation(ths, control, twoPerson);
            }

            protected override bool PerformResults(CastConvertEx ths, string epicJazzName, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                bool flag2 = false;
                IGameObject conjuredObject = ths.PreCreateObject();
                if (((conjuredObject is Fish) && !(ths.Target.Parent is ISurface)) || (conjuredObject is FailureObject))
                {
                    conjuredObject.Destroy();
                    conjuredObject = null;
                    flag2 = true;
                }

                if (conjuredObject != null)
                {
                    int num = kMaxConversionValue[0x1];
                    int skillLevel = control.GetSkillLevel (ths.Actor.SimDescription);
                    if ((skillLevel >= definition.SpellSettings.mMinSkillLevel) && (skillLevel <= 0xa))
                    {
                        num = kMaxConversionValue[skillLevel];
                    }
                    if (conjuredObject.Value > num)
                    {
                        flag2 = true;
                    }
                }

                bool succeeded = false;
                if (!flag2 && spellCastingSucceeded)
                {
                    succeeded = true;
                    ths.AnimateSim("SuccessIdle");
                    ths.AnimateSim("Success");
                    ths.OnSpellSuccess(conjuredObject);
                }
                else 
                {
                    succeeded = base.PerformResults(ths, epicJazzName, definition, control, false, spellCastingEpiclyFailed);
                }

                if (!succeeded && (conjuredObject != null))
                {
                    conjuredObject.Destroy();
                    conjuredObject = null;
                }

                return succeeded;
            }
        }

        public new class Definition : MagicWand.CastConvert.Definition, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CastConvertEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                MagicControl control = MagicControl.GetBestControl(actor, this);
                if (control == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Control");
                    return false;
                }

                if (target.GetContainedObject((Slot)(0xa820f8a6)) != null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Slot Full");
                    return false;
                }
                return true;
            }

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get
                {
                    return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kSpellLevels[0xb], CastConvertEx.kMotiveDrain, 0);
                }
            }

            public PersistedSettings.SpellSettings SpellSettings
            {
                get
                {
                    return sSettings;
                }
                set
                {
                    sSettings = value;
                }
            }
        }
    }
}
