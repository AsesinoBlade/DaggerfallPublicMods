using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Items;


namespace DaggerfallWorkshop
{

    public class _startTelescopes : MonoBehaviour
    {
        static Mod mod;
        private KeyCode currKey;
        public KeyCode toggleKey = KeyCode.End;
        public KeyCode escKey = KeyCode.Escape;
        public string toggleString = "END";
        public KeyCode upKey = KeyCode.UpArrow;
        public string upString = "Up Arrow";
        public KeyCode downKey = KeyCode.DownArrow;
        public string downString = "Down Arrow";
        
        private Camera fovCamera;
        float prevFov;
        float origFov;
        protected static bool telescopeEnabled = false;
        public static int telescopeCost = 7;
        public static PlayerEntity pe;
        public static bool curParalyzed;
        public Texture2D telescopeLens;
        public Texture2D raindrops;
        public int screenX;
        public int screenY;
        public KeyCode thisKeyCode;
        public bool telescopeOverlay = true;
        public Rect pos;
        public Camera mainCamera = GameManager.Instance.MainCamera;
        public Rect TelescopeDirectionPos;
        public GUIStyle guiStyle = new GUIStyle();
        public bool showTelescopeDirections = false;

        public int[] levelTbl = new int[20];
        public int[] zoomTbl = new int[20];

        public int currZoom = 1;
        public int maxLevel = 1;

        public static bool TelescopeEnabled
        {

            get { return telescopeEnabled; }
            set
            {
                telescopeEnabled = value;
            }
        }

        public void Awake()
        {
            pe = FindPlayerEntity();
            Debug.Log("tele reading parms");
            AssignSettings();
            Debug.Log("tele read parms");
            
            mod.IsReady = true;
        }

        public void Start()
        {
            ItemCollection.RegisterCustomItem(typeof(Telescope).ToString(), typeof(Telescope));

            GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (camObj)
            {
                fovCamera = camObj.GetComponent<Camera>();
                origFov = fovCamera.fieldOfView;
                prevFov = origFov;
                PopulateZoomLevel();
            }

            screenX = Screen.width;
            screenY = Screen.height;

            pos = new Rect(Vector2.zero, new Vector2(screenX, screenY));


            telescopeLens = CalculateTexture(screenX, screenY);
            raindrops = (Texture2D)Resources.Load("textures\raindrops.IMG.jpg");
            PlayerEnterExit.OnTransitionInterior += CheckAlchemist;


            guiStyle.normal.textColor = Color.yellow;
            guiStyle.fontSize = 20;

            int posX = 0;
            int posY = 0;
            if (telescopeOverlay)
                if (screenX > screenY ) {
                    posX = (screenX / 2 + screenY / 2) + 10;
                    posY = screenY / 2 - 100;
                }
                else
                {
                    posX = screenX / 2 - 100 ;
                    posY = (screenX / 2 + screenY / 2) + 10; ;
                }
            else
            {
                posX = screenX / 2 - 100;
                posY = (screenY - 250);
            }

            TelescopeDirectionPos = new Rect(posX, posY, 200, 200);
        }

        Texture2D CalculateTexture(int w, int h)
        {
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

        void AssignSettings()
        {
            var settings = mod.GetSettings();

            int dropDownExitKey = settings.GetValue<int>("ExitTelescopeKey", "DropDownExitKeys");
            toggleKey = AssignFromDropDown(dropDownExitKey, out toggleString);

            int dropDownZoomInKey = settings.GetValue<int>("ZoomInKey", "DropDownZoomInKey");
            upKey = AssignFromDropDown(dropDownZoomInKey, out upString);

            int dropDownZoomOutKey = settings.GetValue<int>("ZoomOutKey", "DropDownZoomOutKey");
            downKey = AssignFromDropDown(dropDownZoomOutKey, out downString);

            telescopeCost = settings.GetValue<int>("TelecopePrice", "TelescopePrice");
            telescopeOverlay = settings.GetValue<bool>("TelescopeOverlay", "UseTelescopeOverlay");

            showTelescopeDirections = settings.GetValue<bool>("TelescopeOverlay", "ShowTelescopeDirectionsOnScreen");


            return;
        }

        PlayerEntity FindPlayerEntity()
        {
            DaggerfallEntityBehaviour entityBehaviour;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            entityBehaviour = player.GetComponent<DaggerfallEntityBehaviour>();
            PlayerEntity playerEntity = entityBehaviour.Entity as PlayerEntity;

            return playerEntity;
        }


        KeyCode AssignFromDropDown(int dropDownKey, out string keyString)
        {
            KeyCode key = KeyCode.End;
            switch (dropDownKey)
            {
                case 0: //End
                    key = KeyCode.End;
                    keyString = "End";
                    break;
                case 1: //Del
                    key = KeyCode.Delete;
                    keyString = "Delete";
                    break;
                case 2:  //Home
                    key = KeyCode.Home;
                    keyString = "Home";
                    break;
                case 3:  //Ins
                    key = KeyCode.Insert;
                    keyString = "Insert";
                    break;
                case 4:  //Page Up
                    key = KeyCode.PageUp;
                    keyString = "Page Up";
                    break;
                case 5: //Page Down
                    key = KeyCode.PageDown;
                    keyString = "Page Down";
                    break;
                case 6: //Up Arrow
                    key = KeyCode.UpArrow;
                    keyString = "Up Arrow";
                    break;
                case 7: //Down Arrow
                    key = KeyCode.DownArrow;
                    keyString = "Down Arrow";
                    break;
                case 8:  //Left Arrow
                    key = KeyCode.LeftArrow;
                    keyString = "Left Arrow";
                    break;
                case 9: //Right Arrow
                    key = KeyCode.RightArrow;
                    keyString = "Right Arrow";
                    break;
                case 10: //F8
                    key = KeyCode.F8;
                    keyString = "F8";
                    break;
                case 11: //F9
                    key = KeyCode.F9;
                    keyString = "F9";
                    break;
                case 12:  //F10
                    key = KeyCode.F10;
                    keyString = "F10";
                    break;
                case 13: //F11
                    key = KeyCode.F11;
                    keyString = "F11";
                    break;
                case 14: //F12
                    key = KeyCode.F12;
                    keyString = "F12";
                    break;
                default:
                    keyString = String.Format("{0}", (char)(dropDownKey + 50));
                    key = (KeyCode)Enum.Parse(typeof(KeyCode), keyString);
                    break;
            }
            return key;
        }

        public void CheckAlchemist(EventArgs e)
        {
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideOpenShop && GameManager.Instance.PlayerEnterExit.BuildingType == DaggerfallConnect.DFLocation.BuildingTypes.Alchemist)
            {
                DaggerfallLoot[] gos = FindObjectsOfType<DaggerfallLoot>() as DaggerfallLoot[];

                foreach (DaggerfallLoot gObj in gos)
                {
                         if (gObj.ContainerType == LootContainerTypes.ShopShelves)
                            gObj.gameObject.AddComponent<AddTelescope>();
                }
            }
        }

