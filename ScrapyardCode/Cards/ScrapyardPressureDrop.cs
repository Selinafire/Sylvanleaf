using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Scrapyard.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Scrapyard.Cards;

[RegisterCard(typeof(ScrapyardCardPool))]
public sealed class ScrapyardPressureDrop : ScrapyardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StrengthLoss", 4m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(CardKeyword.Exhaust);

    public ScrapyardPressureDrop() : base(2, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        if (cardPlay.Target.GetPowerAmount<StrengthPower>() <= 0)
        {
            return;
        }

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            cardPlay.Target,
            -DynamicVars["StrengthLoss"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthLoss"].UpgradeValueBy(2m);
    }
}
