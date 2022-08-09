// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: InconsolableCellist, Hazelnut, Numidium
// Extended:     Asesino, Pango
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Banking;
using System.Linq;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;


namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class AsesinoInventoryWindow : DaggerfallInventoryWindow
    {
        protected Button localCloseFilterButton;
        protected bool filterButtonNeedUpdate;
        protected static Button localFilterButton;
        protected static TextBox localFilterTextBox;
        protected static string filterString = null;
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


        public static string Amulet { get;  set; }
        public static string Bracelet { get;  set; }
        public static string Bracer { get;  set; }
        public static string Ring { get;  set; }
        public static string Mark { get;  set; }
        public static string Crystal { get;  set; }
        public static string Head { get;  set; }
        public static string RightArm { get;  set; }
        public static string LeftArm { get;  set; }
        public static string Cloak { get;  set; }
        public static string ChestArmor { get;  set; }
        public static string ChestClothes { get;  set; }
        public static string RightHand { get;  set; }
        public static string LeftHand { get;  set; }
        public static string LegsArmor { get;  set; }
        public static string LegsClothes { get;  set; }
        public static string Feet { get;  set; }
        public static bool SplitTabsOnLootDrops { get; set; }
        public static bool SplitTabsOnWagon { get; set; }

        public AsesinoInventoryWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {

        }

        protected override void Setup()
        {
            base.Setup();
            SetupTargetIconPanelFilterBox();
        }

        public override void Update()
        {
            base.Update();

            if (localFilterTextBox.HasFocus() && (Input.GetKeyDown(KeyCode.Return)))
                SetFocus(null);

            if (filterButtonNeedUpdate)
            {
                filterButtonNeedUpdate = false;
                UpdateFilterButton();
            }
        }

        protected void SetupTargetIconPanelFilterBox()
        {
            string toolTipText = string.Empty;
            toolTipText = "Press Filter Button to Open Filter Text Box.\rAnything typed into text box will autofilter.\rFor negative filter, type '-' in front.\rFor example, -steel weapon will find all weapons not made of steel.";


            localFilterTextBox = DaggerfallUI.AddTextBoxWithFocus(new Rect(new Vector2(1, 24), new Vector2(47, 8)), "filter pattern", localTargetIconPanel);
            localFilterTextBox.VerticalAlignment = VerticalAlignment.Bottom;
            localFilterTextBox.OnType += LocalFilterTextBox_OnType;

            localFilterTextBox.OverridesHotkeySequences = true;

            localFilterButton = DaggerfallUI.AddButton(new Rect(40, 25, 15, 8), localTargetIconPanel);
            localFilterButton.Label.Text = "Filter";
            localFilterButton.ToolTip = defaultToolTip;
            localFilterButton.ToolTipText = toolTipText;
            
            localFilterButton.Label.TextScale = 0.75f;
            localFilterButton.Label.ShadowColor = Color.black;
            localFilterButton.VerticalAlignment = VerticalAlignment.Bottom;
            localFilterButton.HorizontalAlignment = HorizontalAlignment.Left;
            localFilterButton.BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);
            localFilterButton.OnMouseClick += LocalFilterButton_OnMouseClick;
            
            localCloseFilterButton = DaggerfallUI.AddButton(new Rect(47, 25, 8, 8), localTargetIconPanel);
            localCloseFilterButton.Label.Text = "x";
            localCloseFilterButton.Label.TextScale = 0.75f;
            localCloseFilterButton.Label.ShadowColor = Color.black;
            localCloseFilterButton.VerticalAlignment = VerticalAlignment.Bottom;
            localCloseFilterButton.HorizontalAlignment = HorizontalAlignment.Right;
            localCloseFilterButton.BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);
            localCloseFilterButton.OnMouseClick += LocalCloseFilterButton_OnMouseClick;

            filterButtonNeedUpdate = true;
        }


        public override void OnPop()
        {
            ClearFilterFields();
            base.OnPop();
        }

        protected override void SelectTabPage(TabPages tabPage)
        {
            // Select new tab page
            base.SelectTabPage(tabPage);

            ClearFilterFields();
            FilterRemoteItems();
            remoteItemListScroller.Items = remoteItemsFiltered;
        }

        private void UpdateFilterButton()
        {
            if (localFilterButton != null && localFilterTextBox != null && localCloseFilterButton != null)
                if (filterString != null && localFilterButton.Enabled)
                {
                    localFilterButton.Enabled = false;
                    localFilterTextBox.Enabled = true;
                    localCloseFilterButton.Enabled = true;
                }
                else
                {
                    localFilterButton.Enabled = true;
                    localFilterTextBox.Enabled = false;
                    localCloseFilterButton.Enabled = false;
                }
        }

        protected override void FilterLocalItems()
        {
            // Clear current references
            localItemsFiltered.Clear();

            if (localItems != null)
            {
                // Add items to list
                for (int i = 0; i < localItems.Count; i++)
                {
                    DaggerfallUnityItem item = localItems.GetItem(i);
                    // Add if not equipped
                    if (!item.IsEquipped)
                    {
                        if (ItemPassesFilter(item))
                            AddLocalItem(item);
                    }

                }
            }
        }

        protected override void FilterRemoteItems()
        {
            DaggerfallUnityItem item;
            // Clear current references
            remoteItemsFiltered.Clear();

            // Add items to list
            if (remoteItems != null)
                for (int i = 0; i < remoteItems.Count; i++)
                {

                    item = remoteItems.GetItem(i);
                    if (ItemPassesFilter(item) && TabPassesFilter(item))
                        remoteItemsFiltered.Add(item);
                }
        }

        bool TabPassesFilter(DaggerfallUnityItem item )
        {
            bool isWeaponOrArmor = (item.ItemGroup == ItemGroups.Weapons || item.ItemGroup == ItemGroups.Armor);

            if (usingWagon && !AsesinoInventoryWindow.SplitTabsOnWagon)
                return true;

            if (!usingWagon && !AsesinoInventoryWindow.SplitTabsOnLootDrops)
                return true;

            // Add based on view
            if (selectedTabPage == TabPages.WeaponsAndArmor)
            {
                // Weapons and armor
                if (isWeaponOrArmor && !item.IsEnchanted)
                    return true;
                else
                    return false;
            }
            else if (selectedTabPage == TabPages.MagicItems)
            {
                // Enchanted items
                if (item.IsEnchanted || item.IsOfTemplate((int) MiscItems.Spellbook))
                    return true;
                else
                    return false;
            }
            else if (selectedTabPage == TabPages.Ingredients)
            {
                // Ingredients
                if (item.IsIngredient && !item.IsEnchanted)
                    return true;
                else
                    return false;
            }
            else if (selectedTabPage == TabPages.ClothingAndMisc)
            {
                // Everything else
                if (!isWeaponOrArmor && !item.IsEnchanted && !item.IsIngredient &&
                    !item.IsOfTemplate((int) MiscItems.Spellbook))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        protected string GetSearchTags(DaggerfallUnityItem item)
        {
            var equipSlot = GameManager.Instance.PlayerEntity.ItemEquipTable.GetEquipSlot(item);
            switch (equipSlot)
            {
                case EquipSlots.None:
                    return string.Empty;
                case EquipSlots.Amulet0:
                case EquipSlots.Amulet1:
                    return Amulet;
                case EquipSlots.Bracelet0:
                case EquipSlots.Bracelet1:
                    return Bracelet;
                case EquipSlots.Bracer0:
                case EquipSlots.Bracer1:
                    return Bracer;
                case EquipSlots.Ring0:
                case EquipSlots.Ring1:
                    return Ring;
                case EquipSlots.Mark0:
                case EquipSlots.Mark1:
                    return Mark;
                case EquipSlots.Crystal0:
                case EquipSlots.Crystal1:
                    return Crystal;
                case EquipSlots.Head:
                    return Head;
                case EquipSlots.RightArm:
                    return RightArm;
                case EquipSlots.LeftArm:
                    return LeftArm;
                case EquipSlots.Cloak1:
                case EquipSlots.Cloak2:
                    return Cloak;
                case EquipSlots.ChestClothes:
                    return ChestClothes;
                case EquipSlots.ChestArmor:
                    return ChestArmor;
                case EquipSlots.RightHand:
                case EquipSlots.Gloves:
                    return RightHand;
                case EquipSlots.LeftHand:
                    return LeftHand;
                case EquipSlots.LegsArmor:
                    return LegsArmor;
                case EquipSlots.LegsClothes:
                    return LegsClothes;
                case EquipSlots.Feet:
                    return Feet;
                default:
                    return string.Empty;
            }

        }
        
        protected bool ItemPassesFilter(DaggerfallUnityItem item)
        {
            bool iterationPass = false;
            bool isRecipe = false;
            string recipeName = string.Empty;

            string str = string.Empty;
            str = GetSearchTags(item);

            Type itemClassType;
            if (item.TemplateIndex > ItemHelper.LastDFTemplate)
                if (GameManager.Instance.ItemHelper.GetCustomItemClass(item.TemplateIndex, out itemClassType) )
                    if (itemClassType != null)
                    {
                        str += " " + itemClassType.ToString();
                    }

            if (String.IsNullOrEmpty(filterString))
                return true;

            if (item.LongName.ToLower().Contains("recipe"))
            {
                TextFile.Token[] tokens = ItemHelper.GetItemInfo(item, DaggerfallUnity.TextProvider);
                MacroHelper.ExpandMacros(ref tokens, item);
                recipeName = tokens[0].text;
                isRecipe = true;
            }

            foreach (string word in filterString.Split(' '))
            {
                if (word.Trim().Length > 0)
                {
                    if (word[0] == '-')
                    {
                        string wordLessFirstChar = word.Remove(0, 1);
                        iterationPass = true;
                        if (item.LongName.IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = false;
                        else if (isRecipe &&  recipeName.IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = false;
                        else if (itemGroupNames[(int)item.ItemGroup].IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = false;
                        else if (str != null && str.IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = false;
                    }
                    else
                    {
                        iterationPass = false;
                        if (item.LongName.IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = true;
                        else if (isRecipe && recipeName.IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = true;
                        else if (itemGroupNames[(int)item.ItemGroup].IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = true;
                        else if (str != null && str.IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                                iterationPass = true;

                    }

                    if (!iterationPass)
                        return false;
                }
            }

            return true;
        }

        private void ClearFilterFields()
        {
            filterString = null;
            if (localFilterTextBox != null)
                localFilterTextBox.Text = string.Empty;
            UpdateFilterButton();
        }

        private void LocalFilterButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            filterString = "";
            localFilterTextBox.SetFocus();
            filterButtonNeedUpdate = true;
        }

        private void LocalCloseFilterButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ClearFilterFields();
            Refresh(false);
            SetFocus(null);
            filterButtonNeedUpdate = true;
        }

        private void LocalFilterTextBox_OnType()
        {
            filterString = localFilterTextBox.Text.ToLower();
            Refresh(false);
        }
    }
}
