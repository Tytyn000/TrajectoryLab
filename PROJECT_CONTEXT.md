# TrajectoryLab — Contexte du projet

## Objectif

TrajectoryLab est une application de simulation scientifique de trajectoires en C#.

Le programme doit permettre de simuler, comparer et visualiser des trajectoires avec différents modèles physiques et différentes méthodes numériques.

Durée prévue du développement initial : environ trois semaines.

## Technologies

- C#
- .NET 10
- solution au format .slnx
- xUnit pour les tests
- Git et GitHub Desktop
- future interface WPF
- future bibliothèque graphique : ScottPlot

## Structure actuelle

TrajectoryLab/
├── src/
│   ├── TrajectoryLab.Core/
│   └── TrajectoryLab.Cli/
├── tests/
│   └── TrajectoryLab.Tests/
└── TrajectoryLab.slnx

TrajectoryLab.Core contient le moteur scientifique.
TrajectoryLab.Cli permet de tester le moteur sans interface.
TrajectoryLab.Tests contient les tests automatisés.

## Convention du repère

- X : Est
- Y : Nord
- Z : altitude
- unités SI
- temps en secondes
- distance en mètres
- vitesse en mètres par seconde
- accélération en mètres par seconde carrée

## Éléments déjà implémentés

- Vector3D utilisant des double
- SimulationState
- SimulationParameters
- SimulationResult
- LaunchVelocity
- IAccelerationModel
- ConstantGravityModel
- CompositeAccelerationModel
- DragAccelerationModel
- INumericalSolver
- EulerSolver
- RungeKutta4Solver
- StateDerivative
- TrajectorySimulator
- détection de l’impact au sol
- interpolation du point d’impact
- comparaison de scénarios dans la CLI
- projet de tests xUnit
- compilation et exécution fonctionnelles

## Modèles physiques actuels

- gravité constante
- traînée aérodynamique quadratique
- densité de l’air constante
- vent constant stocké dans les paramètres
- mouvement tridimensionnel

Formule de la traînée :

a_drag = -(rho * Cd * A / (2m)) * |v_relative| * v_relative

avec :

v_relative = v_projectile - v_vent

## Méthodes numériques actuelles

### Euler explicite

Méthode simple utilisée comme référence pédagogique.
Elle produit une erreur numérique visible et une dérive de l’énergie.

### Runge-Kutta d’ordre 4

Méthode principale actuelle.
Elle produit des résultats très proches de la solution analytique dans le vide.

## Résultat de validation obtenu

Scénario :

- vitesse initiale : 50 m/s
- élévation : 45°
- azimut : 0°
- gravité : 9,80665 m/s²
- pas de temps : 0,01 s
- départ et impact à l’altitude 0

Résultats RK4 approximatifs :

- durée du vol : 7,210482 s
- portée numérique : 254,929027 m
- portée analytique : 254,929049 m
- erreur sur la portée : environ 0,000022 m
- altitude maximale : environ 63,732151 m
- vitesse d’impact : environ 50 m/s

## Architecture recherchée

Le moteur scientifique ne doit pas dépendre de l’interface graphique.

Chaque phénomène physique doit être représenté par un modèle indépendant pouvant être combiné avec les autres.

Exemples :

- modèle de gravité
- modèle de traînée
- modèle atmosphérique
- modèle de vent
- force de Coriolis
- effet Magnus

Les solveurs numériques doivent également rester indépendants des modèles physiques.

## Prochaines étapes prévues

1. Séparer les paramètres :
   - ProjectileParameters
   - EnvironmentParameters
   - SimulationSettings

2. Créer une abstraction pour l’atmosphère :
   - IAtmosphereModel
   - ConstantAtmosphereModel
   - ExponentialAtmosphereModel
   - modèle ISA simplifié

3. Ajouter des modèles de vent :
   - vent constant
   - vent variant avec l’altitude
   - vent par couches

4. Calculer :
   - température
   - pression
   - densité
   - vitesse du son
   - nombre de Mach

5. Rendre le coefficient de traînée dépendant du nombre de Mach.

6. Ajouter :
   - force de Coriolis
   - effet Magnus
   - gravité variable
   - autres planètes

7. Ajouter des méthodes numériques :
   - Euler semi-implicite
   - point milieu
   - RK45 adaptatif

8. Ajouter :
   - analyse de convergence
   - conservation de l’énergie
   - balayage de paramètres
   - simulations Monte-Carlo

9. Créer une interface WPF basique avec ScottPlot.

10. Ajouter :
    - sauvegarde JSON
    - export CSV
    - export des graphiques
    - documentation GitHub
    - GitHub Actions

## Conventions de code

- noms des types et membres en anglais
- PascalCase pour les noms C#
- calculs internes en double précision
- classes courtes et spécialisées
- validation explicite des paramètres
- tests pour chaque nouveau modèle physique
- commentaires uniquement lorsqu’ils expliquent une décision ou une formule
- aucune logique scientifique directement dans l’interface