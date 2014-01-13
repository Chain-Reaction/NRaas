using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Replacers
{
    public class SocialRHSReplacer
    {
        public static string Perform<NEWCLASS>(string action, string function)
        {
            return Perform<NEWCLASS>(action, function, function);
        }
        public static string Perform<NEWCLASS>(string action, string oldFunction, string newFunction)
        {
            try
            {
                MethodInfo oldFunc = typeof(SocialCallback).GetMethod(oldFunction);
                if (oldFunc == null) return oldFunction + ": Old Not Found";

                MethodInfo newFunc = typeof(NEWCLASS).GetMethod(newFunction);
                if (newFunc == null) return newFunction + ": New Not Found";

                List<SocialRuleRHS> rules = SocialRuleRHS.Get(action);
                if (rules == null) return action + ": Action Not Found";

                bool found = false;
                foreach (SocialRuleRHS rule in rules)
                {
                    if ((rule.ProceduralEffectBeforeUpdate != null) && (rule.ProceduralEffectBeforeUpdate.ToString () == oldFunc.ToString ()))
                    {
                        rule.mProceduralEffectBeforeUpdate = newFunc;
                        found = true;
                    }

                    if ((rule.ProceduralEffectAfterUpdate != null) && (rule.ProceduralEffectAfterUpdate.ToString() == oldFunc.ToString()))
                    {
                        rule.mProceduralEffectAfterUpdate = newFunc;
                        found = true;
                    }
                }

                if (found)
                {
                    return null;
                }
                else
                {
                    return action + " - " + oldFunction + " - " + newFunction + ": No Change";
                }
            }
            catch (Exception e)
            {
                Common.Exception(action + " - " + oldFunction + " - " + newFunction, e);
                return action + " - " + oldFunction + " - " + newFunction + ": Exception";
            }
        }
    }
}
