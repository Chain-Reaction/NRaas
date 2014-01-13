using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
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
using System.Diagnostics;
using System.Text;

namespace NRaas.GoHereSpace.Helpers
{
    public class GroupingSituationHelper : Common.IWorldLoadFinished
    {
        static Tracer sTracer = new Tracer();

        public void OnWorldLoadFinished()
        {
            EventTracker.AddListener(EventTypeId.kGroupingWithSim, OnGroupingWithSim);
        }

        protected static ListenerAction OnGroupingWithSim(Event e)
        {
            try
            {
                Sim actor = e.Actor as Sim;
                if (actor != null)
                {
                    if (GoHere.Settings.DisallowAutoGroup(actor))
                    {
                        GroupingSituation situation = actor.GetSituationOfType<GroupingSituation>();
                        if (situation != null)
                        {
                            sTracer.Perform();

                            if (sTracer.mAllow)
                            {
                                if (!GoHere.Settings.mIgnoreLogs)
                                {
                                    Common.DebugStackLog(actor.FullName);
                                }

                                situation.Exit();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Exception(e.Actor, e.TargetObject, ex);
            }

            return ListenerAction.Keep;
        }

        public class Tracer : StackTracer
        {
            public bool mAllow;

            public Tracer()
            {
                AddTest(typeof(Sims3.Gameplay.Socializing.SocialCallback), "Void OnLetsGoOnDateAccepted", OnDisallow);
                AddTest(typeof(Sims3.Gameplay.Socializing.SocialCallback), "Void OnLetsHangOutAccepted", OnDisallow);
            }

            protected bool OnDisallow(StackTrace trace, StackFrame frame)
            {
                mAllow = false;
                return true;
            }

            public override void Reset()
            {
                mAllow = true;

                base.Reset();
            }
        }
    }
}
