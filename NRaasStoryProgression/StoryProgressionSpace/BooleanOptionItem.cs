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
    public abstract class BooleanOptionItem : GenericOptionItem<bool>
    {
        public BooleanOptionItem(bool value)
            : base(value, value)
        { }

        public sealed override object PersistValue
        {
            set
            {
                if (value is string)
                {
                    bool newValue;
                    if (!bool.TryParse(value as string, out newValue)) return;

                    SetValue(newValue);
                }
                else
                {
                    SetValue((bool)value);
                }
            }
        }

        protected override string GetLocalizationValueKey()
        {
            return "Boolean";
        }

        protected override bool PrivatePerform()
        {
            SetValue(!Value);
            return true;
        }
    }
}
