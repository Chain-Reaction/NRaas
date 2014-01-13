using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
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
    public class SimDisplayEx
    {
        static float sMagicMotiveSizeControlLeft = -1;

        private static bool IsOccult(SimInfo info)
        {
            Sim actor = Sim.ActiveActor;
            if (actor == null) return false;

            if (actor.SimDescription.ChildOrAbove)
            {
                if (actor.SimDescription.IsFairy) return true;

                if (actor.SimDescription.IsGenie) return true;
            }
            
            if (actor.SimDescription.TeenOrAbove)
            {
                if (actor.SimDescription.IsWitch) return true;

                if (actor.SimDescription.IsUnicorn) return true;
            }

            return false;
        }

        private static float GetMagicLevel(Sim actor, OccultTypes exclude, out OccultTypes selection)
        {
            if (exclude != OccultTypes.Fairy)
            {
                Motive motive = actor.Motives.GetMotive(CommodityKind.AuraPower);
                if (motive != null)
                {
                    selection = OccultTypes.Fairy;
                    return motive.UIValue;
                }
            }

            if (exclude != OccultTypes.Witch)
            {
                Motive motive = actor.Motives.GetMotive(CommodityKind.MagicFatigue);
                if (motive != null)
                {
                    selection = OccultTypes.Witch;
                    return -motive.UIValue;
                }
            }

            if (exclude != OccultTypes.Genie)
            {
                OccultGenie genie = actor.OccultManager.GetOccultType(OccultTypes.Genie) as OccultGenie;
                if (genie != null)
                {
                    selection = OccultTypes.Genie;
                    return (genie.MagicPoints.mCurrentMagicPointValue * 100) / genie.MagicPoints.mMaxMagicPoints;
                }
            }

            if (exclude != OccultTypes.Unicorn)
            {
                OccultUnicorn unicorn = actor.OccultManager.GetOccultType(OccultTypes.Unicorn) as OccultUnicorn;
                if (unicorn != null)
                {
                    selection = OccultTypes.Unicorn;
                    return (unicorn.MagicPoints.mCurrentMagicPointValue * 100) / unicorn.MagicPoints.mMaxMagicPoints;
                }
            }

            selection = OccultTypes.None;
            return 0;
        }

        private static void OnMotivesChanged(SimDisplay ths, SimInfo info)
        {
            if ((info != null) && ths.mMagicMotiveBarWindow.Visible)
            {
                Sim actor = Sim.ActiveActor;
                if (actor == null) return;

                float leftExtent = -1;
                float rightExtent = 0;

                bool dual = false;

                if (actor.Motives != null)
                {
                    OccultTypes selection;
                    rightExtent = GetMagicLevel(actor, OccultTypes.None, out selection);

                    if (selection != OccultTypes.None)
                    {
                        dual = true;

                        OccultTypes selection2;
                        float value = GetMagicLevel(actor, selection, out selection2);

                        if (selection2 != OccultTypes.None)
                        {
                            leftExtent = rightExtent;
                            rightExtent = value;
                        }
                        else
                        {
                            dual = false;
                        }
                    }
                }

                Rect area = ths.mMagicMotiveSizeControlWin.Area;

                if (sMagicMotiveSizeControlLeft == -1)
                {
                    sMagicMotiveSizeControlLeft = area.TopLeft.x;
                }

                float leftFactor = 0;
                float rightFactor = (rightExtent + 100f) / 200f;

                if (dual)
                {
                    leftFactor = (100 - leftExtent) / 400f;
                    rightFactor /= 2f;
                    rightFactor += 0.5f;
                }

                if (Common.kDebugging)
                {
                    Common.DebugNotify("Left: " + leftExtent + "\nRight: " + rightExtent + "\nDual: " + dual + "\nLeftFactor: " + leftFactor + "\nRightFactor: " + rightFactor);
                }

                area.Set(sMagicMotiveSizeControlLeft + (leftFactor * ths.mMagicMotiveSizeReferenceWin.Area.Width), area.TopLeft.y, sMagicMotiveSizeControlLeft + (rightFactor * ths.mMagicMotiveSizeReferenceWin.Area.Width), area.BottomRight.y);

                ths.mMagicMotiveSizeControlWin.Area = area;
                if (ths.mFlashMagicMotiveBar)
                {
                    ths.mMagicMotiveChangeWindow.Visible = true;
                    Simulator.AddObject(new Sims3.UI.OneShotFunctionTask(ths.OnMagicMotiveChangeFinish, StopWatch.TickStyles.Seconds, 3f));
                    ths.mFlashMagicMotiveBar = false;
                }
            }
        }

        public static void UpdateMagicMotiveBar(SimDisplay ths, SimInfo info)
        {
            if (((info != null) && (info.OccultInfo != null)) && (IsOccult(info)))
            {
                ths.mMagicMotiveBarWindow.Visible = true;
                OnMotivesChanged(ths, info);
            }
            else
            {
                ths.mMagicMotiveBarWindow.Visible = false;
            }
        }
    }
}
