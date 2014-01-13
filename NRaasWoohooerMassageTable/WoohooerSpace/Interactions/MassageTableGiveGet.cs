using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using Sims3.Store.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class MassageTableGiveGet : Interaction<Sim, MassageTable>, IImmediateInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public Sim mMassagee;
        public Sim mMasseuse;
        public MassageTable.MassageInfo mMassInfo;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<MassageTable, MassageTable.GiveGet.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = MassageTable.GiveGet.Singleton;
            MassageTable.GiveGet.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<MassageTable, MassageTable.GiveGet.Definition>(MassageTable.GiveGet.Singleton);
        }

        public override void Init(ref InteractionInstanceParameters parameters)
        {
            try
            {
                base.Init(ref parameters);

                Sim target = null;
                bool mGetting = false;
                if (Autonomous)
                {
                    mMassInfo = MassageTable.MassageInfos[RandomUtil.GetInt(0x4)];

                    bool romantic = ((mMassInfo.mType == MassageTable.MassageType.Romantic) || (mMassInfo.mType == MassageTable.MassageType.AmazingRomantic));

                    List<Sim> validMassageSims = GetValidMassageSims(Target, Actor, romantic, parameters.Autonomous);
                    if (validMassageSims.Count == 0x0)
                    {
                        return;
                    }

                    target = validMassageSims[RandomUtil.GetInt(validMassageSims.Count - 0x1)];
                }
                else
                {
                    target = GetSelectedObject() as Sim;
                    Definition interactionDefinition = InteractionDefinition as Definition;
                    mMassInfo = interactionDefinition.mMassInfo;
                    mGetting = interactionDefinition.mGetting;
                }

                if (target != null)
                {
                    if (mGetting)
                    {
                        mMasseuse = target;
                        mMassagee = Actor;
                    }
                    else
                    {
                        mMasseuse = Actor;
                        mMassagee = target;
                    }

                    bool cancelled = false;
                    Sim masseuse = null;
                    Sim massagee = null;
                    if (mMasseuse.IsSelectable)
                    {
                        masseuse = mMasseuse;
                        massagee = mMassagee;
                    }
                    else
                    {
                        masseuse = mMassagee;
                        massagee = mMasseuse;
                    }

                    bool allowAge = false;

                    string reason;
                    GreyedOutTooltipCallback callback = null;
                    if (CommonSocials.CanGetRomantic(Actor, target, Autonomous, false, true, ref callback, out reason))
                    {
                        allowAge = true;
                    }

                    if (((mMassInfo.mType == MassageTable.MassageType.Romantic) || (mMassInfo.mType == MassageTable.MassageType.AmazingRomantic)) && !allowAge)
                    {
                        if (!Autonomous)
                        {
                            masseuse.ShowTNSIfSelectable(MassageTable.LocalizeString("InvalidRomanticMassage", new object[] { massagee }), StyledNotification.NotificationStyle.kGameMessageNegative);
                        }
                        cancelled = true;
                    }

                    if (target.GetRelationship(Actor, true).AreEnemies())
                    {
                        if (!Autonomous)
                        {
                            masseuse.ShowTNSIfSelectable(MassageTable.LocalizeString("NoMassageForEnemy", new object[] { massagee }), StyledNotification.NotificationStyle.kGameMessageNegative);
                        }
                        cancelled = true;
                    }

                    if (cancelled)
                    {
                        Cancelled = true;
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(parameters.Actor, parameters.Target, e);
            }
        }

        public override bool Run()
        {
            try
            {
                if ((!Cancelled && (mMasseuse != null)) && (mMassagee != null))
                {
                    MassageTable.GiveMassage entry = MassageTable.GiveMassage.Singleton.CreateInstance(Target, mMasseuse, mMasseuse.InheritedPriority(), Autonomous, true) as MassageTable.GiveMassage;
                    if (entry != null)
                    {
                        entry.mMassInfo = mMassInfo;
                        entry.mMassagee = mMassagee;
                        if (!mMasseuse.InteractionQueue.Add(entry))
                        {
                            return false;
                        }

                        MassageTable.GiveMassage.Definition interactionDefinition = entry.InteractionDefinition as MassageTable.GiveMassage.Definition;
                        interactionDefinition.mMassInfo = mMassInfo;
                    }
                }
                return false;
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

        private static List<Sim> GetValidMassageSims(MassageTable ths, Sim actor, bool romantic, bool autonomous)
        {
            List<Sim> list = new List<Sim>(ths.LotCurrent.GetSims());
            list.Remove(actor);

            for(int i=list.Count-1; i>=0; i--)
            {
                Sim sim = list[i];

                if ((sim.SimDescription.ChildOrBelow || !sim.IsHuman) || sim.SimDescription.IsFrankenstein)
                {
                    list.RemoveAt(i);
                    continue;
                }
                else if ((sim.InteractionQueue == null) || !sim.InteractionQueue.CanPlayerQueue())
                {
                    list.RemoveAt(i);
                    continue;
                }

                if (romantic)
                {
                    string reason;
                    GreyedOutTooltipCallback callback = null;
                    if (!CommonSocials.CanGetRomantic(actor, sim, autonomous, false, true, ref callback, out reason))
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }
            }
            return list;
        }

        public class Definition : MassageTable.GiveGet.Definition
        {
            public Definition()
            { }
            public Definition(bool getting, MassageTable.MassageInfo massInfo, string[] menuPath)
                : base(getting, massInfo, menuPath)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new MassageTableGiveGet();
                result.Init(ref parameters);
                return result;
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                bool romantic = ((mMassInfo.mType == MassageTable.MassageType.Romantic) || (mMassInfo.mType == MassageTable.MassageType.AmazingRomantic));

                NumSelectableRows = 0x1;
                List<Sim> validMassageSims = GetValidMassageSims(parameters.Target as MassageTable, parameters.Actor as Sim, romantic, parameters.Autonomous);
                PopulateSimPicker(ref parameters, out listObjs, out headers, validMassageSims, false);
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, MassageTable target, List<InteractionObjectPair> results)
            {
                try
                {
                    if (!actor.SimDescription.IsFrankenstein)
                    {
                        string[] givePath = new string[] { MassageTable.LocalizeString("GiveMassage", new object[0x0]) + Localization.Ellipsis };
                        string[] getPath = new string[] { MassageTable.LocalizeString("GetMassage", new object[0x0]) + Localization.Ellipsis };

                        foreach (MassageTable.MassageInfo info in MassageTable.MassageInfos)
                        {
                            bool flag = true;
                            bool flag2 = true;
                            if (info.mType == MassageTable.MassageType.Romantic)
                            {
                                if (actor.TraitManager.HasElement(TraitNames.Flirty))
                                {
                                    flag = false;
                                }
                            }
                            else if (info.mType == MassageTable.MassageType.AmazingRomantic)
                            {
                                flag2 = false;
                                if (!actor.TraitManager.HasElement(TraitNames.Flirty))
                                {
                                    flag = false;
                                }
                            }

                            if (flag)
                            {
                                results.Add(new InteractionObjectPair(new Definition(false, info, givePath), target));
                            }

                            if (flag2)
                            {
                                results.Add(new InteractionObjectPair(new Definition(true, info, getPath), target));
                            }
                        }
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(actor, target, e);
                }
            }

        }
    }
}
