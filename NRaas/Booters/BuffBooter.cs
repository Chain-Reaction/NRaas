using NRaas.CommonSpace.Booters;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.CommonSpace.Booters
{
    public class BuffBooter : BooterHelper.ListingBooter
    {
        public BuffBooter()
            : this(VersionStamp.sNamespace + ".Buffs", true)
        { }
        public BuffBooter(string reference, bool testDirect)
            : base("BuffFile", "File", reference, testDirect)
        { }

        public override BooterHelper.BootFile GetBootFile(string reference, string name, bool primary)
        {
            return new BooterHelper.DataBootFile(reference, name, primary);
        }

        protected override void PerformFile(BooterHelper.BootFile file)
        {
            BooterHelper.DataBootFile dataFile = file as BooterHelper.DataBootFile;
            if (dataFile == null) return;

            if ((dataFile.GetTable("BuffList") == null) && (dataFile.GetTable("Buffs") == null)) return;

            try
            {
                BuffManager.ParseBuffData(dataFile.Data, true);
            }
            catch (Exception e)
            {
                Common.Exception(file.ToString(), e);

                BooterLogger.AddError(file + ": ParseBuffData Error");
            }
        }
    }
}
