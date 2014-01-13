using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupPatterns : DelayedLoadupOption
    {
        public static void BuildPatternCache()
        {
            Pattern.FlushPatternCache();
            Pattern.sPatternCategories.Clear();
            Pattern.sPatternCache.Clear();
            KeySearch search = new KeySearch(0xd4d9fbe5);
            foreach (ResourceKey key in search)
            {
                string xmlFragment = Simulator.LoadXMLString(key);
                if ((xmlFragment != null) && (xmlFragment != string.Empty))
                {
                    XmlTextReader reader = new XmlTextReader(xmlFragment, XmlNodeType.Document, null);
                    while (reader.Read())
                    {
                        if (reader.IsStartElement("patternlist"))
                        {
                            try
                            {
                                Pattern.ParseCategories(reader);
                            }
                            catch (Exception e)
                            {
                                Common.Exception("ResourceKey: " + key, e);
                            }
                        }
                    }
                }
            }
            Pattern.sCacheBuilt = true;
        }

        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupPatterns");

            if (!Pattern.sCacheBuilt)
            {
                BuildPatternCache();
            }

            foreach (string category in Pattern.GetPatternCategories())
            {
                try
                {
                    ulong id = ResourceUtils.HashString64(category);
                }
                catch
                {
                    Pattern.sPatternCategories.Remove(category);

                    Overwatch.Log("  Unhashable Pattern Category : " + category);
                }
            }
        }
    }
}
