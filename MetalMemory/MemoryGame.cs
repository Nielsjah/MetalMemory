using System.IO;
using System.Windows;
using System.Windows.Controls;

//using System.Diagnostics;

namespace MetalMemory
{
    public class MemoryGame
    {
        public int nKaartenGelijk = 0;

        public Context context { get; private set; }
        public Grid hGrid { get; }

        public Point gridSize { get; private set; }
        public int nColumns { get; private set; }
        public int nRows { get; private set; }

        public int nKaartenTotaal { get; private set; }
        public int nKaartenUniek { get; private set; }

        public MemoryGrid memoryGrid { get; private set; }
        public MemoryLogic memoryLogic { get; private set; }
        public MemoryCards memoryCards { get; private set; }

        public MemoryGame(Context pContext)
        {
            // setup de benodigde data
            context = pContext;
            hGrid = context.gameGrid;

            // initialize het speelveld grid
            memoryGrid = new MemoryGrid(hGrid);
        }

        // load game function entry
        public void RunGame()
        {
            Stream loadStream = File.Open(GameData.sSaveFile, FileMode.Open);

            SetupGame(loadStream: loadStream);

            loadStream.Close();
        }

        // new game function entry
        public void RunGame(MemoryPlayers players, Point gridGroote, TimerMode timerMode, int nKaartenGelijk)
        {
            SetupGame(players, gridGroote, timerMode, nKaartenGelijk);
        }

        // create/load a game
        private void SetupGame(MemoryPlayers players = null, Point gridGroote = new Point(), TimerMode timerMode = 0, int nKaartenGelijk = 0, Stream loadStream = null)
        {
            // Check of we een spel moeten laden
            bool loadGame = loadStream != null;

            // setup de benodigde data
            // deze data leest de MemoryGrid en MemoryCards object
            if (loadGame)
                SetupSpeelveldData((Point)loadStream.Load(), (int)loadStream.Load());
            else
                SetupSpeelveldData(gridGroote, nKaartenGelijk);

            // maak de het speelveld in de grid
            memoryGrid.Create(nColumns, nRows);

            // leg de kaarten uit op het speelveld
            memoryCards = new MemoryCards(this, loadStream);

            // setup de game logic
            memoryLogic = new MemoryLogic(this, timerMode, players, loadStream);
        }

        public void SaveGame()
        {
            Stream saveStream = File.Open(GameData.sSaveFile, FileMode.Create);

            saveStream.Save(gridSize);
            saveStream.Save(nKaartenGelijk);

            memoryCards.SaveGame(saveStream);

            memoryLogic.SaveGame(saveStream);

            saveStream.Close();
        }

        public bool CanLoad()
        {
            if (File.Exists(GameData.sSaveFile)) return true;
            return false;
        }

        public void ExitGame()
        {
            memoryGrid.Delete(memoryCards.lSymboolButtons);
            memoryCards = null;
            memoryLogic.ExitGame();
        }

        private void SetupSpeelveldData(Point gridSize, int nKaartenGelijk)
        {
            // bewaar de gridSize point voor saven
            this.gridSize = gridSize;
            this.nKaartenGelijk = nKaartenGelijk;

            // laad de columns en row data
            nColumns = (int)gridSize.X;
            nRows = (int)gridSize.Y;

            // bereken het totaal aantal kaarten en de hoeveelheid unieke kaarten
            nKaartenTotaal = nColumns * nRows;
            nKaartenUniek = nKaartenTotaal / nKaartenGelijk;
        }       
    }
}

