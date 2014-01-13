using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Tests
{
    public class SocialTest : VectorBooter.Test
    {
        List<string> mSocials = new List<string>();

        bool mAllowAccept;
        bool mAllowReject;

        bool mAllowRecipient;
        bool mAllowInitiator;

        public SocialTest(XmlDbRow row)
        {
            string name = row.GetString("GUID");

            if (BooterLogger.Exists(row, "Socials", name))
            {
                mSocials = row.GetStringList("SocialName", ',');

                for (int i = mSocials.Count - 1; i >= 0; i--)
                {
                    if (ActionData.Get(mSocials[i]) == null)
                    {
                        BooterLogger.AddError(name + " Invalid Social: " + mSocials[i]);

                        mSocials.RemoveAt(i);
                    }
                }
            }

            if (BooterLogger.Exists(row, "AllowAccept", name))
            {
                mAllowAccept = row.GetBool("AllowAccept");
            }

            if (BooterLogger.Exists(row, "AllowReject", name))
            {
                mAllowReject = row.GetBool("AllowReject");
            }

            if (BooterLogger.Exists(row, "AllowRecipient", name))
            {
                mAllowRecipient = row.GetBool("AllowRecipient");
            }

            if (BooterLogger.Exists(row, "AllowInitiator", name))
            {
                mAllowInitiator = row.GetBool("AllowInitiator");
            }
        }

        public override void GetEvents(Dictionary<EventTypeId,bool> events)
        {
            if (events.ContainsKey(EventTypeId.kSocialInteraction)) return;

            events.Add(EventTypeId.kSocialInteraction, true);
        }

        public override bool IsSuccess(Event e)
        {
            SocialEvent socialEvent = e as SocialEvent;
            if (socialEvent == null) return false;

            if (!mAllowAccept)
            {
                if (socialEvent.WasAccepted) return false;
            }
            else if (!mAllowReject)
            {
                if (!socialEvent.WasAccepted) return false;
            }

            if (!mAllowRecipient)
            {
                if (!socialEvent.WasRecipient) return false;
            }
            else if (!mAllowInitiator)
            {
                if (socialEvent.WasRecipient) return false;
            }

            return mSocials.Contains(socialEvent.SocialName);
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " AllowAccept: " + mAllowAccept;
            result += Common.NewLine + " AllowReject: " + mAllowReject;
            result += Common.NewLine + " AllowRecipient: " + mAllowRecipient;
            result += Common.NewLine + " AllowInitiator: " + mAllowInitiator;

            foreach (string id in mSocials)
            {
                result += Common.NewLine + " Social: " + id;
            }

            return result;
        }
    }
}
