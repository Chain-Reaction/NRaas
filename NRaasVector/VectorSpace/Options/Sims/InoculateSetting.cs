using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.VectorSpace.Booters;
using NRaas.VectorSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Options.Sims
{
    public class InoculateSetting : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "Inoculate";
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            List<VectorBooter.Item> choices = VectorBooter.GetVectorItems(parameters.mTarget.SimDescription);

            CommonSelection<VectorBooter.Item>.Results items = new CommonSelection<VectorBooter.Item>(Name, choices).SelectMultiple();
            if ((items == null) || (items.Count == 0)) return OptionResult.Failure;

            string vectors = null;

            foreach (VectorBooter.Item item in items)
            {
                VectorControl.Inoculate(parameters.mTarget.SimDescription, item.Value, true, false);

                vectors += Common.NewLine + " " + item.Value.GetLocalizedName(parameters.mTarget.IsFemale);
            }

            if (!string.IsNullOrEmpty(vectors))
            {
                Common.Notify(parameters.mTarget, Common.Localize("Inoculate:Success", parameters.mTarget.IsFemale, new object[] { parameters.mTarget }) + vectors);
            }

            return OptionResult.SuccessClose;
        }
    }
}
