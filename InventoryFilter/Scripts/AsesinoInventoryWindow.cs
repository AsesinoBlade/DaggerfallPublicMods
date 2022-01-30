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

        protected new void SelectTabPage(TabPages tabPage)
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
                    if (ItemPassesFilter(item))
                        remoteItemsFiltered.Add(item);
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
                    return RegisterInventoryWindow.Amulet;
                case EquipSlots.Bracelet0:
                case EquipSlots.Bracelet1:
                    return RegisterInventoryWindow.Bracelet;
                case EquipSlots.Bracer0:
                case EquipSlots.Bracer1:
                    return RegisterInventoryWindow.Bracer;
                case EquipSlots.Ring0:
                case EquipSlots.Ring1:
                    return RegisterInventoryWindow.Ring;
                case EquipSlots.Mark0:
                case EquipSlots.Mark1:
                    return RegisterInventoryWindow.Mark;
                case EquipSlots.Crystal0:
                case EquipSlots.Crystal1:
                    return RegisterInventoryWindow.Crystal;
                case EquipSlots.Head:
                    return RegisterInventoryWindow.Head;
                case EquipSlots.RightArm:
                    return RegisterInventoryWindow.RightArm;
                case EquipSlots.LeftArm:
                    return RegisterInventoryWindow.LeftArm;
                case EquipSlots.Cloak1:
                case EquipSlots.Cloak2:
                    return RegisterInventoryWindow.Cloak;
                case EquipSlots.ChestClothes:
                    return RegisterInventoryWindow.ChestClothes;
                case EquipSlots.ChestArmor:
                    return RegisterInventoryWindow.ChestArmor;
                case EquipSlots.RightHand:
                case EquipSlots.Gloves:
                    return RegisterInventoryWindow.RightHand;
                case EquipSlots.LeftHand:
                    return RegisterInventoryWindow.LeftHand;
                case EquipSlots.LegsArmor:
                    return RegisterInventoryWindow.LegsArmor;
                case EquipSlots.LegsClothes:
                    return RegisterInventoryWindow.LegsClothes;
                case EquipSlots.Feet:
                    return RegisterInventoryWindow.Feet;
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

            if (str != string.Empty)
            {
                Debug.Log($"returned {str}");
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
                        else if (str.IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
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
                        else if (str.IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
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
