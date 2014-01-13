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
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class SetSimDescOperationEx : CASLogic.SetSimDescOperation
    {
        public SetSimDescOperationEx(ResourceKey newSimDescKey)
            : base(newSimDescKey)
        {}
        public SetSimDescOperationEx(SimDescription newSimDesc, bool inHousehold)
            : base(newSimDesc, inHousehold)
        { }

        public override void Initialize()
        {
            try
            {
                if (mNewSimDescKey != ResourceKey.kInvalidResourceKey)
                {
                    AddStep(new SetSimDescStepEx(mNewSimDescKey));
                }
                else if (mNewSimDesc != null)
                {
                    AddStep(new SetSimDescStepEx(mNewSimDesc));
                    
                    OccultTypes occult = OccultTypes.None;
                    if (mNewSimDesc.SupernaturalData != null)
                    {
                        occult = mNewSimDesc.SupernaturalData.OccultType;
                    }
                    AddStep(new CASLogic.SetOccultTypeStep(occult, mInHousehold));
                }
            }
            catch (Exception e)
            {
                Common.Exception("Initialize", e);
            }
        }
    }
}
