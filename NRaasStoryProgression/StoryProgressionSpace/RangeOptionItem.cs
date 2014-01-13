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
    public abstract class RangeOptionItem : GenericOptionItem<Vector2>
    {
        Vector2 mMinMaxRange;

        public RangeOptionItem(Vector2 defValue, Vector2 minMaxRange)
            : base(defValue, defValue)
        {
            mMinMaxRange = minMaxRange;
        }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        public override string GetUIValue(bool pure)
        {
            string text = Value.x.ToString ("F1") + " : " + Value.y.ToString ("F1");

            if (!pure)
            {
                if (Value.Equals(Default))
                {
                    text = "(" + text + ")";
                }
            }

            return text;
        }

        public sealed override object PersistValue
        {
            set
            {
                if (value is string)
                {
                    string text = value as string;

                    string[] range = null;
                    if (text.StartsWith("(")) // EA format for Vector2
                    {
                        range = text.Replace("(", "").Replace(")", "").Split(',');
                    }
                    else
                    {
                        range = text.Split(':');
                    }

                    if (range.Length != 2) return;

                    float min;
                    if (!float.TryParse(range[0].Trim (), out min)) return;

                    float max;
                    if (!float.TryParse(range[1].Trim(), out max)) return;

                    SetValue (Adjust (min, max));
                }
                else
                {
                    SetValue ((Vector2)value);
                }
            }
        }

        protected Vector2 Adjust(float min, float max)
        {
            if (min > max)
            {
                float temp = min;
                min = max;
                max = temp;
            }

            min = Bound(min);
            max = Bound(max);

            return new Vector2 (min, max);
        }

        protected float Bound (float value)
        {
            if (value < mMinMaxRange.x)
            {
                return mMinMaxRange.x;
            }
            else if (value > mMinMaxRange.y)
            {
                return mMinMaxRange.y;
            }
            else
            {
                return value;
            }
        }

        protected override bool PrivatePerform()
        {
            string text = StringInputDialog.Show(Name, Localize("Prompt"), Value.x.ToString() + " : " + Value.y.ToString(), 256, StringInputDialog.Validation.None);
            if (text == null) return false;

            string[] range = text.Split(':');

            if (range.Length != 2)
            {
                SimpleMessageDialog.Show(Name, StoryProgression.Localize("Range:FormatError"));
                return false;
            }

            float min;
            if (!float.TryParse(range[0].Trim(), out min))
            {
                SimpleMessageDialog.Show(Name, StoryProgression.Localize("Range:MinError"));
                return false;
            }

            float max;
            if (!float.TryParse(range[1].Trim(), out max))
            {
                SimpleMessageDialog.Show(Name, StoryProgression.Localize("Range:MaxError"));
                return false;
            }

            SetValue (Adjust (min, max));
            return true;
        }
    }
}
