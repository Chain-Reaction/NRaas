using System;
using System.Collections.Generic;
using System.Text;

namespace Alcohol
{
    public enum ListOfSelfInteractions
    {
        KnockedOut = 0,
        Bladderate = 1,
        GetNaked = 2,
        TalkToSelf = 3,
        Sing = 4,
        BadBuzz = 5,
        DrunkCry = 6,
        DrunkHysteria = 7
    }

    public enum ListOfInteractionsToOthers
    {
        Greet = 0,
        Chat = 1,
        Gossip = 2,

        //Flirty
        Flirt = 3,
        AskSign = 4,
        Kiss = 5,

        //Fight
        Slap = 6,
        Fight = 7,
        Browl = 8
    }

    public enum DrunkState
    {
        None = 0, 
        Buzzed = 1, 
        Tipsy = 2,
        Drunk = 3, 
        Hangover = 4
    }
}
