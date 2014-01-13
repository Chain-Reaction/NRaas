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
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Buffs
{
    public class AddBuff : SimFromList, IBuffsOption
    {
        List<BuffNames> mBuffs = new List<BuffNames>();

        public override string GetTitlePrefix()
        {
            return "AddBuff";
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

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<Moodlet.Item> options = new List<Moodlet.Item>();
                foreach (BuffInstance instance in GetBuffChoices())
                {
                    int value = 0;
                    if (me.CreatedSim.BuffManager.HasElement(instance.mBuffGuid))
                    {
                        value = 1;
                    }

                    if (singleSelection)
                    {
                        if ((me.GetCASAGSAvailabilityFlags() & instance.mBuff.AvailabilityFlags) == CASAGSAvailabilityFlags.None)
                        {
                            continue;
                        }
                    }

                    options.Add(new Moodlet.Item(instance, value));
                }

                CommonSelection<Moodlet.Item>.Results choices = new CommonSelection<Moodlet.Item>(Name, me.FullName, options).SelectMultiple();
                if ((choices == null) || (choices.Count == 0)) return false;

                mBuffs.Clear();
                foreach (Moodlet.Item item in choices)
                {
                    mBuffs.Add(item.Value);
                }
            }

            foreach (BuffNames buff in mBuffs)
            {
                me.CreatedSim.BuffManager.AddElement(buff, Origin.None);
            }
            return true;
        }
    }
}
