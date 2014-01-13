using NRaas.CommonSpace.Tasks;
using NRaas.TestSpace.States;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class CRAPJailBreak : Common, Common.IPreLoad
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static CRAPJailBreakTask sTask = null;

        static CRAPJailBreak()
        {
            sEnableLoadLog = true;

            Bootstrap();
        }
        public CRAPJailBreak()
        { }

        public static void InsanityWriteLog(string text)
        {
            DebugWriteLog(text);
        }

        public static void InsanityException(string msg, Exception e)
        {
            Exception(msg, e);
        }

        public void OnPreLoad()
        {
            sTask = new CRAPJailBreakTask();
            sTask.AddToSimulator();

            GameStates.sSingleton.mInWorldState = new InWorldStateEx();

            List<StateMachineState> states = GameStates.sSingleton.mStateMachine.mStateList;
            for (int i = states.Count - 1; i >= 0; i--)
            {
                Type type = states[i].GetType();

                if (type == typeof(InWorldState))
                {
                    states[i] = GameStates.sSingleton.mInWorldState;
                    states[i].SetStateMachine(GameStates.sSingleton.mStateMachine);
                }
            }
        }

        public class CRAPJailBreakTask : RepeatingTask
        {
            private static void OnAcceptHousehold(WindowBase sender, UIButtonClickEventArgs eventArgs)
            {
                try
                {
                    CASPuck ths = CASPuck.Instance;

                    Sims3.UI.Function f = null;
                    if (!ths.mUiBusy && !ths.mAttemptingToAddSim)
                    {
                        ths.mUiBusy = true;
                        if (f == null)
                        {
                            f = delegate
                            {
                                CASController.Singleton.SetCurrentState(CASState.Summary);
                                if (ths.ShowRequiredItemsDialogTask())
                                {
                                    ths.AcceptHouseholdCallback();
                                }
                                else
                                {
                                    ths.mUiBusy = false;
                                }
                            };
                        }
                        Simulator.AddObject(new Sims3.UI.OneShotFunctionTask(f));
                    }
                    eventArgs.Handled = true;
                }
                catch (Exception e)
                {
                    Common.Exception("OnAcceptHousehold", e);
                }
            }

            protected override bool OnPerform()
            {
                CASPuck puck = CASPuck.Instance;
                if (puck == null) return true;

                if (puck.mAcceptButton != null)
                {
                    puck.mAcceptButton.Click -= puck.OnAcceptHousehold;
                    puck.mAcceptButton.Click -= OnAcceptHousehold;
                    puck.mAcceptButton.Click += OnAcceptHousehold;
                }

                return true;
            }
        }
    }
}
