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
using System.Linq;
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
        protected static Button localCloseFilterButton;
        protected bool filterButtonNeedUpdate;
        protected static Button localFilterButton;
        protected static Button localSortButton;
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

        public static bool ClearFilter { get; set; }
        public static int SortCriteria { get; set; }
        public static bool CheckGeneralStore { get; set; }
        public static bool CheckPawnShops { get; set; }
        public static bool CheckArmorer { get; set; }
        public static bool CheckWeaponShop { get; set; }
        public static bool CheckAlchemist { get; set; }
        public static bool CheckClothingStore { get; set; }
        public static bool CheckBookStore { get; set; }
        public static bool CheckGemStore { get; set; }


        #region Constructors

        public AsesinoTradeWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous,
            WindowModes windowMode, IGuild guild)
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
            ClearFilter = true;
            SetupTargetIconPanelFilterBox();
        }

        protected virtual void SetupTargetIconPanelFilterBox()
        {
            string toolTipText = string.Empty;
            toolTipText =
                "Press Filter Button to Open Filter Text Box.\rAnything typed into text box will autofilter.\rFor negative filter, type '-' in front.\rFor example, -steel weapon will find all weapons not made of steel.";


            localFilterTextBox = DaggerfallUI.AddTextBoxWithFocus(new Rect(new Vector2(1, 24), new Vector2(47, 8)),
                "filter pattern", localTargetIconPanel);
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

            localSortButton = DaggerfallUI.AddButton(new Rect(47, 10, 32, 8), localTargetIconPanel);
            localSortButton.Label.TextScale = 0.75f;
            localSortButton.VerticalAlignment = VerticalAlignment.Middle;
            localSortButton.HorizontalAlignment = HorizontalAlignment.Left;
            localSortButton.Label.ShadowColor = Color.black;
            localSortButton.BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);
            SortCriteria = 1;
            localSortButton.OnMouseClick += LocalSortButton_OnMouseClick;
            localSortButton.Label.Text = "By Name";

            filterButtonNeedUpdate = true;
        }

        void SetLocalSortButton(int val)
        {
            switch (val)
            {
                case 2:
                    localSortButton.Label.Text = "By Weight";
                    break;
                case 3:
                    localSortButton.Label.Text = "By Value";
                    break;
                case 4:
                    localSortButton.Label.Text = "By Value/Kg";
                    break;
                default:
                    localSortButton.Label.Text = "By Name";
                    break;
            }

            ClearFilter = false;
            SelectTabPage(selectedTabPage);
            ClearFilter = true;
            return;
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


        protected override void SelectTabPage(TabPages tabPage)
        {
            // Select new tab page
            base.SelectTabPage(tabPage);

            if (ClearFilter)
                ClearFilterFields();
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
                        if (ItemPassesFilter(item) && TabPassesFilter(item))
                            AddLocalItem(item);
                    }
                    else
                    {
                        if(GameManager.Instance.PlayerEnterExit.BuildingType == DaggerfallConnect.DFLocation.BuildingTypes.Alchemist &&
                           item.LongName.ToLower().Contains("potion"))
                        {
                            if (ItemPassesFilter(item) && TabPassesFilter(item))
                                AddLocalItem(item);
                        }

                    }
                }

                if (localItemsFiltered.Count > 0)
                    SortMe(ref localItemsFiltered);
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
                FilterRemoteItemsWithoutRepair();
        }


        protected virtual void FilterRemoteItemsWithoutRepair()
        {
            DaggerfallUnityItem item;
            // Clear current references
            remoteItemsFiltered.Clear();

            // Add items to list
            if (remoteItems != null)
            {
                for (int i = 0; i < remoteItems.Count; i++)
                {

                    item = remoteItems.GetItem(i);
                    if (ItemPassesFilter(item) && TabPassesFilter(item))
                        remoteItemsFiltered.Add(item);
                }
            }

            if (remoteItemsFiltered.Count > 0)
                SortMe(ref remoteItemsFiltered);
        }

        protected bool TabPassesFilter(DaggerfallUnityItem item)
        {
            bool isWeaponOrArmor = (item.ItemGroup == ItemGroups.Weapons || item.ItemGroup == ItemGroups.Armor);

            if (WindowMode == WindowModes.Sell || WindowMode == WindowModes.SellMagic)
                return true;

            var buildingType = GameManager.Instance.PlayerEnterExit.BuildingType;

            switch (buildingType)
            {
                case DFLocation.BuildingTypes.GeneralStore:
                    if (!AsesinoTradeWindow.CheckGeneralStore)
                        return true;
                    break;
                case DFLocation.BuildingTypes.PawnShop:
                    if (!AsesinoTradeWindow.CheckPawnShops)
                        return true;
                    break;
                case DFLocation.BuildingTypes.Armorer:
                    if (!AsesinoTradeWindow.CheckArmorer)
                        return true;
                    break;
                case DFLocation.BuildingTypes.WeaponSmith:
                    if (!AsesinoTradeWindow.CheckWeaponShop)
                        return true;
                    break;
                case DFLocation.BuildingTypes.Bookseller:
                    if (!AsesinoTradeWindow.CheckBookStore)
                        return true;
                    break;
                case DFLocation.BuildingTypes.Alchemist:
                    if (!AsesinoTradeWindow.CheckAlchemist)
                        return true;
                    break;
                case DFLocation.BuildingTypes.ClothingStore:
                    if (!AsesinoTradeWindow.CheckClothingStore)
                        return true;
                    break;
                case DFLocation.BuildingTypes.GemStore:
                    if (!AsesinoTradeWindow.CheckGemStore)
                        return true;
                    break;
                default:
                    return true;
            }

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
                if (item.IsEnchanted || item.IsOfTemplate((int)MiscItems.Spellbook))
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
                    !item.IsOfTemplate((int)MiscItems.Spellbook))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        protected virtual bool ItemPassesFilter(DaggerfallUnityItem item)
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

        protected static bool SortMe(ref List<DaggerfallUnityItem> sortList)
        {

            switch (SortCriteria)
            {
                case 2:
                    sortList = sortList.OrderByDescending(x => x.IsQuestItem).ThenByDescending(x => x.weightInKg).ToList();
                    return true;
                case 3:
                    sortList = sortList.OrderByDescending(x => x.IsQuestItem).ThenByDescending(x => x.value).ToList();
                    return true;
                case 4:
                    sortList = sortList.OrderByDescending(x => x.IsQuestItem).ThenByDescending(x => x.value / (x.weightInKg == 0 ? 1 : x.weightInKg)).ToList();
                    return true;
                default:
                    sortList = sortList.OrderByDescending(x => x.IsQuestItem).ThenBy(x => x.LongName).ToList();
                    return true;
            }

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

        private void LocalSortButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SortCriteria += 1;
            if (SortCriteria > 4 || SortCriteria < 1)
                SortCriteria = 1;

            SetLocalSortButton(SortCriteria);
            return;
        }

        private void LocalFilterTextBox_OnType()
        {
            filterString = localFilterTextBox.Text.ToLower();
            Refresh(false);
        }
        

    }

}
