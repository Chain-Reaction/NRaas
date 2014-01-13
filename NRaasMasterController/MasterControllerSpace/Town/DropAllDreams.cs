using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class DropAllDreams : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "DropAllDreams";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (AcceptCancelDialog.Show(Common.Localize("DropAllDreams:Prompt")))
            {
                int count = 0;

                List<Sim> list = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
                foreach (Sim sim in list)
                {
                    if (sim.Household == Household.ActiveHousehold) continue;

                    if ((sim.DreamsAndPromisesManager != null) ||
                        (sim.OpportunityManager != null))
                    {
                        sim.NullSelectableSimManagers();

                        count++;
                    }
                }

                SimpleMessageDialog.Show(Name, Common.Localize("DropAllDreams:Result", false, new object[] { count }));
            }
            return OptionResult.SuccessClose;
        }
    }
}
