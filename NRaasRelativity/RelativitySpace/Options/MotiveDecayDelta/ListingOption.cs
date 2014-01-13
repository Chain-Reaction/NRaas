using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RelativitySpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RelativitySpace.Options.MotiveDecayDelta
{
    public class ListingOption : InteractionOptionList<IMotiveDecayDeltaOption, GameObject>, IPrimaryOption<GameObject>, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "MotiveDecayDeltaRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IMotiveDecayDeltaOption> GetOptions()
        {
            List<IMotiveDecayDeltaOption> results = new List<IMotiveDecayDeltaOption>();

            foreach (MotiveKeyValue key in Relativity.Settings.GetMotiveDecayList())
            {
                results.Add(new MotiveDecayDeltaFactor(key.mKey));
            }

            results.Add(new AddNewFactor.ListingOption());

            return results;
        }

        public void Import(Persistence.Lookup settings)
        {
            Relativity.Settings.ClearMotiveDecayFactors();

            foreach (MotiveKeyValue key in settings.GetList<MotiveKeyValue>("Motives"))
            {
                Relativity.Settings.SetMotiveDecayFactor(key.mKey, key.mValue);
            }
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Motives", Relativity.Settings.GetMotiveDecayList());
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }
    }
}
