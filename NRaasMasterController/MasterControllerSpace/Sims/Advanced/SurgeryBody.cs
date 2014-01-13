using NRaas.CommonSpace.Options;
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
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class SurgeryBody : CASBase, IAdvancedOption
    {
        public override string GetTitlePrefix()
        {
            return "BodySurgery";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        protected override CASMode GetMode(SimDescription sim, ref OutfitCategories startCategory, ref int startIndex, ref EditType editType)
        {
            return CASMode.PlasticSurgeryBody;
        }
    }
}
