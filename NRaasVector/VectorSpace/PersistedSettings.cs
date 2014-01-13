using NRaas.CommonSpace.Helpers;
using NRaas.VectorSpace.Booters;
using NRaas.VectorSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.VectorSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("The length of time spent visiting the doctor")]
        protected static int kDoctorsVisitLength = 60;

        [Tunable, TunableComment("The cost of a consultation")]
        protected static int kDoctorsVisitCost = 500;

        [Tunable, TunableComment("The increase in resistance per visit to hospital")]
        protected static int kResistanceBoost = 20;

        [Tunable, TunableComment("The reduction in infection rate of low protection shots")]
        protected static int kLowProtectionRating = 50;

        [Tunable, TunableComment("The reduction in infection rate of high protection shots")]
        protected static int kHighProtectionRating = 90;

        [Tunable, TunableComment("The number of sim hours protection is effective")]
        protected static int kProtectionDuration = 48;

        [Tunable, TunableComment("The base number of strains an inoculation protects against")]
        protected static int kInoculationRating = 0;

        [Tunable, TunableComment("The minum difference between strains to reinfect a sim")]
        protected static int kStrainReinfectionDifference = 5;

        [Tunable, TunableComment("Whether to allow active sims to be a patient zero")]
        protected static bool kOutbreakAllowActive = false;

        [Tunable, TunableComment("Whether to show notices when a sim contracts a disease")]
        protected static bool kOutbreakShowNotices = false;

        [Tunable, TunableComment("The number of initial infected during an outbreak")]
        protected static int kNumPatientZero = 1;

        [Tunable, TunableComment("A percent ratio to the length of each stage in a disease")]
        protected static int kStageRatio = 100;

        [Tunable, TunableComment("Whether to allow inactive families to purchase remedies")]
        protected static bool kAllowInactivePurchases = true;

        [Tunable, TunableComment("Whether to allow the mod to alter relationship")]
        protected static bool kAllowRelationshipDelta = true;

        Dictionary<ulong, List<DiseaseVector>> mVectors = new Dictionary<ulong, List<DiseaseVector>>();

        Dictionary<string, DiseaseVector.Variant> mVariants = new Dictionary<string, DiseaseVector.Variant>();

        Dictionary<string, bool> mEnabled = new Dictionary<string, bool>();
        Dictionary<string, bool> mIgnored = new Dictionary<string, bool>();
        Dictionary<string, bool> mManual = new Dictionary<string, bool>();
        Dictionary<string, bool> mSettings = new Dictionary<string, bool>();

        Dictionary<string, int> mMotiveAdjustmentRatio = new Dictionary<string, int>();

        public int mDoctorsVisitLength = kDoctorsVisitLength;
        public int mDoctorsVisitCost = kDoctorsVisitCost;
        public int mResistanceBoost = kResistanceBoost;
        public int mLowProtectionRating = kLowProtectionRating;
        public int mHighProtectionRating = kHighProtectionRating;
        public int mProtectionDuration = kProtectionDuration;
        public int mInoculationRating = kInoculationRating;
        public bool mAllowInactivePurchases = kAllowInactivePurchases;

        public bool mAllowRelationshipDelta = kAllowRelationshipDelta;

        public bool mOutbreakAllowActive = kOutbreakAllowActive;
        public bool mOutbreakShowNotices = kOutbreakShowNotices;

        public int mNumPatientZero = kNumPatientZero;
        public int mStageRatio = kStageRatio;
        public int mStrainReinfectionDifferenceV2 = kStrainReinfectionDifference;

        bool mDebugging = false;

        public IEnumerable<KeyValuePair<ulong,List<DiseaseVector>>> AllVectors
        {
            get { return mVectors; }
        }

        public void ClearVectors(SimDescription sim)
        {
            PrivateGetVectors(sim).Clear();
        }

        public void AddVector(SimDescription sim, DiseaseVector vector)
        {
            PrivateGetVectors(sim).Add(vector);

            if (!vector.IsInoculated)
            {
                if (Common.kDebugging)
                {
                    OutbreakControl.ShowNotice(sim, vector, "Infected: ");
                }
            }
        }

        public void ClearSettings()
        {
            mSettings.Clear();
        }

        public bool IsSet(string setting)
        {
            return mSettings.ContainsKey(setting);
        }

        public void SetCustom(string setting, bool value)
        {
            if (value)
            {
                mSettings[setting] = true;
            }
            else
            {
                mSettings.Remove(setting);
            }
        }

        public void ClearMotiveAdjustmentRatio()
        {
            mMotiveAdjustmentRatio.Clear();
        }

        public void SetMotiveAdjustmentRatio(string guid, int value)
        {
            mMotiveAdjustmentRatio[guid] = value;
        }

        public int GetMotiveAdjustmentRatio(string guid)
        {
            int value;
            if (!mMotiveAdjustmentRatio.TryGetValue(guid, out value)) return 100;

            return value;
        }

        public void ClearAutomated()
        {
            mManual.Clear();
        }

        public bool IsAutomated(string guid)
        {
            return !mManual.ContainsKey(guid);
        }

        public void SetAutomated(string guid, bool value)
        {
            // Reverse
            if (value)
            {
                mManual.Remove(guid);
            }
            else
            {
                mManual[guid] = true;
            }
        }

        public void ClearEnabled()
        {
            mEnabled.Clear();
        }

        public bool IsEnabled(string guid)
        {
            return mEnabled.ContainsKey(guid);
        }

        public void SetEnabled(string guid, bool value)
        {
            if (value)
            {
                mEnabled[guid] = true;
            }
            else
            {
                mEnabled.Remove(guid);
            }
        }

        public void ClearIgnore()
        {
            mIgnored.Clear();
        }

        public bool IsIgnored(string guid)
        {
            return mIgnored.ContainsKey(guid);
        }

        public void SetIgnore(string guid, bool value)
        {
            if (value)
            {
                mIgnored[guid] = true;
            }
            else
            {
                mIgnored.Remove(guid);
            }
        }

        public void RemoveSim(ulong sim)
        {
            mVectors.Remove(sim);
        }

        public string Dump()
        {
            StringBuilder builder = new StringBuilder();

            Dictionary<ulong,List<SimDescription>> sims = SimListing.AllSims<SimDescription>(null, true);

            foreach(KeyValuePair<ulong, List<DiseaseVector>> simPair in mVectors)
            {
                if (simPair.Value.Count == 0) continue;

                List<SimDescription> choices;
                if (!sims.TryGetValue(simPair.Key, out choices)) continue;

                builder.Append(Common.NewLine + choices[0].FullName);

                foreach (DiseaseVector vector in simPair.Value)
                {
                    builder.Append(Common.NewLine + " " + vector.UnlocalizedName);
                }
            }

            return builder.ToString();
        }

        public DiseaseVector.Variant GetCurrentStrain(VectorBooter.Data vector)
        {
            DiseaseVector.Variant strain;
            if (mVariants.TryGetValue(vector.Guid, out strain))
            {
                return strain;
            }
            else
            {
                return new DiseaseVector.Variant(vector);
            }
        }

        public DiseaseVector.Variant GetNewStrain(VectorBooter.Data vector)
        {
            return GetNewStrain(vector, GetCurrentStrain(vector), true);
        }
        public DiseaseVector.Variant GetNewStrain(VectorBooter.Data vector, DiseaseVector.Variant strain)
        {
            return GetNewStrain(vector, strain, false);
        }
        protected DiseaseVector.Variant GetNewStrain(VectorBooter.Data vector, DiseaseVector.Variant strain, bool force)
        {
            DiseaseVector.Variant newStrain = new DiseaseVector.Variant(vector, strain);

            if ((force) || (strain.Mutated))
            {
                // Doing this stops the previous strain from reentering this process
                strain.Mutated = false;

                long id = 1;

                DiseaseVector.Variant oldStrain;
                if (mVariants.TryGetValue(vector.Guid, out oldStrain))
                {
                    if (oldStrain.Strain < strain.Strain)
                    {
                        oldStrain.Strain = strain.Strain;
                    }

                    id = oldStrain.Strain;
                }
                else
                {
                    oldStrain = null;
                }

                if ((oldStrain == null) || (oldStrain.Variation(vector) < newStrain.Variation(vector)))
                {
                    newStrain.Strain++;

                    oldStrain = new DiseaseVector.Variant(newStrain, id + 1);

                    mVariants.Remove(vector.Guid);
                    mVariants.Add(vector.Guid, oldStrain);

                    if (Common.kDebugging)
                    {
                        Common.DebugNotify("BETTER " + vector.Guid + " Strain:" + Common.NewLine + oldStrain);
                    }
                }
                else if (RandomUtil.RandomChance01(vector.StrainMutationRate * (force ? 2 : 1)))
                {
                    newStrain.Strain = id + 1;

                    if (force)
                    {
                        oldStrain.Strain = newStrain.Strain;

                        if (Common.kDebugging)
                        {
                            Common.DebugNotify("OUTBREAK " + vector.Guid + " Strain:" + Common.NewLine + newStrain);
                        }
                    }
                    else
                    {
                        if (Common.kDebugging)
                        {
                            Common.DebugNotify("RANDOM " + vector.Guid + " Strain:" + Common.NewLine + newStrain);
                        }
                    }
                }
            }

            return newStrain;
        }

        public bool HasVectors(Sim sim)
        {
            return HasVectors(sim.SimDescription);
        }
        public bool HasVectors(SimDescription sim)
        {
            List<DiseaseVector> vectors;
            if (!mVectors.TryGetValue(sim.SimDescriptionId, out vectors)) return false;

            return (vectors.Count > 0);
        }

        public void RemoveVector(SimDescription sim, string guid)
        {
            List<DiseaseVector> vectors;
            if (!mVectors.TryGetValue(sim.SimDescriptionId, out vectors)) return;

            for (int i = vectors.Count - 1; i >= 0; i--)
            {
                if (vectors[i].Guid == guid)
                {
                    vectors.RemoveAt(i);
                }
            }
        }

        public IEnumerable<DiseaseVector> GetVectors(Sim sim)
        {
            return GetVectors(sim.SimDescription);
        }
        public IEnumerable<DiseaseVector> GetVectors(SimDescription sim)
        {
            return PrivateGetVectors(sim);
        }

        protected List<DiseaseVector> PrivateGetVectors(SimDescription sim)
        {
            List<DiseaseVector> vectors;
            if (!mVectors.TryGetValue(sim.SimDescriptionId, out vectors))
            {
                vectors = new List<DiseaseVector>();
                mVectors.Add(sim.SimDescriptionId, vectors);
            }

            return vectors;
        }

        public DiseaseVector GetVector(Sim sim, string guid)
        {
            return GetVector(sim.SimDescription, guid);
        }
        public DiseaseVector GetVector(SimDescription sim, string guid)
        {
            foreach (DiseaseVector vector in GetVectors(sim))
            {
                if (vector.Guid == guid) return vector;
            }

            return null;
        }

        public bool Debugging
        {
            get
            {
                return mDebugging;
            }
            set
            {
                mDebugging = value;

                Common.kDebugging = value;
            }
        }
    }
}
