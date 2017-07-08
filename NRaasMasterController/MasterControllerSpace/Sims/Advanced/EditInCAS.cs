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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class EditInCAS : CASBase, IAdvancedOption
    {
        bool mCAB = false;

        bool mAlwaysCAS = false;

        public EditInCAS()
        {
            mAlwaysCAS = true;
        }
        public EditInCAS(bool cab)
        {
            mCAB = cab;
        }

        public override string GetTitlePrefix()
        {
            return "CAS";
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            return base.PrivateAllow(me);
        }

        protected override CASMode GetMode(SimDescription sim, ref OutfitCategories startCategory, ref int startIndex, ref EditType editType)
        {
            if (!mAlwaysCAS)
            {
                if (mCAB)
                {
                    return CASMode.CreateABot;
                }
                else if (sim.IsEP11Bot)
                {
                    return CASMode.EditABot;
                }
            }

            // Intergration using EA Edit in CAS will bring the bot into the human CAS
            if (sim.IsEP11Bot)
            {
                return CASMode.EditABot;
            }

            return CASMode.Full;
        }
    }
}
