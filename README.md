# iBay

## La base de données

Par défaut le type de base de données utilisé est `LocalDB` avec l'instance `MSSQLLocalDB`.

Dans le fichier `appsettings.json`, il est possible d'adapter la base de données SQL Server en modifiant cette ligne :
```json
"DefaultConnection": "Server=(LocalDB)\\MSSQLLocalDB; Integrated Security=true;"
```

## Comment lancer le projet ?

### Méthode 1

Ouvrir le projet avec Visual Studio en ouvrant le fichier `iBay.sln` (pour ouvrir la solution). Puis lancer le projet en cliquant sur `Play`.

### Méthode 2

Lancer le fichier `./API/bin/Release/net8.0/publish/API.exe` (ATTENTION l'application est lancée avec la base de données par défaut).