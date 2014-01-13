using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.VectorSpace.Helpers
{
    public class PurchaseControl : Common.IWorldLoadFinished, Common.IWorldQuit
    {
        static Dictionary<Household, bool> sChecks = new Dictionary<Household, bool>();

        public void OnWorldLoadFinished()
        {
            new Common.AlarmTask(3, TimeUnit.Hours, OnPerform, 3, TimeUnit.Hours);
        }

        public void OnWorldQuit()
        {
            sChecks.Clear();
        }

        public static void AddCheck(Household house)
        {
            if (!Vector.Settings.mAllowInactivePurchases) return;

            if (SimTypes.IsSpecial(house)) return;

            if (sChecks.ContainsKey(house)) return;

            sChecks.Add(house, true);
        }

        protected static void GetInoculates(SimDescription sim, List<DiseaseVector> potentials, bool showingSigns)
        {
            foreach (DiseaseVector vector in Vector.Settings.GetVectors(sim))
            {
                if (!vector.CanInoculate) continue;
                
                if (sim.FamilyFunds < vector.InoculationCost) continue;

                if (vector.IsInoculationUpToDate) continue;

                if (showingSigns)
                {
                    if (!vector.HadFirstSign) continue;
                }

                potentials.Add(vector);
            }
        }

        protected static void GetResisters(SimDescription sim, List<DiseaseVector> potentials)
        {
            foreach (DiseaseVector vector in Vector.Settings.GetVectors(sim))
            {
                if (!vector.CanBoostResistance) continue;

                if (!vector.NeedsResistance) continue;

                if (sim.FamilyFunds < vector.ResistanceCost) continue;

                potentials.Add(vector);
            }
        }

        protected static void OnPerform()
        {
            foreach (Household house in sChecks.Keys)
            {
                Task.Perform(house);
            }

            sChecks.Clear();
        }

        public class Task : Common.FunctionTask
        {
            Household mHouse;

            protected Task(Household house)
            {
                mHouse = house;
            }

            public static void Perform(Household house)
            {
                new Task(house).AddToSimulator();
            }

            protected override void OnPerform()
            {
                if (mHouse == Household.ActiveHousehold) return;

                List<SimDescription> babies = new List<SimDescription>();
                List<SimDescription> teens = new List<SimDescription>();
                List<SimDescription> adults = new List<SimDescription>();
                List<SimDescription> teensAndAdults = new List<SimDescription>();

                foreach(SimDescription sim in Households.All(mHouse))
                {
                    if ((sim.IsPet) || (sim.ToddlerOrBelow))
                    {
                        babies.Add(sim);
                    }
                    else if (sim.YoungAdultOrAbove)
                    {
                        adults.Add(sim);
                        teensAndAdults.Add(sim);
                    }
                    else if (sim.Teen)
                    {
                        teens.Add(sim);
                        teensAndAdults.Add(sim);
                    }
                }

                if (adults.Count == 0)
                {
                    adults = teens;
                }

                bool purchaseInoculation = false;

                foreach(SimDescription sim in adults)
                {                
                    if (ScoringLookup.GetScore("NRaasVectorPurchaseInoculation", sim) > 0)
                    {
                        purchaseInoculation = true;
                        break;
                    }
                }

                if (purchaseInoculation)
                {
                    List<DiseaseVector> potentials = new List<DiseaseVector>();

                    foreach(SimDescription sim in babies)
                    {
                        GetInoculates(sim, potentials, false);
                    }

                    while (potentials.Count > 0)
                    {
                        DiseaseVector potential = RandomUtil.GetRandomObjectFromList(potentials);
                        potentials.Remove(potential);

                        DiseaseVector inoculate = null;

                        List<SimDescription> potentialDonors = new List<SimDescription>();

                        foreach(SimDescription sim in teensAndAdults)
                        {
                            DiseaseVector vector = Vector.Settings.GetVector(sim, potential.Guid);
                            if (vector == null) continue;

                            if (!vector.IsInoculationUpToDate)
                            {
                                potentialDonors.Add(sim);
                            }

                            if (!vector.IsInoculated) continue;

                            inoculate = vector;
                            break;
                        }

                        if (inoculate == null)
                        {
                            if (mHouse.FamilyFunds < potential.InoculationCost) continue;

                            inoculate = new DiseaseVector(potential.Data, Vector.Settings.GetCurrentStrain(potential.Data));
                            inoculate.Inoculate(potential.Data.InoculationStrain, true);

                            if (potentialDonors.Count > 0)
                            {
                                SimDescription donor = RandomUtil.GetRandomObjectFromList(potentialDonors);
                                VectorControl.Inoculate(donor, inoculate, true, false);
                            }

                            mHouse.ModifyFamilyFunds(-potential.InoculationCost);

                            Common.DebugNotify("Donor Inoculate: " + mHouse.Name + Common.NewLine + "Cost: " + potential.InoculationCost + Common.NewLine + potential.GetUnlocalizedDescription());
                        }
                        else
                        {
                            Common.DebugNotify("Existing Inoculate: " + mHouse.Name + Common.NewLine + "Cost: " + potential.InoculationCost + Common.NewLine + potential.GetUnlocalizedDescription());
                        }

                        if (inoculate != null)
                        {
                            foreach (SimDescription child in babies)
                            {
                                VectorControl.Inoculate(child, inoculate, false, false);
                            }

                            return;
                        }
                    }

                    potentials.Clear();

                    foreach (SimDescription sim in Households.All(mHouse))
                    {
                        // Babies were handled earlier
                        if (sim.ToddlerOrBelow) continue;

                        GetInoculates(sim, potentials, true);
                    }

                    while (potentials.Count > 0)
                    {
                        DiseaseVector potential = RandomUtil.GetRandomObjectFromList(potentials);
                        potentials.Remove(potential);

                        if (mHouse.FamilyFunds < potential.InoculationCost) continue;

                        potential.Inoculate(potential.Data.InoculationStrain, true);

                        mHouse.ModifyFamilyFunds(-potential.InoculationCost);

                        if (Common.kDebugging)
                        {
                            Common.DebugNotify("Inoculate: " + mHouse.Name + Common.NewLine + "Cost: " + potential.InoculationCost + Common.NewLine + potential.GetUnlocalizedDescription());
                        }

                        return;
                    }
                }

                List<SimDescription> choices = new List<SimDescription>(adults);

                while (choices.Count > 0)
                {
                    SimDescription choice = RandomUtil.GetRandomObjectFromList(choices);
                    choices.Remove(choice);

                    if (ScoringLookup.GetScore("NRaasVectorPurchaseResistance", choice) > 0)
                    {
                        List<DiseaseVector> potentials = new List<DiseaseVector>();

                        GetResisters(choice, potentials);

                        if (potentials.Count > 0)
                        {
                            DiseaseVector potential = RandomUtil.GetRandomObjectFromList(potentials);

                            mHouse.ModifyFamilyFunds(-potential.ResistanceCost);

                            potential.AlterResistance(Vector.Settings.mResistanceBoost);

                            potential.SetToIdentified();

                            if (Common.kDebugging)
                            {
                                Common.DebugNotify("Boost: " + mHouse.Name + Common.NewLine + "Cost: " + potential.ResistanceCost + Common.NewLine + potential.GetUnlocalizedDescription());
                            }
                        }
                    }

                    /*
                    if (ScoringLookup.GetScore("NRaasVectorPurchaseProtection", choice) > 0)
                    {
                        purchaseProtection = true;
                    }
                    */
                }
            }
        }
    }
}
