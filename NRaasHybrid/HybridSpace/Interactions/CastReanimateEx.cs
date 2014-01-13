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
using Sims3.Gameplay.Objects;
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
    public class CastReanimateEx : MagicWand.CastReanimate, IMagicalInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        Proxy mProxy = new Proxy();

        public void OnPreLoad()
        {
            Tunings.Inject<Urnstone, MagicWand.CastReanimate.Definition, Definition>(false);

            sOldSingleton = MagicWand.CastReanimate.Singleton;
            MagicWand.CastReanimate.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Urnstone, MagicWand.CastReanimate.Definition>(MagicWand.CastReanimate.Singleton);
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

        public class Proxy : FireIceBlastProxy<CastReanimateEx, Urnstone>
        {
            protected override bool InitialPrep(CastReanimateEx ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                if (!ths.Actor.RouteToPointRadius(ths.Target.Position, kRoutingDistance))
                {
                    return false;
                }

                return true;
            }
        }

        public new class Definition : MagicWand.CastReanimate.Definition, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CastReanimateEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Urnstone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim actor, Urnstone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                MagicControl control = MagicControl.GetBestControl(actor, this);
                if (control == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Control");
                    return false;
                }

                if (target == null)
                {
                    return false;
                }
                if (target.DeadSimsDescription == null)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Gameplay/ActorSystems/OccultWitch:SimMovedOn", new object[0x0]));
                    return false;
                }
                if (target.DeadSimsDescription.ChildOrBelow)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Gameplay/ActorSystems/OccultWitch:ReviveDead", new object[0x0]));
                    return false;
                }
                if (target.DeadSimsDescription.IsPet)
                {
                    return false;
                }
                if ((target.DeadSimsDescription.CreatedSim != null) && target.DeadSimsDescription.CreatedSim.IsDying())
                {
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
                    return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kSpellLevels[0xa], CastConvertEx.kMotiveDrain, 0);
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
