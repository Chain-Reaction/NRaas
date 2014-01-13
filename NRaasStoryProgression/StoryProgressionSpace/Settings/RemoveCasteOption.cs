using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
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
    public class RemoveCasteOption : OperationSettingOption<GameObject>, ICasteOption
    {
        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "RemoveCaste";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<Item> choices = new List<Item>();

            Sim sim = mTarget as Sim;
            if (sim != null)
            {
                IEnumerable<CasteOptions> castes = StoryProgression.Main.Options.GetSim(sim).Castes;
                if (castes != null)
                {
                    foreach (CasteOptions option in castes)
                    {
                        choices.Add(new Item(option));
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
                        Dictionary<CasteOptions, bool> lookup = new Dictionary<CasteOptions, bool>();

                        foreach (SimDescription desc in Households.All(lot.Household))
                        {
                            IEnumerable<CasteOptions> castes = StoryProgression.Main.Options.GetSim(desc).Castes;
                            if (castes != null)
                            {
                                foreach (CasteOptions option in castes)
                                {
                                    if (lookup.ContainsKey(option)) continue;
                                    lookup.Add(option, true);

                                    choices.Add(new Item(option));
                                }
                            }
                        }
                    }
                }
                else if (Common.IsRootMenuObject(mTarget))
                {
                    foreach (CasteOptions option in StoryProgression.Main.Options.AllCastes)
                    {
                        choices.Add(new Item(option));
                    }
                }
            }

            CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, choices).SelectMultiple();
            if ((selection == null) || (selection.Count == 0)) return OptionResult.Failure;

            if (sim != null)
            {
                SimData data = StoryProgression.Main.Options.GetSim(sim);

                foreach (Item item in selection)
                {
                    data.RemoveValue<ManualCasteOption,CasteOptions>(item.Value);
                }
            }
            else
            {
                Lot lot = mTarget as Lot;
                if (lot != null)
                {
                    if (lot.Household != null)
                    {
                        Dictionary<CasteOptions, bool> lookup = new Dictionary<CasteOptions, bool>();

                        foreach (SimDescription desc in Households.All(lot.Household))
                        {
                            SimData data = StoryProgression.Main.Options.GetSim(desc);

                            foreach (Item item in selection)
                            {
                                data.RemoveValue<ManualCasteOption, CasteOptions>(item.Value);
                            }
                        }
                    }
                }
                else if (Common.IsRootMenuObject(mTarget))
                {
                    foreach (Item item in selection)
                    {
                        StoryProgression.Main.Options.RemoveCaste(item.Value);
                    }
                }
            }

            return OptionResult.SuccessRetain;
        }

        public class Item : ValueSettingOption<CasteOptions>
        {
            public Item(CasteOptions option)
                : base(option, option.Name, 0)
            { }
        }
    }
}
