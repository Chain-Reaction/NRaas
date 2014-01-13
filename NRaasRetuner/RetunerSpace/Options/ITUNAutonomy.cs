using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options
{
    public class ITUNAutonomy : EnumSettingOption<PersistedSettings.Autonomy, GameObject>, IPrimaryOption<GameObject>
    {
        protected override PersistedSettings.Autonomy Value
        {
            get
            {
                return Retuner.Settings.mInteractionAutonomy;
            }
            set
            {
                Retuner.Settings.mInteractionAutonomy = value;

                if (value == PersistedSettings.Autonomy.NoChange)
                {
                    Common.Notify(Common.Localize(GetTitlePrefix() + ":Restart"));
                }
                else
                {
                    Common.Notify(Common.Localize(GetTitlePrefix() + ":Reload"));
                }

                ApplyChanges();
            }
        }

        public override string GetTitlePrefix()
        {
            return "ITUNAutonomy";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override PersistedSettings.Autonomy Default
        {
            get { return PersistedSettings.Autonomy.NoChange; }
        }

        public static void ApplyChanges()
        {
            PersistedSettings.Autonomy autonomy = Retuner.Settings.mInteractionAutonomy;
            if (autonomy == PersistedSettings.Autonomy.NoChange) return;

            bool isAutonomous = (autonomy == PersistedSettings.Autonomy.Autonomous);

            foreach (InteractionTuning tuning in InteractionTuning.sAllTunings.Values)
            {
                if (isAutonomous)
                {
                    tuning.RemoveFlags(InteractionTuning.FlagField.DisallowAutonomous);
                }
                else
                {
                    tuning.AddFlags(InteractionTuning.FlagField.DisallowAutonomous);
                }
            }
        }
    }
}
