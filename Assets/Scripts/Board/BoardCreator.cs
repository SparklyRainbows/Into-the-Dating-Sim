using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private GameObject tilePrefab;

    public Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();

    private List<Tile> forecast = new List<Tile>();

    private Point[] dirs = new Point[4] {
        new Point(0, 1),
        new Point(0, -1),
        new Point(1, 0),
        new Point(-1, 0)
    };

    private Color pathColor = Color.blue;
    private Color selectedTileColor = new Color32(0, 107, 87, 255);
    private Color enemyTileColor = Color.red;
    private Color defaultTileColor = Color.white;

    #region Creating/Destroying
    public void CreateBoard(TerrainUnitInfo info) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Point p = new Point(i, j);
                Tile tile = Instantiate(tilePrefab, transform).GetComponent<Tile>();

                tile.GetComponent<UnitStats>().SetTerrainInfo(info);
                tile.GetComponent<TerrainUnit>().Place(tile);

                tile.SetPosition(p);
                tiles.Add(p, tile);
            }
        }
    }

    public void SetTile(Point point, TerrainUnitInfo info) {
        Tile t = GetTile(point);

        if (t == null) {
            t = Instantiate(tilePrefab, transform).GetComponent<Tile>();
            t.SetPosition(point);
            tiles.Add(point, t);
        }

        t.GetComponent<UnitStats>().SetTerrainInfo(info);
        t.GetComponent<TerrainUnit>().Place(t);
        t.GetComponent<TerrainUnit>().SetSortingLayer();
    }

    public TerrainUnitInfo SetTile(Point point, int tile) {
        TerrainUnitInfo info = null;

        switch (tile) {
            case 0:
                info = GameInformation.instance.GetTerrainInfo(Terrain.GRASS);
                break;
            case 4:
                info = GameInformation.instance.GetTerrainInfo(Terrain.RESTAURANT);
                break;
            default:
                Debug.LogWarning("Map generation tile not set up: " + tile);
                break;

        }

        if (info == null) {
            info = GameInformation.instance.GetTerrainInfo(Terrain.GRASS);
        }
        SetTile(point, info);

        return info;
    }

    public void DestroyBoard() {
        foreach (Tile tile in tiles.Values) {
            Destroy(tile.gameObject);
        }

        tiles = new Dictionary<Point, Tile>();
    }
    #endregion

    #region Find path between two tiles
    public void SelectTilesBetween(Tile start, Tile end, Func<Tile, bool> CanTraverse) {
        List<Tile> tiles = FindTilesBetween(start, end, CanTraverse);

        for (int i = tiles.Count - 1; i >= 0; --i) {
            if (tiles[i] != null && !forecast.Contains(tiles[i])) {
                 tiles[i].GetComponent<SpriteRenderer>().color = pathColor;
            }
        }
    }

    private List<Tile> FindTilesBetween(Tile start, Tile end, Func<Tile, bool> CanTraverse) {
        ClearSearch();
        List<Tile> open = new List<Tile>();
        List<Tile> closed = new List<Tile>();

        start.distance = 0;
        open.Add(start);

        while (open.Count > 0) {
            Tile t = GetTileWithLowestDist(open);
            closed.Add(t);
            open.Remove(t);

            if (t == end) {
                break;
            }

            for (int i = 0; i < 4; ++i) {
                Tile next = GetTile(t.pos + dirs[i]);

                if (next == null || closed.Contains(next) || !CanTraverse(next))
                    continue;

                next.distance = t.distance + 1;

                if (!open.Contains(next)) {
                    next.prev = t;
                    open.Insert(0, next);
                } else {
                    if (open[open.IndexOf(next)].distance > next.distance) {
                        open[open.IndexOf(next)].distance = next.distance;
                    }
                }
            }
        }

        List<Tile> path = new List<Tile>();

        Tile curr = end;
        while (curr != start) {
            path.Add(curr);
            curr = curr.prev;
        }
        path.Add(start);

        return path;
    }

    private Tile GetTileWithLowestDist(List<Tile> tiles) {
        Tile t = null;
        int min = int.MaxValue;

        for (int i = 0; i < tiles.Count; i++) {
            if (tiles[i].distance < min) {
                min = tiles[i].distance;
                t = tiles[i];
            }
        }

        return t;
    }
    #endregion
    
    public List<Tile> Search(Tile start, Func<Tile, Tile, bool> addTile) {
        List<Tile> retValue = new List<Tile>();
        retValue.Add(start);

        ClearSearch();
        Queue<Tile> checkNext = new Queue<Tile>();
        Queue<Tile> checkNow = new Queue<Tile>();

        start.distance = 0;
        checkNow.Enqueue(start);

        while (checkNow.Count > 0) {
            Tile t = checkNow.Dequeue();

            for (int i = 0; i < 4; ++i) {
                Tile next = GetTile(t.pos + dirs[i]);

                if (next == null || next.distance <= t.distance + 1)
                    continue;

                if (addTile(t, next)) {
                    next.distance = t.distance + 1;
                    next.prev = t;
                    checkNext.Enqueue(next);
                    retValue.Add(next);
                }
            }

            if (checkNow.Count == 0)
                SwapReference(ref checkNow, ref checkNext);
        }

        return retValue;
    }

    public List<Tile> Search(Tile start, Func<Tile, Tile, int, bool> addTile, int range) {
        List<Tile> retValue = new List<Tile>();
        retValue.Add(start);

        ClearSearch();
        Queue<Tile> checkNext = new Queue<Tile>();
        Queue<Tile> checkNow = new Queue<Tile>();

        start.distance = 0;
        checkNow.Enqueue(start);

        while (checkNow.Count > 0) {
            Tile t = checkNow.Dequeue();

            for (int i = 0; i < 4; ++i) {
                Tile next = GetTile(t.pos + dirs[i]);

                if (next == null || next.distance <= t.distance + 1)
                    continue;

                if (addTile(t, next, range)) {
                    next.distance = t.distance + 1;
                    next.prev = t;
                    checkNext.Enqueue(next);
                    retValue.Add(next);
                }
            }

            if (checkNow.Count == 0)
                SwapReference(ref checkNow, ref checkNext);
        }

        return retValue;
    }

    public void SelectTiles(List<Tile> tiles, UnitType type = UnitType.PLAYER) {
        Color c = new Color();
        if (type == UnitType.PLAYER) {
            c = selectedTileColor;
        } else if (type == UnitType.ENEMY) {
            c = enemyTileColor;
        }


        for (int i = tiles.Count - 1; i >= 0; --i) {
            if (tiles[i] != null && !forecast.Contains(tiles[i])) {
                if (!tiles[i].isEnemyForecast) {
                    tiles[i].GetComponent<SpriteRenderer>().color = c;
                }
            }
        }
    }

    public void DeSelectTiles(List<Tile> tiles) {
        for (int i = tiles.Count - 1; i >= 0; --i) {
            if (tiles[i] != null && !forecast.Contains(tiles[i])) {
                if (tiles[i].isEnemyForecast) {
                    tiles[i].GetComponent<SpriteRenderer>().color = enemyTileColor;
                } else {
                    tiles[i].GetComponent<SpriteRenderer>().color = defaultTileColor;
                }
            }
        }
    }

    public Tile GetTile(Point p) {
        return tiles.ContainsKey(p) ? tiles[p] : null;
    }

    private void SwapReference(ref Queue<Tile> a, ref Queue<Tile> b) {
        Queue<Tile> temp = a;
        a = b;
        b = temp;
    }

    private void ClearSearch() {
        foreach (Tile t in tiles.Values) {
            t.prev = null;
            t.distance = int.MaxValue;
        }
    }

    #region Forecast
    public void DamageForecast(Tile tile, int damage) {
        if (tile == null) {
            return;
        }
        tile.GetComponent<Unit>().ShowDamageForecast(damage);
    }

    public void HideDamageForecast(List<Tile> tiles) {
        foreach (Tile t in tiles) {
            t.GetComponent<Unit>().HideDamageForecast();
        }
    }

    public void Forecast(Tile tile) {
        if (tile == null) {
            return;
        }

        forecast.Add(tile);
        tile.GetComponent<SpriteRenderer>().color = enemyTileColor;
        tile.isEnemyForecast = true;
    }

    public void DeselectForecastWithoutColor(List<Tile> tiles) {
        if (tiles == null) {
            return;
        }

        foreach (Tile t in tiles) {
            forecast.Remove(t);

            if (!forecast.Contains(t) && t != null) {
                t.GetComponent<Unit>().HideDamageForecast();
            }
        }
    }

    public void DeselectForecast(List<Tile> tiles) {
        if (tiles == null) {
            return;
        }

        foreach (Tile t in tiles) {
            forecast.Remove(t);

            if (!forecast.Contains(t)) {
                t.GetComponent<SpriteRenderer>().color = defaultTileColor;
                t.GetComponent<Unit>().HideDamageForecast();
                t.isEnemyForecast = false;
            }
        }
    }

    public void ClearForecast() {
        foreach (Tile t in forecast) {
            t.GetComponent<SpriteRenderer>().color = defaultTileColor;
            t.GetComponent<Unit>().HideDamageForecast();
            t.isEnemyForecast = false;
        }

        forecast.Clear();

        foreach (Tile t in tiles.Values) {
            t.isEnemyForecast = false;
            t.GetComponent<SpriteRenderer>().color = defaultTileColor;
        }
    }
    #endregion
}
