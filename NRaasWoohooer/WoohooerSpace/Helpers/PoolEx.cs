using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class PoolEx
    {
        public static bool ReactIfSimIsTeenOrBelow(Sim sim, List<ulong> skinnyDipperIDList)
        {
            if ((sim.SimDescription.TeenOrBelow) && (sim.ObjectId.IsValid) && (sim.IsOutside))
            {
                SkinnyDippersReactToTeenOrBelow(skinnyDipperIDList);
                return true;
            }

            return false;
        }

        public static void SkinnyDippersReactToTeenOrBelow(List<ulong> skinnyDipperIDList)
        {
            new InteractionPriority(InteractionPriorityLevel.High);
            foreach (ulong num in skinnyDipperIDList)
            {
                SimDescription description = SimDescription.Find(num);
                if (description == null) continue;

                // Custom
                Sim createdSim = description.CreatedSim;
                if (createdSim == null) continue;

                // Custom
                if (createdSim.Posture == null) continue;

                if (!createdSim.Posture.HasBeenCanceled)
                {
                    createdSim.Posture.CancelPosture(createdSim);
                }
            }
        }

        public static void ReactToSkinnyDippers(Sim sim, GameObject objectSkinnyDippedIn, InteractionDefinition skinnyDipInteraction, List<ulong> skinnyDipperList)
        {
            if (!sim.SimDescription.TeenOrBelow && ((skinnyDipperList != null) && (skinnyDipperList.Count != 0x0)))
            {
                List<ulong> skinnyDippers = new List<ulong>(skinnyDipperList);
                skinnyDippers.Remove(sim.SimDescription.SimDescriptionId);
                if (skinnyDippers.Count != 0x0)
                {
                    bool flag = Pool.ShouldReactPositiveToSkinnyDipper(sim, skinnyDippers);
                    bool flag2 = false;
                    if (flag && !sim.HasTrait(TraitNames.Hydrophobic))
                    {
                        flag2 = RandomUtil.RandomChance(Pool.kChancePositiveReactionSimsJoin);
                    }

                    SimDescription simDesc = SimDescription.Find(RandomUtil.GetRandomObjectFromList(skinnyDippers));
                    if (simDesc != null)
                    {
                        Sim createdSim = simDesc.CreatedSim;

                        Pool.ReactToSkinnyDipper instance = Pool.ReactToSkinnyDipper.Singleton.CreateInstance(createdSim, sim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as Pool.ReactToSkinnyDipper;
                        instance.IsPositive = flag;
                        instance.ShouldJoin = flag2;
                        instance.DippingObject = objectSkinnyDippedIn;
                        instance.SkinnyDipInteraction = skinnyDipInteraction;
                        sim.InteractionQueue.AddNextIfPossible(instance);
                    }
                }
            }
        }

        public static void AddSkinnyDipper(Pool ths, Sim s)
        {
            if (ths == null) return;

            ulong simDescriptionId = s.SimDescription.SimDescriptionId;

            //if (!ths.mSkinnyDipperList.Contains(simDescriptionId))
            {
                foreach (ulong num2 in ths.mSkinnyDipperList)
                {
                    SimDescription description = SimDescription.Find(num2);
                    if (description != null)
                    {
                        Sim createdSim = description.CreatedSim;
                        EventTracker.SendEvent(EventTypeId.kGoSkinnyDipping, createdSim, s);
                        EventTracker.SendEvent(EventTypeId.kGoSkinnyDipping, s, createdSim);
                    }
                }

                EventTracker.SendEvent(EventTypeId.kGoSkinnyDipping, s);
                //ths.mSkinnyDipperList.Add(simDescriptionId);

                if ((ths.LotCurrent != null) && (!ths.LotCurrent.IsWorldLot))
                {
                    if (ths.mSkinnyDipBroadcaster != null)
                    {
                        try
                        {
                            ths.mSkinnyDipBroadcaster.ExecuteOnEnterCallbackOnSimsInRadius(s);
                        }
                        catch (Exception e)
                        {
                            ths.mSkinnyDipBroadcaster.Dispose();
                            ths.mSkinnyDipBroadcaster = null;

                            Common.DebugException(ths, s, e);
                        }
                    }

                    if (ths.mSkinnyDipBroadcaster == null)
                    {
                        ths.mSkinnyDipBroadcaster = new PoolRoomCheckBroadcaster(ths, Pool.kSkinnyDipReactionParams, new EnterSkinnyDippingPoolAreaProxy(ths).Perform);
                    }
                }
                else
                {
                    ths.mSkinnyDipBroadcaster.Dispose();
                    ths.mSkinnyDipBroadcaster = null;
                }

                // Custom
                if ((!Woohooer.Settings.mAllowTeenSkinnyDip) && (Woohooer.Settings.mEnforceSkinnyDipPrivacy))
                {
                    if ((ths.mChildEnteredBroadcaster == null) && ths.IsOutside)
                    {
                        ths.mChildEnteredBroadcaster = new ReactionBroadcaster(ths, Pool.kChildEnterReactionParams, new ChildEnterSkinnyDippingPoolAreaProxy(ths).Perform);
                    }

                    ths.CheckIfTeenOrBelowOnLot();
                    if (!ths.mHasAddedLotCheckForChild)
                    {
                        ths.mHasAddedLotCheckForChild = true;
                        Sim.sOnLotChangedDelegates -= ths.OnChildLotChanged;
                        Sim.sOnLotChangedDelegates += ths.OnChildLotChanged;
                    }
                }
                else
                {
                    if ((ths.mChildEnteredBroadcaster == null) && ths.IsOutside)
                    {
                        ths.mChildEnteredBroadcaster = new ReactionBroadcaster(ths, Pool.kChildEnterReactionParams, HotTubBaseEx.OnStub);
                    }

                    ths.mHasAddedLotCheckForChild = true;

                    Sim.sOnLotChangedDelegates -= ths.OnChildLotChanged;
                }
            }
        }

        public class ChildEnterSkinnyDippingPoolAreaProxy
        {
            Pool mPool;

            public ChildEnterSkinnyDippingPoolAreaProxy(Pool pool)
            {
                mPool = pool;
            }

            public void Perform(Sim sim, ReactionBroadcaster broadcaster)
            {
                ReactIfSimIsTeenOrBelow(sim, mPool.mSkinnyDipperList);
            }
        }

        public class EnterSkinnyDippingPoolAreaProxy
        {
            Pool mPool;

            public EnterSkinnyDippingPoolAreaProxy(Pool pool)
            {
                mPool = pool;
            }

            public void Perform(Sim sim, ReactionBroadcaster broadcaster)
            {
                ReactToSkinnyDippers(sim, mPool, GetInPool.SkinnyDipSingleton, mPool.mSkinnyDipperList);
            }
        }
    }
}
