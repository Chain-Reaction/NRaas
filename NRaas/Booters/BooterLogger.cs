using NRaas.CommonSpace.Booters;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.CommonSpace.Booters
{
    public class BooterLogger : Common.TraceLogger<BooterLogger>
    {
        readonly static BooterLogger sLogger = new BooterLogger();

        public static void AddTrace(string msg)
        {
            sLogger.PrivateAddTrace(msg);
        }
        public static void AddError(string msg)
        {
            sLogger.PrivateAddError(msg);
        }

        public static bool Exists(XmlDbRow row, string key, string name)
        {
            if (row.Exists(key)) return true;
            
            AddError(name + " " + key + " Missing");
            return false;
        }

        protected override string Name
        {
            get { return "Messages"; }
        }

        protected override BooterLogger Value
        {
            get { return sLogger; }
        }
    }
}
