using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using System;

namespace NRaas.CareerSpace.Interactions
{
    public static class Shakedown
    {
        public static bool Test(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (NRaas.Careers.Settings.mMaxShakedown <= 0) return false;

                if (actor.Occupation is Criminal) return true;

                if (actor.Occupation is LawEnforcement) return true;

                OmniCareer actorCareer = actor.Occupation as OmniCareer;
                if (actorCareer != null)
                {
                    return actorCareer.CanShakedown();
                }

                return false;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnAccepted(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                int funds = RandomUtil.GetInt(NRaas.Careers.Settings.mMaxShakedown / 2, NRaas.Careers.Settings.mMaxShakedown);

                if (funds > target.FamilyFunds)
                {
                    funds = target.FamilyFunds;
                }

                if (funds <= 0) return;

                target.ModifyFunds(-funds);

                actor.ModifyFunds(funds);

                if (actor.IsSelectable)
                {
                    Common.Notify(actor, Common.Localize("Shakedown:Notice", actor.IsFemale, target.IsFemale, new object[] { actor, target, funds }));
                }

                Relationship relation = Relationship.Get(actor, target, true);
                if (relation != null)
                {
                    relation.LTR.UpdateLiking(NRaas.Careers.Settings.mShakedownRelationChange);
                }

                OmniCareer actorCareer = actor.Occupation as OmniCareer;
                if (actorCareer != null)
                {
                    actorCareer.AddShakedownFunds(funds);
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
