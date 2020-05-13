using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //KEY:
    //0 = empty map of just grass DONE
    //1 = mountain DONE
    //2 = river DONE
    //3 = forest DONE
    //4 = restaurant DONE
    //5 = enemy spawn point DONE
    //6 = player spawn point DONE
    //Important Points: need 3x4 ish area of grass where enemies spawn
    // 4-8 restaurants per map
    // larger mountains towards edge of map, smaller ones can be put anywhere
    //

    private int numRestaurants = 5;
    private int numForests = 4;
    #region Grids
    public int[,] ChebyshevGrid;

    public int[,] BoolGrid;

    public int[,] LevelMap;
    #endregion

    public int[,] GenerateLevel()
    {
        ChebyshevGrid = GenerateNewChebyshev();
        bool viable = false;
        BoolGrid = GenerateBlankGrid();
        
        LevelMap = GenerateBlankGrid();

        CreateSpawnPoint(LevelMap, BoolGrid);

        CreateSpawnPointPlayers(LevelMap, BoolGrid);

        CreateRestaurants(LevelMap, BoolGrid);

        CreatePond(LevelMap, BoolGrid); //Tries to create one pond not on the edge(2x2 area of water)

        CreateRiver(LevelMap, BoolGrid); //Tries to create an L shaped river (3 blocks in one direction, one block to the side)

        CreateEdgeMountainRange(LevelMap, BoolGrid); //Tries to create one mountain, either dot, 2x1, 2x2, 3x1

        CreateInnerHill(LevelMap, BoolGrid); //Tries to create one 2x2 mountain not on the edge

        CreateForests(LevelMap, BoolGrid); //Tries to create 4 1x1, 2x1, or 1x1x1 L shaped forest

        viable = DoesAPathExist(LevelMap);

        if (viable) return LevelMap;
        else return GenerateLevel();

        #region Print Functions
        /*int rowLength = LevelMap.GetLength(0);
        int colLength = LevelMap.GetLength(1);
        string arrayString = "";
        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < colLength; j++)
            {
                arrayString += string.Format("{0} ", LevelMap[i, j]);
            }
            arrayString += System.Environment.NewLine + System.Environment.NewLine;
        }

        Debug.Log(arrayString);


        int rowLength1 = BoolGrid.GetLength(0);
        int colLength1 = BoolGrid.GetLength(1);
        string arrayString1 = "";
        for (int i = 0; i < rowLength1; i++)
        {
            for (int j = 0; j < colLength1; j++)
            {
                arrayString1 += string.Format("{0} ", BoolGrid[i, j]);
            }
            arrayString1 += System.Environment.NewLine + System.Environment.NewLine;
        }

        Debug.Log(arrayString1);*/

        #endregion


    }

    public void ResetProtocol() {
        LevelMap = GenerateBlankGrid();
        BoolGrid = GenerateBlankGrid();
        CreateSpawnPoint(LevelMap, BoolGrid);
        CreateSpawnPointPlayers(LevelMap, BoolGrid);
        //Debug.LogError("FUCKED UP");
    }

    //RETURNS TRUE OR FALSE DEPENDING ON IF THERE'S A PATH OR NOT
    public bool DoesAPathExist(int[,] levelMap) {
        bool[,] visited = new bool[8, 8];
        bool flag = false;
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (levelMap[i, j] == 5 && !visited[i, j]) {
                    if (PathHelper(levelMap, i, j, visited)) {
                        flag = true;
                        break;
                    }
                }
            }
        }
        return flag;
    }

    public bool PathHelper(int[,] matrix, int i, int j, bool[,] visited) {
        //checking if the coordinate is within the bounds of the box and if they're not mountains, rivers, restaurants and unvisted
        if (isSafe(i, j, matrix) && ((matrix[i, j] != 1) || (matrix[i, j] != 2) || (matrix[i, j] != 4)) && !visited[i,j]) {
            visited[i, j] = true;
            if (matrix[i, j] == 6) {
                return true;
            }
            bool up = PathHelper(matrix, i - 1, j, visited);
            if (up) return true;
            bool left = PathHelper(matrix, i, j - 1, visited);
            if (left) return true;
            bool down = PathHelper(matrix, i + 1, j, visited);
            if (down) return true;
            bool right = PathHelper(matrix, i, j + 1, visited);
            if (right) return true;
        }
        return false;
    }

    public bool isSafe(int i, int j, int[,] matrix) {
        if (i >= 0 && i < 8 && j >= 0 && j < 8) {
            return true;
        }
        return false;
    }

    public void CreateSpawnPointPlayers(int[,] map, int[,] boolGrid)
    {
        bool vertical = (Random.value > 0.5f);
        bool populateDown = (Random.value > 0.5f);
        int startingX = Random.Range(0, 6);
        int startingY = Random.Range(0, 8);

        if (startingX == 6)
        {
            vertical = true;
        }
        if (startingY >= 6)
        {
            populateDown = true;
        }
        if (startingY <= 2)
        {
            populateDown = false;
        }
        if (populateDown)
        {
            if (vertical)
            {
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (boolGrid[startingX + x, startingY - y] == 1) {
                            ResetProtocol();
                            return;
                        }
                        map[startingX + x, startingY - y] = 6;
                        boolGrid[startingX + x, startingY - y] = 1;
                    }
                }
            }
            else
            {
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        if (boolGrid[startingX + x, startingY - y] == 1) {
                            ResetProtocol();
                            return;
                        }
                        map[startingX + x, startingY - y] = 6;
                        boolGrid[startingX + x, startingY - y] = 1;
                    }
                }
            }

        }
        else
        {
            if (vertical)
            {
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (boolGrid[startingX + x, startingY + y] == 1) {
                            ResetProtocol();
                            return;
                        }
                        map[startingX + x, startingY + y] = 6;
                        boolGrid[startingX + x, startingY + y] = 1;
                    }
                }
            }
            else
            {
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        if (boolGrid[startingX + x, startingY + y] == 1)
                        {
                            ResetProtocol();
                            return;
                        }
                        map[startingX + x, startingY + y] = 6;
                        boolGrid[startingX + x, startingY + y] = 1;
                    }
                }
            }
        }
    }

    public void CreateSpawnPoint(int[,] map, int[,] boolGrid) {
        bool vertical = (Random.value > 0.5f);
        bool populateDown = (Random.value > 0.5f);
        int startingX = Random.Range(0, 6);
        int startingY = Random.Range(0, 8);

        if (startingX == 5) {
            vertical = true;
        }
        if (startingY >= 5) {
            populateDown = true;
        }
        if (startingY <= 2) {
            populateDown = false;
        }
        if (populateDown)
        {
            if (vertical)
            {
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        map[startingX + x, startingY - y] = 5;
                        boolGrid[startingX + x, startingY - y] = 1;
                    }
                }
            }
            else
            {
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        map[startingX + x, startingY - y] = 5;
                        boolGrid[startingX + x, startingY - y] = 1;
                    }
                }
            }

        }
        else {
            if (vertical)
            {
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        map[startingX + x, startingY + y] = 5;
                        boolGrid[startingX + x, startingY + y] = 1;
                    }
                }
            }
            else
            {
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        map[startingX + x, startingY + y] = 5;
                        boolGrid[startingX + x, startingY + y] = 1;
                    }
                }
            }
        }
    }

    public void CreateRestaurants(int[,] map, int[,] boolGrid) {
        //size 1 restaurant = 1
        //size 2 restaurant = 1.5
        //size 3 restaurant = 1.75
        float restaurants = 0f;
        int loopNum = 0;
        while (restaurants < numRestaurants || loopNum >= 20) {
            int restaurantSize = Random.Range(1, 4);
            if (restaurantSize == 1)
            {
                Vector2 position = GetRandomViableCoordinate(boolGrid);
                map[(int)position.x, (int)position.y] = 4;
                boolGrid[(int)position.x, (int)position.y] = 1;
                restaurants += 1;
            }
            else if (restaurantSize == 2)
            {
                bool viable = false;
                Vector2 position = GetRandomViableNoEdgeCoordinate(boolGrid);
                int direction = Random.Range(0, 4);

                if (direction == 0)
                { //up 
                    if (boolGrid[(int)position.x, (int)position.y + 1] == 0)
                    {
                        map[(int)position.x, (int)position.y] = 4;
                        map[(int)position.x, (int)position.y + 1] = 4;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y + 1] = 1;
                        viable = true;
                    }
                }
                else if (direction == 1)
                { //down
                    if (boolGrid[(int)position.x, (int)position.y - 1] == 0)
                    {
                        map[(int)position.x, (int)position.y] = 4;
                        map[(int)position.x, (int)position.y - 1] = 4;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y - 1] = 1;
                        viable = true;
                    }
                }
                else if (direction == 2)
                { //left
                    if (boolGrid[(int)position.x - 1, (int)position.y] == 0)
                    {
                        map[(int)position.x, (int)position.y] = 4;
                        map[(int)position.x - 1, (int)position.y] = 4;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x - 1, (int)position.y] = 1;
                        viable = true;
                    }
                }
                else if (direction == 3)
                { //right
                    if (boolGrid[(int)position.x + 1, (int)position.y] == 0)
                    {
                        map[(int)position.x, (int)position.y] = 4;
                        map[(int)position.x + 1, (int)position.y] = 4;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x + 1, (int)position.y] = 1;
                        viable = true;
                    }
                }

                if (viable)
                {
                    restaurants += 1.75f;
                }

            }
            else if (restaurantSize == 3) {
                bool viable = false;
                Vector2 position = GetRandomViableNoEdgeCoordinate(boolGrid);
                int direction = Random.Range(0, 4);

                if (direction == 0)
                { //up + left
                    if ((boolGrid[(int)position.x, (int)position.y + 1] == 0) && (boolGrid[(int)position.x - 1, (int)position.y] == 0))
                    {
                        map[(int)position.x, (int)position.y] = 4;
                        map[(int)position.x, (int)position.y + 1] = 4;
                        map[(int)position.x - 1, (int)position.y] = 4;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y + 1] = 1;
                        boolGrid[(int)position.x - 1, (int)position.y] = 1;
                        viable = true;
                    }
                }
                else if (direction == 1)
                { //left + down
                    if ((boolGrid[(int)position.x, (int)position.y - 1] == 0) && (boolGrid[(int)position.x - 1, (int)position.y] == 0))
                    {
                        map[(int)position.x, (int)position.y] = 4;
                        map[(int)position.x, (int)position.y - 1] = 4;
                        map[(int)position.x - 1, (int)position.y] = 4;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y - 1] = 1;
                        boolGrid[(int)position.x - 1, (int)position.y] = 1;
                        viable = true;
                    }
                }
                else if (direction == 2)
                { //down + right
                    if ((boolGrid[(int)position.x, (int)position.y - 1] == 0) && (boolGrid[(int)position.x + 1, (int)position.y] == 0))
                    {
                        map[(int)position.x, (int)position.y] = 4;
                        map[(int)position.x, (int)position.y - 1] = 4;
                        map[(int)position.x + 1, (int)position.y] = 4;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y - 1] = 1;
                        boolGrid[(int)position.x + 1, (int)position.y] = 1;
                        viable = true;
                    }
                }
                else if (direction == 3)
                { //right + up
                    if ((boolGrid[(int)position.x, (int)position.y + 1] == 0) && (boolGrid[(int)position.x + 1, (int)position.y] == 0))
                    {
                        map[(int)position.x, (int)position.y] = 4;
                        map[(int)position.x, (int)position.y + 1] = 4;
                        map[(int)position.x + 1, (int)position.y] = 4;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y + 1] = 1;
                        boolGrid[(int)position.x + 1, (int)position.y] = 1;
                        viable = true;
                    }
                }

                if (viable)
                {
                    restaurants += 2;
                }
            }

            loopNum += 1;
        }
    }

    public Vector2 GetRandomViableCoordinate(int[,] boolGrid) {
        
        bool unsatisfied = true;
        while (unsatisfied) {
            int x = Random.Range(0, 8);
            int y = Random.Range(0, 8);
            if (boolGrid[x, y] == 0)
            {
                unsatisfied = false;
                Vector2 retVal = new Vector2(x, y);
                return retVal;
            }
        }
        Vector2 defaultRet = new Vector2(0, 0);
        return defaultRet;
    }

    public Vector2 GetRandomViableNoEdgeCoordinate(int[,] boolGrid)
    {
        bool unsatisfied = true;
        while (unsatisfied)
        {
            int x = Random.Range(1, 7);
            int y = Random.Range(1, 7);
            if (boolGrid[x, y] == 0)
            {
                unsatisfied = false;
                Vector2 retVal = new Vector2(x, y);
                return retVal;
            }
        }
        Vector2 defaultRet = new Vector2(0, 0);
        return defaultRet;
    }

    public Vector2 GetRandomViableEdgeCoordinate(int[,] boolGrid) {
        bool unsatisfied = true;
        while (unsatisfied)
        {
            int x;
            int y;
            bool column = (Random.value > 0.5f);
            if (column) {
                bool max = (Random.value > 0.5f);
                if (max)
                {
                    x = 7;
                }
                else {
                    x = 0;
                }
                y = Random.Range(1, 7);
            }
            else {
                bool max = (Random.value > 0.5f);
                if (max)
                {
                    y = 7;
                }
                else
                {
                    y = 0;
                }
                x = Random.Range(1, 7);
            }
            if (boolGrid[x, y] == 0)
            {
                unsatisfied = false;
                Vector2 retVal = new Vector2(x, y);
                return retVal;
            }
        }
        Vector2 defaultRet = new Vector2(0, 0);
        return defaultRet;
    }

    public void CreateEdgeMountainRange(int[,] map, int[,] boolGrid) {
        bool keepLooping = true;
        int iterations = 0;
        while (keepLooping && iterations <= 10) {
            Vector2 point = GetRandomViableEdgeCoordinate(boolGrid);
            int mountainType = Random.Range(0, 4);
            if (mountainType == 0)
            { //dot mountain
                map[(int)point.x, (int)point.y] = 1;
                boolGrid[(int)point.x, (int)point.y] = 1;
                keepLooping = false;
            }
            else if (mountainType == 1)
            { //2x2 mountain range
                if (point.x <= 6 && point.y <= 6)
                {
                    if ((map[(int)point.x, (int)point.y] == 0) && (map[(int)point.x + 1, (int)point.y] == 0) &&
                    (map[(int)point.x, (int)point.y + 1] == 0) && (map[(int)point.x + 1, (int)point.y + 1] == 0)
                    )
                    {
                        map[(int)point.x, (int)point.y] = 1;
                        map[(int)point.x + 1, (int)point.y] = 1;
                        map[(int)point.x, (int)point.y + 1] = 1;
                        map[(int)point.x + 1, (int)point.y + 1] = 1;
                        boolGrid[(int)point.x, (int)point.y] = 1;
                        boolGrid[(int)point.x + 1, (int)point.y] = 1;
                        boolGrid[(int)point.x, (int)point.y + 1] = 1;
                        boolGrid[(int)point.x + 1, (int)point.y + 1] = 1;
                        keepLooping = false;
                    }
                }
            }
            else if (mountainType == 2)
            { //2x1 mountain range
                if (((point.x == 0) || (point.x == 7)))
                { //on the vertical edges (right edge won't have any horizontals)
                    bool vertical = (Random.value > 0.5f);
                    if (vertical && point.y != 7) //vertical edge vertical 2x1
                    {
                        if ((map[(int)point.x, (int)point.y + 1] == 0))
                        {
                            map[(int)point.x, (int)point.y] = 1;
                            map[(int)point.x, (int)point.y + 1] = 1;
                            boolGrid[(int)point.x, (int)point.y] = 1;
                            boolGrid[(int)point.x, (int)point.y + 1] = 1;
                            keepLooping = false;
                        }
                    }
                    else if (point.x != 7)
                    { //vertical edge horizontal 2x1
                        if ((map[(int)point.x + 1, (int)point.y] == 0))
                        {
                            map[(int)point.x, (int)point.y] = 1;
                            map[(int)point.x + 1, (int)point.y] = 1;
                            boolGrid[(int)point.x, (int)point.y] = 1;
                            boolGrid[(int)point.x + 1, (int)point.y] = 1;
                            keepLooping = false;
                        }
                    }
                }
                else
                { //on the horizontal edges (top edge won't have vertials)
                    bool vertical = (Random.value > 0.5f);
                    if (vertical && point.y != 7) //bottom edge vertical 2x1
                    {
                        if ((map[(int)point.x, (int)point.y + 1] == 0))
                        {
                            map[(int)point.x, (int)point.y] = 1;
                            map[(int)point.x, (int)point.y + 1] = 1;
                            boolGrid[(int)point.x, (int)point.y] = 1;
                            boolGrid[(int)point.x, (int)point.y + 1] = 1;
                            keepLooping = false;
                        }
                    }
                    else if (point.x != 7)
                    { //horizontal edge horizontal 2x1
                        if ((map[(int)point.x + 1, (int)point.y] == 0))
                        {
                            map[(int)point.x, (int)point.y] = 1;
                            map[(int)point.x + 1, (int)point.y] = 1;
                            boolGrid[(int)point.x, (int)point.y] = 1;
                            boolGrid[(int)point.x + 1, (int)point.y] = 1;
                            keepLooping = false;
                        }
                    }
                }
            }
            else if (mountainType == 3) { //3x1 mountain range only on the edges
                if ((point.x == 0) || (point.x == 7))
                {
                    if (point.y <= 5)
                    {
                        if ((map[(int)point.x, (int)point.y + 1] == 0) && (map[(int)point.x, (int)point.y + 2] == 0))
                        {
                            map[(int)point.x, (int)point.y] = 1;
                            map[(int)point.x, (int)point.y + 1] = 1;
                            map[(int)point.x, (int)point.y + 2] = 1;
                            boolGrid[(int)point.x, (int)point.y] = 1;
                            boolGrid[(int)point.x, (int)point.y + 1] = 1;
                            boolGrid[(int)point.x, (int)point.y + 2] = 1;
                            keepLooping = false;
                        }
                    }
                }
                else {
                    if (point.x <= 5) {
                        if ((map[(int)point.x + 1, (int)point.y] == 0) && (map[(int)point.x + 2, (int)point.y] == 0)) {
                            map[(int)point.x, (int)point.y] = 1;
                            map[(int)point.x + 1, (int)point.y] = 1;
                            map[(int)point.x + 2, (int)point.y] = 1;
                            boolGrid[(int)point.x, (int)point.y] = 1;
                            boolGrid[(int)point.x + 1, (int)point.y] = 1;
                            boolGrid[(int)point.x + 2, (int)point.y] = 1;
                            keepLooping = false;
                        }
                    }
                }
            }

            iterations++;
        }
    }

    public void CreateInnerHill(int[,] map, int[,] boolGrid) {
        bool unsatisfied = true;
        int iterations = 0;
        while (unsatisfied && iterations <= 10)
        {
            Vector2 position = GetRandomViableNoEdgeCoordinate(boolGrid);
            if ((map[(int)position.x, (int)position.y] == 0) && (map[(int)position.x + 1, (int)position.y] == 0) &&
                (map[(int)position.x, (int)position.y + 1] == 0) && (map[(int)position.x + 1, (int)position.y + 1] == 0)
                )
            {
                map[(int)position.x, (int)position.y] = 1;
                map[(int)position.x + 1, (int)position.y] = 1;
                map[(int)position.x, (int)position.y + 1] = 1;
                map[(int)position.x + 1, (int)position.y + 1] = 1;
                boolGrid[(int)position.x, (int)position.y] = 1;
                boolGrid[(int)position.x + 1, (int)position.y] = 1;
                boolGrid[(int)position.x, (int)position.y + 1] = 1;
                boolGrid[(int)position.x + 1, (int)position.y + 1] = 1;
                unsatisfied = false;
            }
            else
            {
                iterations++;
            }
        }
    }

    public void CreateRiver(int[,] map, int[,] boolGrid) {
        bool keepLooping = true;
        int iterations = 0;
        while (keepLooping && iterations <= 10) {
            Vector2 point = GetRandomViableEdgeCoordinate(boolGrid);
            bool column;
            bool populateNegatively = false;
            if (point.x == 0 || point.x == 7)
            {
                column = true;
                if (point.y >= 5)
                {
                    populateNegatively = true;
                }
            }
            else
            {
                column = false;
                if (point.x >= 5)
                {
                    populateNegatively = true;
                }
            }

            if (column)
            {
                if (populateNegatively)
                {
                    if ((map[(int)point.x, (int)point.y - 1] == 0) && (map[(int)point.x, (int)point.y - 2] == 0))
                    {
                        if (point.x == 0)
                        {
                            if (map[(int)point.x + 1, (int)point.y - 2] == 0)
                            {
                                map[(int)point.x, (int)point.y] = 2;
                                map[(int)point.x, (int)point.y - 1] = 2;
                                map[(int)point.x, (int)point.y - 2] = 2;
                                map[(int)point.x + 1, (int)point.y - 2] = 2;
                                boolGrid[(int)point.x, (int)point.y] = 1;
                                boolGrid[(int)point.x, (int)point.y - 1] = 1;
                                boolGrid[(int)point.x, (int)point.y - 2] = 1;
                                boolGrid[(int)point.x + 1, (int)point.y - 2] = 1;
                                keepLooping = false;
                            }
                        }
                        else
                        {
                            if (map[(int)point.x - 1, (int)point.y - 2] == 0)
                            {
                                map[(int)point.x, (int)point.y] = 2;
                                map[(int)point.x, (int)point.y - 1] = 2;
                                map[(int)point.x, (int)point.y - 2] = 2;
                                map[(int)point.x - 1, (int)point.y - 2] = 2;
                                boolGrid[(int)point.x, (int)point.y] = 1;
                                boolGrid[(int)point.x, (int)point.y - 1] = 1;
                                boolGrid[(int)point.x, (int)point.y - 2] = 1;
                                boolGrid[(int)point.x - 1, (int)point.y - 2] = 1;
                                keepLooping = false;
                            }

                        }
                    }
                }
                else
                {
                    if ((map[(int)point.x, (int)point.y + 1] == 0) && (map[(int)point.x, (int)point.y + 2] == 0))
                    {
                        if (point.x == 0)
                        {
                            if (map[(int)point.x + 1, (int)point.y + 2] == 0)
                            {
                                map[(int)point.x, (int)point.y] = 2;
                                map[(int)point.x, (int)point.y + 1] = 2;
                                map[(int)point.x, (int)point.y + 2] = 2;
                                map[(int)point.x + 1, (int)point.y + 2] = 2;
                                boolGrid[(int)point.x, (int)point.y] = 1;
                                boolGrid[(int)point.x, (int)point.y + 1] = 1;
                                boolGrid[(int)point.x, (int)point.y + 2] = 1;
                                boolGrid[(int)point.x + 1, (int)point.y + 2] = 1;
                                keepLooping = false;
                            }
                        }
                        else
                        {
                            if (map[(int)point.x - 1, (int)point.y + 2] == 0)
                            {
                                map[(int)point.x, (int)point.y] = 2;
                                map[(int)point.x, (int)point.y + 1] = 2;
                                map[(int)point.x, (int)point.y + 2] = 2;
                                map[(int)point.x - 1, (int)point.y + 2] = 2;
                                boolGrid[(int)point.x, (int)point.y] = 1;
                                boolGrid[(int)point.x, (int)point.y + 1] = 1;
                                boolGrid[(int)point.x, (int)point.y + 2] = 1;
                                boolGrid[(int)point.x - 1, (int)point.y + 2] = 1;
                                keepLooping = false;
                            }

                        }
                    }
                }
            }
            else
            { //row
                if (populateNegatively)
                {
                    if ((map[(int)point.x - 1, (int)point.y] == 0) && (map[(int)point.x - 2, (int)point.y] == 0))
                    {
                        if (point.y == 0)
                        {
                            if (map[(int)point.x - 2, (int)point.y + 1] == 0)
                            {
                                map[(int)point.x, (int)point.y] = 2;
                                map[(int)point.x - 1, (int)point.y] = 2;
                                map[(int)point.x - 2, (int)point.y] = 2;
                                map[(int)point.x - 2, (int)point.y + 1] = 2;
                                boolGrid[(int)point.x, (int)point.y] = 1;
                                boolGrid[(int)point.x - 1, (int)point.y] = 1;
                                boolGrid[(int)point.x - 2, (int)point.y] = 1;
                                boolGrid[(int)point.x - 2, (int)point.y + 1] = 1;
                                keepLooping = false;
                            }
                        }
                        else
                        {
                            if (map[(int)point.x - 2, (int)point.y - 1] == 0)
                            {
                                map[(int)point.x, (int)point.y] = 2;
                                map[(int)point.x - 1, (int)point.y] = 2;
                                map[(int)point.x - 2, (int)point.y] = 2;
                                map[(int)point.x - 2, (int)point.y - 1] = 2;
                                boolGrid[(int)point.x, (int)point.y] = 1;
                                boolGrid[(int)point.x - 1, (int)point.y] = 1;
                                boolGrid[(int)point.x - 2, (int)point.y] = 1;
                                boolGrid[(int)point.x - 2, (int)point.y - 1] = 1;
                                keepLooping = false;
                            }

                        }
                    }
                }
                else
                {
                    if ((map[(int)point.x + 1, (int)point.y] == 0) && (map[(int)point.x + 2, (int)point.y] == 0))
                    {
                        if (point.y == 0)
                        {
                            if (map[(int)point.x + 2, (int)point.y + 1] == 0)
                            {
                                map[(int)point.x, (int)point.y] = 2;
                                map[(int)point.x + 1, (int)point.y] = 2;
                                map[(int)point.x + 2, (int)point.y] = 2;
                                map[(int)point.x + 2, (int)point.y + 1] = 2;
                                boolGrid[(int)point.x, (int)point.y] = 1;
                                boolGrid[(int)point.x + 1, (int)point.y] = 1;
                                boolGrid[(int)point.x + 2, (int)point.y] = 1;
                                boolGrid[(int)point.x + 2, (int)point.y + 1] = 1;
                                keepLooping = false;
                            }
                        }
                        else
                        {
                            if (map[(int)point.x + 2, (int)point.y - 1] == 0)
                            {
                                map[(int)point.x, (int)point.y] = 2;
                                map[(int)point.x + 1, (int)point.y] = 2;
                                map[(int)point.x + 2, (int)point.y] = 2;
                                map[(int)point.x + 2, (int)point.y - 1] = 2;
                                boolGrid[(int)point.x, (int)point.y] = 1;
                                boolGrid[(int)point.x + 1, (int)point.y] = 1;
                                boolGrid[(int)point.x + 2, (int)point.y] = 1;
                                boolGrid[(int)point.x + 2, (int)point.y - 1] = 1;
                                keepLooping = false;
                            }

                        }
                    }
                }

            }
            iterations++;
        }
        

    }

    public void CreatePond(int[,] map, int[,] boolGrid) {
        bool unsatisfied = true;
        int iterations = 0;
        while (unsatisfied && iterations <=10) {
            Vector2 position = GetRandomViableNoEdgeCoordinate(boolGrid);
            if ((map[(int)position.x, (int)position.y] == 0) && (map[(int)position.x + 1, (int)position.y] == 0) &&
                (map[(int)position.x, (int)position.y+1] == 0) && (map[(int)position.x + 1, (int)position.y + 1] == 0)
                )
            {
                map[(int)position.x, (int)position.y] = 2;
                map[(int)position.x + 1, (int)position.y] = 2;
                map[(int)position.x, (int)position.y + 1] = 2;
                map[(int)position.x + 1, (int)position.y + 1] = 2;
                boolGrid[(int)position.x, (int)position.y] = 1;
                boolGrid[(int)position.x + 1, (int)position.y] = 1;
                boolGrid[(int)position.x, (int)position.y + 1] = 1;
                boolGrid[(int)position.x + 1, (int)position.y + 1] = 1;
                unsatisfied = false;
            }
            else {
                iterations++;
            }
        }
    }

    public void CreateForests(int[,] map, int[,] boolGrid) {
        //size 1 forest = 1
        //size 2 forest = 1.5
        //size 3 forest = 1.75
        float forests = 0f;
        int loopNum = 0;
        while (forests < numForests && loopNum <= 20)
        {
            int forestType = Random.Range(1, 4);
            if (forestType == 1)
            {
                Vector2 position = GetRandomViableCoordinate(boolGrid);
                map[(int)position.x, (int)position.y] = 3;
                boolGrid[(int)position.x, (int)position.y] = 1;
                forests += 1;
            }
            else if (forestType == 2)
            {
                bool viable = false;
                Vector2 position = GetRandomViableNoEdgeCoordinate(boolGrid);
                int direction = Random.Range(0, 4);

                if (direction == 0)
                { //up 
                    if (boolGrid[(int)position.x, (int)position.y + 1] == 0)
                    {
                        map[(int)position.x, (int)position.y] = 3;
                        map[(int)position.x, (int)position.y + 1] = 3;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y + 1] = 1;
                        viable = true;
                    }
                }
                else if (direction == 1)
                { //down
                    if (boolGrid[(int)position.x, (int)position.y - 1] == 0)
                    {
                        map[(int)position.x, (int)position.y] = 3;
                        map[(int)position.x, (int)position.y - 1] = 3;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y - 1] = 1;
                        viable = true;
                    }
                }
                else if (direction == 2)
                { //left
                    if (boolGrid[(int)position.x - 1, (int)position.y] == 0)
                    {
                        map[(int)position.x, (int)position.y] = 3;
                        map[(int)position.x - 1, (int)position.y] = 3;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x - 1, (int)position.y] = 1;
                        viable = true;
                    }
                }
                else if (direction == 3)
                { //right
                    if (boolGrid[(int)position.x + 1, (int)position.y] == 0)
                    {
                        map[(int)position.x, (int)position.y] = 3;
                        map[(int)position.x + 1, (int)position.y] = 3;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x + 1, (int)position.y] = 1;
                        viable = true;
                    }
                }

                if (viable)
                {
                    forests += 1.75f;
                }

            }
            else if (forestType == 3)
            {
                bool viable = false;
                Vector2 position = GetRandomViableNoEdgeCoordinate(boolGrid);
                int direction = Random.Range(0, 4);

                if (direction == 0)
                { //up + left
                    if ((boolGrid[(int)position.x, (int)position.y + 1] == 0) && (boolGrid[(int)position.x - 1, (int)position.y] == 0))
                    {
                        map[(int)position.x, (int)position.y] = 3;
                        map[(int)position.x, (int)position.y + 1] = 3;
                        map[(int)position.x - 1, (int)position.y] = 3;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y + 1] = 1;
                        boolGrid[(int)position.x - 1, (int)position.y] = 1;
                        viable = true;
                    }
                }
                else if (direction == 1)
                { //left + down
                    if ((boolGrid[(int)position.x, (int)position.y - 1] == 0) && (boolGrid[(int)position.x - 1, (int)position.y] == 0))
                    {
                        map[(int)position.x, (int)position.y] = 3;
                        map[(int)position.x, (int)position.y - 1] = 3;
                        map[(int)position.x - 1, (int)position.y] = 3;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y - 1] = 1;
                        boolGrid[(int)position.x - 1, (int)position.y] = 1;
                        viable = true;
                    }
                }
                else if (direction == 2)
                { //down + right
                    if ((boolGrid[(int)position.x, (int)position.y - 1] == 0) && (boolGrid[(int)position.x + 1, (int)position.y] == 0))
                    {
                        map[(int)position.x, (int)position.y] = 3;
                        map[(int)position.x, (int)position.y - 1] = 3;
                        map[(int)position.x + 1, (int)position.y] = 3;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y - 1] = 1;
                        boolGrid[(int)position.x + 1, (int)position.y] = 1;
                        viable = true;
                    }
                }
                else if (direction == 3)
                { //right + up
                    if ((boolGrid[(int)position.x, (int)position.y + 1] == 0) && (boolGrid[(int)position.x + 1, (int)position.y] == 0))
                    {
                        map[(int)position.x, (int)position.y] = 3;
                        map[(int)position.x, (int)position.y + 1] = 3;
                        map[(int)position.x + 1, (int)position.y] = 3;
                        boolGrid[(int)position.x, (int)position.y] = 1;
                        boolGrid[(int)position.x, (int)position.y + 1] = 1;
                        boolGrid[(int)position.x + 1, (int)position.y] = 1;
                        viable = true;
                    }
                }

                if (viable)
                {
                    forests += 2;
                }
            }

            loopNum += 1;
        }
    }

    public int[,] GenerateBlankGrid() {
        int[,] map = new int[8, 8];
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                map[x, y] = 0;
            }
        }
        return map;
    }

    public int[,] GenerateNewChebyshev() {
        int[,] ChebyshevGrid = new int[,] {
        {0,0,0,0,0,0,0,0},
        {0,1,1,1,1,1,1,0},
        {0,1,2,2,2,2,1,0},
        {0,1,2,3,3,2,1,0},
        {0,1,2,3,3,2,1,0},
        {0,1,2,2,2,2,1,0},
        {0,1,1,1,1,1,1,0},
        {0,0,0,0,0,0,0,0}
    };
        return ChebyshevGrid;
    }

    public TerrainUnitInfo[,] GenerateLevel(int[,] levelMap) {
        TerrainUnitInfo[,] level = new TerrainUnitInfo[8, 8];

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                level[i, j] = GetTerrain(levelMap[i, j]);
            }
        }

        return level;
    }

    private TerrainUnitInfo GetTerrain(int num) {
        Terrain terrain = Terrain.GRASS;

        switch (num) {
            case 0:
                terrain = Terrain.GRASS;
                break;
            case 1:
                terrain = Terrain.MOUNTAIN;
                break;
            case 2:
                terrain = Terrain.RIVER;
                break;
            case 3:
                terrain = Terrain.FOREST;
                break;
            case 4:
                terrain = Terrain.RESTAURANT;
                break;
        }

        return GameInformation.instance.GetTerrainInfo(terrain);
    }
}
