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
    public class CastUpgradeEx : MagicWand.MagicallyUpgrade, IMagicalSubInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        Proxy mProxy = new Proxy();

        public void OnPreLoad()
        {
            Tunings.Inject<GameObject, MagicWand.MagicallyUpgrade.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GameObject, MagicWand.MagicallyUpgrade.Definition>(Singleton);
        }

        public MagicWand Wand
        {
            set { mWand = value; }
        }

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

        public class Proxy : InteractionProxy<CastUpgradeEx, GameObject>
        {
            protected override bool SeparateCalls
            {
                get { return false; }
            } 

            protected override bool InitialPrep(CastUpgradeEx ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                Route r = ths.Actor.CreateRoute();
                if (!r.PlanToPointRadialRange(ths.Target.Position, 0.5f, kRoutingDistance, RouteDistancePreference.PreferNearestToRouteDestination, RouteOrientationPreference.TowardsObject, ths.Target.LotCurrent.LotId, new int[] { ths.Target.RoomId }).Succeeded() || !ths.Actor.DoRoute(r))
                {
                    return false;
                }

                return true;
            }

            protected override bool SetupAnimation(CastUpgradeEx ths, MagicControl control, bool twoPerson)
            {
                ths.EnterStateMachine("PracticeSpellcasting", "Enter", "x");

                return base.SetupAnimation(ths, control, twoPerson);
            }

            protected override bool PerformResults(CastUpgradeEx ths, string epicJazzName, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                ths.mUpgradeEffect = VisualEffect.Create("ep7ObjectUpgrade_main");
                ths.mUpgradeEffect.SetPosAndOrient(ths.Target.PositionOnFloor, ths.Target.ForwardVector, Vector3.UnitY);
                ths.mUpgradeEffect.Start();
                ths.AnimateSim("Practice");
                bool succeeded = ths.DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                ths.mUpgradeEffect.Stop();
                ths.mUpgradeEffect.Dispose();
                ths.mUpgradeEffect = null;
                if (succeeded && !ths.Actor.HasExitReason(ExitReason.CancelExternal))
                {
                    ths.AddOneShotScriptEventHandler(0x65, ths.ShowSuccessVfx);
                    ths.AddOneShotScriptEventHandler(0x66, ths.ShowFailVfx);
 
                    if (spellCastingSucceeded)
                    {
                        succeeded = true;
                        ths.AnimateSim("CastSuccess");
                        ths.Target.Upgradable.CurrentUpgrade = (ths.InteractionDefinition as Definition).SelectedUpgrade;
                    }
                    else if ((ths.Target.Repairable != null) && (spellCastingEpiclyFailed))
                    {
                        succeeded = false;
                        ths.AnimateSim("CastFail");
                        ths.Target.Repairable.BreakObject(ths.Actor, true);
                    }
                    else
                    {
                        succeeded = false;
                        ths.AnimateSim("CastFail");
                    }
                }
                else
                {
                    ths.AnimateSim("Exit");
                }

                return succeeded;
            }
        }

        public new class Definition : MagicWand.MagicallyUpgrade.Definition, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CastUpgradeEx();
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

                if (target.UpgradableComponent == null)
                {
                    return false;
                }

                if ((target.Repairable != null) && (target.Repairable.Broken))
                {
                    return false;
                }

                switch (SelectedUpgrade)
                {
                    case Upgrade.Unbreakable:
                        if (!target.Repairable.HasChanceOfBreaking())
                        {
                            return false;
                        }
                        break;

                    case Upgrade.BoostedChannelsOnTV:
                        {
                            ITelevision television = target as ITelevision;
                            if ((television != null) && television.HasAllChannels)
                            {
                                greyedOutTooltipCallback = delegate
                                {
                                    return Localization.LocalizeString("Gameplay/Objects/Electronics/TV/UpgradeBoostChannels:HasAllChannels", new object[0x0]);
                                };
                                return false;
                            }
                            break;
                        }
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
                    return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kSpellLevels[0x9], CastConvertEx.kMotiveDrain, 0);
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
