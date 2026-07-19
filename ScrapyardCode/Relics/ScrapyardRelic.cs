using MegaCrit.Sts2.Core.Entities.Relics;
using Scrapyard.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.Relics;

[RegisterRelic(typeof(ScrapyardRelicPool))]
[RegisterCharacterStarterRelic(typeof(ScrapyardCharacter))]
public sealed class ScrapyardRelic : ScrapyardElectricCoreRelic
{
    protected override decimal ElectricPotentialAmount => 6m;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png");
}
