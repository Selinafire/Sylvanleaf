using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Scrapyard.Characters;
using Scrapyard.Keywords;
using Scrapyard.ScrapyardCode.ElectricPotential;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Scrapyard.Cards;

[RegisterCard(typeof(ScrapyardCardPool))]
public sealed class ScrapyardSupernova : ScrapyardCard, IScrapyardDecisiveCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Electric", 3m)
    ];

    public ScrapyardSupernova() : base(4, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = CombatState;
        ArgumentNullException.ThrowIfNull(combatState);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(2)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        if (!ScrapyardKeywordState.WasDecisiveTriggered(this))
        {
            return;
        }

        await ScrapyardElectricPotentialHelper.ApplyDelta(
            choiceContext,
            combatState.HittableEnemies,
            -DynamicVars["Electric"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Electric"].UpgradeValueBy(1m);
    }
}
