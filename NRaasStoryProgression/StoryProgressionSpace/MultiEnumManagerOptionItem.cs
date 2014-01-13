using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public abstract class MultiEnumManagerOptionItem<TManager, TType> : MultiListedManagerOptionItem<TManager,TType>, IGeneralOption
        where TManager : StoryProgressionObject
        where TType : struct
    {
        public MultiEnumManagerOptionItem(TType[] value)
            : base(value)
        { }
        public MultiEnumManagerOptionItem(List<TType> value)
            : base(value)
        { }

        protected override bool PersistCreate(ref TType defValue, string value)
        {
            return ParserFunctions.TryParseEnum<TType>(value, out defValue, defValue);
        }

        protected virtual bool Allow(TType value)
        {
            return true;
        }

        protected override List<IGenericValueOption<TType>> GetAllOptions()
        {
            List<IGenericValueOption<TType>> results = new List<IGenericValueOption<TType>>();

            foreach(TType value in Enum.GetValues(typeof(TType)))
            {
                if (!Allow(value)) continue;

                results.Add(new EnumListItem(this, value));
            }

            return results;
        }

        public class EnumListItem : BaseListItem<MultiEnumManagerOptionItem<TManager, TType>>
        {
            public EnumListItem(MultiEnumManagerOptionItem<TManager, TType> option, TType value)
                : base(option, value)
            { }
        }
    }
}
