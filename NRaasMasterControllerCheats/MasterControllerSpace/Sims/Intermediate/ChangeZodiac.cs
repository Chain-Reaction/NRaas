using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class ChangeZodiac : SimFromList, IIntermediateOption
    {
        Zodiac mZodiac = Zodiac.Unset;

        public override string GetTitlePrefix()
        {
            return "Zodiac";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<ZodiacSign.Item> allOptions = new List<ZodiacSign.Item>();

                foreach (Zodiac zodiac in Enum.GetValues(typeof(Zodiac)))
                {
                    if (zodiac == Zodiac.Unset) continue;

                    allOptions.Add(new ZodiacSign.Item(zodiac));
                }

                ZodiacSign.Item choice = new CommonSelection<ZodiacSign.Item>(Name, me.FullName, allOptions).SelectSingle();
                if (choice == null) return false;

                mZodiac = choice.Value;
            }

            me.mZodiacSign = mZodiac;
            return true;
        }
    }
}
