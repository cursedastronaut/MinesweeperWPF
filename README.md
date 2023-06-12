# TPIHM6 - Dynamic

Dans ce TP, nous allons manipuler depuis le code l'ajout et la suppression de contrôles afin de rendre notre application dynamique, et nous allons nous appuyez sur les Layouts pour gérer leur positionnement automatique.

## Travail à rendre
A la fin des deux séances (un total de 4 heures de travail), vous devrez soumettre votre projet à l'aide de travo.

## Le sujet

Avancer dans les études, c'est comme avancer dans un champ de mines. Il faut prendre son temps pour analyser son environnement afin de ne pas se précipiter sur une mauvaise réponse. C'est une leçon importante, c'est pourquoi la direction de l'IUT souhaite vous la faire rentrer dans la tête en réalisant un jeu de démineur.

### Le démineur?

Voici des exemples de jeux du démineur, qui était un jeu classique intégré de Windows dans ses précédentes versions. Celui-ci n’est plus intégré mais on trouve beaucoup de versions alternatives:

<img src="./img/minesweeper1.png" width="30%"/>
<img src="./img/minesweeper2.png" width="30%"/>
<img src="./img/minesweeper3.png" width="30%"/>

#### Le but du jeu est le suivant :  
Le joueur joue sur une grille dont les cellules sont toutes masquées au démarrage (et dont certaines peuvent contenir des bombes) et doit révéler toutes les cellules qui ne contiennent pas de bombes. Dans certaines versions, on considère qu’il doit le faire le plus vite possible.
 
Pour faire cela, il peut cliquer sur une cellule afin de révéler son contenu. Si la cellule cliquée contenait une bombe, la partie est immédiatement perdue. 
 
Si la cellule cliquée ne contenait pas de bombe, alors la partie peut continuer et la grille se découvre en réalisation la vérification suivante :
- Si la cellule cliquée (qui ne contenait pas de bombe donc) possède au moins une bombe dans son voisinage immédiat (une case autour dans toutes les directions, même les diagonales) alors cette cellule révèle le nombre de bombes dans ce voisinage immédiat et on s’arrête là.
- En revanche, si la cellule cliquée n’a aucune bombe dans ses voisins immédiats, alors celle-ci révèle une case vide, et le jeu va effectuer la vérification sur ses voisins immédiats également. De voisin en voisin, si beaucoup de cases visitées sont vides et non entourées de bombes, on peut révéler en un click un grand morceau de la grille
 
Il y a un peu de chance à avoir au démarrage car on a aucun indice pour savoir où cliquer, mais une fois que l’on a révélé des nombres, le but du jeu est d’utiliser ces nombres pour déduire où sont probablement les bombes.
  
De déduction en déduction, on va essayer de découvrir toutes les cellules (vides et numérotées) qui ne contiennent pas de bombes.
 

## Objectifs :

Ton application doit permettre de pouvoir manipuler certains paramètres avant de lancer une nouvelle partie. En particulier, on doit pouvoir choisir la taille de la grille (tu peux utiliser que des grilles carrées pour simplifier) et le nombre de bombes.

Attention, il y a deux défis majeurs dans ce travail. Tu auras besoin d’aller chercher de l’information dans la documentation / sur Internet.

- Le premier, c’est de correctement mettre en œuvre la création dynamique de contrôles. L’idée c’est qu’au début de chaque partie, tu créés une nouvelle grille de contrôles. Il te faudra donc le faire depuis le code et il faudra penser à assigner à ces contrôles créés dynamiquement la procédure événementielle que tu écriras et qui fera appel aux algorithmes du jeu.
- Le second, ce sont les algorithmes permettant de faire fonctionner correctement le jeu. En particulier, l’algorithme qui parcourt les voisins d’une cellule cliquée qui n’est pas trivial. Il s’agit d’un algorithme récursif, c’est-à-dire un algorithme qui s’appelle lui-même. Tu n’as pas encore vu ce type d’algorithme mais des aides te sont fournies dans la suite du document.

Comme tu peux le voir, il y a 3 séances pour ce TP, car il y a plusieurs difficultés. Essaye de te donner des objectifs pour chaque séance. Par exemple :
- 1ère séance : avoir une grille dynamique de contrôles au démarrage de l’application ainsi que le placement aléatoire des bombes dans le jeu.
- 2ème séance : avoir les principaux algorithmes de jeu d’implémentés
- 3ème séance : avoir le menu permettant de configurer la taille de la grille, le nombre de bombes et de relancer une nouvelle partie.
Profites-en pour personnaliser aussi ton jeu avec tes couleurs ou tes animations si tu le souhaites !


## Récupérer le projet à l'aide de travo

Pour récupérer le projet et le soumettre à la fin de la séance, vous allez devoir utiliser travo.
travo est un ensemble de scripts Python maintenu par des enseignants-chercheurs de Paris-Saclay et du Québec facilitant l'utilisation de GIT pour les enseignants. En fait les commandes travo effectuent un ensemble de commande GIT pour vous.
A l'aide d'un terminal PowerShell, tapez la commande suivante pour récupérer le projet :
```
travow fetch https://git.iut-orsay.fr/tpihm/tpihm6
```

Il vous sera demandé vos identifiants ADONIS (de l'IUT) puis le projet sera téléchargé sur votre ordinateur (dans le répertoire "tpihm6"). Si vous êtes à l'aise avec GIT, vous pouvez voir qu'en réalité, vous allez travailler sur un fork (une copie dans votre espace) du projet de l'enseignant. Vous pouvez effectuer vous mêmes les commit et les push dans votre fork, mais il est plus simple d'utiliser les commandes travo pour l'instant.
Sauvegarder ou soumettre votre travail se fera donc ensuite à l'aide de la commande :
```
travow submit tpihm6
```

Vous pouvez faire autant de "submit" que vous voulez. C'est une bonne pratique pour ne pas perdre votre travail.

## Conseils

### Remettre ton travail
N'oublie pas de soumettre ton travail avec la commande :
```
travow submit tpihm6
```
