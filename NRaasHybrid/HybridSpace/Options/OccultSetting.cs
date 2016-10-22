using NRaas.CommonSpace.Helpers;
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
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.HybridSpace.Options
{
    public abstract class OccultSetting<TObject> : ListedSettingOption<OccultTypes, TObject>
        where TObject : class, IGameObject
    {
        public override string GetLocalizedValue(OccultTypes value)
        {
            return OccultTypeHelper.GetLocalizedName(value);
        }

        protected override bool Allow(OccultTypes value)
        {
            /*
            switch (value)
            {
                case OccultTypes.Witch:
                case OccultTypes.Genie:
                case OccultTypes.Fairy:
                    return true;
            }
             */

            return false;
        }

        public override bool ConvertFromString(string value, out OccultTypes newValue)
        {
            return ParserFunctions.TryParseEnum<OccultTypes>(value, out newValue, OccultTypes.None);
        }

        public override string ConvertToString(OccultTypes value)
        {
            return value.ToString();
        }
    }
}
