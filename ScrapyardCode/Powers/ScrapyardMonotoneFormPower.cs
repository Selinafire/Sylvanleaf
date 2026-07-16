using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Scrapyard.Energy;
using Scrapyard.Keywords;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardMonotoneFormPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardMonotoneFormPower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardMonotoneFormPowerBig.png");

    public override Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
        {
            return Task.CompletedTask;
        }

        var costs = ScrapyardKeywordState.GetPlayedCardCostsThisTurn(cardPlay.Card.Owner);
        if (costs.Count == 0 || !IsStrictlyMonotone(costs))
        {
            return Task.CompletedTask;
        }

        Flash();
        ScrapyardEnergySystem.GainMultiplicative(cardPlay.Card.Owner, costs[^1]);
        return Task.CompletedTask;
    }

    private static bool IsStrictlyMonotone(IReadOnlyList<int> costs)
    {
        if (costs.Count <= 1)
        {
            return true;
        }

        var direction = Math.Sign(costs[1] - costs[0]);
        if (direction == 0)
        {
            return false;
        }

        for (var i = 2; i < costs.Count; i++)
        {
            if (Math.Sign(costs[i] - costs[i - 1]) != direction)
            {
                return false;
            }
        }

        return true;
    }
}
