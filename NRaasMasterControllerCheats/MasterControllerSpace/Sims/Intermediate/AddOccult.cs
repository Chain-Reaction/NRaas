using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class AddOccult : OccultBase
    {
        public override string GetTitlePrefix()
        {
            return "AddOccult";
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!base.Run(me, singleSelection)) return false;

            foreach(SelectionCriteria.Occult.Values type in mTypes)
            {
                if (type.mOccult != OccultTypes.None)
                {                    
                    OccultTypeHelper.Add(me, type.mOccult, false, true);
                }
                else
                {
                    Urnstones.SimToPlayableGhost(me, type.mDeathType);
                }
            }

            return true;
        }
    }
}
