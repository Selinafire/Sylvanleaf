using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Scrapyard.Cards;
using Scrapyard.Characters;
using Scrapyard.Keywords;

namespace Scrapyard.Energy;

public static class ScrapyardEnergySystem
{
    public const int StartingEnergy = 12;

    private static readonly Dictionary<Player, ScrapyardEnergySpendMode> SpendModes = new();

    public static bool IsScrapyardCard(CardModel card)
    {
        if (card is ScrapyardCard)
        {
            return true;
        }

        return !card.IsCanonical && card.Owner?.Character is ScrapyardCharacter;
    }

    public static int GetEnergyCostToSpend(CardModel card)
    {
        if (card.EnergyCost.CostsX)
        {
            if (card.IsCanonical)
            {
                return 0;
            }

            return card.Owner?.PlayerCombatState?.Energy ?? 0;
        }

        return card.EnergyCost.GetWithModifiers(CostModifiers.All);
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
        ScrapyardKeywordState.ResetTurn(player);
    }

    public static bool CanSpend(Player player, int cost)
    {
        RefreshTurnState(player);

        var combatState = player.PlayerCombatState;
        if (combatState is null)
        {
            return false;
        }

        var energy = combatState.Energy;
        return GetSpendMode(player) switch
        {
            ScrapyardEnergySpendMode.Divide => cost != 0 && energy % cost == 0,
            ScrapyardEnergySpendMode.Subtract => cost >= 0 && energy >= cost,
            _ => false
        };
    }

    public static bool CanSpend(Player player, CardModel card, int cost)
    {
        RefreshTurnState(player);

        if (card.EnergyCost.CostsX)
        {
            return player.PlayerCombatState?.Energy >= 0;
        }

        return CanSpend(player, cost);
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

    public static int PreviewEnergyAfterSpend(Player player, CardModel card, int cost)
    {
        RefreshTurnState(player);

        if (card.EnergyCost.CostsX)
        {
            return 0;
        }

        return PreviewEnergyAfterSpend(player, cost);
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
        var after = PreviewEnergyAfterSpend(player, card, cost);
        if (card.EnergyCost.CostsX)
        {
            card.EnergyCost.CapturedXValue = Math.Max(0, before);
        }

        ScrapyardKeywordState.RecordPlayedCard(player, card, cost, after);
        combatState.Energy = after;
        return Math.Max(0, before - after);
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
        ScrapyardKeywordState.RefreshTurnState(player);
    }
}
