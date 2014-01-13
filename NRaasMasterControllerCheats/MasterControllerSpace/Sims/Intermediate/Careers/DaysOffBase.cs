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
    public abstract class DaysOffBase : CareerOption
    {
        protected int mValue = 0;

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected abstract Occupation GetOccupation(SimDescription me);

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (GetOccupation(me) == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Occupation occupation = GetOccupation(me);
            if (occupation == null) return false;

            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me, occupation.PaidDaysOff }), "0", 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                if (!int.TryParse(text, out mValue))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            occupation.mDaysOff += mValue;

            if (occupation.mDaysOff < 0)
            {
                occupation.mDaysOff = 0;
            }

            return true;
        }
    }
}
