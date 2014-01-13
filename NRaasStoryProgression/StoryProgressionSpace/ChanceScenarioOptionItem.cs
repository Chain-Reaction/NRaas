using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios;
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
    public abstract class ChanceScenarioOptionItem<TManager,TScenario> : IntegerScenarioOptionItem<TManager,TScenario>
        where TManager : Manager
        where TScenario : Scenario, new()
    {
        int mTally;

        public ChanceScenarioOptionItem(int value)
            : base(value)
        { }

        public override string GetUIValue(bool pure)
        {            
            string text = base.GetUIValue(pure);

            if (Manager.DebuggingEnabled)
            {
                text += " [" + mTally + "]";
            }

            return text;
        }

        public sealed override object PersistValue
        {
            get
            {
                return base.PersistValue + "," + mTally;
            }
            set
            {
                if (value is string)
                {
                    string[] values = ((string)value).Split(',');

                    if (values.Length > 0)
                    {
                        SetValue(int.Parse(values[0]));
                    }

                    if (values.Length > 1)
                    {
                        mTally = int.Parse(values[1]);
                    }
                }
                else
                {
                    SetValue((int)value);
                }
            }
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if ((fullUpdate) && (!initialPass))
            {
                if (RandomUtil.RandomChance(mTally))
                {
                    base.PrivateUpdate(fullUpdate, initialPass);
                    mTally = 0;
                }
                else
                {
                    Manager.AddStat("Chance Fail " + Name, mTally);

                    mTally += Value;

                    if (mTally > 100)
                    {
                        mTally = 0;
                    }
                }

                // Save changes to Tally
                Persist();
            }
        }
    }
}
