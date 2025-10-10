Voici un exemple complet d'API ASP.NET Core utilisant les Claims pour l'autorisation !

## **Points clés de l'exemple :**

### **1. Configuration des Policies (Program.cs)**
- `RequireAdminDepartment` : vérifie que le claim "Department" = "Admin" ou "IT"
- `RequireSeniorEmployee` : vérifie le niveau d'employé
- `RequireMinimumAge` : logique personnalisée avec RequireAssertion
- `RequireAdminAccess` : combine plusieurs claims

### **2. Génération du Token JWT (AuthController)**
Les claims sont ajoutés au token lors de la génération. Deux utilisateurs de test :
- **admin/password123** → reçoit les claims Admin
- **user/password123** → reçoit les claims basiques

### **3. Protection des Routes (ProductsController)**
Différents niveaux de protection selon les besoins :
- `[Authorize]` → authentification simple
- `[Authorize(Policy = "...")]` → vérification de claims spécifiques
- Vérification manuelle avec `User.HasClaim()` et `User.FindFirst()`

## **Pour tester :**

1. Appelez `POST /api/auth/login` avec les credentials
2. Récupérez le token JWT
3. Utilisez-le dans le header : `Authorization: Bearer {votre_token}`
4. Testez les différentes routes pour voir les autorisations en action

Les routes `/admin` et `/premium` seront interdites pour l'utilisateur "user", mais accessibles pour "admin" !
