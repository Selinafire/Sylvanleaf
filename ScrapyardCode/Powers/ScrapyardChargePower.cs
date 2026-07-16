using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardChargePower : ModPowerTemplate
{
    private readonly HashSet<AttackCommand> _chargedAttacks = [];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardChargePower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardChargePowerBig.png");

    public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
    {
        if (Amount <= 0
            || hitCount <= 0
            || attack.Attacker != Owner
            || attack.ModelSource is not CardModel { Type: CardType.Attack })
        {
            return hitCount;
        }

        _chargedAttacks.Add(attack);
        Flash();
        return hitCount + 1;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (_chargedAttacks.Remove(command))
        {
            await PowerCmd.Decrement(this);
        }
    }
}
