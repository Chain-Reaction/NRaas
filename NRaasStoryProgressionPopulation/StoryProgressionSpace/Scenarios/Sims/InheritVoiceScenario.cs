using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class InheritVoiceScenario : AgeUpBaseScenario
    {
        public InheritVoiceScenario()
        { }
        public InheritVoiceScenario(SimDescription sim)
            : base (sim)
        { }
        protected InheritVoiceScenario(InheritVoiceScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "InheritVoice";
        }
        
        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.YoungAdultOrAbove)
            {
                IncStat("Too Old");
                return false;
            }

            Genealogy childGenealogy = sim.Genealogy;
            if ((childGenealogy == null) || (childGenealogy.Parents.Count == 0))
            {
                IncStat("No Parents");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            Genealogy childGenealogy = Sim.Genealogy;
            if ((childGenealogy == null) || (childGenealogy.Parents.Count == 0))
            {
                return false;
            }

            SimDescription mom = null, dad = null;
            Relationships.GetParents(Sim, out mom, out dad);

            return PrivatePerform(this, mom, dad, false);
        }

        public bool Run(Common.IStatGenerator stats, Manager manager, SimDescription mom, SimDescription dad)
        {
            if (Sim == null) return false;

            Manager = manager;

            return PrivatePerform(stats, mom, dad, true);
        }

        protected bool PrivatePerform(Common.IStatGenerator stats, SimDescription mom, SimDescription dad, bool forcePitch)
        {
            VoiceVariationType voice = VoiceVariationType.A;

            if (Sim.Child)
            {
                if (RandomUtil.CoinFlip())
                {
                    voice = VoiceVariationType.B;
                }

                stats.IncStat("Voice Child Random");
            }
            else if (Sim.TeenOrAbove)
            {
                if (RandomUtil.CoinFlip())
                {
                    if (dad != null)
                    {
                        voice = dad.VoiceVariation;

                        stats.IncStat("Voice From Dad");
                    }
                    else if (mom != null)
                    {
                        voice = mom.VoiceVariation;

                        stats.IncStat("Voice From Mom");
                    }
                    else
                    {
                        voice = unchecked((VoiceVariationType)RandomUtil.GetInt(0, 2));

                        stats.IncStat("Voice Teen Random");
                    }
                }
                else
                {
                    if (mom != null)
                    {
                        voice = mom.VoiceVariation;

                        stats.IncStat("Voice From Mom");
                    }
                    else if (dad != null)
                    {
                        voice = dad.VoiceVariation;

                        stats.IncStat("Voice From Dad");
                    }
                    else
                    {
                        voice = unchecked((VoiceVariationType)RandomUtil.GetInt(0, 2));

                        stats.IncStat("Voice Teen Random");
                    }
                }
            }

            Sim.VoiceVariation = voice;

            if ((forcePitch) || (Sim.Baby))
            {
                if (Sim.IsMale)
                {
                    if (dad != null)
                    {
                        Sim.VoicePitchModifier = dad.VoicePitchModifier;

                        stats.IncStat("Pitch From Dad");
                    }
                    else
                    {
                        Sim.VoicePitchModifier = RandomUtil.GetFloat(0f, 0.6f);

                        stats.IncStat("Pitch Random");
                    }
                }
                else
                {
                    if (mom != null)
                    {
                        Sim.VoicePitchModifier = mom.VoicePitchModifier;

                        stats.IncStat("Pitch From Mom");
                    }
                    else
                    {
                        Sim.VoicePitchModifier = RandomUtil.GetFloat(0.4f, 1f);

                        stats.IncStat("Pitch Random");
                    }
                }

                float fMutation = RandomUtil.GetFloat(-0.1f, 0.1f);

                Sim.VoicePitchModifier += fMutation;

                if (Sim.VoicePitchModifier < 0f)
                {
                    fMutation = RandomUtil.GetFloat(0f, 0.2f);

                    Sim.VoicePitchModifier = fMutation;
                }
                else if (Sim.VoicePitchModifier > 1f)
                {
                    fMutation = RandomUtil.GetFloat(0f, 0.2f);

                    Sim.VoicePitchModifier = 1f - fMutation;
                }

                stats.AddStat("Baby Pitch", Sim.VoicePitchModifier);
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new InheritVoiceScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim,InheritVoiceScenario>
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerSim main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    SimFromBinEvents.OnSimFromBinUpdate += OnUpdate;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "InheritVoice";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public static void OnUpdate(Common.IStatGenerator stats, SimDescription sim, SimDescription mom, SimDescription dad, Manager manager)
            {
                new InheritVoiceScenario(sim).Run(stats, manager, dad, mom);
            }
        }
    }
}
