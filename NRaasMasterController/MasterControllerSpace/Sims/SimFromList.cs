using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Dialogs;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public abstract class SimFromList : OptionItem, IInteractionOptionItem<IActor,SimDescriptionObject,GameHitParameters<SimDescriptionObject>>
    {
        bool mApplyAll = false;

        bool mAskedAndAnswered = false;

        protected virtual bool AllowRunOnActive
        {
            get { return true; }
        }

        protected bool ApplyAll
        {
            get
            {
                return mApplyAll;
            }
            set
            {
                mApplyAll = value;
            }
        }

        protected virtual bool AllSimsOnFilterCancel
        {
            get { return false; }
        }

        protected virtual OptionResult RunResult
        {
            get { return OptionResult.SuccessClose; }
        }

        public override void Reset()
        {
            base.Reset();

            mApplyAll = false;
            mAskedAndAnswered = false;
        }

        protected virtual bool AutoApplyAll()
        {
            return false;
        }

        protected virtual bool CanApplyAll ()
        {
            return false;
        }

        protected virtual bool PromptToApplyAll()
        {
            return true;
        }

        protected virtual int GetMaxSelection()
        {
            return 1;
        }

        public virtual ObjectPickerDialogEx.CommonHeaderInfo<IMiniSimDescription> GetAuxillaryColumn()
        {
            return null;
        }

        public bool Test(GameHitParameters<SimDescriptionObject> parameters)
        {
            return Test(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!base.Allow(parameters)) return false;

            Sim sim = parameters.mTarget as Sim;
            if (sim != null)
            {
                if ((sim != parameters.mActor) || (AllowRunOnActive))
                {
                    if (!Test(sim.SimDescription)) return false;
                }
            }
            else
            {
                SimDescriptionObject simObject = parameters.mTarget as SimDescriptionObject;
                if (simObject != null)
                {
                    if (!Test(simObject.mSim)) return false;
                }
                else
                {
                    Lot lot = GetLot(parameters.mTarget, parameters.mHit);
                    if (lot != null)
                    {
                        if (lot.Household != null)
                        {
                            bool success = false;
                            foreach (SimDescription member in CommonSpace.Helpers.Households.All(lot.Household))
                            {
                                if (Test(member))
                                {
                                    success = true;
                                    break;
                                }
                            }

                            if (!success) return false;
                        }
                    }
                }
            }

            return true;
        }

        protected virtual bool TestValid
        {
            get { return true; }
        }

        protected virtual bool ShowProgress
        {
            get { return false; }
        }

        protected bool Test(SimDescription me)
        {
            if (!AllowSpecies(me)) return false;

            return PrivateAllow(me);
        }
        protected bool Test(MiniSimDescription me)
        {
            if (!AllowSpecies(me)) return false;

            return PrivateAllow(me);
        }

        protected virtual bool PrivateAllow(SimDescription me)
        {
            if ((TestValid) && (!me.IsValidDescription)) return false;

            return true;
        }

        protected virtual bool PrivateAllow(MiniSimDescription me)
        {
            return false;
        }

        protected virtual bool AllowSpecies(IMiniSimDescription me)
        {
            return true;
        }

        public virtual IEnumerable<CASAgeGenderFlags> GetSpeciesFilter()
        {
            return MasterController.Settings.mDefaultSpecies;
        }

        public bool Test(IMiniSimDescription me)
        {
            if (me is SimDescription)
            {
                return Test(me as SimDescription);
            }
            else if (me is MiniSimDescription)
            {
                return Test(me as MiniSimDescription);
            }
            else
            {
                return false;
            }
        }

        protected virtual List<SimSelection.ICriteria> GetCriteria(GameHitParameters<GameObject> parameters)
        {
            Lot lot = GetLot(parameters.mTarget, parameters.mHit);
            if (lot != null)
            {
                if (lot.Household != null)
                {
                    if ((!GameStates.IsOnVacation) || (lot.Household != Household.ActiveHousehold))
                    {
                        List<SimSelection.ICriteria> criteria = new List<SimSelection.ICriteria>();
                        criteria.Add(new LotAndFamily(lot));

                        return criteria;
                    }
                }
                else
                {
                    List<SimSelection.ICriteria> criteria = new List<SimSelection.ICriteria>();
                    criteria.Add(new CurrentLot(lot));

                    return criteria;
                }
            }

            return SelectionCriteria.SelectionOption.List;
        }

        protected virtual bool Run(IMiniSimDescription me, bool singleSelection)
        {
            try
            {
                if (me is SimDescription)
                {
                    return Run(me as SimDescription, singleSelection);
                }
                else if (me is MiniSimDescription)
                {
                    return Run(me as MiniSimDescription, singleSelection);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Common.Exception(me as SimDescription, e);
                return false;
            }
        }

        public virtual IEnumerable<SimSelection.ICriteria> AlterCriteria(IEnumerable<SimSelection.ICriteria> allCriteria, bool manual, bool canceled)
        {
            return allCriteria;
        }

        public OptionResult Perform(GameHitParameters<SimDescriptionObject> parameters)
        {
            return Perform(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        protected virtual bool Run(SimDescription me, bool singleSelection)
        {
            return false;
        }

        protected virtual bool Run(MiniSimDescription me, bool singleSelection)
        {
            return false;
        }

        protected virtual OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            bool applyAll = ApplyAll || AutoApplyAll();

            ApplyAll = false;

            bool singleSelection = (sims.Count == 1);

            try
            {
                bool progressDisplayed = false;

                foreach (IMiniSimDescription sim in sims)
                {
                    if ((ApplyAll) && (ShowProgress) && (!progressDisplayed))
                    {
                        ProgressDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/Global:Processing", new object[0x0]), false);
                        progressDisplayed = true;
                    }

                    if (!Run(sim, singleSelection)) return OptionResult.Failure;

                    if (applyAll)
                    {
                        ApplyAll = true;
                    }
                    else if (!mAskedAndAnswered)
                    {
                        mAskedAndAnswered = true;

                        if (sims.Count > 1)
                        {
                            if (AcceptCancelDialog.Show(Common.Localize("SimInteraction:ApplyAll")))
                            {
                                ApplyAll = true;
                            }
                        }
                    }
                }
            }
            finally
            {
                ProgressDialog.Close();
            }

            return RunResult;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            mAskedAndAnswered = !CanApplyAll();

            List<IMiniSimDescription> sims = null;

            Sim target = parameters.mTarget as Sim;

            bool okayed = true;

            if ((target != null) && ((target != parameters.mActor) || (AllowRunOnActive)))
            {
                sims = new List<IMiniSimDescription>();

                sims.Add (target.SimDescription);
            }
            else
            {
                SimDescriptionObject simObject = parameters.mTarget as SimDescriptionObject;
                if (simObject != null)
                {
                    sims = new List<IMiniSimDescription>();

                    sims.Add(simObject.mSim);
                }
                else
                {
                    Sim actorSim = parameters.mActor as Sim;

                    sims = GetSelection(actorSim.SimDescription, Name, GetCriteria(parameters), GetMaxSelection(), CanApplyAll(), out okayed);
                }
            }

            if ((sims != null) && (okayed))
            {
                return RunAll(sims);
            }
            return OptionResult.Failure;
        }

        protected bool PerformApplyAll(string title, ICollection<IMiniSimDescription> sims, List<IMiniSimDescription> results)
        {
            if ((!PromptToApplyAll()) || (AcceptCancelDialog.Show(Common.Localize("SimSelection:Prompt", false, new object[] { title, sims.Count }))))
            {
                results.AddRange(sims);
                return true;
            }

            return false;
        }

        public List<IMiniSimDescription> GetSelection(IMiniSimDescription me, string title, ICollection<SimSelection.ICriteria> criteria, int maxSelection, bool canApplyAll, out bool okayed)
        {
            okayed = false;

            bool criteriaCanceled;
            SimSelection sims = SimSelection.Create(title, me, this, criteria, true, canApplyAll, out criteriaCanceled);
            if (sims.IsEmpty)
            {
                SimpleMessageDialog.Show(title, Common.Localize("SimSelection:NoChoices"));
                return null;
            }

            okayed = true;

            List<IMiniSimDescription> results = new List<IMiniSimDescription>();

            if (sims.All.Count == 1)
            {
                results.AddRange(sims.All);
            }
            else
            {
                if ((ApplyAll) || ((AllSimsOnFilterCancel) && (criteriaCanceled)))
                {
                    if (!PerformApplyAll(title, sims.All, results))
                    {
                        okayed = false;
                    }

                    mAskedAndAnswered = true;
                }
                else
                {
                    SimSelection.Results choices = sims.SelectMultiple(maxSelection);

                    okayed = choices.mOkayed;
                    results.AddRange (choices);

                    if ((results.Count == 0) && (okayed) && (sims.All.Count > 1) && (maxSelection <= sims.All.Count))
                    {
                        if (!PerformApplyAll(title, sims.All, results))
                        {
                            okayed = false;
                        }
                    }

                    if ((canApplyAll) && (results.Count == sims.All.Count))
                    {
                        mAskedAndAnswered = true;
                        mApplyAll = true;
                    }
                }
            }

            return results;
        }
    }
}
