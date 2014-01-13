using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public abstract class SimIDOption : GenericOptionItem<SimID>, INotExportableOption
    {
        [Persistable(false)]
        SimDescription mSim;

        public SimIDOption()
            : base (null, null)
        { }
        public SimIDOption(SimID value)
            : base (value, value)
        { }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        public override string GetUIValue(bool pure)
        {
            if (Value == null) return "";

            SimDescription sim = Value.SimDescription;
            if (sim == null) return "";

            return (sim.FullName);
        }

        public override object PersistValue
        {
            set
            {
                SetValue (value as SimID);
            }
        }

        public SimDescription SimDescription
        {
            get
            {
                if (Value == null)
                {
                    return null;
                }
                else
                {
                    if (mSim == null)
                    {
                        mSim = Value.SimDescription;
                    }

                    return mSim;
                }
            }
            set
            {
                if (value == null)
                {
                    SetValue (null);
                }
                else if (SimDescription != value)
                {
                    SetValue (new SimID(value));
                }

                mSim = value;
            }
        }

        public override bool ShouldDisplay ()
        {
            return false;
        }

        protected override bool PrivatePerform()
        {
            SimDescription sim = null;

            try
            {
                sim = GetSelection();
                if (sim != null)
                {
                    SimDescription = sim;
                    return true;
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
            return false;
        }

        protected virtual bool Allow(SimDescription me, SimScenarioFilter scoring)
        {
            if (!me.IsValidDescription) return false;

            return true;
        }

        protected abstract SimScenarioFilter GetScoring();

        protected SimDescription GetSelection()
        {
            if (Sim.ActiveActor == null) return null;

            SimSelection sims = SimSelection.Create(this);
            if (sims.IsEmpty)
            {
                SimpleMessageDialog.Show(Name, StoryProgression.Localize("SimID:Error"));
                return null;
            }

            return sims.SelectSingle();
        }

        public class SimSelection : ProtoSimSelection<SimDescription>
        {
            SimIDOption mOption;

            SimScenarioFilter mScoring;

            private SimSelection(SimIDOption option)
                : base(option.Name, null, true, false)
            {
                mOption = option;
                mScoring = option.GetScoring();

                AddColumn(new ScoringColumn(mScoring));
            }

            public static SimSelection Create(SimIDOption option)
            {
                SimSelection selection = new SimSelection(option);
                bool canceled;
                selection.FilterSims(new List<ICriteria>(), null, true, out canceled);
                return selection;
            }

            protected override bool Allow(SimDescription sim)
            {
                if (SimTypes.IsDead(sim)) return false;

                return mOption.Allow(sim, mScoring);
            }

            public class ScoringColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
            {
                SimScenarioFilter mScoring;

                public ScoringColumn(SimScenarioFilter scoring)
                    : base("NRaas.StoryProgression.SimID:ScoreTitle", "NRaas.StoryProgression.SimID:ScoreTooltip", 40)
                {
                    mScoring = scoring;
                }

                public override ObjectPicker.ColumnInfo GetValue(SimDescription sim)
                {
                    int score = 0;
                    if ((sim != null) && (mScoring != null))
                    {
                        mScoring.Score(sim, null, false, out score);
                    }

                    if (score >= 0)
                    {
                        return new ObjectPicker.TextColumn("+" + score.ToString("D4"));
                    }
                    else
                    {
                        return new ObjectPicker.TextColumn(score.ToString("D4"));
                    }
                }
            }
        }
    }
}
