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
    public abstract class StringOptionItem : GenericOptionItem<string>, IGenericAddOption<string>
    {
        public StringOptionItem(string value)
            : base(value, value)
        { }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        public string AddValue(string value)
        {
            SetValue(Value + value, true);
            return Value;
        }

        public override object PersistValue
        {
            set
            {
                SetValue (value as string);
            }
        }

        protected virtual string GetPrompt()
        {
            return Localize("Prompt");
        }

        protected virtual string Validate(string value)
        {
            return value;
        }

        protected override bool PrivatePerform()
        {
            string text = StringInputDialog.Show(Name, GetPrompt(), Value, 256, StringInputDialog.Validation.None);
            if (string.IsNullOrEmpty(text)) return false;

            SetValue(Validate(text));
            return true;
        }
    }
}
