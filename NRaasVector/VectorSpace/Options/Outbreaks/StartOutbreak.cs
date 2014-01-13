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

namespace NRaas.VectorSpace.Options.Outbreaks
{
    public class StartOutbreak : OperationSettingOption<GameObject>, IOutbreaksOption
    {
        public override string GetTitlePrefix()
        {
            return "StartOutbreak";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<VectorBooter.Item> choices = VectorBooter.GetVectorItems(null);

            CommonSelection<VectorBooter.Item>.Results items = new CommonSelection<VectorBooter.Item>(Name, choices).SelectMultiple();
            if ((items == null) || (items.Count == 0)) return OptionResult.Failure;

            foreach (VectorBooter.Item item in items)
            {
                OutbreakControl.StartOutbreak(item.Value, true);
            }

            return OptionResult.SuccessRetain;
        }
    }
}
