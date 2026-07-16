using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Scrapyard.Energy;

namespace Scrapyard.Ui;

public static class ScrapyardCardCostPatches
{
    [HarmonyPatch]
    private static class NCardUpdateEnergyCostVisualsPatch
    {
        private static readonly FieldInfo EnergyIconField = AccessTools.Field(typeof(NCard), "_energyIcon");
        private static readonly FieldInfo EnergyLabelField = AccessTools.Field(typeof(NCard), "_energyLabel");

        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(NCard),
                "UpdateEnergyCostVisuals",
                [typeof(PileType)]);
        }

        private static void Postfix(NCard __instance)
        {
            var model = __instance.Model;
            if (model is null || !ScrapyardEnergySystem.IsScrapyardCard(model) || model.EnergyCost.CostsX)
            {
                return;
            }

            var cost = ScrapyardEnergySystem.GetEnergyCostToSpend(model);
            if (cost >= 0)
            {
                return;
            }

            ((MegaLabel)EnergyLabelField.GetValue(__instance)!).SetTextAutoSize(cost.ToString());
            ((TextureRect)EnergyIconField.GetValue(__instance)!).Visible = true;
        }
    }
}
