# PCSpecs

**PCSpecs** est une application de bureau Windows qui permet de consulter les composants de son ordinateur et de surveiller ses performances en temps réel.

## ✨ Fonctionnalités

### 🖥️ Specs
- Affichage des composants de l'ordinateur.
- Informations sur le processeur (CPU).
- Informations sur la carte graphique (GPU).
- Informations sur la mémoire RAM.
- Interface organisée en cartes.

### 📊 Performance in use
- Utilisation du CPU (%).
- Utilisation du GPU (%).
- Utilisation de la RAM (%).
- Graphiques de performances sur 60 secondes.
- Températures du CPU et du GPU lorsque les capteurs sont disponibles.
- Fréquences CPU et GPU.
- Utilisation de la VRAM.
- Vitesse des ventilateurs lorsque disponible.
- Consommation électrique lorsque disponible.
- Utilisation du disque système.
- Vitesse du réseau (téléchargement et envoi).
- Top 5 des processus utilisant le CPU et la RAM.

### 🎨 Interface
- Design sombre et moderne.
- Thème violet.
- Navigation simple.
- Langues française et anglaise.

## ⚙️ Technologies

- C#
- WPF
- .NET 9
- LibreHardwareMonitor
- System.Management

## 💻 Configuration requise

- Windows 10 ou Windows 11.
- .NET 9 SDK pour compiler le projet.
- Visual Studio 2022 pour modifier le code source.

## 📥 Installation

1. Télécharge la dernière version de PCSpecs.
2. Extrais les fichiers si nécessaire.
3. Ouvre `PCSpecs.exe`.
4. Consulte les composants et les performances de ton ordinateur.

## 🔨 Compiler le projet

Ouvre un terminal dans le dossier contenant le fichier `.csproj`, puis exécute :

```powershell
dotnet build
```

## 📦 Publier en fichier EXE

Pour générer une version autonome de PCSpecs dans Windows PowerShell :

```Windows PowerShell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

## ⚠️ Informations sur les capteurs

- Certaines mesures peuvent afficher `N/D` si le matériel ne les expose pas.
- La température du CPU peut nécessiter le lancement de l'application en administrateur.
- Les mesures disponibles dépendent du matériel et de ses capteurs.

## 📄 Licence

Aucune licence spécifiée pour le moment.

---

**PCSpecs — Know your PC.**
