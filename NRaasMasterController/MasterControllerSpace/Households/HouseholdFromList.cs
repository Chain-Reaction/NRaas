using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Dialogs;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public abstract class HouseholdFromList : Sims.SimFromList
    {
        public override string HotkeyID
        {
            get { return "house" + base.HotkeyID; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            Sim sim = parameters.mTarget as Sim;
            if (sim != null)
            {
                if (!Allow(GetLot (sim.SimDescription), sim.Household)) return false;
            }
            else
            {
                Lot lot = GetLot(parameters.mTarget, parameters.mHit);
                if (lot != null)
                {
                    if (!lot.IsResidentialLot)
                    {
                        if (lot.IsActive)
                        {
                            if (lot.CurrentLotDisplayLevel < 0) return false;
                        }
                        else
                        {
                            if (lot.LastDisplayedLevel < 0) return false;
                        }
                    }

                    if (!Allow(lot, lot.Household)) return false;
                }
            }

            return true;
        }

        protected virtual Lot GetLot(SimDescription sim)
        {
            if (sim.LotHome != null)
            {
                return sim.LotHome;
            }
            else
            {
                return sim.VirtualLotHome;
            }
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return (me.Household != null);
        }

        protected virtual bool Allow(Lot lot, Household house)
        {
            if ((lot == null) && (house == null)) return false;

            if (house != null)
            {
                bool success = false;
                foreach (SimDescription member in CommonSpace.Helpers.Households.All(house))
                {
                    if (Test(member))
                    {
                        success = true;
                        break;
                    }
                }

                if (!success) return false;
            }

            return true;
        }

        protected abstract OptionResult Run(Lot lot, Household house);

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Dictionary<Household, LotHouseItem> houses = new Dictionary<Household, LotHouseItem>();

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription sim = miniSim as SimDescription;
                if (sim == null) continue;

                if (sim.Household == null) continue;

                houses[sim.Household] = new LotHouseItem(sim.LotHome, sim.Household, 1);
            }

            List<LotHouseItem> results = new List<LotHouseItem>();

            foreach (LotHouseItem lotHouse in houses.Values)
            {
                results.Add(lotHouse);
            }

            return RunAll(results);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (IsSimOption.sSet)
            {
                return base.Run(parameters);
            }

            List<LotHouseItem> houses = new List<LotHouseItem>();

            bool okayed = true;

            Sim sim = parameters.mTarget as Sim;
            if ((sim != null) && (sim.Household != null))
            {
                Lot lot = GetLot(sim.SimDescription);

                houses.Add(new LotHouseItem(lot, sim.Household, GetCount(lot, sim.Household)));
            }
            else
            {
                Lot lot = GetLot(parameters.mTarget, parameters.mHit);

                if ((lot != null) && (!lot.IsBaseCampLotType))
                {
                    houses.Add(new LotHouseItem(lot, lot.Household, GetCount(lot, lot.Household)));
                }
                else
                {
                    Sim actorSim = parameters.mActor as Sim;

                    List<LotHouseItem> results = GetSelection(actorSim.SimDescription, Name, null, GetMaxSelection(), out okayed);
                    if (results != null)
                    {
                        foreach (LotHouseItem house in results)
                        {
                            houses.Add(house);
                        }
                    }
                }
            }

            if (!okayed) return OptionResult.Failure;

            return RunAll(houses);
        }

        protected virtual OptionResult RunAll(List<LotHouseItem> houses)
        {
            if (houses != null)
            {
                bool askedAndAnswered = !CanApplyAll ();

                foreach (LotHouseItem house in houses)
                {
                    if (!Allow(house.mLot, house.mHouse)) continue;

                    if (Run(house.mLot, house.mHouse) == OptionResult.Failure) return OptionResult.Failure;

                    if (!askedAndAnswered)
                    {
                        askedAndAnswered = true;

                        if (houses.Count > 1)
                        {
                            if ((AutoApplyAll()) || (AcceptCancelDialog.Show (Common.Localize("HouseholdInteraction:ApplyAll"))))
                            {
                                ApplyAll = true;
                            }
                        }
                    }
                }

                return RunResult;
            }
            return OptionResult.Failure;
        }

        protected virtual int GetCount(Lot lot, Household house)
        {
            if (house != null)
            {
                return CommonSpace.Helpers.Households.NumSimsIncludingPregnancy(house);
            }
            else
            {
                return 0;
            }
        }

        public List<LotHouseItem> GetSelection(SimDescription me, string title, List<SimSelection.ICriteria> criteria, int maxSelection, out bool okayed)
        {
            okayed = false;

            bool criteriaCanceled;
            SimSelection sims = SimSelection.Create(title, me, this, criteria, true, false, out criteriaCanceled);
            if (sims.IsEmpty)
            {
                SimpleMessageDialog.Show(title, Common.Localize ("HouseholdInteraction:NoChoices"));
                return null;
            }

            Dictionary<Lot, bool> lotLookup = new Dictionary<Lot, bool>();
            Dictionary<Household, bool> houseLookup = new Dictionary<Household, bool>();

            List<LotHouseItem> houses = new List<LotHouseItem>();

            foreach (IMiniSimDescription miniSim in sims.All)
            {
                SimDescription sim = miniSim as SimDescription;
                if (sim == null) continue;

                if (sim.Household == null) continue;

                if (sim.Household.IsPreviousTravelerHousehold) continue;
                
                if ((sim.LotHome != null) && (!lotLookup.ContainsKey(sim.LotHome)))
                {
                    lotLookup.Add(sim.LotHome, true);
                }

                if (houseLookup.ContainsKey(sim.Household)) continue;
                houseLookup.Add(sim.Household, true);

                if (!Allow(GetLot (sim), sim.Household)) continue;

                houses.Add(new LotHouseItem(sim.LotHome, sim.Household, GetCount(sim.LotHome, sim.Household)));
            }

            foreach (Lot lot in LotManager.AllLots)
            {
                if (lotLookup.ContainsKey(lot)) continue;
                lotLookup.Add(lot, true);

                if (!Allow(lot, null)) continue;

                houses.Add(new LotHouseItem(lot, null, GetCount(lot, null)));
            }

            if (houses.Count == 1)
            {
                return houses;
            }
            else
            {
                if ((AllSimsOnFilterCancel) && (criteriaCanceled))
                {
                    return houses;
                }
                else
                {
                    if (CanApplyAll())
                    {
                        houses.Add(new LotHouseItem(null, null, 0));
                    }

                    CommonSelection<LotHouseItem>.Results selection = new CommonSelection<LotHouseItem>(title, houses, new LotTypeColumn()).SelectMultiple(maxSelection);

                    okayed = selection.mOkayed;

                    if ((selection.Count == 0) && (okayed))
                    {
                        return houses;
                    }
                    else
                    {
                        foreach (LotHouseItem item in selection)
                        {
                            if (item.IsAll)
                            {
                                return houses;
                            }
                        }

                        return new List<LotHouseItem>(selection);
                    }
                }
            }
        }

        public class LotTypeColumn : ObjectPickerDialogEx.CommonHeaderInfo<LotHouseItem>
        {
            static Dictionary<CommercialLotSubType, Lot.CommercialSubTypeData> sComercialLotTypes;

            public LotTypeColumn()
				: base("NRaas.MasterController.OptionList:LotTypeTitle", "NRaas.MasterController.OptionList:LotTypeTitle", /*40*/ 70)
            { }

            public override ObjectPicker.ColumnInfo GetValue(LotHouseItem item)
            {
                if (sComercialLotTypes == null)
                {
                    sComercialLotTypes = new Dictionary<CommercialLotSubType, Lot.CommercialSubTypeData>();

                    foreach (Lot.CommercialSubTypeData commData in Lot.sCommnunityTypeData)
                    {
                        sComercialLotTypes[commData.CommercialLotSubType] = commData;
                    }
                }

                string result = "";

                if ((item.mLot != null) && (item.mLot.IsCommunityLot))
                {
                    Lot.CommercialSubTypeData commData;
                    if (sComercialLotTypes.TryGetValue(item.mLot.CommercialLotSubType, out commData))
                    {
                        result = Localization.LocalizeString(commData.LocalizationStringKey, new object[0x0]);
                    }
                }

                return new ObjectPicker.TextColumn(result);
            }
        }

        public class LotHouseItem : CommonOptionItem
        {
            public readonly Lot mLot;
            public readonly Household mHouse;

            public LotHouseItem(Lot lot, Household house, int count)
                : base (null, count)
            {
				if ((lot == null) && (house == null))
                {
                    mName = "(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")";
                }
                else if (house != null)
                {
                    mName = SelectionCriteria.TownFamily.GetQualifiedName(house);
                }
                else if (lot is WorldLot)
                {
                    mName = "(" + Common.Localize("HouseholdInteraction:WorldLot") + ")";
                }
                else
                {
                    mName = lot.Name;
					if (!string.IsNullOrEmpty(mName))
                    {
                        mName += " - ";
                    }
                    mName += lot.Address;

                    if (string.IsNullOrEmpty(mName))
                    {
                        mName = EAText.GetNumberString(lot.LotId);
                    }
                }

                if (lot != null)
                {
                    mThumbnail = lot.GetThumbnailKey();
                }

                mLot = lot;
                mHouse = house;
            }

            public override string DisplayValue
            {
                get { return null; }
            }

            public override int ValueWidth
            {
                get
                {
                    return 20;
                }
            }

            public bool IsAll
            {
                get
                {
                    if (mLot != null) return false;

                    return (mHouse == null);
                }
            }
        }
    }
}
