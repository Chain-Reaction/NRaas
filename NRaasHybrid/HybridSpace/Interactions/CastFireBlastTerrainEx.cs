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
    public class CastFireBlastTerrainEx : MagicWand.CastFireBlastTerrain, IMagicalInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        Proxy mProxy = new Proxy();

        public void OnPreLoad()
        {
            Tunings.Inject<Terrain, MagicWand.CastFireBlastTerrain.Definition, Definition>(false);

            sOldSingleton = MagicWand.CastFireBlastTerrain.Singleton;
            MagicWand.CastFireBlastTerrain.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Terrain, MagicWand.CastFireBlastTerrain.Definition>(MagicWand.CastFireBlastTerrain.Singleton);
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

        public class Proxy : FireIceBlastProxy<CastFireBlastTerrainEx, Terrain>
        {
            protected override bool InitialPrep(CastFireBlastTerrainEx ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                Vector3 targetPosition = ths.GetTargetPosition();
                Route r = ths.Actor.CreateRoute();
                RoutePlanResult result = r.PlanToPointRadius(targetPosition, kRoutingDistance, RouteOrientationPreference.TowardsObject, LotManager.GetLotAtPoint(targetPosition).LotId, new int[] { World.GetRoomId(targetPosition) });
                if (!result.Succeeded() || World.IsInPool(result.mDestination))
                {
                    ths.Actor.PlayRouteFailure();
                    return false;
                }

                if (!ths.Actor.DoRoute(r))
                {
                    return false;
                }

                return true;
            }
        }

        public new class Definition : MagicWand.CastFireBlastTerrain.Definition, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CastFireBlastTerrainEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Terrain target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim actor, Terrain target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                MagicControl control = MagicControl.GetBestControl(actor, this);
                if (control == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Control");
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
                    return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kSpellLevels[0x4], CastFireBlastEx.kMotiveDrain, 0);
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
