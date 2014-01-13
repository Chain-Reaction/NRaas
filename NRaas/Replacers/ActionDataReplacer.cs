using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Replacers
{
    public class ActionDataReplacer
    {
        public static string Perform<NEWCLASS>(string function)
        {
            return Perform<NEWCLASS>(function, function);
        }
        public static string PerformKey<NEWCLASS>(string function, string key)
        {
            return Perform<NEWCLASS>(function, function, key);
        }
        public static string Perform<NEWCLASS>(string oldFunction, string newFunction)
        {
            return Perform<NEWCLASS>(oldFunction, newFunction, null);
        }
        public static string Perform<NEWCLASS>(string oldFunction, string newFunction, string key)
        {
            try
            {
                MethodInfo oldFunc = typeof(SocialTest).GetMethod(oldFunction);
                if (oldFunc == null) return oldFunction + " Old Not Found";

                MethodInfo newFunc = typeof(NEWCLASS).GetMethod(newFunction);
                if (newFunc == null) return newFunction + " New Not Found";

                if (ActionData.sData.Values.Count == 0)
                {
                    return oldFunction + " - " + newFunction + ": No Actions";
                }

                bool found = false;

                if (!string.IsNullOrEmpty(key))
                {
                    ActionData data = ActionData.Get(key);
                    if (data != null)
                    {
                        if ((data.ProceduralTest != null) && (data.ProceduralTest.ToString() == oldFunc.ToString()))
                        {
                            data.ProceduralTest = newFunc;
                            found = true;
                        }
                    }
                }
                else
                {
                    foreach (ActionData data in ActionData.sData.Values)
                    {
                        if ((data.ProceduralTest != null) && (data.ProceduralTest.ToString() == oldFunc.ToString()))
                        {
                            data.ProceduralTest = newFunc;
                            found = true;
                        }
                    }
                }

                if (found)
                {
                    return null;
                }
                else
                {
                    return oldFunction + " - " + newFunction + ": No Change";
                }
            }
            catch (Exception e)
            {
                Common.Exception(oldFunction + " - " + newFunction, e);
                return oldFunction + " - " + newFunction + ": Exception";
            }
        }
    }
}
