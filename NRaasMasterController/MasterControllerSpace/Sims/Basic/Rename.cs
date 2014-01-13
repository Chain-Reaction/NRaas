using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class Rename : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "Rename";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            string text = StringInputDialog.Show(Name, Common.Localize ("Rename:FirstName"), me.FirstName, 256, StringInputDialog.Validation.None);
            if ((text == null) || (text == ""))
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Rename:Error"));
                return false;
            }
            me.FirstName = text;

            text = StringInputDialog.Show(Name, Common.Localize("Rename:LastName"), me.LastName, 256, StringInputDialog.Validation.None);
            if ((text == null) || (text == ""))
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Rename:Error"));
                return false;
            }
            me.LastName = text;

            if (me.Household == Household.ActiveHousehold)
            {
                HudModel hudModel = Sims3.UI.Responder.Instance.HudModel as HudModel;
                if ((me.CreatedSim != null) && (hudModel.FindSimInfo(me.CreatedSim.ObjectId) != null))
                {
                    hudModel.NotifyNameChanged(me.CreatedSim.ObjectId);
                }
            }

            return true;
        }
    }
}
