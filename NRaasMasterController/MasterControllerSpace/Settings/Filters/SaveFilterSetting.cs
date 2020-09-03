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

namespace NRaas.MasterControllerSpace.Settings.Filters
{
    public class SaveFilterSetting : FilterSettingOption, IPersistence
    {
        string mCallingMod = string.Empty;

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

        public OptionResult RunExternal(string callingMod, List<string> forbiddenCrit)
        {
            this.mCallingMod = callingMod;
            base.mForbiddenCrit = forbiddenCrit;

            if (Sim.ActiveActor == null) return OptionResult.Failure;
            return this.Run(new GameHitParameters<GameObject>(Sim.ActiveActor, Sim.ActiveActor, GameObjectHit.NoHit));
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<SimSelection.ICriteria> criteria = this.RunCriteriaSelection(parameters, 20, true);

            if (criteria.Count == 0) return OptionResult.Failure;

            string name = null;

            while (true)
            {
                name = StringInputDialog.Show(Name, Common.Localize("SaveFilterSetting:Prompt"), name, 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(name))
                {
                    return OptionResult.Failure;
                }

                if (mCallingMod != string.Empty)
                {
                    name = mCallingMod + "." + name;
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

            NRaas.MasterController.Settings.mFilters.Add(new SavedFilter(name, criteria));

            SimpleMessageDialog.Show(Name, Common.Localize("SaveFilterSetting:Success"));
            return OptionResult.SuccessRetain;
        }
    }
}
