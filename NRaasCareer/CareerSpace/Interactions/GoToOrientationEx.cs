using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using System;

namespace NRaas.CareerSpace.Interactions
{
    public class GoToOrientationEx : OrientationSituation.GoToOrientation, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Lot, OrientationSituation.GoToOrientation.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                if (mOrientationRouteTarget == null)
                {
                    mOrientationRouteTarget = Target.GetObjects<IStudentServicesBooth>();
                }

                if ((mOrientationRouteTarget != null) && (mOrientationRouteTarget.Length > 0x0))
                {
                    mThisRouteTarget = RandomUtil.GetRandomObjectFromList(mOrientationRouteTarget);
                }

                bool routed = false;
                if (mThisRouteTarget != null)
                {
                    routed = Actor.RouteToObjectRadius(mThisRouteTarget, 7f);
                    if (!routed)
                    {
                        routed = Actor.RouteToObjectRadius(mThisRouteTarget, 10f);
                    }
                }

                if (!routed && (Actor.LotCurrent != Target))
                {
                    routed = Actor.RouteToLot(Target.LotId);
                }

                if (mThisRouteTarget != null)
                {
                    return mThisRouteTarget.PushSimToFreebie(this, Actor);
                }

                return false;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : OrientationSituation.GoToOrientation.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoToOrientationEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
