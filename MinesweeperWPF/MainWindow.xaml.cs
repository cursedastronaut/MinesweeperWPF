using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
//using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MinesweeperWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public struct int2
		{
			public int x;
			public int y;
		}

		private const int IS_A_MINE = 9;
		private const int PERCENTAGE_OF_BOMB = 10;

		private List<Tuple<string, int, int>> difficulties = new List<Tuple<string, int, int>>();

		private int2 gridSize = new int2();
		private int numberOfBomb = 0;
		private int flagLeft = 0;
		private int flagTotal = 0;
		private int numberOfCellsLeft = 0;
		private List<List<int>> gridValues = new List<List<int>>(); //0: no bomb, 9 = bomb here
		private Boolean firstClick = false;

		public MainWindow()
		{
			InitializeComponent();
			difficulties.Add(new Tuple<string, int, int>("Débutant (9x9)",		9,	9));
			difficulties.Add(new Tuple<string, int, int>("Moyen (16x16)",		16,	16));
			difficulties.Add(new Tuple<string, int, int>("Difficile (16x30)",	16,	30));
			reset();
		}

		private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private UIElement GetUIElementFromPosition(Grid g, int col, int row)
		{
			return g.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
		}

		private void BTN_Play_Click(object sender, RoutedEventArgs e)
		{
			GRD_Menu.Visibility = Visibility.Hidden;

			ListBox difficultyBox = (ListBox)GRD_Menu.Children[0];
			CheckBox debugMode = (CheckBox)GRD_Menu.Children[3];
			gridSize.x = difficulties[(difficultyBox.SelectedIndex == -1 ? 0 : difficultyBox.SelectedIndex)].Item2;
			gridSize.y = difficulties[(difficultyBox.SelectedIndex == -1 ? 0 : difficultyBox.SelectedIndex)].Item3;

			GRDGame.Visibility = Visibility.Visible;
			GRDGame.Children.Clear();
			initGrid();
			generateMine();
			updateUI();
#if DEBUG
			if (debugMode.IsChecked == true)
				turnMinesRed();
#endif
		}

		private void initGrid()
		{
			firstClick = true;
			GRDGame.ColumnDefinitions.Clear();
			for (int i = 0; i < gridSize.x; i++)
			{
				GRDGame.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			}
			for (int i = 0; i < gridSize.y; i++)
			{
				GRDGame.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
			}

			for (int i = 0; i < gridSize.x; i++)
			{
				gridValues.Add(new List<int>());
				for (int j = 0; j < gridSize.y; j++)
				{
					Border b = new Border();
					b.BorderThickness = new Thickness(1);
					b.BorderBrush = new SolidColorBrush(Colors.LightBlue);
					b.SetValue(Grid.RowProperty, j);
					b.SetValue(Grid.ColumnProperty, i);
					Grid grid = new Grid();
					Button button = new Button();
					button.Background = new SolidColorBrush(Colors.AliceBlue);
					button.MouseRightButtonDown += BTN_Flag;
					button.Click += BTN_Discover;

					grid.Children.Add(button);

					b.Child = grid;

					GRDGame.Children.Add(b);

					gridValues[i].Add(0);
				}
			}
		}

		private void BTN_Flag(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			if (button.Content == "🚩")
			{
				flagLeft++;
				button.Content = "";
			} else if (flagLeft > 0) {
				flagLeft--;
				button.Content = "🚩";
			}
		}

		private void BTN_Discover(object sender, RoutedEventArgs e)
		{
			updateUI();
			Button button = (Button)sender;
			if (button.Content == "🚩")
				return;


			//Ici je pars du principe que dans chaque cellule de la grille, j'ai un Border qui contient une grille qui contient mon bouton. 
			Border b = (Border)VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(button));
			int col = Grid.GetColumn(b);
			int row = Grid.GetRow(b);

			Grid tempGrid = (Grid)VisualTreeHelper.GetParent(button);
			tempGrid.Children.Clear();

			if (firstClick) {
				if (gridValues[col][row] == IS_A_MINE)
				{
					bool hasFoundSuitablePlaceForBomb = false;
					while (!hasFoundSuitablePlaceForBomb)
					{
						Random rnd = new Random();
						int colNewMine = rnd.Next() % gridSize.x;
						int rowNewMine = rnd.Next() % gridSize.y;
						hasFoundSuitablePlaceForBomb = gridValues[colNewMine][rowNewMine] == IS_A_MINE;
						if (hasFoundSuitablePlaceForBomb)
							gridValues[colNewMine][rowNewMine] = IS_A_MINE;
					}
					gridValues[col][row] = 0;
				}
				generateMineMap();
				firstClick = false;
			}


			Label label = formatLabelGrid(gridValues[col][row]);
			if (gridValues[col][row] == IS_A_MINE)
			{
				MessageBox.Show("You lost!");
				reset();
			} else {
				numberOfCellsLeft--;
				tempGrid.Children.Add(label);
				bool isZero = gridValues[col][row] == 0;
				gridValues[col][row] = -1;

				//Quad pattern, no diagonal discovery.
				if (isZero)
				{
					discoverCell(col - 1, row);
					discoverCell(col + 1, row);
					discoverCell(col	, row - 1);
					discoverCell(col	, row + 1);
				}
				if (numberOfCellsLeft <= 0)
				{
					MessageBox.Show("You won!");
					reset();
				}
			}
		}

		private Color chooseColor(int level)
		{
			switch (level)
			{
				case 0: return Colors.Black;
				case 1: return Colors.OrangeRed;
				case 2: return Colors.Purple;
				case 3: return Colors.PaleVioletRed;
				case 4: return Colors.MediumVioletRed;
				case 5: return Colors.Red;
				case 6: return Colors.Crimson;
				case 7: return Colors.Red;
				case 8: return Colors.DeepPink;
				default: return Colors.Black;
			}
		}

		private void discoverCell(int col, int row)
		{
			if (col < 0 || row < 0 || col == gridSize.x || row == gridSize.y || gridValues[col][row] == -1) { return; }

			if (gridValues[col][row] == IS_A_MINE) return;

			numberOfCellsLeft--;
			Border border = (Border)GetUIElementFromPosition(GRDGame, col, row);
			Grid tempGrid = (Grid)border.Child;

			tempGrid.Children.Clear();

			Label label = formatLabelGrid(gridValues[col][row]);
			tempGrid.Children.Add(label);
			bool isZero = gridValues[col][row] == 0;
			gridValues[col][row] = -1;

			if (isZero)
			{
				discoverCell(col-1, row);
				discoverCell(col  , row-1);
				discoverCell(col+1, row);
				discoverCell(col  , row+1);
			}
		}

		private void generateMineMap() {
			//Generate mine map
			for (int col = 0; col < gridSize.x; col++)
			{
				for (int row = 0; row < gridSize.y; row++)
				{
					if (gridValues[col][row] == IS_A_MINE) { continue; }
					for (int offsetCol = (col == 0 ? 0 : -1); offsetCol <= ((col == gridSize.x - 1) ? 0 : 1); ++offsetCol)
					{
						for (int offsetRow = (row == 0 ? 0 : -1); offsetRow <= ((row == gridSize.y - 1) ? 0 : 1); ++offsetRow)
						{
							if (gridValues[col + offsetCol][row + offsetRow] == IS_A_MINE)
							{
								gridValues[col][row]++;
							}
						}
					}
				}
			}
		}

		private void generateMine()
		{
			//Generate mines
			numberOfBomb = (gridSize .x* gridSize.y) / PERCENTAGE_OF_BOMB;
			flagTotal = numberOfBomb;
			flagLeft = flagTotal;
			numberOfCellsLeft = gridSize .x* gridSize.y - numberOfBomb;
			for (int i = 0; i < numberOfBomb; ++i)
			{
				Random rnd = new Random();
				gridValues[rnd.Next() % gridSize.x][rnd.Next() % gridSize.y] = IS_A_MINE; //Setting a mine
			}
		}

