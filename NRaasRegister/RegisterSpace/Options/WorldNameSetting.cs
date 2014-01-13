using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Options
{
    public abstract class WorldNameSetting<TObject> : ListedSettingOption<WorldName, TObject>, Common.IDelayedWorldLoadFinished
        where TObject : class, IGameObject
    {
        static Common.MethodStore sGetWorlds = new Common.MethodStore("NRaasTraveler", "NRaas.TravelerSpace.Helpers.WorldData", "GetWorlds", new Type[] { typeof(Dictionary<WorldName, string>) });

        static Dictionary<WorldName, string> sWorlds = new Dictionary<WorldName, string>();

        public void OnDelayedWorldLoadFinished()
        {
            if (sGetWorlds.Valid)
            {
                sGetWorlds.Invoke<bool>(new object[] { sWorlds });
            }
        }

        public override string GetLocalizedValue(WorldName value)
        {
            string result;
            if (sWorlds.TryGetValue(value, out result))
            {
                return result;
            }

            return value.ToString();
        }

        protected override bool Allow(GameHitParameters<TObject> parameters)
        {
            if (!sGetWorlds.Valid) return false;

            return base.Allow(parameters);
        }

        public override bool ConvertFromString(string value, out WorldName newValue)
        {
            ParserFunctions.TryParseEnum<WorldName>(value, out newValue, WorldName.Undefined);

            int result;
            if (!int.TryParse(value, out result)) return false;

            newValue = (WorldName)result;
            return true;
        }

        public override string ConvertToString(WorldName value)
        {
            return value.ToString();
        }

        protected override List<ListedSettingOption<WorldName, TObject>.Item> GetOptions()
        {
            List<Item> results = new List<Item>();

            foreach (WorldName value in sWorlds.Keys)
            {
                results.Add(new Item(this, value));
            }

            return results;
        }
    }
}
