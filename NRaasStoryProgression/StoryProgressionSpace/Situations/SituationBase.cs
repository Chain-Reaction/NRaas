using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.StoryProgressionSpace.Situations
{
    public abstract class SituationBase : RootSituation
    {
        public readonly Sim Worker;

        private CarService mCar;

        // Methods
        protected SituationBase()
        { }
        public SituationBase(Lot lot, Sim worker)
            : base(lot)
        {
            Worker = worker;

            worker.InteractionQueue.CancelAllInteractions();

            worker.AssignRole(this);
        }

        protected CarService GetCar()
        {
            if (mCar == null)
            {
                mCar = CarNpcManager.Singleton.CreateServiceCar(Firefighter.Instance.ServiceType);
            }
            return mCar;
        }

        public override void CleanUp()
        {
            if (mCar != null)
            {
                mCar.Destroy();
            }

            base.CleanUp();
        }

        public override void OnParticipantDeleted(Sim participant)
        {
            if (participant == Worker)
            {
                base.Exit();
            }
        }

        public class RouteToLot<ParentSituation, NextSituation> : ChildSituation<ParentSituation>
            where ParentSituation : SituationBase
            where NextSituation : ChildSituation<ParentSituation>
        {
            // Fields
            private const int kAttemptsToRouteToLot = 0x3;
            protected int mAttemptsToRouteToLot;
            private float mRouteTime = 10f;
            private static ConstructorInfo sNewNextSituation;

            // Methods
            static RouteToLot()
            {
                RouteToLot<ParentSituation, NextSituation>.sNewNextSituation = typeof(NextSituation).GetConstructor(new Type[] { typeof(ParentSituation) });
            }

            protected RouteToLot()
            { }
            public RouteToLot(ParentSituation parent) 
                : base(parent)
            { }

            public RouteToLot(ParentSituation parent, int previousRouteCount) 
                : this(parent)
            {
                mAttemptsToRouteToLot = previousRouteCount;
            }

            protected virtual NextSituation ConstructNextSituation()
            {
                return (RouteToLot<ParentSituation, NextSituation>.sNewNextSituation.Invoke(new object[] { Parent }) as NextSituation);
            }

            public override void Init(ParentSituation parent)
            {
                if (parent.Worker.SimDescription.TeenOrAbove)
                {
                    ForceSituationSpecificInteraction(parent.Lot, parent.Worker, new DriveToLotInServiceCar.Definition(parent.GetCar(), false, true, true, null, mRouteTime), null, OnRouteSucceeded, OnRouteFailed);
                }
                else
                {
                    ForceSituationSpecificInteraction(parent.Lot, parent.Worker, GoToLot.Singleton, null, OnRouteSucceeded, OnRouteFailed);
                }
            }

            protected virtual void OnRouteFailed()
            {
                Common.DebugNotify("RouteToLot.OnRouteFailed");
                Exit();
            }

            protected virtual void OnRouteFailed(Sim actor, float x)
            {
                if (!Simulator.CheckYieldingContext(false))
                {
                    Exit();
                }
                else if (actor.LotCurrent == Lot)
                {
                    OnRouteSucceeded();
                }
                else if (mAttemptsToRouteToLot < 0x3)
                {
                    mAttemptsToRouteToLot++;
                    Init(Parent);
                }
                else
                {
                    OnRouteFailed();
                }
            }

            protected virtual void OnRouteSucceeded()
            {
                Parent.SetState(ConstructNextSituation());
            }

            protected virtual void OnRouteSucceeded(Sim actor, float x)
            {
                OnRouteSucceeded();
            }

            public void SetRouteTime(float routeTime)
            {
                mRouteTime = routeTime;
            }
        }
    }
}

