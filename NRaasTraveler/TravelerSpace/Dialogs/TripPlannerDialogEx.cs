using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Dialogs
{
    public class TripPlannerDialogEx : TripPlannerDialog
    {
        public TripPlannerDialogEx(List<IDestinationInfo> destinations, List<int> durations, List<ISimTravelInfo> simTravelInfoList, ISimTravelInfo defaultTraveler, ModalDialog.PauseMode pauseMode)
            : base(destinations, durations, simTravelInfoList, defaultTraveler, pauseMode)
        {
            mDestinationGrid.Clear();

            PopulateDestinationGrids(destinations);

            mDestinationGrid.ItemClicked -= OnDestinationGridItemClicked;
            mDestinationGrid.ItemClicked += OnDestinationGridItemClickedEx;

            mSimListGrid[0x0].ItemDoubleClicked -= OnGridItemDoubleClicked;
            mSimListGrid[0x0].ItemDoubleClicked += OnGridItemDoubleClickedEx;

            mSimListGrid[0x1].ItemDoubleClicked -= OnGridItemDoubleClicked;
            mSimListGrid[0x1].ItemDoubleClicked += OnGridItemDoubleClickedEx;

            mAcceptButton.Click -= OnOKClick;
            mAcceptButton.Click += OnOKClickEx;

            mLeftArrow.Click -= OnArrowClick;
            mLeftArrow.Click += OnArrowClickEx;

            mRightArrow.Click -= OnArrowClick;
            mRightArrow.Click += OnArrowClickEx;

            mAcceptButton.Enabled = (GameStates.sTravelData == null);
            mAcceptButton.TooltipText = Common.Localize("Travel::IncludeAll");

            if (GameStates.sTravelData != null)
            {
                mAcceptButton.Enabled = (mSimListGrid[1].Count == GameStates.sTravelData.mTravelerIds.Count);
            }
        }
      
        private void OnArrowClickEx(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                while (mSimTravelInfoList[0].Remove(mActivator)) ;

                if (sender.ID != 0x4)
                {
                    ISimTravelInfo selectedTag = mSimListGrid[0].SelectedTag as ISimTravelInfo;
                    if (selectedTag == mActivator)
                    {
                        PopulateGrids();
                        return;
                    }
                }

                OnArrowClick(sender, eventArgs);

                if (GameStates.sTravelData != null)
                {
                    mAcceptButton.Enabled = (mSimListGrid[1].Count == GameStates.sTravelData.mTravelerIds.Count);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnArrowClickEx", e);
            }
        }

        private void OnOKClickEx(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                while (mSimTravelInfoList[1].Remove(mActivator)) ;

                if (!mBlockInput)
                {
                    if (mSimListGrid[0x1].Count == 0)
                    {
                        mResult = new Result(mCurrentDuration, 0, mDestinationGrid.SelectedTag as IDestinationInfo, new List<ISimDescription>());

                        OnTriggerOk();
                        return;
                    }
                }

                OnOKClick(sender, eventArgs);
            }
            catch (Exception e)
            {
                Common.Exception("OnOKClickEx", e);
            }
        }

        private void OnGridItemDoubleClickedEx(ItemGrid sender, ItemGridCellClickEvent itemClicked)
        {
            try
            {
                bool clear = false;

                if (!mBlockInput)
                {
                    ISimTravelInfo mTag = itemClicked.mTag as ISimTravelInfo;
                    if (mActivator == mTag)
                    {
                        if (sender.ID == 0xbd)
                        {
                            clear = true;

                            mSimTravelInfoList[0].Add(mActivator);

                            mSimTravelInfoList[0].AddRange(mSimTravelInfoList[1]);

                            mSimTravelInfoList[1].Clear();
                        }
                        else
                        {
                            while (mSimTravelInfoList[0].Remove(mActivator)) ;

                            PopulateGrids();
                            return;
                        }
                    }
                }

                OnGridItemDoubleClicked(sender, itemClicked);

                if (clear)
                {
                    mSimListGrid[0x1].Clear();
                }

                if (GameStates.sTravelData != null)
                {
                    mAcceptButton.Enabled = (mSimListGrid[1].Count == GameStates.sTravelData.mTravelerIds.Count);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnGridItemDoubleClickedEx", e);
            }
        }

        private new void PopulateDestinationGrids(List<IDestinationInfo> destinations)
        {
            foreach (IDestinationInfo info in destinations)
            {
                WindowBase windowByExportID = UIManager.LoadLayout(ResourceKey.CreateUILayoutKey("TripPlannerDestinationEntry", 0x0)).GetWindowByExportID(0x1);
                if (windowByExportID != null)
                {
                    windowByExportID.TooltipText = info.Name;
                    Window childByID = windowByExportID.GetChildByID(0xa0, true) as Window;

                    // Stops the game from darkening the image using a transperancy
                    childByID.ShadeColor = new Color(0xffffffff);

                    ImageDrawable drawable = childByID.Drawable as ImageDrawable;
                    if (drawable != null)
                    {
                        drawable.Image = WorldData.GetConfirmImage(info.Index);
                        childByID.Invalidate();
                    }
                    mDestinationGrid.AddItem(new ItemGridCellItem(windowByExportID, info));
                }
            }
            mDestinationGrid.SelectedItem = 0x0;
            UpdateDestination(mDestinationGrid.SelectedTag as IDestinationInfo);
        }

        private new void UpdateDestination(IDestinationInfo destination)
        {
            mDestinationDescription.Caption = destination.Description;
            mDestinationDescription.CursorIndex = 0x0;
            mConfirmDestinationDescription.Caption = destination.ConfirmDescription;

            ImageDrawable drawable = mConfirmDestinationImage.Drawable as ImageDrawable;
            if (drawable != null)
            {
                drawable.Image = WorldData.GetConfirmImage(destination.Index);
            }

            mConfirmDestinationImage.Invalidate();
            PopulateGrids();
        }

        private void OnDestinationGridItemClickedEx(ItemGrid sender, ItemGridCellClickEvent itemClicked)
        {
            try
            {
                bool wasClear = (mSimListGrid[1].Count == 0);

                Audio.StartSound("ui_secondary_button");
                UpdateDestination(itemClicked.mTag as IDestinationInfo);

                if (wasClear)
                {
                    mSimListGrid[1].Clear();
                }

                if (GameStates.sTravelData != null)
                {
                    mAcceptButton.Enabled = (mSimListGrid[1].Count == GameStates.sTravelData.mTravelerIds.Count);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnDestinationGridItemClicked", e);
            }
        }

        public new static Result Show(List<IDestinationInfo> destinations, List<int> durations, List<ISimTravelInfo> simTravelInfoList, ISimTravelInfo defaultTraveler)
        {
            if (ModalDialog.EnableModalDialogs && (sDialog == null))
            {
                sDialog = new TripPlannerDialogEx(destinations, durations, simTravelInfoList, defaultTraveler, ModalDialog.PauseMode.PauseSimulator);
                sDialog.StartModal();
                Result mResult = sDialog.mResult;
                sDialog = null;
                return mResult;
            }
            return null;
        }
    }
}