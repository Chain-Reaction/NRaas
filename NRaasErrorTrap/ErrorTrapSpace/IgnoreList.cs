using NRaas.CommonSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ErrorTrapSpace
{
    public class IgnoreList : Common.IPreLoad
    {
        public class Ignore
        {
            public readonly bool mFullReset;
            public readonly bool mDebugging;

            public Ignore(bool fullReset, bool debugging)
            {
                mFullReset = fullReset;
                mDebugging = debugging;
            }
        }

        static Dictionary<string, Dictionary<string, Ignore>> sIgnored = new Dictionary<string, Dictionary<string, Ignore>>();

        private static void AddIgnored(Type type, string text, bool fullReset, bool debugging)
        {
            AddIgnored(type.ToString(), text, fullReset, debugging);
        }
        private static void AddIgnored(string type, string text, bool fullReset, bool debugging)
        {
            type = type.ToLower();
            text = text.ToLower();

            Dictionary<string, Ignore> ignored;
            if (!sIgnored.TryGetValue(type, out ignored))
            {
                ignored = new Dictionary<string, Ignore>();
                sIgnored.Add(type, ignored);
            }

            if (ignored.ContainsKey(text)) return;

            BooterLogger.AddTrace("IgnoreList: Added Type: " + type + ", Stack: " + text);

            ignored.Add(text, new Ignore(fullReset, debugging));
        }

        public void OnPreLoad()
        {
            try
            {
                // Register bouncincg the EA Role Manager prior to switching it out
                AddIgnored(typeof(NullReferenceException), "RoleManager:VerifyRoleGivingObjectRoles ()", true, false);

                // Error regarding the bird cage at the Pet Store
                AddIgnored(typeof(NullReferenceException), "Sims3.Gameplay.Objects.MinorPets.Bird:MoveToJazzState", true, false);

                // Animation issue regarding standing idles
                AddIgnored(typeof(SacsErrorException), "Sims3.Gameplay.Actors.Sim:LoopIdle", false, false);

                AddIgnored(typeof(InvalidOperationException), "Sims3.Gameplay.Autonomy.Autonomy:ScoreInteractions", false, false);

                // Routing Error
                AddIgnored(typeof(SacsErrorException), "StateMachineClient:CheckForException", false, true);
                
                /*
                AddIgnored(typeof(NullReferenceException), "CarNpcManager:DriveAwayInNpcCar (", true);
                AddIgnored(typeof(NullReferenceException), "CarNpcManager:DriveFromPointToMatrix (", true);
                AddIgnored(typeof(NotSupportedException), "Sims3.Gameplay.Autonomy.Sims3.Gameplay.Autonomy.AutonomyManager:Simulate ()", true);
                */

                XmlDbData data = null;

                try
                {
                    data = XmlDbData.ReadData("NRaas.ErrorTrap.IgnoreList");
                }
                catch (Exception e)
                {
                    Common.Exception("NRaas.ErrorTrap.IgnoreList", e);

                    BooterLogger.AddError("IgnoreList: XML Format Error");
                    return;
                }

                if (data == null) return;

                XmlDbTable table = null;
                data.Tables.TryGetValue("IgnoreList", out table);
                if ((table == null) || (table.Rows == null))
                {
                    BooterLogger.AddError("IgnoreList: Missing IgnoreList Table");
                    return;
                }

                foreach (XmlDbRow row in table.Rows)
                {
                    string exceptionType = row.GetString("ExceptionType");
                    string exceptionStack = row.GetString("ExceptionStack");

                    bool fullReset = false;
                    if (row.Exists("FullReset"))
                    {
                        fullReset = row.GetBool("FullReset");
                    }

                    if (string.IsNullOrEmpty(exceptionType))
                    {
                        BooterLogger.AddError("IgnoreList: ExceptionType field missing or empty");

                        foreach (string column in row.ColumnNames)
                        {
                            BooterLogger.AddError(column + "= " + row[column]);
                        }

                        continue;
                    }

                    AddIgnored(exceptionType, exceptionStack, fullReset, false);
                }
            }
            catch (Exception e)
            {
                Common.Exception("IgnoreList:OnPreLoad", e);
            }
        }

        public static bool IsIgnored(Exception e, out bool fullReset)
        {
            string type = e.GetType().ToString().ToLower();

            /*
            if ((e is StackOverflowException) && (!e.StackTrace.Contains("NRaas.")))
            {
                fullReset = true;
                return true;
            }
            */

            Dictionary<string, Ignore> ignore;
            if (sIgnored.TryGetValue(type, out ignore))
            {
                string stack = e.StackTrace.ToLower();

                foreach (KeyValuePair<string, Ignore> text in ignore)
                {
                    if (stack.Contains(text.Key))
                    {
                        fullReset = text.Value.mFullReset;

                        if ((Common.kDebugging) && (text.Value.mDebugging))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            fullReset = true;
            return false;
        }
    }
}
