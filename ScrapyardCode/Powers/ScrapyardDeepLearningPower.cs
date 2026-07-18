using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardDeepLearningPower : ModPowerTemplate
{
    private bool _generatedSlimedRetain;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardDeepLearningPower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardDeepLearningPowerBig.png");

    public void EnableRetainedSlimed()
    {
        _generatedSlimedRetain = true;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner.Player)
        {
            return;
        }

        Flash();

        for (var i = 0; i < Amount; i++)
        {
            var slimed = combatState.CreateCard<Slimed>(player);
            if (_generatedSlimedRetain)
            {
                CardCmd.ApplyKeyword(slimed, CardKeyword.Retain);
            }

            await CardPileCmd.AddGeneratedCardToCombat(slimed, PileType.Hand, player);
        }
    }
}
