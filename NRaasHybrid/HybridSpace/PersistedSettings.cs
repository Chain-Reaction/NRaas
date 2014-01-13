using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to create a special outfit to use when a werewolf initially transforms")]
        protected static bool kSpecialWerewolfOutfit = true;

        [Tunable, TunableComment("The valid occult types for 'Banish Sim'")]
        protected static OccultTypes[] kValidOccultBanishSim = new OccultTypes[] { OccultTypes.Genie };

        [Tunable, TunableComment("The skill level at which 'Banish Sim' becomes available")]
        protected static int kSkillLevelBanishSim = 5;

        [Tunable, TunableComment("Whether to allow fairies, genies, etc to skate normally")]
        protected static bool kAllowOccultSkating = true;

        [Tunable, TunableComment("Whether to use the EA Standard two person animations for certain spell interactions")]
        protected static bool kEnforceTwoPersonAnimation = true;

        public bool mSpecialWerewolfOutfit = kSpecialWerewolfOutfit;

        public List<OccultTypes> mValidOccultBanishSim = new List<OccultTypes>(kValidOccultBanishSim);

        public int mSkillLevelBanishSim = kSkillLevelBanishSim;

        public bool mAllowOccultSkating = kAllowOccultSkating;

        public bool mDebugging = false;

        public bool mEnforceTwoPersonAnimation = kEnforceTwoPersonAnimation;

        Dictionary<Type, SpellSettings> mSettings = new Dictionary<Type, SpellSettings>();

        public PersistedSettings()
        {
            ApplySettings();
        }

        public bool Debugging
        {
            get
            {
                return mDebugging;
            }
            set
            {
                mDebugging = value;

                Common.kDebugging = value;
            }
        }

        public SpellSettings GetSettings(IMagicalDefinition definition)
        {
            SpellSettings settings;
            if (!mSettings.TryGetValue(definition.GetType(), out settings))
            {
                settings = definition.DefaultSettings;
                if (settings != null)
                {
                    mSettings.Add(definition.GetType(), settings);
                }
            }

            return settings;
        }

        public void ApplySettings()
        {
            foreach (IMagicalDefinition definition in Common.DerivativeSearch.Find<IMagicalDefinition>())
            {
                definition.SpellSettings = GetSettings(definition);
            }
        }

        [Persistable]
        public class SpellSettings
        {
            public OccultTypes mValidTypes;

            public int mMinSkillLevel;

            public float mMinMana;

            public int mOffIntendedLevel;

            public SpellSettings(OccultTypes validTypes, int minSkillLevel, float minMana, int offIntendedLevel)
            {
                mValidTypes = validTypes;
                mMinSkillLevel = minSkillLevel;
                mMinMana = minMana;
                mOffIntendedLevel = offIntendedLevel;
            }

            public int GetMinSkillLevel(bool offIntended)
            {
                if (offIntended)
                {
                    return (mMinSkillLevel + mOffIntendedLevel);
                }
                else
                {
                    return mMinSkillLevel;
                }
            }
        }

        public class Loadup : Common.IWorldLoadFinished
        {
            public void OnWorldLoadFinished()
            {
                Hybrid.Settings.ApplySettings();
            }
        }
    }
}
