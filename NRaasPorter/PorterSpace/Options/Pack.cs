using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
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
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.PorterSpace.Options
{
    public class Pack : OperationSettingOption<GameObject>, ILotOption
    {
        public override string GetTitlePrefix()
        {
            return "Pack";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            Lot lot = Porter.GetLot(parameters.mTarget);
            if (lot == null) return false;

            if (lot.Household == null) return false;

            return base.Allow(parameters);
        }

        protected static bool IsNeeded(KeyValuePair<SimDescription, List<SimDescription>> item, Dictionary<SimDescription, List<SimDescription>> sims)
        {
            if (item.Value.Count <= 1) return false;

            if (item.Key.Genealogy == null) return false;

            foreach (Genealogy child in item.Key.Genealogy.Children)
            {
                if (child.SimDescription == null) continue;

                List<SimDescription> refSims;
                if (sims.TryGetValue(child.SimDescription, out refSims))
                {
                    bool bMissing = false;

                    foreach (SimDescription refSim in item.Value)
                    {
                        if (!refSims.Contains(refSim))
                        {
                            bMissing = true;
                            break;
                        }
                    }

                    if (!bMissing) return false;
                }
            }

            return true;
        }

        protected List<SimDescription> GetSimSelection(List<Household> houses)
        {
            Dictionary<SimDescription, bool> finalSet = new Dictionary<SimDescription, bool>();

            List<SimDescription> originals = new List<SimDescription>();

            {
                List<SimDescription> sims = new List<SimDescription>();
                foreach (Household house in houses)
                {
                    if (house.IsServiceNpcHousehold)
                    {
                        List<SimDescription> all = SimDescription.GetHomelessSimDescriptionsFromUrnstones();

                        sims.AddRange(all);

                        originals.AddRange(all);
                    }
                    else
                    {
                        sims.AddRange(house.AllSimDescriptions);

                        originals.AddRange(house.AllSimDescriptions);
                    }

                    if (house.LotHome != null)
                    {
                        List<Urnstone> stones = new List<Urnstone>(house.LotHome.GetObjects<Urnstone>());

                        foreach (Urnstone stone in stones)
                        {
                            if (stone.DeadSimsDescription == null) continue;

                            sims.Add(stone.DeadSimsDescription);

                            originals.Add(stone.DeadSimsDescription);
                        }
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    finalSet.Clear();

                    int index = 0;
                    while (index < sims.Count)
                    {
                        SimDescription sim = sims[index];
                        index++;

                        if (sim == null) continue;

                        if (finalSet.ContainsKey(sim)) continue;
                        finalSet.Add(sim, true);

                        if (sim.Partner != null)
                        {
                            sims.Add(sim.Partner);
                        }

                        if (sim.IsPregnant)
                        {
                            SimDescription father = SimDescription.Find(sim.Pregnancy.DadDescriptionId);
                            if (father != null)
                            {
                                sims.Add(father);
                            }
                        }

                        if ((sim.TeenOrBelow) && (i == 1) && (sim.Genealogy != null))
                        {
                            foreach (Genealogy parent in sim.Genealogy.Parents)
                            {
                                if (parent.SimDescription == null) continue;

                                sims.Add(parent.SimDescription);
                            }
                        }
                    }

                    if (houses.Count > 1) break;

                    sims = new List<SimDescription>(finalSet.Keys);

                    foreach (SimDescription sim in finalSet.Keys)
                    {
                        if (sim.Genealogy == null) continue;

                        foreach (Genealogy parent in sim.Genealogy.Parents)
                        {
                            if (parent.SimDescription == null) continue;

                            sims.Add(parent.SimDescription);
                        }

                        foreach (Genealogy child in sim.Genealogy.Children)
                        {
                            if (child.SimDescription == null) continue;

                            sims.Add(child.SimDescription);
                        }

                        foreach (Genealogy sibling in sim.Genealogy.Siblings)
                        {
                            if (sibling.SimDescription == null) continue;

                            sims.Add(sibling.SimDescription);
                        }
                    }
                }
            }

            Dictionary<SimDescription, List<SimDescription>> fullSet = new Dictionary<SimDescription, List<SimDescription>>();

            foreach (SimDescription sim in finalSet.Keys)
            {
                GetParents(sim, fullSet);
            }

            finalSet.Clear();
            foreach (KeyValuePair<SimDescription, List<SimDescription>> item in fullSet)
            {
                if ((!originals.Contains(item.Key)) && (!houses.Contains(item.Key.Household)))
                {
                    if (!IsNeeded(item, fullSet)) continue;
                }

                GetChildren(item.Key, fullSet, finalSet);
            }

            PackSelection.Results selection = new PackSelection(new List<SimDescription>(finalSet.Keys)).SelectMultiple(0);
            if (!selection.mOkayed) return null;

            return new List<SimDescription>(selection);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Lot lot = Porter.GetLot(parameters.mTarget);
            if (lot == null) return OptionResult.Failure;

            List<SimDescription> selection = null;

            List<HouseholdItem> allHouses = new List<HouseholdItem>();

            foreach (Household house in Household.sHouseholdList)
            {
                allHouses.Add(new HouseholdItem(house, house == lot.Household));
            }

            string houseName = null;

            while (selection == null)
            {
                List<Household> houses = HouseholdSelection.Perform(lot.Household.Name, allHouses);
                if ((houses == null) || (houses.Count == 0)) return OptionResult.Failure;

                houseName = houses[0].Name;

                selection = GetSimSelection(houses);
            }

            if ((selection == null) || (selection.Count == 0)) return OptionResult.Failure;

            Dictionary<Household, int> finalHouses = new Dictionary<Household, int>();

            int nextID = 1;

            foreach (SimDescription sim in selection)
            {
                if (sim.Household == null) continue;

                if (sim.Household.IsServiceNpcHousehold) continue;

                if (!finalHouses.ContainsKey(sim.Household))
                {
                    finalHouses.Add(sim.Household, nextID);
                    nextID++;
                }
            }

            string name = StringInputDialog.Show(Common.Localize("Title"), Common.Localize("Pack:NamePrompt", false, new object[] { finalHouses.Count, selection.Count }), houseName);
            if (string.IsNullOrEmpty(name)) return OptionResult.Failure;

            Household export = Household.Create();

            SpeedTrap.Sleep();

            foreach (Household house in finalHouses.Keys)
            {
                if (house.LotHome != null)
                {
                    export.SetFamilyFunds (export.FamilyFunds + house.FamilyFunds + house.LotHome.Cost);
                }
                else
                {
                    export.SetFamilyFunds (export.FamilyFunds + house.NetWorth());
                }
            }

            Dictionary<SimDescription, Household> saveHouses = new Dictionary<SimDescription, Household>();

            Dictionary<Sim, bool> resetDNP = new Dictionary<Sim, bool>();

            Dictionary<Household, bool> inventoried = new Dictionary<Household, bool>();

            foreach (SimDescription sim in selection)
            {
                if (sim.CreatedSim != null)
                {
                    sim.CreatedSim.SetReservedVehicle(null);

                    if (sim.CreatedSim.DreamsAndPromisesManager != null)
                    {
                        sim.CreatedSim.NullDnPManager();

                        if (!resetDNP.ContainsKey(sim.CreatedSim))
                        {
                            resetDNP.Add(sim.CreatedSim, true);
                        }
                    }

                    if ((sim.Household != null) && (!inventoried.ContainsKey(sim.Household)))
                    {
                        inventoried.Add(sim.Household, true);

                        if ((sim.Household.SharedFamilyInventory != null) &&
                            (sim.Household.SharedFamilyInventory.Inventory != null))
                        {
                            foreach (GameObject obj in Inventories.QuickFind<GameObject>(sim.Household.SharedFamilyInventory.Inventory))
                            {
                                if (Inventories.TryToMove(obj, sim.CreatedSim.Inventory, false)) continue;

                                Inventories.TryToMove(obj.Clone(), export.SharedFamilyInventory.Inventory);
                            }
                        }

                        if ((sim.Household.SharedFridgeInventory != null) &&
                            (sim.Household.SharedFridgeInventory.Inventory != null))
                        {
                            foreach (GameObject obj in Inventories.QuickFind<GameObject>(sim.Household.SharedFridgeInventory.Inventory))
                            {
                                if (Inventories.TryToMove(obj, sim.CreatedSim.Inventory, false)) continue;

                                Inventories.TryToMove(obj.Clone(), export.SharedFridgeInventory.Inventory);
                            }
                        }
                    }
                }

                int id = 0;
                if ((sim.Household != null) && (finalHouses.ContainsKey(sim.Household)))
                {
                    id = finalHouses[sim.Household];
                }
                else
                {
                    Urnstone grave = Urnstones.CreateGrave(sim, false);
                    if (grave == null) continue;

                    SpeedTrap.Sleep();

                    bool success = false;
                    try
                    {
                        success = Urnstones.GhostToPlayableGhost(grave, Household.NpcHousehold, lot.EntryPoint());
                    }
                    catch (Exception exception)
                    {
                        Common.DebugException(grave.DeadSimsDescription, exception);
                    }

                    if (!success)
                    {
                        Porter.Notify(Common.Localize("Pack:SimFailure", sim.IsFemale, new object[] { sim }));

                        Porter.PlaceGraveTask.Perform(sim);
                        //export.SharedFamilyInventory.Inventory.TryToMove(grave);
                        continue;
                    }
                }

                HouseData data = new HouseData(id, sim);

                sim.mBio = data.ToString();

                saveHouses.Add(sim, sim.Household);

                sim.OnHouseholdChanged(export, false);

                export.mMembers.Add(sim, null);

                Porter.AddExport(sim);
            }

            string packageName = null;

            try
            {
                try
                {
                    ProgressDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/Global:Processing", new object[0x0]), false);

                    if (export.mMembers.Count > 0)
                    {
                        export.Name = "NRaas.Porter:" + name;

                        ThumbnailHelper.CacheSimAtlasesForHousehold(export);
                        ThumbnailManager.GenerateHouseholdThumbnail(export.HouseholdId, export.HouseholdId, ThumbnailSizeMask.Large);

                        packageName = BinEx.ExportHousehold(Bin.Singleton, export, false);
                        if (packageName != null)
                        {
                            BinModel.Singleton.AddToExportBin(packageName);
                        }
                    }

                    foreach (Sim sim in resetDNP.Keys)
                    {
                        try
                        {
                            sim.ResetDnP();
                        }
                        catch(Exception e)
                        {
                            Common.DebugException(sim, e);
                        }
                    }

                    List<Urnstone> graves = Inventories.QuickFind<Urnstone>(export.SharedFamilyInventory.Inventory);

                    foreach (Urnstone grave in graves)
                    {
                        Porter.PlaceGraveTask.Perform(grave.DeadSimsDescription);
                    }

                    while (export.mMembers.Count > 0)
                    {
                        SimDescription sim = export.mMembers.SimDescriptionList[0];
                        if (sim != null)
                        {
                            sim.OnHouseholdChanged(saveHouses[sim], false);

                            if ((sim.Household == null) || (sim.Household.IsServiceNpcHousehold))
                            {
                                Porter.PlaceGraveTask.Perform(sim);
                            }
                        }

                        export.mMembers.RemoveAt(0);
                    }

                    export.Destroy();
                    export.Dispose();
                }
                finally
                {
                    ProgressDialog.Close();
                }
            }
            catch(ExecutionEngineException)
            { 
                // Ignored
            }
            catch (Exception e)
            {
                Common.Exception(name, e);
                packageName = null;
            }

            if (packageName != null)
            {
                SimpleMessageDialog.Show(Common.Localize("Title"), Common.Localize("Pack:Success", false, new object[] { export.Name }));
            }
            else
            {
                SimpleMessageDialog.Show(Common.Localize("Title"), Common.Localize("Pack:Failure"));
            }

            return OptionResult.SuccessClose;
        }

        protected static void GetChildren(SimDescription sim, Dictionary<SimDescription, List<SimDescription>> lookup, Dictionary<SimDescription, bool> results)
        {
            List<SimDescription> sims = new List<SimDescription>();
            sims.Add(sim);

            if (!results.ContainsKey(sim))
            {
                results.Add(sim, true);
            }

            int index = 0;
            while (index < sims.Count)
            {
                SimDescription child = sims[index];
                index++;

                if (child.Genealogy == null) continue;

                foreach (Genealogy gc in child.Genealogy.Children)
                {
                    if (gc.SimDescription == null) continue;

                    if (!lookup.ContainsKey(gc.SimDescription)) continue;

                    if (!results.ContainsKey(gc.SimDescription))
                    {
                        results.Add(gc.SimDescription, true);
                    }

                    sims.Add(gc.SimDescription);
                }
            }
        }

        protected static void GetParents(SimDescription sim, Dictionary<SimDescription, List<SimDescription>> lookup)
        {
            List<SimDescription> sims = new List<SimDescription>();
            sims.Add(sim);

            int index = 0;
            while (index < sims.Count)
            {
                SimDescription parent = sims[index];
                index++;

                List<SimDescription> refSims;
                if (!lookup.TryGetValue(parent, out refSims))
                {
                    refSims = new List<SimDescription>();
                    lookup.Add(parent, refSims);
                }

                if (!refSims.Contains(sim))
                {
                    if (sim != parent)
                    {
                        refSims.Add(sim);
                    }

                    if (parent.Genealogy != null)
                    {
                        foreach (Genealogy gp in parent.Genealogy.Parents)
                        {
                            if (gp.SimDescription == null) continue;

                            sims.Add(gp.SimDescription);
                        }
                    }
                }
            }
        }
    }
}
