﻿public enum TColor { WHITE, BLACK };
public class Square
{
    public TColor Color { set; get; } // Either WHITE or BLACK
    public int Number { set; get; } // Either a clue number or -1 (Note: A BLACK square is always -1)
    public Square()                // Initialize a square to WHITE and its clue number to -1 
    {
        Color = TColor.WHITE;
        Number = -1;
    }
}

public class Puzzle
{
    private Square[,] grid; 

    private int N;

    private int M;

    private List<int> acrossClues = new List<int>(0);
    private List<int> downClues = new List<int>(0);

    /// <summary>
    /// Takes in a parameter N, then creates an N by N grid of initially all white squares.
    /// Will throw exception of N is not greater than 1
    /// </summary>
    /// 
    /// <param name="N"></param>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Puzzle(int N)
    {
        //throws exception if number of squares isnt atleast 2
        if (N<1)
        {
            throw new ArgumentOutOfRangeException("puzzle size must be greater than 0");
        }
        this.N = N;
        init(N);
    }

    /// <summary>
    /// Initializes a grid for the crossword.
    /// </summary>
    /// 
    /// <param name="N"></param>
    private void init(int N)//this is made into a seperate method, so that Initialize(int M) can be called on the same Puzzle object multiple times to recreate the grid, instead of making new object
    {
        grid = new Square[N, N]; // creates a new array delegated to be squares
        for (int j = 0; j < N; j++)
            for (int i = 0; i < N; i++)
                grid[j, i] = new Square(); // calls square constructer on each index
    }

    /// <summary>
    /// Takes in a parameter M, then creates M amount of black squares to the Puzzle object that called this method.
    /// If M is Greater than the amount of squares the Puzzle object has, InvalidOperationException is thrown.
    /// If M is less than 0, InvalidOperationException is thrown.
    /// </summary>
    /// 
    /// <param name="M"></param>
    /// 
    /// <exception cref="InvalidOperationException"></exception>
    public void Initialize(int M)
    {
        if (this.M != 0 || acrossClues.Count != 0 || downClues.Count != 0) //checks to see if black squares, across clues, or down clues were already added, recreates if so
        {
            init(N);
        }

        if (M < 0) //if M is negative throw exception
        {
            throw new InvalidOperationException("cannot have negative black squares");
        }
        else if (M > (N * N)) // if M is greater than available amount of black squares throw exception
        {
            throw new InvalidOperationException("cannot have more black squares than white");
        }
        else
        {
            //creates a list of coords and randomly selects a spot 
            //then removes it from list to ensure the same spot cant be selected again
            //adds black squares and removes from list, M amount of times
            this.M = M;
            int puzzleSize = N* N;

            Random rand = new Random();

            //creates a list of strings with each index being a comma delimited coordinate. ie: (x,y)
            //will go through the list and add the coords sequentially from 0,0 .. N,N
            List<string> coords = new List<String>(puzzleSize); 
            for (int i = 0; i != puzzleSize; i++)
            {
                int x = i / N;//current x is calculated by truncated value of i/N, ensuring wraparound of row
                int y = i % N;//current y is calculated by reminder of i%N, ensuring a wraparound of column
                coords.Add(x + "," + y);
            } 
            //generates a random number from 0 to #squares
            //and then access the List at that random number, splits x and y components of index via comma delimiter,
            //then removes it that index from list
            for (int i = M;i != 0; i--)
            {
                int randNum = rand.Next(0,coords.Count);
                string[] randCoords =  coords[randNum].Split(',') ;
                grid[int.Parse(randCoords[0]),int.Parse( randCoords[1])].Color = TColor.BLACK; //converts string at index[0] and index[1] to x and y coords
                coords.RemoveAt(randNum);
            }
            
        }
    }


    // Number the crossword grid (6 marks)
    public void Number()
    {
        //initialize across and down clues length to puzzle size
        this.acrossClues = new List<int>(N * N);
        this.downClues = new List<int>(N * N);

        //count numbered squares, 
        int num = 1;

        //create a puzzle with a outline of BLACK squares around a center
        Puzzle p = new Puzzle(N + 2);
        for (int i = 0; i<N+2;i++)
        {
            p.grid[0, i].Color = TColor.BLACK;
        }
        for (int i = 0; i < N + 2; i++)
        {
            p.grid[N+1, i].Color = TColor.BLACK;
        }
        for (int i = 0; i< N+2; i++)
        {
            p.grid[i, 0].Color = TColor.BLACK;
        }
        for (int i = 0; i < N + 2; i++)
        {
            p.grid[i, N+1].Color = TColor.BLACK;
        }

        //put this.grid into center of new puzzle
        for (int i = 1; i != N+1; i++)
        {
            for (int j = 1; j != N+1; j++)
            {
                p.grid[i, j].Color = this.grid[i-1, j-1].Color;
            }
        }
       
        //number grid now
        for (int i = 1; i != N + 1; i++)
        {
            for (int j = 1; j != N + 1; j++)
            {
                //if square is not black
                if (p.grid[i,j].Color != TColor.BLACK)
                {
                    
                    //acrossClues
                    if (p.grid[i,j-1].Color == TColor.BLACK && p.grid[i,j+1].Color != TColor.BLACK)//if spot left is black and right is not black
                    {
                       
                        this.grid[i - 1, j - 1].Number = num;
                        acrossClues.Add(num);
                        num++;     
                    }
                    //downClues
                    if (p.grid[i-1, j].Color == TColor.BLACK && p.grid[i+1,j].Color != TColor.BLACK)//if spot above is black and below is not black
                    {
                        if (this.grid[i - 1, j - 1].Number != -1) // if grid is already numbered, just add it to downclues
                        {
                            downClues.Add(this.grid[i - 1, j - 1].Number);
                        }
                        else //if its not numbered, number grid, add to downclues, and increase num
                        {
                            this.grid[i - 1, j - 1].Number = num;
                            downClues.Add(num);
                            num++;
                            
                        }
                    }
                    
                }
            }
        }
        
    }


