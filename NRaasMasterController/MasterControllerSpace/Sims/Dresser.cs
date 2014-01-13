using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public class Dresser : CASBase, ISimOption
    {
        EditType mEditType = EditType.None;

        OutfitCategories mStartCategory = OutfitCategories.None;

        public Dresser()
        { }
        public Dresser(EditType editType, OutfitCategories startCategory)
        {
            mEditType = editType;
            mStartCategory = startCategory;
        }

        public override string GetTitlePrefix()
        {
            return "Dresser";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Common.kDebugging)
            {
                if (GameUtils.IsInstalled(ProductVersion.EP2)) return false;
            }

            return base.Allow(parameters);
        }

        protected override CASMode GetMode(SimDescription sim, ref OutfitCategories startCategory, ref int startIndex, ref EditType editType)
        {
            editType = mEditType;

            if (mStartCategory != OutfitCategories.None)
            {
                startCategory = mStartCategory;
                startIndex = 0;
            }

            if (sim == null) 
            {
                return CASMode.Dresser;
            }
            else if (sim.IsEP11Bot)
            {
                return CASMode.EditABot;
            } 
            else if (sim.IsHuman)
            {
                return CASMode.Dresser;
            }
            else if (sim.IsHorse)
            {
                return CASMode.Tack;
            }
            else if (sim.IsCat || sim.IsADogSpecies)
            {
                return CASMode.Collar;
            }
            else
            {
                return CASMode.Dresser;
            }
        }
    }
}
