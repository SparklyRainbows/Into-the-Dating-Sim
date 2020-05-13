using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleController : StateMachine {
    public RestController restController;

    public BattleUI battleUI;
    public CameraRig cameraRig;
    public BoardCreator board;
    public Transform tileSelectionIndicator;
    public Point pos;

    public GameObject playerUnitPrefab;
    public GameObject enemyPrefab;

    private PlayerUnit _currentUnit;
    public PlayerUnit currentUnit { get { return _currentUnit; }
        set {
            if (_currentUnit != null) {
                battleUI.Deselect(currentUnit.gameObject.GetComponent<UnitStats>().playerUnitInfo);
            }

            _currentUnit = value;

            if (_currentUnit != null) {
                battleUI.Select(currentUnit.gameObject.GetComponent<UnitStats>().playerUnitInfo);
            }
        }
    }
    public PlayerUnit otherUnit;

    public Tile currentTile { get { return board.GetTile(pos); } }
    public AttackPattern attackPattern;

    public List<PlayerUnit> playerUnits = new List<PlayerUnit>();
    public List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
    public List<TerrainUnit> restaurantUnits = new List<TerrainUnit>();

    public bool talked;

    void Start() {
        GameInformation.instance.PlayBattle();
        ChangeState<InitBattleState>();
    }

    public void ResetAll() {
        playerUnits.Clear();
        enemyUnits.Clear();
        restaurantUnits.Clear();

        _currentUnit = null;
    }

    public void EndBattle(bool won) {
        if (won) {
            ChangeState<EndBattleState>();
        } else {
            GameInformation.instance.SetBGMVolume(0);
            SceneManager.LoadScene("GameOver");
        }
    }

    public int GetRestaurantHealth() {
        int health = 0;

        foreach (TerrainUnit unit in restaurantUnits) {
            if (!unit.GetComponent<UnitStats>().IsDead()) {
                health++;
            }
        }

        return health;
    }

    public bool RestaurantsDestroyed() {
        foreach (TerrainUnit unit in restaurantUnits) {
            if (!unit.GetComponent<UnitStats>().IsDead()) {
                return false;
            }
        }

        return true;
    }

    public void ResetTurn() {
        ChangeState<PlayerTurnStartState>();
    }
}