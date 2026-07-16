using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Scrapyard.Characters;
using Scrapyard.ScrapyardCode.ElectricPotential;
using Scrapyard.ScrapyardCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Scrapyard.Cards;

[RegisterCard(typeof(ScrapyardCardPool))]
public sealed class ScrapyardPollution : ScrapyardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Charge", 2m),
        new DynamicVar("Electric", 3m)
    ];

    public ScrapyardPollution() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ScrapyardChargePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Charge"].BaseValue,
            Owner.Creature,
            this);

        await ScrapyardElectricPotentialHelper.ApplyDelta(
            choiceContext,
            [Owner.Creature],
            -DynamicVars["Electric"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Charge"].UpgradeValueBy(1m);
    }
}
