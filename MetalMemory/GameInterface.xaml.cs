using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Timers;

using System.Diagnostics;

namespace MetalMemory
{
    public partial class GameInterface : Page
    {
        // private int TimeRemaining = 30;             //geeft aan hoeveel tijd er nog over is

        private Context context;
        private MemoryPlayers pPlayerGroup;
        private GameStatus pGameStatus;
        private MemoryGrid playerGrid;
        private List<PlayerBord> playerBords;
        private List<UIElement> bordControls;
        private Timer popupTimer;
        private Timer timerClock;

        public GameInterface(Context pContext)
        {
            context = pContext;
            InitializeComponent();

            // initialize het playerBord grid
            playerGrid = new MemoryGrid(PlayerGrid);
        }

        public void Setup(MemoryLogic gameLogic)
        {
            pPlayerGroup = gameLogic.playerGroup;
            pGameStatus = gameLogic.gameStatus;

            playerGrid.Create(2, 2);

            playerBords = new List<PlayerBord>();
            bordControls = new List<UIElement>();

            // timer om de kaarten na een tijdje terug te leggen
            popupTimer = new Timer();
            popupTimer.Interval = 1000;     // tijd tot kaarten weer terug vallen in ms
            popupTimer.AutoReset = false;   // zorgt dat een timer een keer aftelt

            // registreer de callback functie KaartenTerugLeggen voor het event popupTimer Elapsed
            popupTimer.Elapsed += popupVerwijderen;

            // timer om de speeltijd bij te houden
            timerClock = new Timer();
            timerClock.Interval = 1000;
            timerClock.AutoReset = true;

            // registreer de callback functie gameTimerUpdate voor het event timerClock Elapsed
            timerClock.Elapsed += gameTimerUpdate;

            // zet de speeltijd timer aan
            timerClock.Enabled = true;

            int index = 0;
            foreach (MemoryPlayer player in pPlayerGroup.players)
            {
                PlayerBord playerBord = new PlayerBord(this) { naam = player.naam, score = player.score };
                playerBords.Add(playerBord);

                bordControls.Add(playerBord.pControl);
                playerGrid.AddByIndex(playerBord.pControl, index++);

                if (index == pPlayerGroup.players.Count)
                    playerBord.pControl.Loaded += playerBordsLoaded;
            }
        }

        private void playerBordsLoaded(object sender, RoutedEventArgs e)
        {
            UpdateAanZet();
        }

        public void ExitGame()
        {
            timerClock.Close();
            popupTimer.Close();
            playerBords = null;
            playerGrid.Delete(bordControls);
            bordControls = null;
        }

        public void UpdateScore(string popupScore)
        {
            int playerIndex = pPlayerGroup.aanZet;
            MemoryPlayer player = pPlayerGroup.players[playerIndex];
            PlayerBord playerBord = playerBords[playerIndex];

            playerBord.popupScore = popupScore;
            playerBord.score = player.score;

            // start timer die KaartenTerugLeggen aanroept
            popupTimer.Enabled = true;
        }

        public void UpdateAanZet()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SetAanZetVisibility(pPlayerGroup.prevSpeler, Visibility.Hidden);
                SetAanZetVisibility(pPlayerGroup.aanZet, Visibility.Visible);
            });
        }

        private void popupVerwijderen(object sender, ElapsedEventArgs e)
        {
            int playerIndex = pPlayerGroup.aanZet;
            MemoryPlayer player = pPlayerGroup.players[playerIndex];
            PlayerBord playerBord = playerBords[playerIndex];
            playerBord.popupScore = "";
        }

        private void gameTimerUpdate(object sender, ElapsedEventArgs e)
        {
            int playerIndex = pPlayerGroup.aanZet;
            MemoryPlayer player = pPlayerGroup.players[playerIndex];
            PlayerBord playerBord = playerBords[playerIndex];

            TimeSpan playerSpeeltijd = new TimeSpan(0, 0, player.speeltijd++);
            playerBord.speeltijd = string.Format("{0}:{1:00}", (int)playerSpeeltijd.TotalMinutes, playerSpeeltijd.Seconds);

            Application.Current.Dispatcher.Invoke(() =>
            {
                TimeSpan gameSpeeltijd = new TimeSpan(0, 0, pGameStatus.speeltijd++);
            gameTimer.Text = string.Format("{0}:{1:00}", (int)gameSpeeltijd.TotalMinutes, gameSpeeltijd.Seconds);
            });
        }

        private void SetAanZetVisibility(int spelerIndex, Visibility visibility)
        {
            PlayerBord playerBord = playerBords[spelerIndex];
            playerBord.status.Visibility = visibility;
        }

        private void MainMenu(object sender, RoutedEventArgs e)
        {
            // vraag de speler of het spel moet worden beëindigd
            MessageBoxResult antwoord = MessageBox.Show(
                "Weet u het zeker ?",
                "Dit beëindigd het spel.",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            // als het antwoord ja is stoppen we het spel
            // en navigeren we terug naar het hoofdmenu
            if (antwoord == MessageBoxResult.Yes)
                context.hoofdMenu.ExitGame();
        }

        private void SaveGame(object sender, RoutedEventArgs e)
        {
            context.memoryGame.SaveGame();
        }
    }
}
