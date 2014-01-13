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
    public class CastMotiveCurseEx : MagicWand.CastMotiveCurse, IMagicalInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        CastSpellProxy<CastMotiveCurseEx> mProxy = new CastSpellProxy<CastMotiveCurseEx>();

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, MagicWand.CastMotiveCurse.Definition, Definition>(false);

            sOldSingleton = MagicWand.CastMotiveCurse.Singleton;
            MagicWand.CastMotiveCurse.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, MagicWand.CastMotiveCurse.Definition>(MagicWand.CastMotiveCurse.Singleton);
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

        public new class Definition : MagicWand.CastMotiveCurse.Definition, IMagicalDefinition
        {
            PersistedSettings.SpellSettings mSettings;

            public Definition()
            { }
            public Definition(CommodityKind motive, IMagicalDefinition settings, MagicControl control)
                : base(motive, control.GetMinSkillLevel(settings))
            {
                mSettings = settings.SpellSettings;
            }

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get { return null; }
            }

            public PersistedSettings.SpellSettings SpellSettings
            {
                get
                {
                    return mSettings;
                }
                set
                { }
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CastMotiveCurseEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                MagicControl control = MagicControl.GetBestControl(actor, HungerDefinition.sSingleton);
                if (control != null)
                {
                    if (target.SimDescription.IsVampire)
                    {
                        results.Add(new InteractionObjectPair(new Definition(CommodityKind.VampireThirst, HungerDefinition.sSingleton, control), target));
                    }
                    else
                    {
                        results.Add(new InteractionObjectPair(new Definition(CommodityKind.Hunger, HungerDefinition.sSingleton, control), target));
                    }
                }

                control = MagicControl.GetBestControl(actor, BladderDefinition.sSingleton);
                if (control != null)
                {
                    results.Add(new InteractionObjectPair(new Definition(CommodityKind.Bladder, BladderDefinition.sSingleton, control), target));
                }

                control = MagicControl.GetBestControl(actor, EnergyDefinition.sSingleton);
                if (control != null)
                {
                    results.Add(new InteractionObjectPair(new Definition(CommodityKind.Energy, EnergyDefinition.sSingleton, control), target));
                }

                control = MagicControl.GetBestControl(actor, SocialDefinition.sSingleton);
                if (control != null)
                {
                    results.Add(new InteractionObjectPair(new Definition(CommodityKind.Social, SocialDefinition.sSingleton, control), target));
                }

                control = MagicControl.GetBestControl(actor, HygieneDefinition.sSingleton);
                if (control != null)
                {
                    results.Add(new InteractionObjectPair(new Definition(CommodityKind.Hygiene, HygieneDefinition.sSingleton, control), target));
                }

                control = MagicControl.GetBestControl(actor, FunDefinition.sSingleton);
                if (control != null)
                {
                    results.Add(new InteractionObjectPair(new Definition(CommodityKind.Fun, FunDefinition.sSingleton, control), target));
                }
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a == target)
                {
                    return false;
                }

                return CastSpellEx.CommonSpellTests(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }

        public class HungerDefinition : IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public static HungerDefinition sSingleton = new HungerDefinition();

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get { return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kMotiveCurseUnlockLevels[0x1], CastMotiveCharmEx.kMotiveDrain, 0); }
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

        public class BladderDefinition : IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public static BladderDefinition sSingleton = new BladderDefinition();

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get { return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kMotiveCurseUnlockLevels[0x0], CastMotiveCharmEx.kMotiveDrain, 0); }
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

        public class EnergyDefinition : IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public static EnergyDefinition sSingleton = new EnergyDefinition();

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get { return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kMotiveCurseUnlockLevels[0x2], CastMotiveCharmEx.kMotiveDrain, 0); }
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

        public class SocialDefinition : IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public static SocialDefinition sSingleton = new SocialDefinition();

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get { return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kMotiveCurseUnlockLevels[0x5], CastMotiveCharmEx.kMotiveDrain, 0); }
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

        public class HygieneDefinition : IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public static HygieneDefinition sSingleton = new HygieneDefinition();

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get { return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kMotiveCurseUnlockLevels[0x3], CastMotiveCharmEx.kMotiveDrain, 0); }
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

        public class FunDefinition : IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public static FunDefinition sSingleton = new FunDefinition();

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get { return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kMotiveCurseUnlockLevels[0x4], CastMotiveCharmEx.kMotiveDrain, 0); }
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
