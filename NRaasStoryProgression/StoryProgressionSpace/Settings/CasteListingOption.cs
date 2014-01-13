using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class CasteListingOption : InteractionOptionList<ICasteOption, GameObject>, IPrimaryOption<GameObject>
    {
        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "CasteOptions";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;

            return base.Allow(parameters);
        }

        public override List<ICasteOption> GetOptions()
        {
            List<ICasteOption> results = new List<ICasteOption>();

            results.Add(new AddCasteOption());
            results.Add(new RemoveCasteOption());

            Dictionary<CasteOptions, bool> casteOptions = new Dictionary<CasteOptions, bool>();

            Sim sim = mTarget as Sim;
            if (sim != null)
            {
                IEnumerable<CasteOptions> castes = StoryProgression.Main.Options.GetSim(sim).Castes;
                if (castes != null)
                {
                    foreach (CasteOptions option in castes)
                    {
                        casteOptions[option] = true;
                    }

                    SimDescription head = SimTypes.HeadOfFamily(sim.Household);
                    if (head != null)
                    {
                        castes = StoryProgression.Main.Options.GetSim(head).Castes;
                        if (castes != null)
                        {
                            foreach (CasteOptions option in castes)
                            {
                                if (option.GetValue<CasteApplyToHouseOption, bool>())
                                {
                                    casteOptions[option] = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Lot lot = mTarget as Lot;
                if (lot != null)
                {
                    if (lot.Household != null)
                    {
                        foreach (SimDescription desc in Households.All(lot.Household))
                        {
                            IEnumerable<CasteOptions> castes = StoryProgression.Main.Options.GetSim(desc).Castes;
                            if (castes != null)
                            {
                                foreach (CasteOptions option in castes)
                                {
                                    casteOptions[option] = true;
                                }
                            }
                        }
                    }
                }
                else if (Common.IsRootMenuObject(mTarget))
                {
                    foreach (CasteOptions option in StoryProgression.Main.Options.AllCastes)
                    {
                        casteOptions[option] = true;
                    }
                }
            }

            foreach (CasteOptions option in casteOptions.Keys)
            {
                results.Add(new ChangeCasteOptions(option));
            }

            return results;
        }
    }
}
