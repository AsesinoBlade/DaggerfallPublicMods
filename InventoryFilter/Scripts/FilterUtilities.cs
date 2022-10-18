using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using UnityEngine;

public class FilterUtilities : MonoBehaviour
{
    public static string Amulet { get; set; } = "neck amulet torc";
    public static string Bracelet { get; set; } = "wrist bracelet";
    public static string Bracer { get; set; } = "arm wrist bracer";
    public static string Ring { get; set; } = "finger hand ring";
    public static string Mark { get; set; } = "wrist mark";
    public static string Crystal { get; set; } = "neck crystal";
    public static string Head { get; set; } = "head helm helmet hat cap";
    public static string RightArm { get; set; } = "arm right pauldron";
    public static string LeftArm { get; set; } = "arm left pauldron";
    public static string Cloak { get; set; } = "back cloak cape shawl";
    public static string ChestArmor { get; set; } = "chest cuirass";
    public static string ChestClothes { get; set; } = "chest shirt strap armband eodoric tunic surcoat coat robe";
    public static string RightHand { get; set; } = "hand glove right weapon two-handed guantlet";
    public static string LeftHand { get; set; } = "left hand weapon shield";
    public static string LegsArmor { get; set; } = "leg legs greave greaves";
    public static string LegsClothes { get; set; } = "leg khajiit suit loincloth skirt pant";
    public static string Feet { get; set; } = "foot feet boot shoe sandal solleret";

    protected static string[] itemGroupNames = new string[]
    {
        "drugs",
        "uselessitems1",
        "armor",
        "weapons",
        "magicitems",
        "artifacts",
        "mensclothing",
        "books",
        "furniture",
        "uselessitems2",
        "religiousitems",
        "maps",
        "womensclothing",
        "paintings",
        "gems",
        "plantingredients1",
        "plantingredients2",
        "creatureingredients1",
        "creatureingredients2",
        "creatureingredients3",
        "miscellaneousingredients1",
        "metalingredients",
        "miscellaneousingredients2",
        "transportation",
        "deeds",
        "jewellery",
        "questitems",
        "miscitems",
        "currency"
    };


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected static string GetSearchTags(DaggerfallUnityItem item)
    {
        var equipSlot = GameManager.Instance.PlayerEntity.ItemEquipTable.GetEquipSlot(item);
        switch (equipSlot)
        {
            case EquipSlots.None:
                return string.Empty;
            case EquipSlots.Amulet0:
            case EquipSlots.Amulet1:
                return FilterUtilities.Amulet;
            case EquipSlots.Bracelet0:
            case EquipSlots.Bracelet1:
                return FilterUtilities.Bracelet;
            case EquipSlots.Bracer0:
            case EquipSlots.Bracer1:
                return FilterUtilities.Bracer;
            case EquipSlots.Ring0:
            case EquipSlots.Ring1:
                return FilterUtilities.Ring;
            case EquipSlots.Mark0:
            case EquipSlots.Mark1:
                return FilterUtilities.Mark;
            case EquipSlots.Crystal0:
            case EquipSlots.Crystal1:
                return FilterUtilities.Crystal;
            case EquipSlots.Head:
                return FilterUtilities.Head;
            case EquipSlots.RightArm:
                return FilterUtilities.RightArm;
            case EquipSlots.LeftArm:
                return FilterUtilities.LeftArm;
            case EquipSlots.Cloak1:
            case EquipSlots.Cloak2:
                return FilterUtilities.Cloak;
            case EquipSlots.ChestClothes:
                return FilterUtilities.ChestClothes;
            case EquipSlots.ChestArmor:
                return FilterUtilities.ChestArmor;
            case EquipSlots.RightHand:
            case EquipSlots.Gloves:
                return FilterUtilities.RightHand;
            case EquipSlots.LeftHand:
                return FilterUtilities.LeftHand;
            case EquipSlots.LegsArmor:
                return FilterUtilities.LegsArmor;
            case EquipSlots.LegsClothes:
                return FilterUtilities.LegsClothes;
            case EquipSlots.Feet:
                return FilterUtilities.Feet;
            default:
                return string.Empty;
        }

    }

    public static bool ItemPassesFilter(DaggerfallUnityItem item)
    {
        bool iterationPass = false;
        bool isRecipe = false;
        string recipeName = string.Empty;

        string str = string.Empty;

        if (item != null)
        {
            str = GetSearchTags(item);


            if (str != null && str != string.Empty)
                Debug.Log($"found {str}");

            Type itemClassType;
            if (item.TemplateIndex > ItemHelper.LastDFTemplate)
                if (GameManager.Instance.ItemHelper.GetCustomItemClass(item.TemplateIndex, out itemClassType))
                    if (itemClassType != null)
                    {
                        str += " " + itemClassType.ToString();
                    }

            if (String.IsNullOrEmpty(AsesinoInventoryWindow.filterString))
                return true;

            if (item.LongName.ToLower().Contains("recipe"))
            {
                TextFile.Token[] tokens = ItemHelper.GetItemInfo(item, DaggerfallUnity.Instance.TextProvider);
                MacroHelper.ExpandMacros(ref tokens, item);
                recipeName = tokens[0].text;
                isRecipe = true;
            }

            foreach (string word in AsesinoInventoryWindow.filterString.Split(' '))
            {
                if (word.Trim().Length > 0)
                {
                    if (word[0] == '-')
                    {
                        string wordLessFirstChar = word.Remove(0, 1);
                        iterationPass = true;
                        if (item.LongName.IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = false;
                        else if (isRecipe &&
                                 recipeName.IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = false;
                        else if (itemGroupNames[(int)item.ItemGroup]
                                     .IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = false;
                        else if (str != null &&
                                 str.IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = false;
                    }
                    else
                    {
                        iterationPass = false;
                        if (item.LongName.IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = true;
                        else if (isRecipe && recipeName.IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = true;
                        else if (itemGroupNames[(int)item.ItemGroup]
                                     .IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = true;
                        else if (str != null && str.IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = true;
                    }

                    if (!iterationPass)
                        return false;
                }
            }
        }

        return true;
    }
}
