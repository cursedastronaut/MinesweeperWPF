using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperWPF
{
	internal class EveryString
	{
		public static string TITLE = "Démineur";
		public static string BOMBS = "bombes";
		public static string HELP_CONTENT = "Le jeu \"Démineur\" consiste à " +
			"découvrir toutes les cases où il n'y a pas de bombes. Pour cela, " +
			"vous aurez plusieurs indications:\n" +
			"\t- Lorsque vous cliquez sur une case, elle affichera le nombre de " +
			"mines autour d'elle (clic gauche).\n" +
			"\t- Vous avez des drapeaux que vous pouvez mettre sur les endroits que " +
			"vous souhaitez marquer (clic droit)";
		public static string HELP_TITLE = "Aide";
		public static string MENU_INCORRECT_NUMBER_SIZE = "Nombre incorrect! Taille: ";
		public static string MENU_INCORRECT_NUMBER_BOMBS = ". Bombes: Entre 0 et ";
		public static string MENU_INCORRECT_NUMBER_PERCENT = "% de colonnes x lignes.";
		public static string MENU_INCORRECT_NUMBER_NOTVALID = "Veuillez entrer un nombre valide.";
		public static string MENU_BACK = "Retour";
		public static string MENU_TIMES = "Temps";
		public static string POPUP_QUIT_CONFIRMATION = "Souhaitez-vous quitter le jeu ?";
		public static string POPUP_QUIT_TITLE = "Démineur";
		public static string POPUP_WIN = "Félicitations, vous avez gagné!";
		public static string POPUP_LOST = "Dommage, vous avez perdu...";
		public static string UI_CELLS_LEFT = "Cases restantes: ";
		public static string UI_FLAGS = "Drapeaux: ";
		public static string UI_TIMES = "Temps: ";
		public static string UI_WIN = "Vous avez gagné! Appuyez sur ENTRÉE pour revenir au menu principal.";
		public static string UI_LOST = "Vous avez perdu! Appuyez sur ENTRÉE pour revenir au menu principal.";
		public static string POPUP_ABANDON = "Souhaitez-vous abandonner ?";

		public static string DIFFICULTY_EASY = "Débutant";
		public static string DIFFICULTY_MEDIUM = "Moyen";
		public static string DIFFICULTY_HARD = "Difficile";
		public static string DIFFICULTY_CUSTOM = "Custom";
	}
}
