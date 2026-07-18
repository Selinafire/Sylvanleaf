using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardInterferencePower : ModPowerTemplate
{
    private readonly List<CardModel> _capturedCards = [];
    private CardModel? _sourceCard;
    private bool _armed;
    private bool _replaying;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardInterferencePower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardInterferencePowerBig.png");

    public void SetSourceCard(CardModel card)
    {
        _sourceCard = card;
    }

    public override Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_armed
            || _replaying
            || _capturedCards.Count >= Amount
            || cardPlay.Card.Owner != Owner.Player
            || ReferenceEquals(cardPlay.Card, _sourceCard))
        {
            return Task.CompletedTask;
        }

        Flash();
        _capturedCards.Add(cardPlay.Card);
        if (_capturedCards.Count >= Amount)
        {
            _armed = true;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
        {
            return;
        }

        if (_capturedCards.Count == 0)
        {
            await PowerCmd.Remove(this);
            return;
        }

        _armed = true;
    }

    public override async Task BeforeHandDraw(
        MegaCrit.Sts2.Core.Entities.Players.Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (!_armed || player != Owner.Player)
        {
            return;
        }

        _replaying = true;
        foreach (var card in _capturedCards.ToList())
        {
            if (card.Owner == player && card.CombatState == combatState)
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }

        _replaying = false;
        await PowerCmd.Remove(this);
    }
}
