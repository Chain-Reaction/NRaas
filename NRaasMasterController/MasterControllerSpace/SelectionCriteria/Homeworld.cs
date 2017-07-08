using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class Homeworld : SelectionTestableOptionList<Homeworld.Item, WorldName, WorldName>, Common.IDelayedWorldLoadFinished
    {
        static Common.MethodStore sGetHomeworld = new Common.MethodStore("NRaasTraveler", "NRaas.Traveler", "GetSimHomeworld", new Type[] { typeof(ulong) });
        static Common.MethodStore sGetWorlds = new Common.MethodStore("NRaasTraveler", "NRaas.TravelerSpace.Helpers.WorldData", "GetWorlds", new Type[] { typeof(Dictionary<WorldName, string>) });

        static Dictionary<WorldName, string> sWorlds = new Dictionary<WorldName, string>();

        public override string GetTitlePrefix()
        {
            return "Criteria.Homeworld";
        }

        public override bool IsSpecial
        {
            get { return true; }
        }

        public void OnDelayedWorldLoadFinished()
        {
            if (sGetWorlds.Valid)
            {
                sGetWorlds.Invoke<Dictionary<WorldName, string>>(new object[] { sWorlds });
            }
        }

        public class Item : TestableOption<WorldName, WorldName>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<WorldName, WorldName> results)
            {
                if (sGetHomeworld.Valid)
                {
                    WorldName name = sGetHomeworld.Invoke<WorldName>(new object[] { me.SimDescriptionId });

                    if (name != WorldName.Undefined)
                    {
                        results[name] = name;
                        return true;
                    }
                }

                results[me.HomeWorld] = me.HomeWorld;
                return true;
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<WorldName, WorldName> results)
            {
                if (sGetHomeworld.Valid)
                {
                    WorldName name = sGetHomeworld.Invoke<WorldName>(new object[] { me.SimDescriptionId });

                    if (name != WorldName.Undefined)
                    {
                        results[name] = name;
                        return true;
                    }
                }

                results[me.HomeWorld] = me.HomeWorld;
                return true;
            }

            public override void SetValue(WorldName value, WorldName storeType)
            {
                mValue = value;

                mName = GetName(value);
            }

            public static string GetWorldName(WorldName value)
            {
                string result;
                if (sWorlds.TryGetValue(value, out result))
                {
                    return result;
                }

                return value.ToString();
            }

            public static string GetName(WorldName world)
            {
                if (sGetWorlds.Valid)
                {
                    return GetWorldName(world);
                }

                string homeWorld = Common.LocalizeEAString("Gameplay/Visa/TravelUtil:" + world + "Full");
                if (!homeWorld.Contains("****"))
                {
                    return homeWorld;
                }
                else if (Common.kDebugging) 
                {
                    return world.ToString();
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
