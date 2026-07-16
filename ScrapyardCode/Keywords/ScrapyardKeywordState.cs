using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Scrapyard.Energy;

namespace Scrapyard.Keywords;

public static class ScrapyardKeywordState
{
    private static readonly Dictionary<Player, int> CardsPlayedThisTurn = new();
    private static readonly Dictionary<Player, int> TrackedTurnNumbers = new();
    private static readonly Dictionary<Player, int> PreviousCardCosts = new();
    private static readonly Dictionary<Player, List<int>> PlayedCardCosts = new();
    private static readonly Dictionary<CardModel, bool> WasInitiativeCard = new();
    private static readonly Dictionary<CardModel, bool> WasFollowUpCard = new();
    private static readonly Dictionary<CardModel, bool> WasDecisiveCard = new();

    public static void ResetTurn(Player player)
    {
        TrackedTurnNumbers[player] = player.PlayerCombatState?.TurnNumber ?? 0;
        CardsPlayedThisTurn[player] = 0;
        PlayedCardCosts[player] = [];
        PreviousCardCosts.Remove(player);
        WasInitiativeCard.Clear();
        WasFollowUpCard.Clear();
        WasDecisiveCard.Clear();
    }

    public static void RefreshTurnState(Player player)
    {
        var turnNumber = player.PlayerCombatState?.TurnNumber ?? 0;
        if (TrackedTurnNumbers.TryGetValue(player, out var trackedTurnNumber) && trackedTurnNumber == turnNumber)
        {
            return;
        }

        ResetTurn(player);
    }

    public static void RecordPlayedCard(Player player, CardModel card, int cost, int finalEnergy)
    {
        RefreshTurnState(player);

        WasInitiativeCard[card] = CardsPlayedThisTurn.GetValueOrDefault(player) == 0;
        WasFollowUpCard[card] = IsFollowUpPending(card);
        WasDecisiveCard[card] = card is IScrapyardDecisiveCard && finalEnergy == 1;
        CardsPlayedThisTurn[player] = CardsPlayedThisTurn.GetValueOrDefault(player) + 1;
        PlayedCardCosts.GetValueOrDefault(player)?.Add(cost);
        PreviousCardCosts[player] = cost;
    }

    public static IReadOnlyList<int> GetPlayedCardCostsThisTurn(Player player)
    {
        RefreshTurnState(player);
        return PlayedCardCosts.GetValueOrDefault(player) ?? [];
    }

    public static bool WasInitiativeTriggered(CardModel card)
    {
        if (card.IsCanonical)
        {
            return false;
        }

        if (card.Owner is not null)
        {
            RefreshTurnState(card.Owner);
        }

        return WasInitiativeCard.TryGetValue(card, out var wasInitiative) && wasInitiative;
    }

    public static bool IsInitiativePending(CardModel card)
    {
        if (card.IsCanonical)
        {
            return false;
        }

        var owner = card.Owner;
        if (owner is null)
        {
            return false;
        }

        RefreshTurnState(owner);
        return CardsPlayedThisTurn.GetValueOrDefault(owner) == 0;
    }

    public static bool WasFollowUpTriggered(CardModel card)
    {
        if (card.IsCanonical)
        {
            return false;
        }

        if (card.Owner is not null)
        {
            RefreshTurnState(card.Owner);
        }

        return WasFollowUpCard.TryGetValue(card, out var wasFollowUp) && wasFollowUp;
    }

    public static bool IsFollowUpPending(CardModel card)
    {
        if (card.IsCanonical)
        {
            return false;
        }

        var owner = card.Owner;
        if (card is not IScrapyardFollowUpCard || owner is null)
        {
            return false;
        }

        RefreshTurnState(owner);
        if (!PreviousCardCosts.TryGetValue(owner, out var previousCost))
        {
            return false;
        }

        return AreCostsMultiples(ScrapyardEnergySystem.GetEnergyCostToSpend(card), previousCost);
    }

    public static bool WasDecisiveTriggered(CardModel card)
    {
        if (card.IsCanonical)
        {
            return false;
        }

        if (card.Owner is not null)
        {
            RefreshTurnState(card.Owner);
        }

        return WasDecisiveCard.TryGetValue(card, out var wasDecisive) && wasDecisive;
    }

    public static bool IsDecisivePending(CardModel card)
    {
        if (card.IsCanonical)
        {
            return false;
        }

        if (card is not IScrapyardDecisiveCard || card.Owner is not { } owner)
        {
            return false;
        }

        RefreshTurnState(owner);
        var cost = ScrapyardEnergySystem.GetEnergyCostToSpend(card);
        return ScrapyardEnergySystem.CanSpend(owner, cost)
            && ScrapyardEnergySystem.PreviewEnergyAfterSpend(owner, cost) == 1;
    }

    private static bool AreCostsMultiples(int currentCost, int previousCost)
    {
        var current = Math.Abs(currentCost);
        var previous = Math.Abs(previousCost);

        if (current == 0 || previous == 0)
        {
            return false;
        }

        return current % previous == 0 || previous % current == 0;
    }
}
