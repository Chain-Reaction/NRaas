using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefComboRabbitHole : Dereference<ComboRabbitHole>
    {
        protected override DereferenceResult Perform(ComboRabbitHole reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "ContainedRabbitholes", field, objects))
            {
                if (Performing)
                {
                    RemoveKeys(reference.ContainedRabbitholes, objects);

                    try
                    {
                        RabbitHole hole = Find<RabbitHole>(objects);
                        if (hole != null)
                        {
                            switch (hole.Guid)
                            {
                                case RabbitHoleType.DaySpa:
                                    reference.AddComboPiece("DaySpa", "Gameplay/Objects/RabbitHoles/DaySpa:DaySpa");
                                    break;
                                case RabbitHoleType.Bookstore:
                                    reference.AddComboPiece("Bookstore", "Gameplay/Objects/RabbitHoles/Bookstore:Bookstore");
                                    break;
                                case RabbitHoleType.BusinessAndJournalism:
                                    reference.AddComboPiece("Business", "Gameplay/Objects/RabbitHoles/Business:Business");
                                    break;
                                case RabbitHoleType.Restaurant:
                                    reference.AddComboPiece("RestaurantBistro", "Gameplay/Objects/RabbitHoles/Restaurant:Restaurant");
                                    break;
                                case RabbitHoleType.CityHall:
                                    reference.AddComboPiece("CityHall", "Gameplay/Objects/RabbitHoles/CityHall:CityHall");
                                    break;
                                case RabbitHoleType.PoliceStation:
                                    reference.AddComboPiece("PoliceStation", "Gameplay/Objects/RabbitHoles/PoliceStation:PoliceStation");
                                    break;
                                case RabbitHoleType.MilitaryBase:
                                    reference.AddComboPiece("Military", "Gameplay/Objects/RabbitHoles/MilitaryBase:MilitaryBase");
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(reference, e);
                    }

                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
