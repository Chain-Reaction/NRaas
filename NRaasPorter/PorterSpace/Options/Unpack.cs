using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Proxies;
using NRaas.CommonSpace.Selection;
using NRaas.CommonSpace.Tasks;
using NRaas.PorterSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.PorterSpace.Options
{
    public class Unpack : OperationSettingOption<GameObject>, ILotOption
    {
        bool mPrompt = true;
        bool mAskedAndAnswered = false;

        public override string GetTitlePrefix()
        {
            return "Unpack";
        }

        protected bool Satisfies(Lot lot, List<SimDescription> sims)
        {
            List<HomeInspection.Result> results = new HomeInspection(lot).Satisfies(sims);

            foreach (HomeInspection.Result result in results)
            {
                switch (result.mReason)
                {
                    case HomeInspection.Reason.TooFewBeds:
                        continue;
                }
                return false;
            }

            return true;
        }

        public override void Reset()
        {
            mPrompt = true;
            mAskedAndAnswered = false;

            base.Reset();
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            Lot lot = Porter.GetLot(parameters.mTarget);
            if (lot == null) return false;

            return (lot.Household == null);
        }

        protected static List<SimDescription> FindSim(SimDescription me, Dictionary<ulong, IMiniSimDescription> residents)
        {
            List<SimDescription> choices = new List<SimDescription>();

            foreach (IMiniSimDescription miniSim in residents.Values)
            {
                SimDescription sim = miniSim as SimDescription;
                if (sim == null) continue;

                if (me == sim) continue;

                if (me.FirstName != sim.FirstName) continue;

                if (me.FavoriteColor != sim.FavoriteColor) continue;

                if (me.IsVampire == sim.IsVampire)
                {
                    if (me.FavoriteFood != sim.FavoriteFood) continue;
                }

                if (me.FavoriteMusic != sim.FavoriteMusic) continue;

                choices.Add(sim);
            }

            return choices;
        }

        protected static void UnusedLog(string text)
        { }

        protected bool HandleDoppleganger(SimDescription loadedSim, Dictionary<ulong, IMiniSimDescription> preExistingSims, Dictionary<ulong, IMiniSimDescription> allSims, List<SimDescription> dependents)
        {
            List<SimDescription> choices = FindSim(loadedSim, preExistingSims);

            if (choices.Count == 0) return false;

            SimDescription existingSim = null;

            if ((choices.Count == 1) && (!mAskedAndAnswered))
            {
                mAskedAndAnswered = true;

                mPrompt = !AcceptCancelDialog.Show(Common.Localize("Doppleganger:Prompt"));
            }

            if ((choices.Count == 1) && (!mPrompt))
            {
                existingSim = choices[0];
            }
            else
            {
                choices.Add(loadedSim);

                existingSim = new DopplegangerSelection(loadedSim, choices).SelectSingle();
            }

            if (existingSim == null) return false;

            if (existingSim == loadedSim) return false;

            if (existingSim.Genealogy == null)
            {
                existingSim.Fixup();
            }

            if ((loadedSim.Genealogy != null) && (existingSim.Genealogy != null))
            {
                loadedSim.Genealogy.ClearDerivedData();

                dependents.AddRange(Relationships.GetChildren(loadedSim));

                new Relationships.RepairParents().Perform(existingSim, loadedSim.Genealogy.mNaturalParents, UnusedLog, allSims);
                new Relationships.RepairChildren().Perform(existingSim, loadedSim.Genealogy.mChildren, UnusedLog, allSims);
                new Relationships.RepairSiblings().Perform(existingSim, loadedSim.Genealogy.mSiblings, UnusedLog, allSims);

                new Relationships.RepairParents().Perform(existingSim, loadedSim.Genealogy.mNaturalParents, UnusedLog, preExistingSims);
                new Relationships.RepairChildren().Perform(existingSim, loadedSim.Genealogy.mChildren, UnusedLog, preExistingSims);
                new Relationships.RepairSiblings().Perform(existingSim, loadedSim.Genealogy.mSiblings, UnusedLog, preExistingSims);
            }

            List<Relationship> relations = new List<Relationship>(Relationship.GetRelationships(loadedSim));
            if (relations != null)
            {
                foreach (Relationship relation in relations)
                {
                    if (relation == null) continue;

                    Relationship newRelation = Relationship.Get(existingSim, relation.GetOtherSimDescription(loadedSim), true);
                    if (newRelation == null) continue;

                    newRelation.LTR.ForceChangeState(relation.LTR.CurrentLTR);
                    newRelation.LTR.SetLiking(relation.LTR.Liking);
                    newRelation.LTR.AddInteractionBit (relation.LTR.LTRInteractionBits);
                }
            }

            if (loadedSim.mPartner != null)
            {
                dependents.Add(loadedSim.mPartner);
            }

            if ((loadedSim.Genealogy != null) && (existingSim.Genealogy != null))
            {
                if ((existingSim.Genealogy.Spouse == null) && (loadedSim.Genealogy.Spouse != null))
                {
                    existingSim.Genealogy.mSpouse = loadedSim.Genealogy.Spouse;
                    existingSim.Genealogy.mSpouse.mSpouse = existingSim.Genealogy;

                    existingSim.mPartner = existingSim.Genealogy.mSpouse.SimDescription;

                    if (existingSim.mPartner != null)
                    {
                        existingSim.mPartner.mPartner = existingSim;
                    }
                }
            }

            if ((existingSim.mPartner == null) && (loadedSim.mPartner != null))
            {
                existingSim.mPartner = loadedSim.mPartner;
                existingSim.mPartner.mPartner = existingSim;
            }

            dependents.Add(existingSim);

            return true;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.StringBuilder log = new Common.StringBuilder("Run");

            try
            {
                Lot targetLot = Porter.GetLot(parameters.mTarget);
                if (targetLot == null) return OptionResult.Failure;

                Dictionary<ulong, IMiniSimDescription> preExistingSims = SimListing.GetSims<IMiniSimDescription>(null, true);

                Household me = targetLot.Household;
                if (me == null)
                {
                    me = GetImportSelection(targetLot);

                    if (me == null) return OptionResult.Failure;
                }

                Dictionary<ulong, IMiniSimDescription> allSims = SimListing.GetSims<IMiniSimDescription>(null, true);

                Dictionary<int, HouseData> houses = new Dictionary<int, HouseData>();
                Dictionary<int, HouseData> doppleHouses = new Dictionary<int, HouseData>();

                List<SimDescription> cleanUp = new List<SimDescription>();
                List<SimDescription> fixUp = new List<SimDescription>();

                List<Household> importedHouses = new List<Household>();
                importedHouses.Add(me);

                List<SimDescription> checkSims = new List<SimDescription>();

                log += Common.NewLine + "A";

                List<SimDescription> sims = new List<SimDescription>(me.AllSimDescriptions);
                foreach (SimDescription sim in sims)
                {
                    if (sim == null) continue;

                    log += Common.NewLine + sim.FullName;

                    Porter.AddExport(sim);

                    string description = sim.mBio;
                    if (string.IsNullOrEmpty(description)) continue;

                    if (!description.Contains("NRaas.Porter:")) continue;

                    description = description.Replace("NRaas.Porter:", "");
                    if (string.IsNullOrEmpty(description)) continue;

                    HouseData lookup = new HouseData(description);

                    lookup.Reconcile(sim);

                    ValidateTask.Perform(sim);

                    List<SimDescription> dependents = new List<SimDescription>();
                    if (HandleDoppleganger(sim, preExistingSims, allSims, dependents))
                    {
                        if (!houses.ContainsKey(lookup.mID))
                        {
                            if (!doppleHouses.ContainsKey(lookup.mID))
                            {
                                doppleHouses.Add(lookup.mID, lookup);
                            }
                        }

                        checkSims.AddRange(dependents);

                        cleanUp.Add(sim);
                        continue;
                    }
                    else
                    {
                        fixUp.Add(sim);
                    }

                    HouseData data;
                    if (!houses.TryGetValue(lookup.mID, out data))
                    {
                        data = lookup;
                        houses.Add(data.mID, data);
                    }

                    doppleHouses.Remove(lookup.mID);

                    data.mSims.Add(sim);
                }

                log += Common.NewLine + "B";

                foreach (SimDescription sim in fixUp)
                {
                    log += Common.NewLine + sim.FullName;

                    new Relationships.RepairParents().Perform(sim, UnusedLog, allSims);
                    new Relationships.RepairChildren().Perform(sim, UnusedLog, allSims);
                    new Relationships.RepairSiblings().Perform(sim, UnusedLog, allSims);
                }

                log += Common.NewLine + "C";

                foreach (SimDescription cleanup in cleanUp)
                {
                    log += Common.NewLine + cleanup.FullName;

                    try
                    {
                        if (cleanup.Household != null)
                        {
                            cleanup.Household.Remove(cleanup);
                        }

                        checkSims.Remove(cleanup);

                        cleanup.Genealogy.ClearAllGenealogyInformation();
                        cleanup.Dispose();
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(log, e);
                    }
                }

                log += Common.NewLine + "D";

                if (houses.Count == 0)
                {
                    SimpleMessageDialog.Show(Common.Localize("Title"), Common.Localize("Unpack:NotProper"));
                    return OptionResult.Failure;
                }
                else if (houses.Count == 1)
                {
                    int funds = -1;
                    foreach (HouseData data in houses.Values)
                    {
                        if (data.mID != 0)
                        {
                            funds = data.mFunds;
                        }
                    }

                    if (funds >= 0)
                    {
                        me.SetFamilyFunds(funds);

                        SimpleMessageDialog.Show(Common.Localize("Title"), Common.Localize("Unpack:Unneeded"));
                        return OptionResult.Failure;
                    }
                }

                log += Common.NewLine + "E";

                int unpacked = 0, failed = 0;

                List<HouseData> sorted = new List<HouseData>(houses.Values);
                sorted.Sort(new Comparison<HouseData>(HouseData.SortByCost));

                foreach (HouseData data in sorted)
                {
                    log += Common.NewLine + "House: " + data.mID;

                    if (data.mID != 1)
                    {
                        Household house = null;

                        if (data.mID != 0)
                        {
                            Lot lot = FindLot(data.mFunds, data.mSims);
                            if (lot == null)
                            {
                                lot = FindLot(-1, null);
                                if (lot == null)
                                {
                                    failed++;
                                    continue;
                                }
                            }

                            house = Household.Create();

                            importedHouses.Add(house);

                            lot.MoveIn(house);

                            house.Name = data.mName;

                            int finalFunds = (data.mFunds - lot.Cost);
                            if (finalFunds >= 0)
                            {
                                house.SetFamilyFunds(finalFunds);

                                me.ModifyFamilyFunds(-data.mFunds);
                            }
                            else
                            {
                                house.SetFamilyFunds(0);

                                me.ModifyFamilyFunds(-data.mFunds);
                                me.ModifyFamilyFunds(finalFunds);
                            }

                            if (me.FamilyFunds < 0)
                            {
                                me.SetFamilyFunds(0);
                            }

                            if (house.FamilyFunds < 0)
                            {
                                house.SetFamilyFunds(0);
                            }

                            unpacked++;
                        }

                        foreach (SimDescription sim in data.mSims)
                        {
                            log += Common.NewLine + sim.FullName;

                            if (house != null)
                            {
                                log += Common.NewLine + "Moved";

                                me.Remove(sim);
                                house.Add(sim);

                                Instantiation.Perform(sim, null);
                            }
                            else
                            {
                                log += Common.NewLine + "PlaceGrave";

                                Porter.PlaceGraveTask.Perform(sim);
                            }
                        }

                        if (house != null)
                        {
                            string name = house.LotHome.Name;
                            if (string.IsNullOrEmpty(name))
                            {
                                name = house.LotHome.Address;
                            }

                            Porter.Notify(Common.Localize("Unpack:Success", false, new object[] { house.Name, name }), house.LotHome.ObjectId);
                        }
                    }
                    else
                    {
                        unpacked++;
                    }

                    foreach (SimDescription sim in data.mSims)
                    {
                        sim.mBio = null;
                    }
                }

                log += Common.NewLine + "F";

                foreach (SimDescription checkSim in checkSims)
                {
                    log += Common.NewLine + checkSim.FullName;

                    if (checkSim.ChildOrBelow)
                    {
                        bool found = false;

                        Household parentHousehold = null;
                        foreach (SimDescription parent in Relationships.GetParents(checkSim))
                        {
                            if ((checkSim.Household != null) && (parent.Household == checkSim.Household))
                            {
                                found = true;
                                break;
                            }
                            else
                            {
                                parentHousehold = parent.Household;
                            }
                        }

                        if (!found)
                        {
                            if (parentHousehold == null)
                            {
                                if ((checkSim.Household != null) && (checkSim.Household.NumMembers == 1))
                                {
                                    foreach (Household house in Household.sHouseholdList)
                                    {
                                        foreach (SimDescription sim in Households.All(house))
                                        {
                                            if (Relationships.IsCloselyRelated(checkSim, sim, false))
                                            {
                                                parentHousehold = house;
                                                break;
                                            }
                                        }

                                        if (parentHousehold != null) break;
                                    }
                                }
                            }

                            if (parentHousehold != null)
                            {
                                if (checkSim.Household != null)
                                {
                                    checkSim.Household.Remove(checkSim);
                                }

                                parentHousehold.Add(checkSim);

                                Instantiation.AttemptToPutInSafeLocation(checkSim.CreatedSim, false);
                            }
                        }
                    }
                }

                log += Common.NewLine + "G";

                foreach (Household house in importedHouses)
                {
                    foreach (Sim sim in Households.AllSims(house))
                    {
                        foreach (GameObject obj in Inventories.QuickFind<GameObject>(sim.Inventory))
                        {
                            bool moveToFamily = false;

                            if (!sim.Inventory.ValidForThisInventory(obj))
                            {
                                moveToFamily = true;
                            }
                            else if (obj is IStageProp)
                            {
                                moveToFamily = true;
                            }

                            if (moveToFamily)
                            {
                                sim.Inventory.RemoveByForce(obj);

                                Inventories.TryToMove(obj, house.SharedFamilyInventory.Inventory);
                            }
                        }
                    }
                }

                log += Common.NewLine + "H";

                int doppleFunds = 0;
                foreach (HouseData data in doppleHouses.Values)
                {
                    doppleFunds += data.mFunds;
                }

                me.ModifyFamilyFunds(-doppleFunds);

                if (me.FamilyFunds < 0)
                {
                    me.SetFamilyFunds(0);
                }

                SimpleMessageDialog.Show(Common.Localize("Title"), Common.Localize("Unpack:Completion", false, new object[] { unpacked, failed }));
            }
            catch (Exception e)
            {
                Common.Exception(log, e);
            }

            return OptionResult.SuccessClose;
        }

        public Lot FindLot(int funds, List<SimDescription> sims)
        {
            List<Lot> choices = new List<Lot>();
            foreach (Lot lot in LotManager.AllLots)
            {
                if (lot.Household != null) continue;

                if (lot.IsWorldLot) continue;

                if (!lot.IsResidentialLot) continue;

                Lot.LotMetrics metrics = new Lot.LotMetrics();
                lot.GetLotMetrics(ref metrics);

                if (metrics.FridgeCount == 0) continue;

                if ((funds > 0) && (lot.Cost > funds)) continue;

                if (sims != null)
                {
                    if (!Satisfies(lot, sims)) continue;
                }

                choices.Add (lot);
            }

            if (choices.Count == 0) return null;

            choices.Sort(new Comparison<Lot>(SortByCost));

            return choices[0];
        }

        private static int SortByCost(Lot a, Lot b)
        {
            if (a.Cost > b.Cost)
            {
                return 1;
            }
            if (a.Cost < b.Cost)
            {
                return -1;
            }
            return 0;
        }

        public class ValidateTask : Common.FunctionTask
        {
            SimDescription mSim;

            protected ValidateTask(SimDescription sim)
            {
                mSim = sim;
            }

            public static void Perform(SimDescription sim)
            {
                new ValidateTask(sim).AddToSimulator();
            }

            protected override void OnPerform()
            {
                OccultTypeHelper.ValidateOccult(mSim, null);

                OccultTypeHelper.TestAndRebuildWerewolfOutfit(mSim);
            }
        }

        public class ImportSelection : ProtoSelection<IExportBinContents>
        {
            public ImportSelection(ICollection<IExportBinContents> contents)
                : base(Common.Localize("Household:Title"), null, contents)
            {
                AddColumn(new NameColumn());
                AddColumn(new CountColumn());
            }

            protected override bool AllowRow(IExportBinContents item)
            {
                return (!string.IsNullOrEmpty(item.HouseholdName));
            }

            public class NameColumn : ObjectPickerDialogEx.CommonHeaderInfo<IExportBinContents>
            {
                public NameColumn()
                    : base("NRaas.Porter.Household:ListTitle", "NRaas.Porter.Household:ListTooltip", 370)
                { }

                public override ObjectPicker.ColumnInfo GetValue(IExportBinContents item)
                {
                    string name = item.HouseholdName;

                    name = name.Replace("NRaas.Porter:", "NRaas: ");

                    return new ObjectPicker.TextColumn(name);
                }
            }

            public class CountColumn : ObjectPickerDialogEx.CommonHeaderInfo<IExportBinContents>
            {
                public CountColumn()
                    : base("NRaas.Porter.OptionList:CountTitle", "NRaas.Porter.OptionList:CountTooltip", 20)
                { }

                public override ObjectPicker.ColumnInfo GetValue(IExportBinContents item)
                {
                    return new ObjectPicker.TextColumn(EAText.GetNumberString(item.HouseholdSims.Count));
                }
            }
        }

        protected static Household GetImportSelection(Lot lot)
        {
            BinModel.Singleton.PopulateExportBin();

            ExportBinContents contents = new ImportSelection(BinModel.Singleton.ExportBinContents).SelectSingle() as ExportBinContents;
            if (contents == null) return null;

            List<Household> houses = new List<Household>();

            Household household = null;
            ProgressDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/Global:Processing", new object[0x0]), false);

            Dictionary<string, List<News.NewsTuning.ArticleTuning>> namedArticles = News.sNewsTuning.mNamedArticles;

            try
            {
                // Doing so stops Marriage notices of imported sims from appearing in the newspaper
                News.sNewsTuning.mNamedArticles = new Dictionary<string, List<News.NewsTuning.ArticleTuning>>();

                HouseholdContentsProxy houseContents = HouseholdContentsProxy.Import(contents.PackageName);

                household = houseContents.Household;
                if (household == null) return null;

                List<ulong> indexMap = ExportBinContentsEx.CreateIndexMap(household);

                lot.MoveIn(household);

                CreateActors(lot);

                BinCommonEx.CreateInventories(household, houseContents.Contents, indexMap);

                BinCommon.UpdateImportedUrnstones(household, lot);

                household.FixupGenealogy();
            }
            finally
            {
                News.sNewsTuning.mNamedArticles = namedArticles;

                try
                {
                    ProgressDialog.Close();
                }
                catch
                { }
            }

            SpeedTrap.Sleep();

            return household;
        }

        protected static void CreateActors(Lot lot)
        {
            List<Sim> sims = new List<Sim>();
            foreach (SimDescription description in Households.All(lot.Household))
            {
                try
                {
                    description.GetMiniSimForProtection().AddProtection(MiniSimDescription.ProtectionFlag.PartialFromPlayer);

                    description.HomeWorld = GameUtils.GetCurrentWorld();
                    if (description.CreatedSim == null)
                    {
                        if (description.Weight < 0f)
                        {
                            description.ChangeBodyShape(0f, description.Fitness, -description.Weight);
                        }
                        else
                        {
                            description.ChangeBodyShape(description.Weight, description.Fitness, 0f);
                        }

                        FixInvisibleTask.Perform(description, false);

                        Sim item = Instantiation.Perform(description, null);
                        if (item != null)
                        {
                            sims.Add(item);
                        }
                    }
                    else
                    {
                        sims.Add(description.CreatedSim);
                    }
                }
                catch(Exception e)
                {
                    Common.Exception(description, e);
                }
            }

            /*
            if (lot != null)
            {
                BinCommon.PlaceSims(sims, lot);
            }*/
        }
    }
}
