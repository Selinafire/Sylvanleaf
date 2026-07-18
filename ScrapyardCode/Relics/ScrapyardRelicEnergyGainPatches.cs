using System.Diagnostics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Scrapyard.Characters;

namespace Scrapyard.Relics;

public static class ScrapyardRelicEnergyGainPatches
{
    [HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.GainEnergy))]
    private static class PlayerCmdGainEnergyPatch
    {
        private static void Prefix(Player player, out RelicEnergyGainState __state)
        {
            __state = new RelicEnergyGainState(
                player?.Character is ScrapyardCharacter && IsCalledFromRelic(),
                player?.PlayerCombatState?.Energy ?? 0);
        }

        private static void Postfix(Player player, RelicEnergyGainState __state)
        {
            if (!__state.ShouldConvert || player.PlayerCombatState is null)
            {
                return;
            }

            var current = player.PlayerCombatState.Energy;
            var gained = current - __state.EnergyBefore;
            if (gained <= 0)
            {
                return;
            }

            player.PlayerCombatState.Energy = __state.EnergyBefore * EnergyGainMultiplier(gained);
        }
    }

    private static bool IsCalledFromRelic()
    {
        foreach (var frame in new StackTrace().GetFrames() ?? [])
        {
            if (IsRelicType(frame.GetMethod()?.DeclaringType))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsRelicType(Type? type)
    {
        while (type is not null)
        {
            if (type.IsSubclassOf(typeof(RelicModel)))
            {
                return true;
            }

            type = type.DeclaringType;
        }

        return false;
    }

    private static int EnergyGainMultiplier(int amount)
    {
        return amount switch
        {
            1 => 2,
            2 => 6,
            3 => 12,
            4 => 36,
            _ => 36
        };
    }

    private readonly record struct RelicEnergyGainState(bool ShouldConvert, int EnergyBefore);
}
