using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.CommonSpace.Booters
{
    public class ScoringBooter : BooterHelper.ByRowListingBooter
    {
        int mIndex = 0;

        public ScoringBooter()
            : this ("MethodFile", VersionStamp.sNamespace + ".Scoring", true)
        { }
        public ScoringBooter(string listing, string reference, bool testDirect)
            : base("Methods", listing, "File", reference, testDirect)
        { }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            BooterHelper.DataBootFile dataFile = file as BooterHelper.DataBootFile;
            if (dataFile == null) return;

            bool success = false;

            mIndex++;

            try
            {
                string methodName = row.GetString("Name");
                if (string.IsNullOrEmpty(methodName))
                {
                    BooterLogger.AddError(file + " : Method " + mIndex + " Unnamed");
                }
                else if (ScoringLookup.GetScoring(methodName, false) != null)
                {
                    BooterLogger.AddError(methodName + " Name already in use");
                    return;
                }
                else
                {
                    Type classType = row.GetClassType("FullClassName");
                    if (classType == null)
                    {
                        BooterLogger.AddError(methodName + " Unknown FullClassName: " + row.GetString("FullClassName"));
                    }
                    else
                    {
                        IListedScoringMethod scoring = null;
                        try
                        {
                            scoring = classType.GetConstructor(new Type[0]).Invoke(new object[0]) as IListedScoringMethod;
                        }
                        catch
                        { }

                        if (scoring == null)
                        {
                            BooterLogger.AddError(methodName + ": Constructor Fail " + row.GetString("FullClassName"));
                        }
                        else
                        {
                            XmlDbTable scoringTable = dataFile.GetTable(methodName);
                            if (scoringTable == null)
                            {
                                BooterLogger.AddError(methodName + ": Table Missing");
                            }
                            else
                            {
                                if (scoring.Parse(row, scoringTable))
                                {
                                    BooterLogger.AddTrace("Added Scoring : " + methodName);

                                    ScoringLookup.AddScoring(methodName, scoring);

                                    success = true;
                                }
                                else
                                {
                                    BooterLogger.AddError(methodName + ": Parsing Fail");
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (!success)
                {
                    foreach (string column in row.ColumnNames)
                    {
                        BooterLogger.AddError(column + "= " + row[column]);
                    }
                }
            }
        }
    }
}