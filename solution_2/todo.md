Solution 1 - Overlay de surlignage :

Conserve tous vos templates existants
Ajoute une couche de surlignage semi-transparente par-dessus
Plus simple à implémenter

Solution 2 - Surlignage précis du texte :

Met en surbrillance uniquement les caractères correspondants
Plus sophistiqué visuellement
Nécessite le TextHighlightConverter

Solution 3 - Overlay simple :

Version hybride avec overlay d'opacité
Bon compromis entre simplicité et visibilité

Pour implémenter :

Ajoutez la propriété FilterText à votre ViewModel
Choisissez une des versions de templates selon vos préférences
Ajoutez les convertisseurs nécessaires
Modifiez votre XAML pour inclure la TextBox de filtrage

Recommandation : Je suggère la Solution 1 car elle :

Préserve parfaitement vos templates existants
Offre un bon contraste visuel
Est facile à maintenir
Fonctionne bien avec toutes les couleurs de fond
