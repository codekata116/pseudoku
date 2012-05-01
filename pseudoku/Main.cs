using System;
using System.IO;

namespace pseudoku
{
	public class MainClass
	{
		public static int Main (string[] args)
		{
			/*
			 * Ensure a file has been provided.
			 * Provides usage information if no parameters are provided.
			 */
			#region Usage Information
			if (args.Length <= 0) 
			{
				Console.WriteLine("Error: no file provided.");
				return(-1);
			}
			#endregion
			
			/* 
			 * File stream into an array.
			 * Stores stream as char, then converts to int.
			 * Assumes values are delimited by ' '.
			 */
			#region Retrieves Sudoku Grid From File
			int[,] intGrid = new int[9,9];
			
			// Store stream as char
			using (StreamReader gridReader = new StreamReader(args[0]))
			{
				String input;
				int rowNum = 0;
				
				for (;;)
				{
					// Read next line
					input = gridReader.ReadLine();
					
					// End of file; stop reading the file
					if (input == null) break;
					
					// Assert (number of rows <= 9).
					// Note: rowNum = [0,8] b/c arrays are 0-indexed
					if (rowNum >= 9)
					{
						Console.WriteLine("Error: too many rows in the grid.");
						return(-1);
					}
					
					// Split the current line
					string[] stringGrid = input.Split(' ');
					
					// Assert (number of columns = 9) for current row
					if (stringGrid.Length > 9)
					{
						Console.WriteLine("Error: too many columns in row {0}.", rowNum + 1);
						return(-1);
					}
					else if (stringGrid.Length < 9)
					{
						Console.WriteLine("Error: not enough columns in row {0}.", rowNum + 1);
						return(-1);
					}
					
					// Store current line as int
					for (int colNum = 0; colNum < 9; colNum++)
					{
						if (!int.TryParse(stringGrid[colNum], out intGrid[rowNum,colNum]))
						{
							// Character in current cell is not an integer
							Console.WriteLine("Error: invalid value. \""
												+ stringGrid[colNum]
												+ "\" found at row "
												+ rowNum
												+ " and at column "
												+ colNum
												+ "."
												);
							return(-1);
						}
					}
					
					// Move to the next row of grid
					rowNum ++;
				}
				
				// Assert rowNum = 9
				if (rowNum < 9)
				{
					Console.WriteLine("Error: not enough rows in the grid.");
					return(-1);
				}
				
			}
			#endregion
			
			#region Create Sudoku Object
			// Create the new Sudoku object
			Sudoku puzzle = new Sudoku();
			
			// Stores any possible error messages
			string errorMessage;
			
			// Adds the grid to the Sudoku object
			if (!puzzle.addGrid(intGrid,out errorMessage))
			{
				Console.WriteLine(errorMessage);
				return (-1);
			}
			#endregion
		
			#region Attempts to Solve Sudoku puzzle
			if (puzzle.solve())
			{
				int[,] finalGrid = puzzle.getCurrentGrid();

				Console.WriteLine("Solution:");				
				for (int rowNum = 0; rowNum < 9; rowNum ++)
				{
					for (int colNum = 0; colNum < 9; colNum ++)
					{
						Console.Write(finalGrid[rowNum,colNum]);
						Console.Write(" ");
					}
					Console.WriteLine();
				}
			}
			else
			{
				Console.WriteLine("Invalid Sudoku puzzle. Unable to solve puzzle.");
			}
			#endregion

			return(0);
		}
		
		private void showUsage()
		{
			Console.WriteLine("This is the usage details - not implemented");	
		}
	}
}

