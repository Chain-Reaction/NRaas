using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options
{
    public class TestOption : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "Test";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return GoHere.Settings.Debugging;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Pair<int, Dictionary<int, bool>> dic;
            if (PlumbBob.SelectedActor.mAllowedRooms.TryGetValue(LotManager.ActiveLot.LotId, out dic))
            {
                foreach (KeyValuePair<int, bool> pair in dic.Second)
                {
                    Common.Notify("Roomid: " + pair.Key + ". Allowed: " + pair.Value);
                }
            }

            FilterHelper.CreateFilterWithRandomCriteria(PlumbBob.SelectedActor.SimDescription.GetMiniSimDescription(), new List<string>(), new Dictionary<string,string>(), new int[] {1, 3}, new Dictionary<string,int[]>());
            /*
            Common.StringBuilder msg = new Common.StringBuilder("TestOption");

            try
            {
                AddTuning(new InteractionObjectPair(GoHereAnd.sOtherLotSingleton, Terrain.Singleton), msg);
            }
            finally
            {
                Common.WriteLog(msg);
            }
             */

            return OptionResult.SuccessClose;
        }

        public static InteractionTuning GetTuning(string interaction, string className, Common.StringBuilder msg)
        {
            msg += Common.NewLine + "Interaction: " + interaction;
            msg += Common.NewLine + " Hash: " + ResourceUtils.HashString32(interaction);
            msg += Common.NewLine + "ClassName: " + className;
            msg += Common.NewLine + " Hash: " + ResourceUtils.HashString32(className);

            Dictionary<uint, InteractionTuning> dictionary;
            InteractionTuning tuning;
            if (!AutonomyTuning.sTuning.TryGetValue(ResourceUtils.HashString32(interaction), out dictionary))
            {
                msg += Common.NewLine + "A";
                return null;
            }

            dictionary.TryGetValue(ResourceUtils.HashString32(className), out tuning);

            msg += Common.NewLine + "B " + (tuning != null);
            return tuning;
        }

        public static InteractionTuning GetTuning(Type interactionType, string tuningFile, Type targetType, Common.StringBuilder msg)
        {
            msg += Common.NewLine + "GetTuning";

            string str;
        Label_0000:
            str = targetType.FullName;

            msg += Common.NewLine + "FullName: " + str;

            InteractionTuning tuning = GetTuning(tuningFile, str, msg);
            if (tuning != null)
            {
                msg += Common.NewLine + "A";

                return tuning;
            }
            if (targetType != typeof(GameObject))
            {
                targetType = targetType.BaseType;
                if (targetType != null)
                {
                    goto Label_0000;
                }
            }
            if ((interactionType != null) && Reflection.IsTypeAssignableFrom(typeof(InteractionDefinition), interactionType))
            {
                msg += Common.NewLine + "InteractionType: " + interactionType;

                while (interactionType.BaseType != typeof(object))
                {
                    if (interactionType.BaseType == null)
                    {
                        break;
                    }

                    msg += Common.NewLine + "BaseType: " + interactionType.BaseType;

                    if (interactionType.BaseType.IsGenericType)
                    {
                        Type baseType = interactionType.BaseType;
                        Type[] genericArguments = baseType.GetGenericArguments();
                        while ((genericArguments.Length != 0x3) && (baseType.BaseType != null))
                        {
                            genericArguments = baseType.BaseType.GetGenericArguments();
                        }

                        if ((genericArguments.Length == 0x3) && genericArguments[0x1].IsInterface)
                        {
                            msg += Common.NewLine + "TuningFule: " + tuningFile;
                            msg += Common.NewLine + "FullName: " + genericArguments[0x1].FullName;

                            InteractionTuning tuning2 = AutonomyTuning.GetTuning(tuningFile, genericArguments[0x1].FullName);
                            if (tuning2 != null)
                            {
                                msg += Common.NewLine + "B";

                                return tuning2;
                            }
                        }
                    }
                    interactionType = interactionType.BaseType;
                }
            }

            msg += Common.NewLine + "C";

            return null;
        }

        public void AddTuning(InteractionObjectPair ths, Common.StringBuilder msg)
        {
            msg += Common.NewLine + "AddTuning";

            if (ths.InteractionDefinition != null)
            {
                msg += Common.NewLine + "A";

                bool flag;
                Type key = ths.InteractionDefinition.GetType();
                if (!InteractionObjectPair.sRequiresTuningCache.TryGetValue(key, out flag))
                {
                    msg += Common.NewLine + "B";

                    flag = true;
                    for (Type type2 = key; (type2 != null) && flag; type2 = type2.BaseType)
                    {
                        flag &= type2.GetCustomAttributes(typeof(DoesntRequireTuningAttribute), false).Length == 0x0;
                    }
                    InteractionObjectPair.sRequiresTuningCache.Add(key, flag);
                }
                if (flag)
                {
                    msg += Common.NewLine + "C";

                    Type b = null;
                    if (ths.mTarget != null)
                    {
                        b = ths.mTarget.GetType();
                    }
                    else
                    {
                        b = ths.mTargetType;
                    }
                    if (b != null)
                    {
                        InteractionTuning tuning;
                        Pair<Type, Type> pair = new Pair<Type, Type>(key, b);
                        /*
                        if (InteractionObjectPair.sTuningCache.TryGetValue(pair, out tuning))
                        {
                            msg += Common.NewLine + "D";

                            ths.mTuning = tuning;
                        }
                        else*/
                        {
                            msg += Common.NewLine + "E";

                            string fullName = key.FullName;
                            tuning = GetTuning(key, fullName, b, msg);
                            ths.mTuning = tuning;
                            InteractionObjectPair.sTuningCache[pair] = tuning;
                        }
                    }
                }
            }
        }
    }
}
