using MegaCrit.Sts2.Core.Entities.Relics;
using Scrapyard.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.Relics;

// RegisterRelic 会把遗物注册进指定遗物池。
// RegisterCharacterStarterRelic 会把它作为 ScrapyardCharacter 的初始遗物。
[RegisterRelic(typeof(ScrapyardRelicPool))]
[RegisterCharacterStarterRelic(typeof(ScrapyardCharacter))]
public sealed class ScrapyardRelic : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png");
}
