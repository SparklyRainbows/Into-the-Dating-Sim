using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBattleState : BattleState {
    private MapGenerator generator;

    private List<Point> unitTiles = new List<Point>();
    private List<Point> enemyTiles = new List<Point>();

    public override void Enter() {
        base.Enter();

        generator = board.GetComponent<MapGenerator>();
        StartCoroutine(Init());
    }

    public override void Exit() {
        base.Exit();

        unitTiles.Clear();
        enemyTiles.Clear();
    }

    IEnumerator Init() {
        CreateBoard();

        Point p = new Point(0, 0);
        SelectTile(p);

        SpawnPlayers();
        SpawnEnemies();

        owner.battleUI.StartBattle();

        tileSelectionIndicator.GetComponent<Cursor>().Show();
        tileSelectionIndicator.GetComponent<Cursor>().canMove = true;
        cameraRig.gameObject.SetActive(true);

        yield return null;

        if (GameInformation.instance.viewedTutorial) {
            owner.ChangeState<PlayerTurnStartState>();
        } else {
            owner.ChangeState<TutorialState>();
        }
    }

    private void CreateBoard() {
        int[,] levelMap = generator.GenerateLevel();
        TerrainUnitInfo[,] terrainMap = generator.GenerateLevel(levelMap);

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                Point p = new Point(i, j);
                TerrainUnitInfo info = terrainMap[i, j];

                board.SetTile(p, info);

                if (info.terrain == Terrain.RESTAURANT) {
                    owner.restaurantUnits.Add(board.GetTile(p).GetComponent<TerrainUnit>());
                }

                int terrainInt = levelMap[i, j];
                if (terrainInt == 5) {
                    enemyTiles.Add(p);
                }
                if (terrainInt == 6) {
                    unitTiles.Add(p);
                }
            }
        }

        battleUI.SetHealthText(owner.restaurantUnits.Count);
    }

    private void SpawnPlayers() {
        for (int i = 0; i < 3; i++) {
            GameObject instance = Instantiate(owner.playerUnitPrefab);

            instance.GetComponent<UnitStats>().SetPlayerInfo(GameInformation.instance.playerInfo[i]);

            int index = Random.Range(0, unitTiles.Count);
            Point p = unitTiles[index];
            unitTiles.RemoveAt(index);

            PlayerUnit unit = instance.GetComponent<PlayerUnit>();
            unit.Place(board.GetTile(p));

            unit.SetBoard(board);

            owner.playerUnits.Add(unit);
            unit.Match();
        }
    }

    private void SpawnEnemies() {
        int difficulty = GameInformation.instance.GetLevelDifficulty();

        int currentDifficulty = 0;
        int numOfEnemies = 0;
        while (currentDifficulty < difficulty && numOfEnemies < 6) {
            GameObject enemy = Instantiate(owner.enemyPrefab);

            int index = Random.Range(0, enemyTiles.Count);
            Point p = enemyTiles[index];
            enemyTiles.RemoveAt(index);

            EnemyUnit enemyUnit = enemy.GetComponent<EnemyUnit>();
            enemyUnit.Place(board.GetTile(p));

            EnemyUnitInfo enemyInfo = GameInformation.instance.GetRandomEnemy();
            enemyUnit.GetComponent<UnitStats>().SetEnemyInfo(enemyInfo);

            owner.enemyUnits.Add(enemyUnit);
            enemyUnit.Match();

            currentDifficulty += enemyInfo.difficulty;
            numOfEnemies++;
        }
    }
}
