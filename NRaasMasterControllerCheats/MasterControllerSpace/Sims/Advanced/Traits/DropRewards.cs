using NRaas.CommonSpace.Dialogs;
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
    public class DropRewards : SimFromList, ITraitOption
    {
        CommonSelection<SimTrait.Item>.Results mSelection = null;

        public override string GetTitlePrefix()
        {
            return "DropRewards";
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

            return (me.TraitManager != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<SimTrait.Item> allOptions = new List<SimTrait.Item>();
                foreach (Sims3.Gameplay.ActorSystems.Trait trait in TraitManager.GetDictionaryTraits)
                {
                    if (!me.TraitManager.HasElement(trait.Guid)) continue;

                    if (!trait.IsReward) continue;

                    allOptions.Add(new SimTrait.Item(trait.Guid, 1));
                }

                mSelection = new CommonSelection<SimTrait.Item>(Name, me.FullName, allOptions).SelectMultiple();
                if ((mSelection == null) || (mSelection.Count == 0)) return false;
            }

            foreach (SimTrait.Item item in mSelection)
            {
                if (item == null) continue;

                TraitNames traitName = item.Value;

                if (!me.TraitManager.HasElement(traitName)) continue;

                Sims3.Gameplay.ActorSystems.Trait trait = TraitManager.GetTraitFromDictionary(traitName);
                if (trait != null)
                {
                    me.RemoveTrait(trait);

                    me.mSpendableHappiness += trait.Score;
                }
            }

            if ((me.CreatedSim != null) && (me.CreatedSim.SocialComponent != null))
            {
                me.CreatedSim.SocialComponent.UpdateTraits();
            }

            return true;
        }
    }
}
