using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public abstract class DualSimFromList : SimFromList
    {
        private List<IMiniSimDescription> mA = null;

        public DualSimFromList()
        {}

        protected bool IsFirst
        {
            get
            {
                return (mA == null);
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mA = null;

            return base.Allow(parameters);
        }

        protected virtual int GetMaxSelectionA()
        {
            return GetMaxSelection();
        }

        protected virtual int GetMaxSelectionB(IMiniSimDescription sim)
        {
            return GetMaxSelection();
        }

        protected virtual ICollection<SimSelection.ICriteria> GetCriteriaA(GameHitParameters<GameObject> parameters)
        {
            return GetCriteria(parameters);
        }

        protected virtual ICollection<SimSelection.ICriteria> GetCriteriaB(GameHitParameters<GameObject> parameters)
        {
            return SelectionCriteria.SelectionOption.List;
        }

        protected virtual IEnumerable<CASAgeGenderFlags> GetSpeciesFilterA()
        {
            return base.GetSpeciesFilter();
        }

        protected virtual IEnumerable<CASAgeGenderFlags> GetSpeciesFilterB()
        {
            return base.GetSpeciesFilter();
        }

        protected abstract string GetTitleA();

        protected abstract string GetTitleB();

        protected bool Run(IMiniSimDescription a, IMiniSimDescription b)
        {
            try
            {
                if (b is SimDescription)
                {
                    SimDescription sim = b as SimDescription;

                    if (!Test(a, sim)) return true;

                    return PrivateRun(a, sim);
                }
                else
                {
                    MiniSimDescription sim = b as MiniSimDescription;

                    if (!Test(a, sim)) return true;

                    return PrivateRun(a, sim);
                }
            }
            catch (Exception e)
            {
                Common.Exception(a, b as SimDescription, e);
                return false;
            }
        }

        protected virtual bool PrivateRun(IMiniSimDescription a, SimDescription b)
        {
            SimDescription simA = a as SimDescription;
            if (simA == null) return false;

            return Run(simA, b);
        }
        protected abstract bool Run(SimDescription a, SimDescription b);

        protected virtual bool PrivateRun(IMiniSimDescription a, MiniSimDescription b)
        {
            SimDescription simA = a as SimDescription;
            if (simA == null) return false;

            return Run(simA, b);
        }
        protected virtual bool Run(SimDescription a, MiniSimDescription b)
        {
            return false;
        }

        protected bool Test(IMiniSimDescription a, SimDescription b)
        {
            if (!AllowSpecies(a, b)) return false;

            return PrivateAllow(a, b);
        }
        protected bool Test(IMiniSimDescription a, MiniSimDescription b)
        {
            if (!AllowSpecies(a, b)) return false;

            return PrivateAllow(a, b);
        }

        protected virtual bool PrivateAllow(IMiniSimDescription a, SimDescription b)
        {
            SimDescription simA = a as SimDescription;
            if (simA == null) return false;

            return PrivateAllow(simA, b);
        }
        protected virtual bool PrivateAllow(SimDescription a, SimDescription b)
        {
            if (a == null) return false;

            if (b == null) return false;

            if (a == b) return false;

            return true;
        }
        protected virtual bool PrivateAllow(IMiniSimDescription a, MiniSimDescription b)
        {
            SimDescription simA = a as SimDescription;
            if (simA == null) return false;

            return PrivateAllow(simA, b);
        }
        protected virtual bool PrivateAllow(SimDescription a, MiniSimDescription b)
        {
            return false;
        }

        protected virtual bool AllowSpecies(IMiniSimDescription a, IMiniSimDescription b)
        {
            return SimTypes.IsEquivalentSpecies(a, b);
        }

        public override IEnumerable<CASAgeGenderFlags> GetSpeciesFilter()
        {
            if (mA != null)
            {
                return GetSpeciesFilterB();
            }
            else
            {
                return GetSpeciesFilterA();
            }
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (IsFirst) return true;

            foreach (IMiniSimDescription a in mA)
            {
                if (Test(a, me)) return true;
            }

            return false;
        }
        protected override bool PrivateAllow(MiniSimDescription me)
        {
            if (IsFirst) return true;

            foreach (IMiniSimDescription a in mA)
            {
                if (Test(a, me)) return true;
            }

            return false;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            bool okayed = true;

            Sim actorSim = parameters.mActor as Sim;

            if (parameters.mTarget is Sim)
            {
                mA = new List<IMiniSimDescription>();
                mA.Add((parameters.mTarget as Sim).SimDescription);
            }
            else
            {
                SimDescriptionObject simObject = parameters.mTarget as SimDescriptionObject;
                if (simObject != null)
                {
                    mA = new List<IMiniSimDescription>();

                    mA.Add(simObject.mSim);
                }
                else
                {
                    mA = null;

                    List<IMiniSimDescription> aMiniList = GetSelection(actorSim.SimDescription, GetTitleA(), GetCriteriaA(parameters), GetMaxSelectionA(), false, out okayed);
                    if (aMiniList != null)
                    {
                        // Must be set after filtering 
                        mA = new List<IMiniSimDescription>(aMiniList);
                    }
                }
            }

            if ((mA == null) || (mA.Count == 0))
            {
                if (okayed)
                {
                    return OptionResult.SuccessClose;
                }
                else
                {
                    return OptionResult.Failure;
                }
            }

            IMiniSimDescription sim = null;

            if (actorSim != null)
            {
                sim = actorSim.SimDescription;
            }

            if (mA.Count == 1)
            {
                sim = mA[0];
            }

            if (sim == null) return OptionResult.Failure;

            List<IMiniSimDescription> bList = new List<IMiniSimDescription>();

            List<IMiniSimDescription> bMiniList = GetSelection(sim, GetTitleB(), GetCriteriaB(parameters), GetMaxSelectionB(sim), false, out okayed);
            if (bMiniList != null)
            {
                foreach (IMiniSimDescription miniSim in bMiniList)
                {
                    bList.Add(miniSim);
                }
            }

            if (bList.Count == 0)
            {
                if (okayed)
                {
                    return OptionResult.SuccessClose;
                }
                else
                {
                    return OptionResult.Failure;
                }
            }

            bool askedAndAnswered = !CanApplyAll();

            foreach (IMiniSimDescription a in mA)
            {
                foreach (IMiniSimDescription b in bList)
                {
                    if (!Run(a, b)) return OptionResult.Failure;

                    if (!askedAndAnswered)
                    {
                        askedAndAnswered = true;

                        if ((mA.Count > 1) || (bList.Count > 1))
                        {
                            if (AcceptCancelDialog.Show(Common.Localize("SimInteraction:ApplyAll")))
                            {
                                ApplyAll = true;
                            }
                        }
                    }
                }
            }

            return OptionResult.SuccessClose;
        }
    }
}
