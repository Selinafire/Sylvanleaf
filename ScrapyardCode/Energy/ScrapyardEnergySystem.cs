using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Scrapyard.Characters;

namespace Scrapyard.Energy;

public static class ScrapyardEnergySystem
{
    public const int StartingEnergy = 12;

    private static readonly Dictionary<Player, ScrapyardEnergySpendMode> SpendModes = new();
    private static readonly Dictionary<Player, int> CardsPlayedThisTurn = new();
    private static readonly Dictionary<Player, int> TrackedTurnNumbers = new();
    private static readonly Dictionary<CardModel, bool> WasFirstCardThisTurn = new();

    public static bool IsScrapyardCard(CardModel card)
    {
        return card.Owner?.Character is ScrapyardCharacter;
    }

    public static ScrapyardEnergySpendMode GetSpendMode(Player player)
    {
        return SpendModes.GetValueOrDefault(player, ScrapyardEnergySpendMode.Divide);
    }

    public static void SetSpendMode(Player player, ScrapyardEnergySpendMode mode)
    {
        SpendModes[player] = mode;
    }

    public static void ResetSpendMode(Player player)
    {
        SpendModes[player] = ScrapyardEnergySpendMode.Divide;
        CardsPlayedThisTurn[player] = 0;
        TrackedTurnNumbers[player] = player.PlayerCombatState?.TurnNumber ?? 0;
    }

    public static bool CanSpend(Player player, int cost)
    {
        RefreshTurnState(player);

        if (cost == 0)
        {
            return false;
        }

        var combatState = player.PlayerCombatState;
        if (combatState is null)
        {
            return false;
        }

        var energy = combatState.Energy;
        return GetSpendMode(player) switch
        {
            ScrapyardEnergySpendMode.Divide => energy % cost == 0,
            ScrapyardEnergySpendMode.Subtract => cost > 0 && energy >= cost,
            _ => false
        };
    }

    public static int PreviewEnergyAfterSpend(Player player, int cost)
    {
        RefreshTurnState(player);

        if (cost == 0)
        {
            return player.PlayerCombatState?.Energy ?? 0;
        }

        var combatState = player.PlayerCombatState;
        if (combatState is null)
        {
            return 0;
        }

        var energy = combatState.Energy;
        return GetSpendMode(player) switch
        {
            ScrapyardEnergySpendMode.Divide => energy / cost,
            ScrapyardEnergySpendMode.Subtract => energy - cost,
            _ => energy
        };
    }

    public static int Spend(Player player, CardModel card, int cost)
    {
        RefreshTurnState(player);

        var combatState = player.PlayerCombatState;
        if (combatState is null)
        {
            return 0;
        }

        var before = combatState.Energy;
        var after = PreviewEnergyAfterSpend(player, cost);
        WasFirstCardThisTurn[card] = CardsPlayedThisTurn.GetValueOrDefault(player) == 0;
        CardsPlayedThisTurn[player] = CardsPlayedThisTurn.GetValueOrDefault(player) + 1;
        combatState.Energy = after;
        return Math.Max(0, before - after);
    }

    public static bool WasFirstPlayedCardThisTurn(CardModel card)
    {
        if (card.Owner is not null)
        {
            RefreshTurnState(card.Owner);
        }

        if (WasFirstCardThisTurn.TryGetValue(card, out var wasFirst))
        {
            return wasFirst;
        }

        var owner = card.Owner;
        return owner is not null && CardsPlayedThisTurn.GetValueOrDefault(owner) == 0;
    }

    public static bool IsFirstCardPendingThisTurn(CardModel card)
    {
        var owner = card.Owner;
        if (owner is null)
        {
            return false;
        }

        RefreshTurnState(owner);
        return CardsPlayedThisTurn.GetValueOrDefault(owner) == 0;
    }

    public static void GainAdditive(Player player, int amount)
    {
        RefreshTurnState(player);

        var combatState = player.PlayerCombatState;
        if (combatState is not null)
        {
            combatState.Energy += amount;
        }
    }

    public static void GainMultiplicative(Player player, int multiplier)
    {
        RefreshTurnState(player);

        var combatState = player.PlayerCombatState;
        if (combatState is not null)
        {
            combatState.Energy *= multiplier;
        }
    }

    private static void RefreshTurnState(Player player)
    {
        var turnNumber = player.PlayerCombatState?.TurnNumber ?? 0;
        if (TrackedTurnNumbers.TryGetValue(player, out var trackedTurnNumber) && trackedTurnNumber == turnNumber)
        {
            return;
        }

        TrackedTurnNumbers[player] = turnNumber;
        CardsPlayedThisTurn[player] = 0;
        WasFirstCardThisTurn.Clear();
    }
}
