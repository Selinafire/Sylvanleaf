using STS2RitsuLib.Interop.AutoRegistration;

namespace Scrapyard.Keywords;

[RegisterOwnedCardKeyword(ScrapyardCardKeywords.InitiativeStem)]
internal sealed class ScrapyardInitiativeKeywordRegistration
{
}

[RegisterOwnedCardKeyword(ScrapyardCardKeywords.FollowUpStem)]
internal sealed class ScrapyardFollowUpKeywordRegistration
{
}

[RegisterOwnedCardKeyword(ScrapyardCardKeywords.DecisiveStem)]
internal sealed class ScrapyardDecisiveKeywordRegistration
{
}
