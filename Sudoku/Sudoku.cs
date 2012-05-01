using System;

public class Sudoku
{
	int[,] currGrid;
	int[,] numPossibleValues;
	Boolean[,][] isValidTracker;
	Boolean solved;
	int numSteps;
	
	// Constructor
	public Sudoku ()
	{
		this.currGrid = new int[9,9];
		this.numPossibleValues = new int[9,9];
		this.isValidTracker = new Boolean[9,9][];
		
		// Initialize all number of possible values to 9
		// Initialize all values to true (is valid)
		for (int rowNum = 0; rowNum < 9; rowNum ++)
		{
			for (int colNum = 0; colNum < 9; colNum ++)
			{
				this.numPossibleValues[rowNum,colNum] = 9;
				this.isValidTracker[rowNum,colNum] = new Boolean[9];
				for (int entryNum = 0; entryNum < 9; entryNum ++)
				{
					isValidTracker[rowNum,colNum][entryNum] = true;
				}
			}
		}
	}
	
	/*
	 * Add grid to Sudoku object.
	 * Checks if grid is valid.
	 */
	public Boolean addGrid(int[,] grid, out string errorMessage)
	{
		// Stores value in the grid with the current coordinates
		int currEntry;
		
		for (int rowNum = 0; rowNum < 9; rowNum ++)
		{
			for (int colNum = 0; colNum < 9; colNum ++)
			{
				// Get value in the grid with the current coordinates
				currEntry = grid[rowNum,colNum];
			
				// Check if value is in [1,9]
				if(currEntry >= 1 && currEntry <= 9)
				{
					if (!this.addEntry(rowNum,colNum,currEntry))
					{
						errorMessage = "Illegal value of "
										+ currEntry
										+ " at column "
										+ (colNum + 1)
										+ " and row "
										+ (rowNum + 1)
										+ "."
										;
						return false;
					}
				}
				else if(currEntry == 0)
				{
					//do nothing
				}
				else
				{
					// Error: this is not a valid Sudoku puzzle.
					// Current implementation of file I/O does not allow this to occur
					errorMessage = "Invalid value of "
									+ currEntry
									+ " at column "
									+ (colNum + 1)
									+ " and row "
									+ (rowNum + 1)
									+ "."
									;
					return false;
				}
			}
		}
		
		errorMessage = "";	// Must be modified due to "out" modifier
		return true;
	}

	/*
	 *	Checks if a given entry for a given coordinate is valid
	 *	Returns false if an entry is invalid.
	 *	Returns true otherwise.
	 */
	private Boolean isValid(int entryRowNum, int entryColNum, int entry)
	{
		return this.isValidTracker[entryRowNum,entryColNum][entry - 1];
	}
	
	/*
	 *	Adds a given entry to the given coordinate.
	 *	Returns false if unable to add entry.
	 *	Otherwise returns true.
	 */
	private Boolean addEntry(int entryRowNum, int entryColNum, int entry)
	{
		// Checks if we can add the entry
		if (!this.isValid(entryRowNum,entryColNum,entry)) return false;
		
		// Adds the entry
		this.currGrid[entryRowNum,entryColNum] = entry;
		
		// Updates isValidTracker for the entire row
		for (int rowNum = 0; rowNum < 9; rowNum ++)
		{
			if (isValidTracker[rowNum,entryColNum][entry - 1]) numPossibleValues[rowNum,entryColNum]--;
			isValidTracker[rowNum,entryColNum][entry - 1] = false;
		}
		
		// Updates isValidTracker for the entire column
		for (int colNum = 0; colNum < 9; colNum ++)
		{
			if (isValidTracker[entryRowNum,colNum][entry - 1]) numPossibleValues[entryRowNum,colNum]--;
			isValidTracker[entryRowNum,colNum][entry - 1] = false;
		}
		
		// Updates isValidTracker for [1,9] for the given coordinates
		for (int entryNum = 0; entryNum < 9; entryNum ++)
		{
			if (isValidTracker[entryRowNum,entryColNum][entryNum]) numPossibleValues[entryRowNum,entryColNum]--;
			isValidTracker[entryRowNum,entryColNum][entryNum] = false;
		}

		// Updates isValidTracker for the 3x3 block that the given coordinates is in
		int rowBlock = entryRowNum / 3;
		int colBlock = entryColNum / 3;

		for (int rowNum = (rowBlock * 3); rowNum < ((rowBlock + 1) * 3); rowNum++)
		{
			for (int colNum = (colBlock * 3); colNum < ((colBlock + 1) * 3); colNum++)
			{
				if (isValidTracker[rowNum,colNum][entry - 1]) numPossibleValues[rowNum,colNum]--;
				isValidTracker[rowNum,colNum][entry - 1] = false;
			}
		}
		
		return true;
	}

	/*
	 *	Retrives the current Sudoku grid
	 */
	public int[,] getCurrentGrid()
	{
		return this.currGrid;
	}
	
