using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Timers;

//using System.Diagnostics;

namespace MetalMemory
{
    public partial class GameInterface : Page
    {
        // private int TimeRemaining = 30;             //geeft aan hoeveel tijd er nog over is

        private Context context;
        private MemoryPlayers pPlayerGroup;
        private MemoryGrid playerGrid;
        private List<PlayerBord> playerBords;
        private List<UIElement> bordControls;
        private Timer popupTimer;

        public GameInterface(Context pContext)
        {
            context = pContext;
            InitializeComponent();

            // initialize het playerBord grid
            playerGrid = new MemoryGrid(PlayerGrid);
        }

        public void Setup(MemoryPlayers playerGroup)
        {
            pPlayerGroup = playerGroup;

            playerGrid.Create(2, 2);

            playerBords = new List<PlayerBord>();
            bordControls = new List<UIElement>();

            // timer om de kaarten na een tijdje terug te leggen
            popupTimer = new Timer();
            popupTimer.Interval = 1000;     // tijd tot kaarten weer terug vallen in ms
            popupTimer.AutoReset = false;   // zorgt dat een timer een keer aftelt

            // registreer de callback functie KaartenTerugLeggen voor het event Elapsed
            popupTimer.Elapsed += popupVerwijderen;

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
            popupTimer.Close();
            playerBords = null;
            playerGrid.Delete(bordControls);
            bordControls = null;
        }

        public void UpdateScore(string popupScore)
        {
            //Debug.WriteLine("GameInterface UpdateScore");
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