        private void OnGUI()
        {
            if (!TelescopeEnabled)
                return;

            if (Event.current.type.Equals(EventType.Repaint) && !GameManager.IsGamePaused)
            {
                GUI.depth = 1;
                if (telescopeOverlay)
                    GUI.DrawTexture(pos, telescopeLens, ScaleMode.StretchToFill);

                if (showTelescopeDirections)
                {
                    string str = levelTbl[currZoom].ToString() + " X\n\n" + "Press [" + toggleString + "] to exit. \n\n" + "Press [" + upString + "] to zoom In.\n" + "Press [" + downString + "] to zoom out.";
                    GUI.Label(TelescopeDirectionPos, str, guiStyle);
                }
            }
        }

         public void Update()
        {
            if (!TelescopeEnabled)
                return;

            if (DaggerfallUI.UIManager.TopWindow.GetType() != typeof(DaggerfallHUD))
            {
                fovCamera.fieldOfView = origFov;
                return;
            }

            fovCamera.fieldOfView = prevFov;

            if (GameManager.Instance.StateManager.CurrentState == StateManager.StateTypes.Game && (Input.GetKeyDown(toggleKey) || Input.GetKeyDown(escKey)))
            {
                TelescopeEnabled = !TelescopeEnabled;
                fovCamera.fieldOfView = origFov;
                pe.IsParalyzed = curParalyzed;
                currZoom = 1;
            }

            if (TelescopeEnabled)
            {
                pe.IsParalyzed = true;
                if ((Input.GetAxis("Mouse ScrollWheel") > 0f) || (Input.GetKeyDown(upKey)))
                {

                    if (currZoom < maxLevel)
                    prevFov = zoomTbl[++currZoom];
                    fovCamera.fieldOfView = prevFov;
                }

                if ((Input.GetAxis("Mouse ScrollWheel") < 0f) || (Input.GetKeyDown(downKey)))
                {

                    if (currZoom > 1)
                    prevFov = zoomTbl[--currZoom];
                    fovCamera.fieldOfView = prevFov;
                }
            }
        }

        private void PopulateZoomLevel()
        {
            double factor;
            double zoomedFOV;
            int cnt = 1;
            int pos = 1;
            int magnification = 1;
            int prevZFov = 0;
            int zFOV = 2;

            factor = 2.0 * (Math.Tan(0.5 * origFov * Math.PI / 180.0));
            while (cnt <= 16 )
            {
                zoomedFOV = 2.0 * Math.Atan(factor / (2.0 * magnification)) * 180.0 / Math.PI;
                zFOV = (int)Math.Round(zoomedFOV, 0);
                if (zFOV < 1)
                    zFOV = 1;
                if (zFOV != prevZFov)
                {
                    levelTbl[pos] = magnification;
                    zoomTbl[pos] = zFOV;
                    prevZFov = zFOV;
                    pos++;
                }

                if (cnt >= 10)
                    magnification += 5;
                else
                    magnification++;
                cnt++;
            }
            cnt = 16;
            maxLevel = pos - 1;
        }

        private void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            List<DaggerfallUnityItem> telescopes = GameManager.Instance.PlayerEntity.Items.SearchItems(ItemGroups.QuestItems, 0);
            foreach (DaggerfallUnityItem item in telescopes)
            {
                Debug.LogError("telescope found =" + item.shortName);
            }
        }

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            Debug.Log("Tele about to add component");
            go.AddComponent<_startTelescopes>();
            Debug.Log("Tele added component");
        }
    }
}

