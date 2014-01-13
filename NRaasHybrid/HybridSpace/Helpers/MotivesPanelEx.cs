using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Helpers
{
    public class MotivesPanelEx
    {
        public static void OnMotivesChanged(MotivesPanel ths, SimInfo info)
        {
            Sim sim = Sim.ActiveActor;

            if (sim.SimDescription.IsMummy)
            {
                ths.mBladderMummyWraps.Visible = true;
                ths.mEnergyMummyWraps.Visible = true;
                ths.mBladderMotiveText.TextColor = new Color(0xffcbc7b6);
                ths.mEnergyMotiveText.TextColor = new Color(0xffcbc7b6);
            }
            else
            {
                ths.mBladderMummyWraps.Visible = false;
                ths.mEnergyMummyWraps.Visible = false;
                ths.mBladderMotiveText.TextColor = new Color(ths.mDefaultMotiveTextColor);
                ths.mEnergyMotiveText.TextColor = new Color(ths.mDefaultMotiveTextColor);
            }

            if (sim.SimDescription.IsFrankenstein)
            {
                ths.mHygieneFrankenSimWraps.Visible = true;
            }
            else
            {
                ths.mHygieneFrankenSimWraps.Visible = false;
            }

            if (sim.SimDescription.IsEP11Bot)
            {
                ths.mScrollBorderOverlayRobot.Visible = true;
                ths.mSocialRobotLock.Visible = !ths.mHudModel.RobotSocialMotiveEnabled;
                ths.mHygieneRobotLock.Visible = !ths.mHudModel.RobotFunMotiveEnabled;
            }
            else
            {
                ths.mScrollBorderOverlayRobot.Visible = false;
                ths.mSocialRobotLock.Visible = false;
                ths.mHygieneRobotLock.Visible = false;
            }

            if (sim.SimDescription.IsVampire)
            {
                ths.mScrollBorderOverlay.Visible = false;
                ths.mScrollBorderOverlayVampire.Visible = true;
            }
            else
            {
                ths.mScrollBorderOverlay.Visible = true;
                ths.mScrollBorderOverlayVampire.Visible = false;
            }

            if (sim.SimDescription.IsUnicorn)
            {
                ths.mScrollBorderOverlayUnicorn.Visible = true;
                if (info.mAge == CASAgeGenderFlags.Child)
                {
                    ths.mScrollBorderOverlayUnicornFun.Visible = false;
                }
                else
                {
                    ths.mScrollBorderOverlayUnicornFun.Visible = true;
                }
            }
            else
            {
                ths.mScrollBorderOverlayUnicorn.Visible = false;
                ths.mScrollBorderOverlayUnicornFun.Visible = false;
            }

            if (sim.SimDescription.IsGenie)
            {
                ths.mHungerGenieSparkle.Visible = info.OccultInfo.ShowMotiveOverlays;
                ths.mBladderGenieSparkle.Visible = info.OccultInfo.ShowMotiveOverlays;
                ths.mEnergyGenieSparkle.Visible = info.OccultInfo.ShowMotiveOverlays;
                ths.mSocialGenieSparkle.Visible = info.OccultInfo.ShowMotiveOverlays;
                ths.mHygieneGenieSparkle.Visible = info.OccultInfo.ShowMotiveOverlays;
                ths.mFunGenieSparkle.Visible = info.OccultInfo.ShowMotiveOverlays;
                ths.mScrollBorderOverlay.Visible = false;
                ths.mScrollBorderOverlayGenie.Visible = true;
            }
            else
            {
                ths.mHungerGenieSparkle.Visible = false;
                ths.mBladderGenieSparkle.Visible = false;
                ths.mEnergyGenieSparkle.Visible = false;
                ths.mSocialGenieSparkle.Visible = false;
                ths.mHygieneGenieSparkle.Visible = false;
                ths.mFunGenieSparkle.Visible = false;
                ths.mScrollBorderOverlay.Visible = true;
                ths.mScrollBorderOverlayGenie.Visible = false;
            }

            if (sim.SimDescription.IsPlantSim)
            {
                ths.mBladderPlantSimWraps.Visible = true;
                ths.mHungerPlantSimWraps.Visible = true;
                switch (info.mAge)
                {
                    case CASAgeGenderFlags.Baby:
                    case CASAgeGenderFlags.Toddler:
                        ths.mHygienePlantSimWraps.Visible = true;
                        break;
                    default:
                        ths.mHygienePlantSimWraps.Visible = false;
                        break;
                }
            }
            else
            {
                ths.mBladderPlantSimWraps.Visible = false;
                ths.mHungerPlantSimWraps.Visible = false;
                ths.mHygienePlantSimWraps.Visible = false;
            }

            ths.mBladderMotiveText.Caption = Common.LocalizeEAString(info.OccultInfo.GetMotiveNameKey(MotiveID.Bladder));
            ths.mEnergyMotiveText.Caption = Common.LocalizeEAString(info.OccultInfo.GetMotiveNameKey(MotiveID.Energy));
            ths.mFunMotiveText.Caption = Common.LocalizeEAString(info.OccultInfo.GetMotiveNameKey(MotiveID.Fun));
            ths.mHungerMotiveText.Caption = Common.LocalizeEAString(info.OccultInfo.GetMotiveNameKey(MotiveID.Hunger));
            ths.mSocialMotiveText.Caption = Common.LocalizeEAString(info.OccultInfo.GetMotiveNameKey(MotiveID.Social));
            ths.mHygieneMotiveText.Caption = Common.LocalizeEAString(info.OccultInfo.GetMotiveNameKey(MotiveID.Hygiene));

            if (info.mIsFoal)
            {
                ths.mMotiveFunTopWin.Visible = false;
            }

            /* Performed by Core update, doing it again makes the needs bar flucuate
            for (int i = 0x0; i < ths.mMotiveCount; i++)
            {
                ths.UpdateMotiveBar(i, info);
            }
            */
        }
    }
}
