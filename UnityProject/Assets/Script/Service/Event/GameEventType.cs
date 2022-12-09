using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saltyfish.Event
{
    /// <summary>
    /// 这个枚举用于拓展已有的事件
    /// </summary>
    public enum GameEventType
    {

        OnGameLaunch,
        OnUnitHpChanged,
        OnUnitRecoverHp,
        OnUnitDamaged,
        OnUnitDead,
        OnUnitAllDelete,
        OnUnitAdd,
        OnUnitDelete,
        OnUnitRespawn,
        OnTeamUnitAdd,
        OnTeamUnitDelete,
        OnTowerUnitAdd,
        OnTowerUnitDelete,
        OnTeamOrTowerUnitDead,

        OnEnemyAliveCountChange,

        OnInteractBegin,
        OnInteractEnd,

        //Skill in battle
        OnPlayerCastSkillCD,
        OnPlayerGetSkill,
        OnPlayerLoseSkill,
        OnControllerChange,

        //Skill in bag
        OnUpgradeSkill,
        OnGetSkillInBag,
        OnLoseSkillInBag,
        OnEquipSkill,
        OnUnEquipSkill,
        OnEquipedSkillReplaced,

        //Recruit
        OnGetShip,
        OnLoseShip,
        OnUpdateShip,
        OnRenameShip,
        OnShipPropertyItemSelect,

        //Placement
        OnShipFollowed,
        OnShipUnFollowed,
        OnFollowedShipReplaced,
        OnShipTowerReplaced,
        OnShipTowerAdd,
        OnShipTowerRemove,
        OnCloseTowerBuilder,

        //Currency
        OnCurrencyChange,

        //Upgradable
        OnPlayerSupportUpgrade,

        OnBasementStoredHpChange,

        //GameEvent
        OnLeaveBattle,
    }
}
