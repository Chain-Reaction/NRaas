using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class LeafPileWoohoo : LeafPileBaseWoohoo
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        public LeafPileWoohoo()
        { }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<LeafPile, LeafPile.PushWoohooInLeafPile.Definition>(SafeSingleton);
            interactions.Add<LeafPile>(RiskySingleton);
            interactions.Add<LeafPile>(TryForBabySingleton);
        }

        public override void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, LeafPile.WoohooInPileOrStack.Definition, ProxyDefinition>(false);

            InteractionTuning tuning = Tunings.GetTuning<Sim, LeafPile.WoohooInPileOrStack.Definition>();
            if (tuning != null)
            {
                tuning.Availability.RemoveFlags(Availability.FlagField.DisallowedIfPregnant);
                tuning.Availability.Teens = true;
            }

            tuning = Tunings.GetTuning<Sim, LeafPile.WoohooInPileOrStackB.Definition>();
            if (tuning != null)
            {
                tuning.Availability.RemoveFlags(Availability.FlagField.DisallowedIfPregnant);
                tuning.Availability.Teens = true;
                tuning.Availability.AddFlags(Availability.FlagField.AllowGreetedSims);
                tuning.Availability.AddFlags(Availability.FlagField.AllowNonGreetedSimsIfObjectOutsideAutonomous);
                tuning.Availability.AddFlags(Availability.FlagField.AllowNonGreetedSimsIfObjectOutsideUserDirected);
                tuning.Availability.AddFlags(Availability.FlagField.AllowOnCommunityLots);
                tuning.Availability.AddFlags(Availability.FlagField.AllowOnAllLots);
            }

            Woohooer.InjectAndReset<LeafPile, ProxyDefinition, SafeDefinition>(false);
            Woohooer.InjectAndReset<LeafPile, ProxyDefinition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<LeafPile, ProxyDefinition, TryForBabyDefinition>(false);

            LeafPile.PushWoohooInLeafPile.WoohooSingleton = SafeSingleton;
            LeafPile.PushWoohooInLeafPile.TryForBabySingleton = TryForBabySingleton;
        }

        protected override bool UsingNakedOutfit
        {
            get { return Woohooer.Settings.mNakedOutfitLeafPile; }
        }

        public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<Sim, LeafPileWoohoo, BaseLeafPileDefinition>
        {
            public ProxyDefinition(BaseLeafPileDefinition definition)
                : base(definition)
            { }

            public override Sim ITarget(InteractionInstance interaction)
            {
                return interaction.Target as Sim;
            }
        }

        public abstract class BaseLeafPileDefinition : CommonWoohoo.PotentialDefinition<LeafPile>
        {
            public BaseLeafPileDefinition()
            { }
            public BaseLeafPileDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.LeafPile;
            }

            public override Sim GetTarget(Sim actor, LeafPile target, InteractionInstance interaction)
            {
                LeafPile stall = target as LeafPile;

                List<Sim> sims = new List<Sim>(stall.ActorsUsingMe);
                sims.Remove(actor);

                if (sims.Count == 1)
                {
                    return sims[0];
                }
                else
                {
                    return base.GetTarget(actor, target, interaction);
                }
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                LeafPile house = obj as LeafPile;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                LeafPileWoohoo entry = new ProxyDefinition(this).CreateInstance(target, actor, priority, false, true) as LeafPileWoohoo;
                entry.WoohooObject = house;
                actor.InteractionQueue.PushAsContinuation(entry, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, LeafPile obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return obj.CanWooHooIn();
            }
        }

        public class SafeDefinition : BaseLeafPileDefinition
        {
            public SafeDefinition()
            { }
            public SafeDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                return CommonWoohoo.WoohooStyle.Safe;
            }

            public override string GetInteractionName(Sim actor, LeafPile target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, LeafPile obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "LeafPileWoohoo", isAutonomous, true, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeDefinition(target));
            }
        }

        public class RiskyDefinition : BaseLeafPileDefinition
        {
            public RiskyDefinition()
            { }
            public RiskyDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.Risky;
            }

            public override string GetInteractionName(Sim actor, LeafPile target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Woohooer.Settings.GetRiskyChanceText(actor);
            }

            protected override bool Satisfies(Sim a, Sim target, LeafPile obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesRisky(a, target, "LeafPileRisky", isAutonomous, true, ref greyedOutTooltipCallback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyDefinition(target));
            }
        }

        public class TryForBabyDefinition : BaseLeafPileDefinition
        {
            public TryForBabyDefinition()
            { }
            public TryForBabyDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return CommonWoohoo.WoohooStyle.TryForBaby;
            }

            public override string GetInteractionName(Sim actor, LeafPile target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, LeafPile obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(a, target, "LeafPileTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new TryForBabyDefinition(target));
            }
        }

        public class LocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.LeafPile; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is LeafPile;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<LeafPile>(new Predicate<LeafPile>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<LeafPile>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP8)) return false;
                }

                return Woohooer.Settings.mAutonomousLeafPile;
            }

            public bool TestUse(LeafPile obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (LeafPile obj in actor.LotCurrent.GetObjects<LeafPile>(new Predicate<LeafPile>(TestUse)))
                {
                    if ((testFunc != null) && (!testFunc(obj, null))) continue;

                    results.Add(obj);
                }

                return results;
            }

            public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
            {
                switch (style)
                {
                    case CommonWoohoo.WoohooStyle.Safe:
                        return new SafeDefinition(target);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return new RiskyDefinition(target);
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return new TryForBabyDefinition(target);
                }

                return null;
            }
        }
    }
}
