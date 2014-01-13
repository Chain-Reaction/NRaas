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
    public abstract class IntegerOptionItem : GenericOptionItem<int>, IGenericAddOption<int>
    {
        public IntegerOptionItem(int value)
            : base(value, value)
        { }

        public int AddValue(int value)
        {
            SetValue(Value + value, true);
            return Value;
        }

        public override object PersistValue
        {
            set
            {
                if (value is string)
                {
                    int newValue;
                    if (!int.TryParse(value as string, out newValue)) return;

                    SetValue(newValue);
                }
                else
                {
                    SetValue ((int)value);
                }
            }
        }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        protected virtual string GetPrompt()
        {
            return Localize("Prompt");
        }

        protected virtual int Validate(int value)
        {
            return value;
        }

        protected override bool PrivatePerform()
        {
            string text = StringInputDialog.Show(Name, GetPrompt(), Value.ToString(), 256, StringInputDialog.Validation.None);
            if (string.IsNullOrEmpty(text)) return false;

            int iValue = 0;
            if (!int.TryParse(text, out iValue))
            {
                SimpleMessageDialog.Show(Name, StoryProgression.Localize("Integer:Error"));
                return false;
            }

            SetValue (Validate(iValue));
            return true;
        }
    }
}
