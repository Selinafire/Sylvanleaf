using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Scrapyard.Characters;

namespace Scrapyard.Relics;

public static class ScrapyardRelicMaxEnergyPatches
{
    private static readonly Type[] ModifyMaxEnergySignature =
    [
        typeof(Player),
        typeof(decimal)
    ];

    [HarmonyPatch]
    private static class RelicModifyMaxEnergyPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            return typeof(RelicModel).Assembly
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(RelicModel)))
                .Select(type => AccessTools.DeclaredMethod(type, nameof(RelicModel.ModifyMaxEnergy), ModifyMaxEnergySignature))
                .Where(method => method is not null)!;
        }

        private static void Postfix(Player player, decimal amount, ref decimal __result)
        {
            if (player.Character is not ScrapyardCharacter)
            {
                return;
            }

            var addedMaxEnergy = (int)(__result - amount);
            if (addedMaxEnergy <= 0 || __result != amount + addedMaxEnergy)
            {
                return;
            }

            __result = amount * MaxEnergyIncreaseMultiplier(addedMaxEnergy);
        }
    }

    private static int MaxEnergyIncreaseMultiplier(int addedMaxEnergy)
    {
        return addedMaxEnergy switch
        {
            1 => 2,
            2 => 6,
            3 => 12,
            4 => 36,
            _ => 36
        };
    }
}
