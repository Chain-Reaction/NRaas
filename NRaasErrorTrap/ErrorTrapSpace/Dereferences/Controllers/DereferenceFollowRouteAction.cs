using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences.Controllers
{
    public class DereferenceFollowRouteAction : DereferenceController<FollowRouteAction>
    {
        static Dictionary<Sim, InteractionInstance> sSims;

        public override void Clear()
        {
            sSims = null;

            base.Clear();
        }

        protected override void PreProcess(FollowRouteAction obj, object parent, FieldInfo field)
        {
            IScriptProxy proxy = Simulator.GetProxy(obj.mRoutingObjectId);
            if (proxy == null) return;

            Sim sim = proxy.Target as Sim;
            if (sim == null) return;

            SimRoutingComponent component = sim.RoutingComponent as SimRoutingComponent;
            if (component == null) return;

            if (component.mRouteActions.Contains(obj)) return;

            if (sSims == null)
            {
                sSims = new Dictionary<Sim, InteractionInstance>();
            }

            if (!sSims.ContainsKey(sim))
            {
                sSims.Add(sim, sim.CurrentInteraction);

                ErrorTrap.DebugLogCorrection("Potential Follow Route Action Reset: " + sim.FullName);
            }
        }

        protected override void Perform(FollowRouteAction obj, object parent, FieldInfo field)
        { }

        public override void Perform()
        {
            if (sSims != null)
            {
                foreach (KeyValuePair<Sim, InteractionInstance> pair in sSims)
                {
                    new DelayedResetTask(pair.Key, pair.Value);
                }

                sSims.Clear();
            }

            sSims = null;

            base.Perform();
        }

        public class DelayedResetTask : Common.AlarmTask
        {
            Sim mSim;

            InteractionInstance mInstance;

            public DelayedResetTask(Sim sim, InteractionInstance instance)
                : base(30f, TimeUnit.Minutes)
            {
                mInstance = instance;
                mSim = sim;
            }

            protected override void OnPerform()
            {
                if (!object.ReferenceEquals(mSim.CurrentInteraction, mInstance)) return;

                ErrorTrap.DebugLogCorrection("Follow Route Action Reset: " + mSim.FullName);

                new ResetSimTask(mSim);
            }
        }
    }
}
