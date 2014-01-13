using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
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
    /*
    public class RemoveSimEx : OccultGenie.RemoveSim, Common.IPreLoad, Common.IAddInteraction, IMagicalInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, OccultGenie.RemoveSim.Definition, Definition>(false);

            sOldSingleton = OccultGenie.RemoveSim.Singleton;
            OccultGenie.RemoveSim.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim, OccultGenie.RemoveSim.Definition>(OccultGenie.RemoveSim.Singleton);
        }

        public new Sim Actor
        {
            get
            {
                return base.Actor;
            }
        }

        public int SpellLevel
        {
            get
            {
                return Hybrid.Settings.mSkillLevelBanishSim;
            }
        }

        public void DrainMotives()
        {
            MagicPointControl.UsePoints(Actor, null, kMagicPointCost, OccultTypes.Genie);
        }

        private static InteractionInstance CreateInstanceFromParameters(Terrain.TeleportMeHere.Definition ths, ref InteractionInstanceParameters parameters)
        {
            InteractionInstance instance = ths.CreateInstance(ref parameters);
            instance.ConfigureInteraction();
            return instance;
        }

        public override bool Run()
        {
            try
            {
                Lot lot;
                StandardEntry();
                BeginCommodityUpdates();
                if (Actor.SimDescription.ChildOrBelow)
                {
                    Actor.PlaySoloAnimation("c_genie_removeSim_x", true, ProductVersion.EP6);
                }
                else
                {
                    Actor.PlaySoloAnimation("a_genie_removeSim_x", true, ProductVersion.EP6);
                }

                EndCommodityUpdates(true);
                Relationship.Get(Actor, Target, true).LTR.UpdateLiking(kLTRHitFromRemoving);
                Target.InteractionQueue.CancelAllInteractions();

                List<Lot> allCommunityLots = LotManager.GetAllCommunityLots();
                int num = 0x0;
                do
                {
                    // Custom
                    lot = allCommunityLots[RandomUtil.GetInt(allCommunityLots.Count-1)];
                    if (num++ > allCommunityLots.Count)
                    {
                        StandardExit();
                        return false;
                    }
                }
                while (lot.CommercialLotSubType == CommercialLotSubType.kEP6_BigShow);

                Sim target = Target;
                if (MagicPointControl.IsFailure(this, OccultTypes.Genie))
                {
                    target = Actor;
                }

                Vector3 centerPosition = lot.GetCenterPosition();
                Vector3 fwd = new Vector3(target.ForwardVector);
                World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(centerPosition);
                fglParams.BooleanConstraints = FindGoodLocationBooleans.Routable;
                fglParams.RequiredRoomID = 0x0;
                if (GlobalFunctions.FindGoodLocation(target, fglParams, out centerPosition, out fwd))
                {
                    Terrain.TeleportMeHere.Definition interaction = new Terrain.TeleportMeHere.Definition(true, "_Genie");
                    InteractionObjectPair iop = new InteractionObjectPair(interaction, Terrain.Singleton);
                    InteractionInstanceParameters parameters = new InteractionInstanceParameters(iop, target, new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), false, false);
                    Terrain.TeleportMeHere entry = CreateInstanceFromParameters(interaction, ref parameters) as Terrain.TeleportMeHere;
                    entry.SetAndReserveDestination(centerPosition);
                    target.InteractionQueue.Add(entry);
                    EventTracker.SendEvent(EventTypeId.kRemoveSim, Actor, target);
                }

                StandardExit();

                MagicPointControl.UsePoints(Actor, null, kMagicPointCost, OccultTypes.Genie);
                return true;
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

        public new class Definition : OccultGenie.RemoveSim.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new RemoveSimEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (DaycareSituation.IsInDaycareSituationWith(target, a))
                {
                    return false;
                }
                if (target.SimDescription.IsBonehilda)
                {
                    return false;
                }
                if (!MagicPointControl.HasPoints(a, OccultTypes.Genie, true))
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return Localization.LocalizeString(a.IsFemale, "Gameplay/Actors/Sim/GenieOutOfPoints:OutOfPoints", new object[] { a });
                    };
                    return false;
                }
                if (target.SimDescription.IsDead || (target.SimDescription.Service != null))
                {
                    return false;
                }
                if (GameUtils.IsOnVacation())
                {
                    return false;
                }

                if ((isAutonomous) || (target == a) || (!target.SimDescription.ChildOrAbove) || (!a.SimDescription.ChildOrAbove))
                {
                    return false;
                }

                if (!Hybrid.IsValidOccult(a, Hybrid.Settings.mValidOccultBanishSim))
                {
                    return false;
                }

                List<Lot> allCommunityLots = LotManager.GetAllCommunityLots();
                if (allCommunityLots == null)
                {
                    return false;
                }
                return true;
            }
        }
    }
    */
}
