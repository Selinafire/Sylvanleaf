using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Scrapyard.Energy;

public static class ScrapyardEnergyPatches
{
    [HarmonyPatch]
    private static class CardModelCanPlayPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(CardModel),
                nameof(CardModel.CanPlay),
                [typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType()]);
        }

        private static void Postfix(
            CardModel __instance,
            ref bool __result,
            ref UnplayableReason reason,
            ref AbstractModel? preventer)
        {
            if (!ScrapyardEnergySystem.IsScrapyardCard(__instance) || __instance.Owner is null)
            {
                return;
            }

            var cost = ScrapyardEnergySystem.GetEnergyCostToSpend(__instance);
            var canSpend = ScrapyardEnergySystem.CanSpend(__instance.Owner, __instance, cost);

            if (__result)
            {
                if (!canSpend)
                {
                    __result = false;
                    reason = UnplayableReason.EnergyCostTooHigh;
                    preventer = null;
                }

                return;
            }

            if (reason == UnplayableReason.EnergyCostTooHigh && canSpend)
            {
                __result = true;
                reason = UnplayableReason.None;
                preventer = null;
            }
        }
    }

    [HarmonyPatch]
    private static class CardModelSpendResourcesPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(CardModel), nameof(CardModel.SpendResources), []);
        }

        private static bool Prefix(CardModel __instance, ref Task<(int, int)> __result)
        {
            if (!ScrapyardEnergySystem.IsScrapyardCard(__instance) || __instance.Owner is null)
            {
                return true;
            }

            var cost = ScrapyardEnergySystem.GetEnergyCostToSpend(__instance);
            var spent = ScrapyardEnergySystem.Spend(__instance.Owner, __instance, cost);

            __result = Task.FromResult((spent, 0));
            return false;
        }
    }
}
