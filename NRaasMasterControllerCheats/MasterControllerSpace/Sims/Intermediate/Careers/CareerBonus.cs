using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class CareerBonus : CareerOption
    {
        float mValue = 0;

        public override string GetTitlePrefix()
        {
            return "CareerBonus";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (!me.TeenOrAbove) return false;

            return (me.Occupation is Career);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Career career = me.Occupation as Career;
            if (career == null) return true;

            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("CareerBonus:Prompt", me.IsFemale, new object[] { me, career.mPayPerHourExtra }), "0", 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                if (!float.TryParse(text, out mValue))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            career.mPayPerHourExtra += mValue;

            if (career.mPayPerHourExtra < 0)
            {
                career.mPayPerHourExtra = 0;
            }

            return true;
        }
    }
}
