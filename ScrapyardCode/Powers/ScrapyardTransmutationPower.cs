using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardTransmutationPower : ModPowerTemplate
{
    private const int NewMaxEnergy = 36;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardTransmutationPower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardTransmutationPowerBig.png"
    );

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return NewMaxEnergy;
    }
}
