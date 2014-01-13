using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
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
    public class Stylist : CASBase, ISimOption
    {
        EditType mEditType = EditType.None;

        OutfitCategories mStartCategory = OutfitCategories.None;

        public Stylist()
        { }
        public Stylist(EditType editType)
        {
            mEditType = editType;
        }
        public Stylist(EditType editType, OutfitCategories startCategory)
        {
            mEditType = editType;
            mStartCategory = startCategory;
        }

        public override string Name
        {
            get
            {
                return Common.LocalizeEAString("Gameplay/Roles/RoleStylist:StylistCareerTitle");
            }
        }

        public override string GetTitlePrefix()
        {
            return "Stylist";
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            return base.RunAll(sims);
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
                return CASMode.Stylist;
            }
            else if (sim.IsEP11Bot)
            {
                return CASMode.EditABot;
            }
            else if (sim.IsHuman)
            {
                return CASMode.Stylist;
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
                return CASMode.Stylist;
            }
        }
    }
}
