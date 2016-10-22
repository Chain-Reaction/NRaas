using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class OrderServices : Computer.ComputerInteraction, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public static readonly InteractionDefinition RandomSingleton = new Definition(true);

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
            interactions.AddNoDupTest<Computer>(RandomSingleton);
        }

        protected static Dictionary<int, SimDescription> GetPotentials(Sim actor)
        {
            CASAgeGenderFlags allow = CASAgeGenderFlags.None;
            if ((actor.SimDescription.Teen && Woohooer.Settings.AllowTeen(true)) || (actor.SimDescription.YoungAdultOrAbove && Woohooer.Settings.AllowTeenAdult(true)))
            {
                allow |= CASAgeGenderFlags.Teen;
            }

            if (actor.SimDescription.Teen && Woohooer.Settings.AllowTeen(true) && Woohooer.Settings.AllowTeenAdult(true))
            {
                allow |= CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder;
            }

            if (actor.SimDescription.YoungAdultOrAbove)
            {
                allow |= CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder;
            }

            Dictionary<int, List<SimDescription>> potentials = KamaSimtra.GetPotentials(allow, true);

            Dictionary<int, SimDescription> choices = new Dictionary<int, SimDescription>();            
            for (int i = 1; i <= 10; i++)
            {                
                List<SimDescription> fullList;
                if (!potentials.TryGetValue(i, out fullList)) continue;

                bool needFemale = false;

                if (actor.SimDescription.CanAutonomouslyBeRomanticWithGender(CASAgeGenderFlags.Male))
                {
                    if (actor.SimDescription.CanAutonomouslyBeRomanticWithGender(CASAgeGenderFlags.Female))
                    {
                        if (RandomUtil.CoinFlip())
                        {
                            needFemale = true;
                        }
                    }
                    else
                    {
                        needFemale = false;
                    }
                }
                else if (actor.SimDescription.CanAutonomouslyBeRomanticWithGender(CASAgeGenderFlags.Female))
                {
                    needFemale = true;
                }
                else
                {
                    needFemale = !actor.IsFemale;
                }

                List<SimDescription> randomList = new List<SimDescription>();                

                foreach (SimDescription sim in fullList)
                {                   
                    if (sim.IsFemale != needFemale) continue;

                    if (sim.Household == actor.Household) continue;                    

                    string reason;
                    GreyedOutTooltipCallback callback = null;
                    if (!CommonSocials.CanGetRomantic(actor.SimDescription, sim, false, true, true, ref callback, out reason))
                    {                                              
                        continue;
                    }                    

                    if (choices.ContainsValue(sim)) continue;                    

                    randomList.Add(sim);
                }

                if (randomList.Count > 0)
                {
                    choices.Add(i, RandomUtil.GetRandomObjectFromList(randomList));
                }
            }

            return choices;
        }

        public override bool Run()
        {
            SimDescription hooker = null;

            Definition def = base.InteractionDefinition as Definition;
            if (def == null)
            {                
                return false;
            }

            try
            {
                StandardEntry();
                if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                {                    
                    StandardExit();
                    return false;
                }

                Target.StartVideo(Computer.VideoType.Chat);
                base.AnimateSim("GenericTyping");

                bool succeeded = true;              
                if (base.DoTimedLoop(Computer.CheckWeather.kTimeToBrowse, ~(ExitReason.Replan | ExitReason.PlayIdle | ExitReason.ObjectStateChanged | ExitReason.MidRoutePushRequested | ExitReason.MaxSkillPointsReached | ExitReason.BuffFailureState | ExitReason.MoodFailure)))
                {                    
                    Dictionary<int, SimDescription> potentials = GetPotentials(Actor);

                    if (potentials.Values.Count > 0)
                    {                        
                        if (!def.mRandom)
                        {                            
                            hooker = new SimSelection(Common.Localize("OrderServices:Title"), Actor.SimDescription, potentials.Values, SimSelection.Type.ProfessionalServices, -1000).SelectSingle();
                            if (hooker == null)
                            {
                                Common.Notify(Common.Localize("Rendezvous:NoSelect", Actor.IsFemale));
                                succeeded = false;
                            }
                        }
                        else
                        {
                            int val = 0;
                            if (Actor.IsSelectable)
                            {
                                string text = StringInputDialog.Show(Common.Localize("OrderServices:Title"), Common.Localize("OrderServices:Prompt"), val.ToString());
                                if (string.IsNullOrEmpty(text))
                                {
                                    succeeded = false;
                                }

                                if (!int.TryParse(text, out val))
                                {
                                    SimpleMessageDialog.Show(Common.Localize("OrderServices:Title"), Common.Localize("Numeric:Error"));
                                    succeeded = false;
                                }
                            }
                            else
                            {
                                val = Actor.FamilyFunds * 2 / 100;
                            }

                            if (val > Actor.FamilyFunds)
                            {
                                if (Actor.IsSelectable)
                                {
                                    Common.Notify(Common.Localize("OrderServices:NoFunds", Actor.IsFemale));
                                }
                                succeeded = false;
                            }

                            if (succeeded)
                            {
                                // how much sexy can we afford?
                                bool allAbove = false;
                                foreach (SimDescription sim in potentials.Values)
                                {
                                    KamaSimtra skill = sim.SkillManager.GetSkill<KamaSimtra>(KamaSimtra.StaticGuid);
                                    if (skill == null) continue;                                    

                                    if (skill.GetPayment() > Actor.FamilyFunds)
                                    {                                        
                                        allAbove = true;
                                    }
                                    else
                                    {                                        
                                        allAbove = false;
                                    }

                                    if (skill.GetPayment() > val) continue;                                    
                                    hooker = sim;
                                }

                                if (hooker == null)
                                {
                                    if (!allAbove)
                                    {
                                        if (Actor.IsSelectable)
                                        {
                                            Common.Notify(Common.Localize("OrderServices:StopBeingCheap"));
                                        }
                                        succeeded = false;
                                    }
                                    else
                                    {
                                        if (Actor.IsSelectable)
                                        {
                                            Common.Notify(Common.Localize("OrderServices:NoFunds", Actor.IsFemale));
                                        }
                                        succeeded = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Common.Notify(Common.Localize("OrderServices:NoChoices", Actor.IsFemale));
                        succeeded = false;
                    }
                }               

                if (succeeded && hooker != null)
                {
                    GetSimToLotEx(hooker, Actor.LotHome, def.mRandom);
                }

                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                StandardExit();
                return succeeded;
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

        public void GetSimToLotEx(SimDescription simDesc, Lot actorHome, bool random)
        {
            bool success = SimEx.GetSimToSimHome(simDesc, base.Actor, new Callback(KamaSimtraSettings.ServiceData.GoToLotSuccessEx));

            if (success)
            {
                if (random)
                {
                    // someone will be over
                    if (base.Actor.IsSelectable)
                    {
                        Common.Notify(Common.Localize("OrderServices:Success", Actor.IsFemale));
                    }
                }
                else
                {
                    // selected sim will be over
                    Common.Notify(Common.Localize("OrderServices:SuccessSpecific", Actor.IsFemale, new object[] { "\"" + KamaSimtra.Settings.GetAlias(simDesc) + "\"" }));
                }
            }
            else
            {
                if (base.Actor.IsSelectable)
                {
                    Phone.CallInviteOver.ShowInviteFailedDialog(this.GetInteractionName());
                }
                return;
            }

            KamaSimtraSettings.ServiceData data = KamaSimtra.Settings.GetServiceData(base.Actor.SimDescription.SimDescriptionId, true);
            data.mRequester = base.Actor.SimDescription.SimDescriptionId;
            data.mProfessional = simDesc.SimDescriptionId;
            data.mWasRandom = random;
            data.SetupPushAlarm();
            KamaSimtra.Settings.SetServiceData(base.Actor.SimDescription.SimDescriptionId, data);
        }               

        public class Definition : InteractionDefinition<Sim, Computer, OrderServices>
        {
            public bool mRandom = false;

            public Definition()
            { }
            public Definition(bool random)
            {
                mRandom = random;
            }

            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                if (mRandom)
                {
                    return Common.Localize("OrderServicesRandom:MenuName", actor.IsFemale);
                }
                else
                {
                    return Common.Localize("OrderServicesSpecific:MenuName", actor.IsFemale);
                }
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                using (WoohooTuningControl control = new WoohooTuningControl(parameters.InteractionObjectPair.Tuning, Woohooer.Settings.mAllowTeenWoohoo))
                {
                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }
            }            

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {                  
                if (!SimClock.IsNightTime())
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not Night Time");
                    return false;
                }                

                if (isAutonomous)
                {
                    if (!mRandom)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not Random");
                        return false;
                    }

                    if (!Woohooer.Settings.mAutonomousComputer)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("No Autonomous");
                        return false;
                    }

                    if (a.LotCurrent != a.LotHome)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not at home");
                        return false;
                    }

                    if (ScoringLookup.GetScore("LikeProfessional", a.SimDescription) < 0)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Score Fail");
                        return false;
                    }
                    
                    if (!CommonWoohoo.SatisfiesCooldown(a, RandomUtil.GetRandomObjectFromList<Sim>(LotManager.Actors), isAutonomous, ref greyedOutTooltipCallback))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Cooldown Fail");
                        return false;
                    }

                    if (!CommonWoohoo.HasWoohooableObject(a.LotHome, a, RandomUtil.GetRandomObjectFromList<Sim>(LotManager.Actors)))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("No Woohooable Objects Fail");
                        return false;
                    }

                    if (GetPotentials(a).Count == 0)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("No Choices");
                        return false;
                    }
                }
                else
                {
                    if (!KamaSimtra.Settings.mShowRegisterInteraction)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("User Hidden");
                        return false;
                    }
                }

                return true;
            }                       
        }        
    }
}
