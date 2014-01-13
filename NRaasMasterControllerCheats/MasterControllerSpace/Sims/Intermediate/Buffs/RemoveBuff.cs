using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Buffs
{
    public class RemoveBuff : SimFromList, IBuffsOption
    {
        Dictionary<BuffNames, Moodlet.Item> mAllOptions = new Dictionary<BuffNames,Moodlet.Item>();

        List<BuffNames> mBuffs = new List<BuffNames>();

        public override string GetTitlePrefix()
        {
            return "RemoveBuff";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.BuffManager == null) return false;

            return base.PrivateAllow(me);
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected virtual IEnumerable<BuffInstance> GetBuffChoices()
        {
            return BuffManager.BuffDictionary.Values;
        }

        protected override CommonSpace.Options.OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            mAllOptions.Clear();

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription sim = miniSim as SimDescription;
                if (sim == null) continue;

                Sim createdSim = sim.CreatedSim;
                if (createdSim == null) continue;

                if (createdSim.BuffManager == null) continue;

                foreach (BuffInstance buff in createdSim.BuffManager.List)
                {
                    Moodlet.Item item;
                    if (!mAllOptions.TryGetValue(buff.Guid, out item))
                    {
                        item = new Moodlet.Item(buff, 0);
                        mAllOptions.Add(buff.Guid, item);
                    }

                    item.IncCount();
                }
            }

            return base.RunAll(sims);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                CommonSelection<Moodlet.Item>.Results choices = new CommonSelection<Moodlet.Item>(Name, me.FullName, mAllOptions.Values).SelectMultiple();
                if ((choices == null) || (choices.Count == 0)) return false;

                mBuffs.Clear();
                foreach (Moodlet.Item item in choices)
                {
                    mBuffs.Add(item.Value);
                }
            }

            foreach (BuffNames buff in mBuffs)
            {
                me.CreatedSim.BuffManager.RemoveElement(buff);
            }
            return true;
        }
    }
}
