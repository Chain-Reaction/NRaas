using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Rewards;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class ChangeAlmaMaterName : SimFromList, ICareerOption
    {
        string mSchoolName = null;

        public override string GetTitlePrefix()
        {
            return "AlmaMaterName";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.AlmaMater != AlmaMater.Community) return false;

            return base.PrivateAllow(me);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                mSchoolName = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me }), me.AlmaMaterName);
                if (string.IsNullOrEmpty(mSchoolName)) return false;
            }

            me.AlmaMaterName = mSchoolName;

            if (me.CreatedSim != null)
            {
                foreach (Diploma diploma in Inventories.QuickFind<Diploma>(me.CreatedSim.Inventory))
                {
                    diploma.mSchoolName = me.AlmaMaterName;
                }
            }
            return true;
        }
    }
}
