using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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
    public abstract class PerformanceBase : CareerOption
    {
        float mValue = 0;

        public PerformanceBase()
        {}

        protected abstract float GetValue (SimDescription me);

        protected abstract void SetValue(SimDescription me, float value);

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("Performance:Prompt"), GetValue(me).ToString(), 256, StringInputDialog.Validation.None);
                if ((text == null) || (text == "")) return false;

                if (!float.TryParse(text, out mValue))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }

                if (mValue > 100f)
                {
                    mValue = 100f;
                }
                else if (mValue < -100f)
                {
                    mValue = -100f;
                }
            }

            SetValue (me, mValue);
            return true;
        }
    }
}
