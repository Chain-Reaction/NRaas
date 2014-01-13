using Sims3.Gameplay.Careers;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using System;

namespace NRaas.CommonSpace.Helpers
{
    public class CareerManagerEx
    {
        // Cutdown version of function
        public static bool ImportContent(CareerManager ths, ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            Occupation job = ImportJobContent(ths, 0x139b80d1, resKeyTable, objIdTable, reader);
            if (job != null)
            {
                ths.mJob = job;
            }

            uint num;
            reader.ReadUint32(0xb8e5462, out num, 0);
            for (uint i = 0; i < num; i++)
            {
                Occupation occupation = ImportJobContent(ths, 0x1f4ce7a9 + i, resKeyTable, objIdTable, reader);
                if (occupation != null)
                {
                    ths.QuitCareers[occupation.Guid] = occupation;
                }
            }

            School school = ImportJobContent(ths, 0xda42e1c9, resKeyTable, objIdTable, reader) as School;
            if (school != null)
            {
                ths.mSchool = school;
            }

            job = ImportJobContent(ths, 0x6b334d3d, resKeyTable, objIdTable, reader);
            if (job != null)
            {
                ths.mRetiredCareer = job;
            }

            return true;
        }

        public static Occupation ImportJobContent(CareerManager ths, uint key, ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            Occupation occupation = null;
            IPropertyStreamReader child = reader.GetChild(key);
            if (child != null)
            {
                ulong num;
                child.ReadUint64(0xae293ba5, out num, 0L);
                
                // Custom, EA Standard occupations are handled by EA
                if (Enum.IsDefined(typeof(OccupationNames), num)) return null;

                {
                    OccupationNames guid = (OccupationNames)num;
                    if (guid != OccupationNames.Undefined)
                    {
                        Occupation staticOccupation = CareerManager.GetStaticOccupation(guid);
                        if (staticOccupation != null)
                        {
                            occupation = staticOccupation.Clone();
                            occupation.OwnerDescription = ths.mSimDescription;
                            occupation.ImportContent(resKeyTable, objIdTable, child);
                        }
                    }
                }
            }

            return occupation;
        }
    }
}
