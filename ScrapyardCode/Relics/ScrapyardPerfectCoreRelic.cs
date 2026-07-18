using Scrapyard.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.Relics;

[RegisterRelic(typeof(ScrapyardRelicPool))]
public sealed class ScrapyardPerfectCoreRelic : ScrapyardElectricCoreRelic
{
    protected override int TriggerCount => 5;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png");
}
