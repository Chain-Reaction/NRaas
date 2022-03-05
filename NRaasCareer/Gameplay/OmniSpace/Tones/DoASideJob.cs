using NRaas.Gameplay.Careers;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI.Hud;

namespace NRaas.Gameplay.OmniSpace.Tones
{
    [Persistable]
    public class DoASideJob : CareerToneEx
    {
        // Methods
        public override void BeginCareerTone(InteractionInstance interactionInstance)
        {
            base.BeginCareerTone(interactionInstance);

            Criminal career = OmniCareer.Career<Criminal>(Career);
            if (career == null) return;

            if (career.TotalTimeToCompleteCurrentSideJob < 0.5f)
            {
                career.TotalTimeToCompleteCurrentSideJob = Criminal.DoASideJob.kNumHoursDoingSideJobsToGetBonus * (1f + RandomUtil.GetFloat(-Criminal.DoASideJob.kSideJobDurationVariance, Criminal.DoASideJob.kSideJobDurationVariance));
            }
        }

        public static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, name, "Gameplay/Careers/Criminal/DoASideJob:" + name, parameters);
        }

        public override void OnTimePassed(InteractionInstance interactionInstance, float totalTime, float deltaTime)
        {
            base.OnTimePassed(interactionInstance, totalTime, deltaTime);

            Criminal career = OmniCareer.Career<Criminal>(Career);
            if (career == null) return;

            career.AddTimeSpentDoingSideJobs(deltaTime / 60f);
            if (career.TotalTimeSpentDoingSideJobs >= (career.WhenSideJobBonusLastGiven + career.TotalTimeToCompleteCurrentSideJob))
            {
                career.WhenSideJobBonusLastGiven = career.TotalTimeSpentDoingSideJobs;
                career.TotalTimeToCompleteCurrentSideJob = Criminal.DoASideJob.kNumHoursDoingSideJobsToGetBonus * (1f + RandomUtil.GetFloat(-Criminal.DoASideJob.kSideJobDurationVariance, Criminal.DoASideJob.kSideJobDurationVariance));
                if ((base.Career.OwnerDescription != null) && (base.Career.OwnerDescription.Household != null))
                {
                    int num = Criminal.DoASideJob.kBaseSideJobsBonusAmount + (Criminal.DoASideJob.kSideJobBonusAmountExtraPerCareerLevel * (career.CurLevel.Level - 0x1));
                    num = (int)(num * (1f + RandomUtil.GetFloat(-Criminal.DoASideJob.kSideJobBonusAmountVariance, Criminal.DoASideJob.kSideJobBonusAmountVariance)));
                    num -= num % 0x5;
                    Household household = base.Career.OwnerDescription.Household;
                    household.ModifyFamilyFunds (num);
                    base.Career.ShowOccupationTNS(LocalizeString(base.Career.OwnerDescription, "SideJobBonusText", new object[] { base.Career.OwnerDescription, num }));
                }
            }
        }

        public override bool Test(InteractionInstance inst, out StringDelegate reason)
        {
            if (!base.Test(inst, out reason)) return false;

            OmniCareer career = Career as OmniCareer;
            return (career != null);
        }
    }
}
