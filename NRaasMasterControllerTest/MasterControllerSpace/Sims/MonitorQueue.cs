using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public class MonitorQueue : OptionItem, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "MonitorQueue";
        }

        public bool Test(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Test(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        public OptionResult Perform(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Perform(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        public class QueueMonitor
        {
            Sim mSim;

            public QueueMonitor(Sim sim)
            {
                mSim = sim;
            }

            public void OnChanged()
            {
                Common.StringBuilder noticeText = new Common.StringBuilder();
                Common.StringBuilder logText = new Common.StringBuilder();
                Common.ExceptionLogger.Convert(mSim, noticeText, logText);

                Common.DebugStackLog(logText);
            }
        }

        public class RoutingMonitor
        {
            Sim mSim;

            public RoutingMonitor(Sim sim)
            {
                mSim = sim;
            }

            public void OnPerform(RouteEvent e)
            {
                Common.StringBuilder msg = new Common.StringBuilder();
                msg.Append("Data: " + e.Data);
                msg.Append(Common.NewLine + "Data2: " + e.Data2);
                msg.Append(Common.NewLine + "DistanceRemaining: " + e.DistanceRemaining);
                msg.Append(Common.NewLine + "DistanceTravelled: " + e.DistanceTravelled);
                msg.Append(Common.NewLine + "EventDirection: " + e.EventDirection);
                msg.Append(Common.NewLine + "EventPosition: " + e.EventPosition);
                msg.Append(Common.NewLine + "EventSource: " + e.EventSource);
                msg.Append(Common.NewLine + "EventType: " + e.EventType);
                msg.Append(Common.NewLine + "ObjectID: " + e.ObjectID);

                GameObject obj = GameObject.GetObject<GameObject>(e.ObjectID);
                if (obj != null)
                {
                    msg.Append(Common.NewLine + "Object Type: " + obj.GetType());
                }

                Route route = mSim.RoutingComponent.GetCurrentRoute();
                if (route != null)
                {
                    msg.Append(Common.NewLine + "NumPaths: " + route.GetNumPaths());

                    for (uint i = 0x0; i < route.GetNumPaths(); i++)
                    {
                        PathData pathData = route.GetPathData(i);

                        msg.Append(Common.NewLine + "PathType: " + pathData.PathType);
                        msg.Append(Common.NewLine + "PortalStartPos: " + pathData.PortalStartPos);
                        msg.Append(Common.NewLine + "ObjectId: " + pathData.ObjectId);

                        obj = GameObject.GetObject<GameObject>(pathData.ObjectId);
                        if (obj != null)
                        {
                            msg.Append(Common.NewLine + "Object Type: " + obj.GetType());
                        }
                    }
                }

                Common.DebugStackLog(msg);
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            try
            {
                Sim sim = parameters.mTarget as Sim;

                sim.InteractionQueue.QueueChanged += new QueueMonitor(sim).OnChanged;

                //sim.RoutingComponent.OnRoutingEvent += new RoutingMonitor(sim).OnPerform;

                SimpleMessageDialog.Show(Name, sim.FullName + " is now being monitored.");
            }
            catch (Exception e)
            {
                GameHitParameters<GameObject>.Exception(parameters, e);
            }
            return OptionResult.SuccessClose;
        }
    }
}
