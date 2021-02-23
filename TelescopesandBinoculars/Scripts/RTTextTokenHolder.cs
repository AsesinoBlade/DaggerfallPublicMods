// Project:         RepairTools mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2020 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    6/27/2020, 4:00 PM
// Last Edit:		8/1/2020, 12:05 AM
// Version:			1.00
// Special Thanks:  Hazelnut and Ralzar
// Modifier:		Hazelnut

using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;

namespace Telescopes
{
    public class RTTextTokenHolder
    {
        public static TextFile.Token[] ItemPoisonTextTokens(uint tokenID, bool toolBroke, DaggerfallUnityItem itemPoisoned)
        {
            if (toolBroke)
            {
                switch (tokenID)
                {
                    case 900:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your " + itemPoisoned.LongName + "'s blade.",
                            "",
                            "The bottle is now empty,",
                            "you discard it.");
                    case 901:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison " + itemPoisoned.LongName + ".",
                            "",
                            "The bottle is now empty,",
                            "you throw it away.");
                    case 902:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison " + itemPoisoned.LongName + ".",
                            "",
                            "the bottle is now empty,",
                            "you toss it to your side.");
                    case 903:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your " + itemPoisoned.LongName + ".",
                            "",
                            "The bottle is now empty,",
                            "you discard them");
                    case 904:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your " + itemPoisoned.LongName + "'s frame.",
                            "",
                            "The bottle is now empty,",
                            "you throw the empty container away.");
                    case 905:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your " + itemPoisoned.LongName + ". It surges with energy.",
                            "",
                            "the bottle is now empty,",
                            "you toss the empty container away");
                    default:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "Text Token Not Found");
                }
            }
            else
            {
                switch (tokenID)
                {
                    case 900:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your " + itemPoisoned.LongName + "'s blade.");
                    case 901:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your " + itemPoisoned.LongName);
                    case 902:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your " + itemPoisoned.LongName + ".");
                    case 903:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your " + itemPoisoned.LongName + ".");
                    case 904:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your  " + itemPoisoned.LongName + "'s frame.");
                    case 905:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "You poison your " + itemPoisoned.LongName + ". It surges with energy.");
                    default:
                        return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                            TextFile.Formatting.JustifyCenter,
                            "Text Token Not Found");
                }
            }
        }
    }
}