using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Scrapyard.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Scrapyard.Cards;

[RegisterCard(typeof(ScrapyardCardPool))]
public sealed class ScrapyardFiveElements : ScrapyardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(CardKeyword.Exhaust);

    public ScrapyardFiveElements() : base(5, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(5)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
