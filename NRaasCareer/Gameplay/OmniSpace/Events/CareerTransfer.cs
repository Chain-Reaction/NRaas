using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System.Collections.Generic;

namespace NRaas.Gameplay.OmniSpace.Events
{
    public class CareerTransfer : Career.EventTransferToDifferentCareer
    {
        int mAbsoluteLevel = 0;
        int mRelativeLevelMin = 0;
        int mRelativeLevelMax = 0;

        OccupationNames mTransferCareer;

        string mStringKey;

        string mTransferBranch = "Base";

        // Methods
        public CareerTransfer()
        { }
        public CareerTransfer(XmlDbRow row, Dictionary<string, Dictionary<int, CareerLevel>> careerLevels, string careerName)
            : base(row, careerLevels, null)
        {
            mStringKey = "NRaas.CareerTransfer:" + row.GetString("EventType");

            if (row.Exists("TransferCareer"))
            {
                string guid = row.GetString("TransferCareer");

                ParserFunctions.TryParseEnum<OccupationNames>(guid, out mTransferCareer, OccupationNames.Undefined);

                if (mTransferCareer == OccupationNames.Undefined)
                {
                    mTransferCareer = unchecked((OccupationNames)ResourceUtils.HashString64(guid));
                }
            }

            if (row.Exists("TransferBranch"))
            {
                mTransferBranch = row.GetString("TransferBranch");
            }

            if (row.Exists("TransferLevelAbsolute"))
            {
                mAbsoluteLevel = row.GetInt("TransferLevelAbsolute");
            }

            if (row.Exists("TransferLevelRelative"))
            {
                string relativeLevel = row.GetString("TransferLevelRelative");

                List<string> relativeLevels = new List<string>(relativeLevel.Split(new char[] { ':' }));
                
                mRelativeLevelMin = int.Parse(relativeLevels[0]);
                if (relativeLevels.Count > 1)
                {
                    mRelativeLevelMax = int.Parse(relativeLevels[1]);
                }
                else
                {
                    if (mRelativeLevelMin > 0)
                    {
                        mRelativeLevelMax = mRelativeLevelMin;
                        mRelativeLevelMin = 0;
                    }
                }
            }
        }

        public static string LocalizeString(bool isFemale, string name, params object[] parameters)
        {
            return Common.LocalizeEAString(isFemale, name, parameters);
        }

        public override int GetNewCareerLevel(Career oldCareer)
        {
            if (mAbsoluteLevel > 0)
            {
                return mAbsoluteLevel;
            }
            else
            {
                int level = oldCareer.CurLevel.Level;
                return RandomUtil.GetInt(level - mRelativeLevelMin, level + mRelativeLevelMax);
            }
        }

        public override bool IsEligible(Career c)
        {
            if (NewCareer == null) return false;

            return base.IsEligible(c);
        }

        public override void RunEvent(Career c)
        {
            Career newCareer = NewCareer as Career;
            if (newCareer == null) return;

            int newCareerLevel = this.GetNewCareerLevel(c);
            CareerLevel level = null;
            foreach (Dictionary<int, CareerLevel> dictionary in newCareer.CareerLevels.Values)
            {
                if (dictionary.TryGetValue(newCareerLevel, out level))
                {
                    ObjectGuid simObjectId = new ObjectGuid();
                    if (c.OwnerDescription.CreatedSim != null)
                    {
                        simObjectId = c.OwnerDescription.CreatedSim.ObjectId;
                    }
                    if (Display(LocalizeString(c.OwnerDescription.IsFemale, mStringKey, new object[] { c.OwnerDescription, level.GetLocalizedName(c.OwnerDescription), level.Level, c.CurLevel.GetLocalizedName(c.OwnerDescription), c.CareerLevel }), simObjectId, c.OwnerDescription.IsFemale, c))
                    {
                        DoTransfer(c, newCareerLevel);
                    }
                    break;
                }
            }
        }

        // Properties
        public override string BranchName
        {
            get { return mTransferBranch; }
        }

        public override Occupation NewCareer
        {
            get { return CareerManager.GetStaticCareer(mTransferCareer); }
        }
    }
}
