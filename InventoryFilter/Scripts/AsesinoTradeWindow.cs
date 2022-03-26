// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    Pango
// Extended by:     Asesino, Pango
// Notes:
//
using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements trade windows, based on inventory window.
    /// </summary>
    public partial class AsesinoTradeWindow : DaggerfallTradeWindow, IMacroContextProvider
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

        #region Constructors

        public AsesinoTradeWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous, WindowModes windowMode, IGuild guild)
            : base(uiManager, previous, windowMode, guild)
        {


        }

        public AsesinoTradeWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous, WindowModes.Sell, null)
        {


        }

        #endregion

        protected override void Setup()
        {
            base.Setup();
            SetupTargetIconPanelFilterBox();
        }

        protected  void SetupTargetIconPanelFilterBox()
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



        public override void Update()
        {
            base.Update();

            if (localFilterTextBox.HasFocus() && Input.GetKeyDown(KeyCode.Return))
                SetFocus(null);

            if (filterButtonNeedUpdate)
            {
                filterButtonNeedUpdate = false;
                UpdateFilterButton();
            }
       }


        protected new void SelectTabPage(TabPages tabPage)
        {
            // Select new tab page
            base.SelectTabPage(tabPage);

            //ClearFilterFields();
            FilterRemoteItems();
            remoteItemListScroller.Items = remoteItemsFiltered;
        }
        
        protected override void FilterLocalItems()
        {
            localItemsFiltered.Clear();

            // Add any basket items to filtered list first, if not using wagon
            if (WindowMode == WindowModes.Buy && !UsingWagon && BasketItems != null)
            {
                for (int i = 0; i < BasketItems.Count; i++)
                {
                    DaggerfallUnityItem item = BasketItems.GetItem(i);
                    // Add if not equipped
                    if (!item.IsEquipped)
                    {
                        AddLocalItem(item);
                    }

                }
            }
            // Add local items to filtered list
            if (localItems != null)
            {
                for (int i = 0; i < localItems.Count; i++)
                {
                    // Add if not equipped & accepted for selling
                    DaggerfallUnityItem item = localItems.GetItem(i);
                    if (!item.IsEquipped && (
                            (WindowMode != WindowModes.Sell && WindowMode != WindowModes.SellMagic) ||
                            (WindowMode == WindowModes.Sell && ItemTypesAccepted.Contains(item.ItemGroup)) ||
                            (WindowMode == WindowModes.SellMagic && item.IsEnchanted)))
                    {
                        if (ItemPassesFilter(item))
                            AddLocalItem(item);
                    }
                    else
                    {
                        if(GameManager.Instance.PlayerEnterExit.BuildingType == DaggerfallConnect.DFLocation.BuildingTypes.Alchemist &&
                           item.LongName.ToLower().Contains("potion"))
                        {
                            if (ItemPassesFilter(item))
                                AddLocalItem(item);
                        }

                    }
                }
            }
        }

        protected override void FilterRemoteItems()
        {
            if (WindowMode == WindowModes.Repair)
            {
                // Clear current references
                remoteItemsFiltered.Clear();

                // Add items to list if they are not being repaired or are being repaired here. 
                if (remoteItems != null)
                {
                    for (int i = 0; i < remoteItems.Count; i++)
                    {
                        DaggerfallUnityItem item = remoteItems.GetItem(i);
                        if (!item.RepairData.IsBeingRepaired() || item.RepairData.IsBeingRepairedHere())
                            remoteItemsFiltered.Add(item);
                        if (item.RepairData.IsRepairFinished())
                            item.currentCondition = item.maxCondition;
                    }
                }
                UpdateRepairTimes(false);
            }
            else
                FilterRemoteItemsWithoutExtraFilter();
        }

        protected  void FilterRemoteItemsWithoutExtraFilter()
        {
            // Clear current references
            remoteItemsFiltered.Clear();

            // Add items to list
            if (remoteItems != null)
                for (int i = 0; i < remoteItems.Count; i++)
                    if (ItemPassesFilter(remoteItems.GetItem(i)))
                        remoteItemsFiltered.Add(remoteItems.GetItem(i));
        }
        
        protected bool ItemPassesFilter(DaggerfallUnityItem item)
        {
            bool iterationPass = false;

            if (String.IsNullOrEmpty(filterString))
                return true;

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
                        else if (itemGroupNames[(int)item.ItemGroup].IndexOf(wordLessFirstChar, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = false;
                    }
                    else
                    {
                        iterationPass = false;
                        if (item.LongName.IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = true;
                        else if (itemGroupNames[(int)item.ItemGroup].IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1)
                            iterationPass = true;
                    }

                    if (!iterationPass)
                        return false;
                }
            }

            return true;
        }
        

        private void UpdateFilterButton()
        {
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
        private void ClearFilterFields()
        {
            filterString = null;
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
            filterButtonNeedUpdate = true;
        }


        private void LocalFilterTextBox_OnType()
        {
            filterString = localFilterTextBox.Text.ToLower();
            Refresh(false);
        }
        

    }

}
