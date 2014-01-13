using NRaas.MasterControllerSpace.Sims;
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
    public class SaveSimToWorldStepEx : CASLogic.SaveSimToWorldStep
    {
        public SaveSimToWorldStepEx()
        {}

        public override void Apply()
        {
            try
            {
                CASLogic logic = CASLogic.sSingleton;
                if ((logic.mSimDescEditExisting != null) && (logic.CASMode == CASMode.Mermaid))
                {
                    if (!(logic.mSimDescEditExisting.SupernaturalData is CASMermaidData))
                    {
                        logic.mSimDescEditExisting.mSupernaturalData = new CASMermaidData();
                    }
                }

                base.Apply();

                if (logic.CASMode == CASMode.Mermaid)
                {
                    CASBase.RecacheOutfits();
                }
                else
                {
                    CASBase.ApplyAllChanges();
                }
            }
            catch (Exception e)
            {
                Common.Exception("Apply", e);
            }
        }
    }
}
