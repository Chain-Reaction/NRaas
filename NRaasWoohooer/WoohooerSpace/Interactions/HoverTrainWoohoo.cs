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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class HoverTrainWoohoo : HoverTrainStation.TravelToWith, Common.IPreLoad, Common.IAddInteraction
    {
        static readonly InteractionDefinition SafeSingleton = new SafeDefinition();
        static readonly InteractionDefinition RiskySingleton = new RiskyDefinition();
        static readonly InteractionDefinition TryForBabySingleton = new TryForBabyDefinition();

        Sim mWoohooee;

        HoverTrainStation mDestination;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<HoverTrainStation, HoverTrainStation.PushWoohooOnTrain.Definition>(SafeSingleton);
            interactions.Add<HoverTrainStation>(RiskySingleton);
            interactions.Add<HoverTrainStation>(TryForBabySingleton);
        }

        public void OnPreLoad()
        {
            Woohooer.InjectAndReset<Sim, HoverTrainStation.PushWoohooOnTrain.Definition, ProxyDefinition>(false);

            InteractionTuning tuning = Tunings.GetTuning<Sim, HoverTrainStation.WooHooSocialInteraction.Definition>();
            if (tuning != null)
            {
                tuning.Availability.RemoveFlags(Availability.FlagField.DisallowedIfPregnant);
            }

            tuning = Tunings.GetTuning<Sim, HoverTrainStation.WooHooSocialInteraction.Definition>();
            if (tuning != null)
            {
                tuning.Availability.RemoveFlags(Availability.FlagField.DisallowedIfPregnant);
                tuning.Availability.Teens = true;
            }

            Woohooer.InjectAndReset<HoverTrainStation, ProxyDefinition, SafeDefinition>(false);
            Woohooer.InjectAndReset<HoverTrainStation, ProxyDefinition, RiskyDefinition>(false);
            Woohooer.InjectAndReset<HoverTrainStation, ProxyDefinition, TryForBabyDefinition>(false);

            HoverTrainStation.PushWoohooOnTrain.WoohooSingleton = SafeSingleton;
            HoverTrainStation.PushWoohooOnTrain.TryForBabySingleton = TryForBabySingleton;
        }

        public override Lot GetTargetLot()
        {
            if (mDestination != null)
            {
                return mDestination.LotCurrent;
            }
            return null;
        }

        public override bool Run()
        {
            try
            {
                IWooHooDefinition definition = InteractionDefinition as IWooHooDefinition;

                if (mWoohooee == null)
                {
                    mWoohooee = definition.ITarget(this);
                }

                Tutorialette.TriggerLesson(Lessons.FutureTravel, Actor);

                List<Sim> followersFromSelectedObjects = new List<Sim>();
                followersFromSelectedObjects.Add(mWoohooee);

                bool flag = mDestination.RouteOutside(Actor, followersFromSelectedObjects);

                CommonWoohoo.RunPostWoohoo(Actor, mWoohooee, Target, definition.GetStyle(this), definition.GetLocation(Target), true);

                Actor.BuffManager.AddElement(BuffNames.MissedMyStop, Origin.FromWooHooOnHoverTrain);

                if (CommonPregnancy.IsSuccess(Actor, mWoohooee, Autonomous, definition.GetStyle(this)))
                {
                    CommonPregnancy.Impregnate(Actor, mWoohooee, Autonomous, definition.GetStyle(this));
                }

                mWoohooee.BuffManager.AddElement(BuffNames.MissedMyStop, Origin.FromWooHooOnHoverTrain);

                return flag;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
                return false;
            }
        }

        // Hover Train woohoo requires that the definition derive from ItemDefinition
        public abstract class PrimaryProxyDefinition<TDefinition> : ItemDefinition, IWooHooDefinition
            where TDefinition : IWooHooProxyDefinition
        {
            TDefinition mDefinition;

            public PrimaryProxyDefinition(TDefinition definition)
            {
                mIsWooHoo = true;
                mMenuPath = new string[0];

                mDefinition = definition;
            }

            public TDefinition Definition
            {
                get { return mDefinition; }
            }

            public override string GetInteractionName(Sim actor, HoverTrainStation target, InteractionObjectPair iop)
            {
                return mDefinition.GetName(actor, target, iop);
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return InteractionTestResult.Pass;
            }

            public override bool Test(Sim actor, HoverTrainStation target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }

            public virtual Sim ITarget(InteractionInstance interaction)
            {
                return mDefinition.ITarget(interaction);
            }

            public CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return mDefinition.GetLocation(obj);
            }

            public CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return mDefinition.GetStyle(interaction);
            }

            public int Attempts
            {
                set { mDefinition.Attempts = value; }
            }

            public InteractionDefinition ProxyClone(Sim target)
            {
                return mDefinition.ProxyClone(target);
            }
        }

        public class ProxyDefinition : PrimaryProxyDefinition<BaseHoverTrainDefinition>
        {
            public ProxyDefinition(BaseHoverTrainDefinition definition)
                : base(definition)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new HoverTrainWoohoo();
                na.Init(ref parameters);
                return na;
            }
        }

        public abstract class BaseHoverTrainDefinition : CommonWoohoo.PotentialDefinition<HoverTrainStation>
        {
            public BaseHoverTrainDefinition()
            { }
            public BaseHoverTrainDefinition(Sim target)
                : base(target)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.HoverTrain;
            }

            public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
            {
                HoverTrainStation station = obj as HoverTrainStation;

                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);

                HoverTrainStation[] listOfObjects = HoverTrainStation.GetHoverTrainStations().ToArray();
                if (listOfObjects.Length <= 1) return;
  
                int choiceIndex = RandomUtil.GetInt(listOfObjects.Length - 0x1);
                HoverTrainStation destination = listOfObjects[choiceIndex];
                if (destination == station)
                {
                    choiceIndex++;
                    if (choiceIndex == listOfObjects.Length)
                    {
                        choiceIndex = 0x0;
                    }
                    destination = listOfObjects[choiceIndex];
                }

                HoverTrainWoohoo entry = new ProxyDefinition(this).CreateInstance(station, actor, priority, false, true) as HoverTrainWoohoo;
                entry.mWoohooee = target;
                entry.mDestination = destination;
                actor.InteractionQueue.PushAsContinuation(entry, true);
            }

            protected override bool Satisfies(Sim actor, Sim target, HoverTrainStation obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                return true;
            }
        }

        public class SafeDefinition : BaseHoverTrainDefinition
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

            public override string GetInteractionName(Sim actor, HoverTrainStation target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
            }

            protected override bool Satisfies(Sim actor, Sim target, HoverTrainStation obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
            {
                if (!base.Satisfies(actor, target, obj, isAutonomous, ref callback)) return false;

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "HoverTrainWoohoo", isAutonomous, true, true, ref callback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new SafeDefinition(target));
            }
        }

        public class RiskyDefinition : BaseHoverTrainDefinition
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

            public override string GetInteractionName(Sim actor, HoverTrainStation target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Woohooer.Settings.GetRiskyChanceText(actor);
            }

            protected override bool Satisfies(Sim a, Sim target, HoverTrainStation obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesRisky(a, target, "HoverTrainRisky", isAutonomous, true, ref greyedOutTooltipCallback);
            }

            public override InteractionDefinition ProxyClone(Sim target)
            {
                return new ProxyDefinition(new RiskyDefinition(target));
            }
        }

        public class TryForBabyDefinition : BaseHoverTrainDefinition
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

            public override string GetInteractionName(Sim actor, HoverTrainStation target, InteractionObjectPair iop)
            {
                return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
            }

            protected override bool Satisfies(Sim a, Sim target, HoverTrainStation obj, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Satisfies(a, target, obj, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonPregnancy.SatisfiesTryForBaby(a, target, "HoverTrainTryForBaby", isAutonomous, true, ref greyedOutTooltipCallback);
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
                get { return CommonWoohoo.WoohooLocation.HoverTrain; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is HoverTrainStation;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<HoverTrainStation>(new Predicate<HoverTrainStation>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<HoverTrainStation>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP11))
                    {                        
                        return false;
                    }
                }

                return Woohooer.Settings.mAutonomousHoverTrain;
            }

            public bool TestUse(HoverTrainStation obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (HoverTrainStation obj in actor.LotCurrent.GetObjects<HoverTrainStation>(new Predicate<HoverTrainStation>(TestUse)))
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
