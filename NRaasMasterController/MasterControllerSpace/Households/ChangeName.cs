using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class ChangeName : HouseholdFromList, IHouseholdOption
    {
        protected override int GetMaxSelection()
        {
            return 0;
        }

        public override string GetTitlePrefix()
        {
            return "HouseholdName";
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            return (me != null);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            if (me == null) return OptionResult.Failure;

            string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt"), me.Name);
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            if (AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":SimsPrompt", false, new object[] { me.Name, text })))
            {
                foreach (SimDescription sim in CommonSpace.Helpers.Households.All(me))
                {
                    if (sim.LastName.Trim().ToLower() == me.Name.Trim().ToLower())
                    {
                        sim.LastName = text;

                        if (me == Household.ActiveHousehold)
                        {
                            HudModel hudModel = Sims3.UI.Responder.Instance.HudModel as HudModel;
                            if (sim.CreatedSim != null)
                            {
                                Household.AddDirtyNameSimID(sim.SimDescriptionId);
                                hudModel.NotifyNameChanged(sim.CreatedSim.ObjectId);
                            }
                        }
                    }
                }
            }

            me.Name = text;
            return OptionResult.SuccessClose;
        }
    }
}
