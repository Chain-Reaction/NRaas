using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class RandomizeVoices : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "InheritVoices";
        }

        public static int Inherit(SimDescription sim, Dictionary<SimDescription,bool> completed)
        {
            if (sim == null) return 0;

            if (completed.ContainsKey(sim)) return 0;

            int count = 0;
            if ((sim.VoicePitchModifier == 0.5f) && (sim.VoiceVariation == VoiceVariationType.A))
            {
                if (sim.Genealogy == null) return 0;

                if (sim.Genealogy.Parents.Count == 0)
                {
                    Inherit(sim, null, null);
                    count++;
                }
                else
                {
                    SimDescription mom = null, dad = null;

                    if ((sim.Genealogy.Parents[0].SimDescription == null) ||
                        (sim.Genealogy.Parents[0].SimDescription.IsMale))
                    {
                        dad = sim.Genealogy.Parents[0].SimDescription;

                        if (sim.Genealogy.Parents.Count == 2)
                        {
                            mom = sim.Genealogy.Parents[1].SimDescription;
                        }
                    }
                    else
                    {
                        mom = sim.Genealogy.Parents[0].SimDescription;

                        if (sim.Genealogy.Parents.Count == 2)
                        {
                            dad = sim.Genealogy.Parents[1].SimDescription;
                        }
                    }

                    if (mom != null)
                    {
                        count += Inherit(mom, completed);
                    }

                    if (dad != null)
                    {
                        count += Inherit(dad, completed);
                    }

                    Inherit(sim, dad, mom);
                    count++;
                }
            }

            completed.Add(sim, true);
            return count;
        }

        public static void Inherit(SimDescription child, SimDescription dad, SimDescription mom)
        {
            VoiceVariationType voice = VoiceVariationType.B;

            if (RandomUtil.CoinFlip())
            {
                if (dad != null)
                {
                    voice = dad.VoiceVariation;
                }
                else if (mom != null)
                {
                    voice = mom.VoiceVariation;
                }
                else
                {
                    voice = unchecked((VoiceVariationType)RandomUtil.GetInt(0, 2));
                }
            }
            else
            {
                if (mom != null)
                {
                    voice = mom.VoiceVariation;
                }
                else if (dad != null)
                {
                    voice = dad.VoiceVariation;
                }
                else
                {
                    voice = unchecked((VoiceVariationType)RandomUtil.GetInt(0, 2));
                }
            }

            child.VoiceVariation = voice;

            if (child.IsMale)
            {
                if (dad != null)
                {
                    child.VoicePitchModifier = dad.VoicePitchModifier;
                }
                else
                {
                    child.VoicePitchModifier = RandomUtil.GetFloat(0f, 0.6f);
                }
            }
            else
            {
                if (mom != null)
                {
                    child.VoicePitchModifier = mom.VoicePitchModifier;
                }
                else
                {
                    child.VoicePitchModifier = RandomUtil.GetFloat(0.4f, 1f);
                }
            }

            float fMutation = RandomUtil.GetFloat(-0.1f, 0.1f);

            child.VoicePitchModifier += fMutation;

            if (child.VoicePitchModifier < 0f)
            {
                fMutation = RandomUtil.GetFloat(0f, 0.2f);

                child.VoicePitchModifier = fMutation;
            }
            else if (child.VoicePitchModifier > 1f)
            {
                fMutation = RandomUtil.GetFloat(0f, 0.2f);

                child.VoicePitchModifier = 1f - fMutation;
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (AcceptCancelDialog.Show(Common.Localize("InheritVoices:Prompt")))
            {
                Dictionary<SimDescription, bool> completed = new Dictionary<SimDescription, bool>();

                int count = 0;

                List<Sim> list = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>());
                foreach (Sim sim in list)
                {
                    count += Inherit (sim.SimDescription, completed);
                }

                SimpleMessageDialog.Show(Name, Common.Localize("InheritVoices:Result", false, new object[] { count }));
            }
            return OptionResult.SuccessClose;
        }
    }
}
