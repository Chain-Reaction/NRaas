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
    public class ChangeSocialTrait : SimFromList, ITraitOption
    {
        public override string GetTitlePrefix()
        {
            return "ChangeSocialTrait";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool Allow(CommonSpace.Options.GameHitParameters<GameObject> parameters)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP9)) return false;

            return base.Allow(parameters);
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.TraitManager == null) return false;

            //if (!me.TraitManager.mSocialGroupTraitEnabled) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            List<SimTrait.Item> allOptions = new List<SimTrait.Item>();
            foreach (Sims3.Gameplay.ActorSystems.Trait trait in TraitManager.GetDictionaryTraits)
            {
                if (trait.IsReward) continue;

                if (!trait.TraitValidForAgeSpecies(me.GetCASAGSAvailabilityFlags())) continue;

                int count = 0;
                if (me.TraitManager.mSocialGroupTraitGuid == trait.Guid)
                {
                    count = 1;
                }
                else if (me.HasTrait(trait.Guid))
                {
                    continue;
                }

                allOptions.Add(new SimTrait.Item (trait.Guid, count));
            }

            SimTrait.Item selection = new CommonSelection<SimTrait.Item>(Name, me.FullName, allOptions, new SimTrait.AuxillaryColumn()).SelectSingle();
            if (selection == null) return false;

            TraitNames traitName = selection.Value;

            me.RemoveSocialGroupTrait();

            if (traitName != me.TraitManager.mSocialGroupTraitGuid)
            {
                Sims3.Gameplay.ActorSystems.Trait selTrait = TraitManager.GetTraitFromDictionary(traitName);
                if (selTrait != null)
                {
                    me.TraitManager.mSocialGroupTraitEnabled = true;
                    me.AddSocialGroupTrait(selTrait);
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
