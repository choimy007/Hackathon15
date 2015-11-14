using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.
    
public class BoardManager : MonoBehaviour
{
    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Count
    {
        public int minimum;             //Minimum value for our Count class.
        public int maximum;             //Maximum value for our Count class.
        
        
        //Assignment constructor.
        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
    
    
    public int outsideC = 15;                                         //Number of columns in our game board.
    public int outsideR = 15;                                            //Number of rows in our game board.
    // Inside variables
    public int insideC = 8;
    public int insideR = 4;
    //public Count wallCount = new Count (5, 9);                      //Lower and upper limit for our random number of walls per level.
    //public Count foodCount = new Count (1, 5);                      //Lower and upper limit for our random number of food items per level.
    public GameObject exit;                                         //Prefab to spawn for exit.
    public GameObject[] floorTiles;                                 //Array of floor prefabs.
    //public GameObject[] wallTiles;                                  //Array of wall prefabs.
    public GameObject[] foodTiles;                                  //Array of food prefabs.
    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
    public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.
    public GameObject[] houseTiles;                                 //Array of house prefabs.
    public GameObject[] cookTiles;
    public GameObject window;
    public GameObject insideF;
    public GameObject insideW;
    //public GameObject player;
    
    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    private List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.
    
    private static char X = 'X';                                    //Walls
    private static char O = 'O';                                    //Floors
    private static char H = 'H';                                    //House
    private static char C = 'C';                                    //Cooking Counter
    private static char E = 'E';                                    //Exit
    private static char W = 'W';                                    //Window
    
    public static char[,] field = new char[,]
    {
            {X, X, X, X, X, X, X, X, X, X, X, X, X, X, X},  //0
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //1
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //2
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //3
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //4
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //5
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //6
            {X, O, O, O, O, O, O, H, O, O, O, O, O, O, X},  //7
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //8
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //9
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //10
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //11
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //12
            {X, O, O, O, O, O, O, O, O, O, O, O, O, O, X},  //13
            {X, X, X, X, X, X, X, X, X, X, X, X, X, X, X}   //14
    };

    public static char[,] inside = new char[,]
    {
        { O,O,O,E,O,O,O,O },
        { O,O,O,O,O,O,O,O },
        { O,O,C,O,O,O,O,O },
        { X,X,X,X,W,X,X,X }
    };

    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList (int columns, int rows)
    {
        //Clear our list gridPositions.
        gridPositions.Clear ();
        
        //Loop through x axis (columns).
        for(int x = 1; x < columns-1; x++)
        {
            //Within each column, loop through y axis (rows).
            for(int y = 1; y < rows-1; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add (new Vector3(x, y, 0f));
            }
        }
    }
    
   
    // Takes in the 2D array and puts floors and walls accordingly
    GameObject arrayToBoard(char[,] mazeArray, int x, int y, int level)
    {
        if (level == 1)
        {
            // x is the number of columns = width
            // z is the number of rows = height
            char tileStr = mazeArray[x, y];

            //// If there is an X, return a Wall
            //if (tileStr == 'X')
            //    return wallTiles[Random.Range(0, wallTiles.Length)];

            // If there is an O, return a Floor
            if (tileStr == 'O')
                return floorTiles[Random.Range(0, floorTiles.Length)];

            // If there is an H, return the House
            else if (tileStr == 'H')
                return houseTiles[Random.Range(0, houseTiles.Length)];

            // Otherwise return a floor tile
            else
                return floorTiles[Random.Range(0, floorTiles.Length)];
        }

        else if (level == 0)
        {
            // x is the number of columns = width
            // z is the number of rows = height
            char tileStr = mazeArray[x, y];

            // If there is an X, return a Wall
            if (tileStr == 'X')
                return insideW;

            // If there is an O, return a Floor
            if (tileStr == 'O')
                return insideF;

            // If there is an H, return the House
            else if (tileStr == 'C')
                return cookTiles[Random.Range(0, houseTiles.Length)];

            else if (tileStr == 'W')
                return window;

            else if (tileStr == 'E')
                return exit;

            // Otherwise return a floor tile
            else
                return insideF;
        }
        else
            return floorTiles[Random.Range(0, floorTiles.Length)];
    }


    //Sets up the outer walls and floor (background) of the game board.
    void BoardSetup (int columns, int rows, int level)
    {
        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject ("Board").transform;
        
        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for(int x = -1; x < columns + 1; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for(int y = -1; y < rows + 1; y++)
            {
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                GameObject toInstantiate;
                
                //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                if(x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
                
                else if (level == 1)
                    toInstantiate = arrayToBoard(field, x, y, level);

                else
                    toInstantiate = arrayToBoard(inside, y, x, level);

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance =
                    Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
                
                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent (boardHolder);
            }
        }
    }
    
    
    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition ()
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range (0, gridPositions.Count);
        
        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPositions[randomIndex];
        
        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt (randomIndex);
        
        //Return the randomly selected Vector3 position.
        return randomPosition;
    }
    
    
    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range (minimum, maximum+1);
        
        //Instantiate objects until the randomly chosen limit objectCount is reached
        for(int i = 0; i < objectCount; i++)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();
            
            //Choose a random tile from tileArray and assign it to tileChoice
            GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
            
            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
    
    
    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupScene (int level)
    {
        //Creates the outer walls and floor.
        if (GameManager.level == 0)
        {
            BoardSetup(insideC, insideR, level);
            InitialiseList(insideC, insideR);
        }

        else //if (GameManager.level == 1)
        {
            BoardSetup(outsideC, outsideR, level);
            InitialiseList(outsideC, outsideR);
        }

        //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
        //LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);

        //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
        //LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

        //Determine number of enemies based on current level number, based on a logarithmic progression
        int enemyCount = (int)Mathf.Log(level, 2f);
        
        //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
        //LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
        
        //Instantiate the exit tile in the upper right hand corner of our game board
        //Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}