using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Scrapyard.Energy;
using Scrapyard.Keywords;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardDynamicCompilePower : ModPowerTemplate
{
    private int _initiativeDraw;
    private int _followUpBlock;
    private int _decisiveEnergy;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardDynamicCompilePower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardDynamicCompilePowerBig.png");

    public void AddCompileInstance(bool upgraded)
    {
        _initiativeDraw += upgraded ? 2 : 1;
        _followUpBlock += upgraded ? 7 : 5;
        _decisiveEnergy += 1;
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
        {
            return;
        }

        if (ScrapyardKeywordState.WasInitiativeTriggered(cardPlay.Card))
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, _initiativeDraw > 0 ? _initiativeDraw : Amount, Owner.Player);
        }

        if (ScrapyardKeywordState.WasFollowUpTriggered(cardPlay.Card))
        {
            Flash();
            await CreatureCmd.GainBlock(
                Owner,
                _followUpBlock > 0 ? _followUpBlock : 5m * Amount,
                ValueProp.Move,
                cardPlay);
        }

        if (ScrapyardKeywordState.WasDecisiveTriggered(cardPlay.Card))
        {
            Flash();
            ScrapyardEnergySystem.GainAdditive(Owner.Player, _decisiveEnergy > 0 ? _decisiveEnergy : Amount);
        }
    }
}
