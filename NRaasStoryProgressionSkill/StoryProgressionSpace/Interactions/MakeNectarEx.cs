using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Skills;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class MakeNectarEx : NectarMaker.MakeNectar, IAlarmOwner, Common.IPreLoad
    {
        public void OnPreLoad()
        {
            Tunings.Inject<NectarMaker, NectarMaker.MakeNectar.Definition, Definition>(false);
        }

        public override void ConfigureInteraction()
        {
            try
            {
                Definition interactionDefinition = base.InteractionDefinition as Definition;
                mStyle = interactionDefinition.mStyle;
                TimedStage stage = new TimedStage(GetInteractionName(), NectarMaker.kPushButtonTimeMinutes, false, true, true);
                Stages = new List<Stage>(new Stage[] { stage });
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        public override bool Run()
        {
            try
            {
                if ((base.Target.CurrentState == NectarMaker.NectarMakerState.FruitAddable) || (base.Target.CurrentState == NectarMaker.NectarMakerState.SquishingFruit))
                {
                    SquishGrapes instance = SquishGrapes.Singleton.CreateInstance(Target, Actor, mPriority, Autonomous, true) as SquishGrapes;
                    instance.MakeStyleToPush = mStyle;
                    return base.Actor.InteractionQueue.PushAsContinuation(instance, true);
                }

                if (!base.Run()) return false;

                List<AlarmHandle> handles = new List<AlarmHandle>();
                foreach (KeyValuePair<AlarmHandle, List<AlarmManager.Timer>> timers in Target.LotCurrent.AlarmManager.mTimers)
                {
                    bool found = false;

                    foreach (AlarmManager.Timer timer in timers.Value)
                    {
                        if (timer.ObjectRef != Target) continue;

                        if (!timer.CallBack.Equals(new AlarmTimerCallback(Target.FinishedMakingNectarCallback))) continue;

                        found = true;
                        break;
                    }

                    if (found)
                    {
                        handles.Add(timers.Key);
                    }
                }

                foreach (AlarmHandle handle in handles)
                {
                    Target.RemoveAlarm(handle);
                }

                float extendedNectarationTimeMultiplier = 1f;
                if (mStyle == NectarMaker.MakeNectarStyle.ExtendedNectaration)
                {
                    extendedNectarationTimeMultiplier = NectarMaker.kExtendedNectarationTimeMultiplier;
                }

                Target.LotCurrent.AlarmManager.AddAlarm(NectarMaker.kMachineRunTimeMinutes * extendedNectarationTimeMultiplier, TimeUnit.Minutes, FinishedMakingNectarCallback, "Nectar Maker- Finish Nectar", AlarmType.DeleteOnReset, this);

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        protected void FinishedMakingNectarCallback()
        {
            string msg = null;

            try
            {
                List<Ingredient> fruitsUsed = Inventories.QuickFind<Ingredient>(Target.Inventory);
                foreach (GameObject obj2 in fruitsUsed)
                {
                    Target.Inventory.SetNotInUse(obj2);
                }

                msg += "1";

                if (((fruitsUsed != null) && (fruitsUsed.Count > 0x0)) && (Target.mLastSimToMake != null))
                {
                    float num = 0f;
                    float num2 = (((float)RandomGen.NextDouble()) * (NectarMaker.kMaxGlobalValueVariance - NectarMaker.kMinGlobalValueVariance)) + NectarMaker.kMinGlobalValueVariance;
                    int skillLevel = Target.mLastSimToMake.SkillManager.GetSkillLevel(SkillNames.Nectar);
                    float num4 = ((NectarMaker.kLevel10Multiplier - NectarMaker.kLevel0Multiplier) * (((float)skillLevel) / 10f)) + NectarMaker.kLevel0Multiplier;
                    fruitsUsed.Sort();
                    string str = "";
                    bool flag = true;
                    Ingredient ingredient = fruitsUsed[0x0];
                    string key = "";
                    Dictionary<string, int> ingredients = new Dictionary<string, int>();
                    if (ingredient != null)
                    {
                        key = ingredient.Data.Key;
                    }

                    msg += "2";

                    float num5 = 0f;
                    foreach (Ingredient ingredient2 in fruitsUsed)
                    {
                        if (flag && !key.Equals(ingredient2.Data.Key))
                        {
                            flag = false;
                        }
                        str = str + ingredient2.Data.Key;
                        if (!ingredients.ContainsKey(ingredient2.Data.Key))
                        {
                            ingredients.Add(ingredient2.Data.Key, 0x1);
                        }
                        else
                        {
                            ingredients[ingredient2.Data.Key]++;
                        }
                        num += ingredient2.Data.NectarValue;
                        num5 += (float)ingredient2.GetQuality();
                        Target.Inventory.TryToRemove(ingredient2);
                    }

                    msg += "3";

                    num5 /= (float)fruitsUsed.Count;
                    uint hash = ResourceUtils.HashString32(str);
                    float num7 = 1f;
                    if (!flag)
                    {
                        num7 = NectarMaker.CalculateHashMultiplier(hash);
                    }

                    msg += "4";

                    float makeStyleValueModifier = Target.GetMakeStyleValueModifier(Target.mLastUsedMakeStyle);
                    int makeStyleBottleModifier = Target.GetMakeStyleBottleModifier(Target.mLastUsedMakeStyle);
                    float baseValue = ((((((num * num2) * num4) * Target.mFlavorMultiplier) * num7) * NectarMaker.kQualityLevelMultiplier[((int)num5) - 0x1]) * Target.mMultiplierFromFeet) * makeStyleValueModifier;
                    int numBottles = (NectarMaker.kNumBottlesPerBatch + Target.mBottleDifference) + makeStyleBottleModifier;
                    NectarSkill skill = Target.mLastSimToMake.SkillManager.GetSkill<NectarSkill>(SkillNames.Nectar);
                    if ((skill != null) && skill.IsNectarMaster())
                    {
                        numBottles += NectarSkill.kExtraBottlesNectarMaster;
                    }

                    msg += "5";

                    List<NectarMaker.IngredientNameCount> list2 = new List<NectarMaker.IngredientNameCount>();
                    foreach (string str3 in ingredients.Keys)
                    {
                        list2.Add(new NectarMaker.IngredientNameCount(str3, ((float)ingredients[str3]) / ((float)fruitsUsed.Count)));
                    }

                    msg += "6";

                    list2.Sort();
                    string str4 = "";
                    string str5 = "";
                    if (list2.Count > 0x0)
                    {
                        str4 = IngredientData.NameToDataMap[list2[0x0].IngredientName].Name;
                    }
                    if (list2.Count > 0x1)
                    {
                        str5 = IngredientData.NameToDataMap[list2[0x1].IngredientName].Name;
                    }

                    msg += "7";

                    List<NectarSkill.NectarBottleInfo> topNectarsMade = new List<NectarSkill.NectarBottleInfo>();
                    if (skill != null)
                    {
                        topNectarsMade = skill.mTopNectarsMade;
                    }

                    msg += "8";

                    string defaultEntryText = "";
                    if (string.IsNullOrEmpty(str5))
                    {
                        defaultEntryText = Common.LocalizeEAString(false, "Gameplay/Objects/CookingObjects/NectarBottle:NectarNameOneFruit", new object[] { str4 });
                    }
                    else
                    {
                        defaultEntryText = Common.LocalizeEAString(false, "Gameplay/Objects/CookingObjects/NectarBottle:NectarName", new object[] { str4, str5 });
                        if (defaultEntryText.Length > 0x28)
                        {
                            defaultEntryText = Common.LocalizeEAString(false, "Gameplay/Objects/CookingObjects/NectarBottle:NectarNameOneFruit", new object[] { str4 });
                        }
                    }

                    msg += "9";

                    bool nameFound = false;
                    foreach (NectarSkill.NectarBottleInfo info in topNectarsMade)
                    {
                        if (info.mFruitHash == hash)
                        {
                            defaultEntryText = info.mBottleName;
                            nameFound = true;
                        }
                    }

                    msg += "A";

                    bool promptName = false;

                    string entryKey = "Gameplay/Objects/HobbiesSkills/NectarMaker:";
                    if (flag)
                    {
                        entryKey = entryKey + "NameBottleDialogJustOneFruit";
                    }
                    else if (num7 < NectarMaker.kPoorComboThreshold)
                    {
                        entryKey = entryKey + "NameBottleDialogTerribly";
                    }
                    else if (num7 < NectarMaker.kWellComboThreshold)
                    {
                        entryKey = entryKey + "NameBottleDialogPoor";
                    }
                    else if (num7 < NectarMaker.kGreatComboThreshold)
                    {
                        entryKey = entryKey + "NameBottleDialogWell";
                    }
                    else if (num7 < NectarMaker.kAmazingComboThreshold)
                    {
                        entryKey = entryKey + "NameBottleDialogGreat";
                        promptName = true;
                    }
                    else
                    {
                        entryKey = entryKey + "NameBottleDialogAmazing";
                        promptName = true;
                    }

                    msg += "B";

                    string name = defaultEntryText;

                    if (promptName)
                    {
                        if ((!nameFound) && (NRaas.StoryProgression.Main.GetValue<NectarPushScenario.NameGreatNectarOption, bool>()))
                        {
                            name = StringInputDialog.Show(Actor.SimDescription.FullName, Common.LocalizeEAString(entryKey), defaultEntryText, 0x28, StringInputDialog.Validation.ObjectRequireName);
                        }
                        else
                        {
                            List<object> parameters = StoryProgression.Main.Stories.AddGenderNouns(Actor.SimDescription);
                            parameters.Add(defaultEntryText);

                            Common.Notify(Common.Localize("MadeNectar:Results", Actor.IsFemale, parameters.ToArray()), Actor.ObjectId);
                        }
                    }

                    msg += "C";

                    bool flag2 = false;
                    if (skill != null)
                    {
                        skill.MadeXBottles(numBottles);
                        skill.UsedFruits(fruitsUsed);
                        skill.NectarTypeMade(new NectarSkill.NectarBottleInfo(hash, name, ingredients, (int)baseValue));
                        flag2 = skill.ReachedMaxLevel();
                    }

                    msg += "D";

                    int dateNum = ((int)SimClock.ConvertFromTicks(GameStates.TimeInHomeworld.Ticks, TimeUnit.Weeks)) + 0x1;
                    for (int i = 0x0; i < numBottles; i++)
                    {
                        NectarBottle item = GlobalFunctions.CreateObjectOutOfWorld("NectarBottle", null, new NectarBottleObjectInitParams(hash, name, list2, "Gameplay/Objects/HobbiesSkills/NectarMaker:Weeks", dateNum, baseValue, baseValue, Target.mLastSimToMake, flag2)) as NectarBottle;
                        Target.mBottles.Add(item);
                        EventTracker.SendEvent(EventTypeId.kMadeNectar, Target.mLastSimToMake.CreatedSim, item);
                    }
                    if (Target.mBottles.Count > 0x0)
                    {
                        Target.mCurrentStateMachine.SetActor("nectarBottle", Target.mBottles[0x0]);
                    }

                    msg += "E";

                    Target.mCurrentStateMachine.RequestState("nectarMaker", "Exit");
                    Target.mCurrentStateMachine.Dispose();
                    Target.mCurrentStateMachine = null;
                    Target.mMultiplierFromFeet = 1f;
                    Target.mLastUsedMakeStyle = NectarMaker.MakeNectarStyle.Basic;
                    Target.mLastSimToMake = null;
                    Target.CurrentState = NectarMaker.NectarMakerState.FruitAddable;

                    msg += "F";
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, msg, e);
            }
        }

        public new class Definition : NectarMaker.MakeNectar.Definition
        {
            public Definition(NectarMaker.MakeNectarStyle style)
                : base(style)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new MakeNectarEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}

