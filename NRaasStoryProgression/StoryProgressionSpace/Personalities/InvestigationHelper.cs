using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class InvestigationHelper
    {
        string mStoryName = null;

        IntegerOption.OptionValue mMinimum;
        IntegerOption.OptionValue mMaximum;

        public InvestigationHelper()
        { }

        public override string ToString()
        {
            string text = "InvestigateStoryName=" + mStoryName;

            text += Common.NewLine + "InvestigateMinimum=" + mMinimum;
            text += Common.NewLine + "InvestigateMaximum=" + mMaximum;

            return text;
        }

        public int GetMinimum(int defValue)
        {
            if (mMinimum != null)
            {
                return mMinimum.Value;
            }
            else
            {
                return defValue;
            }
        }

        public int GetMaximum(int defValue)
        {
            if (mMaximum != null)
            {
                return mMaximum.Value;
            }
            else
            {
                return defValue;
            }
        }

        public string GetStoryName(string defName)
        {
            if (!string.IsNullOrEmpty(mStoryName))
            {
                return mStoryName;
            }
            else
            {
                return defName;
            }
        }

        public bool Parse(XmlDbRow row, StoryProgressionObject manager, IUpdateManager updater, ref string error)
        {
            mStoryName = row.GetString("InvestigateStoryName");

            mMinimum = new IntegerOption.OptionValue();
            if (!mMinimum.Parse(row, "InvestigateMinimum", manager, updater, ref error))
            {
                mMinimum = null;

                error = null;
                //return false;
            }

            mMaximum = new IntegerOption.OptionValue();
            if (!mMaximum.Parse(row, "InvestigateMaximum", manager, updater, ref error))
            {
                mMaximum = null;

                error = null;
                //return false;
            }

            return true;
        }
    }
}
