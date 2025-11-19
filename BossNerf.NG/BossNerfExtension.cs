using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Services;

namespace BossNerf.NG;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class BossNerfExtension(
    ModHelper modHelper, 
    DatabaseServer databaseServer,
    DatabaseService databaseService,
    ISptLogger<BossNerfExtension> logger) : IOnLoad
{
    public Task OnLoad()
    {
        var tables = databaseServer.GetTables();
        logger.LogWithColor("[BossNerfNG] The mod has been uploaded successfully.", LogTextColor.Green);

        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var bossesConfig = modHelper.GetJsonDataFromFile<BossConfig[]>(pathToMod, "config.json");

        foreach (var bossConfig in bossesConfig)
        {
            var boss = tables.Bots.Types[bossConfig.BossName];
            if (boss == null)
            {
                logger.Error($"[BossNerfNG] Boss {bossConfig.BossName} not found");
                continue;
            }

            AdjustedBotHealth(boss, bossConfig.BossAdjustment);

            foreach (var followerName in bossConfig.Followers)
            {
                var follower = tables.Bots.Types[followerName];
                if (follower == null)
                {
                    logger.Error($"[BossNerfNG] Follower {follower} not found");
                    continue;
                }
                
                AdjustedBotHealth(follower, bossConfig.FollowerAdjustment);
            }
        }
        
        return Task.CompletedTask;
    }

    private void AdjustedBotHealth(BotType bot, int adjustmentValue)
    {
        var playerHealth = databaseService
            .GetGlobals()
            .Configuration
            .Health
            .ProfileHealthSettings
            .BodyPartsSettings;

        foreach (var bodyPart in bot.BotHealth.BodyParts)
        {
            bodyPart.Head.Min = playerHealth.Head.Minimum + adjustmentValue;
            bodyPart.Head.Max = playerHealth.Head.Maximum + adjustmentValue;
            bodyPart.Chest.Min = playerHealth.Chest.Minimum + adjustmentValue;
            bodyPart.Chest.Max = playerHealth.Chest.Maximum + adjustmentValue;
            bodyPart.Stomach.Min = playerHealth.Stomach.Minimum + adjustmentValue;
            bodyPart.Stomach.Max = playerHealth.Stomach.Maximum + adjustmentValue;
            bodyPart.LeftArm.Min = playerHealth.LeftArm.Minimum + adjustmentValue;
            bodyPart.LeftArm.Max = playerHealth.LeftArm.Maximum + adjustmentValue;
            bodyPart.RightArm.Min = playerHealth.RightArm.Minimum + adjustmentValue;
            bodyPart.RightArm.Max = playerHealth.RightArm.Maximum + adjustmentValue;
            bodyPart.LeftLeg.Min = playerHealth.LeftLeg.Minimum + adjustmentValue;
            bodyPart.LeftLeg.Max = playerHealth.LeftLeg.Maximum + adjustmentValue;
            bodyPart.RightLeg.Min = playerHealth.RightLeg.Minimum + adjustmentValue;
            bodyPart.RightLeg.Max = playerHealth.RightLeg.Maximum + adjustmentValue;
        }
    }
}