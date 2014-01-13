using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class ChangeLTWCount : SimFromList, IAdvancedOption
    {
        new float mCount = 0;

        public override string GetTitlePrefix()
        {
            return "ChangeLTWCount";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.DreamsAndPromisesManager == null) return false;

            ActiveDreamNode node = me.CreatedSim.DreamsAndPromisesManager.LifetimeWishNode;
            if (node == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Sim sim = me.CreatedSim;
            if (sim == null) return false;

            ActiveDreamNode node = sim.DreamsAndPromisesManager.LifetimeWishNode;
            if (node == null) return false;

            float value = node.InternalCount;

            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me }), value.ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                mCount = 0;
                if (!float.TryParse(text, out mCount))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            node.InternalCount += mCount;

            return true;
        }
    }
}
