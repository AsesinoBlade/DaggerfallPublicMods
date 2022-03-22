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
using DaggerfallWorkshop.Game.Guilds;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class AsesinoCharSheetWindow : DaggerfallCharacterSheetWindow
    {
        private const int noAffiliationsMsgId = 19;
        PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

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
        }

        private void classsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowReputations();
        }

        protected override void ShowSkillsDialog(List<DFCareer.Skills> skills, bool twoColumn = false)
        {
            bool secondColumn = false;
            bool showHandToHandDamage = false;
            List<TextFile.Token> tokens = new List<TextFile.Token>();
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


        protected  void ShowReputations()
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

            if (playerEntity.Skills.GetPermanentSkillValue((int)skill) >= 100)
                item.text = "MASTERED";
            else if (playerEntity.AlreadyMasteredASkill() && playerEntity.Skills.GetPermanentSkillValue((int)skill) >= 95)
                item.text = "Maxed";
            else
                item2.text = $"{num} / {num2}";
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