    // Print out the numbers for the Across and Down clues (in order) (4 marks)
    public void PrintClues()
    {
        Console.WriteLine("Across clues: ");
        for (int i = 0; i < acrossClues.Count; i++)  //loops through the index i of across clues.
        {
            Console.Write(acrossClues[i] + "  ");  //prints the value of acrossClue at index i
        }

        Console.WriteLine(" "); //Blank line for asthetics

        Console.WriteLine("Down clues: ");
        for (int i = 0; i < downClues.Count; i++)  //loops through the index i of down clues.
        {
            Console.Write(downClues[i] + "  "); //Down clues values at index i are printed.

        }
        Console.WriteLine();
    }
    /// <summary>
    /// Prints out Puzzle object that called this method onto the console, seperating each row with dashes.
    /// Black squares are denoted with X, Numbered white squares are denoted by their number, and white squares are depicted as blank spots.
    /// </summary>
    public void PrintGrid()
    {

        //will first find if the highest digit is greater than 1 for formatting purposes of the boxes of the grid

        //the highest amount of digits for acrossclues, down clues, and between both
        int digitsAmount;
        int highestAcross = 0;
        int highestDown = 0;

        //find the last index, the highest, if the index is greater than 0
        if (acrossClues.Count != 0)
        {
            highestAcross = acrossClues[acrossClues.Count - 1];
        }
        if (downClues.Count != 0)
        {
            highestDown = downClues[downClues.Count - 1];
        }

        string spaces = "";
        //if both highest and down are 0, will make digits equal to 1
        if (highestAcross ==0 && highestDown ==0)
        {
            digitsAmount = 1;
        }
        //otherwise will find the highest number between two and calculate the amount of digits the number has
        else if (highestAcross > highestDown)
        {
            digitsAmount = (int)Math.Floor(Math.Log10(highestAcross) + 1);
        }
        else
        {
            digitsAmount = (int)Math.Floor(Math.Log10(highestDown) + 1);
        }

        //add spaces based on amount of digits 
        for (int i = 1;i!=digitsAmount;i++)
        {
            spaces += " ";
        }

        //the function will now create boxes, added extra spaces for formatting if the highest number has more than 1 digit
        
        for(int a = 0; a < N; a++) // makes top row of dashes and lines
            Console.Write( spaces+ " -" + spaces+ " |");

        //for loop that will go through each 
        for (int j = 0; j < N; j++) // goes through each row, doing a writeline to break into next line
        {
            Console.WriteLine();

            for (int i = 0; i < N; i++)//goes through each column, using write() based on number and colour, and then to draw a line
            {
                if (grid[j, i].Number != -1) // if its a numbered square
                {
                    // if the digits of the numbered square is greater than 1 it will will cut off the beginning spaces based on how many digits their are
                    int curDigitsAmount = (int)Math.Floor(Math.Log10(grid[j, i].Number));        
                    string numberString = " " + spaces + grid[j, i].Number;
                    if (curDigitsAmount != 0) 
                    {
                        numberString = numberString.Substring(curDigitsAmount);
                    }
                    Console.Write(numberString);
                }

                else if (grid[j, i].Color == TColor.BLACK)// if the square is black
                {
                    Console.Write(spaces+" X");
                }
                else // if the square is a non-numbered white square
                {
                    Console.Write(spaces +"  ");
                }
                Console.Write(spaces + " |");
            }

            Console.WriteLine(); //a bottom row of dashes and lines
            for (int a = 0; a < N; a++)
                Console.Write( spaces+ " -" + spaces+ " |");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// checks if the puzzle object that called this method has 180 degree rotational symmetry. returns true if symmetrical false otherwise.
    /// </summary>
    /// 
    /// <returns>boolean</returns>
    public bool Symmetric() //goes through half the square objects in the puzzle grid, comparing against its 180 degree counterpart
    {
        //one before the middle element of an odd size 2d array, or the final element in the middle row of an even size 2d array (half -1 or half of all squares respectively)
        //this calculation ensures all squares with a 180 degree counterpart are compared exactly once only
        int middwayItem = ((N * N) - (2 * N % 2)) / 2; 
       
        for (int i = 0; i != middwayItem; i++)
        {
            int x = i / N;//current x is calculated by truncated value of i/N, ensuring wraparound of row
            int y = i % N;//current y is calculated by reminder of i%N, ensuring a wraparound of column

            if (grid[x, y].Color != grid[(N - 1) - x, (N - 1) - y].Color)//compares current item, to its 180 degree counterpart
            {
                return false; 
            }
        }
        return true;
    }

}


