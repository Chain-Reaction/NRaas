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
    public class ObjectComponents
    {
        public delegate void Logger(string text);

        public static void Cleanup(GameObject obj, Logger log)
        {
            if (obj.mObjComponents != null)
            {
                for (int i = obj.mObjComponents.Count - 1; i >= 0; i--)
                {
                    if (obj.mObjComponents[i] == null)
                    {
                        obj.mObjComponents.RemoveAt(i);

                        if (log != null)
                        {
                            log("Corrupt Object Component Deleted: " + GetName(obj));
                        }
                    }
                }
            }
        }

        protected static string GetName(IGameObject obj)
        {
            try
            {
                string name = obj.GetLocalizedName();
                if (!string.IsNullOrEmpty(name))
                {
                    return name.Trim();
                }
            }
            catch
            { }

            return obj.GetType().ToString();
        }

        public static bool AddComponent<C>(GameObject ths, params object[] args) 
            where C : ObjectComponent
        {
            Type cType = typeof(C);
            bool staticAllowed = true;// Simulator.GetProxy(ths.ObjectId) == null;
            if (!ths.AddComponentInternal(cType, staticAllowed, args)) return false;

            if (ths.mObjComponents != null)
            {
                Type type = typeof(C);
                for (int i = 0x0; i < ths.mObjComponents.Count; i++)
                {
                    ObjectComponent component = ths.mObjComponents[i];
                    if (component is C)
                    {
                        component.OnStartup();
                        break;
                    }
                }
            }

            return true;
        }
    }
}

