using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Dialogs
{
    public class FamilyTreeDialog : Sims3.UI.FamilyTreeDialog
    {
        static MiniSimDescription sConstructorStub;

        Dictionary<IMiniSimDescription, SimTreeInfo> mSimTreeInfoEx = new Dictionary<IMiniSimDescription, SimTreeInfo>();

        static FamilyTreeDialog()
        {
            sConstructorStub = new MiniSimDescription();
            sConstructorStub.mGenealogy = new Genealogy();
        }

        public FamilyTreeDialog(IMiniSimDescription sim)
            : base(sConstructorStub)
        {
            mSimTreeInfos.Clear();
            mSimTreeInfoEx.Clear();

            RefreshTree(sim);
        }

        public static void UpdateThumb(FamilyTreeThumb ths)
        {
            ImageDrawable drawable = ths.ThumbWin.Drawable as ImageDrawable;
            if (drawable != null)
            {
                drawable.Image = UIManager.GetThumbnailImage(MiniSims.GetThumbnailKey(ths.SimDescription, ThumbnailSize.Large, 0x0));
                ths.ThumbWin.Invalidate();
            }
        }

        // Retain
        private new FamilyTreeThumb CreateFamilyTreeThumb(IMiniSimDescription desc, Vector2 pos)
        {
            FamilyTreeThumb windowByExportID = UIManager.LoadLayout(ResourceKey.CreateUILayoutKey("HUDSimologyFamilyTreeThumbItem", 0x0)).GetWindowByExportID(0x1) as FamilyTreeThumb;
            //windowByExportID.SimDescription = desc;
            windowByExportID.mSimDesc = desc;

            UpdateThumb(windowByExportID);

            windowByExportID.Position = pos;
            mThumbParentWin.AddChild(windowByExportID);

            // Custom
            windowByExportID.MouseUp += OnFamilyTreeThumbClick;
            windowByExportID.TooltipText = desc.FullName;
            return windowByExportID;
        }

        private PartnerType GetPartnerType(SimTreeInfo simA, SimTreeInfo simB)
        {
            if (simA == null) return PartnerType.None;

            if (simB == null) return PartnerType.None;

            return GetPartnerType(simA.mSimDescription, simB.mSimDescription);
        }
        private PartnerType GetPartnerType(IMiniSimDescription simA, IMiniSimDescription simB)
        {
            if ((simA.CASGenealogy.ISpouse != null) && (simA.CASGenealogy.ISpouse.IMiniSimDescription == simB))
            {
                return simA.CASGenealogy.PartnerType;
            }

            IMiniSimDescription spouseA = FindDeadPartner(simA, false);
            if (spouseA != simB)
            {
                return PartnerType.None;
            }

            IMiniSimDescription spouseB = FindDeadPartner(simB, false);
            if (spouseB != simA)
            {
                return PartnerType.None;
            }

            SimDescription a = simA as SimDescription;
            SimDescription b = simB as SimDescription;

            if ((a != null) && (b != null))
            {
                Relationship relation = Relationship.Get(a, b, false);
                if (relation == null)
                {
                    return PartnerType.None;
                }
                else
                {
                    switch (relation.CurrentLTR)
                    {
                        case LongTermRelationshipTypes.Ex:
                        case LongTermRelationshipTypes.Partner:
                            return PartnerType.BGFriend;
                        case LongTermRelationshipTypes.ExSpouse:
                        case LongTermRelationshipTypes.Spouse:
                            return PartnerType.Marriage;
                        case LongTermRelationshipTypes.Fiancee:
                            return PartnerType.Fiance;
                        default:
                            return PartnerType.None;
                    }
                }
            }

            return PartnerType.None;
        }

        // Retain
        private new Rect GenericLayoutParents(int x, int y, SimTreeInfo simA, SimTreeInfo simB)
        {
            return GenericLayoutParents(x, y, simA, simB, true, true);
        }

        // Retain
        private new Rect GenericLayoutParents(int x, int y, SimTreeInfo simA, SimTreeInfo simB, bool fromHalf, bool toHalf)
        {
            if (simA != null)
            {
                if (simA.mWin == null)
                {
                    simA.mWin = CreateFamilyTreeThumb(simA.mSimDescription, new Vector2((float)x, (float)y));
                    simA.mBottomBounds = simA.mWin.Area;
                }

                x = (int)simA.mWin.Area.BottomRight.x;
            }

            if (simB != null)
            {
                x += (int)X_DIST_BETWEEN_THUMBS;
                if (simB.mWin == null)
                {
                    simB.mWin = CreateFamilyTreeThumb(simB.mSimDescription, new Vector2((float)x, (float)y));
                    simB.mBottomBounds = simB.mWin.Area;
                }

                x = (int)simB.mWin.Area.BottomRight.x;
            }

            PartnerType partnerType = GetPartnerType(simA, simB);

            // Changed
            if ((simA == null) || (simB == null) || (partnerType == PartnerType.None))
            {
                if ((simA != null) && (simB != null))
                {
                    return Rect.Union(simA.mWin.Area, simB.mWin.Area);
                }
                else if (simA != null)
                {
                    return simA.mWin.Area;
                }
                else if (simB != null)
                {
                    return simB.mWin.Area;
                }
                else
                {
                    return new Rect((float)x, (float)y, (float)x, (float)y);
                }
            }
            else
            {
                Color barColor = new Color(0xff367cdd);
                switch (partnerType)
                {
                    case PartnerType.Fiance:
                        barColor = new Color(0xffebd03e);
                        break;

                    case PartnerType.BGFriend:
                        barColor = new Color(0xffdb4dc5);
                        break;
                }

                Rect b = Rect.Union(Rect.Union(ConnectSims(simA, simB, false, fromHalf, toHalf, barColor), simA.mWin.Area), simB.mWin.Area);
                simA.mBottomBounds = Rect.Union(simA.mBottomBounds, b);
                simB.mBottomBounds = Rect.Union(simB.mBottomBounds, b);

                return b;
            }
        }

        private new SimTreeInfo GetSimTreeInfo(IMiniSimDescription desc)
        {
            return GetSimTreeInfo(desc, true);
        }
        private new SimTreeInfo GetSimTreeInfo(IMiniSimDescription desc, bool addIfNotPresent)
        {
            if (desc != null)
            {
                SimTreeInfo info;
                if (mSimTreeInfoEx.TryGetValue(desc, out info))
                {
                    return info;
                }

                if (addIfNotPresent)
                {
                    SimTreeInfo item = new SimTreeInfo(desc);
                    mSimTreeInfoEx.Add(desc, item);

                    mSimTreeInfos.Add(item);
                    return item;
                }
            }
            return null;
        }

        private IMiniSimDescription FindDeadPartner(IMiniSimDescription miniSim, bool mustBeSpouse)
        {
            if (miniSim.CASGenealogy.ISpouse != null)
            {
                if (mustBeSpouse)
                {
                    if (miniSim.CASGenealogy.PartnerType != PartnerType.Marriage)
                    {
                        return null;
                    }
                }

                return miniSim.CASGenealogy.ISpouse.IMiniSimDescription;
            }

            SimDescription sim = miniSim as SimDescription;
            if (sim == null)
            {
                return null;
            }

            Relationship choice = null;
            DateAndTime latest = new DateAndTime();

            bool endNow = false;

            foreach (Relationship relation in Relationship.Get(sim))
            {
                SimDescription other = relation.GetOtherSimDescription(sim);
                if (other == null) continue;

                switch (relation.CurrentLTR)
                {
                    case LongTermRelationshipTypes.Partner:
                    case LongTermRelationshipTypes.Spouse:
                    case LongTermRelationshipTypes.Fiancee:
                        if (!miniSim.IsDead)
                        {
                            // Live partners take precedence
                            if (!other.IsDead)
                            {
                                choice = relation;
                                endNow = true;
                                break;
                            }
                        }
                        break;
                }

                if (endNow)
                {
                    break;
                }

                switch (relation.CurrentLTR)
                {
                    case LongTermRelationshipTypes.Ex:
                    case LongTermRelationshipTypes.Partner:
                    case LongTermRelationshipTypes.ExSpouse:
                    case LongTermRelationshipTypes.Spouse:
                    case LongTermRelationshipTypes.Fiancee:
                        if ((!miniSim.IsDead) && (!other.IsDead)) continue;

                        if ((choice == null) || (latest < relation.LTR.WhenStateStarted))
                        {
                            choice = relation;
                            latest = relation.LTR.WhenStateStarted;

                        }
                        break;
                }
            }

            if (choice == null)
            {
                return null;
            }
            else
            {
                if (mustBeSpouse)
                {
                    if (endNow)
                    {
                        return null;
                    }

                    switch (choice.CurrentLTR)
                    {
                        case LongTermRelationshipTypes.Spouse:
                        case LongTermRelationshipTypes.ExSpouse:
                            break;
                        default:
                            return null;
                    }
                }

                return choice.GetOtherSimDescription(sim);
            }
        }

        // Retain
        private new void Layout(int x, int y)
        {
            StringBuilder msg = new StringBuilder();
            
            msg.Append("Layout" + Common.NewLine);

            try
            {
                SimTreeInfo currentSimInfo = GetSimTreeInfo(mCurrentSim);

                Dictionary<IMiniSimDescription, SimTreeInfo> usedInfo = new Dictionary<IMiniSimDescription, SimTreeInfo>();
                RecurseLayoutParents(x, y, currentSimInfo, usedInfo, NRaas.MasterController.Settings.mFamilyTreeLevels);

                SimTreeInfo spouseInfo = null;
                if (mCurrentSim.CASGenealogy.ISpouse != null)
                {
                    spouseInfo = GetSimTreeInfo(mCurrentSim.CASGenealogy.ISpouse.IMiniSimDescription);
                }
                else
                {
                    spouseInfo = GetSimTreeInfo(FindDeadPartner(mCurrentSim, true));

                    if (spouseInfo != null)
                    {
                        IMiniSimDescription otherSpouse = FindDeadPartner(spouseInfo.mSimDescription, true);
                        if (otherSpouse != mCurrentSim)
                        {
                            spouseInfo = null;
                        }
                    }
                }

                msg.Append("A");

                bool flag = false;
                if (spouseInfo != null)
                {
                    List<IMiniSimDescription> childList = new List<IMiniSimDescription>();
                    flag = GetChildren(mCurrentSim, spouseInfo.mSimDescription, ref childList, true);
                }

                msg.Append("B");

                Rect rect = GenericLayoutParents(x, y, currentSimInfo, spouseInfo, !flag, true);
                IMiniSimDescription parent1 = null;
                IMiniSimDescription parent2 = null;
                GetParents(mCurrentSim, ref parent1, ref parent2);

                msg.Append("C");

                List<IMiniSimDescription> list2 = new List<IMiniSimDescription>();
                List<IMiniSimDescription> list3 = new List<IMiniSimDescription>();
                foreach (IGenealogy genealogy in mCurrentSim.CASGenealogy.ISiblings)
                {
                    try
                    {
                        if (mCurrentSim.CASGenealogy.IsHalfSibling(genealogy) || ((mCurrentSim.CASGenealogy.IParents.Count > 0x1) && (mCurrentSim.CASGenealogy.IParents[0x0].ISpouse != mCurrentSim.CASGenealogy.IParents[0x1])))
                        {
                            list3.Add(genealogy.IMiniSimDescription);
                        }
                        else
                        {
                            list2.Add(genealogy.IMiniSimDescription);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(genealogy.IMiniSimDescription, null, msg.ToString(), e);
                    }
                }

                msg.Append("D");

                x = (int)currentSimInfo.mWin.Position.x;
                foreach (IMiniSimDescription description3 in list2)
                {
                    try
                    {
                        x -= (int)(FamilyTreeThumb.kRegularArea.x + X_DIST_BETWEEN_THUMBS);
                        SimTreeInfo simTreeInfo = GetSimTreeInfo(description3);
                        simTreeInfo.mWin = CreateFamilyTreeThumb(description3, new Vector2((float)x, (float)y));
                        simTreeInfo.mBottomBounds = simTreeInfo.mWin.Area;
                    }
                    catch (Exception e)
                    {
                        Common.Exception(description3, null, msg.ToString(), e);
                    }
                }

                msg.Append("E");

                foreach (IMiniSimDescription description4 in list3)
                {
                    try
                    {
                        x -= (int)(FamilyTreeThumb.kRegularArea.x + X_DIST_BETWEEN_THUMBS);
                        SimTreeInfo info3 = GetSimTreeInfo(description4);
                        info3.mWin = CreateFamilyTreeThumb(description4, new Vector2((float)x, (float)y));
                        info3.mBottomBounds = info3.mWin.Area;
                    }
                    catch (Exception e)
                    {
                        Common.Exception(description4, null, msg.ToString(), e);
                    }
                }

                msg.Append("F");

                if (list2.Count > 0x0)
                {
                    IMiniSimDescription desc = list2[list2.Count - 0x1];
                    if (desc != mCurrentSim)
                    {
                        Rect rect2 = ConnectSims(currentSimInfo, GetSimTreeInfo(desc), true, true, true, new Color(0xff367cdd));
                        foreach (IMiniSimDescription description6 in list2)
                        {
                            try
                            {
                                if ((description6 != desc) && (description6 != mCurrentSim))
                                {
                                    SimTreeInfo info4 = GetSimTreeInfo(description6);
                                    Rect area = info4.mWin.Area;
                                    area.TopLeft = new Vector2(area.TopLeft.x, rect2.TopLeft.y);
                                    area.BottomRight = new Vector2(area.BottomRight.x, rect2.TopLeft.y);
                                    ConnectBoundsV(area, info4.mWin.Area, new Color(0xff367cdd));
                                }
                            }
                            catch (Exception e)
                            {
                                Common.Exception(description6, null, msg.ToString(), e);
                            }
                        }
                    }
                }

                msg.Append("G");

                foreach (IMiniSimDescription description7 in list3)
                {
                    try
                    {
                        SimTreeInfo info5 = GetSimTreeInfo(description7);
                        Window childWindow = LoadCornerPiece(CONNECTOR_IMAGE_CORNER_TOP_LEFT);
                        Window window2 = LoadStraightPiece(CONNECTOR_IMAGE_HORIZONTAL_DOTTED);
                        childWindow.Position = new Vector2(info5.mWin.Position.x + ((info5.mWin.Area.Width - childWindow.Area.Width) / 2f), info5.mWin.Position.y - childWindow.Area.Height);
                        window2.Position = new Vector2(childWindow.Area.BottomRight.x, childWindow.Area.TopLeft.y + 1f);
                        Rect rect4 = window2.Area;
                        rect4.Width = 10f;
                        window2.Area = rect4;
                        mThumbParentWin.AddChild(childWindow);
                        mThumbParentWin.AddChild(window2);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(description7, null, msg.ToString(), e);
                    }
                }

                msg.Append("H");

                x = (int)rect.TopLeft.x;
                y = (int)currentSimInfo.mWin.Area.BottomRight.y;
                if (spouseInfo != null)
                {
                    x = (int)(rect.TopLeft.x + ((rect.Width - FamilyTreeThumb.kRegularArea.x) / 2f));
                }

                RecurseLayoutChildren(x, y, currentSimInfo, spouseInfo, NRaas.MasterController.Settings.mFamilyTreeLevels, true, false);
                if (spouseInfo != null)
                {
                    x = (int)(rect.TopLeft.x + (FamilyTreeThumb.kRegularArea.x / 2f));
                    RecurseLayoutChildren(x, y, currentSimInfo, spouseInfo, NRaas.MasterController.Settings.mFamilyTreeLevels, false, true);
                }

                msg.Append("I");

                foreach (SimTreeInfo info6 in mSimTreeInfoEx.Values)
                {
                    try
                    {
                        if (info6.mWin == null) continue;

                        if (info6.mSimDescription == null) continue;

                        if (info6.mSimDescription.CASGenealogy == null) continue;

                        msg.Append("J");

                        bool flag2 = true;
                        if (flag2 && (info6.mSimDescription.CASGenealogy.IParents.Count > 0x0))
                        {
                            SimTreeInfo item = GetSimTreeInfo(info6.mSimDescription.CASGenealogy.IParents[0x0].IMiniSimDescription, false);
                            flag2 &= (item != null);
                        }

                        msg.Append("K");

                        if (flag2 && (info6.mSimDescription.CASGenealogy.IParents.Count > 0x1))
                        {
                            SimTreeInfo info8 = GetSimTreeInfo(info6.mSimDescription.CASGenealogy.IParents[0x1].IMiniSimDescription, false);
                            flag2 &= (info8 != null);
                        }

                        msg.Append("L");

                        if ((flag2) && (info6.mSimDescription.CASGenealogy.ISpouse != null))
                        {
                            SimTreeInfo info9 = GetSimTreeInfo(info6.mSimDescription.CASGenealogy.ISpouse.IMiniSimDescription, false);
                            flag2 &= (info9 != null);
                        }

                        msg.Append("M");

                        if (flag2 && (info6.mSimDescription.CASGenealogy.ISiblings.Count > 0x0))
                        {
                            foreach (IGenealogy genealogy2 in info6.mSimDescription.CASGenealogy.ISiblings)
                            {
                                SimTreeInfo info10 = GetSimTreeInfo(genealogy2.IMiniSimDescription, false);
                                flag2 &= (info10 != null);
                            }
                        }

                        msg.Append("N");

                        if (flag2 && (info6.mSimDescription.CASGenealogy.IChildren.Count > 0x0))
                        {
                            foreach (IGenealogy genealogy3 in info6.mSimDescription.CASGenealogy.IChildren)
                            {
                                SimTreeInfo info11 = GetSimTreeInfo(genealogy3.IMiniSimDescription, false);
                                flag2 &= (info11 != null);
                            }
                        }

                        msg.Append("O");

                        if (!flag2)
                        {
                            info6.mWin.ShowMoreInfo = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(info6.mSimDescription, null, msg.ToString(), e);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(msg.ToString(), e);
            }
        }

        // Retain
        private void OnFamilyTreeThumbClick(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                FamilyTreeThumb thumb = sender as FamilyTreeThumb;
                if (thumb != null)
                {
                    if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
                    {
                        RefreshTree(thumb.SimDescription);
                    }
                    else
                    {
                        ShowMenuTask.Perform(thumb.SimDescription);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnFamilyTreeThumbClick", e);
            }
        }

        public class ShowMenuTask : Common.FunctionTask
        {
            IMiniSimDescription mSim;

            protected ShowMenuTask(IMiniSimDescription sim)
            {
                mSim = sim;
            }

            public static void Perform(IMiniSimDescription sim)
            {
                new ShowMenuTask(sim).AddToSimulator();
            }

            protected override void OnPerform()
            {
                Sims3.Gameplay.UI.PieMenu.ClearInteractions();
                Sims3.Gameplay.UI.PieMenu.HidePieMenuSimHead = true;

                Sim activeActor = Sim.ActiveActor;
                if (activeActor != null)
                {
                    Sim sim = null;

                    SimDescription simDesc = mSim as SimDescription;
                    if (simDesc != null)
                    {
                        sim = simDesc.CreatedSim;
                    }

                    if (sim != null)
                    {
                        if (activeActor.InteractionQueue.CanPlayerQueue())
                        {
                            bool success = false;
                            try
                            {
                                List<InteractionObjectPair> interactions = new List<InteractionObjectPair>();
                                interactions.Add(new InteractionObjectPair(CallOver.Singleton, sim));

                                List<InteractionObjectPair> others = sim.GetAllInteractionsForActor(activeActor);
                                if (others != null)
                                {
                                    interactions.AddRange(others);
                                }

                                InteractionDefinitionOptionList.Perform(activeActor, new GameObjectHit(GameObjectHitType.Object), interactions);
                                success = true;
                            }
                            catch (Exception e)
                            {
                                Common.Exception(activeActor, sim, e);
                            }

                            if (!success)
                            {
                                List<InteractionObjectPair> immediateInteractions = new List<InteractionObjectPair>();

                                foreach (InteractionObjectPair interaction in sim.mInteractions)
                                {
                                    if (interaction.InteractionDefinition is IImmediateInteractionDefinition)
                                    {
                                        immediateInteractions.Add(interaction);
                                    }
                                }

                                List<InteractionObjectPair> interactions = sim.BuildInteractions(activeActor, immediateInteractions);

                                InteractionDefinitionOptionList.Perform(activeActor, new GameObjectHit(GameObjectHitType.Object), interactions);
                            }
                        }
                    }
                    else
                    {
                        List<InteractionObjectPair> interactions = new List<InteractionObjectPair>();

                        interactions.Add(new InteractionObjectPair(NRaas.MasterControllerSpace.Interactions.SimDescriptionInteraction.Singleton, new SimDescriptionObject(mSim)));

                        InteractionDefinitionOptionList.Perform(activeActor, new GameObjectHit(GameObjectHitType.Object), interactions);
                    }
                }
            }
        }

        // Retain
        private new Rect RecurseLayoutChildren(int x, int y, SimTreeInfo parent1, SimTreeInfo parent2, int recurseLevel, bool growXPositive, bool notWithParent2)
        {
            Rect rect = new Rect((float)x, (float)y, (float)x, (float)y);

            StringBuilder msg = new StringBuilder();
            msg.Append("RecurseLayoutChildren" + Common.NewLine);

            try
            {
                if (recurseLevel == 0x0)
                {
                    return rect;
                }

                msg.Append("A");

                int num = y + ((int)Y_DIST_BETWEEN_THUMBS);
                int num2 = x;
                List<IMiniSimDescription> childList = new List<IMiniSimDescription>();
                if (!GetChildren((parent1 != null) ? parent1.mSimDescription : null, (parent2 != null) ? parent2.mSimDescription : null, ref childList, notWithParent2))
                {
                    return rect;
                }

                msg.Append("B");

                SimTreeInfo firstChild = null;
                SimTreeInfo lastChild = null;

                Rect a = new Rect((float)x, (float)num, (float)x, (float)num);
                foreach (IMiniSimDescription description in childList)
                {
                    try
                    {
                        if (description == null) continue;

                        lastChild = GetSimTreeInfo(description);
                        if (firstChild == null)
                        {
                            firstChild = lastChild;
                        }

                        if (growXPositive)
                        {
                            num2 = (int)a.BottomRight.x;
                        }
                        else
                        {
                            num2 = (int)(a.TopLeft.x - FamilyTreeThumb.kRegularArea.x);
                        }

                        lastChild.mWin = CreateFamilyTreeThumb(lastChild.mSimDescription, new Vector2((float)num2, (float)num));
                        lastChild.mBottomBounds = lastChild.mWin.Area;
                        a = Rect.Union(a, lastChild.mWin.Area);
                        Rect b = RecurseLayoutChildren(num2, (int)lastChild.mWin.Area.BottomRight.y, lastChild, null, recurseLevel - 0x1, growXPositive, false);
                        a = Rect.Union(a, b);
                        if (growXPositive)
                        {
                            a = new Rect(a.TopLeft, new Vector2(a.BottomRight.x + X_DIST_BETWEEN_THUMBS, a.BottomRight.y));
                        }
                        else
                        {
                            a = new Rect(new Vector2(a.TopLeft.x - X_DIST_BETWEEN_THUMBS, a.TopLeft.y), a.BottomRight);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(description, null, msg.ToString(), e);
                    }
                }

                msg.Append("C");

                if (firstChild != null)
                {
                    if (((parent1 != null) && (parent2 != null)) && !notWithParent2)
                    {
                        Rect area = firstChild.mWin.Area;
                        area.TopLeft = new Vector2(area.TopLeft.x, parent1.mBottomBounds.BottomRight.y);
                        area.BottomRight = new Vector2(area.BottomRight.x, parent1.mBottomBounds.BottomRight.y);
                        ConnectBoundsV(area, firstChild.mWin.Area, new Color(0xff367cdd));
                    }
                    else
                    {
                        ConnectSims(parent1, firstChild, false, parent2 == null, notWithParent2 || (parent2 == null), new Color(0xff367cdd));
                    }
                }

                msg.Append("D");

                if (firstChild != lastChild)
                {
                    Rect rect5 = ConnectSims(firstChild, lastChild, true, true, true, new Color(0xff367cdd));
                    foreach (IMiniSimDescription description2 in childList)
                    {
                        SimTreeInfo info4 = GetSimTreeInfo(description2);
                        if ((info4 != firstChild) && (info4 != lastChild))
                        {
                            Rect boundsA = info4.mWin.Area;
                            boundsA.TopLeft = new Vector2(boundsA.TopLeft.x, rect5.TopLeft.y);
                            boundsA.BottomRight = new Vector2(boundsA.BottomRight.x, rect5.TopLeft.y);
                            ConnectBoundsV(boundsA, info4.mWin.Area, new Color(0xff367cdd));
                        }
                    }
                }
                return a;
            }
            catch (Exception e)
            {
                Common.Exception((parent1 != null) ? parent1.mSimDescription : null, (parent2 != null) ? parent2.mSimDescription : null, msg.ToString(), e);
                return rect;
            }
        }

        // Retain
        private Rect RecurseLayoutParents(int x, int y, SimTreeInfo child, Dictionary<IMiniSimDescription,SimTreeInfo> usedInfo, int recurseLevel)
        {
            IMiniSimDescription parent1 = null;
            IMiniSimDescription parent2 = null;

            try
            {
                SimTreeInfo parent1Info = null;
                SimTreeInfo parent2Info = null;

                Rect boundsA = new Rect((float)x, (float)y, (float)x, (float)y);
                if (GetParents(child.mSimDescription, ref parent1, ref parent2))
                {
                    if ((parent1 != null) && (!usedInfo.ContainsKey(parent1)))
                    {
                        parent1Info = GetSimTreeInfo(parent1);
                        usedInfo.Add(parent1, parent1Info);
                    }

                    if ((parent2 != null) && (!usedInfo.ContainsKey(parent2)))
                    {
                        parent2Info = GetSimTreeInfo(parent2);
                        usedInfo.Add(parent2, parent2Info);
                    }

                    int num = (int)((y - FamilyTreeThumb.kRegularArea.y) - Y_DIST_BETWEEN_THUMBS);
                    if (recurseLevel == 0x1)
                    {
                        boundsA = GenericLayoutParents(x, num, parent1Info, parent2Info);
                    }
                    else
                    {
                        Rect rect2 = new Rect((float)x, (float)num, (float)x, (float)num);
                        if (parent1Info != null)
                        {
                            rect2 = RecurseLayoutParents(x, num, parent1Info, usedInfo, recurseLevel - 0x1);
                        }

                        if (parent2Info != null)
                        {
                            int num2 = (int)(rect2.BottomRight.x + X_DIST_BETWEEN_THUMBS);
                            rect2 = RecurseLayoutParents(num2, num, parent2Info, usedInfo, recurseLevel - 0x1);
                        }

                        boundsA = GenericLayoutParents(0x0, 0x0, parent1Info, parent2Info);
                    }
                }

                x = (int)boundsA.TopLeft.x;
                if ((parent1Info != null) && (parent2Info != null))
                {
                    x = (int)(boundsA.TopLeft.x + ((boundsA.Width - FamilyTreeThumb.kRegularArea.x) / 2f));
                }

                child.mWin = CreateFamilyTreeThumb(child.mSimDescription, new Vector2((float)x, (float)y));
                child.mBottomBounds = child.mWin.Area;
                if ((parent1Info != null) || (parent2Info != null))
                {
                    if (((parent1Info != null) && (parent2Info != null)) && (GetPartnerType(parent1, parent2) != PartnerType.None))
                    {
                        ConnectBoundsV(boundsA, child.mWin.Area, new Color(0xff367cdd));
                    }
                    else
                    {
                        if (parent1Info != null)
                        {
                            ConnectSims(parent1Info, child, false, true, false, new Color(0xff367cdd));
                        }

                        if (parent2Info != null)
                        {
                            ConnectSims(parent2Info, child, false, true, false, new Color(0xff367cdd));
                        }
                    }
                }

                return Rect.Union(boundsA, child.mWin.Area);
            }
            catch (Exception e)
            {
                string name = null;

                if (child.mSimDescription != null)
                {
                    name += Common.NewLine + "Child 1: " + child.mSimDescription.FullName;
                }

                if (parent1 != null)
                {
                    name += Common.NewLine + "Parent 1: " + parent1.FullName;
                }

                if (parent2 != null)
                {
                    name += Common.NewLine + "Parent 2: " + parent2.FullName;
                }

                Common.Exception(name, e);
                return new Rect();
            }
        }

        // Retain
        private new void RefreshTree(IMiniSimDescription sim)
        {
            WindowBase base2;
            mCurrentSim = sim;
            mNameText.Caption = sim.FullName;
            mSimTreeInfos.Clear();
            mSimTreeInfoEx.Clear();
            mThumbParentWin.DestroyAllChildren();
            mThumbParentWin.Position = Vector2.Zero;
            Rect area = mModalDialogWindow.Area;
            mThumbParentWin.Area = new Rect(mThumbParentWin.Position, mThumbParentWin.Position);
            int x = 0x0;
            int y = 0x0;

            // Changed
            Layout(x, y);

            int num3 = 0x5;
            int num4 = 0x5;
            uint index = 0x0;
            Rect a = mThumbParentWin.Area;
            for (base2 = mThumbParentWin.GetChildByIndex(index); base2 != null; base2 = mThumbParentWin.GetChildByIndex(++index))
            {
                a = Rect.Union(a, base2.Area);
            }

            mThumbParentWin.Area = new Rect(a.TopLeft - new Vector2((float)num3, (float)num4), a.BottomRight + new Vector2((float)num3, (float)num4));
            index = 0x0;
            for (base2 = mThumbParentWin.GetChildByIndex(index); base2 != null; base2 = mThumbParentWin.GetChildByIndex(++index))
            {
                int num6 = (int)(base2.Position.x - mThumbParentWin.Position.x);
                int num7 = (int)(base2.Position.y - mThumbParentWin.Position.y);
                base2.Position = new Vector2((float)num6, (float)num7);
            }

            mThumbParentWin.Position = new Vector2(0f, 0f);
            float num8 = UIManager.ScreenArea.Width - (2f * BUFFER);
            mScrollWindow.Area = new Rect(mThumbParentWin.Position, new Vector2(Math.Min(num8, mThumbParentWin.Area.BottomRight.x), mThumbParentWin.Area.BottomRight.y + SCROLL_BUFFER));
            Rect rect3 = mScrollWindow.Area;
            area = mModalDialogWindow.Area;
            area.Width = Math.Max(MIN_WIDTH, rect3.Width + (2f * BUFFER));
            area.Height = Math.Max(MIN_HEIGHT, (mFamilyTreeText.Area.BottomRight.y + (2f * BUFFER)) + rect3.Height);
            mModalDialogWindow.Area = area;
            area.Height -= mFamilyTreeText.Area.BottomRight.y;
            int width = (int)rect3.Width;
            int height = (int)rect3.Height;
            int num11 = (int)area.Width;
            int num12 = (int)area.Height;
            x = (num11 - width) / 0x2;
            y = ((num12 - height) / 0x2) + ((int)(mFamilyTreeText.Area.BottomRight.y - 15f));
            int num13 = x - ((int)rect3.TopLeft.x);
            int num14 = y - ((int)rect3.TopLeft.y);
            mScrollWindow.Position = new Vector2(mScrollWindow.Position.x + num13, mScrollWindow.Position.y + num14);
            SimTreeInfo simTreeInfo = GetSimTreeInfo(mCurrentSim);
            simTreeInfo.mWin.SelectedThumb = true;
            simTreeInfo.mWin.MoveToFront();
            area = mModalDialogWindow.Area;
            Rect rect4 = mModalDialogWindow.Parent.Area;
            float num15 = area.BottomRight.x - area.TopLeft.x;
            float num16 = area.BottomRight.y - area.TopLeft.y;
            float num17 = rect4.BottomRight.x - rect4.TopLeft.x;
            float num18 = rect4.BottomRight.y - rect4.TopLeft.y;
            float left = (float)Math.Round((double)((num17 - num15) / 2f));
            float top = (float)Math.Round((double)((num18 - num16) / 2f));
            area.Set(left, top, left + num15, top + num16);
            mModalDialogWindow.Area = area;
        }

        public new static void Show(IMiniSimDescription sim)
        {
            if (sDialog == null)
            {
                sDialog = new FamilyTreeDialog(sim);
                sDialog.StartModal();
                sDialog = null;
            }
        }
    }
}

