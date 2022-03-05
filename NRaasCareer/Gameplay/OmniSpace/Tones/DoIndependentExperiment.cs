using NRaas.Gameplay.Careers;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;

namespace NRaas.Gameplay.OmniSpace.Tones
{
    [Persistable]
    public class DoIndependentExperiment : CareerToneEx
    {
        // Methods
        private static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, "DoIndependentExperiment:" + name, "Gameplay/Careers/Science/DoIndependentExperiment:" + name, parameters);
        }

        public override void OnTimePassed(InteractionInstance interactionInstance, float totalTime, float deltaTime)
        {
            base.OnTimePassed(interactionInstance, totalTime, deltaTime);

            Science career = base.Career as Science;
            if (career.mTimeSpentInIndependentExptSinceLastBenefit > Science.DoIndependentExperiment.kTimeSpentInDoExptToGainBenefit)
            {
                string str;
                float value = RandomUtil.GetFloat(0f, 1f);
                if (value < Science.DoIndependentExperiment.kChanceOfGettingPerformanceBonusFromDoIndependentExpt)
                {
                    str = "IncreasedPerformance";
                    career.AddPerformance(Science.DoIndependentExperiment.kAmountOfBonusPerformance);
                }
                else if (value < Science.DoIndependentExperiment.kChanceOfGettingPromotionFromDoIndependentExpt)
                {
                    str = "Promotion";
                    career.AddPerformance(100f);
                }
                else
                {
                    str = "Failure";
                }
                string titleText = LocalizeString(career.OwnerDescription, str, new object[] { career.OwnerDescription });
                StyledNotification.Format format = new StyledNotification.Format(titleText, StyledNotification.NotificationStyle.kGameMessagePositive);
                StyledNotification.Show(format);
                career.mTimeSpentInIndependentExptSinceLastBenefit = 0f;
            }
            else
            {
                career.mTimeSpentInIndependentExptSinceLastBenefit += deltaTime / 60f;
            }
        }
    }
}
