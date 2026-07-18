using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardArcLightPower : ModPowerTemplate
{
    private PlayerCombatState? _subscribedState;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardArcLightPower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardArcLightPowerBig.png");

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
        if (newEnergy % 2 != 0 || Owner.IsDead)
        {
            return;
        }

        TaskHelper.RunSafely(GainBlock());
    }

    private async Task GainBlock()
    {
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}
