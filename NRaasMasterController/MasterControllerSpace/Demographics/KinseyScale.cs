using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class KinseyScale : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "KinseyScale";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            long lTotal = 0, lUndecided = 0, lNeitherMale = 0, lNeitherFemale = 0;

            List<long> scaleMale = new List<long>();
            List<long> scaleFemale = new List<long>();

            for (int i = 0; i <= 4; i++)
            {
                scaleMale.Add(0);
                scaleFemale.Add(0);
            }

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                if (SimTypes.IsSpecial(member)) continue;

                if (!member.TeenOrAbove) continue;

                lTotal++;

                if ((member.mGenderPreferenceFemale == 0) && (member.mGenderPreferenceMale == 0))
                {
                    lUndecided++;
                }
                else if (member.mGenderPreferenceFemale > 0)
                {
                    if (member.mGenderPreferenceMale <= 0)
                    {
                        if (member.IsMale)
                        {
                            scaleMale[0]++; // Exclusively Hetro
                        }
                        else
                        {
                            scaleFemale[4]++; // Exclusively Gay
                        }
                    }
                    else if (member.mGenderPreferenceFemale > member.mGenderPreferenceMale)
                    {
                        if (member.IsMale)
                        {
                            scaleMale[1]++; // Predominantly Hetro
                        }
                        else
                        {
                            scaleFemale[3]++; // Predominantly Gay
                        }
                    }
                    else if (member.mGenderPreferenceFemale == member.mGenderPreferenceMale)
                    {
                        if (member.IsMale)
                        {
                            scaleMale[2]++; // Bi
                        }
                        else
                        {
                            scaleFemale[2]++; // Bi
                        }
                    }
                    else //if (member.mGenderPreferenceFemale < member.mGenderPreferenceMale)
                    {
                        if (member.IsMale)
                        {
                            scaleMale[3]++; // Predominantly Gay
                        }
                        else
                        {
                            scaleFemale[1]++; // Predominantly Hetro
                        }
                    }
                }
                else //if (member.mGenderPreferenceFemale < 0)
                {
                    if (member.mGenderPreferenceMale <= 0)
                    {
                        if (member.IsMale)
                        {
                            lNeitherMale++; // Non sexual
                        }
                        else
                        {
                            lNeitherFemale++; // Non sexual
                        }
                    }
                    else
                    {
                        if (member.IsMale)
                        {
                            scaleMale[4]++; // Exclusively Gay
                        }
                        else
                        {
                            scaleFemale[0]++; // Exclusively Hetro
                        }
                    }
                }
            }

            string body = null;

            for (int i = 0; i <= 4; i++)
            {
                body += Common.Localize("KinseyScale:Element", false, new object[] { i, scaleMale[i], scaleFemale[i], (scaleMale[i] + scaleFemale[i]) });
            }

            return Common.Localize("KinseyScale:Body", false, new object[] { lTotal, lUndecided, lNeitherMale, lNeitherFemale, body, Common.Localize("Boolean:" + SimDescription.AllowSameSexGenderPreferences.ToString()) });
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Sims3.UI.SimpleMessageDialog.Show(Common.Localize("KinseyScale:Header"), GetDetails(sims));
            return OptionResult.SuccessRetain;
        }
    }
}
