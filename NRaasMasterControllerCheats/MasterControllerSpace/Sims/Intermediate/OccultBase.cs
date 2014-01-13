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
    public abstract class OccultBase : SimFromList, IIntermediateOption
    {
        protected List<SelectionCriteria.Occult.Values> mTypes = new List<SelectionCriteria.Occult.Values>();

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected virtual bool IncludeGhostOccult
        {
            get
            {
                return false;
            }
        }

        protected override bool PrivateAllow(MiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.OccultManager == null) return false;

            if (me.IsPregnant) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<SelectionCriteria.Occult.Item> allOptions = new List<SelectionCriteria.Occult.Item>();

                foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
                {
                    if (!OccultTypeHelper.IsInstalled(type)) continue;

                    if (type == OccultTypes.None) continue;

                    if (type == OccultTypes.Ghost)
                    {
                        if (!IncludeGhostOccult) continue;
                    }

                    SelectionCriteria.Occult.Item item = new SelectionCriteria.Occult.Item(type, SimDescription.DeathType.None, 0, "Boolean");

                    if (item.Test(me, false, me))
                    {
                        item.IncCount();
                    }

                    allOptions.Add(item);
                }

                if (!IncludeGhostOccult)
                {
                    foreach (SimDescription.DeathType type in Enum.GetValues(typeof(SimDescription.DeathType)))
                    {
                        if (type == SimDescription.DeathType.None) continue;

                        SelectionCriteria.Occult.Item item = new SelectionCriteria.Occult.Item(OccultTypes.None, type, 0, "Boolean");

                        if (item.Test(me, false, me))
                        {
                            item.IncCount();
                        }

                        allOptions.Add(item);
                    }
                }

                CommonSelection<SelectionCriteria.Occult.Item>.Results choices = new CommonSelection<SelectionCriteria.Occult.Item>(Name, me.FullName, allOptions).SelectMultiple();
                if ((choices == null) || (choices.Count == 0)) return false;

                mTypes.Clear();

                foreach (SelectionCriteria.Occult.Item item in choices)
                {
                    mTypes.Add(item.Value);
                }
            }

            return true;
        }
    }
}
