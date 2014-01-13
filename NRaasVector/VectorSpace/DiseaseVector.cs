using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using NRaas.VectorSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.VectorSpace
{
    [Persistable]
    public class DiseaseVector
    {
        static Common.MethodStore sWoohooerIsRiskyOrTryForBaby = new Common.MethodStore("NRaasWoohooer", "NRaas.Woohooer", "IsRiskyOrTryForBaby", new Type[] { typeof(Event) });

        public static Origin sFromInfectionOrigin = unchecked((Origin)ResourceUtils.HashString64("NRaasFromInfection"));

        public enum Notices
        {
            None = 0x0,
            FirstSign = 0x01,
            Identified = 0x02,
            PaidInnoculaion = 0x04,
        }

        string mGuid;

        Variant mStrain;

        ulong mInfector;

        int mResistance;

        int mStage;

        int mNextCheck;

        float mVirulence = 1f;
        int mProtected;

        long mInoculationStrain;

        Notices mNotices = Notices.None;

        VectorControl.StageType mStageType = VectorControl.StageType.None;

        [Persistable(false)]
        VectorBooter.Data mNonPersistableData;

        public DiseaseVector()
        { }
        public DiseaseVector(VectorBooter.Data data, Variant strain)
        {
            mGuid = data.Guid;
            mNonPersistableData = data;
            mStrain = new Variant(data, strain);

            Start(Notices.None);
        }
        public DiseaseVector(DiseaseVector vector, SimDescription sim)
        {
            mGuid = vector.mGuid;
            mNonPersistableData = vector.mNonPersistableData;
            mStrain = new Variant(mNonPersistableData, vector.mStrain);

            Start(Notices.None);
        }

        public string Guid
        {
            get { return mGuid; }
        }

        public bool NewStrain
        {
            get { return mStrain.IsNew; }
            set { mStrain.IsNew = value; }
        }

        public VectorBooter.Data Data
        {
            get { return mNonPersistableData; }
        }

        public int HighProtectionCost
        {
            get { return mNonPersistableData.HighProtectionCost; }
        }

        public int LowProtectionCost
        {
            get { return mNonPersistableData.LowProtectionCost; }
        }

        public bool IsProtected
        {
            get { return (mProtected > 0); }
        }

        public bool IsIgnored
        {
            get { return Vector.Settings.IsIgnored(Guid); }
        }

        public bool IsActive
        {
            get 
            {
                if (mStageType == VectorControl.StageType.None) return false;

                if ((mStageType & (VectorControl.StageType.Inoculated | VectorControl.StageType.Resisted)) != VectorControl.StageType.None) return false;

                return ((mStageType & (VectorControl.StageType.ShowingSigns | VectorControl.StageType.Contagious | VectorControl.StageType.Mutate)) != VectorControl.StageType.None);
            }
        }

        public bool HasVirulence(VectorControl.Virulence virulence)
        {
            if (!IsContagious) return false;

            return mNonPersistableData.HasVirulence(mStage, virulence);
        }

        public bool IsOutdoors
        {
            get { return (HasVirulence(VectorControl.Virulence.Outdoors)); }
        }

        public bool IsRoom
        {
            get { return (HasVirulence(VectorControl.Virulence.Room)); }
        }

        public bool IsSocial
        {
            get { return (HasVirulence(VectorControl.Virulence.Social)); }
        }

        public bool IsFight
        {
            get { return (HasVirulence(VectorControl.Virulence.Fight)); }
        }

        public bool IsWoohoo
        {
            get { return (HasVirulence(VectorControl.Virulence.Woohoo)); }
        }

        public bool CanInoculate
        {
            get { return mNonPersistableData.CanInoculate(mStage); }
        }

        public long InoculationStrain
        {
            get { return mInoculationStrain; }
        }

        public int InoculationCost
        {
            get { return mNonPersistableData.InoculationCost; }
        }

        public bool HasAnyResistance
        {
            get { return (mResistance > 0); }
        }

        public bool NeedsResistance
        {
            get { return (mResistance < 0); }
        }

        public bool CanBoostResistance
        {
            get 
            {
                if (!IsActive) return false;

                return mNonPersistableData.CanBoostResistance; 
            }
        }

        public int ResistanceCost
        {
            get { return mNonPersistableData.ResistanceCost; }
        }

        public bool IsInoculationUpToDate
        {
            get
            {
                if (!IsInoculated) return false;

                return (InoculationStrain > Vector.Settings.GetCurrentStrain(mNonPersistableData).Strain);
            }
        }

        public bool IsInoculated
        {
            get { return ((mStageType & VectorControl.StageType.Inoculated) == VectorControl.StageType.Inoculated); }
        }

        public bool IsContagious
        {
            get { return ((mStageType & VectorControl.StageType.Contagious) == VectorControl.StageType.Contagious); }
        }

        public bool ShowingSigns
        {
            get { return ((mStageType & VectorControl.StageType.ShowingSigns) == VectorControl.StageType.ShowingSigns); }
        }

        public bool InRemission
        {
            get { return ((mStageType & VectorControl.StageType.Remission) == VectorControl.StageType.Remission); }
        }

        public bool IsResisted
        {
            get { return ((mStageType & VectorControl.StageType.Resisted) == VectorControl.StageType.Resisted); }
        }

        public bool IsMutating
        {
            get { return ((mStageType & VectorControl.StageType.Mutate) == VectorControl.StageType.Mutate); }
        }

        public void SetToIdentified()
        {
            mNotices |= Notices.Identified;
        }

        public bool IsIdentified
        {
            get { return ((mNotices & Notices.Identified) == Notices.Identified); }
        }

        public bool PaidInoculation
        {
            get { return ((mNotices & Notices.PaidInnoculaion) == Notices.PaidInnoculaion); }
        }

        public bool HadFirstSign
        {
            get { return ((mNotices & Notices.FirstSign) == Notices.FirstSign); }
        }

        public ulong Infector
        {
            get { return mInfector; }
        }

        public long Strain
        {
            get { return mStrain.Strain; }
        }

        public int Stage
        {
            get { return mStage; }
        }

        public string StageName
        {
            get 
            {
                if (IsInoculated)
                {
                    return "[Inoculated]";
                }
                else
                {
                    if (mStage < 0) return null;

                    return Data.GetStageName(mStage);
                }
            }
        }

        public Dictionary<string, int> Counters
        {
            get { return mStrain.Counters; }
        }

        public int RawStrength
        {
            get { return mStrain.Strength; }
        }

        public int NetStrength
        {
            get { return (RawStrength - mResistance); }
        }

        public string UnlocalizedName
        {
            get
            {
                return mGuid + " " + mStrain.Strain;
            }
        }

        public string GetLocalizedName(bool isFemale)
        {
            return Common.Localize("Strain:Name", isFemale, new object[] { mStrain.Strain, mNonPersistableData.GetLocalizedName(isFemale) });
        }

        public void SetProtection(int rating, int duration)
        {
            mVirulence = 1f - (rating / 100f);
            mProtected = duration;
        }

        public void Erase(string counter)
        {
            mStrain.Erase(counter);
        }

        public int GetCounter(string counter)
        {
            return mStrain.GetCounter(counter);
        }

        public int Increment(string counter)
        {
            return mStrain.Increment(counter);
        }
        public int Increment(string counter, int delta)
        {
            return mStrain.Increment(counter, delta);
        }
       
        public bool OnLoadFixup()
        {
            mNonPersistableData = VectorBooter.GetVector(Guid);
            if (mNonPersistableData == null) return false;

            if (mStrain == null)
            {
                mStrain = new Variant(mNonPersistableData, Vector.Settings.GetCurrentStrain(mNonPersistableData));
            }

            if (mStage > mNonPersistableData.NumStages)
            {
                mStage = 0;
            }

            return true;
        }

        protected void Init(Notices notices)
        {
            mResistance = 0;
            mNotices = notices;
            mStage = 0;
            mStageType = VectorControl.StageType.None;
        }

        protected void Start(Notices notices)
        {
            Init(notices);
            mStageType = mNonPersistableData.InitialStage(this, out mNextCheck);
        }

        public void AlterResistance(int delta)
        {
            if (!IsActive) return;

            mResistance += delta;
        }

        public void Mutate()
        {
            mStrain = mNonPersistableData.Mutate(mStrain, mStage);
        }

        public void Resist(Event e)
        {
            if (!IsActive) return;

            mNonPersistableData.Resist(e, this);
        }

        public bool Process(SimDescription sim)
        {
            if (!mNonPersistableData.IsEnabled) return false;

            if (mProtected > 0)
            {
                mProtected--;
            }
            else if (mProtected <= 0)
            {
                mVirulence = 1f;
            }

            if ((RawStrength <= 0) || (SimTypes.IsDead(sim)))
            {
                mStageType = VectorControl.StageType.Resisted;
                return false;
            }

            if ((IsInoculated) || (IsResisted)) return false;

            bool noticeShown = false;

            if (mNextCheck > 0)
            {
                mNextCheck--;
            }
            else
            {
                if (InRemission)
                {
                    mResistance = 0;
                    mNotices &= ~Notices.FirstSign;
                }

                mStageType = mNonPersistableData.AdjustStage(this, sim, out mStage, out mNextCheck);

                mNextCheck = (int)(mNextCheck * (Vector.Settings.mStageRatio / 100f));

                string key = mNonPersistableData.GetStory(mStage, IsIdentified);

                if (!string.IsNullOrEmpty(key))
                {
                    if ((SimTypes.IsSelectable(sim)) || (!IsIgnored))
                    {
                        Common.Notify(sim.CreatedSim, Common.Localize("Story:" + key, sim.IsFemale, new object[] { sim, GetLocalizedName(sim.IsFemale) }));
                        noticeShown = true;
                    }
                }
                else if ((!ShowingSigns) && (HadFirstSign))
                {
                    if (SimTypes.IsSelectable(sim))
                    {
                        if (IsIdentified)
                        {
                            Common.Notify(sim.CreatedSim, Common.Localize("Vector:Better", sim.IsFemale, new object[] { sim, GetLocalizedName(sim.IsFemale) }));
                        }
                        else
                        {
                            Common.Notify(sim.CreatedSim, Common.Localize("Vector:BetterUnknown", sim.IsFemale, new object[] { sim }));
                        }
                    }

                    mNotices &= ~Notices.FirstSign;
                }

                if (IsResisted)
                {
                    return false;
                }                
            }

            if (ShowingSigns)
            {
                if (!HadFirstSign)
                {
                    mNotices |= Notices.FirstSign;

                    if (!noticeShown)
                    {
                        if (SimTypes.IsSelectable(sim))
                        {
                            Common.Notify(sim.CreatedSim, Common.Localize("Vector:FirstSign", sim.IsFemale, new object[] { sim }));
                        }
                        else if (!IsIgnored)
                        {
                            OutbreakControl.ShowNotice(sim, this, Common.kDebugging ? "Showing: " : "");
                        }
                    }
                }

                if (sim.CreatedSim != null)
                {
                    BuffGermy.BuffInstanceGermy germy = sim.CreatedSim.BuffManager.GetElement(BuffNames.Germy) as BuffGermy.BuffInstanceGermy;
                    if (germy == null)
                    {
                        OccultTypes currentOccultTypes = sim.OccultManager.mCurrentOccultTypes;

                        try
                        {
                            sim.OccultManager.mCurrentOccultTypes = OccultTypes.None;

                            if (sim.CreatedSim.BuffManager.AddElement(BuffNames.Germy, sFromInfectionOrigin))
                            {
                                germy = sim.CreatedSim.BuffManager.GetElement(BuffNames.Germy) as BuffGermy.BuffInstanceGermy;
                            }
                        }
                        finally
                        {
                            sim.OccultManager.mCurrentOccultTypes = currentOccultTypes;
                        }
                    }

                    if (germy != null)
                    {
                        if (germy.GermyContagionBroadcaster != null)
                        {
                            germy.GermyContagionBroadcaster.Dispose();
                            germy.GermyContagionBroadcaster = null;
                        }

                        germy.mCurrentTotalDurationIncrease = 0;
                        germy.ModifyDuration(30);
                    }
                }

                mNonPersistableData.Symptomize(sim.CreatedSim, this);
            }
            
            if (IsMutating)
            {
                Mutate();
            }

            PurchaseControl.AddCheck(sim.Household);

            return true;
        }

        public bool AlterRelationship(SimDescription sim, SimDescription source, bool positive)
        {
            if (!ShowingSigns) return false;

            if (source == null) return false;

            if (!positive)
            {
                // You really can't avoid the sim if you live together
                if (sim.Household == source.Household) return false;
            }

            if (!Vector.Settings.mAllowRelationshipDelta) return false;

            Relationship relation = Relationship.Get(sim, source, true);
            if (relation == null) return false;

            int delta = mNonPersistableData.GetRelationshipDelta(mStage, positive);
            if (delta == 0) return false;

            if (relation.AreFriendsOrRomantic()) return false;

            if (delta < 0)
            {
                // If we are already negative, don't make it further
                if (relation.CurrentLTRLiking <= 0) return false;
            }
            else
            {
                if (mInfector == source.SimDescriptionId) return false;
            }

            relation.LTR.UpdateLiking(delta);
            return true;
        }

        public float GetInfectionRate(VectorControl.Virulence type)
        {
            return mNonPersistableData.GetInfectionRate(mStage, type) * mVirulence;
        }

        public bool Infect(SimDescription sim, SimDescription source, VectorControl.Virulence type, Event e)
        {
            return Infect(sim, source, type, e, false);
        }
        public bool Infect(SimDescription sim, bool force)
        {
            return Infect(sim, null, VectorControl.Virulence.Inert, null, force);
        }
        protected bool Infect(SimDescription sim, SimDescription source, VectorControl.Virulence type, Event e, bool force)
        {
            DiseaseVector existing = Vector.Settings.GetVector(sim, Guid);

            if (!force) 
            {
                if (!IsContagious)
                {
                    ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Not Contagious");
                    return false;
                }

                float chance = GetInfectionRate(type);
                if (chance == 0)
                {
                    ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Not Infectious");
                    return false;
                }
                else
                {
                    if (type == VectorControl.Virulence.Woohoo)
                    {
                        if ((!IsProtected) && (!SimTypes.IsSelectable(sim)))
                        {
                            int rating = Vector.Settings.mHighProtectionRating;
                            int cost = Data.HighProtectionCost;
                            if ((cost == 0) || (sim.FamilyFunds < cost))
                            {
                                rating = Vector.Settings.mLowProtectionRating;
                                cost = Data.LowProtectionCost;
                            }

                            if (cost <= sim.FamilyFunds)
                            {
                                if (ScoringLookup.GetScore("NRaasVectorPurchaseProtection", sim, source) > 0)
                                {
                                    sim.ModifyFunds(-cost);

                                    SetProtection(rating, Vector.Settings.mProtectionDuration);

                                    chance = GetInfectionRate(type);

                                    ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Protection Purchased");

                                    if (Common.kDebugging)
                                    {
                                        Common.DebugNotify(UnlocalizedName + Common.NewLine + sim.FullName + Common.NewLine + "Protection Bought");
                                    }
                                }
                            }
                        }

                        if (sWoohooerIsRiskyOrTryForBaby.Valid)
                        {
                            if (!sWoohooerIsRiskyOrTryForBaby.Invoke<bool>(new object[] { e }))
                            {
                                chance /= 10;
                            }
                        }
                    }

                    ScoringLog.sLog.AddStat(UnlocalizedName + " " + type + ": Infection Rate", chance);

                    if (!RandomUtil.RandomChance01(chance))
                    {
                        ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Infection Failure");
                        return false;
                    }

                    switch (type)
                    {
                        case VectorControl.Virulence.Outdoors:
                        case VectorControl.Virulence.Room:
                            break;
                        default:
                            AlterRelationship(sim, source, ((existing != null) && (existing.HadFirstSign)));
                            break;
                    }
                }
            }

            if (!mNonPersistableData.CanInfect(sim, source))
            {
                ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Infection Denied");
                return false;
            }

            if (existing != null)
            {
                return existing.Reconcile(type, mStrain, (source != null) ? source.SimDescriptionId : 0);
            }
            else
            {
                mStrain = Vector.Settings.GetNewStrain(Data, mStrain);

                if (source != null)
                {
                    mInfector = source.SimDescriptionId;
                }

                Vector.Settings.AddVector(sim, Clone(sim));

                ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": New Infection");
                return true;
            }
        }

        protected bool Reconcile(VectorControl.Virulence type, Variant newStrain, ulong infector)
        {
            if (IsInoculated)
            {
                if (InoculationStrain >= newStrain.Strain)
                {
                    ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Inoculated");
                    return false;
                }
                else
                {
                    // Inoculation has ended
                    mStrain = new Variant(mNonPersistableData, newStrain);
                    mStrain.IsNew = true;

                    mInfector = infector;

                    Start(mNotices & Notices.Identified);

                    ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Inculation Failed");
                    return true;
                }
            }

            if ((Strain + Data.MinimumStrainDifference) <= newStrain.Strain)
            {
                if ((!ShowingSigns) && (RawStrength <= newStrain.Strength)) // Only infect if not currently showing signs
                {
                    // New infection beats old one, replace it outright
                    mStrain = new Variant(mNonPersistableData, newStrain);
                    mStrain.IsNew = true;

                    mInfector = infector;

                    if (IsResisted)
                    {
                        Start(mNotices & Notices.Identified);

                        ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Stronger Reinfection");
                        return true;
                    }
                    else
                    {
                        if (!InRemission)
                        {
                            Start(mNotices & Notices.Identified);

                            ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Stronger Reinfection");
                            return true;
                        }
                        else
                        {
                            ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Stronger In Remission");
                            return false;
                        }
                    }
                }
                else if (IsResisted)
                {
                    mStrain = Vector.Settings.GetNewStrain(Data, newStrain);
                    mStrain.IsNew = true;

                    mInfector = infector;

                    if (!InRemission)
                    {
                        Start(mNotices & Notices.Identified);

                        ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": New Strain Reinfection");
                        return true;
                    }
                    else
                    {
                        ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": New Strain In Remission");
                        return false;
                    }
                }
                else
                {
                    ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Weaker Infection");
                    return false;
                }
            }
            else
            {
                ScoringLog.sLog.IncStat(UnlocalizedName + " " + type + ": Weaker Strain");
                return false;
            }
        }

        public bool Inoculate(long strain, bool paid)
        {
            Init(Notices.Identified);
            mStageType = VectorControl.StageType.Inoculated;

            bool newInoculate = false;
            if (mInoculationStrain < strain)
            {
                mInoculationStrain = strain;
                newInoculate = true;
            }

            if (paid)
            {
                mNotices |= Notices.PaidInnoculaion;
            }

            return newInoculate;
        }

        public string GetDoctorNotice(bool isFemale)
        {
            string result = null;

            if (IsIdentified)
            {
                result += Common.Localize("DoctorNotice:OldTitle", isFemale, new object[] { GetLocalizedName(isFemale) });
            }
            else
            {
                result += Common.Localize("DoctorNotice:NewTitle", isFemale, new object[] { GetLocalizedName(isFemale) });
            }

            result += Common.Localize("DoctorNotice:" + Guid, isFemale);

            foreach (VectorControl.StageType type in Enum.GetValues(typeof(VectorControl.StageType)))
            {
                switch (type)
                {
                    case VectorControl.StageType.None:
                    case VectorControl.StageType.Contagious: // Handled later
                    case VectorControl.StageType.Mutate:
                        continue;
                }

                if ((mStageType & type) != type) continue;

                result += Common.Localize("DoctorNotice:" + type, isFemale);
            }

            Dictionary<string, ResistanceBooter.Data> resistances = new Dictionary<string, ResistanceBooter.Data>();
            Data.GetResistances(resistances);

            if (mResistance < 0)
            {
                bool first = true;

                foreach (ResistanceBooter.Data resistance in resistances.Values)
                {
                    if (resistance.Delta <= 0) continue;

                    if (first)
                    {
                        first = false;
                        result += Common.NewLine + Common.NewLine + Common.Localize("DoctorNotice:Inflamatory", isFemale);
                    }

                    result += Common.NewLine + " " + resistance.GetLocalizedName(isFemale);
                }

                if (first)
                {
                    result += Common.NewLine + Common.NewLine + Common.Localize("DoctorNotice:InflamatoryNone", isFemale);

                    foreach (ResistanceBooter.Data resistance in resistances.Values)
                    {
                        if (resistance.Delta >= 0) continue;

                        result += Common.NewLine + " " + resistance.GetLocalizedName(isFemale);
                    }
                }
            }
            else if (mResistance >= 0)
            {
                bool first = true;

                foreach (ResistanceBooter.Data resistance in resistances.Values)
                {
                    if (resistance.Delta <= 0) continue;

                    if (first)
                    {
                        first = false;
                        result += Common.NewLine + Common.NewLine + Common.Localize("DoctorNotice:Resistance", isFemale);
                    }

                    result += Common.NewLine + " " + resistance.GetLocalizedName(isFemale);
                }

                if (first)
                {
                    result += Common.NewLine + Common.NewLine + Common.Localize("DoctorNotice:ResistanceNone", isFemale);

                    foreach (ResistanceBooter.Data resistance in resistances.Values)
                    {
                        if (resistance.Delta >= 0) continue;

                        result += Common.NewLine + " " + resistance.GetLocalizedName(isFemale);
                    }
                }
            }

            result += Common.NewLine;

            if (IsContagious)
            {
                bool first = true;

                foreach(VectorControl.Virulence virulence in Enum.GetValues(typeof(VectorControl.Virulence)))
                {
                    if (virulence == VectorControl.Virulence.Inert) continue;

                    float rate = GetInfectionRate(virulence);
                    if (rate == 0) continue;

                    if (first)
                    {
                        first = false;
                        result += Common.NewLine + Common.Localize("DoctorNotice:Contagious", isFemale);
                    }

                    result += Common.NewLine + Common.Localize("Research:" + virulence, isFemale, new object[] { (int)(rate * 100) });
                }

                if (HighProtectionCost > 0)
                {
                    result += Common.NewLine + Common.Localize("DoctorNotice:Protection", isFemale, new object[] { Data.HighProtectionCost });
                }
            }

            result += Common.NewLine;

            if (!IsInoculated)
            {
                if (CanInoculate)
                {
                    result += Common.NewLine + Common.Localize("DoctorNotice:Inoculate", isFemale, new object[] { Data.InoculationCost, Data.InoculationStrain });
                }
            }
            else
            {
                long currentStrain = Vector.Settings.GetCurrentStrain(mNonPersistableData).Strain;

                result += Common.NewLine + Common.Localize("DoctorNotice:CurrentInoculate", isFemale, new object[] { InoculationStrain, currentStrain });

                if ((InoculationStrain + 10) < currentStrain)
                {
                    result += Common.NewLine + Common.Localize("DoctorNotice:CurrentInoculateSafe", isFemale);
                }
                else
                {
                    result += Common.NewLine + Common.Localize("DoctorNotice:CurrentInoculateUnsafe", isFemale);
                }
            }

            if (IsActive)
            {
                if (Data.CanBoostResistance)
                {
                    result += Common.NewLine + Common.Localize("DoctorNotice:Booster", isFemale, new object[] { Data.ResistanceCost });
                }
            }

            result += Common.NewLine;

            mNotices |= Notices.Identified;

            return result;
        }

        public virtual DiseaseVector Clone(SimDescription sim)
        {
            return new DiseaseVector(this, sim);
        }

        public string GetUnlocalizedDescription()
        {
            string msg = "Disease: " + UnlocalizedName;

            msg += Common.NewLine + " Resistance: " + mResistance;
            msg += Common.NewLine + " Virulence: " + mVirulence;
            msg += Common.NewLine + " Protected: " + mProtected;
            msg += Common.NewLine + " Stage: " + mNonPersistableData.GetStageName(mStage);
            msg += Common.NewLine + " NextCheck: " + mNextCheck;
            msg += Common.NewLine + " Type: " + mStageType;
            msg += Common.NewLine + " Infector: " + mInfector;
            msg += Common.NewLine + " InoculationStrain: " + mInoculationStrain;

            msg += Common.NewLine + " Strain: " + mStrain;

            return msg;
        }

        public override string ToString()
        {
            string msg = GetUnlocalizedDescription();

            msg += Common.NewLine + mNonPersistableData;

            return msg;
        }

        [Persistable]
        public class Variant
        {
            long mStrain;

            int mStrength;

            bool mMutated;

            bool mNew;

            Dictionary<string, int> mCounters = new Dictionary<string,int>();

            public Variant()
            { }
            public Variant(VectorBooter.Data vector)
                : this(vector, 1, vector.InitialStrength, new Dictionary<string, int>())
            { }
            public Variant(Variant strain, long strainID)
            {
                mStrain = strainID;
                mStrength = strain.Strength;
                mCounters = new Dictionary<string,int>(strain.Counters);
            }
            public Variant(VectorBooter.Data vector, Variant strain)
                : this(vector, strain.Strain, strain.Strength, strain.mCounters)
            { }
            protected Variant(VectorBooter.Data vector, long strain, int strength, Dictionary<string, int> counters)
            {
                mStrain = strain;
                mStrength = strength;
                mMutated = false;

                Dictionary<string, bool> mutables = vector.AllMutables;

                foreach (KeyValuePair<string, int> pair in counters)
                {
                    if (!mutables.ContainsKey(pair.Key)) continue;

                    mCounters.Add(pair.Key, pair.Value);
                }
            }

            public int Variation(VectorBooter.Data vector)
            {
                int result = Math.Abs(mStrength - vector.InitialStrength);

                Dictionary<string, bool> mutables = vector.AllMutables;

                foreach (KeyValuePair<string, int> pair in mCounters)
                {
                    if (!mutables.ContainsKey(pair.Key)) continue;

                    result += Math.Abs(pair.Value);
                }

                return result;
            }

            public bool IsNew
            {
                get { return mNew; }
                set { mNew = value; }
            }

            public bool Mutated
            {
                get { return mMutated; }
                set { mMutated = value; }
            }

            public long Strain
            {
                get { return mStrain; }
                set { mStrain = value; }
            }

            public int Strength
            {
                get { return mStrength; }
                set { mStrength = value; }
            }

            public Dictionary<string, int> Counters
            {
                get { return mCounters; }
            }

            public void Erase(string counter)
            {
                mCounters.Remove(counter);
            }

            public int GetCounter(string counter)
            {
                if (string.IsNullOrEmpty(counter)) return 0;

                int value;
                if (!mCounters.TryGetValue(counter, out value)) return 0;

                return value;
            }

            public int Increment(string counter)
            {
                return Increment(counter, 1);
            }
            public int Increment(string counter, int delta)
            {
                int value;
                if (!mCounters.TryGetValue(counter, out value))
                {
                    mCounters[counter] = delta;
                    return delta;
                }
                else
                {
                    value += delta;
                    mCounters[counter] = value;
                    return value;
                }
            }
            
            public bool Mutate(VectorBooter.Mutable mutation)
            {
                int delta = RandomUtil.GetInt(mutation.mMinimum, mutation.mMaximum);
                if (delta == 0) return false;

                if (mutation.mCounter == "Strength")
                {
                    mStrength += delta;
                }
                else
                {
                    Increment(mutation.mCounter, delta);
                }

                mMutated = true;
                return true;
            }

            public override string ToString()
            {
                string result = "Variant: " + mStrain;

                result += Common.NewLine + " Strength: " + mStrength;

                if (mCounters != null)
                {
                    foreach (KeyValuePair<string, int> counter in mCounters)
                    {
                        result += Common.NewLine + " " + counter.Key + ": " + counter.Value;
                    }
                }

                return result;
            }

        }
    }
}
