using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFilters
{
    public abstract class FilterBaseOption<TTarget> : GenericSettingOption<DoorPortalComponentEx.DoorSettings, TTarget>
        where TTarget : class, IGameObject
    {
        public override void SetImportValue(string value)
        {
            //Value = value;
        }

        protected override OptionResult Run(GameHitParameters<TTarget> parameters)
        {
            Value = Value;           

            Common.Notify(ToString());

            if (!FilterHelper.FilterHasMatches(mName))
            {
                Common.Notify(Common.Localize("DoorOptions:FilterHasNoMatches"));
            }

            return OptionResult.SuccessRetain;
        }
    }
}