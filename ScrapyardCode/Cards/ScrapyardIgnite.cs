using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Scrapyard.Characters;
using Scrapyard.Energy;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.Cards;

[RegisterCard(typeof(ScrapyardCardPool))]
[RegisterCharacterStarterCard(typeof(ScrapyardCharacter), 1)]
[RegisterOwnedCardKeyword(InitiativeKeywordStem)]
public sealed class ScrapyardIgnite : ModCardTemplate
{
    private const string InitiativeKeywordStem = "INITIATIVE";
    private const string InitiativeKeywordId = $"{Entry.ModId}_KEYWORD_{InitiativeKeywordStem}";
    private const int BaseEnergyCost = 4;
    private const CardType CardKind = CardType.Attack;
    private const CardRarity CardRarityValue = CardRarity.Basic;
    private const TargetType CardTarget = TargetType.AnyEnemy;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/ScrapyardStrike.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move),
        new CardsVar(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [InitiativeKeywordId.GetModCardKeyword()];

    public ScrapyardIgnite() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
    {
    }

    protected override bool ShouldGlowGoldInternal => ScrapyardEnergySystem.IsFirstCardPendingThisTurn(this);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        if (ScrapyardEnergySystem.WasFirstPlayedCardThisTurn(this))
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
