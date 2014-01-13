using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.CAS;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class CopyGenetics : DualSimFromList, IAdvancedOption
    {
        public enum Style
        {
            Full,
            NonZero,
            NonZeroOnlySliders,
            OnlySliders,
        }

        Style mStyle;

        public override string GetTitlePrefix()
        {
            return "CopyGenetics";
        }

        protected override int GetMaxSelectionB(IMiniSimDescription sim)
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override string GetTitleA()
        {
            return Common.Localize("Copy:Source");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("Copy:Destination");
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.GetOutfit(OutfitCategories.Everyday, 0) == null) return false;

            //if (SimTypes.IsSkinJob(me)) return false;

            if (me.Household == null) return false;

            if (me.Household == Household.ActiveHousehold) return true;

            return (!SimTypes.IsTourist(me));
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            if (!ApplyAll)
            {
                List<Item> choices = new List<Item>();

                foreach(Style style in Enum.GetValues(typeof(Style)))
                {
                    choices.Add(new Item (style));
                }

                Item choice = new CommonSelection<Item>(Name, choices).SelectSingle();
                if (choice == null) return false;

                mStyle = choice.Value;
            }

            bool onlyNonZero = false;
            bool onlySliders = false;

            switch (mStyle)
            {
                case Style.NonZero:
                    onlyNonZero = true;
                    onlySliders = false;
                    break;
                case Style.NonZeroOnlySliders:
                    onlyNonZero = true;
                    onlySliders = true;
                    break;
                case Style.OnlySliders:
                    onlyNonZero = false;
                    onlySliders = true;
                    break;
            }

            FacialBlends.CopyGenetics(a, b, onlyNonZero, onlySliders);

            new SavedOutfit.Cache(b).PropagateGenetics(b, CASParts.sPrimary);

            return true;
        }

        public class Item : ValueSettingOption<Style>
        {
            public Item(Style value)
                : base(value, Common.Localize("CopyGenetics:" + value), -1)
            { }
        }
    }
}
