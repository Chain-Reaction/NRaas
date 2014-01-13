using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public abstract class AgeBase : SimFromList, IIntermediateOption
    {
        protected int mAge = 0;

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(MiniSimDescription me)
        {
            return true;
        }

        protected override bool Run(IMiniSimDescription me, bool singleSelection)
        {
            int age, maxAge;
            GetAge(me, out age, out maxAge);

            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me, maxAge }), age.ToString());
                if (string.IsNullOrEmpty(text)) return false;

                mAge = 0;
                if (!int.TryParse(text, out mAge))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            return true;
        }

        protected void GetAge(IMiniSimDescription me, out int age, out int maxAge)
        {
            SimDescription simDesc = me as SimDescription;
            MiniSimDescription miniDesc = me as MiniSimDescription;

            float agingYearsSinceLastAgeTransition = 0;
            if (simDesc != null)
            {
                agingYearsSinceLastAgeTransition = simDesc.AgingYearsSinceLastAgeTransition;
            }
            else if (miniDesc != null)
            {
                agingYearsSinceLastAgeTransition = miniDesc.AgingYearsSinceLastAgeTransition;
            }

            maxAge = (int)AgingManager.Singleton.AgingYearsToSimDays(AgingManager.GetMaximumAgingStageLength(me));

            age = (int)AgingManager.Singleton.AgingYearsToSimDays(agingYearsSinceLastAgeTransition);
        }

        protected void AlterAge(IMiniSimDescription me, int newAge)
        {
            SimDescription simDesc = me as SimDescription;
            MiniSimDescription miniDesc = me as MiniSimDescription;

            if ((simDesc != null) && (simDesc.AgingState == null))
            {
                simDesc.AgingState = new AgingState(simDesc);
            }

            float agingYearsSinceLastAgeTransition = 0;
            if (simDesc != null)
            {
                agingYearsSinceLastAgeTransition = simDesc.AgingYearsSinceLastAgeTransition;
            }
            else if (miniDesc != null)
            {
                agingYearsSinceLastAgeTransition = miniDesc.AgingYearsSinceLastAgeTransition;
            }

            int originalAge = (int)AgingManager.Singleton.AgingYearsToSimDays(agingYearsSinceLastAgeTransition);

            if (newAge != originalAge)
            {
                if (simDesc != null)
                {
                    AgingManager.Singleton.CancelAgingAlarmsForSim(simDesc.AgingState);

                    simDesc.AgingYearsSinceLastAgeTransition = AgingManager.Singleton.SimDaysToAgingYears(newAge);

                    if ((simDesc.Household == Household.ActiveHousehold) && (simDesc.CreatedSim != null))
                    {
                        (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnSimAgeChanged(simDesc.CreatedSim.ObjectId);
                    }
                }
                else if (miniDesc != null)
                {
                    miniDesc.AgingYearsSinceLastAgeTransition = AgingManager.Singleton.SimDaysToAgingYears(newAge);
                }
            }
        }
    }
}
