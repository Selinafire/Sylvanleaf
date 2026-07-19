using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Scrapyard.Characters;
using Scrapyard.ScrapyardCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.Cards;

[RegisterCard(typeof(ScrapyardCardPool))]
public sealed class ScrapyardEvolution : ScrapyardCard
{
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/ScrapyardHeavyDefend.png");

    public ScrapyardEvolution() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var extraDraws = IsUpgraded ? 2 : 1;
        await PowerCmd.Apply<ScrapyardEvolutionPower>(
            choiceContext,
            Owner.Creature,
            extraDraws,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // Handled in OnPlay via IsUpgraded check
    }
}
