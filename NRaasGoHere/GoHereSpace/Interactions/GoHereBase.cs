using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public abstract class GoHereBase : Terrain.GoHere, Common.IPreLoad, Common.IAddInteraction
    {
        public abstract void OnPreLoad();

        public abstract void AddInteraction(Common.InteractionInjectorList interactions);

        public override void Init(ref InteractionInstanceParameters parameters)
        {
            base.Init(ref parameters);

            if (GoHere.Settings.mAllowGoHereStack)
            {
                if (mPriority.Value < 0)
                {
                    RaisePriority();
                }
            }
        }

        public override bool Run()
        {
            try
            {
                LotLocation location = LotLocation.Invalid;
                Lot lot = LotManager.GetLot(World.GetLotLocation(Destination, ref location));
                if (lot != null)
                {
                    if (Teleport.Perform(Actor, lot, true)) return true;
                }

                return base.Run();
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

        public override bool ShouldReplace(InteractionInstance interaction)
        {
            if (GoHere.Settings.mAllowGoHereStack)
            {
                return false;
            }
            else
            {
                return base.ShouldReplace(interaction);
            }
        }

        public class Teleport : Common.FunctionTask
        {
            Sim mSim;

            GameObject mParent;

            Lot mDestination;

            protected Teleport(Sim sim, Lot destination, bool checkVampire)
            {
                mSim = sim;
                mDestination = destination;
            }

            public static bool Perform(Sim sim, Lot destination, bool checkVampire)
            {
                if (checkVampire)
                {
                    if (!GoHere.Settings.mVampireTeleport) return false;

                    if (!SimTypes.IsOccult(sim.SimDescription, Sims3.UI.Hud.OccultTypes.Vampire)) return false;
                }

                if (sim.LotCurrent == destination) return false;

                new Teleport(sim, destination, checkVampire).AddToSimulator();
                return true;
            }

            protected override void OnPerform()
            {
                if ((mSim.Posture != null) && (mSim.Posture.Container != null))
                {
                    mParent = mSim.Posture.Container as GameObject;

                    mSim.Posture = null;
                }

                TerrainInteraction instance = new Terrain.TeleportMeHere.Definition(false).CreateInstanceWithCallbacks(Terrain.Singleton, mSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, null, OnCompleted, OnFailed) as TerrainInteraction;

                Door frontDoor = mDestination.FindFrontDoor();
                if (frontDoor != null)
                {
                    Vector3 forward;
                    World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(frontDoor.Position);
                    fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                    fglParams.InitialSearchDirection = RandomUtil.GetInt(0x0, 0x7);
                    if (GlobalFunctions.FindGoodLocation(mSim, fglParams, out instance.Destination, out forward))
                    {
                        mSim.InteractionQueue.Add(instance);
                    }
                }
            }

            public void OnCompleted(Sim s, float x)
            {
                try
                {
                    if (mParent is ICrib)
                    {
                        mParent.SetObjectToReset();
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(s, e);
                }
            }

            public void OnFailed(Sim s, float x)
            {
                try
                {
                    Teleport.Perform(mSim, mDestination, false);
                }
                catch (Exception e)
                {
                    Common.Exception(s, e);
                }
            }
        }
    }
}
