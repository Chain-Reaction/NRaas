using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Proxies
{
    public class HouseholdContentsProxy : IExportableContent, IMetaTagExporter
    {
        HouseholdContents mContents;

        public HouseholdContentsProxy()
        {
            mContents = new HouseholdContents();
        }
        public HouseholdContentsProxy(Household household)
        {
            mContents = new HouseholdContents(household);
        }

        public HouseholdContents Contents
        {
            get { return mContents; }
        }

        public Household Household
        {
            get { return mContents.Household; }
        }

        public bool ExportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamWriter writer)
        {
            try
            {
                return mContents.ExportContent(resKeyTable, objIdTable, writer);
            }
            catch (Exception e)
            {
                Common.Exception("ExportContent", e);
                return false;
            }
        }

        public bool ExportMetaTags(IPropertyStreamWriter writer)
        {
            try
            {
                return mContents.ExportMetaTags(writer);
            }
            catch (Exception e)
            {
                Common.Exception("ExportMetaTags", e);
                return false;
            }
        }

        public bool ImportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            try
            {
                int num;
                IPropertyStreamReader child = reader.GetChild(0x846d3a8c);
                if (child != null)
                {
                    mContents.mHousehold = Household.Create();

                    // Custom Function
                    if (!HouseholdEx.ImportContent(mContents.Household, resKeyTable, objIdTable, child))
                    {
                        return false;
                    }
                }

                reader.ReadInt32(0x68d30928, out num, 0x2);
                mContents.mInventories = new ulong[num];
                int[] values = new int[num];
                reader.ReadInt32(0xa3b065d7, out values);
                for (int i = 0x0; i < values.Length; i++)
                {
                    ResourceKey key = resKeyTable[values[i]];
                    mContents.mInventories[i] = key.InstanceId;
                }
                return true;
            }
            catch (Exception e)
            {
                Common.Exception("ImportContent", e);
                return false;
            }
        }

        public static HouseholdContentsProxy Import(string packageName)
        {
            ulong lotId = DownloadContent.ImportHouseholdContentsFromExportBin(packageName);
            if (lotId != 0x0L)
            {
                HouseholdContentsProxy householdContents = new HouseholdContentsProxy();
                if (DownloadContent.ImportHouseholdContents(householdContents, lotId))
                {
                    householdContents.mContents.ContentId = lotId;
                    return householdContents;
                }
            }

            return null;
        }
    }
}
