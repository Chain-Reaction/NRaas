using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Situations
{
    public class GoHereWithSituationEx : GoHereWithSituation
    {
        protected GoHereWithSituationEx()
        { }
        public GoHereWithSituationEx(Sim owner, Sim leader, List<Sim> followers, Route route, AtMeetupPointCallback meetupCallback, OnFailBehavior failBehavior, Vector3 teleportDest)
            : base(owner, leader, followers, route, meetupCallback, failBehavior, teleportDest)
        { }

        public override void OnParticipantDeleted(Sim s)
        {
            // Shortens the stack
            new ParticipantDeletedTask(this, s, base.OnParticipantDeleted).AddToSimulator();
        }

        public static void CreateSituation(LeaderGoHereWith ths)
        {
            ths.mSituation = new GoHereWithSituationEx(ths.mOwner, ths.Actor, ths.mFollowers, ths.mRouteToFollow, ths.mMeetupPointCallback, ths.mOnFailBehavior, ths.mTeleportDestination);
            ths.mSituation.LeaderInteraction = ths;
        }

        public class ParticipantDeletedTask : Common.FunctionTask
        {
            GoHereWithSituationEx mSituation;

            Sim mSim;

            OnCall mFunc;

            public delegate void OnCall(Sim sim);

            public ParticipantDeletedTask(GoHereWithSituationEx sit, Sim sim, OnCall func)
            {
                mSituation = sit;
                mSim = sim;
                mFunc = func;
            }

            protected override void OnPerform()
            {
                try
                {
                    mFunc(mSim);
                }
                catch (Exception e)
                {
                    mSituation.Exit();

                    Common.DebugException(mSim, e);                        
                }
            }
        }
    }
}
