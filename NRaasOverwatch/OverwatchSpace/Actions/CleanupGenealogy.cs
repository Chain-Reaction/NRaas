using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using NRaas.OverwatchSpace.Settings;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Actions
{
    public class CleanupGenealogy : BooleanOption, Common.IDelayedWorldLoadFinished, IActionOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanupGenealogy";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanupGenealogy;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanupGenealogy = value;
            }
        }

        public void OnDelayedWorldLoadFinished()
        {
            PerformAction(false);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            PerformAction(true);
            return OptionResult.SuccessClose;
        }

        protected static bool Remove(List<Genealogy> ths, Genealogy me)
        {
            return (ths.RemoveAll((e) => { return object.ReferenceEquals(e, me); }) > 0);
        }

        protected static bool Contains(List<Genealogy> ths, Genealogy me)
        {
            return (ths.Find((e) => { return object.ReferenceEquals(e, me); }) != null);
        }

        protected static void SetGenealogy(IMiniSimDescription pureSim, Genealogy genealogy)
        {
            if (genealogy == null) return;

            SimDescription sim = pureSim as SimDescription;
            if (sim != null)
            {
                if ((sim.mGenealogy == null) || 
                    (!object.ReferenceEquals(sim.mGenealogy, genealogy)))
                {
                    sim.mGenealogy = genealogy;

                    Overwatch.Log("Replaced SimDesc Genealogy: " + Relationships.GetFullName(pureSim));
                }

                sim.mGenealogy.mSim = sim;
                sim.mGenealogy.mMiniSim = MiniSimDescription.Find(sim.SimDescriptionId);
            }
            else
            {
                MiniSimDescription miniSim = pureSim as MiniSimDescription;
                if (miniSim != null)
                {
                    if ((miniSim.mGenealogy == null) || 
                        (!object.ReferenceEquals(miniSim.mGenealogy, genealogy)))
                    {
                        miniSim.mGenealogy = genealogy;

                        Overwatch.Log("Replaced MiniSim Genealogy: " + Relationships.GetFullName(pureSim));
                    }

                    if (genealogy.mSim == null)
                    {
                        genealogy.mSim = miniSim.mGenealogy.mSim;
                    }
                    genealogy.mMiniSim = miniSim;
                }
            }
        }

        protected static void RepairMissingPartner(SimDescription trueSim)
        {
            if ((trueSim.Partner != null) && (trueSim.Partner.Partner != trueSim))
            {
                trueSim.Partner.mPartner = trueSim;

                Overwatch.Log("Missing Partner Relinked");
                Overwatch.Log("  " + Relationships.GetFullName(trueSim) + " - " + Relationships.GetFullName(trueSim.Partner));
            }

            MiniSimDescription miniSim = MiniSimDescription.Find(trueSim.SimDescriptionId);
            if (miniSim != null)
            {
                if (trueSim.Partner == null)
                {
                    miniSim.PartnerSimDescriptionId = 0;
                }
                else
                {
                    miniSim.PartnerSimDescriptionId = trueSim.Partner.SimDescriptionId;
                }
            }
        }

        protected static void RepairSpouse(IMiniSimDescription simA, Dictionary<ulong, IMiniSimDescription> lookup)
        {
            SimDescription trueSimA = simA as SimDescription;

            IMiniSimDescription simB = null;

            Genealogy genealogy = simA.CASGenealogy as Genealogy;

            if (genealogy.Spouse != null)
            {
                simB = Relationships.GetSim(genealogy.Spouse, lookup);
            }

            if (trueSimA != null)
            {
                if ((simB == null) && (simA.IsMarried))
                {
                    simB = trueSimA.Partner;
                }

                if (simB == null)
                {
                    List<Relationship> relations = new List<Relationship>(Relationship.GetRelationships(trueSimA));
                    foreach (Relationship relation in relations)
                    {
                        if (relation.LTR.CurrentLTR == LongTermRelationshipTypes.Spouse)
                        {
                            simB = relation.GetOtherSimDescription(trueSimA);
                            if (simB != null)
                            {
                                simB = Relationships.Find(simB.SimDescriptionId, lookup);
                            }

                            if (simB is SimDescription)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            SimDescription trueSimB = simB as SimDescription;

            if ((simA != null) && (simB != null))
            {
                Genealogy geneA = simA.CASGenealogy as Genealogy;
                Genealogy geneB = simB.CASGenealogy as Genealogy;

                if ((geneA != null) && (geneB != null))
                {
                    if (!object.ReferenceEquals(geneA.Spouse, geneB))
                    {
                        geneA.mSpouse = geneB;

                        Overwatch.Log("Missing Spouse Link Attached A");
                        Overwatch.Log("  " + Relationships.GetFullName(geneA) + " - " + Relationships.GetFullName(geneB));
                    }

                    if (!object.ReferenceEquals(geneB.Spouse, geneA))
                    {
                        geneB.mSpouse = geneA;

                        Overwatch.Log("Missing Spouse Link Attached B");
                        Overwatch.Log("  " + Relationships.GetFullName(geneB) + " - " + Relationships.GetFullName(geneA));
                    }
                }
            }

            if ((trueSimA != null) && (trueSimB != null))
            {
                if (!object.ReferenceEquals(trueSimA.Partner, trueSimB))
                {
                    // Don't call Partner, it requires genealogy
                    trueSimA.mPartner = trueSimB;

                    Overwatch.Log("Missing Partner Link Attached A");
                    Overwatch.Log("  " + Relationships.GetFullName(simA) + " - " + Relationships.GetFullName(simB));
                }

                if (!object.ReferenceEquals(trueSimB.Partner, trueSimA))
                {
                    // Don't call Partner, it requires genealogy
                    trueSimB.mPartner = trueSimA;

                    Overwatch.Log("Missing Partner Link Attached B");
                    Overwatch.Log("  " + Relationships.GetFullName(simB) + " - " + Relationships.GetFullName(simA));
                }
            }
        }

        public static void CustomAnnihilation(SimDescription badSim, SimDescription goodSim)
        {
            if ((badSim == null) || (goodSim == null)) return;

            if (object.ReferenceEquals(badSim, goodSim)) return;

            Overwatch.Log("Annihilation: " + Relationships.GetFullName(badSim));

            if (object.ReferenceEquals(goodSim.mMaternityOutfits, badSim.mMaternityOutfits))
            {
                badSim.mMaternityOutfits = new OutfitCategoryMap();
                Overwatch.Log("Doppleganger mMaternityOutfits copy " + Relationships.GetFullName(badSim));
            }

            if (object.ReferenceEquals(goodSim.mOutfits, badSim.mOutfits))
            {
                badSim.mOutfits = new OutfitCategoryMap();
                Overwatch.Log("Doppleganger mOutfits copy " + Relationships.GetFullName(badSim));
            }

            if (object.ReferenceEquals(goodSim.CelebrityManager, badSim.CelebrityManager))
            {
                badSim.CelebrityManager = null;
                Overwatch.Log("Doppleganger CelebrityManager copy " + Relationships.GetFullName(badSim));
            }

            if (object.ReferenceEquals(goodSim.CareerManager, badSim.CareerManager))
            {
                badSim.CareerManager = null;
                Overwatch.Log("Doppleganger CareerManager copy " + Relationships.GetFullName(badSim));
            }

            /*
            if ((badSim.Household != null) && (object.ReferenceEquals(goodSim.mHousehold, badSim.mHousehold)))
            {
                badSim.mHousehold = null;
                Overwatch.Log("Doppleganger mHousehold copy " + GetFullName(badSim));
            }
            */

            if ((badSim.mPartner != null) && (object.ReferenceEquals(goodSim.mPartner, badSim.mPartner)))
            {
                badSim.mPartner = null;
                Overwatch.Log("Doppleganger mPartner copy " + Relationships.GetFullName(badSim));
            }

            if (object.ReferenceEquals(goodSim.mGenealogy, badSim.mGenealogy))
            {
                badSim.mGenealogy = new Genealogy(badSim);
                Overwatch.Log("Doppleganger mGenealogy copy " + Relationships.GetFullName(badSim));
            }

            if ((badSim.AssignedRole != null) && (object.ReferenceEquals(goodSim.AssignedRole, badSim.AssignedRole)))
            {
                badSim.AssignedRole = null;
                Overwatch.Log("Doppleganger AssignedRole copy " + Relationships.GetFullName(badSim));
            }

            Annihilation.Perform(badSim, true);
        }

        protected static void AddToList(List<IGenealogy> geneList, Dictionary<IGenealogy, bool> allGenealogies, List<Genealogy> genes)
        {
            if (genes == null) return;

            foreach (Genealogy gene in genes)
            {
                if (allGenealogies.ContainsKey(gene)) continue;

                allGenealogies.Add(gene, true);

                geneList.Add(gene);
            }
        }

        public void PerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Cleanup Genealogy");

                Dictionary<IMiniSimDescription, IMiniSimDescription> duplicates = new Dictionary<IMiniSimDescription, IMiniSimDescription>();

                Dictionary<ulong, IMiniSimDescription> lookup = new Dictionary<ulong, IMiniSimDescription>();

                Dictionary<IMiniSimDescription,bool> urnstones = new Dictionary<IMiniSimDescription,bool>();

                foreach(SimDescription sim in SimDescription.GetHomelessSimDescriptionsFromUrnstones())
                {
                    if (urnstones.ContainsKey(sim)) continue;

                    urnstones.Add(sim, true);
                }

                Dictionary<IGenealogy, bool> allGenealogies = new Dictionary<IGenealogy, bool>();

                List<IGenealogy> geneList = new List<IGenealogy>();

                foreach (KeyValuePair<ulong, List<IMiniSimDescription>> ids in SimListing.AllSims<IMiniSimDescription>(null, true, false))
                {
                    List<IMiniSimDescription> list = ids.Value;

                    foreach (IMiniSimDescription miniSim in list)
                    {
                        if (miniSim.CASGenealogy == null) continue;

                        if (allGenealogies.ContainsKey(miniSim.CASGenealogy)) continue;

                        allGenealogies.Add(miniSim.CASGenealogy, true);

                        geneList.Add(miniSim.CASGenealogy);
                    }

                    IMiniSimDescription choice = null;
                    if (list.Count == 1)
                    {
                        choice = list[0];
                    }
                    else if (list.Count > 1)
                    {
                        List<IMiniSimDescription> choices = new List<IMiniSimDescription>();

                        foreach (IMiniSimDescription miniSim in list)
                        {
                            SimDescription sim = miniSim as SimDescription;
                            if (sim == null) continue;

                            if (sim.Household == Household.ActiveHousehold)
                            {
                                choices.Add(miniSim);
                            }
                        }

                        if (choices.Count >= 1)
                        {
                            choice = choices[0];

                            Overwatch.Log("Doppleganger Active-Inactive (" + list.Count + "): " + Relationships.GetFullName(choice));
                        }
                        else
                        {
                            choices.Clear();
                            foreach (IMiniSimDescription sim in list)
                            {
                                if (urnstones.ContainsKey(sim))
                                {
                                    choices.Add(sim);
                                }
                            }

                            if (choices.Count == 1)
                            {
                                choice = choices[0];

                                Overwatch.Log("Doppleganger Urnstone (" + list.Count + "): " + Relationships.GetFullName(choice));
                            }
                            else
                            {
                                choices.Clear();
                                foreach (IMiniSimDescription sim in list)
                                {
                                    Genealogy genealogy = sim.CASGenealogy as Genealogy;

                                    if ((genealogy != null) && (object.ReferenceEquals(sim, genealogy.mSim)))
                                    {
                                        choices.Add(sim);
                                    }
                                }

                                if (choices.Count == 1)
                                {
                                    choice = choices[0];

                                    Overwatch.Log("Doppleganger Personal Genealogy (" + list.Count + "): " + Relationships.GetFullName(choice));
                                }
                                else
                                {
                                    choices.Clear();

                                    MiniSimDescription miniSim = MiniSimDescription.Find(ids.Key);
                                    if ((miniSim != null) && (miniSim.Genealogy != null) && (miniSim.Genealogy.mSim != null))
                                    {
                                        foreach (IMiniSimDescription sim in list)
                                        {
                                            if (object.ReferenceEquals(sim, miniSim.Genealogy.mSim))
                                            {
                                                choices.Add(sim);
                                            }
                                        }
                                    }

                                    if (choices.Count == 1)
                                    {
                                        choice = choices[0];

                                        Overwatch.Log("Doppleganger MiniSim Genealogy (" + list.Count + "): " + Relationships.GetFullName(choice));
                                    }
                                    else
                                    {
                                        choices.Clear();

                                        int maxLinks = 0;

                                        foreach (IMiniSimDescription sim in list)
                                        {
                                            if (choice == null)
                                            {
                                                choice = sim;
                                            }
                                            else
                                            {
                                                int numLinks = 0;

                                                Genealogy gene = sim.CASGenealogy as Genealogy;
                                                if (gene != null)
                                                {
                                                    if (gene.mNaturalParents != null)
                                                    {
                                                        numLinks += gene.mNaturalParents.Count;
                                                    }

                                                    if (gene.mChildren != null)
                                                    {
                                                        numLinks += gene.mChildren.Count;
                                                    }

                                                    if (gene.mSiblings != null)
                                                    {
                                                        numLinks += gene.mSiblings.Count;
                                                    }

                                                    if (maxLinks < numLinks)
                                                    {
                                                        maxLinks = numLinks;

                                                        choice = sim;
                                                    }
                                                }
                                            }
                                        }

                                        Overwatch.Log("Doppleganger By Link Count (" + list.Count + "): " + Relationships.GetFullName(choice));
                                    }
                                }
                            }
                        }
                    }

                    if (choice != null)
                    {
                        lookup.Add(choice.SimDescriptionId, choice);

                        foreach (IMiniSimDescription sim in list)
                        {
                            if (object.ReferenceEquals(sim, choice)) continue;

                            duplicates.Add(sim, choice);

                            Genealogy genealogy = ReconcileGenealogy(choice.CASGenealogy as Genealogy, sim.CASGenealogy as Genealogy);
                            if (genealogy != null)
                            {
                                SetGenealogy(choice, genealogy);
                            }
                        }
                    }
                }

                int index = 0;
                while (index < geneList.Count)
                {
                    Genealogy genealogy = geneList[index] as Genealogy;
                    index++;

                    if (genealogy == null) continue;

                    AddToList(geneList, allGenealogies, genealogy.mNaturalParents);
                    AddToList(geneList, allGenealogies, genealogy.mChildren);
                    AddToList(geneList, allGenealogies, genealogy.mSiblings);
                }

                Dictionary<string, List<Genealogy>> broken = new Dictionary<string, List<Genealogy>>();

                foreach (IGenealogy iGene in geneList)
                {
                    Genealogy gene = iGene as Genealogy;
                    if (gene == null) continue;

                    if ((gene.mMiniSim == null) && (gene.mSim == null))
                    {
                        if (!string.IsNullOrEmpty(gene.Name))
                        {
                            List<Genealogy> genes;
                            if (!broken.TryGetValue(gene.Name, out genes))
                            {
                                genes = new List<Genealogy>();
                                broken.Add(gene.Name, genes);
                            }

                            genes.Add(gene);
                        }
                    }

                    MiniSimDescription miniSim = gene.mMiniSim as MiniSimDescription;
                    if (miniSim == null) continue;

                    if ((MiniSimDescription.sMiniSims != null) && (!MiniSimDescription.sMiniSims.ContainsKey(miniSim.SimDescriptionId)))
                    {
                        MiniSimDescription.sMiniSims.Add(miniSim.SimDescriptionId, miniSim);

                        if (!lookup.ContainsKey(miniSim.SimDescriptionId))
                        {
                            lookup.Add(miniSim.SimDescriptionId, miniSim);
                        }

                        Overwatch.Log("Genealogy Minisim Added " + Relationships.GetFullName(miniSim));
                    }
                }

                CleanupMiniSimGenealogy(lookup, broken);

                foreach(IMiniSimDescription sim in lookup.Values)
                {
                    try
                    {
                        IMiniSimDescription lookupSim = Relationships.Find(sim.SimDescriptionId, lookup);

                        Genealogy genealogy = null;

                        if (lookupSim != null)
                        {
                            genealogy = lookupSim.CASGenealogy as Genealogy;

                            if ((genealogy != null) && (sim.CASGenealogy != null) && (!object.ReferenceEquals(genealogy, sim.CASGenealogy)))
                            {
                                sim.CASGenealogy = genealogy;

                                Overwatch.Log("Lookup Genealogy Replaced: " + Relationships.GetFullName(sim));
                            }
                        }

                        if (genealogy == null)
                        {
                            genealogy = sim.CASGenealogy as Genealogy;
                        }

                        if (genealogy == null)
                        {
                            MiniSimDescription miniSim = MiniSimDescription.Find(sim.SimDescriptionId);
                            if (miniSim != null)
                            {
                                genealogy = miniSim.mGenealogy;

                                if ((genealogy != null) && (sim.CASGenealogy != null) && (!object.ReferenceEquals(genealogy, sim.CASGenealogy)))
                                {
                                    sim.CASGenealogy = genealogy;

                                    Overwatch.Log("MiniSim Genealogy Replaced: " + Relationships.GetFullName(sim));
                                }
                            }
                        }

                        if (genealogy == null)
                        {
                            SimDescription trueSim = sim as SimDescription;
                            if (trueSim != null)
                            {
                                trueSim.Fixup();

                                genealogy = sim.CASGenealogy as Genealogy;
                                if (genealogy == null)
                                {
                                    Overwatch.Log("No SimDesc Genealogy: " + Relationships.GetFullName(sim));
                                }
                                else
                                {
                                    Overwatch.Log("Genealogy Fixup Performed: " + Relationships.GetFullName(sim));
                                }
                            }
                            else 
                            {
                                MiniSimDescription miniSim = sim as MiniSimDescription;
                                if (miniSim != null)
                                {
                                    // This minisim may be part of a family tree, so must be uncorrupted
                                    genealogy = new Genealogy(miniSim.FullName);

                                    Overwatch.Log("MiniSim Genealogy Created: " + Relationships.GetFullName(sim));
                                }
                                else
                                {
                                    Overwatch.Log("No MiniSim Genealogy: " + Relationships.GetFullName(sim));
                                }
                            }
                        }

                        SetGenealogy(sim, genealogy);
                    }
                    catch (Exception e)
                    {
                        Common.Exception("Phase One: " + Relationships.GetFullName(sim), e);
                    }
                }

                foreach (IMiniSimDescription sim in lookup.Values)
                {
                    try
                    {
                        SimDescription trueSim = sim as SimDescription;
                        if (trueSim != null)
                        {
                            MiniSimDescription miniSim = MiniSimDescription.Find(trueSim.SimDescriptionId);
                            if (miniSim != null)
                            {
                                List<MiniRelationship> relationships = new List<MiniRelationship>(miniSim.MiniRelationships);
                                foreach (MiniRelationship relationship in relationships)
                                {
                                    IMiniSimDescription otherSim = MiniSimDescription.Find(relationship.GetOtherSimDescriptionId(sim));
                                    if (otherSim == null)
                                    {
                                        miniSim.mMiniRelationships.Remove(relationship);
                                    }
                                    else
                                    {
                                        Genealogy gene = otherSim.CASGenealogy as Genealogy;
                                        if (gene == null)
                                        {
                                            miniSim.mMiniRelationships.Remove(relationship);
                                        }
                                    }
                                }
                            }

                            Dictionary<ulong, bool> existingRelations = new Dictionary<ulong, bool>();

                            Dictionary<SimDescription, Relationship> oldRelations;
                            if (Relationship.sAllRelationships.TryGetValue(trueSim, out oldRelations))
                            {
                                Dictionary<SimDescription, Relationship> newRelations = new Dictionary<SimDescription, Relationship>();

                                foreach (KeyValuePair<SimDescription, Relationship> relation in new Dictionary<SimDescription, Relationship>(oldRelations))
                                {
                                    IMiniSimDescription townSim;
                                    if (lookup.TryGetValue(relation.Key.SimDescriptionId, out townSim))
                                    {
                                        if (!object.ReferenceEquals(townSim, relation.Key))
                                        {
                                            Overwatch.Log(Relationships.GetFullName(sim) + " Dropped bad relation for " + Relationships.GetFullName(relation.Key));

                                            Relationships.SafeRemoveRelationship(relation.Value);
                                            continue;
                                        }
                                    }

                                    if (existingRelations.ContainsKey(relation.Key.SimDescriptionId))
                                    {
                                        Overwatch.Log(Relationships.GetFullName(sim) + " Dropped duplicate relation for " + Relationships.GetFullName(relation.Key));

                                        Relationships.SafeRemoveRelationship(relation.Value);
                                        continue;
                                    }

                                    existingRelations.Add(relation.Key.SimDescriptionId, true);

                                    Relationships.RepairRelationship(relation.Value, Overwatch.Log);

                                    newRelations.Add(relation.Key, relation.Value);
                                }

                                Relationship.sAllRelationships[trueSim] = newRelations;
                            }

                            /*
                            MiniSimDescription miniDesc = MiniSimDescription.Find(trueSim.SimDescriptionId);
                            if (miniDesc != null)
                            {
                                for (int i = miniDesc.mMiniRelationships.Count - 1; i >= 0; i--)
                                {
                                    MiniRelationship relation = miniDesc.mMiniRelationships[i];

                                    if (existingRelations.ContainsKey(relation.SimDescriptionId))
                                    {
                                        miniDesc.mMiniRelationships.RemoveAt(i);

                                        Overwatch.Log(GetFullName(trueSim) + " Dropped duplicate mini relation");
                                    }
                                }
                            }
                            */

                            RepairMissingPartner(trueSim);

                            // Legacy repair for an issue with an earlier Phase of Overwatch
                            List<Relationship> relations = new List<Relationship>(Relationship.GetRelationships(trueSim));
                            foreach (Relationship relation in relations)
                            {
                                if ((relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Divorce)) ||
                                    (relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.BreakUp)) ||
                                    (relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.BreakEngagement)) ||
                                    (relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Marry)) ||
                                    (relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Propose)) ||
                                    (relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.MakeCommitment)))
                                {
                                    relation.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.HaveBeenPartners);
                                }
                            }
                        }

                        Genealogy genealogy = sim.CASGenealogy as Genealogy;
                        if (genealogy != null)
                        {
                            genealogy.ClearDerivedData();

                            new Relationships.RepairParents().Perform(sim, Overwatch.Log, lookup);
                            new Relationships.RepairChildren().Perform(sim, Overwatch.Log, lookup);
                            new Relationships.RepairSiblings().Perform(sim, Overwatch.Log, lookup);

                            RepairSpouse(sim, lookup);

                            if ((sim.IsMarried) && (genealogy.mPartnerType != PartnerType.Marriage))
                            {
                                genealogy.mPartnerType = PartnerType.Marriage;

                                if (genealogy.Spouse != null)
                                {
                                    genealogy.Spouse.mPartnerType = PartnerType.Marriage;
                                }

                                Overwatch.Log(Relationships.GetFullName(sim) + " PartnerType Corrected");
                            }
                        }

                        RepairMiniSim(trueSim);
                    }
                    catch (Exception e)
                    {
                        Common.Exception("Phase Two: " + Relationships.GetFullName(sim), e);
                    }
                }

                CleanupMiniSims(lookup);

                // Must be performed after repairing the genealogy as the mSim is wiped from the genealogy during deletion
                foreach (KeyValuePair<IMiniSimDescription, IMiniSimDescription> duplicate in duplicates)
                {
                    CustomAnnihilation(duplicate.Key as SimDescription, duplicate.Value as SimDescription);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }

        protected static Genealogy ReconcileGenealogy(Genealogy newGene, Genealogy oldGene)
        {
            if (newGene == null) return oldGene;

            if (oldGene == null) return null;

            ReconcileGenealogy("Parent", Relationships.GetFullName(newGene), newGene.mNaturalParents, oldGene.mNaturalParents);
            ReconcileGenealogy("Child", Relationships.GetFullName(newGene), newGene.mChildren, oldGene.mChildren);
            ReconcileGenealogy("Sibling", Relationships.GetFullName(newGene), newGene.mSiblings, oldGene.mSiblings);

            return null;
        }
        protected static void ReconcileGenealogy(string type, string name, List<Genealogy> newGenes, List<Genealogy> oldGenes)
        {
            if ((newGenes == null) || (oldGenes == null)) return;

            foreach (Genealogy oldGene in oldGenes)
            {
                if (newGenes.Find((e) => { return object.ReferenceEquals(e, oldGene); }) != null) continue;

                if (newGenes.Find((e) => { return (e.IMiniSimDescription == oldGene.IMiniSimDescription); }) != null) continue;

                newGenes.Add(oldGene);

                Overwatch.Log("Merged " + type + " Genealogy: " + name);
            }

            oldGenes.Clear();
        }

        protected static void RepairMiniSim(SimDescription sim)
        {
            if (sim == null) return;

            MiniSimDescription miniSim = MiniSimDescription.Find(sim.SimDescriptionId);
            if (miniSim != null)
            {
                if (sim.Partner != null)
                {
                    if (miniSim.PartnerSimDescriptionId != sim.Partner.SimDescriptionId)
                    {
                        Overwatch.Log(Relationships.GetFullName(sim) + " MiniSim Partner Reset");

                        miniSim.PartnerSimDescriptionId = sim.Partner.SimDescriptionId;
                    }
                }
                else
                {
                    if (miniSim.PartnerSimDescriptionId != 0)
                    {
                        Overwatch.Log(Relationships.GetFullName(sim) + " MiniSim Partner Cleared");

                        miniSim.PartnerSimDescriptionId = 0;
                    }
                }
            }
        }

        protected void CleanupMiniSimGenealogy(Dictionary<ulong, IMiniSimDescription> existingSims, Dictionary<string, List<Genealogy>> brokenGenealogy)
        {
            if (MiniSimDescription.sMiniSims == null) return;

            List<MiniSimDescription> miniSims = new List<MiniSimDescription>(MiniSimDescription.sMiniSims.Values);
            foreach (MiniSimDescription miniSim in miniSims)
            {
                try
                {
                    if (miniSim == null) continue;

                    if (!string.IsNullOrEmpty(miniSim.FullName))
                    {
                        List<Genealogy> broken;
                        if (brokenGenealogy.TryGetValue(miniSim.FullName, out broken))
                        {
                            foreach (Genealogy gene in broken)
                            {
                                gene.mMiniSim = miniSim;

                                Overwatch.Log("Broken By Name Genealogy: " + Relationships.GetFullName(miniSim));
                            }
                        }
                    }

                    IMiniSimDescription existingSim = null;
                    if (!existingSims.TryGetValue(miniSim.SimDescriptionId, out existingSim))
                    {
                        existingSim = null;
                    }

                    if (miniSim.CASGenealogy == null)
                    {
                        if (existingSim != null)
                        {
                            miniSim.mGenealogy = existingSim.CASGenealogy as Genealogy;
                        }

                        if (miniSim.mGenealogy == null)
                        {
                            miniSim.mGenealogy = new Genealogy(miniSim.FullName);

                            Overwatch.Log("Created Missing MiniSim Genealogy: " + Relationships.GetFullName(miniSim));
                        }
                        else
                        {
                            Overwatch.Log("Assigned Missing MiniSim Genealogy: " + Relationships.GetFullName(miniSim));
                        }
                    }

                    if (miniSim.mGenealogy.mMiniSim == null)
                    {
                        miniSim.mGenealogy.mMiniSim = miniSim;

                        Overwatch.Log("Reassigned MiniSim In Genealogy: " + Relationships.GetFullName(miniSim));
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(miniSim.FullName, e);
                }
            }
        }

        protected void CleanupMiniSims(Dictionary<ulong, IMiniSimDescription> existingSims)
        {
            if (MiniSimDescription.sMiniSims == null) return;

            List<MiniSimDescription> miniSims = new List<MiniSimDescription>(MiniSimDescription.sMiniSims.Values);
            foreach (MiniSimDescription miniSim in miniSims)
            {
                try
                {
                    if (miniSim == null) continue;

                    if (miniSim.mTraits == null)
                    {
                        // Correction for a reported script error
                        miniSim.mTraits = new List<TraitNames>();
                    }

                    IMiniSimDescription existingSim = null;

                    string reason = null;

                    if (!existingSims.TryGetValue(miniSim.SimDescriptionId, out existingSim))
                    {
                        reason = "No Exist Mini Sim Removed: " + Relationships.GetFullName(miniSim);
                    }
                    else
                    {
                        miniSim.mFirstName = existingSim.FirstName;
                        miniSim.mLastName = existingSim.LastName;

                        string name = miniSim.FullName;
                        if ((name == null) || (string.IsNullOrEmpty(name.Trim())))
                        {
                            reason = "No Name Mini Sim Removed: " + Relationships.GetFullName(miniSim);
                        }
                        else if (existingSim is MiniSimDescription)
                        {
                            bool found = false;

                            if (miniSim.mHomeLotId != 0)
                            {
                                //Overwatch.Log(" " + Relationships.GetFullName(miniSim) + " HomeLotId Save");

                                found = true;
                            }
                            else if (!string.IsNullOrEmpty(miniSim.JobOrServiceName))
                            {
                                //Overwatch.Log(" " + Relationships.GetFullName(miniSim) + " JobOrServiceName Save");

                                found = true;
                            }
                            else if ((miniSim.MiniRelationships != null) && (miniSim.mMiniRelationships.Count > 0))
                            {
                                //Overwatch.Log(" " + Relationships.GetFullName(miniSim) + " Relation Save");

                                found = true;
                            }
                            else if (miniSim.Genealogy != null)
                            {
                                if ((miniSim.Genealogy.Spouse != null) || (miniSim.Genealogy.Children.Count > 0) || (miniSim.Genealogy.Parents.Count > 0) || (miniSim.Genealogy.Siblings.Count > 0))
                                {
                                    //Overwatch.Log(" " + Relationships.GetFullName(miniSim) + " Genealogy Save");

                                    found = true;
                                }
                            }

                            if (!found)
                            {
                                reason = "No Relation Mini Sim Removed: " + Relationships.GetFullName(miniSim);
                            }
                        }

                        if (!string.IsNullOrEmpty(reason))
                        {
                            try
                            {
                                Annihilation.RemoveMSD(miniSim.SimDescriptionId);

                                Overwatch.Log(reason);
                                continue;
                            }
                            catch(Exception e)
                            {
                                Common.DebugException(miniSim.FullName, e);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(miniSim.FullName, e);
                }
            }

            if (!GameStates.IsOnVacation)
            {
                foreach (MiniSimDescription sim in MiniSimDescription.GetVacationWorldSimDescriptions())
                {
                    try
                    {
                        List<MiniRelationship> relations = new List<MiniRelationship>(sim.MiniRelationships);
                        foreach (MiniRelationship relation in relations)
                        {
                            MiniSimDescription other = MiniSimDescription.Find(relation.SimDescriptionId);
                            if (other == null) continue;

                            MiniRelationship otherRelation = other.FindMiniRelationship(sim, false);
                            if (otherRelation == null)
                            {
                                Overwatch.Log("Bad MiniRelationship Dropped " + Relationships.GetFullName(sim));

                                sim.mMiniRelationships.Remove(relation);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }
            }
        }
    }
}
