using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Toys;
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
    public class AutonomousSplashInPuddleEx : Sim.AutonomousSplashInPuddle, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.AutonomousSplashInPuddle.Definition, Definition>(false);

            sOldSingleton = Sim.AutonomousSplashInPuddle.Singleton as InteractionDefinition;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Sim.AutonomousSplashInPuddle.Definition>(Singleton as InteractionDefinition);
        }

        public override bool Run()
        {
			//Common.Notify("Running splash");
			ulong lotId = Actor.LotCurrent.LotId;
			List<Vector3> rainPuddles = World.GetRainPuddles(lotId);
			if (rainPuddles.Count == 0)
			{
				//Common.Notify("first false");
				return false;
			}
			Vector3 position = Actor.Position;
			List<DistanceHelper> list = new List<DistanceHelper>(rainPuddles.Count);
			foreach (Vector3 item in rainPuddles)
			{
				if (Terrain.SplashInPuddle.IsPuddleValid(item, false, true))
				{
					list.Add(new DistanceHelper(item, (position - item).LengthSqr()));
				}
			}
			list.Sort(delegate (DistanceHelper x, DistanceHelper y)
			{
				return x.Distance.CompareTo(y.Distance);
			});
			Vector3 vector = Vector3.OutOfWorld;
			for (int i = 0; i < 3 && i < list.Count; i++)
			{
				Route route = Actor.CreateRoute();
				if (route.PlanToPoint(list[i].Point).Succeeded())
				{
					vector = list[i].Point;
					break;
				}
			}
			if (vector == Vector3.OutOfWorld)
			{
				//Common.Notify("second false");
				return false;
			}
			InteractionDefinition singleton = Terrain.SplashInPuddle.Singleton;
			Terrain.PuddleInteraction puddleInteraction = singleton.CreateInstance(Terrain.Singleton, Actor, GetPriority(), base.Autonomous, base.CancellableByPlayer) as Terrain.PuddleInteraction;
			puddleInteraction.Hit = new GameObjectHit(GameObjectHitType.LotTerrain);
			puddleInteraction.Hit.mPoint = vector;
			puddleInteraction.SetDestination(vector);
			return Actor.InteractionQueue.PushAsContinuation(puddleInteraction, false);
		}

        public new class Definition : Sim.AutonomousSplashInPuddle.Definition
        {
            public Definition()
            { }

			public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
			{
				InteractionInstance na = new AutonomousSplashInPuddleEx();
				na.Init(ref parameters);
				return na;
			}

			public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                // No idea why this is gated to mermaids?
                //if (!OccultTypeHelper.HasType(a, OccultTypes.Mermaid)) return false;

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
