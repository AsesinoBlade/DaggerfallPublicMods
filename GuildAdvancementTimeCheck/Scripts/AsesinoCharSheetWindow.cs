using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Guilds;
using UnityEngine.Localization.SmartFormat.Utilities;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class AsesinoCharSheetWindow : DaggerfallCharacterSheetWindow
    {
        private const int noAffiliationsMsgId = 19;
        PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

        struct BuildingInfo
        {
            public string name;
            public DFLocation.BuildingTypes buildingType;
            public int buildingKey;
            public Vector2 position;
        }

        public AsesinoCharSheetWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            base.Setup();
            // Affiliations button
            Button classButton = DaggerfallUI.AddButton(new Rect(4, 23, 132, 8), NativePanel);
            classButton.OnMouseClick += classsButton_OnMouseClick;
            characterPortrait.OnMouseClick += showRepairs_OnMouseClick;
        }

        private void classsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowReputations();
        }

        private void showRepairs_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowRepairs();
        }

        protected TextFile.Token HighLightToken(string text)
        {
            var token = new TextFile.Token()
            {
                text = text,
                formatting = TextFile.Formatting.TextHighlight
            };
            return token;
        }
        protected override void ShowSkillsDialog(List<DFCareer.Skills> skills, bool twoColumn = false)
        {
            bool secondColumn = false;
            bool showHandToHandDamage = false;
            List<TextFile.Token> tokens = new List<TextFile.Token>();


            if (twoColumn)
            {
                var tk = new List<TextFile.Token>();
                tk.Add(HighLightToken("Skill"));
                tk.Add(TextFile.TabToken);
                tk.Add(HighLightToken("Next Lvl"));
                tk.Add(TextFile.TabToken);
                tk.Add(HighLightToken("   Curr Lvl"));
                tk.Add(TextFile.TabToken);
                tk.Add(HighLightToken("  ATR"));
                tokens.AddRange(tk);

                tk = new List<TextFile.Token>();
                tk.Add(TextFile.TabToken);
                tk.Add(HighLightToken("Skill"));
                tk.Add(TextFile.TabToken);
                tk.Add(HighLightToken("Next Lvl"));
                tk.Add(TextFile.TabToken);
                tk.Add(HighLightToken("Curr Lvl"));
                tk.Add(TextFile.TabToken);
                tk.Add(HighLightToken("ATR"));
                tk.Add(TextFile.NewLineToken);
                tokens.AddRange(tk);
            }
            else
            {
                tokens.Add(HighLightToken("Skill"));
                tokens.Add(TextFile.TabToken);
                tokens.Add(TextFile.TabToken);
                tokens.Add(HighLightToken("Next Lvl"));
                tokens.Add(TextFile.TabToken);
                tokens.Add(HighLightToken("Curr Lvl"));
                tokens.Add(TextFile.TabToken);
                tokens.Add(HighLightToken("ATR"));
                tokens.Add(TextFile.NewLineToken);
            }
            for (int i = 0; i < skills.Count; i++)
            {
                if (!showHandToHandDamage && (skills[i] == DFCareer.Skills.HandToHand))
                    showHandToHandDamage = true;

                if (!twoColumn)
                {
                    tokens.AddRange(CreateSkillTokens(skills[i]));
                    if (i < skills.Count - 1)
                        tokens.Add(TextFile.NewLineToken);
                }
                else
                {
                    if (!secondColumn)
                    {
                        tokens.AddRange(CreateSkillTokens(skills[i], twoColumn: true));
                        secondColumn = !secondColumn;
                    }
                    else
                    {
                        tokens.AddRange(CreateSkillTokens(skills[i], true, 136));
                        secondColumn = !secondColumn;
                        if (i < skills.Count - 1)
                            tokens.Add(TextFile.NewLineToken);
                    }
                }
            }

            if (showHandToHandDamage)
            {
                tokens.Add(TextFile.NewLineToken);
                TextFile.Token HandToHandDamageToken = new TextFile.Token();
                int minDamage = FormulaHelper.CalculateHandToHandMinDamage(playerEntity.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
                int maxDamage = FormulaHelper.CalculateHandToHandMaxDamage(playerEntity.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
                HandToHandDamageToken.text = DaggerfallUnity.Instance.TextProvider.GetSkillName(DFCareer.Skills.HandToHand) + " dmg: " + minDamage + "-" + maxDamage;
                HandToHandDamageToken.formatting = TextFile.Formatting.Text;
                tokens.Add(HandToHandDamageToken);
            }

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetHighlightColor(DaggerfallUI.DaggerfallUnityStatIncreasedTextColor);
            messageBox.SetTextTokens(tokens.ToArray(), null, false);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
        }

        private int GetRegionAndMapIndex(int mapID, out int mapIndex)
        {
            mapIndex = -1;
            if (GameManager.Instance.PlayerGPS.CurrentRegion.MapIdLookup.ContainsKey(mapID))
            {
                mapIndex = GameManager.Instance.PlayerGPS.CurrentRegion.MapIdLookup[mapID];
                return GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            }

            for (int region = 0; region < GameManager.Instance.PlayerEntity.RegionData.Length; region++)
            {
                var regionData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(region);
                if (regionData.MapIdLookup == null || regionData.MapIdLookup.Count == 0)
                    continue;
                if (!regionData.MapIdLookup.ContainsKey(mapID))
                    continue;

                mapIndex = regionData.MapIdLookup[mapID];
                return region;
            }
            
            return -1;
        }
        private string GetTownName(int region,int mapIndex)
        {
            var regionData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(region);

            foreach (KeyValuePair<string, int> kvp in regionData.MapNameLookup) 
            {
                if (kvp.Value == mapIndex)
                {
                    return kvp.Key;
                }
            }

            return string.Empty;
        }

        private string GetBuildingName(int region, int mapIndex, int buildingKey)
        {
            var regionData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(region);
            var location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(region, mapIndex);


            ExteriorAutomap.BlockLayout[] blockLayout = GameManager.Instance.ExteriorAutomap.ExteriorLayout;

            DFBlock[] blocks = RMBLayout.GetLocationBuildingData(location);
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;

            int[] workStats = new int[12];
            int index = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x, ++index)
                {
                    ref readonly DFBlock block = ref blocks[index];
                    BuildingSummary[] buildingsInBlock = RMBLayout.GetBuildingData(block, x, y);
                    for (int i = 0; i < buildingsInBlock.Length; ++i)
                    {
                        ref readonly BuildingSummary buildingSummary = ref buildingsInBlock[i];
                        if (buildingSummary.buildingKey == buildingKey)
                        {
                            try
                            {
                                BuildingInfo item;
                                item.buildingType = buildingSummary.BuildingType;
                                return BuildingNames.GetName(
                                    buildingSummary.NameSeed,
                                    buildingSummary.BuildingType,
                                    buildingSummary.FactionId,
                                    location.Name,
                                    TextManager.Instance.GetLocalizedRegionName(location.RegionIndex));

                            }
                            catch (Exception e)
                            {
                                string exceptionMessage = string.Format(
                                    "exception occured in function BuildingNames.GetName (exception message: " +
                                    e.Message + @") with params:
                                                                        seed: {0}, type: {1}, factionID: {2}, locationName: {3}, regionName: {4}",
                                    buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId,
                                    location.Name, location.RegionName);
                                DaggerfallUnity.LogMessage(exceptionMessage, true);
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        protected override void ShowAffiliationsDialog()
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            List<IGuild> guildMemberships = GameManager.Instance.GuildManager.GetMemberships();

            if (guildMemberships.Count == 0)
                DaggerfallUI.MessageBox(noAffiliationsMsgId);
            else
            {
                TextFile.Token tab = TextFile.TabToken;
                tab.x = 125;
                tokens.Add(new TextFile.Token()
                {
                    text = "Affiliation",
                    formatting = TextFile.Formatting.TextHighlight
                }); ;
                tokens.Add(tab);
                tokens.Add(new TextFile.Token()
                {
                    text = "Rank",
                    formatting = TextFile.Formatting.TextHighlight
                });
                tokens.Add(TextFile.NewLineToken);

                foreach (IGuild guild in guildMemberships)
                {
                    tokens.Add(TextFile.CreateTextToken(guild.GetAffiliation()));
                    tokens.Add(tab);
                    tokens.Add(TextFile.CreateTextToken(guild.GetTitle() //)); DEBUG rep:
                        + " (rep:" + guild.GetReputation(playerEntity).ToString() + ")"));
                    var guildData = guild.GetGuildData();
                    var ts = Mathf.Clamp(28 - (Guild.CalculateDaySinceZero(DaggerfallUnity.Instance.WorldTime.Now) - guildData.lastRankChange), 0, 28);
                    if (ts == 0)
                        tokens.Add(TextFile.CreateTextToken("  Visit Guild for Advancement Evaluation."));
                    else
                        tokens.Add(TextFile.CreateTextToken("  Next Advancement Evaluation in " + ts.ToString() + " days."));

                    tokens.Add(TextFile.NewLineToken);
                }

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(tokens.ToArray(), null, false);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
        }

        protected void ShowRepairs()
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            List<IGuild> guildMemberships = GameManager.Instance.GuildManager.GetMemberships();

            var otherCount = GameManager.Instance.PlayerEntity.OtherItems.Count;

            if (otherCount <= 0)
            {
                tokens.Add(TextFile.CreateTextToken($"No Pending Repairs"));
                tokens.Add(TextFile.NewLineToken);
            }
            else
            {
                foreach (DaggerfallUnityItem otherItem in GameManager.Instance.PlayerEntity.OtherItems.items.Values)
                {
                    string pattern = @"MapID=(\d+),\s*BuildingKey=(\d+)";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(otherItem.RepairData.GetSaveData().sceneName);
                    var mapIDString = match.Groups[1].Value;

                    var buildingKeyString = match.Groups[2].Value;
                    int mapID, buildingKey;

                    if (!int.TryParse(mapIDString, out mapID))
                        return;
                    if (!int.TryParse(buildingKeyString, out buildingKey))
                        return;

                    int mapIndex = -1;

                    var regionId = GetRegionAndMapIndex(mapID, out mapIndex);
                    if (regionId < 0 || mapIndex < 0)
                        return;
                    string regionName = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionName(regionId);

                    var townName = GetTownName(regionId, mapIndex);
                    if (townName == string.Empty)
                        return;

                    var buildingName = GetBuildingName(regionId, mapIndex, buildingKey);

                    if(otherItem.RepairData.DaysUntilRepaired() > 1)
                    {
                        tokens.Add(TextFile.CreateTextToken(
                            $"My {otherItem.LongName} is being repaired at {buildingName} in {townName} "));
                        tokens.Add(TextFile.NewLineToken);
                        tokens.Add(TextFile.CreateTextToken($"     in {regionName} region.  It should be ready in {otherItem.RepairData.DaysUntilRepaired()} days."));
                        tokens.Add(TextFile.NewLineToken);
                        tokens.Add(TextFile.NewLineToken);
                    }
                    else if(otherItem.RepairData.DaysUntilRepaired() == 1)
                    {
                        tokens.Add(TextFile.CreateTextToken(
                            $"My {otherItem.LongName} is being repaired at {buildingName} in {townName} "));
                        tokens.Add(TextFile.NewLineToken);
                        tokens.Add(TextFile.CreateTextToken($"     in {regionName} region.  It should be ready tomorrow."));
                        tokens.Add(TextFile.NewLineToken);
                        tokens.Add(TextFile.NewLineToken);
                    }
                    else
                    {
                        tokens.Add(TextFile.CreateTextToken(
                            $"My {otherItem.LongName} is being repaired at {buildingName} in {townName} "));
                        tokens.Add(TextFile.NewLineToken);
                        tokens.Add(TextFile.CreateTextToken($"     in {regionName} region.  It should be ready."));
                        tokens.Add(TextFile.NewLineToken);
                        tokens.Add(TextFile.NewLineToken);
                    }

                }

            }

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.EnableVerticalScrolling(Screen.height /2);
            messageBox.SetTextTokens(tokens.ToArray(), null, false);
            messageBox.ClickAnywhereToClose = true;

            DaggerfallMessageBox houseMsg = ShowHouses(messageBox);
            if (houseMsg != null)
                messageBox.AddNextMessageBox(houseMsg);

            messageBox.Show();
        }

        DaggerfallMessageBox ShowHouses(DaggerfallMessageBox msgBox)

        {
            DaggerfallMessageBox houseMsg = new DaggerfallMessageBox(uiManager, msgBox);
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            TextFile.Token tab = TextFile.TabToken;
            tab.x = 60;
            int n = 0;
            tokens.Add(new TextFile.Token(TextFile.Formatting.JustifyCenter, null));
            tokens.Add(TextFile.CreateFormatTextToken($"Homes Owned", TextFile.Formatting.TextHighlight));
            tokens.Add(new TextFile.Token(TextFile.Formatting.JustifyCenter, null));
            tokens.Add(TextFile.NewLineToken);
            tokens.Add(new TextFile.Token()
            {
                text = "Region",
                formatting = TextFile.Formatting.TextHighlight
            }); 
            tokens.Add(tab);
            tokens.Add(new TextFile.Token()
            {
                text = "Town",
                formatting = TextFile.Formatting.TextHighlight
            });
            tokens.Add(TextFile.NewLineToken);
            foreach (var home in DaggerfallWorkshop.Game.Banking.DaggerfallBankManager.Houses)
            {
                if (!string.IsNullOrEmpty(home.location))
                {
                    n++;
                    tokens.Add(TextFile.CreateTextToken(DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionName(home.regionIndex)));
                    tokens.Add(tab);
                    tokens.Add(TextFile.CreateTextToken(home.location));

                    tokens.Add(TextFile.NewLineToken);
                }
            }

            if (n == 0)
            {
                tokens.Clear();
                tokens.Add(TextFile.CreateTextToken($"No Homes Owned"));
                tokens.Add(TextFile.NewLineToken);
            }

            houseMsg.SetTextTokens(tokens.ToArray());
            houseMsg.EnableVerticalScrolling(Screen.height / 2);
            houseMsg.SetTextTokens(tokens.ToArray(), null, false);
            houseMsg.ClickAnywhereToClose = true;

            return houseMsg;

        }
        protected void ShowReputations()
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            List<IGuild> guildMemberships = GameManager.Instance.GuildManager.GetMemberships();
            TextFile.Token tab = TextFile.TabToken;
            tab.x = 40;

            tokens.Add(new TextFile.Token()
            {
                text = "Class",
                formatting = TextFile.Formatting.TextHighlight
            }); ;
            tokens.Add(tab);
            tokens.Add(new TextFile.Token()
            {
                text = "Reputation",
                formatting = TextFile.Formatting.TextHighlight
            }); ;
            tokens.Add(TextFile.NewLineToken);

            for (int n = 0; n < 5; n++)
            {
                var rep = GameManager.Instance.PlayerEntity.SGroupReputations[n];
                string repString = string.Empty;
                if (rep < -80)
                    repString = "Hated";
                else if (rep < -60)
                    repString = "Pond Scum";
                else if (rep < -40)
                    repString = "Villain";
                else if (rep < -20)
                    repString = "Criminal";
                else if (rep < 0)
                    repString = "Undependable";
                else if (rep == 0)
                    repString = "Common Citizen";
                else if (rep > 80)
                    repString = "Revered";
                else if (rep > 60)
                    repString = "Esteemed";
                else if (rep > 40)
                    repString = "Honored";
                else if (rep > 20)
                    repString = "Admired";
                else if (rep > 10)
                    repString = "Respected";
                else
                    repString = "Dependable";

                tokens.Add(TextFile.CreateTextToken($"{((FactionFile.SocialGroups)n).ToString()}"));
                tokens.Add(tab);
                tokens.Add(TextFile.CreateTextToken($"{repString}"));
                tokens.Add(TextFile.NewLineToken);
            }

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetTextTokens(tokens.ToArray(), null, false);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
        }

        public int CurrentTallyCount(DFCareer.Skills skill)
        {
            int num = (int)skill;
            int num2 = 65536 - (((int)(playerEntity.Reflexes) - 2) << 13);
            return playerEntity.SkillUses[num] * num2 >> 16;
        }

        public int TallysNeededToAdvance(DFCareer.Skills skill)
        {
            int num = (int)skill;
            int advancementMultiplier = DaggerfallSkills.GetAdvancementMultiplier((DFCareer.Skills)num);
            float advancementMultiplier2 = ((DaggerfallEntity)playerEntity).Career.AdvancementMultiplier;
            return FormulaHelper.CalculateSkillUsesForAdvancement((int)((DaggerfallEntity)playerEntity).Skills.GetPermanentSkillValue(num), advancementMultiplier, advancementMultiplier2, ((DaggerfallEntity)playerEntity).Level);
        }

        private TextFile.Token[] CreateSkillTokens(DFCareer.Skills skill, bool twoColumn = false, int startPosition = 0)
        {
            bool skillRecentlyIncreased = playerEntity.GetSkillRecentlyIncreased(skill);
            int num = CurrentTallyCount(skill);
            int num2 = TallysNeededToAdvance(skill);
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            TextFile.Formatting formatting = (skillRecentlyIncreased ?  TextFile.Formatting.TextHighlight : TextFile.Formatting.Text);
            TextFile.Token item = default(TextFile.Token);
            item.formatting = formatting;
            item.text = DaggerfallUnity.Instance.TextProvider.GetSkillName(skill);
            TextFile.Token item2 = default(TextFile.Token);
            item2.formatting = formatting;

            if (playerEntity.Skills.GetPermanentSkillValue((int)skill) >= FormulaHelper.MaxStatValue())
                item.text = "MASTERED";
            else if (playerEntity.AlreadyMasteredASkill() && playerEntity.Skills.GetPermanentSkillValue((int)skill) >= FormulaHelper.MaxStatValue()*0.95)
                item.text = "Maxed";
            else
            {
                if (RegisterCharSheetWindow.showPct)
                {
                    var pct = num * 100 / num2;
                    if (pct >= 100)
                        item2.text = "Ready";
                    else
                        item2.text = $"{pct}%";
                }
                else
                {
                    if (num >= num2)
                        item2.text = "Ready";
                    else
                        item2.text = string.Format("{0} / {1}", num, num2);
                }
            }
            TextFile.Token item3 = default(TextFile.Token);
            item3.formatting = formatting;
            item3.text = $"{((DaggerfallEntity)playerEntity).Skills.GetLiveSkillValue(skill)}%";
            var primaryStat = DaggerfallSkills.GetPrimaryStat(skill);
            var item4 = default(TextFile.Token);
            item4.formatting = formatting;
            item4.text = DaggerfallUnity.Instance.TextProvider.GetAbbreviatedStatName(primaryStat);
            var item5 = default(TextFile.Token);
            item5.formatting = TextFile.Formatting.PositionPrefix;
            var item6 = default(TextFile.Token);
            item6.formatting = TextFile.Formatting.PositionPrefix;
            if (!twoColumn)
            {
                tokens.Add(item);
                tokens.Add(item6);
                tokens.Add(item6);
                tokens.Add(item2);
                tokens.Add(item6);
                tokens.Add(item3);
                tokens.Add(item6);
                tokens.Add(item4);
            }
            else
            {
                if (startPosition != 0)
                {
                    item5.x = startPosition;
                    tokens.Add(item5);
                }
                tokens.Add(item);
                item5.x = startPosition + 55;
                tokens.Add(item5);
                tokens.Add(item2);
                item5.x = startPosition + 95;
                tokens.Add(item5);
                tokens.Add(item3);
                item5.x = startPosition + 112;
                tokens.Add(item5);
                tokens.Add(item4);
            }
            return tokens.ToArray();
        }

    }
}