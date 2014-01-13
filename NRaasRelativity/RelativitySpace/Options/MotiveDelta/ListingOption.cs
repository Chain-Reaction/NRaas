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

namespace NRaas.RelativitySpace.Options.MotiveDelta
{
    public class ListingOption : InteractionOptionList<IMotiveDeltaOption, GameObject>, IPrimaryOption<GameObject>, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "MotiveDeltaRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IMotiveDeltaOption> GetOptions()
        {
            List<IMotiveDeltaOption> results = new List<IMotiveDeltaOption>();

            foreach (MotiveKeyValue key in Relativity.Settings.GetMotiveDeltaList())
            {
                results.Add(new MotiveDeltaFactor(key.mKey));
            }

            results.Add(new AddNewFactor.ListingOption());

            return results;
        }

        public void Import(Persistence.Lookup settings)
        {
            Relativity.Settings.ClearMotiveFactors();

            foreach (MotiveKeyValue key in settings.GetList<MotiveKeyValue>("Motives"))
            {
                Relativity.Settings.SetMotiveFactor(key.mKey, key.mValue);
            }
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Motives", Relativity.Settings.GetMotiveDeltaList());
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }
    }
}
