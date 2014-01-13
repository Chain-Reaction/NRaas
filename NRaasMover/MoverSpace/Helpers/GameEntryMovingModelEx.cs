using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MoverSpace.Helpers
{
    public class GameEntryMovingModelEx : GameEntryMovingModel
    {
        protected GameEntryMovingModelEx()
        { }
        public GameEntryMovingModelEx(Household household)
            : base(household)
        { }

        public override MoveValidity IsLotValid(bool isSource, ref string reason)
        {
            return GameplayMovingModelEx.BaseIsLotValid(this, mSourceSimList, mTargetSimList, mSourceHousehold, isSource, ref reason);
        }

        public override int GetHouseholdBuyingPowerFunds(bool isSource)
        {
            if (Mover.Settings.mFreeRealEstate)
            {
                return int.MaxValue;
            }
            else
            {
                return GetHouseholdFunds(isSource);
            }
        }

        public override int GetHouseholdFunds(bool isSource)
        { 
            return GameplayMovingModelEx.GetHouseholdFunds(this, isSource);
        }

        public override int GetLotWorth(bool isSource)
        {
            if (Mover.Settings.mFreeRealEstate) return 0;

            if (isSource)
            {
                if (mSourceHousehold.LotHome != null)
                {
                    return Mover.GetLotCost(mSourceHousehold.LotHome);
                }
            }
            else
            {
                if (mHouseboatSize != Sims3.SimIFace.Enums.HouseboatSize.None)
                {
                    return GetTargetLotCost();
                }
                else if (mTargetLot != null)
                {
                    return Mover.GetLotCost(mTargetLot);
                }
            }

            return -1;
        }

        public override void Apply()
        {
            try
            {
                List<SimDescription> allSims = new List<SimDescription>();
                if (mSourceSimList != null)
                {
                    foreach (KeyValuePair<ISimDescription, bool> pair in mSourceSimList)
                    {
                        SimDescription sim = pair.Key as SimDescription;
                        if (sim == null) continue;

                        allSims.Add(sim);
                    }
                }

                if (mTargetSimList != null)
                {
                    foreach (KeyValuePair<ISimDescription, bool> pair in mTargetSimList)
                    {
                        SimDescription sim = pair.Key as SimDescription;
                        if (sim == null) continue;

                        allSims.Add(sim);
                    }
                }

                using (DreamCatcher.HouseholdStore store = new DreamCatcher.HouseholdStore(allSims, Mover.Settings.mDreamCatcher))
                {
                    try
                    {
                        SplitMergeHouseholds += OnMerge;

                        base.Apply();
                    }
                    catch (Exception e)
                    {
                        Common.Exception("Apply", e);
                    }
                    finally
                    {
                        SplitMergeHouseholds -= OnMerge;

                        ProgressDialog.Close();
                    }
                }
            }
            catch (ExecutionEngineException)
            { }
        }

        private static void OnMerge(Household sourceHousehold, Household targetHousehold, List<Sim> simsMovingToTargetHousehold, bool familyInventoriesWillMerge)
        {
            try
            {
                if (familyInventoriesWillMerge)
                {
                    Households.TransferData(targetHousehold, sourceHousehold);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnMerge", e);
            }
        } 

        public static void MergeHouseholds(EditTownController ths, UIBinInfo from, UIBinInfo to)
        {
            try
            {
               Household household = Household.Find(from.HouseholdId);
                if (household != null)
                {
                    IMovingModel model = new GameEntryMovingModelEx(household);
                    if (model != null)
                    {
                        model.SetTargetLot(to.LotId);
                        EditTownMergeDialog.Show(model);

                        Common.FunctionTask.Perform(ths.MergeHouseholdsTask);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("MergeHouseholds", e);
            }
        }

        public static void SplitHousehold(EditTownController ths, UIBinInfo from)
        {
            try
            {
                if (ths == null) return;

                Household household = Household.Find(from.HouseholdId);
                if (household != null)
                {
                    IMovingModel model = new GameEntryMovingModelEx(household);
                    if (model != null)
                    {
                        EditTownSplitDialog.Show(model);
                        Simulator.AddObject(new OneShotFunctionWithParams(ths.SplitHouseholdsTask, from));
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("SplitHousehold", e);
            }
        }
    }
}
