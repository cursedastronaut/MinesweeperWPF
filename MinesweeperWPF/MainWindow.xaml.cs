﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
//using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

#if DEBUG
		private const bool DEBUG_MODE = true;
#else
		private const bool DEBUG_MODE = false;
#endif
		//CONSTANTS
		private const double	PERCENTAGE_OF_BOMBS_ALLOWED = 0.4;
		private const int		PERCENTAGE_OF_BOMB = 10;
		private const int		IS_A_MINE = 9;
		private const int		MAX_CELLS_FACTOR = 50;
		private const string	LBL_UI_DEFAULT_CONTENT = "Mon Super Démineur";

		//Difficulty list
		private List<Tuple<string, int, int>> difficulties = new List<Tuple<string, int, int>>();

		private int2			gridSize = new int2();
		private int				numberOfBomb = 0;
		private int				flagLeft = 0;
		private int				flagTotal = 0;
		private int				numberOfCellsLeft = 0;
		private List<List<int>>	gridValues = new List<List<int>>(); //0: no bomb, 9 = bomb here
		private bool			firstClick = false;
		private bool			gameDone = false;
		private bool			customBombNumber = false;
		private bool			menuLoadingDone = false;

		public MainWindow()
		{
			InitializeComponent();
			difficulties.Add(new Tuple<string, int, int>("Débutant (9x9)",		9,	9));
			difficulties.Add(new Tuple<string, int, int>("Moyen (16x16)",		16,	16));
			difficulties.Add(new Tuple<string, int, int>("Difficile (16x30)",	16,	30));
			difficulties.Add(new Tuple<string, int, int>("Custom",				0,	0));
			KeyDown += HandleKeyPress;
			reset();
		}

		private void HandleKeyPress(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:		if (gameDone) reset(); break;
				case Key.Escape:
					if (gridSize.x > 0) //Since it gets reset, it's a good way to know whether we're in game or not.
					{
						if (MessageBox.Show("Souhaitez-vous abandonner ?", "Démineur", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
							reset();
					} else {
						if (MessageBox.Show("Souhaitez-vous quitter le jeu ?", "Démineur", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
							Application.Current.Shutdown();
					}
					break;
				default: break;
			}
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
			if (LST_Difficulties.SelectedIndex == (difficulties.Count - 1)) {
				try
				{
					gridSize.x = Int32.Parse(TXT_Columns.Text);
					gridSize.y = Int32.Parse(TXT_Rows.Text);
					customBombNumber = (bool)CHK_CustomBombNumber.IsChecked;
					if (customBombNumber)
						numberOfBomb = Int32.Parse(TXT_Bombs.Text);
					/*
					 I forbid bomb counts over 40% of the total cells number, just like in the Windows XP and Vista version
					 as pseudo-random mine generation may create a seemingly infinite (too long for user) loop, making it
					 looked like the program has crashed.
					 */
					if (
						gridSize.x <= 0 || gridSize.x > MAX_CELLS_FACTOR || gridSize.y <= 0 || gridSize.y > MAX_CELLS_FACTOR
						|| (customBombNumber && numberOfBomb <= 0) || (customBombNumber && (double)numberOfBomb > ((gridSize.x * gridSize.y) * PERCENTAGE_OF_BOMBS_ALLOWED)) //see comment above
						)
						throw new Exception();
				} catch (Exception ex)
				{
					LBL_UI.Content = "Nombre incorrect! Taille: " + (MAX_CELLS_FACTOR + 1) + ". Bombes: Entre 0 et " + ((gridSize.x * gridSize.y) * PERCENTAGE_OF_BOMBS_ALLOWED) + " (colonnes x lignes x 0.4).";
					return;
				}
			} else {
				gridSize.x = difficulties[(LST_Difficulties.SelectedIndex == -1 ? 0 : LST_Difficulties.SelectedIndex)].Item2;
				gridSize.y = difficulties[(LST_Difficulties.SelectedIndex == -1 ? 0 : LST_Difficulties.SelectedIndex)].Item3;
			}

			GRD_Menu.Visibility = Visibility.Hidden;
			GRDGame.Visibility = Visibility.Visible;
			GRDGame.Children.Clear();
			initGrid();
			generateMine();
			updateUI();
#if DEBUG
			//Not casting will cause an error.
			if ((bool)CHK_DebugMode.IsChecked)
				turnMinesRed();
#endif
		}

		private void initGrid()
		{
			firstClick = true;
			GRDGame.ColumnDefinitions.Clear();
			GRDGame.RowDefinitions.Clear();
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
			if (gameDone) { }
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

			//It's a common rule in Minesweeper games to never be able to get blown up on your first click
			//so we move the bomb somewhere else after the first click
			if (firstClick) {
				if (gridValues[col][row] == IS_A_MINE)
				{
					bool hasFoundSuitablePlaceForBomb = false;
					while (!hasFoundSuitablePlaceForBomb)
					{
						Random rnd = new Random();
						int colNewMine = rnd.Next() % gridSize.x;
						int rowNewMine = rnd.Next() % gridSize.y;
						hasFoundSuitablePlaceForBomb = gridValues[colNewMine][rowNewMine] != IS_A_MINE;
						if (hasFoundSuitablePlaceForBomb)
							gridValues[colNewMine][rowNewMine] = IS_A_MINE;
					}
					gridValues[col][row] = 0;
#if DEBUG
					if ((bool)CHK_DebugMode.IsChecked)
						MessageBox.Show("DEBUG: You clicked on a mine on your first click. "
							+ "Just like in the original game, it was then moved somewhere else"
							+ ". This message only appears in Debug Mode.", "Debug Mode");
#endif
				}
				generateMineMap();
				firstClick = false;
			}


			Label label = formatLabelGrid(gridValues[col][row]);
			if (gridValues[col][row] == IS_A_MINE)
			{
				gameDone = true;
				tempGrid.Children.Add(getMineImage());
				LBL_UI.Content = "Vous avez perdu! Appuyez sur ENTRÉE pour revenir au menu principal.";
				MessageBox.Show("Dommage, vous avez perdu...", "Démineur");
				discoverAllTiles();
			}
			else {
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
					LBL_UI.Content = "Vous avez gagné! Appuyez sur ENTRÉE pour revenir au menu principal.";
					MessageBox.Show("Félicitations, vous avez gagné!", "Démineur");
					discoverAllTiles();
					gameDone = true;
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

		//Puts in each tile the number of mines around it.
		private void generateMineMap() {
			//Generate mine map
			for (int col = 0; col < gridSize.x; col++)
				for (int row = 0; row < gridSize.y; row++)
					if (gridValues[col][row] != IS_A_MINE)
						for (int offsetCol = (col == 0 ? 0 : -1); offsetCol <= ((col == gridSize.x - 1) ? 0 : 1); ++offsetCol)
							for (int offsetRow = (row == 0 ? 0 : -1); offsetRow <= ((row == gridSize.y - 1) ? 0 : 1); ++offsetRow)
								if (gridValues[col + offsetCol][row + offsetRow] == IS_A_MINE)
									gridValues[col][row]++;

		}

		//Generate mines
		private void generateMine()
		{
			if (!customBombNumber)
				numberOfBomb = (gridSize .x* gridSize.y) / PERCENTAGE_OF_BOMB;

			flagTotal = numberOfBomb;
			flagLeft = flagTotal;
			numberOfCellsLeft = gridSize .x* gridSize.y - numberOfBomb;
			for (int i = 0; i < numberOfBomb; ++i)
			{
				bool hasFoundSuitablePlaceForBomb = false;
				while (!hasFoundSuitablePlaceForBomb)
				{
					Random rnd = new Random();
					int colNewMine = rnd.Next() % gridSize.x;
					int rowNewMine = rnd.Next() % gridSize.y;
					hasFoundSuitablePlaceForBomb = gridValues[colNewMine][rowNewMine] != IS_A_MINE;
					if (hasFoundSuitablePlaceForBomb)
						gridValues[colNewMine][rowNewMine] = IS_A_MINE;
				}
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
			LBL_UI.Content = LBL_UI_DEFAULT_CONTENT;

			//Reset game variables
			gameDone			= false;
			customBombNumber	= false;
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

			GRDGame.Children.Clear();
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

		private void GRD_Menu_Loaded(object sender, RoutedEventArgs e)
		{
			//Listbox for game difficulty
			foreach (var difficulty in difficulties)
			{
				LST_Difficulties.Items.Add(difficulty.Item1);
			}
			menuLoadingDone = true;
		}

		private void LST_Difficulties_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//If User selected last option, which is Custom
			if (LST_Difficulties.SelectedIndex == (difficulties.Count-1))
			{
				CHK_CustomBombNumber	.Visibility = Visibility.Visible;
				TXT_Columns				.Visibility = Visibility.Visible;
				TXT_Rows				.Visibility = Visibility.Visible;
				TXT_Bombs				.Visibility = Visibility.Visible;
				LBL_Columns				.Visibility = Visibility.Visible;
				LBL_Rows				.Visibility = Visibility.Visible;
				LBL_Bombs				.Visibility = Visibility.Visible;
				
			} else {
				CHK_CustomBombNumber	.Visibility = Visibility.Hidden;
				TXT_Columns				.Visibility = Visibility.Hidden;
				TXT_Rows				.Visibility = Visibility.Hidden;
				TXT_Bombs				.Visibility = Visibility.Hidden;
				LBL_Columns				.Visibility = Visibility.Hidden;
				LBL_Rows				.Visibility = Visibility.Hidden;
				LBL_Bombs				.Visibility = Visibility.Hidden;
			}
			checkCustomValues(null, null);
		}

		//Hides the debug mode checkbox if it's not debug mode. Even if someone were to hack the binaries
		//or the RAM to make it reappear, the functions are not compiled into Release mode, so the game
		//would continue without caring.
		private void hidingDebugCheckbox(object sender, RoutedEventArgs e)
		{
			if (!DEBUG_MODE) CHK_DebugMode.Visibility = Visibility.Hidden;
		}

		private Image getMineImage()
		{
			Image image = new Image();
			BitmapImage bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\mine.png"));
			image.Source = bitmap;
			return image;
		}

		//Discovers all tiles upon death or win.
		private void discoverAllTiles()
		{
			for (int col = 0; col < gridSize.x; col++)
			{
				for(int row = 0; row < gridSize.y; row++)
				{
					if (gridValues[col][row] != -1)
					{
						Border border = (Border)GetUIElementFromPosition(GRDGame, col, row);
						Grid tempGrid = (Grid)border.Child;

						tempGrid.Children.Clear();
						if (gridValues[col][row] == IS_A_MINE)
						{
							tempGrid.Children.Add(getMineImage());

						} else {
							Label label = formatLabelGrid(gridValues[col][row]);
							tempGrid.Children.Add(label);
						}
					}

				}

			}
		}

		private void checkCustomValues(object sender, RoutedEventArgs e)
		{
			if (!menuLoadingDone) return;
			if (LST_Difficulties.SelectedIndex != (difficulties.Count - 1))
			{
				BTN_Play.IsEnabled = true;
				LBL_UI.Content = LBL_UI_DEFAULT_CONTENT;
				return;
			}

			TXT_Bombs.IsEnabled = CHK_CustomBombNumber.IsChecked == true;

            /*
			I forbid bomb counts over 40% of the total cells number, just like in the Windows XP and Vista version
			as pseudo-random mine generation may create a seemingly infinite (too long for user) loop, making it
			looked like the program has crashed.
			*/
            try
			{
				BTN_Play.IsEnabled = !(
						Int32.Parse(TXT_Columns.Text) <= 0 || Int32.Parse(TXT_Columns.Text) > MAX_CELLS_FACTOR || Int32.Parse(TXT_Rows.Text) <= 0 || Int32.Parse(TXT_Rows.Text) > MAX_CELLS_FACTOR
						|| ((bool)CHK_CustomBombNumber.IsChecked && Int32.Parse(TXT_Bombs.Text) <= 0)
						|| ((bool)CHK_CustomBombNumber.IsChecked && ((double)Int32.Parse(TXT_Bombs.Text) > ((Int32.Parse(TXT_Columns.Text) * Int32.Parse(TXT_Rows.Text)) * PERCENTAGE_OF_BOMBS_ALLOWED))) //see comment above
				);
				LBL_UI.Content = "Nombre incorrect! Taille: " + (MAX_CELLS_FACTOR) + ". Bombes: Entre 0 et " + (PERCENTAGE_OF_BOMBS_ALLOWED * 100) + "% de colonnes x lignes.";
			} catch (Exception ex) {
				LBL_UI.Content = "Veuillez entrer un nombre valide.";
				BTN_Play.IsEnabled = false;
			}

			if (BTN_Play.IsEnabled)
				LBL_UI.Content = LBL_UI_DEFAULT_CONTENT;

		}


	}
}
