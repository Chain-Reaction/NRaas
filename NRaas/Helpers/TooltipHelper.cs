using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class TooltipHelper : Common.IDelayedWorldLoadFinished, Common.IWorldQuit
    {
        public static Tooltip sCurrentTip = null;
        public static ulong sTipObject = 0;
        public static Common.AlarmTask sCurrentAlarm;
        private static Dictionary<Type, List<MethodInfo>> sListeners = new Dictionary<Type, List<MethodInfo>>();        
        public static ulong sLastObjectId = 0;

        public TooltipHelper()
        {
        }

        public void OnDelayedWorldLoadFinished()
        {
            SceneMgrWindow win = UIManager.GetSceneWindow();
            if (win != null)
            {           
                win.GameMouseMove += new ScenePickHandler(SceneWindow_Hover);
            }

            CameraController.OnCameraMapViewEnabledCallback += new CameraController.CameraMapViewEnabledHandler(this.OnMapViewModeEnabled);

            if (Responder.Instance != null)
            {
                Responder.Instance.GameSpeedChanged += new GameSpeedChangedCallback(this.OnGameSpeedChanged);
            }
        }

        public void OnWorldQuit()
        {
            sListeners.Clear();
            HideCurrentTooltip();
        }

        public void OnMapViewModeEnabled(bool enabled)
        {
            if (enabled)
            {
                sLastObjectId = 0;
                HideCurrentTooltip();
            }
        }

        public void OnGameSpeedChanged(Gameflow.GameSpeed newSpeed, bool locked)
        {
            if (newSpeed == Gameflow.GameSpeed.Pause)
            {
                sLastObjectId = 0;
                HideCurrentTooltip();
            }
        }

        public static void AddListener(Type type, MethodInfo method)
        {
            if (!sListeners.ContainsKey(type))
            {
                sListeners.Add(type, new List<MethodInfo>());
            }

            sListeners[type].Add(method);
        }

        public static void RemoveListener(Type type, MethodInfo method)
        {
            if (!sListeners.ContainsKey(type)) return;

            sListeners[type].Remove(method);

            if (sListeners[type].Count == 0)
            {
                sListeners.Remove(type);
            }
        }

        public static bool SceneWindow_Hover(WindowBase w, ref ScenePickArgs args)
        {
            if (Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.Gameplay.Gameflow.GameSpeed.Pause && !Sims3.Gameplay.GameStates.IsBuildBuyLikeState)
            {
                return false;
            }

            if (!UIUtils.IsOkayToStartModalDialog(true))
            {
                return false;
            }

            if (args.mObjectType == ScenePickObjectType.Object)
            {
                if (args.mObjectId == sLastObjectId && sCurrentTip != null)
                {
                    RestartAlarm();
                    return true;
                }
                else
                {
                    sLastObjectId = args.mObjectId;
                    HideCurrentTooltip();
                }

                if (sCurrentTip != null)
                {
                    return false;
                }                     

                IScriptProxy proxy = Simulator.GetProxy(new ObjectGuid(args.mObjectId));

                if (proxy != null && proxy.Target != null)
                {
                    Type type = proxy.Target.GetType();

                    if (type == null)
                    {
                        return false;
                    }

                    Type lastType = null;
                    int loop = 0;
                    while (type != typeof(GameObject))
                    {
                        if (loop > 6)
                        {
                            break;
                        }

                        lastType = type;
                        type = type.BaseType;

                        if (type == typeof(CommonDoor))
                        {
                            break;
                        }

                        loop++;
                    }

                    type = lastType;

                    if (!sListeners.ContainsKey(type))
                    {
                        return false;
                    }

                    Vector2 vector = UIManager.GetCursorPosition();
                    WindowBase baseWin = UIManager.GetWindowFromPoint(vector);

                    string content = string.Empty;
                    foreach (MethodInfo info in sListeners[type])
                    {
                        object[] parameters = new object[] { proxy, args };
                        content += (string)info.Invoke(null, parameters);
                    }

                    if (content.Length > 3)
                    {
                        Vector2 mousePosition = new Vector2(0f, 0f);
                        mousePosition = baseWin.Parent != null ? baseWin.Parent.ScreenToWindow(vector) : baseWin.ScreenToWindow(vector);

                        SetAlarm();
                        sCurrentTip = CreateTooltip(content, mousePosition, baseWin);
                        sTipObject = args.mObjectId;
                    }
                }
            }

            return true;
        }

        public static Tooltip CreateTooltip(string text, Vector2 mousePositon, WindowBase mousedOverWindow)
        {
            Tooltip tooltip = new SimpleTextTooltip(text);
            tooltip.TooltipWindow.Position = tooltip.TooltipWindow.Parent.ScreenToWindow(mousePositon);
            tooltip.TooltipWindow.Visible = true;            

            return tooltip;
        }

        public static void KillAlarm()
        {
            if (sCurrentAlarm != null && UIUtils.IsOkayToStartModalDialog(true)) // stops the game from breaking on travel or loading when paused
            {
                sCurrentAlarm.Dispose();
            }
        }

        public static void RestartAlarm()
        {
            KillAlarm();

            SetAlarm();          
        }

        public static void SetAlarm()
        {
            sCurrentAlarm = new Common.AlarmTask(2f, Sims3.Gameplay.Utilities.TimeUnit.Minutes, HideCurrentTooltip);  
        }

        public static void HideCurrentTooltip()
        {
            if (sLastObjectId == sTipObject && sLastObjectId != 0)
            {
                RestartAlarm();
            }

            if (sCurrentTip != null)
            {
                sCurrentTip.TooltipWindow.Visible = false;
                // because...EA
                sCurrentTip.TooltipWindow.ShadeColor = new Color(sCurrentTip.TooltipWindow.ShadeColor.ARGB & 0xffffff);
                Simulator.AddObject(new TooltipManager.DisposeTooltipTask(sCurrentTip));
                KillAlarm();
                sCurrentTip = null;
                sTipObject = 0L;
            }
        }
    }
}
