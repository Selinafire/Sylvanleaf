using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardElectricPotentialPower: ModPowerTemplate
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;
    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Test/images/powers/test_power.png",
        BigIconPath: "res://Test/images/powers/test_power.png"
    );

    // 敌人回合开始时，玩家与每个敌人逐一比较电势，高的一方对低的一方造成差值伤害
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // 只在敌人回合开始时触发
        if (side != CombatSide.Enemy) return;
        // 每回合只结算一次
        if (_hasTriggeredThisTurn) return;
        _hasTriggeredThisTurn = true;

        // 找到玩家（取第一个活着的）
        var player = combatState.GetCreaturesOnSide(CombatSide.Player).FirstOrDefault(c => c.IsAlive);
        if (player == null) return;

        int playerAmount = player.GetPower<ScrapyardElectricPotentialPower>()?.Amount ?? 0;

        var enemies = combatState.GetCreaturesOnSide(CombatSide.Enemy).Where(e => e.IsAlive).ToList();

        foreach (var enemy in enemies)
        {
            int enemyAmount = enemy.GetPower<ScrapyardElectricPotentialPower>()?.Amount ?? 0;

            if (playerAmount > enemyAmount)
            {
                Flash();
                await CreatureCmd.Damage(
                    new ThrowingPlayerChoiceContext(),
                    new[] { enemy },
                    playerAmount - enemyAmount,
                    ValueProp.Unpowered,
                    player,
                    null,
                    null
                );
            }
            else if (enemyAmount > playerAmount)
            {
                Flash();
                await CreatureCmd.Damage(
                    new ThrowingPlayerChoiceContext(),
                    new[] { player },
                    enemyAmount - playerAmount,
                    ValueProp.Unpowered,
                    enemy,
                    null,
                    null
                );
            }
            // 相等则无事发生
        }
    }

    // 敌人回合结束时重置标记
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
            _hasTriggeredThisTurn = false;
    }

    private static bool _hasTriggeredThisTurn;

}