using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Town;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class ObjectReset : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "ObjectReset";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return new LotProcessor(GetTitlePrefix(), null).Perform(OnReset);
        }

        protected static bool OnReset(IGameObject obj)
        {
            Households.Reset.ResetObject(obj, true);
            return true;
        }
    }
}
