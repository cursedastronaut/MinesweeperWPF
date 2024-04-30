# Lab6 - The dynamic minesweeper

Dans ce TP, nous allons manipuler depuis le code l'ajout et la suppression de contrôles afin de rendre notre application dynamique, et nous allons nous appuyer sur les Layouts pour gérer leur positionnement automatique.

## Travail à rendre
A la fin des trois séances (un total de 6 heures de travail), vous devrez soumettre votre projet à l'aide de travo.

## Le sujet

Avancer dans les études, c'est comme avancer dans un champ de mines. Il faut prendre son temps pour analyser son environnement afin de ne pas se précipiter sur une mauvaise réponse. C'est une leçon importante, c'est pourquoi la direction de l'IUT souhaite vous la faire rentrer dans la tête en réalisant un jeu de démineur.

### Le démineur?

Voici des exemples de jeux du démineur, qui était un jeu classique intégré de Windows dans ses précédentes versions. Celui-ci n’est plus intégré mais on trouve beaucoup de versions alternatives:

<img src="./img/minesweeper1.png" height="200"/>
<img src="./img/minesweeper2.png" height="200"/>
<img src="./img/minesweeper3.png" height="200"/>

#### Le but du jeu est le suivant :  
Le joueur joue sur une grille dont les cellules sont toutes masquées au démarrage (et dont certaines peuvent contenir des bombes) et doit révéler toutes les cellules qui ne contiennent pas de bombes. Dans certaines versions, on considère qu’il doit le faire le plus vite possible.
 
<img src="./img/demoMS1.JPG" width="30%"/>

Pour faire cela, il peut cliquer sur une cellule afin de révéler son contenu. Si la cellule cliquée contenait une bombe, la partie est immédiatement perdue.

<img src="./img/demoMS2.JPG" width="30%"/>

Si la cellule cliquée ne contenait pas de bombe, alors la partie peut continuer et la grille se découvre en réalisation la vérification suivante :
- Si la cellule cliquée (qui ne contenait pas de bombe donc) possède au moins une bombe dans son voisinage immédiat (une case autour dans toutes les directions, même les diagonales) alors cette cellule révèle le nombre de bombes dans ce voisinage immédiat et on s’arrête là.
- En revanche, si la cellule cliquée n’a aucune bombe dans ses voisins immédiats, alors celle-ci révèle une case vide, et le jeu va effectuer la vérification sur ses voisins immédiats également. De voisin en voisin, si beaucoup de cases visitées sont vides et non entourées de bombes, on peut révéler en un click un grand morceau de la grille

<img src="./img/demoMS3.JPG" width="30%"/>

Il y a un peu de chance à avoir au démarrage car on a aucun indice pour savoir où cliquer, mais une fois que l’on a révélé des nombres, le but du jeu est d’utiliser ces nombres pour déduire où sont probablement les bombes.

<img src="./img/demoMS3.JPG" width="30%"/>
<img src="./img/demoMS4.JPG" width="30%"/>

De déduction en déduction, on va essayer de découvrir toutes les cellules (vides et numérotées) qui ne contiennent pas de bombes.
 

## Objectifs :

Ton application doit permettre de pouvoir manipuler certains paramètres avant de lancer une nouvelle partie. En particulier, on doit pouvoir choisir la taille de la grille (tu peux utiliser que des grilles carrées pour simplifier) et le nombre de bombes.

Attention, il y a deux défis majeurs dans ce travail. Tu auras besoin d’aller chercher de l’information dans la documentation / sur Internet.

- Le premier, c’est de correctement mettre en œuvre la création dynamique de contrôles. L’idée c’est qu’au début de chaque partie, tu créés une nouvelle grille de contrôles. Il te faudra donc le faire depuis le code et il faudra penser à assigner à ces contrôles créés dynamiquement la procédure événementielle que tu écriras et qui fera appel aux algorithmes du jeu.
- Le second, ce sont les algorithmes permettant de faire fonctionner correctement le jeu. En particulier, l’algorithme qui parcourt les voisins d’une cellule cliquée qui n’est pas trivial. Il s’agit d’un algorithme récursif, c’est-à-dire un algorithme qui s’appelle lui-même. Tu n’as pas encore vu ce type d’algorithme mais des aides te sont fournies dans la suite du document.

Comme tu peux le voir, il y a 3 séances pour ce TP, car il y a plusieurs difficultés. Essaye de te donner des objectifs pour chaque séance. Par exemple :
- 1ère séance : avoir une grille dynamique de contrôles au démarrage de l’application ainsi que le placement aléatoire des bombes dans le jeu (sans la logique du jeu encore).
- 2ème séance : avoir les principaux algorithmes de jeu d’implémentés
- 3ème séance : avoir le menu permettant de configurer la taille de la grille, le nombre de bombes et de relancer une nouvelle partie. Profites-en pour personnaliser aussi ton jeu avec tes couleurs ou tes animations!


## (1) Récupérer le projet à l'aide de **travo**
<details>
  <summary>Voir les instructions pour récupérer et soumettre le projet (*important*)</summary> 

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