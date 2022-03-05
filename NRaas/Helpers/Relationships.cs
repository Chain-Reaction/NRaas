using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.SimIFace.CAS;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Relationships : Common.IWorldQuit
    {
        static Dictionary<Genealogy, AncestorData> sAncestors = new Dictionary<Genealogy, AncestorData>();

        public delegate void Logger(string text);

        public void OnWorldQuit()
        {
            sAncestors.Clear();
        }

        public static void CheckAddHumanParentFlagOnAdoption(SimDescription human, SimDescription pet)
        {
            if (pet.Child)
            {
                TraitManager traitManager = human.TraitManager;
                if ((traitManager.HasElement(TraitNames.AnimalLover) || (pet.IsCat && traitManager.HasElement(TraitNames.CatPerson))) || (pet.IsADogSpecies && traitManager.HasElement(TraitNames.DogPerson)))
                {
                    Relationship relationship = Relationship.Get(human, pet, true);
                    if (relationship != null)
                    {
                        relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.HumanParentPetRel);
                    }
                }
            }
        }

        public static Dictionary<SimDescription, Relationship> StoreRelations(SimDescription sim, Logger log)
        {
            Dictionary<SimDescription, Relationship> relations;
            if (Relationship.sAllRelationships.TryGetValue(sim, out relations))
            {
                Dictionary<SimDescription, Relationship> newRelations = new Dictionary<SimDescription, Relationship>();

                Dictionary<ulong, SimDescription> required = new Dictionary<ulong, SimDescription>();

                foreach (SimDescription member in GetChildren(sim))
                {
                    if (!required.ContainsKey(member.SimDescriptionId))
                    {
                        required.Add(member.SimDescriptionId, member);
                    }
                }

                foreach (SimDescription member in GetParents(sim))
                {
                    if (!required.ContainsKey(member.SimDescriptionId))
                    {
                        required.Add(member.SimDescriptionId, member);
                    }
                }

                foreach (SimDescription member in GetSiblings(sim))
                {
                    if (!required.ContainsKey(member.SimDescriptionId))
                    {
                        required.Add(member.SimDescriptionId, member);
                    }
                }

                Career career = sim.Occupation as Career;
                if ((career != null) && (career.Boss != null))
                {
                    if (!required.ContainsKey(career.Boss.SimDescriptionId))
                    {
                        required.Add(career.Boss.SimDescriptionId, career.Boss);
                    }
                }

                Dictionary<ulong, SimDescription> existing = new Dictionary<ulong, SimDescription>();

                foreach (KeyValuePair<SimDescription, Relationship> relation in new Dictionary<SimDescription,Relationship> (relations))
                {
                    if (relation.Value.LTR.CurrentLTR == Sims3.UI.Controller.LongTermRelationshipTypes.Stranger)
                    {
                        Relationships.SafeRemoveRelationship(relation.Value);
                    }

                    if (existing.ContainsKey(relation.Key.SimDescriptionId)) continue;

                    SimDescription requiredSim;
                    if (required.TryGetValue(relation.Key.SimDescriptionId, out requiredSim))
                    {
                        if (!object.ReferenceEquals(requiredSim, relation.Key)) continue;
                    }

                    existing.Add(relation.Key.SimDescriptionId, relation.Key);

                    Relationships.RepairRelationship(relation.Value, log);

                    newRelations.Add(relation.Key, relation.Value);
                }

                Relationship.sAllRelationships.Remove(sim);
                Relationship.sAllRelationships.Add(sim, newRelations);

                return relations;
            }

            return null;
        }

        public static void RestoreRelations(SimDescription sim, Dictionary<SimDescription, Relationship> relations)
        {
            if (relations != null)
            {
                Dictionary<SimDescription, Relationship> oldRelations;
                if (Relationship.sAllRelationships.TryGetValue(sim, out oldRelations))
                {
                    List<SimDescription> remove = new List<SimDescription>();

                    foreach (KeyValuePair<SimDescription, Relationship> relation in relations)
                    {
                        if (!oldRelations.ContainsKey(relation.Key))
                        {
                            remove.Add(relation.Key);
                        }
                    }

                    foreach (SimDescription other in remove)
                    {
                        relations.Remove(other);
                    }
                }

                Relationship.sAllRelationships.Remove(sim);
                Relationship.sAllRelationships.Add(sim, relations);
            }
        }

        public static SimDescription GetSim(Genealogy genealogy)
        {
            if (genealogy == null) return null;

            try
            {
                return genealogy.SimDescription;
            }
            catch (Exception e)
            {
                Common.DebugException(genealogy.Name, e);
            }
            return null;
        }

        public static void SetPartner(SimDescription ths, SimDescription x)
        {
            Common.StringBuilder msg = new Common.StringBuilder();

            try
            {
                if ((ths == null) || (x == null)) return;

                msg += "A";

                if (ths.Partner != x)
                {
                    if (x.IsMale)
                    {
                        ths.IncreaseGenderPreferenceMale();
                    }
                    else
                    {
                        ths.IncreaseGenderPreferenceFemale();
                    }

                    if (ths.IsMale)
                    {
                        x.IncreaseGenderPreferenceMale();
                    }
                    else
                    {
                        x.IncreaseGenderPreferenceFemale();
                    }

                    msg += "B";

                    if ((ths.Partner != null) && (ths.Partner != x))
                    {
                        ths.Partner.ClearPartner();
                    }
                    if ((x.Partner != null) && (x.Partner != ths))
                    {
                        x.ClearPartner();
                    }

                    msg += "C";

                    Relationship relationship = Relationship.Get(ths, x, true);
                    if (relationship != null)
                    {
                        // Must be able to create a relationship to set the Partner
                        ths.Partner = x;
                        x.Partner = ths;

                        msg += "D";

                        InformationLearnedAboutSim info = relationship.InformationAbout(x);
                        if (info != null)
                        {
                            info.PartnerKnown = true;
                        }

                        info = relationship.InformationAbout(ths);
                        if (info != null)
                        {
                            info.PartnerKnown = true;
                        }
                    }

                    msg += "E";

                    if ((x.CreatedSim != null) && (x.TraitManager.HasElement(TraitNames.CommitmentIssues)))
                    {
                        ActiveTopic.AddToSim(x.CreatedSim, "Has Made Commitment");
                    }

                    msg += "F";

                    if ((ths.CreatedSim != null) && (ths.TraitManager.HasElement(TraitNames.CommitmentIssues)))
                    {
                        ActiveTopic.AddToSim(ths.CreatedSim, "Has Made Commitment");
                    }

                    msg += "G";
                }
            }
            catch (Exception e)
            {
                Common.Exception(ths, x, msg, e);
            }
        }

        public static List<SimDescription> GetSims(List<Genealogy> genes)
        {
            List<SimDescription> list = new List<SimDescription>();

            if (genes != null)
            {
                foreach (Genealogy gene in genes)
                {
                    SimDescription sim = GetSim(gene);
                    if (sim == null) continue;

                    list.Add(sim);
                }
            }

            return list;
        }

        public static void ResetAncestor(Genealogy a)
        {
            sAncestors.Remove(a);
        }

        public static AncestorData GetAncestorData(Genealogy a)
        {
            AncestorData ancestors;
            if (!sAncestors.TryGetValue(a, out ancestors))
            {
                ancestors = new AncestorData(a);
                sAncestors.Add(a, ancestors);
            }

            return ancestors;
        }

        public static void ClearAncestorData(Genealogy a)
        {
            sAncestors.Remove(a);
        }

        public static bool CanHaveRomanceWith(Logger log, SimDescription ths, SimDescription other, bool testAge, bool allowAdultTeen, bool testRelation, bool thoroughCheck)
        {
            if (!SimTypes.IsEquivalentSpecies(ths, other))
            {
                if (log != null)
                {
                    log("Species Mismatch");
                }
                return false;
            }

            bool flag = false;
            if (!testAge)
            {
                flag = true;
            }
            else
            {
                switch (ths.Age)
                {
                    case CASAgeGenderFlags.Adult:
                    case CASAgeGenderFlags.Elder:
                    case CASAgeGenderFlags.YoungAdult:
                        if (other.YoungAdultOrAbove)
                        {
                            flag = true;
                        }
                        else if (allowAdultTeen)
                        {
                            if (other.Teen)
                            {
                                flag = true;
                            }
                        }
                        break;

                    case CASAgeGenderFlags.Teen:
                        if (allowAdultTeen)
                        {
                            if (other.TeenOrAbove)
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            if (other.Teen)
                            {
                                flag = true;
                            }
                        }
                        break;
                }
            }

            if (!flag)
            {
                if (log != null)
                {
                    log("Age Mismatch: " + ths.Age + " - " + other.Age);
                }
                return false;
            }

            if ((testRelation) && (IsCloselyRelated(ths.Genealogy, other.Genealogy, thoroughCheck)))
            {
                if (log != null)
                {
                    log("IsCloselyRelated");
                }
                return false;
            }

            return true;
        }

        public static bool IsStepRelated(Genealogy a, Genealogy b)
        {
            if (a == null || b == null) return false;

            AncestorData aData = GetAncestorData(a);
            AncestorData bData = GetAncestorData(b);

            return aData.IsStepRelated(a, b);
        }

        public static bool IsBloodRelated(Genealogy a, Genealogy b, bool thoroughCheck)
        {
            if (a == b) return true;

            if (a.Siblings.Contains(b))
            {
                return true;
            }

            AncestorData aData = GetAncestorData(a);
            AncestorData bData = GetAncestorData(b);
            
            if (aData.IsDirect(b))
            {
                return true;
            }
            else if (bData.IsDirect(a))
            {
                return true;
            }
            else if (aData.IsSibling(b))
            {
                return true;
            }
            else if (bData.IsSibling(a))
            {
                return true;
            }

            foreach (Genealogy genealogy3 in a.mNaturalParents)
            {
                foreach (Genealogy genealogy4 in b.mNaturalParents)
                {
                    if (genealogy3.Siblings.Contains(genealogy4))
                    {
                        return true;
                    }
                }
            }

            if (thoroughCheck)
            {
                if (aData.Overlaps(bData)) return true;
            }

            return false;
        }

        protected static bool IsCloselyRelated(Genealogy a, Genealogy b, bool thoroughCheck)
        {
            if ((a == null) || (b == null)) return false;

            if (a == b) return true;

            /*
            if (a.IMiniSimDescription.IsRobot) return false;

            if (b.IMiniSimDescription.IsRobot) return false;
            */

            return (IsBloodRelated(a, b, thoroughCheck) || IsStepRelated(a, b));
        }
        public static bool IsCloselyRelated(SimDescription a, SimDescription b, bool thoroughCheck)
        {
            if ((a == null) || (b == null)) return false;

            if (a == b) return true;

            if (!SimTypes.IsEquivalentSpecies(a, b)) return false;

            if (a.IsRobot) return false;

            if (b.IsRobot) return false;

            if (FutureDescendantService.IsAncestorOf(a, b) || FutureDescendantService.IsAncestorOf(b, a)) return true;

            return IsCloselyRelated(a.Genealogy, b.Genealogy, thoroughCheck);
        }

        public static bool IsPlumbotRelated(SimDescription a, SimDescription b)
        {
            if ((!a.IsEP11Bot && !b.IsEP11Bot) || (a.IsEP11Bot && b.IsEP11Bot) || (a.IsPet || b.IsPet))
            {
                return false;
            }

            if (a.IsEP11Bot)
            {
                foreach (SimDescription sim in GetParents(a))
                {
                    if(sim.SimDescriptionId == b.SimDescriptionId)
                    {
                        return true;
                    }                
                }
            }
            else
            {                
                foreach (SimDescription sim in GetChildren(a))
                {
                    if (sim.SimDescriptionId == b.SimDescriptionId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static List<SimDescription> GetParents(SimDescription sim)
        {
            if ((sim == null) || (sim.Genealogy == null))
            {
                return new List<SimDescription>();
            }

            return GetSims(sim.Genealogy.Parents);
        }

        public static List<SimDescription> GetChildren(SimDescription sim)
        {
            if ((sim == null) || (sim.Genealogy == null))
            {
                return new List<SimDescription>();
            }
            return GetSims(sim.Genealogy.Children);
        }

        public static List<SimDescription> GetSiblings(SimDescription sim)
        {
            if ((sim == null) || (sim.Genealogy == null))
            {
                return new List<SimDescription>();
            }
            return GetSims(sim.Genealogy.Siblings);
        }

        public static void GetParents(SimDescription sim, out SimDescription mom, out SimDescription dad)
        {
            mom = null;
            dad = null;

            if (sim.Genealogy != null)
            {
                if (sim.Genealogy.Parents.Count > 0)
                {
                    mom = GetSim(sim.Genealogy.Parents[0]);

                    if ((mom != null) && (mom.IsMale))
                    {
                        dad = mom;
                        mom = null;
                    }
                }

                if (sim.Genealogy.Parents.Count > 1)
                {
                    SimDescription parent = GetSim(sim.Genealogy.Parents[1]);

                    if ((parent != null) && (parent.IsMale))
                    {
                        if (dad == null)
                        {
                            dad = parent;
                        }
                        else
                        {
                            mom = parent;
                        }
                    }
                    else
                    {
                        if (mom == null)
                        {
                            mom = parent;
                        }
                        else
                        {
                            dad = parent;
                        }
                    }
                }
            }
        }

        public static void SafeRemoveRelationship(Relationship r)
        {
            Dictionary<SimDescription, Relationship> relations;
            if (Relationship.sAllRelationships.TryGetValue(r.SimDescriptionA, out relations))
            {
                relations.Remove(r.SimDescriptionB);
            }

            if (Relationship.sAllRelationships.TryGetValue(r.SimDescriptionB, out relations))
            {
                relations.Remove(r.SimDescriptionA);
            }
        }

        public static bool RepairRelationship(Relationship r, Logger log)
        {
            bool result = false;

            Dictionary<SimDescription, Relationship> relations;
            if (!Relationship.sAllRelationships.TryGetValue(r.SimDescriptionA, out relations))
            {
                relations = new Dictionary<SimDescription, Relationship>();

                Relationship.sAllRelationships.Add(r.SimDescriptionA, relations);
            }

            if (!relations.ContainsKey(r.SimDescriptionB))
            {
                relations.Add(r.SimDescriptionB, r);

                if (log != null)
                {
                    log("Relationship Corrected: " + r.SimDescriptionB.FullName + " - " + r.SimDescriptionA.FullName);
                }

                result = true;
            }

            if (!Relationship.sAllRelationships.TryGetValue(r.SimDescriptionB, out relations))
            {
                relations = new Dictionary<SimDescription, Relationship>();

                Relationship.sAllRelationships.Add(r.SimDescriptionB, relations);
            }

            if (!relations.ContainsKey(r.SimDescriptionA))
            {
                relations.Add(r.SimDescriptionA, r);

                if (log != null)
                {
                    log("Relationship Corrected: " + r.SimDescriptionA.FullName + " - " + r.SimDescriptionB.FullName);
                }

                result = true;
            }

            return result;
        }

        public static string GetFullName(IMiniSimDescription sim)
        {
            return (sim.FullName + " (" + sim.SimDescriptionId + ")");
        }
        public static string GetFullName(Genealogy gene)
        {
            string result = gene.Name;

            IMiniSimDescription miniSim = gene.IMiniSimDescription;
            if (miniSim != null)
            {
                result += " (" + miniSim.SimDescriptionId + ")";
            }

            return result;
        }

        public static IMiniSimDescription GetSim(Genealogy genealogy, Dictionary<ulong, IMiniSimDescription> lookup)
        {
            if (genealogy == null) return null;

            try
            {
                if (genealogy.mSim != null)
                {
                    return Find(genealogy.mSim.SimDescriptionId, lookup);
                }
                else if (genealogy.mMiniSim != null)
                {
                    return Find(genealogy.mMiniSim.SimDescriptionId, lookup);
                }
            }
            catch (Exception e)
            {
                Common.Exception(genealogy.Name, e);
            }
            return null;
        }

        public static IMiniSimDescription Find(ulong id, Dictionary<ulong, IMiniSimDescription> lookup)
        {
            IMiniSimDescription result;
            if (lookup.TryGetValue(id, out result))
            {
                return result;
            }
            return null;
        }

        // From ManagerCareer in SP. Seems fitting here...
        public static bool IsCoworkerOrBoss(Occupation career, SimDescription sim)
        {
            if (career == null) return false;

            if (career.Boss == sim) return true;

            if (career.Coworkers != null)
            {
                if (career.Coworkers.Contains(sim)) return true;
            }

            return false;
        }

        public abstract class RepairGenealogy
        {
            public delegate void Logger(string text);

            protected abstract string Name
            {
                get;
            }

            protected abstract List<Genealogy> GetGenealogy(Genealogy gene);

            protected abstract List<Genealogy> GetReverseGenealogy(Genealogy gene);

            public static bool ValidateMiniSim(IMiniSimDescription sim)
            {
                if (sim == null) return false;

                if (sim is SimDescription) return true;

                string name = sim.FullName;
                if (name != null)
                {
                    name = name.Trim();

                    if (!string.IsNullOrEmpty(name)) return true;
                }

                if (sim.Gender != CASAgeGenderFlags.None) return true;

                if (sim.Age != CASAgeGenderFlags.None) return true;

                return false;
            }

            public void Perform(IMiniSimDescription sim, Logger log, Dictionary<ulong, IMiniSimDescription> lookup)
            {
                if (sim == null) return;

                Genealogy primaryGenealogy = sim.CASGenealogy as Genealogy;
                if (primaryGenealogy == null) return;

                Perform(sim, GetGenealogy(primaryGenealogy), log, lookup);
            }
            public void Perform(IMiniSimDescription sim, List<Genealogy> list, Logger log, Dictionary<ulong, IMiniSimDescription> lookup)
            {
                try
                {
                    if (list == null) return;

                    if (sim == null) return;

                    Genealogy primaryGenealogy = sim.CASGenealogy as Genealogy;
                    if (primaryGenealogy == null) return;

                    Dictionary<ulong, bool> geneLookup = new Dictionary<ulong, bool>();

                    for (int index = list.Count - 1; index >= 0; index--)
                    {
                        Genealogy origGenealogy = list[index];
                        if (origGenealogy == null)
                        {
                            list.RemoveAt(index);

                            log("Empty " + Name + " Link Dropped");
                            log("  " + GetFullName(sim));
                        }
                        else
                        {
                            IMiniSimDescription otherSim = GetSim(origGenealogy, lookup);
                            if (otherSim == null)
                            {
                                if (!ValidateMiniSim(origGenealogy.IMiniSimDescription))
                                {
                                    list.RemoveAt(index);

                                    log(Name + " Broken Link Removed");
                                }
                                else
                                {
                                    log(Name + " Broken Link Left");
                                }

                                log("  " + GetFullName(sim) + " - " + GetFullName(origGenealogy));
                            }
                            else if (!ValidateMiniSim(otherSim))
                            {
                                list.RemoveAt(index);

                                log(Name + " Bogus MiniSim");
                                log("  " + GetFullName(sim) + " - " + GetFullName(origGenealogy));
                            }
                            else
                            {
                                Genealogy otherGenealogy = otherSim.CASGenealogy as Genealogy;
                                if (otherGenealogy != null)
                                {
                                    if (geneLookup.ContainsKey(otherSim.SimDescriptionId))
                                    {
                                        list.RemoveAt(index);

                                        log(Name + " Link Duplicate Removed");
                                        log("  " + GetFullName(sim) + " - " + GetFullName(otherSim));
                                    }
                                    else
                                    {
                                        geneLookup.Add(otherSim.SimDescriptionId, true);

                                        if (!object.ReferenceEquals(origGenealogy, otherGenealogy))
                                        {
                                            list[index] = otherGenealogy;

                                            log(Name + " Link Replaced");
                                            log("  " + GetFullName(sim) + " - " + GetFullName(otherSim));
                                        }

                                        List<Genealogy> reverseList = GetReverseGenealogy(otherGenealogy);
                                        if (reverseList != null)
                                        {
                                            if (reverseList.Find(e => { return object.ReferenceEquals(e, primaryGenealogy); }) == null)
                                            {
                                                reverseList.Add(primaryGenealogy);

                                                log(Name + " Reverse Link Attached");
                                                log("  " + GetFullName(sim) + " - " + GetFullName(otherSim));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    otherSim.CASGenealogy = new Genealogy(otherSim.FullName);

                                    log(Name + " Missing Genealogy Added");
                                    log("  " + GetFullName(sim) + " - " + GetFullName(otherSim));
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, null, Name, e);
                }
            }
        }

        public class RepairParents : RepairGenealogy
        {
            protected override string Name
            {
                get { return "Parent"; }
            }

            protected override List<Genealogy> GetGenealogy(Genealogy gene)
            {
                return gene.mNaturalParents;
            }

            protected override List<Genealogy> GetReverseGenealogy(Genealogy gene)
            {
                return gene.Children;
            }
        }

        public class RepairChildren : RepairGenealogy
        {
            protected override string Name
            {
                get { return "Child"; }
            }

            protected override List<Genealogy> GetGenealogy(Genealogy gene)
            {
                return gene.mChildren;
            }

            protected override List<Genealogy> GetReverseGenealogy(Genealogy gene)
            {
                return gene.Parents;
            }
        }

        public class RepairSiblings : RepairGenealogy
        {
            protected override string Name
            {
                get { return "Sibling"; }
            }

            protected override List<Genealogy> GetGenealogy(Genealogy gene)
            {
                return gene.mSiblings;
            }

            protected override List<Genealogy> GetReverseGenealogy(Genealogy gene)
            {
                return gene.Siblings;
            }
        }

        public class AncestorData
        {
            Dictionary<Genealogy, bool> mDirectAncestors = new Dictionary<Genealogy, bool>();
            Dictionary<Genealogy, bool> mSiblingAncestors = new Dictionary<Genealogy, bool>();
            Dictionary<Genealogy, bool> mStepAncestors = new Dictionary<Genealogy, bool>();

            public AncestorData(Genealogy sim)
            {
                foreach (Genealogy ancestor in sim.Ancestors)
                {
                    if (mDirectAncestors.ContainsKey(ancestor)) continue;

                    mDirectAncestors.Add(ancestor, true);

                    foreach (Genealogy sibling in ancestor.Siblings)
                    {
                        if (mSiblingAncestors.ContainsKey(sibling)) continue;

                        mSiblingAncestors.Add(sibling, true);
                    }
                }
            }

            public bool IsDirect(Genealogy sim)
            {
                return mDirectAncestors.ContainsKey(sim);
            }

            public bool IsSibling(Genealogy sim)
            {
                return mSiblingAncestors.ContainsKey(sim);
            }

            public bool IsStepRelated(Genealogy a, Genealogy b)
            {
                if(mStepAncestors.ContainsKey(b))
                {
                    return mStepAncestors[b];
                }

                if(a.IsStepRelated(b))
                {
                    mStepAncestors.Add(b, true);
                } else {
                    mStepAncestors.Add(b, false);
                }

                return mStepAncestors[b];
            }

            public bool Overlaps(AncestorData data)
            {
                foreach (Genealogy ancestor in mDirectAncestors.Keys)
                {
                    if (data.mDirectAncestors.ContainsKey(ancestor)) return true;
                }

                return false;
            }
        }
    }
}

