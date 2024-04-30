# Lab6 - The dynamic minesweeper

In this lab, we'll use code to add and remove controls to make our application dynamic, and we'll use Layouts to manage their automatic positioning.

# Work to hand in
At the end of the three sessions (a total of 6 hours' work), you'll be asked to submit your project using **travo**.

## The topic

Advancing in your studies is like advancing through a minefield. You have to take your time to analyze your environment, in order not to rush to the wrong answer. It's an important lesson, and that's why the IUT management wants to get it into your head by creating a minesweeper game.

### A minesweeper?

Here are some examples of games for the Minesweeper, which was a classic Windows game in previous versions. It is no longer integrated, but there are many alternative versions:

<img src="./img/minesweeper1.png" height="200"/>
<img src="./img/minesweeper2.png" height="200"/>
<img src="./img/minesweeper3.png" height="200"/>

#### The aim of the game is as follows:  
The player plays on a grid whose cells are all hidden at start-up (some of which may contain bombs) and must reveal all cells that do not contain bombs. In some versions, this is supposed to be done as fast as possible.
 
<img src="./img/demoMS1.JPG" width="30%"/>

To do this, they can click on a cell to reveal its contents. If the clicked cell contains a bomb, the game is immediately lost.

<img src="./img/demoMS2.JPG" width="30%"/>

If the clicked cell did not contain a bomb, then the game can continue and part of the grid is uncovered by performing the following check:
- If the clicked cell (which didn't contain a bomb) has at least one bomb in its immediate vicinity (one cell around in all directions, even diagonally), then this cell reveals the number of bombs in this immediate vicinity and we stop there.
- On the other hand, if the clicked cell has no bombs in its immediate vicinity, then it reveals an empty square, and the game checks its immediate neighbors as well. From neighbor to neighbor, if many of the cells visited are empty and not surrounded by bombs, then a large part of the grid can be revealed with a single click.

<img src="./img/demoMS3.JPG" width="30%"/>

There's a bit of luck involved at the start, as there are no clues as to where to click, but once numbers have been revealed, the aim of the game is to use these numbers to deduce where the bombs are likely to be.

<img src="./img/demoMS3.JPG" width="30%"/>
<img src="./img/demoMS4.JPG" width="30%"/>

From deduction to deduction, we'll try to find all the cells (empty and numbered) that don't contain bombs.
 
## Objectives :

Your application must allow you to manipulate certain parameters before starting a new game. In particular, the user needs to be able to choose the size of the grid (you can use square grids only, for simplicity) and the number of bombs.

There are two major challenges in this job and you'll need to find information in the documentation / on the Internet to handle them : 

- The first is to correctly implement the dynamic creation of controls. The idea is that, at the start of each game, you create a new grid of controls. You'll need to do this from the code and you'll have to assign to these dynamically created controls the event-driven procedure you'll write, which will call the game's algorithms.
- The second challenge involves the algorithms that allow the game to function correctly. In particular, the algorithm that traverses the neighbors of a clicked cell is not trivial. This is a recursive algorithm, i.e. one that calls itself. You haven't yet seen this type of algorithm, but we'll give you a few hints later in this document.

As you can see, there are 3 sessions for this Lab, as there are several difficulties. Try to set objectives for each session. For example:
- 1st session: have a dynamic grid of controls when the application starts up, as well as the random placement of bombs in the game (without the game logic yet).
- 2nd session: have the main game algorithms implemented.
- 3rd session: get the menu for configuring the grid size, the number of bombs and restarting a new game. You can also customize your game with your own colors and animations!

## (1) Retrieve the project using **travo**
<details>
    <summary>See instructions here to retrieve and submit the project (important).</summary> 

> Pour récupérer le projet et le soumettre à la fin des deux séances, vous allez devoir utiliser le script **travo** fourni par le responsable du module. Télécharger ce script [**travoIHM.py**](https://ihm.gitpages.iut-orsay.fr/cours/travoIHM.py) dans votre espace personnel (quelque part dans le lecteur Z:).
> 
> > **travo** est un ensemble de scripts Python maintenu par des enseignants chercheurs de Paris-Saclay et du Québec facilitant l'utilisation de GIT pour les enseignants. En fait les commandes **travo** effectuent un ensemble de commande GIT pour vous. **travo** ainsi que Python sont déjà installés sur les ordinateurs de l'IUT.
> > 
> > Vous pouvez utiliser votre propre ordinateur et installer **travo** dessus (à condition d'avoir installé Python au préalable bien sûr) à l'aide de la commande: 
> > ```
> > pip install travo
> > ```
> 
> Sur l'ordinateur de l'IUT, rendez-vous dans le répertoire C:\WinPython et lancer le programme "WinPython Powershell Prompt.exe", un terminal Powershell va s'ouvrir, prêt à recevoir des commandes Python.
> A l'intérieur de ce terminal PowerShell, tapez d'abord la commande suivante, vous permettant de vous déplacer dans > votre espace personnel :
> 
> ```
> cd Z:\
> ```
> 
> Si vous tapez la commande **ls**, vous devriez voir dans la liste des fichiers de ce répertoire, le script **travoIHM.py** que vous avez téléchargé précédemment (bien sûr, vous pouvez travailler dans un autre répertoire que la racine du répertoire Z:).
> 
> Enfin, pour récupérer le projet, il vous suffit de taper la commande :
> 
> ```
> python travoIHM.py fetch tpihm6
> ```
> 
> Il vous sera demandé vos identifiants ADONIS (de l'IUT) puis le projet sera téléchargé sur votre ordinateur (dans le répertoire "tpihm6"). 
> 
> Sauvegarder ou soumettre votre travail à l'enseignant se fera à l'aide de la commande : 
> ```
> python travoIHM.py submit tpihm6 ####
> ```
> Il vous faut remplacer #### par votre identifiant de groupe **tp2a** ou **tp2b** etc... Ne vous trompez pas de groupe, ce sera des points en moins
> 
> Vous pouvez faire autant de "submit" que vous voulez. C'est une bonne pratique pour ne pas perdre votre travail.
>
> > ### Ajouter un fichier à un projet récupéré
> > 
> > La plupart des projets que vous allez récupérer avec travo contient a priori tous les fichiers dont vous avez besoin. Cependant il est possible que vous ayez besoin d'ajouter des fichiers (des images, des sons, des classes, que sais-je...) à vos projets, afin qu'ils se retrouvent sur le Git.
> > 
> > **travo** ne regarde pas s'il y a de nouveaux fichiers dans vos projets.
> > 
> > Cependant, vous pouvez tout à fait utiliser les commandes **git add** que vous avez dû voir en cours de [Qualité de développement](https://hoangla95.github.io/qualitedevs2/tp1) en vous plaçant dans le répertoire du projet.
> >
> > Une fois les fichiers ajoutés avec la commande **git add**, travo les prendra en compte lors du **submit**.

</details>

## Conseils

Mon premier conseil est de mettre de côté les algorithmes du jeu pour commencer. Concentre toi pour faire apparaitre une grille de boutons, et faire en sorte que lorsque tu cliques sur chaque bouton, cela affiche sa coordonnée dans un MessageBox. Pour la première séance, c'est très bien!

### Attributs

Voici quelques éléments qui peuvent t'aider pour ton application. Tout d'abord, tu auras surement besoin d'ajouter quelques attributs à ta classe qui te permettront de suivre l'état de la partie :
```
private int gridSize = 10;
private int numMine = 10;
private int numCellOpen = 0;
private int[,] matrix;
```
La matrice à deux dimensions sert à conserver les valeurs de chaque case:
- -1, cette case contient une bombe
- 0, cette case est vide et aucune case voisine ne contient une bombe
- n, avec n>0, cette case est vide et il y a n cases voisines autour qui contiennent une bombe

Tu pourrais stocker ces valeurs directement dans les contrôles que tu vas ajouter à ton application (dans la propriété "Content" du "Label" présent dans la grille par exemple), mais tu verras que c'est plus pratique d'utiliser une structure dédiée à laquelle tu pourras te référer depuis n'importe où dans le code.

### Procédure d'initialisation
Tu vas avoir besoin d'écrire une procédure qui réinitialise le jeu. Cette procédure doit notamment :
- Réinitialiser la grille et remettre les compteurs à zéro
- Remplir la grille des contrôles dont tu as besoin
- Placer aléatoirement des bombes en mettant à jour la matrice

Le début de cette procédure de réinitialisation devrait ressembler à ça (on considère que l'on a la grille GRDGame qui a été ajoutée dans le XAML) :
```
matrix = new int[gridSize, gridSize];
numCellOpen = 0;
GRDGame.Children.Clear();
GRDGame.ColumnDefinitions.Clear();
GRDGame.RowDefinitions.Clear();
for (int i = 0; i < gridSize; i++)
{
    GRDGame.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
    GRDGame.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
}
```
Observe comment la grille est vidée de ses enfants, de ses définitions de lignes et de colonnes et comment nous réinitialisons les définitions de lignes et de colonnes.

### Remplir la grille

Ensuite, tu vas avoir besoin de faire une boucle qui, à chaque tour de boucle, va créer le contenu d’une cellule et l’ajouter à la <span style="color:green">grille</span>. Il s’agit en réalité d’une double boucle puisque tu vas devoir boucler sur les colonnes (i) et sur les lignes (j).

```
Border b = new Border();
b.BorderThickness = new Thickness(1);
b.BorderBrush = new SolidColorBrush(Colors.LightBlue);
b.SetValue(Grid.RowProperty, j);
b.SetValue(Grid.ColumnProperty, i);
GRDGame.Children.Add(b);
```
Dans l'image en dessous, je te montre un exemple. La grille GRDGame est en vert et elle fait 2x2. Dans chaque cellule de cette grille, nous metterions un "b", un contrôle de type <span style="color:purple">Border</span> (en violet dans l'image en dessous) mais ça ne suffira pas. Il te faudra également ajouter à chaque "b", une nouvelle <span style="color:green">grille</span> (en vert) contenant un nouveau Label (en blanc, mais caché) et un nouveau <span style="color:blue">Button</span> (en bleu). Au départ, le Label sera invisible et seul le Button sera visible. En cliquant sur le Button, la procédure événementielle qui contrôle la logique du jeu sera déclenchée. Cette procédure devra (entre autres) rendre le Button invisible et de révèler le Label et sa valeur (si la valeur est nulle, elle devra également se déclencher sur ses voisins etc...).

<img src="./img/minesweeperTuto.png" width="60%"/>

Pour cela, il faudra bien penser à assigner à ce Button, juste après l'avoir instancié, la procédure événementielle qui réalise cette logique du jeu.

### Position dans la grille

Afin de réaliser l'application, tu auras parfois besoin d'accéder à un contrôle situé précisement à une coordonnée de la grille, ou alors de récupérer les coordonnées d'un contrôle.

```
Button button = (Button)sender;
//Ici je pars du principe que dans chaque cellule de la grille, j'ai un Border qui contient une grille qui contient mon bouton. 
Border b = (Border)VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(button));
int col = Grid.GetColumn(b);
int row = Grid.GetRow(b);
```

En cadeau, une fonction qui retourne le contrôle situé dans une grille à des coordonnées données :
```
private UIElement GetUIElementFromPosition(Grid g, int col, int row)
{
    return g.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
}
```

### Pseudo-code de l'algorithme récursif

Le gros morceau du démineur est son algorithme récursif, cela veut dire que nous avons une méthode qui s'appelle elle-même. Nous appelerons cette fonction verifieCellule dans la suite du document.

Je vais te présenter le principe générale de cette fonction.

Ici, on imagine que j’ai cliqué sur le bouton situé aux coordonnées (column, row). Je dois donc appeler cette procédure verifieCellule qui va vérifier s’il y avait une bombe à ces coordonnées. 
- S’il y a une bombe, alors l’algorithme s’arrête et la partie est perdue. 
- S’il n’y en a pas, je dois enlever le bouton (le rendre invisible) afin d’afficher le nombre de bombes voisines (en rendant visible le Label et en mettant le nombre correspondant depuis la matrice).
    - Si ce nombre est supérieur à zéro, alors on s'arrête là.
    - Si ce nombre est égale à zéro, je dois alors rappeler verifieCellule sur toutes les cases voisines ! C’est ici que se trouve l’aspect récursif de cette fonction !

Tu feras attention au moment de vérifier une cellule voisine que ses coordonnées ne soient pas en dehors de la grille (le pseudo-code te montre comment faire, la fonction Max renvoie la plus grande valeur entre 2 valeurs et la fonction Min renvoie la plus petite valeur entre 2 valeurs, ces deux fonctions existent en C# dans la bibliothèque Math).

#### Pseudo-Code 
```
fonction Booleen verifieCellule(entier column, entier row)
{
    SI la case n’a pas déjà été vérifiée (le bouton est toujours visible/actif)
    ALORS {
        On cache/désactive le bouton et on affiche la valeur de cette cellule
        SI la case est une bombe 
        ALORS{ partie perdue et on réinitialise le jeu; retourne VRAI }
        SINON{ 
            SI c’était la dernière case à ouvrir 
            ALORS{ partie gagnée et on réinitialise le jeu ; retourne VRAI}
            SINON{
                //On vérifie la valeur de cette cellule
                SI matrice[column,row] est égale à 0 (pas de bombes autour) 
                ALORS{	
                    // la procédure s’appelle ensuite elle-même sur les cellules voisines
                    POUR i de Max(0, column-1) à Min(tailleGrille -1, column+1) {
                        POUR j de Max(0, row-1) à Min(tailleGrille -1, row+1){
                            Booleen resultat = verifieCellule(i,j)
                            SI resultat est égal à VRAI
                            ALORS{ retourne VRAI }
                        }
                    }
                }
            }
        }
    }
    retourne FAUX
}
```
### Remettre ton travail
N'oublie pas de soumettre ton travail à l'enseignant avec la commande **travo** 
```
python travoIHM.py submit tpihm6 ####
```
> en remplaçant #### par ton identifiant de groupe **tp2a** ou **tp2b** etc... Encore une fois, ne te trompe pas de groupe...