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
    public class UpdateCurrentSimOperationEx : CASLogic.UpdateCurrentSimOperation
    {
        public UpdateCurrentSimOperationEx(bool bAllCategories)
            : base(bAllCategories)
        {}

        public override void Initialize()
        {
            try
            {
                mSimIndex = CASLogic.GetSingleton().PreviewSimIndex;
                AddStep(new UpdateCurrentSimStepEx(mbAllCategories));
                AddStep(new UpdateSingleThumbnailStepEx(mSimIndex));
            }
            catch (Exception e)
            {
                Common.Exception("Initialize", e);
            }
        }

        public override void OperationComplete()
        {
            try
            {
                base.OperationComplete();
            }
            catch (Exception e)
            {
                Common.Exception("OperationComplete", e);
            }
        }
    }
}
