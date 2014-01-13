using Sims3.Gameplay.Socializing;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Replacers
{
    public class ActionAvailabilityReplacer
    {
        public static string Perform(string actionKey, Dictionary<ShortTermContextTypes,ShortTermContextTypes> replacements)
        {
            try
            {
                foreach (KeyValuePair<ShortTermContextTypes, Dictionary<LongTermRelationshipTypes, Dictionary<bool, List<string>>>> stc in ActionAvailabilityData.sStcInteractions)
                {
                    ShortTermContextTypes replacement;
                    if (!replacements.TryGetValue(stc.Key, out replacement)) continue;

                    foreach (KeyValuePair<LongTermRelationshipTypes, Dictionary<bool, List<string>>> ltr in stc.Value)
                    {
                        foreach (KeyValuePair<bool, List<string>> active in ltr.Value)
                        {
                            if (active.Value.Contains(actionKey))
                            {
                                active.Value.Remove(actionKey);

                                AddKey(replacement, ltr.Key, active.Key, actionKey);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(actionKey, e);
                return actionKey + ": Exception";
            }
        }

        protected static void AddKey(ShortTermContextTypes stc, LongTermRelationshipTypes ltr, bool isActive, string key)
        {
            Dictionary<LongTermRelationshipTypes, Dictionary<bool, List<string>>> ltrList;
            if (!ActionAvailabilityData.sStcInteractions.TryGetValue(stc, out ltrList)) return;

            Dictionary<bool, List<string>> activeList;
            if (!ltrList.TryGetValue(ltr, out activeList))
            {
                activeList = new Dictionary<bool, List<string>>();
                ltrList.Add(ltr, activeList);
            }

            List<string> keyList;
            if (!activeList.TryGetValue(isActive, out keyList))
            {
                keyList = new List<string>();
                activeList.Add(isActive, keyList);
            }

            if (!keyList.Contains(key))
            {
                keyList.Add(key);
            }
        }
    }
}
