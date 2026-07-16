using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using Scrapyard.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.Cards;

public abstract class ScrapyardCard : ModCardTemplate
{
    protected ScrapyardCard(
        int cost,
        CardType cardType,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true)
        : base(cost, cardType, rarity, targetType, showInCardLibrary)
    {
    }

#pragma warning disable RITSU013 // Runtime card subclasses resolve this to their concrete class name.
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");
#pragma warning restore RITSU013

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            if (this is IScrapyardInitiativeCard)
            {
                yield return ScrapyardCardKeywords.Initiative;
            }

            if (this is IScrapyardFollowUpCard)
            {
                yield return ScrapyardCardKeywords.FollowUp;
            }

            if (this is IScrapyardDecisiveCard)
            {
                yield return ScrapyardCardKeywords.Decisive;
            }
        }
    }

    protected override bool ShouldGlowGoldInternal =>
        this is IScrapyardInitiativeCard && ScrapyardKeywordState.IsInitiativePending(this)
        || this is IScrapyardFollowUpCard && ScrapyardKeywordState.IsFollowUpPending(this)
        || this is IScrapyardDecisiveCard && ScrapyardKeywordState.IsDecisivePending(this);
}
