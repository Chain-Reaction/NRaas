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
    public class AgeAbsolute : AgeBase
    {
        public override string GetTitlePrefix()
        {
            return "AgeAbsolute";
        }

        protected override bool Run(IMiniSimDescription me, bool singleSelection)
        {
            if (!base.Run(me, singleSelection)) return false;

            int age, maxAge;
            GetAge(me, out age, out maxAge);

            age = mAge;
            if (me.Age != CASAgeGenderFlags.Elder)
            {
                if (mAge > maxAge)
                {
                    age = maxAge;
                }
            }

            AlterAge(me, age);
            return true;
        }
    }
}
