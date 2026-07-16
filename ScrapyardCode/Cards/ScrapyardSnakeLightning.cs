using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Scrapyard.Characters;
using Scrapyard.ScrapyardCode.ElectricPotential;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Scrapyard.Cards;

[RegisterCard(typeof(ScrapyardCardPool))]
public sealed class ScrapyardSnakeLightning : ScrapyardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Electric", 7m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(CardKeyword.Retain);

    public ScrapyardSnakeLightning() : base(6, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await ScrapyardElectricPotentialHelper.ApplyDelta(
            choiceContext,
            cardPlay.Target,
            -DynamicVars["Electric"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Electric"].UpgradeValueBy(3m);
    }
}
