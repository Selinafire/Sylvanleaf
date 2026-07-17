using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardSourceOfFlamePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardSourceOfFlamePower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardSourceOfFlamePowerBig.png");

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != Owner.Player)
        {
            return amount;
        }

        for (var i = 0; i < Amount; i++)
        {
            amount *= 2m;
        }

        return amount;
    }
}
