using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Buffs
{
    public class AddBuffCustomList : AddBuff
    {
        public override string GetTitlePrefix()
        {
            return "AddBuffCustomList";
        }

        protected override IEnumerable<BuffInstance> GetBuffChoices()
        {
            IEnumerable<BuffInstance> allBuffs = base.GetBuffChoices();

            List<BuffInstance> buffs = new List<BuffInstance>();

            foreach (BuffInstance buff in allBuffs)
            {
                BuffNames buffName = (BuffNames)buff.BuffGuid;

                if (MasterController.Settings.mCustomBuffs.Contains(buffName.ToString()))
                {
                    buffs.Add(buff);
                }
            }

            return buffs;
        }
    }
}
