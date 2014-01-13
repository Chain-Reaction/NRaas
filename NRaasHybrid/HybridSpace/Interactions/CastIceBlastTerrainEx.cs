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
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.FireFightingObjects;
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
    public class CastIceBlastTerrainEx : MagicWand.CastIceBlastTerrain, IMagicalInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        CastIceBlastTerrainProxy mProxy = new CastIceBlastTerrainProxy();

        public void OnPreLoad()
        {
            Tunings.Inject<Fire, MagicWand.CastIceBlastTerrain.Definition, Definition>(false);

            sOldSingleton = MagicWand.CastIceBlastTerrain.Singleton;
            MagicWand.CastIceBlastTerrain.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Fire, MagicWand.CastIceBlastTerrain.Definition>(MagicWand.CastIceBlastTerrain.Singleton);
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

        public class CastIceBlastTerrainProxy : FireIceBlastProxy<CastIceBlastTerrainEx, IExtinguishable>
        {
            protected override bool InitialPrep(CastIceBlastTerrainEx ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                ths.RequestWalkStyle(Sim.WalkStyle.Run);
                Route r = ths.Target.PlanRouteToExtinguish(ths.Actor);
                if (!r.PlanResult.Succeeded() || !ths.Actor.DoRoute(r))
                {
                    return false;
                }

                return true;
            }

            protected override bool PerformResults(CastIceBlastTerrainEx ths, string epicJazzName, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
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

        public new class Definition : InteractionDefinition<Sim, Fire, CastIceBlastTerrainEx>, IUsableDuringFire, IOverridesVisualType, IHasTraitIcon, IHasMenuPathIcon, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { (MagicWand.LocalizeString(isFemale, "CastSpell", new object[0x0]) + Localization.Ellipsis) };
            }

            public ResourceKey GetPathIcon(Sim actor, GameObject target)
            {
                return ResourceKey.CreatePNGKey("trait_SpellcastingTalent_s", ResourceUtils.ProductVersionToGroupId(ProductVersion.EP7));
            }

            public ResourceKey GetTraitIcon(Sim actor, GameObject target)
            {
                return ResourceKey.CreatePNGKey("trait_SpellcastingTalent_s", ResourceUtils.ProductVersionToGroupId(ProductVersion.EP7));
            }

            public override string GetInteractionName(Sim actor, Fire target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Fire target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                MagicControl control = MagicControl.GetBestControl(a, this);
                if (control == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Control");
                    return false;
                }

                return !target.HasBeenDestroyed;
            }

            public InteractionVisualTypes GetVisualType
            {
                get
                {
                    return InteractionVisualTypes.Trait;
                }
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
