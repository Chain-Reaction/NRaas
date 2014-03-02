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

namespace NRaas.MasterControllerSpace.Sims.Advanced.Traits
{
    public class CleanTraits : SimFromList, ITraitOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanTraits";
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
            if (!base.PrivateAllow(me)) return false;

            return (me.TraitManager != null && !me.IsEP11Bot);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("CleanTraits:Prompt", me.IsFemale, new object[] { me })))
                {
                    return false;
                }
            }

            if (me.TraitManager != null)
            {
                List<Trait> traits = new List<Trait>(me.TraitManager.List);

                foreach (Trait trait in traits)
                {
                    me.TraitManager.RemoveTraitEffects(me, trait.Guid);
                    me.TraitManager.RemoveElement(trait.Guid);
                }

                me.TraitManager.RewardTraits.Clear();
                me.TraitManager.RemoveAllElements();
            }
            return true;
        }
    }
}
