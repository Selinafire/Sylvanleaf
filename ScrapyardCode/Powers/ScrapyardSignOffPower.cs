using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardSignOffPower : ModPowerTemplate
{
    private PlayerCombatState? _subscribedState;
    private int _pendingDraws;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardSignOffPower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardSignOffPowerBig.png");

    public override Task AfterApplied(Creature? creature, CardModel? card)
    {
        SubscribeToEnergyChanges();
        return Task.CompletedTask;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        UnsubscribeFromEnergyChanges();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || _pendingDraws <= 0)
        {
            return;
        }

        var draws = _pendingDraws;
        _pendingDraws = 0;
        Flash();
        await CardPileCmd.Draw(choiceContext, draws, Owner.Player);
    }

    private void SubscribeToEnergyChanges()
    {
        var state = Owner.Player?.PlayerCombatState;
        if (state is null || ReferenceEquals(_subscribedState, state))
        {
            return;
        }

        UnsubscribeFromEnergyChanges();
        _subscribedState = state;
        _subscribedState.EnergyChanged += OnEnergyChanged;
    }

    private void UnsubscribeFromEnergyChanges()
    {
        if (_subscribedState is null)
        {
            return;
        }

        _subscribedState.EnergyChanged -= OnEnergyChanged;
        _subscribedState = null;
    }

    private void OnEnergyChanged(int oldEnergy, int newEnergy)
    {
        if (Math.Sign(oldEnergy) == Math.Sign(newEnergy) || Owner.IsDead)
        {
            return;
        }

        _pendingDraws++;
    }
}
