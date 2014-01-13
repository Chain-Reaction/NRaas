using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public enum NameTakeType : int
    {
        Husband = 0,
        Either = 1,
        Wife = 2,
        FemMale = 3,
        MaleFem = 4,
        None = 5,
        User = 6
    }

    public abstract class NameTakeBaseOption<TManager> : EnumBaseManagerOptionItem<TManager, NameTakeType>, INameTakeOption
        where TManager : StoryProgressionObject
    {
        public NameTakeBaseOption(NameTakeType defValue)
            : base(defValue, defValue)
        { }

        public string GetNameTakeLocalizationValueKey()
        {
            return GetLocalizationValueKey();
        }

        protected override string GetLocalizationValueKey()
        {
            return "NameTakeType";
        }

        protected override NameTakeType Convert(int value)
        {
            return (NameTakeType)value;
        }

        protected override NameTakeType Combine(NameTakeType original, NameTakeType add, out bool same)
        {
            NameTakeType result = original | add;

            same = (result == original);

            return result;
        }
    }
}
