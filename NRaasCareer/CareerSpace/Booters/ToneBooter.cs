using NRaas.CommonSpace.Booters;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;

namespace NRaas.CareerSpace.Booters
{
    public class ToneBooter : BooterHelper.ByRowListingBooter
    {
        public ToneBooter()
            : this(VersionStamp.sNamespace + ".Tones")
        { }
        public ToneBooter(string reference)
            : base("Tone", "ToneFile", "Tones", reference)
        { }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            string toneName = row.GetString("ToneName");
            if (string.IsNullOrEmpty(toneName)) 
            {
                BooterLogger.AddError("Tone found with no name");
                return;
            }

            Type classType = row.GetClassType("FullClassName");
            if (classType == null) 
            {
                BooterLogger.AddError("Tone: " + toneName + " FullClassName no match");
                return;
            }

            string guid = row.GetString("CareerGuid");

            OccupationNames careerGuid = OccupationNames.Undefined;
            ParserFunctions.TryParseEnum<OccupationNames>(guid, out careerGuid, OccupationNames.Undefined);

            if (careerGuid == OccupationNames.Undefined)
            {
                careerGuid = unchecked((OccupationNames)ResourceUtils.HashString64(guid));
            }

            Career staticCareer = CareerManager.GetStaticCareer (careerGuid);
            if (staticCareer == null)
            {
                BooterLogger.AddError("Tone: " + toneName + " CareerGuid no match");
                return;
            }

            staticCareer.SharedData.ToneDefinitions.Add(new CareerBooterToneDefinition(row, classType));
        }

        protected class CareerBooterToneDefinition : CareerToneDefinition
        {
            protected XmlDbRow mRow;

            protected Type mTone;

            public CareerBooterToneDefinition(XmlDbRow row, Type tone)
                : base (row.GetString ("ToneName"))
            {
                mRow = row;
                mTone = tone;
            }

            public override CareerTone CreateToneOverride()
            {
                try
                {
                    if (mTone == null) return null;

                    CareerTone tone = mTone.GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as CareerTone;

                    if (mRow != null)
                    {
                        CareerToneEx toneEx = tone as CareerToneEx;
                        if (toneEx != null)
                        {
                            toneEx.Set(mRow);
                        }
                    }
                    return tone;
                }
                catch (Exception exception)
                {
                    Common.Exception(mName, exception);
                }

                return null;
            }
        }
    }
}
