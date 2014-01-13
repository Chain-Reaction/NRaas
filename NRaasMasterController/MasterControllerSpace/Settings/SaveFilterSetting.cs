using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class SaveFilterSetting : FilterSettingOption, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "SaveFilterSetting";
        }

        public void Import(Persistence.Lookup settings)
        {
            MasterController.Settings.mFilters = settings.GetList<SavedFilter>("");
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("", MasterController.Settings.mFilters);
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            SimSelection.CriteriaSelection.Results uncheckedCriteria = new SimSelection.CriteriaSelection(Name, SelectionCriteria.SelectionOption.List).SelectMultiple(20);
            if (uncheckedCriteria.Count == 0)
            {
                if (uncheckedCriteria.mOkayed)
                {
                    return OptionResult.SuccessClose;
                }
                else
                {
                    return OptionResult.Failure;
                }
            }

            bool showSpecial = false;
            foreach (SimSelection.ICriteria crit in uncheckedCriteria)
            {
                if (crit is SimTypeOr)
                {
                    showSpecial = true;
                    break;
                }
            }

            Sim sim = parameters.mActor as Sim;

            List<IMiniSimDescription> simsList = new List<IMiniSimDescription>();
            foreach (List<IMiniSimDescription> sims in SimListing.AllSims<IMiniSimDescription>(sim.SimDescription, showSpecial).Values)
            {
                if (!showSpecial)
                {
                    sims.RemoveAll((e) => { return SimSelection.IsSpecial(e); });
                }

                simsList.AddRange(sims);
            }

            List<SimSelection.ICriteria> criteria = new List<SimSelection.ICriteria> ();

            foreach (SimSelection.ICriteria crit in uncheckedCriteria)
            {
                // Update changes the sims list, so we need a new copy for each call
                List<IMiniSimDescription> newList = new List<IMiniSimDescription>(simsList);
                if (crit.Update(sim.SimDescription, uncheckedCriteria, newList, false) != SimSelection.UpdateResult.Failure)
                {
                    criteria.Add(crit);
                }
            }

            string name = null;

            while (true)
            {
                name = StringInputDialog.Show(Name, Common.Localize("SaveFilterSetting:Prompt"), name, 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(name))
                {
                    return OptionResult.Failure;
                }

                if (Find(name) == null)
                {
                    break;
                }
                else if (AcceptCancelDialog.Show(Common.Localize("SaveFilterSetting:Exists")))
                {
                    Delete(name);
                    break;
                }
            }
            
            NRaas.MasterController.Settings.mFilters.Add (new SavedFilter(name, criteria));

            SimpleMessageDialog.Show(Name, Common.Localize("SaveFilterSetting:Success"));
            return OptionResult.SuccessRetain;
        }
    }
}
