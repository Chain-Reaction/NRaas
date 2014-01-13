using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.OverwatchSpace.Settings;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NRaas.OverwatchSpace.Actions
{
    public class InstantationCheck : BooleanOption, Common.IWorldLoadFinished
    {
        static Tracer sTracer = new Tracer();

        static Dictionary<ulong, bool> sAlreadyShown = new Dictionary<ulong, bool>();

        public override string GetTitlePrefix()
        {
            return "InstantationCheck";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mInstantationCheck;
            }
            set
            {
                NRaas.Overwatch.Settings.mInstantationCheck = value;
            }
        }

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kSimInstantiated, OnTest);

            new Common.AlarmTask(1, TimeUnit.Days, OnReset);
        }

        public static void OnReset()
        {
            sAlreadyShown.Clear();
        }

        public static void OnTest(Event e)
        {
            if (!Overwatch.Settings.mInstantationCheck) return;

            sTracer.Perform();
            if (!sTracer.mAllow) return;

            bool reset = false;

            Sim sim = e.TargetObject as Sim;
            if (sim == null) return;

            SimDescription simDesc = sim.SimDescription;

            GreyedOutTooltipCallback callback = null;
            if (!RecoverMissingSimTask.Allowed(simDesc, true, ref callback))
            {
                reset = true;
            }

            if (reset)
            {
                string notice = Common.Localize("InstantationCheck:Reset", sim.IsFemale, new object[] { sim });

                if (!sAlreadyShown.ContainsKey(sim.SimDescription.SimDescriptionId))
                {
                    sAlreadyShown.Add(sim.SimDescription.SimDescriptionId, true);

                    Common.Notify(sim, notice);
                }

                sim.Destroy();

                Overwatch.Log(notice);
            }
        }

        public class Tracer : StackTracer
        {
            public bool mAllow = true;

            public Tracer()
            {
                AddTest(typeof(Sims3.Gameplay.ChildAndTeenUpdates.BoardingSchool), "Void RemoveCommon", OnAllow);
            }

            public override void Reset()
            {
                mAllow = true;

                base.Reset();
            }

            protected bool OnAllow(StackTrace trace, StackFrame frame)
            {
                mAllow = false;
                return true;
            }
        }
    }
}
