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
                    text = "affiliation",
                    formatting = TextFile.Formatting.TextHighlight
                }); ;
                tokens.Add(tab);
                tokens.Add(new TextFile.Token()
                {
                    text = TextManager.Instance.GetLocalizedText("rank"),
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

    }
}