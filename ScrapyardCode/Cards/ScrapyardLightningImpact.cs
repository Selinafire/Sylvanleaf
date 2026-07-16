using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Scrapyard.Characters;
using Scrapyard.ScrapyardCode.ElectricPotential;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Scrapyard.Cards;

[RegisterCard(typeof(ScrapyardCardPool))]
public sealed class ScrapyardLightningImpact : ScrapyardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedDraw").WithMultiplier((CardModel card, Creature? _) =>
            ScrapyardElectricPotentialHelper.CountChargedUnits(card.CombatState))
    ];

    public ScrapyardLightningImpact() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var drawCount = ((CalculatedVar)DynamicVars["CalculatedDraw"]).Calculate(cardPlay.Target);
        if (drawCount > 0)
        {
            await CardPileCmd.Draw(choiceContext, drawCount, Owner);
        }
    }
}
