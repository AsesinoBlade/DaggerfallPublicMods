using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using System;
using System.Collections;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop
{

    public class AddTelescope : MonoBehaviour
    {
        DaggerfallLoot daggerfallLoot;
        Telescope telescope;

        private void Start()
        {
            daggerfallLoot = this.GetComponent<DaggerfallLoot>();
            DaggerfallUI.UIManager.OnWindowChange += AddStock;
        }

        private void AddStock(object sender, EventArgs e)
        {
            var manager = (UserInterfaceManager)sender;

            if (UIWindowFactory.GetWindowType(manager.TopWindow.GetType()) == UIWindowType.Trade)
            {
                if (!daggerfallLoot.Items.Contains(telescope))
                {
                    telescope = new Telescope(DaggerfallWorkshop._startTelescopes.telescopeCost);
                    //telescope = new Telescope(GenerateTelescopeItemData());
                    daggerfallLoot.Items.AddItem(telescope, ItemCollection.AddPosition.Front);
                    manager.TopWindow.Update();
                }
            }
        }

        private ItemData_v1 GenerateTelescopeItemData()
        {
            ItemData_v1 itemData = new ItemData_v1();
            itemData.uid = DaggerfallUnity.NextUID;
            itemData.shortName = "Telescope";
            itemData.nativeMaterialValue = 0;
            itemData.dyeColor = DyeColors.Chain;
            itemData.weightInKg = 1.0f;
            itemData.drawOrder = 0;
            itemData.value1 = DaggerfallWorkshop._startTelescopes.telescopeCost;
            itemData.value2 = 0;
            itemData.hits1 = 50;
            itemData.hits2 = 50;
            itemData.hits3 = 0;
            itemData.stackCount = 1;
            itemData.enchantmentPoints = 75;
            itemData.message = 0;
            itemData.legacyMagic = null;
            itemData.customMagic = null;
            itemData.playerTextureArchive = 0;
            itemData.playerTextureRecord = 0;
            itemData.worldTextureArchive = 208;
            itemData.worldTextureRecord = 4;
            itemData.itemGroup = ItemGroups.QuestItems;
            itemData.groupIndex = 0;
            itemData.currentVariant = 0;
            itemData.isQuestItem = false;
            itemData.questUID = 0;
            itemData.questItemSymbol = null;
            itemData.trappedSoulType = MobileTypes.None;
            itemData.className = "Telescope";
            itemData.poisonType = Poisons.None;
            itemData.potionRecipe = 0;
            itemData.repairData = null;
            itemData.timeForItemToDisappear = 0;
            itemData.timeHealthLeechLastUsed = 0;
            return itemData;
        }

    }

}