#if DEBUG
		private void turnMinesRed()
		{
			for (int i = 0; i < gridSize.x; i++)
			{
				for (int j = 0; j < gridSize.y; j++)
				{
					if (gridValues[i][j] != IS_A_MINE) continue;
					Border border = (Border)GetUIElementFromPosition(GRDGame, i, j);
					Grid tempGrid = (Grid)border.Child;
					Button button = (Button)tempGrid.Children[0];
					BrushConverter bc = new BrushConverter();
					button.Background = (Brush)bc.ConvertFrom("#FF0000");
				}
			}
			LBL_UI.Content = numberOfCellsLeft;
		}
#endif

		private void updateUI()
		{
			LBL_UI.Content = "Cases restantes: " + numberOfCellsLeft + "/" + (gridSize.x * gridSize.y - numberOfBomb)
					+ "   Drapeaux: " + flagLeft + "/" + flagTotal + "   ";
		}

		private void reset()
		{
			//Reset game variables
			gridSize.x			= 0;
			gridSize.y			= 0;
			numberOfBomb		= 0;
			flagLeft			= 0;
			flagTotal			= 0;
			numberOfCellsLeft	= 0;
			gridValues = new List<List<int>>(); //0: no bomb, 9 = bomb here
			firstClick = false; //Shouldn't be an issue since it would be false after first click, just in case I resetted it.

			//Hiding the game, and showing Menu
			GRDGame.Visibility = Visibility.Hidden;
			GRD_Menu.Visibility = Visibility.Visible;

			BrushConverter bc = new BrushConverter();

			//Listbox for game difficulty
			ListBox list = new ListBox();
			list.Height		= 128;
			list.Width		= 128;
			list.Margin = new Thickness(0, 128, 0, 0);
			list.Background = (Brush)bc.ConvertFrom("#77C9E5FF");
			list.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");

			foreach (var difficulty in difficulties)
			{
				list.Items.Add(difficulty.Item1);
			}

			//Button to play
			Button play = new Button();
			play.Margin = new Thickness(128, 256+64, 128, 0);
			play.Content = "Jouer";
			play.Click += BTN_Play_Click;
			play.Background = (Brush)bc.ConvertFrom("#77C9E5FF");
			play.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");

			//Title of the game
			Label title = new Label();
			title.Content = "Minesweeper";
			title.FontSize = 48;
			title.Foreground = new SolidColorBrush(Colors.White);
			title.HorizontalContentAlignment = HorizontalAlignment.Center;
			title.Height = 64;
			title.Margin = new Thickness(0, -128, 0, 0);

#if DEBUG
			//Checkbox to enable DebugMode
			CheckBox checkBox = new CheckBox();
			checkBox.Content = "Debug Mode";
			checkBox.Width = 128;
			checkBox.Height = 16;
			checkBox.Margin = new Thickness(0, -16, 0, 0);
#endif

			GRD_Menu.Children.Add(list);
			GRD_Menu.Children.Add(play);
			GRD_Menu.Children.Add(title);
#if DEBUG
			GRD_Menu.Children.Add(checkBox);
#endif

		}

		private Label formatLabelGrid(int value)
		{
			Label label = new Label();
			label.Content = value;
			label.Foreground = new SolidColorBrush(chooseColor(value));
			label.HorizontalAlignment = HorizontalAlignment.Center;
			label.VerticalContentAlignment = VerticalAlignment.Center;
			return label;
		}

	}
}
