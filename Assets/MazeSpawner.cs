
using System.Collections;
using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public static MazeSpawner Instance;

    [SerializeField]
    [Tooltip("The bottom left bound of the maze, from which it will build up and right")]
    private Vector3 origin = new Vector3(-9, 14, 0);
    [SerializeField]
    [Tooltip("The number of blocks tall the maze is (actual height is double)")]
    private int height = 20;
    [SerializeField]
    [Tooltip("The number of blocks wide the maze is (actual width is double)")]
    private int width = 10;
    [Tooltip("The probability of fake paths emerging while creating the real path.")]
    [SerializeField]
    private float complexity = 1f;
    [Tooltip("The length fake paths will go before ending.")]
    [SerializeField]
    private float difficulty = 1f;
    [SerializeField]
    private GameObject wall;

    private bool[,] grid;
    private bool debug = false;
    private GameObject walls;
    private bool generatingMaze = false;
    private ArrayList fakePaths = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        GenerateMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMaze()
    {
        if (generatingMaze)
        {
            Debug.Log("Already generating maze, wait a moment");
            return;
        }
        if (walls != null)
            Destroy(walls);
        grid = new bool[height, width];
        walls = new GameObject("Maze");
        // maze should default to false, false in this case implies a wall

        // maybe randomize the entire grid here to add some variety vs a block of wall? 
        // use some kind of difficulty variable to determine chance of no wall


        // choose spot in bottom row to be true path
        generatingMaze = true;
        int pathY = 0;
        int pathX = Random.Range(1, width - 1);
        int nextY = pathY;
        int nextX = pathX;
        if (debug)
            Debug.Log("Starting Point: x-" + pathX + ", y-" + pathY);
        if (debug)
            Debug.Log("Eating");
        grid[pathY, pathX] = true;
        int direction = 2;
        int count = 0;
        bool retrace = false;
        do
        {
            do
            {
                if (count > 10)
                {
                    // find some way to retrace back and continue from before
                    Debug.Log("Could not find new direction to proceed");
                    retrace = true;
                }
                count++;
                // reset points
                nextY = pathY;
                nextX = pathX;

                // if retracing, go back through open spaces and look for good walls
                int retraceCount = 0;
                int lastDir = direction;
                while (retrace)
                {
                    if (retraceCount > 1000)
                    {
                        Debug.Log("Could not find viable spot from retracing 1000 spots, continuing from topleft-most open spot");
                        for (int row = height - 1; row >= 0; row--) // search from top to bottom for opening
                        {
                            for (int col = 0; col < width; col++)
                            {
                                if (grid[row, col])
                                {
                                    pathX = col;
                                    pathY = row;
                                }
                            }
                        }
                    }
                    int recount = 0;
                    do // look for last space
                    {
                        if (recount > 10)
                        {
                            Debug.Log("Retraced into corner, moving back.");
                            direction = 4;
                        }
                        nextX = pathX;
                        nextY = pathY;
                        int newD;
                        do
                        {
                            newD = Random.Range(0, 4);
                        } while (newD == lastDir);
                        direction = newD;

                        // hypothesize its movement
                        if (direction == 0) // move up
                        {
                            nextY++;
                            lastDir = 2;
                        }
                        else if (direction == 1) // move right
                        {
                            nextX++;
                            lastDir = 3;
                        }
                        else if (direction == 2) // move down
                        {
                            nextY--;
                            lastDir = 0;
                        }
                        else if (direction == 3) // move left
                        {
                            nextX--;
                            lastDir = 1;
                        }
                        recount++;
                    } while (!CheckSpace(nextX, nextY));
                    // go to last space
                    pathX = nextX;
                    pathY = nextY;

                    // print grid

                    if (debug)
                    {
                        string debugGrid = "";
                        for (int row = height - 1; row >= 0; row--) // start bottom left and go right, then up
                        {
                            for (int col = 0; col < width; col++)
                            {
                                if (row == pathY && col == pathX)
                                    debugGrid += 'X';
                                else if (!grid[row, col])
                                {
                                    debugGrid += '#';
                                }
                                else
                                    debugGrid += 'O';
                            }
                            debugGrid += '\n';
                        }
                        Debug.Log(debugGrid);
                    }

                    // check for surrounding thick walls
                    bool found = false;
                    if (CheckThickWall(nextX - 1, nextY, 2))
                    {
                        nextX--;
                        found = true;
                    }
                    else if (CheckThickWall(nextX + 1, nextY, 2))
                    {
                        nextX++;
                        found = true;
                    }
                    else if (CheckThickWall(nextX, nextY - 1, 2))
                    {
                        nextY--;
                        found = true;
                    }
                    else if (CheckThickWall(nextX, nextY + 1, 2))
                    {
                        nextY++;
                        found = true;
                    }
                    if (found)
                    {
                        // advance to it and eat it
                        pathX = nextX;
                        pathY = nextY;
                        if (debug)
                            Debug.Log("Eating");
                        count = 0;
                        grid[pathY, pathX] = true;
                        retrace = false;
                    }
                    retraceCount++;
                }

                // create new direction
                int newDirection;
                do
                {
                    newDirection = Random.Range(0, 4);
                } while (newDirection == direction);
                direction = newDirection;

                // hypothesize its movement
                if (direction == 0) // move up
                    nextY--;
                else if (direction == 1) // move right
                    nextX--;
                else if (direction == 2) // move down
                    nextY++;
                else if (direction == 3) // move left
                    nextX++;

                // check if its a thick wall
            } while (!CheckThickWall(nextX, nextY, 2));

            // advance to it and eat it
            pathX = nextX;
            pathY = nextY;
            if (debug)
                Debug.Log("Eating");
            count = 0;
            grid[pathY, pathX] = true;

            // create fake path
            if (Random.Range(0f, 3 + complexity) < complexity)
            {
                fakePaths.Add((pathX, pathY));
                Debug.Log("Created fake path start point at: " + pathX + ", " + pathY);
            }

            // print grid

            if (debug)
            {
                string debugGrid = "";
                for (int row = height - 1; row >= 0; row--) // start bottom left and go right, then up
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (row == pathY && col == pathX)
                            debugGrid += 'X';
                        else if (!grid[row, col])
                        {
                            debugGrid += '#';
                        }
                        else
                            debugGrid += 'O';
                    }
                    debugGrid += '\n';
                }
                Debug.Log(debugGrid);
            }

            
        } while (pathY < height - 1); // this should eventually create a path of truth reaching the top


        // generate fake paths
        Debug.Log("Generating fake paths");
        foreach ((int, int) path in fakePaths)
        {
            bool pathComplete = SpawnFakePath(path.Item1, path.Item2);
            Debug.Log(pathComplete ? "Fake path completed" : "Fake path reached an end");
        }


        // for each false in the grid, spawn a wall at the proper location
        for (int row = 0; row < height; row++) // start bottom left and go right, then up
        {
            for (int col = 0;  col < width; col++)
            {
                if (!grid[row, col])
                {
                    Instantiate(wall, origin + new Vector3(col * 2, row * 2, 0), Quaternion.identity, walls.transform);
                }
            }
        }

        generatingMaze = false;
    }
    private bool SpawnFakePath(int x, int y)
    {
        int pathY = y;
        int pathX = x;
        int nextY = pathY;
        int nextX = pathX;
        int direction = 2;
        int count = 0;
        do
        {
            do
            {
                if (count > (int)difficulty * 8)
                {
                    // find some way to retrace back and continue from before
                    Debug.Log("Could not find new direction for fake path");
                    return false;
                }
                count++;
                // reset points
                nextY = pathY;
                nextX = pathX;


                // create new direction
                int newDirection;
                do
                {
                    newDirection = Random.Range(0, 4);
                } while (newDirection == direction);
                direction = newDirection;

                // hypothesize its movement
                if (direction == 0) // move up
                    nextY--;
                else if (direction == 1) // move right
                    nextX--;
                else if (direction == 2) // move down
                    nextY++;
                else if (direction == 3) // move left
                    nextX++;

                // check if its a thick wall
            } while (!CheckThickWall(nextX, nextY, 2));

            // advance to it and eat it
            pathX = nextX;
            pathY = nextY;
            count = 0;
            grid[pathY, pathX] = true;

            // print grid

            if (debug)
            {
                string debugGrid = "";
                for (int row = height - 1; row >= 0; row--) // start bottom left and go right, then up
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (row == pathY && col == pathX)
                            debugGrid += 'X';
                        else if (!grid[row, col])
                        {
                            debugGrid += '#';
                        }
                        else
                            debugGrid += 'O';
                    }
                    debugGrid += '\n';
                }
                Debug.Log(debugGrid);
            }
        } while (Random.Range(0f, 1f + difficulty) < difficulty);
        return true;
    }
    private bool CheckWall(int x, int y) // returns true if a wall is at the location, also false if on in bounds
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return !grid[y, x];
        }
        return false;
    }
    private bool CheckSpace(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return grid[y, x];
        }
        return false;
    }
    private bool CheckThickWall(int x, int y, int thickness)
    {
        if (CheckWall(x, y))
        {
            if (y == height - 1 || y == 0)
                return true;
            int surrounding = 0;
            if (CheckWall(x, y - 1))
                surrounding++;
            if (CheckWall(x, y + 1))
                surrounding++;
            if (CheckWall(x + 1, y))
                surrounding++;
            if (CheckWall(x - 1, y))
                surrounding++;
            if (surrounding > thickness)
                return true;
        }
        return false;
    }
}
