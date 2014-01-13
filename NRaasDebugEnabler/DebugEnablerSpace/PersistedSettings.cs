using NRaas.CommonSpace.Helpers;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.DebugEnablerSpace
{
    [Persistable]
    public class PersistedSettings
    {
        public bool mEnabled = true;

        public Dictionary<Type, bool> mInteractions = new Dictionary<Type, bool>();

        public class Persist : Persistence
        {
            public override void Import(Lookup settings)
            {
                DebugEnabler.Settings.mEnabled = settings.GetBool("Enabled", true);

                DebugEnabler.Settings.mInteractions.Clear();

                int count = settings.GetInt("HotKeyCount", 0);

                for (int i = 0; i < count; i++)
                {
                    Type type = settings.GetType("HotKey" + i);
                    if (type == null) continue;

                    DebugEnabler.Settings.mInteractions[type] = true;
                }
            }

            public override void Export(Lookup settings)
            {
                settings.Add("Enabled", DebugEnabler.Settings.mEnabled);

                settings.Add("HotKeyCount", DebugEnabler.Settings.mInteractions.Count);

                int i = 0;
                foreach (Type type in DebugEnabler.Settings.mInteractions.Keys)
                {
                    settings.Add("HotKey" + i, type);
                    i++;
                }
            }

            public override string PersistencePrefix
            {
                get { return "Persistence"; }
            }
        }
    }
}
