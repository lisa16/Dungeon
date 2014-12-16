using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    class Program
    {
        readonly char WALL = '#';
        readonly char ROOM = ' ';
        readonly char ENTRANCE = '*';
        int numOfCuts;
        int maxWidth;
        int maxHeight;
        char[,] arr;
        int seed;
        Random rand;
        int[,] connectedRoomArray;

        /// <summary>
        /// Initializes the seed
        /// </summary>
        public void InitSeed()
        {
            rand = new Random(seed);
        }

        /// <summary>
        /// Generate a random dungeon with the size of maxWidth and maxHeight
        /// </summary>
        public void GenerateDungeon()
        {
            // generate random number of walls to add to the dungeon
            numOfCuts = GenerateNumberOfWalls();
            // create 2D array with maxHeight rows and maxWidth columns
            arr = new char[maxHeight, maxWidth];
            for (int i = 0; i < maxHeight; i++)
            {
                for (int j = 0; j < maxWidth; j++)
                {
                    if (i == 0 || i == maxHeight - 1 || j == 0 || j == maxWidth - 1)
                    {
                        arr[i, j] = WALL;
                    }
                    else
                    {
                        arr[i, j] = ROOM;
                    }
                }
            }

            // add walls at random locations
            for (int a = 0; a < numOfCuts; a++)
            {
                int x = GenerateRandomXCoord();
                int y = GenerateRandomYCoord();
                PlaceWall(x, y);
                while (!IsAllRoomConnected())
                {
                    arr[y, x] = ROOM;
                    x = GenerateRandomXCoord();
                    y = GenerateRandomYCoord();
                    PlaceWall(x, y);
                }
            }

            //add an entrance
            PlaceEntrance();

            //prints out the 2D array that contains the dungeon
            for (int i = 0; i < maxHeight; i++)
            {
                for (int j = 0; j < maxWidth; j++)
                {
                    Console.Write(arr[i, j]);
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Checks to see if all rooms are connected, they are connected if all the rooms are accessible from each other
        /// </summary>
        /// <returns>true if all the rooms are accessible from each other</returns>
        public bool IsAllRoomConnected()
        {
            //If number of connected rooms is equal to number of total rooms, then all rooms must be connected to each other
            bool isAllRoomConnected = GetNumberOfFirstConnectedRoom() == GetNumOfRooms();
            return isAllRoomConnected;
        }

        /// <summary>
        /// Calculates and returns the size of first cluster of rooms
        /// </summary>
        /// <returns>size of first cluster of rooms</returns>
        public int GetNumberOfFirstConnectedRoom()
        {
            int numOfConnectedRooms = 0;
            connectedRoomArray = new int[maxHeight, maxWidth];
            for (int i = 0; i < maxHeight; i++)
            {
                for (int j = 0; j < maxWidth; j++)
                {
                    if(arr[i,j] == ROOM)
                    {
                        CheckValidRoom(j, i);
                        i = maxHeight;
                        j = maxWidth;
                    }
                }
            }

            for (int i = 0; i < maxHeight; i++)
            {
                for (int j = 0; j < maxWidth; j++)
                {
                    if (connectedRoomArray[i, j] == 1)
                    {
                        numOfConnectedRooms++;
                    }
                }
            }

            return numOfConnectedRooms;
        }

        /// <summary>
        /// Finds all the valid moves (up, right, left, down) from the current location
        /// </summary>
        /// <param name="x">x coordinate to move</param>
        /// <param name="y">y coordinate to move</param>
        public void CheckValidRoom(int x, int y)
        {
            if (arr[y, x] == ROOM && (connectedRoomArray[y, x] != 1))
            {
                //change the value in the array to 1 to indicate room has been visited
                connectedRoomArray[y, x] = 1;

                //move down
                if (y + 1 <= maxHeight - 1)
                {
                    CheckValidRoom(x, y + 1);
                }

                //move up
                if (y - 1 >= 0)
                {
                    CheckValidRoom(x, y - 1);
                }

                //move left
                if (x - 1 >= 0)
                {
                    CheckValidRoom(x - 1, y);
                }

                //move right
                if (x + 1 <= maxWidth - 1)
                {
                    CheckValidRoom(x + 1, y);
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Number of locations on the 2D array that are blank (not a wall)
        /// </summary>
        /// <returns>number of blanks in the map</returns>
        public int GetNumOfRooms()
        {
            int num = 0;
            for (int i = 0; i < maxHeight; i++)
            {
                for (int j = 0; j < maxWidth; j++)
                {
                    if (arr[i, j] == ROOM)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        /// <summary>
        /// Generates how many random walls will be added
        /// </summary>
        /// <returns>Number of walls to place in the map</returns>
        public int GenerateNumberOfWalls()
        {
            int maxW = (maxWidth - 1);
            int maxH = (maxHeight - 1);
            int randomMin = (int)(maxW * maxH * 0.2);
            int randomMax = (int)(maxW * maxH * 0.5);
            int numOfWalls = rand.Next(randomMin, randomMax);
            return numOfWalls;
        }

        /// <summary>
        /// Place a wall at position x and y
        /// </summary>
        /// <param name="x">x coord</param>
        /// <param name="y">y coord</param>
        public void PlaceWall(int x, int y)
        {
            arr[y, x] = WALL;
        }

        /// <summary>
        /// Place entrance/exit at a valid location in the map
        /// </summary>
        public void PlaceEntrance()
        {
            int xCoord = GenerateRandomXCoord();
            int yCoord = GenerateRandomYCoord();
            while (!(IsBesideRoom(xCoord, yCoord)))
            {
                xCoord = GenerateRandomXCoord();
                yCoord = GenerateRandomYCoord();
            }
            arr[yCoord, xCoord] = ENTRANCE;
        }

        //
        /// <summary>
        /// Returns true if the given location is beside a room
        /// </summary>
        /// <param name="x">x coord to check</param>
        /// <param name="y">y coord to check</param>
        /// <returns>true if given x and y coord is beside a room</returns>
        public bool IsBesideRoom(int x, int y)
        {
            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;

            //check left
            if (x - 1 >= 0)
            {
                left = (arr[y, x - 1] == ROOM);
            }

            //check right
            if (x + 1 <= maxWidth - 1)
            {
                right = (arr[y, x + 1] == ROOM);
            }

            //check up
            if (y - 1 >= 0)
            {
                up = (arr[y - 1, x] == ROOM);
            }

            //check down
            if (y + 1 <= maxHeight - 1)
            {
                down = (arr[y + 1, x] == ROOM);
            }

            bool isBesideARoom = (left | right | up | down);
            return isBesideARoom;
        }

        /// <summary>
        /// Generates a random x coordinate 
        /// </summary>
        /// <returns>Random x coord</returns>
        public int GenerateRandomXCoord()
        {
            int RandomNum = rand.Next(0, maxWidth);
            return RandomNum;
        }

        /// <summary>
        /// Generates a random y coordinate
        /// </summary>
        /// <returns>Random y coord</returns>
        public int GenerateRandomYCoord()
        {
            int RandomNum = rand.Next(0, maxHeight);
            return RandomNum;
        }

        //first argument = width, second argument = height and third argument is the seed
        //i.e. Program.exe {width} {height} -s {seedNum}
        static void Main(string[] args)
        {
            Program dungeon = new Program();
            //get the first argument which is the maximum width
            dungeon.maxWidth = int.Parse(args[0]);
            //get the second argument which is the maximum height
            dungeon.maxHeight = int.Parse(args[1]);
            //get the third argument which is the seed
            dungeon.seed = int.Parse(args[3]);
            dungeon.InitSeed();

            //Generate a random dungeon 
            dungeon.GenerateDungeon();
            Console.ReadLine();
        }
    }
}
