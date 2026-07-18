using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Scrapyard.Cards;

namespace Scrapyard.Mechanics;

public static class ScrapyardClawMachineTwoState
{
    private static readonly Dictionary<Player, CombatBonus> CombatBonuses = new();
    private static readonly Dictionary<CardModel, int> AppliedBonuses = new();

    public static void ApplyCurrentBonus(ScrapyardClawMachineTwo card)
    {
        var owner = card.Owner;
        if (owner?.PlayerCombatState is null)
        {
            return;
        }

        var bonus = GetBonus(owner);
        var alreadyApplied = AppliedBonuses.GetValueOrDefault(card);
        var delta = bonus - alreadyApplied;
        if (delta <= 0)
        {
            return;
        }

        card.DynamicVars.Damage.BaseValue += delta;
        card.DynamicVars["Hits"].BaseValue += delta;
        AppliedBonuses[card] = bonus;
    }

    public static void Increase(Player player)
    {
        var bonus = GetBonus(player) + 1;
        CombatBonuses[player] = new CombatBonus(player.PlayerCombatState, bonus);

        foreach (var card in CardPile.GetCards(
                     player,
                     PileType.Draw,
                     PileType.Hand,
                     PileType.Discard,
                     PileType.Exhaust,
                     PileType.Play).OfType<ScrapyardClawMachineTwo>())
        {
            ApplyCurrentBonus(card);
        }
    }

    private static int GetBonus(Player player)
    {
        var combatState = player.PlayerCombatState;
        if (!CombatBonuses.TryGetValue(player, out var bonus) || !ReferenceEquals(bonus.CombatState, combatState))
        {
            CombatBonuses[player] = new CombatBonus(combatState, 0);
            return 0;
        }

        return bonus.Amount;
    }

    private sealed record CombatBonus(object? CombatState, int Amount);
}
