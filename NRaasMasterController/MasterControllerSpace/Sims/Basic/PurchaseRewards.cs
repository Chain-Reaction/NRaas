using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
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
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class PurchaseRewards : SimFromList, IBasicOption
    {
        IEnumerable<SimTrait.Item> mChoices;

        Dictionary<TraitNames, bool> mPreviousChoices = new Dictionary<TraitNames, bool>();

        public override string GetTitlePrefix()
        {
            return "PurchaseRewards";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        public override void Reset()
        {
            mChoices = null;

            mPreviousChoices.Clear();

            base.Reset();
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.mSpendableHappiness <= 0) return false;

            return (me.TraitManager != null);
        }

        public static void UpdateInterface(SimDescription me)
        {
            if ((me.CreatedSim != null) && (me.CreatedSim.SocialComponent != null))
            {
                me.CreatedSim.SocialComponent.UpdateTraits();
            }

            if (me.CreatedSim == Sim.ActiveActor)
            {
                (Sims3.UI.Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnLifetimePointsChanged();
                (Sims3.UI.Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnRewardTraitsChanged();
            }
        }

        protected IEnumerable<SimTrait.Item> GetChoices (SimDescription me, int max)
        {
            List<SimTrait.Item> allOptions = new List<SimTrait.Item>();
            foreach (Trait trait in TraitManager.GetDictionaryTraits)
            {
                if (!trait.IsReward) continue;

                if (me != null)
                {
                    if (mPreviousChoices.ContainsKey(trait.Guid)) continue;

                    if (me.TraitManager.HasElement(trait.Guid))
                    {
                        continue;
                    }
                    else if (!trait.TraitValidForAgeSpecies(CASUtils.CASAGSAvailabilityFlagsFromCASAgeGenderFlags(CASAgeGenderFlags.Adult | me.Species)))
                    {
                        continue;
                    }

                    switch (trait.Guid)
                    {
                        case TraitNames.ForeverYoung:
                            if (!me.AgingEnabled) continue;
                            break;
                    }
                }

                if (Traits.IsObjectBaseReward(trait.Guid))
                {
                    if ((me != null) && (me.CreatedSim == null)) continue;
                }

                allOptions.Add(new SimTrait.Item(trait.Guid, trait.Score));
            }

            if (allOptions.Count == 0) return null;

            string title = Name;
            if (me != null)
            {
                title = me.FullName + Common.NewLine + Common.LocalizeEAString(false, "Ui/Caption/HUD/RewardTraitsShopDialog:Available", new object[] { (int)me.mSpendableHappiness });
            }

            CommonSelection<SimTrait.Item>.Results selection = new CommonSelection<SimTrait.Item>(Name, title, allOptions).SelectMultiple(max);
            if ((selection == null) || (selection.Count == 0)) return null;

            if (me != null)
            {
                foreach (SimTrait.Item item in selection)
                {
                    mPreviousChoices[item.Value] = true;
                }
            }

            return selection;
        }

        protected string SetTraits(SimDescription me, IEnumerable<SimTrait.Item> choices)
        {
            string result = Name + Common.NewLine + me.FullName;

            bool success = false;

            foreach (SimTrait.Item selection in choices)
            {
                if (me.TraitManager.HasElement(selection.Value)) continue;

                Trait selected = TraitManager.GetTraitFromDictionary(selection.Value);
                if (selected != null)
                {
                    if (!selected.TraitValidForAgeSpecies(CASUtils.CASAGSAvailabilityFlagsFromCASAgeGenderFlags(CASAgeGenderFlags.Adult | me.Species))) continue;

                    if (me.mSpendableHappiness < selected.Score) continue;

                    me.mSpendableHappiness -= selected.Score;

                    result += Common.NewLine + "  " + selected.TraitName(me.IsFemale);

                    success = true;

                    if (Traits.IsObjectBaseReward(selected.Guid))
                    {
                        if (me.Household == null) continue;

                        if (me.CreatedSim == null) continue;

                        HudModel hudModel = Sims3.UI.Responder.Instance.HudModel as HudModel;
                        if (hudModel != null)
                        {
                            bool showTNS = SimTypes.IsSelectable(me);
                            bool inHouseInventory = false;
                            Sim rewardedSim = me.CreatedSim;
                            hudModel.GiveRewardObjects(me.CreatedSim, selected.Guid, selected.ProductVersion, ref showTNS, ref inHouseInventory, ref rewardedSim);
                        }
                    }
                    else
                    {
                        switch (selected.Guid)
                        {
                            case TraitNames.ForeverYoung:
                                me.AgingEnabled = false;
                                break;
                            default:
                                if (selected.Guid == TraitNames.SuperVampire)
                                {
                                    me.AgingEnabled = false;
                                }

                                if (!me.AddTrait(selected))
                                {
                                    me.mSpendableHappiness += selected.Score;
                                }
                                break;
                        }
                    }
                }
            }

            if (!success)
            {
                result += Common.NewLine + "  " + Common.Localize(GetTitlePrefix() + ":None");
            }

            return result;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (singleSelection)
            {
                while (true)
                {
                    IEnumerable<SimTrait.Item> choices = GetChoices (me, 1);
                    if (choices == null) break;

                    SetTraits(me, choices);
                }

                UpdateInterface(me);
                return true;
            }
            else if (!ApplyAll)
            {
                mChoices = GetChoices(null, 0);
                if (mChoices == null) return false;
            }

            string result = SetTraits(me, mChoices);

            if (!string.IsNullOrEmpty(result))
            {
                Common.Notify(me, result);
            }

            return true;
        }
    }
}
