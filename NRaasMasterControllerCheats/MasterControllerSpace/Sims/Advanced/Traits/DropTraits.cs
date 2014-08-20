using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Traits
{
    public class DropTraits : SimFromList, ITraitOption
    {
        public override string GetTitlePrefix()
        {
            return "DropTraits";
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

            if (me.TraitManager == null) return false;

            if (me.TraitManager.CountVisibleTraits() == 0) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me }), me.TraitManager.CountVisibleTraits().ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                mCount = 0;
                if (!int.TryParse(text, out mCount))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            for (int i = 0; i < mCount; i++)
            {
                Trait trait = me.TraitManager.GetRandomVisibleElement();
                if (trait == null) continue;

                me.TraitManager.RemoveElement(trait.Guid);
            }

            if ((me.CreatedSim != null) && (me.CreatedSim.SocialComponent != null))
            {
                me.CreatedSim.SocialComponent.UpdateTraits();
            }

            return true;
        }
    }
}
