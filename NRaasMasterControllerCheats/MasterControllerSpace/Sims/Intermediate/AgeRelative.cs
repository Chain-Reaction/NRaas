using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class AgeRelative : AgeBase
    {
        public override string GetTitlePrefix()
        {
            return "AgeRelative";
        }

        protected override bool Run(IMiniSimDescription me, bool singleSelection)
        {
            if (!base.Run(me, singleSelection)) return false;

            int age, maxAge;
            GetAge(me, out age, out maxAge);

            age += mAge;

            if (age <= 0)
            {
                age = 0;
            }
            else if ((me.Age != CASAgeGenderFlags.Elder) && (age > maxAge))
            {
                age = maxAge;
            }

            AlterAge(me, age);
            return true;
        }
    }
}
