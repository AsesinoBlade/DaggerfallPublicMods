
using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallConnect;

namespace Telescopes
{
    /// <summary>
    /// Abstract class for all repair items common behaviour
    /// </summary>
    public abstract class AbstractItemScopes : DaggerfallUnityItem
    {
        UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;

        public AbstractItemScopes(ItemGroups itemGroup, int templateIndex) : base(itemGroup, templateIndex)
        {
        }

        public abstract void UpdateCost(int v);

        public abstract int DurabilityLoss { get; }

        public abstract bool IsStationary { get; }

        public abstract uint GetItemID();

        public abstract int MaxZoom { get;}

        public abstract bool NightVision { get; }









        //public abstract Rect TelescopeDirectionPos { get; set; }

        //public abstract Texture2D TelescopeLens { get; set; }

        public abstract Texture2D CalculateTexture();

        public abstract Rect CalculateTelescopeDirectionPos();

        public override bool UseItem(ItemCollection collection)
        {
            if (!GameManager.Instance.PlayerEnterExit.IsPlayerSubmerged)
            {
                Telescopes.TelescopeAndBinoculars.TelescopeEnabled = true;
                Telescopes.TelescopeAndBinoculars.curParalyzed = GameManager.Instance.PlayerEntity.IsParalyzed;
                Telescopes.TelescopeAndBinoculars.isStationary = IsStationary;
                Telescopes.TelescopeAndBinoculars.maxLevel = MaxZoom;
                Telescopes.TelescopeAndBinoculars.TelescopeDirectionPos = CalculateTelescopeDirectionPos();
                Telescopes.TelescopeAndBinoculars.telescopeLens = CalculateTexture();
                Telescopes.TelescopeAndBinoculars.maxLevel = Mathf.Min(Telescopes.TelescopeAndBinoculars.absMaxLevel, MaxZoom);
                Telescopes.TelescopeAndBinoculars.nightVision = NightVision;
                DaggerfallUI.UIManager.PopWindow();
                Telescopes.TelescopeAndBinoculars.TelescopeEnabled = true;
                return true;
            }
            else
                DaggerfallUI.MessageBox($"{this.ItemName} does not work under water.");
            return false;
        }

        public static Texture2D CalculateTelescopeTexture()
        {
            int w = Screen.width;
            int h = Screen.height;
            int cw = w;
            int ch = h;
            Texture2D telescopeLens = new Texture2D(cw, ch);


            bool portrait = w > h ? false : true;

            int r = portrait ? cw / 2 : ch / 2;

            for (int i = 0; i < cw; i++)
                for (int j = 0; j < ch; j++)
                    telescopeLens.SetPixel(i, j, Color.black);
            telescopeLens.Apply();

            for (int i = (int)(cw - r); i < cw + r; i++)
            {
                for (int j = (int)(ch - r); j < ch + r; j++)
                {
                    float dw = i - cw;
                    float dh = j - ch;
                    float d = Mathf.Sqrt(dw * dw + dh * dh);
                    if (d <= r)
                        if (portrait)
                            telescopeLens.SetPixel(i - (int)(cw - r), j + (ch / 2) - r - (int)(ch - r), Color.clear);
                        else
                            telescopeLens.SetPixel(i + (cw / 2) - r - (int)(cw - r), j - (int)(ch - r), Color.clear);
                }
            }
            telescopeLens.Apply();
            telescopeLens.Compress(true);
            return telescopeLens;
        }


        public static Texture2D CalculateBinocularTexture()
        {
            int w = Screen.width;
            int h = Screen.height;

            int cw = w;
            int ch = h;
            Texture2D BinocularsLens = new Texture2D(cw, ch);


            bool portrait = w > h ? false : true;

            int r = portrait ? cw / 2 : ch / 2;
            int spacing = (int)((float)r * 1.1f);
            int left = portrait ? h - spacing : w - spacing;
            int right = portrait ? h + spacing : w + spacing;

            for (int i = 0; i < cw; i++)
                for (int j = 0; j < ch; j++)
                    BinocularsLens.SetPixel(i, j, Color.black);
            BinocularsLens.Apply();

            cw = left;

            for (int i = (int)(cw - r); i < cw + r; i++)
            {
                for (int j = (int)(ch - r); j < ch + r; j++)
                {
                    float dw = i - cw;
                    float dh = j - ch;
                    float d = Mathf.Sqrt(dw * dw + dh * dh);
                    if (d <= r)
                        if (portrait)
                            BinocularsLens.SetPixel(i - (int)(cw - r), j + (ch / 2) - r - (int)(ch - r), Color.clear);
                        else
                            BinocularsLens.SetPixel(i + (cw / 2) - r - (int)(cw - r), j - (int)(ch - r), Color.clear);
                }
            }

            cw = right;

            for (int i = (int)(cw - r); i < cw + r; i++)
            {
                for (int j = (int)(ch - r); j < ch + r; j++)
                {
                    float dw = i - cw;
                    float dh = j - ch;
                    float d = Mathf.Sqrt(dw * dw + dh * dh);
                    if (d <= r)
                        if (portrait)
                            BinocularsLens.SetPixel(i - (int)(cw - r), j + (ch / 2) - r - (int)(ch - r), Color.clear);
                        else
                            BinocularsLens.SetPixel(i + (cw / 2) - r - (int)(cw - r), j - (int)(ch - r), Color.clear);
                }
            }

            BinocularsLens.Apply();
            BinocularsLens.Compress(true);
            return BinocularsLens;
        }
    }
}