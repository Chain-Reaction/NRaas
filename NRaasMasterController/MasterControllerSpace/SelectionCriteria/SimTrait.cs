using NRaas.CommonSpace.Dialogs;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class SimTrait : SimTraitBase<SimTrait.Item>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Trait";
        }

        public class Item : SimTraitBase<SimTrait.Item>.ItemBase
        {
            public Item()
            { }
            public Item(TraitNames guid, int count)
                : base(guid, count)
            { }
        }
    }
}
