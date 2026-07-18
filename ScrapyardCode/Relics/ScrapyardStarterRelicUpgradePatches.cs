using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Scrapyard.Relics;

public static class ScrapyardStarterRelicUpgradePatches
{
    [HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
    private static class TouchOfOrobasGetUpgradedStarterRelicPatch
    {
        private static bool Prefix(RelicModel starterRelic, ref RelicModel __result)
        {
            if (starterRelic is not ScrapyardRelic)
            {
                return true;
            }

            __result = ModelDb.Relic<ScrapyardPerfectCoreRelic>();
            return false;
        }
    }
}
