using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;

namespace NRaas.CommonSpace.Helpers
{
    public class Tunings
    {
        public static InteractionTuning GetTuning<Target, OldType>()
            where Target : IGameObject
            where OldType : InteractionDefinition
        {
            return GetTuning(typeof(OldType), typeof(Target));
        }
        public static InteractionTuning GetTuning(Type oldType, Type target)
        {
            return AutonomyTuning.GetTuning(oldType.FullName, target.FullName);
        }

        public static InteractionTuning Inject<Target, OldType, NewType>(bool clone)
            where Target : IGameObject
            where OldType : InteractionDefinition
            where NewType : InteractionDefinition
        {
            return Inject(typeof(OldType), typeof(Target), typeof(NewType), typeof(Target), clone);
        }
        public static InteractionTuning Inject<OldTarget, OldType, NewTarget, NewType>(bool clone)
            where OldTarget : IGameObject
            where OldType : InteractionDefinition
            where NewTarget : IGameObject
            where NewType : InteractionDefinition
        {
            return Inject(typeof(OldType), typeof(OldTarget), typeof(NewType), typeof(NewTarget), clone);
        }
        protected static InteractionTuning Inject(Type oldType, Type oldTarget, Type newType, Type newTarget, bool clone)
        {
            InteractionTuning tuning = null;
            try
            {
                tuning = AutonomyTuning.GetTuning(newType.FullName, newTarget.FullName);
                if (tuning == null)
                {
                    tuning = AutonomyTuning.GetTuning(oldType, oldType.FullName, oldTarget);
                    if (tuning == null) return null;

                    Common.InjectionLogger.Append(newType.FullName + " " + newTarget.FullName);

                    if (clone)
                    {
                        tuning = CloneTuning(tuning);
                    }

                    AutonomyTuning.AddTuning(newType.FullName, newTarget.FullName, tuning);
                }

                InteractionObjectPair.sTuningCache.Remove(new Pair<Type, Type>(newType, newTarget));
            }
            catch (Exception e)
            {
                Common.Exception("OldType: " + oldType.ToString() + Common.NewLine + "Target: " + oldTarget.ToString() + Common.NewLine + "NewType: " + newType.ToString() + Common.NewLine + "NewTarget: " + newTarget.ToString(), e);
            }

            return tuning;
        }

        private static Tradeoff CloneTradeoff(Tradeoff old)
        {
            Tradeoff result = new Tradeoff();

            result.mFlags = old.mFlags;
            result.mInputs = Common.CloneList(old.mInputs);
            result.mName = old.mName;
            result.mNumParameters = old.mNumParameters;
            result.mOutputs = Common.CloneList(old.mOutputs);
            result.mVariableRestrictions = old.mVariableRestrictions;
            result.TimeEstimate = old.TimeEstimate;

            return result;
        }

        private static Availability CloneAvailability(Availability old)
        {
            Availability result = new Availability();

            result.mFlags = old.mFlags;
            result.AgeSpeciesAvailabilityFlags = old.AgeSpeciesAvailabilityFlags;
            result.CareerThresholdType = old.CareerThresholdType;
            result.CareerThresholdValue = old.CareerThresholdValue;
            result.ExcludingBuffs = Common.CloneList(old.ExcludingBuffs);
            result.ExcludingTraits = Common.CloneList(old.ExcludingTraits);
            result.MoodThresholdType = old.MoodThresholdType;
            result.MoodThresholdValue = old.MoodThresholdValue;
            result.MotiveThresholdType = old.MotiveThresholdType;
            result.MotiveThresholdValue = old.MotiveThresholdValue;
            result.RequiredBuffs = Common.CloneList(old.RequiredBuffs);
            result.RequiredTraits = Common.CloneList(old.RequiredTraits);
            result.SkillThresholdType = old.SkillThresholdType;
            result.SkillThresholdValue = old.SkillThresholdValue;
            result.WorldRestrictionType = old.WorldRestrictionType;
            result.OccultRestrictions = old.OccultRestrictions;
            result.OccultRestrictionType = old.OccultRestrictionType;
            result.SnowLevelValue = old.SnowLevelValue;

            result.WorldRestrictionWorldNames = Common.CloneList(old.WorldRestrictionWorldNames);
            result.WorldRestrictionWorldTypes = Common.CloneList(old.WorldRestrictionWorldTypes);

            return result;
        }
        private static InteractionTuning CloneTuning(InteractionTuning oldTuning)
        {
            InteractionTuning newTuning = new InteractionTuning();

            newTuning.mFlags = oldTuning.mFlags;
            newTuning.ActionTopic = oldTuning.ActionTopic;
            newTuning.AlwaysChooseBest = oldTuning.AlwaysChooseBest;
            newTuning.Availability = CloneAvailability(oldTuning.Availability);
            newTuning.CodeVersion = oldTuning.CodeVersion;
            newTuning.FullInteractionName = oldTuning.FullInteractionName;
            newTuning.FullObjectName = oldTuning.FullObjectName;
            newTuning.mChecks = Common.CloneList(oldTuning.mChecks);
            newTuning.mTradeoff = CloneTradeoff(oldTuning.mTradeoff);
            newTuning.PosturePreconditions = oldTuning.PosturePreconditions;
            newTuning.ScoringFunction = oldTuning.ScoringFunction;
            newTuning.ScoringFunctionOnlyAppliesToSpecificCommodity = oldTuning.ScoringFunctionOnlyAppliesToSpecificCommodity;
            newTuning.ScoringFunctionString = oldTuning.ScoringFunctionString;
            newTuning.ShortInteractionName = oldTuning.ShortInteractionName;
            newTuning.ShortObjectName = oldTuning.ShortObjectName;

            return newTuning;
        }
    }
}

