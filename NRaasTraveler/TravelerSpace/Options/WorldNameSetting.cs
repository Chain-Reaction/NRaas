using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Helpers;
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

namespace NRaas.TravelerSpace.Options
{
    public abstract class WorldNameSetting<TObject> : ListedSettingOption<WorldName, TObject>
        where TObject : class, IGameObject
    {
        public override string GetLocalizedValue(WorldName value)
        {
            return WorldData.GetLocationName(value);
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

            Dictionary<WorldName,string> worlds = new Dictionary<WorldName,string>();
            WorldData.GetWorlds(worlds);

            foreach(WorldName value in worlds.Keys)
            {
                results.Add(new Item(this, value));
            }

            return results;
        }
    }
}
