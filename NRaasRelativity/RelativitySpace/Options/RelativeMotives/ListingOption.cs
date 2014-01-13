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

namespace NRaas.RelativitySpace.Options.RelativeMotives
{
    public class ListingOption : InteractionOptionList<IRelativeMotivesOption, GameObject>, IPrimaryOption<GameObject>, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "RelativeMotivesRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IRelativeMotivesOption> GetOptions()
        {
            List<IRelativeMotivesOption> results = new List<IRelativeMotivesOption>();

            foreach (CommodityKind kind in TuningAlterations.sCommodities)
            {
                results.Add(new RelativeMotive(kind));
            }

            return results;
        }

        public void Import(Persistence.Lookup settings)
        {
            Relativity.Settings.mRelativeMotives.Clear();
            foreach (CommodityKind kind in TuningAlterations.sCommodities)
            {
                bool relative = settings.GetBool(kind.ToString(), true);
                if (relative) continue;

                Relativity.Settings.mRelativeMotives[kind] = relative;
            }
        }

        public void Export(Persistence.Lookup settings)
        {
            foreach (KeyValuePair<CommodityKind, bool> kind in Relativity.Settings.mRelativeMotives)
            {
                settings.Add(kind.Key.ToString(), kind.Value);
            }
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }
    }
}
