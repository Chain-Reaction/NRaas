using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.CAS.CAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class UpdateCurrentSimStepEx : CASLogic.UpdateCurrentSimStep
    {
        public UpdateCurrentSimStepEx(bool bAllCategories)
            : base(bAllCategories)
        {}

        public override void Apply()
        {
            try
            {
                base.Apply();

                Sims.CASBase.ApplyAllChanges();
            }
            catch (Exception e)
            {
                Common.Exception("Apply", e);
            }
        }
    }
}
