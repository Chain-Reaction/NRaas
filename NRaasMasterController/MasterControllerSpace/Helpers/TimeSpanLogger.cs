using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public class TimeSpanLogger : Common.Logger<TimeSpanLogger>, StatBin.IStatBinLogger
    {
        readonly static TimeSpanLogger sLogger = new TimeSpanLogger();

        readonly static Stats sBin = new Stats();

        static bool sChanged = false;

        public static StatBin Bin
        {
            get
            {
                return sBin;
            }
        }

        protected override string Name
        {
            get { return "Time Span Logs"; }
        }

        protected override TimeSpanLogger Value
        {
            get { return sLogger; }
        }

        public void Append(string text)
        {
            sLogger.PrivateAppend(text);
        }
        public static void Append(string text, DateTime now, DateTime then)
        {
            long duration = (now - then).Ticks / TimeSpan.TicksPerMillisecond;
            if (duration > 0)
            {
                sChanged = true;
            }

            Bin.AddStat(text, duration);
        }

        protected override int PrivateLog(Common.StringBuilder builder)
        {
            if (sChanged)
            {
                sBin.Log(sLogger);
                sChanged = false;
            }

            return base.PrivateLog(builder);
        }

        public class Stats : StatBin
        {
            public Stats()
                : base("Time Span")
            { }

            public override void IncStat(string stat)
            {
                sChanged = true;

                base.IncStat(stat);
            }

            public override float AddStat(string stat, float val)
            {
                if (val != 0)
                {
                    sChanged = true;
                }

                return base.AddStat(stat, val);
            }
        }
    }
}
