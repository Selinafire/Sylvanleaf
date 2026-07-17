using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Scrapyard.Energy;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardRareEnergyPower : ModPowerTemplate
{
    private const int Multiplier = 35;
    private bool _armed;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardRareEnergyPower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardRareEnergyPowerBig.png");

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
        {
            return;
        }

        if (Owner.Player?.PlayerCombatState?.Energy > 1)
        {
            _armed = true;
            return;
        }

        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!_armed || !participants.Contains(Owner) || Owner.Player is not { } player)
        {
            return;
        }

        for (var i = 0; i < Amount; i++)
        {
            ScrapyardEnergySystem.GainMultiplicative(player, Multiplier);
        }

        await PowerCmd.Remove(this);
    }
}
