using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class AlienPercentage : SimFromList, IIntermediateOption
    {
        float mPercent = 0;

        public override string GetTitlePrefix()
        {
            return "Criteria.AlienPercentage";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me, 100 }), me.AlienDNAPercentage.ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                mPercent = 0;
                if (!float.TryParse(text, out mPercent))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            me.mAlienDNAPercentage = mPercent;

            if (me.CreatedSim != null)
            {
                me.CreatedSim.Motives.RecreateMotives(me.CreatedSim);
            }
            return true;
        }
    }
}
