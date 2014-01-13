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
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Alchemy;
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
    public class CastIceBlastPlumbingEx : MagicWand.CastIceBlastPlumbing, IMagicalInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        Proxy mProxy = new Proxy();

        public void OnPreLoad()
        {
            Tunings.Inject<GameObject, MagicWand.CastIceBlastPlumbing.Definition, Definition>(false);

            sOldSingleton = MagicWand.CastIceBlastPlumbing.Singleton;
            MagicWand.CastIceBlastPlumbing.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GameObject, MagicWand.CastIceBlastPlumbing.Definition>(MagicWand.CastIceBlastPlumbing.Singleton);
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

        public class Proxy : FireIceBlastProxy<CastIceBlastPlumbingEx, GameObject>
        {
            protected override bool InitialPrep(CastIceBlastPlumbingEx ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                Route r = ths.Actor.CreateRoute();
                if (!r.PlanToPointRadialRange(ths.Target.Position, 0.5f, kRoutingDistance, RouteDistancePreference.PreferNearestToRouteDestination, RouteOrientationPreference.TowardsObject, ths.Target.LotCurrent.LotId, new int[] { ths.Target.RoomId }).Succeeded() || !ths.Actor.DoRoute(r))
                {
                    return false;
                }

                return true;
            }

            protected override bool PerformResults(CastIceBlastPlumbingEx ths, string epicJazzName, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                if (spellCastingSucceeded)
                {
                    ths.AnimateSim("SuccessIdle");
                    SpeedTrap.Sleep((uint)SimClock.ConvertToTicks(2f, TimeUnit.Minutes));
                    ths.AnimateSim("Success");
                    ths.OnSpellSuccess();
                    return true;
                }
                else
                {
                    return base.PerformResults(ths, epicJazzName, definition, control, false, spellCastingEpiclyFailed);
                }
            }
        }

        public new class Definition : MagicWand.CastIceBlastPlumbing.Definition, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CastIceBlastPlumbingEx();
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

                if (target.UpgradableComponent.Unbreakable)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(MagicWand.LocalizeString(actor.IsFemale, "CantBreakWithIceBlast", new object[0x0]));
                    return false;
                }

                return ((target.Repairable != null) && !target.Repairable.Broken);
            }

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get
                {
                    return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kSpellLevels[0x5], CastFireBlastEx.kMotiveDrain, 0);
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
