using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Objects
    {
        public delegate bool OnProcess<T>(T obj)
            where T : GameObject;

        public delegate void Logger(string text);

        public static void Process<T>(OnProcess<T> process, Logger log)
            where T : GameObject
        {
            foreach (T obj in Sims3.Gameplay.Queries.GetGlobalObjects<T>())
            {
                if (process(obj))
                {
                    if (log != null)
                    {
                        log("  Global Altered");
                    }
                }
            }

            foreach (T obj in Sims3.Gameplay.Queries.GetObjects<T>())
            {
                if (process(obj))
                {
                    if (log != null)
                    {
                        log("  Local Altered");
                    }
                }
            }
        }
    }
}

