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
    public class ChangeTraits : SimFromList, ITraitOption
    {
        public override string GetTitlePrefix()
        {
            return "ChangeTraits";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return (me.TraitManager != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            List<SimTrait.Item> allOptions = new List<SimTrait.Item>();
            foreach (Sims3.Gameplay.ActorSystems.Trait trait in TraitManager.GetDictionaryTraits)
            {
                if (trait.IsReward) continue;

                if (!trait.TraitValidForAgeSpecies(me.GetCASAGSAvailabilityFlags())) continue;

                int count = 0;
                if (me.TraitManager.HasElement(trait.Guid))
                {
                    count = 1;
                }

                allOptions.Add(new SimTrait.Item (trait.Guid, count));
            }

            CommonSelection<SimTrait.Item>.Results selection = new CommonSelection<SimTrait.Item>(Name, me.FullName, allOptions, new SimTrait.AuxillaryColumn()).SelectMultiple();
            if (selection.Count == 0) return false;

            foreach (SimTrait.Item item in selection)
            {
                if (item == null) continue;

                TraitNames traitName = item.Value;

                Sims3.Gameplay.ActorSystems.Trait trait = TraitManager.GetTraitFromDictionary(traitName);
                if (trait != null)
                {
                    if (me.TraitManager.HasElement(traitName))
                    {
                        if (me.TraitManager.mSocialGroupTraitGuid == traitName)
                        {
                            me.RemoveSocialGroupTrait();
                        }
                        else if (me.TraitManager.mUniversityGraduateTraitGuid == traitName)
                        {
                            me.RemoveUniversityGraduateTrait();
                        }
                        else
                        {
                            me.RemoveTrait(trait);
                        }
                    }
                    else
                    {
                        int iTraitsForBabiesAndToddlers = TraitManager.kTraitsForBabiesAndToddlers;
                        int iTraitsForChildren = TraitManager.kTraitsForChildren;
                        int iTraitsForTeens = TraitManager.kTraitsForTeens;
                        int iTraitsForYoungAdultAndOlder = TraitManager.kTraitsForYoungAdultAndOlder;

                        try
                        {
                            // Don't use MaxValue as EA adds to this number for [[University]] purposes
                            TraitManager.kTraitsForBabiesAndToddlers = 10000000;
                            TraitManager.kTraitsForChildren = 10000000;
                            TraitManager.kTraitsForTeens = 10000000;
                            TraitManager.kTraitsForYoungAdultAndOlder = 10000000;

                            if (trait.IsHidden)
                            {
                                me.TraitManager.AddHiddenElement(traitName);
                            }
                            else
                            {
                                me.AddTrait(trait);
                            }
                        }
                        finally
                        {
                            TraitManager.kTraitsForBabiesAndToddlers = iTraitsForBabiesAndToddlers;
                            TraitManager.kTraitsForChildren = iTraitsForChildren;
                            TraitManager.kTraitsForTeens = iTraitsForTeens;
                            TraitManager.kTraitsForYoungAdultAndOlder = iTraitsForYoungAdultAndOlder;
                        }
                    }
                }
            }

            if ((me.CreatedSim != null) && (me.CreatedSim.SocialComponent != null))
            {
                me.CreatedSim.SocialComponent.UpdateTraits();
            }

            if (me.TraitManager.CountVisibleTraits() > me.TraitManager.NumTraitsForAge())
            {
                Common.Notify(Common.Localize("ChangeTraits:MaxExceeded", me.IsFemale, new object[] { me, me.TraitManager.CountVisibleTraits(), me.TraitManager.NumTraitsForAge() }));
            }

            return true;
        }
    }
}
