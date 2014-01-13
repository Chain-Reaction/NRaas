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
using Sims3.Gameplay.Objects;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class RemoveOccult : OccultBase
    {
        public override string GetTitlePrefix()
        {
            return "RemoveOccult";
        }

        protected override bool IncludeGhostOccult
        {
            get
            {
                return true;
            }
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!base.Run(me, singleSelection)) return false;

            foreach (SelectionCriteria.Occult.Values type in mTypes)
            {
                if (type.mOccult == OccultTypes.Ghost)
                {
                    Urnstone stone = Urnstones.FindGhostsGrave(me);

                    if ((stone != null) && (me.CreatedSim != null))
                    {
                        stone.GhostToSim(me.CreatedSim, false, false);
                    }
                    else
                    {
                        me.SetDeathStyle(SimDescription.DeathType.None, false);
                        me.IsGhost = false;
                        me.IsNeverSelectable = false;
                        me.ShowSocialsOnSim = true;
                        me.AgingEnabled = true;

                        if (stone != null)
                        {
                            stone.Destroy();
                        }
                    }
                }
                else
                {
                    OccultTypeHelper.Remove(me, type.mOccult, true);
                }
            }

            return true;
        }
    }
}
