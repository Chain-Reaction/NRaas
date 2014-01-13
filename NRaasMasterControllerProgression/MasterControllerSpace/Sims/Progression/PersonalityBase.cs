extern alias SP;

using SimPersonality = SP::NRaas.StoryProgressionSpace.Personalities.SimPersonality;

using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Progression
{
    public abstract class PersonalityBase : SimFromList
    {    
        SimPersonality mPersonality;

        public SimPersonality Personality
        {
            set
            {
                mPersonality = value;
            }
        }

        public override string Name
        {
            get
            {
                return mPersonality.GetLocalizedName();
            }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected abstract bool IsLeader
        {
            get;
        }

        public override ObjectPickerDialogEx.CommonHeaderInfo<IMiniSimDescription> GetAuxillaryColumn()
        {
            return new ScoringColumn(mPersonality, IsLeader);
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return Allow(me, mPersonality);
        }

        protected abstract bool Allow(SimDescription me, SimPersonality personality);

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Run(me, mPersonality);
        }

        protected abstract bool Run(SimDescription me, SimPersonality personality);

        public class Item : ValueSettingOption<string>
        {
            public Item(SimPersonality personality)
                : base(personality.GetTitlePrefix(SP::NRaas.StoryProgressionSpace.ManagerProgressionBase.PrefixType.Pure), personality.GetLocalizedName(), personality.GetClanMembers(true).Count)
            { }
        }

        protected class ScoringColumn : ObjectPickerDialogEx.CommonHeaderInfo<IMiniSimDescription>
        {
            SimPersonality mPersonality;

            bool mLeader;

            public ScoringColumn(SimPersonality personality, bool leader)
                : base("NRaas.StoryProgression.SimID:ScoreTitle", "NRaas.StoryProgression.SimID:ScoreTooltip", 40)
            {
                mPersonality = personality;
                mLeader = leader;
            }

            public override ObjectPicker.ColumnInfo GetValue(IMiniSimDescription sim)
            {
                if (sim == null)
                {
                    return new ObjectPicker.TextColumn("");
                }
                else
                {
                    SP::NRaas.StoryProgressionSpace.Scenarios.SimScenarioFilter scoring = null;

                    if (mLeader)
                    {
                        scoring = mPersonality.CandidateScoring;
                    }
                    else
                    {
                        scoring = mPersonality.MemberRetention;
                    }

                    int score = 0;
                    if ((sim is SimDescription) && (scoring != null))
                    {
                        scoring.Score(sim as SimDescription, null, false, out score);
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
