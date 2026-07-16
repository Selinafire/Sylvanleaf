using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Keywords;

namespace Scrapyard.Keywords;

public static class ScrapyardCardKeywords
{
    public const string InitiativeStem = "INITIATIVE";
    public const string FollowUpStem = "FOLLOW_UP";
    public const string DecisiveStem = "DECISIVE";

    public const string InitiativeId = $"{Entry.ModId}_KEYWORD_{InitiativeStem}";
    public const string FollowUpId = $"{Entry.ModId}_KEYWORD_{FollowUpStem}";
    public const string DecisiveId = $"{Entry.ModId}_KEYWORD_{DecisiveStem}";

    public static CardKeyword Initiative => InitiativeId.GetModCardKeyword();
    public static CardKeyword FollowUp => FollowUpId.GetModCardKeyword();
    public static CardKeyword Decisive => DecisiveId.GetModCardKeyword();
}