	/*
	 *	Solves the Sudoku puzzle.
	 *	Calls all algorithms in the correct order
	 */
	public Boolean solve()
	{
		solved = this.runBruteForce();
		return this.isSolved();
	}
	
	/*
	 * Returns true if the puzzle is solved.
	 * Otherwise returns false.
	 */
	public Boolean isSolved()
	{
		return this.solved;
	}
	
	/*
	 *	Uses backtracking to solve the puzzle.
	 *	Slowest algorithm, and should be ran last.
	 */
	private Boolean runBruteForce()
	{
		// used for taking snapshots of isValidTracker and numPossibleValues
		Boolean [,][] initValidTracker = new Boolean[9,9][];
		int [,] initPossibleValues = new int[9,9];

		// gets snapshot of isValidTracker and numPossible Values after all techniques have been used
		// and before brute force is used; puts snapshots in initValidTracker and initPossibleValues
		for (int rowNum = 0; rowNum < 9; rowNum++)
		{
			for (int colNum = 0; colNum < 9; colNum++)
			{
				initValidTracker[rowNum,colNum] = new Boolean[9];
				initPossibleValues[rowNum,colNum] = numPossibleValues[rowNum,colNum];
				for (int entryNum = 0; entryNum < 9; entryNum++)
				{
					initValidTracker[rowNum,colNum][entryNum] = isValidTracker[rowNum,colNum][entryNum];
				}
			}
		}

		// finds number of zero entries
		int numZeroEntries = 0;

		for (int rowNum = 0; rowNum < 9; rowNum++)
		{
			for (int colNum = 0; colNum < 9; colNum++)
			{
				if (this.currGrid[rowNum,colNum] == 0) numZeroEntries++;
			}
		}

		// a zero entry is a 3-element 'tuple' that consists of
		// the coordinates of the entry and the numerical value of the entry
		int[,] zeroEntries = new int[numZeroEntries,3];
		int indexpos = 0;

		// fills the zero entries array with the coordinates and value of the zero entries
		for (int rowNum = 0; rowNum < 9; rowNum++)
		{
			for (int colNum = 0; colNum < 9; colNum++)
			{
				if (this.currGrid[rowNum,colNum] == 0)
				{
					zeroEntries[indexpos,0] = rowNum;
					zeroEntries[indexpos,1] = colNum;
					zeroEntries[indexpos,2] = 0;
					indexpos++;
				}
			}
		}

		int numFilledEntries = 0;
		int zeroEntryRow, zeroEntryCol, zeroEntryVal;
		bool filledCurEntry;

		// fills in as many empty entries as possible
		while (numFilledEntries < numZeroEntries)
		{
			// DEBUGGING: UPDATING NUMSTEPS //
			numSteps++;

			// gets the row, column, and value of a particular zero entry in the array
			zeroEntryRow = zeroEntries[numFilledEntries,0];
			zeroEntryCol = zeroEntries[numFilledEntries,1];
			zeroEntryVal = zeroEntries[numFilledEntries,2];	
			// resets variable to false for every iteration
			filledCurEntry = false;

			for (int num = zeroEntryVal; num < 9; num++)
			{
				// if the num value is a possible number for the entry, add it as an entry
				if (this.isValid(zeroEntryRow, zeroEntryCol, num+1))
				{
					addEntry(zeroEntryRow, zeroEntryCol, num+1);
					zeroEntries[numFilledEntries,2] = num+1;
					numFilledEntries++;
					filledCurEntry = true;
					break;
				}
			}


			// couldn't fill the entry
			if (!filledCurEntry)
			{
				// if you can't fill the first entry, then puzzle is unsolvable
				if (numFilledEntries == 0)
				{
					return false;
				}
				// if you can't fill the other entries, then backtrack
				else
				{
					zeroEntries[numFilledEntries,2] = 0;
					numFilledEntries--;

					// removes the most recent added entry
					this.currGrid[zeroEntries[numFilledEntries, 0], zeroEntries[numFilledEntries,1]] = 0;

					// reverts isValidTracker and numPossibleValues to the their snapshots at the beginning of brute force
					for (int rowNum = 0; rowNum < 9; rowNum++)
					{
						for (int colNum = 0; colNum < 9; colNum++)
						{
							numPossibleValues[rowNum,colNum] = initPossibleValues[rowNum,colNum];

							for (int entryNum = 0; entryNum < 9; entryNum++)
							{
								isValidTracker[rowNum,colNum][entryNum] = initValidTracker[rowNum,colNum][entryNum];
							}
						}
					}

					// adds the previous entries
					for (int num = 0; num < numFilledEntries; num++)
					{
						addEntry(zeroEntries[num,0], zeroEntries[num,1], zeroEntries[num,2]);
					}
				} // if
			} // if
		} // while

		return true;
	}
}
