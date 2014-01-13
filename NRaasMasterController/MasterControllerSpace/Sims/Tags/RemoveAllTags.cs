using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Tags
{
    public class RemoveAllTags : OptionItem, ITagsOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveAllTags";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            foreach (Sim sim in LotManager.Actors)
            {
                RemoveTag.Perform(sim);
            }

            return OptionResult.SuccessClose;
        }
    }
}
