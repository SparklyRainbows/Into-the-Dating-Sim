using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    private UnitInfo info;
    private EnemyUnitInfo _enemyInfo;
    private PlayerUnitInfo _playerInfo;
    private TerrainUnitInfo _terrainInfo;

    private List<StatusEffects> statuses;
    
    public int health;
    private Healthbar healthbar;

    private void Start() {
        statuses = new List<StatusEffects>();
        healthbar = GetComponentInChildren<Healthbar>();
    }

    #region Info
    private void SetInfo(UnitInfo info) {
        this.info = info;

        health = info.maxHealth;
        gameObject.name = info.name;
        GetComponent<SpriteRenderer>().sprite = info.unitSprite;

        MovementAndAttackMapping.SetMovement(info.movementType, gameObject);
        MovementAndAttackMapping.SetAttack(info.WeaponMain, gameObject);
        MovementAndAttackMapping.SetAttack(info.WeaponSecondary, gameObject);

        GetComponent<Unit>().InitAnimator(info);
    }

    public void SetPlayerInfo(PlayerUnitInfo playerInfo) {
        _playerInfo = playerInfo;
        SetInfo(_playerInfo);

        List<ItemData> combos = GameInformation.instance.GetComboInfo(playerInfo.id);
        foreach (ItemData combo in combos) {
            MovementAndAttackMapping.SetAttack(combo, gameObject);
        }
    }

    public void SetEnemyInfo(EnemyUnitInfo enemyInfo) {
        _enemyInfo = enemyInfo;
        _enemyInfo.InitWeapons();

        SetInfo(_enemyInfo);
    }

    public void SetTerrainInfo(TerrainUnitInfo terrainInfo) {
        _terrainInfo = terrainInfo;
        info = terrainInfo;

        health = info.maxHealth;
        gameObject.name = info.name;
        GetComponent<SpriteRenderer>().sprite = info.unitSprite;

        if (terrainInfo.unitSprites.Length != health && terrainInfo.IsDestructible()) {
            Debug.LogError(terrainInfo.name + "'s unit sprites are not set up correctly.");
        }
    }
    #endregion

    #region Relationships
    private RelationshipRank GetNearbyRelationship() {
        PlayerUnit playerUnit = GetComponent<PlayerUnit>();
        if (playerUnit == null) {
            return RelationshipRank.NONE;
        }

        List<PlayerUnit> nearbyUnits = playerUnit.GetNearbyUnits();

        if (nearbyUnits.Count == 0) {
            return RelationshipRank.NONE;
        }

        RelationshipRank rank = RelationshipRank.NONE;
        foreach(PlayerUnit unit in nearbyUnits) {
            RelationshipRank currentRank = RelationshipManager.GetRelationshipBetween(ID, unit.GetComponent<UnitStats>().ID).GetRank();
            if (currentRank > rank) {
                rank = currentRank;
            }
        }

        return rank;
    }

    public void IncreaseSupport() {
        PlayerUnit playerUnit = GetComponent<PlayerUnit>();
        if (playerUnit == null) {
            return;
        }

        List<PlayerUnit> nearbyUnits = playerUnit.GetNearbyUnits();
        foreach (PlayerUnit unit in nearbyUnits) {
            RelationshipManager.GetRelationshipBetween(ID, unit.GetComponent<UnitStats>().ID).IncreaseSupport();
            unit.PlayHeart();
        }

        if (nearbyUnits.Count > 0) {
            GetComponent<PlayerUnit>().PlayHeart();
        }
    }
    #endregion

    #region Getters
    public int GetAttack(BoardCreator board, ItemData weapon) {
        RelationshipRank rank = GetNearbyRelationship();

        if (rank == RelationshipRank.NONE) {
            return weapon.Damage;
        }

        return weapon.Damage + Relationship.GetAttackBonus(rank);
    }

    public int Move {
        get {
            return info.baseMove;
        }
    }

    public UnitID ID {
        get {
            return info.id;
        }
    }

    public UnitInfo unitInfo {
        get {
            return info;
        }
    }

    public PlayerUnitInfo playerUnitInfo {
        get {
            return _playerInfo;
        }
    }

    public EnemyUnitInfo enemyUnitInfo {
        get {
            return _enemyInfo;
        }
    }

    public TerrainUnitInfo terrainUnitInfo {
        get {
            return _terrainInfo;
        }
    }
    #endregion

    #region Health
    public void HealToFull() {
        health = info.maxHealth;

        if (healthbar == null && _terrainInfo != null) {
            return;
        }

        healthbar.SetHealth(health, info.maxHealth);
    }

    public void TakeDamage(int damage) {
        if (_terrainInfo != null && !_terrainInfo.IsDestructible()) {
            return;
        }

        if (HasStatus(StatusEffects.FREEZE)) {
            RemoveStatus(StatusEffects.FREEZE);
            return;
        }

        string audioStr = info.hurtSFX;
        if (audioStr != null && audioStr.Length > 0) {
            GameInformation.instance.audio.Play(audioStr);
        }

        health -= damage;

        if (health <= 0) {
            health = 0;
        }

        if (_terrainInfo != null) {
            GetComponent<TerrainUnit>().ChangeSprite(health, _terrainInfo);
        }

        if (healthbar == null && _terrainInfo != null) {
            return;
        }

        healthbar.SetHealth(health, info.maxHealth);

        if (!IsDead()) {
            GetComponent<Unit>().TakeDamage();
        }
    }

    public void Heal(int amount) {
        if (healthbar == null && _terrainInfo != null) {
            Debug.LogWarning("Trying to heal terrain.");
            return;
        }

        health += amount;

        if (health > info.maxHealth) {
            health = info.maxHealth;
        }

        healthbar.SetHealth(health, info.maxHealth);
    }

    public bool IsDead() {
        return health <= 0;
    }
    #endregion

    #region Knockback
    public IEnumerator ApplyKnockback(BoardCreator board, Point dir) {
        Point newPoint = GetComponent<Unit>().tile.pos - dir;
        Tile newTile = board.GetTile(newPoint);

        yield return StartCoroutine(Knockback(newTile));

        if (newTile == null) {

        } else if (WillCollideWithObject(newTile)) {
            TakeDamage(1);
            newTile.content.GetComponent<UnitStats>().TakeDamage(1);
        } else {
            Unit unit = GetComponent<Unit>();
            unit.Place(newTile);
            unit.Match();

            if (unit.type == UnitType.ENEMY) {
                GetComponent<EnemyUnit>().MoveAttackTarget(board, dir);
            }
        }
    }

    private bool WillCollideWithObject(Tile tile) {
        Unit u = tile.content.GetComponent<Unit>();

        if (u.type != UnitType.TERRAIN) {
            return true;
        }

        TerrainUnitInfo info = u.GetComponent<UnitStats>().terrainUnitInfo;

        if (!info.IsPassable(u.GetComponent<UnitStats>().HasStatus(StatusEffects.FREEZE),
            !u.GetComponent<UnitStats>().IsDead())) {
            return true;
        }

        return false;
    }

    private IEnumerator Knockback(Tile target) {
        if (target != null) {
            Vector2 targetPos = target.GetPosition();
            Vector2 startPos = transform.position;

            if (target.content != null) {
                targetPos = (target.GetPosition() - startPos) * 2 / 3 + startPos;

                while ((Vector2)transform.position != targetPos) {
                    transform.position = Vector2.MoveTowards(transform.position, targetPos, 0.2f);
                    yield return null;
                }
                while ((Vector2)transform.position != startPos) {
                    transform.position = Vector2.MoveTowards(transform.position, startPos, 0.2f);
                    yield return null;
                }
            } else {
                while ((Vector2)transform.position != targetPos) {
                    transform.position = Vector2.MoveTowards(transform.position, targetPos, 0.2f);
                    yield return null;
                }
            }
        }
    }
    #endregion

    #region Status
    public void AddStatus(StatusEffects status) {
        if (!statuses.Contains(status)) {
            statuses.Add(status);

            if (_terrainInfo != null) {
                if (_terrainInfo.terrain == Terrain.RIVER && status == StatusEffects.BURN) {
                    GetComponent<TerrainUnit>().SetRiverSprite(false, terrainUnitInfo);
                    statuses.Remove(status);
                    statuses.Remove(StatusEffects.FREEZE);
                    return;
                } else if (!_terrainInfo.CanApplyStatus(status)) {
                    statuses.Remove(status);
                    return;
                }
            }

            if (_terrainInfo != null && _terrainInfo.terrain == Terrain.RIVER && status == StatusEffects.FREEZE) {
                GetComponent<TerrainUnit>().SetRiverSprite(true, terrainUnitInfo);
            } else {
                GetComponent<Unit>().PlayStatus(status);
            }

            if (status == StatusEffects.FREEZE) {
                RemoveStatus(StatusEffects.BURN);
            }
            if (status == StatusEffects.BURN) {
                RemoveStatus(StatusEffects.FREEZE);
            }
        }
    }

    public void ApplyStatuses() {
        foreach (StatusEffects status in statuses) {
            ApplyStatus(status);
        }
    }

    private void ApplyStatus(StatusEffects status) {
        switch (status) {
            case StatusEffects.BURN:
                GameInformation.instance.audio.Play("Burn");
                TakeDamage(1);
                return;
            case StatusEffects.REGEN:
                Heal(1);
                return;
            case StatusEffects.FREEZE:
                return;
            default:
                Debug.LogError("Status not found: " + status);
                return;
        }
    }

    public bool HasStatus(StatusEffects status) {
        if (statuses == null || statuses.Count == 0) {
            return false;
        }

        return statuses.Contains(status);
    }

    private void RemoveStatus(StatusEffects status) {
        if (statuses.Contains(status)) {
            if (_terrainInfo != null && _terrainInfo.terrain == Terrain.RIVER && status == StatusEffects.FREEZE) {
                GetComponent<TerrainUnit>().SetRiverSprite(false, terrainUnitInfo);
            } else {
                GetComponent<Unit>().StopPlayingStatus(status);
            }
            
            statuses.Remove(status);

            if (status == StatusEffects.FREEZE) {
                GameInformation.instance.audio.Play("Ice");
            }
        }
    }
    #endregion

    public bool ConductsElectricity() {
        if (playerUnitInfo != null || enemyUnitInfo != null) {
            return true;
        }

        if (terrainUnitInfo != null) {
            return terrainUnitInfo.ConductsElectricity();
        }

        return false;
    }
}
