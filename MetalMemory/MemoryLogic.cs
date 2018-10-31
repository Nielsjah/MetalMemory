using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

//using System.Diagnostics;

namespace MetalMemory
{
    public class MemoryLogic
    {
        public MemoryOverlays KaartFeedback { private get; set; }
        public GameStatus gameStatus { get; private set; }
        public MemoryPlayers playerGroup { get; private set; }

        private MemoryGame currentGame;
        private Context context;
        private MemoryCards memoryCards;
        private GameInterface gameInterface;

        private List<Move> moves;
        private Timer kaartTimer;
        private int nKaartenGelijk;

        public MemoryLogic(MemoryGame game, TimerMode timerMode, MemoryPlayers players, Stream loadStream = null)
        {
            // Check of we een spel moeten laden
            bool loadGame = loadStream != null;

            // copieer de benodige data vanuit het memorygame object game
            currentGame = game;
            context = currentGame.context;
            memoryCards = currentGame.memoryCards;
            gameInterface = currentGame.context.gameInterface;
            nKaartenGelijk = currentGame.nKaartenGelijk;

            // deze lijst gebruiken we om de zetten te onthouden
            moves = new List<Move>();

            // timer om de kaarten na een tijdje terug te leggen
            kaartTimer = new Timer();
            kaartTimer.Interval = 1000;     // tijd tot kaarten weer terug vallen in ms
            kaartTimer.AutoReset = false;   // zorgt dat een timer een keer aftelt

            // registreer de callback functie KaartenTerugLeggen voor het event Elapsed
            kaartTimer.Elapsed += KaartenTerugLeggen;

            // maak de feedback overlay images
            KaartFeedback = new MemoryOverlays(currentGame);

            // Setup de player data
            if (loadGame)
                playerGroup = loadStream.Load() as MemoryPlayers;
            else
                playerGroup = players;

            // als een nieuwe game word gestart dobbelen we wie start
            if (!loadGame)
                 playerGroup.aanZet = Global.RandomNumberGenerator.Next(playerGroup.players.Count);

            // setup of herlaad de game status
            if (loadGame)
                gameStatus = loadStream.Load() as GameStatus;
            else
                gameStatus = new GameStatus(timerMode, currentGame.nKaartenUniek);

            // setup het game interface
            gameInterface.Setup(this);
        }

        public void SaveGame(Stream saveStream)
        {
            saveStream.Save(playerGroup);
            saveStream.Save(gameStatus);
        }

        public void CardClick(object sender, EventArgs e)
        {
            // copieer het button object van de message sender
            Button symboolBttn = sender as Button;

            // als een timer loopt kan geen zet gedaan worden
            if (kaartTimer.Enabled) return;

            // voorkom dat dezelfde grid positie twee keer word geteld als zet
            foreach (Move move in moves)
                if (move.kaartBttn == symboolBttn) return;

            // registreer de zet
            Move currentMove = new Move();
            currentMove.kaartBttn = symboolBttn;
            currentMove.kaartImg = symboolBttn.Content as Image;
            currentMove.kaartData = symboolBttn.Tag as KaartData;
            moves.Add(currentMove);

            // laat kaart zien
            memoryCards.showKaart(symboolBttn, Kaart.voorkant);

            VerwerkZet();
        }

        private List<Point> GetZetLocaties()
        {
            List<Point> zetLocaties = new List<Point>();
            foreach (Move move in moves)
            {
                Point zetLocatie = new Point();
                zetLocatie.X = Grid.GetColumn(move.kaartBttn);
                zetLocatie.Y = Grid.GetRow(move.kaartBttn);
                zetLocaties.Add(zetLocatie);
            }

            return zetLocaties;
        }

        private bool IsKaartenGelijk()
        {
            // maak een lijst van de move symbool IDs
            List<int> lKaartIdsX = new List<int>();
            foreach (Move move in moves)
                lKaartIdsX.Add(move.kaartData.ID);

            // copieer de lijst naar een nieuwe lijst om te vergelijken
            List<int> lKaartIdsY = new List<int>(lKaartIdsX);

            // de laatste ID hoeft niet te worden vergeleken met zichzelf
            lKaartIdsX.RemoveLast();

            // loop door alle symbool IDs heen
            foreach (int kaartIdX in lKaartIdsX)
            {
                // de eerste ID hoeft niet te worden vergeleken met zichzelf
                lKaartIdsY.RemoveFirst();

                // loop door alle vergelijkingen heen
                foreach (int kaartIdY in lKaartIdsY)

                    // als de IDs niet gelijk zijn, zijn niet alle kaarten gelijk
                    if (kaartIdX != kaartIdY) return false;
            }
            // als we door alle vergelijkingen heen zijn, zijn alle kaarten gelijk
            return true;
        }

        private void KaartenTerugLeggen(object sender, ElapsedEventArgs e)
        {
            Action<Move> aktie;

            if (gameStatus.kaartenGelijk)
                aktie = move => memoryCards.VerwijderKaart(move.kaartBttn);
            else
                aktie = move => memoryCards.showKaart(move.kaartBttn, Kaart.achterkant);

            Application.Current.Dispatcher.Invoke(() =>
            {
                KaartFeedback.Show(false);
                foreach (Move move in moves)
                    aktie(move);
            });

            moves.Clear();

            if (gameStatus.kaartenGelijk && !Convert.ToBoolean(--gameStatus.aantalOver))
                GameOver();
            else
                UpdatePlayers();
        }

        private void UpdatePlayers()
        {
            // als de kaarten niet gelijk zijn en de game mode is om en om
            // is de volgende speler aan zet
            if (!gameStatus.kaartenGelijk && gameStatus.timerMode == TimerMode.roundAbout)
                playerGroup.NextPlayer();

            gameStatus.Reset();

            gameInterface.UpdateAanZet();
        }

        private void AddScore()
        {
            if (gameStatus.kaartenGelijk)
            {
                // als het aantal goed nog nul is, is de multiplier 1
                double bonusMultiplier = 1;

                // calculeer de multiplier als het aantal goed hoger is dan 0
                if (Convert.ToBoolean(gameStatus.aantalGoed))
                    bonusMultiplier = gameStatus.aantalGoed * MemoryScore.Match.BonusMultiplier;

                // score is ScorePerKaart maal het aantal weggespeelde kaarten
                int score = MemoryScore.Match.ScorePerKaart * currentGame.nKaartenGelijk;

                // calculeer de totale score
                int totalScore = Convert.ToInt32(score * bonusMultiplier);

                // update de score
                playerGroup.CurrentPlayer().score += totalScore;

                // update het score interface display
                gameInterface.UpdateScore(string.Format("+{0}", totalScore));

                gameStatus.aantalGoed++;
            }
            else
                gameStatus.aantalGoed = 0;
        }

        private void GameOver()
        {
            // stop de speeltijd timer
            gameInterface.timerClock.Enabled = false;

            CalculeerUitkomst();

            context.highScore.AddScores(playerGroup.players);

            string sUitkomst = "";
            if (gameStatus.gelijkSpel)
                sUitkomst = "Gelijk spel, niemand heeft gewonnen.";
            else
            {
                MemoryPlayer playerGewonnen = playerGroup.players[gameStatus.playerGewonnen];
                string playerNaam = playerGewonnen.naam;
                int playerScpre = playerGewonnen.score;
                sUitkomst = string.Format("{0} heeft gewonnen. score {1}", playerNaam, playerScpre);
            }

            MessageBox.Show(
                sUitkomst,
                "Game Over",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            context.hoofdMenu.ExitGame();
        }

        public void ExitGame()
        {
            kaartTimer.Close();
            KaartFeedback = null;
            gameInterface.ExitGame();
        }

        private void VerwerkZet()
        {
            // doe miks als er nog geen 2 kaarten zijn geselecteerd
            if (moves.Count < nKaartenGelijk) return;

            gameStatus.kaartenGelijk = IsKaartenGelijk();

            KaartFeedback.SetGoedFout(gameStatus.kaartenGelijk);

            List<Point> lZetLocaties = GetZetLocaties();
            KaartFeedback.SetLocation(lZetLocaties);

            AddScore();

            KaartFeedback.Show(true);

            // start timer die KaartenTerugLeggen aanroept
            kaartTimer.Enabled = true;
        }

        private void CalculeerUitkomst()
        {
            int playerGewonnen = -1;
            int hoogsteScore = -1;
            int gelijkSpelScore = -1;

            int index = 0;
            foreach (MemoryPlayer player in playerGroup.players)
            {
                // als de score gelijk is met de hoogste score
                // bewaren we dit as gelijkSpel score
                if (player.score == hoogsteScore)
                    gelijkSpelScore = player.score;

                // als de score hoger is dan de hoogste score
                // updaten we de hoogste score en zetten we een winnaar index
                if (player.score > hoogsteScore)
                {
                    hoogsteScore = player.score;
                    playerGewonnen = index;
                }

                index++;
            }

            // als hierna de gelijkSpel score gelijk is met de hoogste score
            // hebben de twee (of meer) winnaars gelijkspel 
            bool gelijkSpel = gelijkSpelScore == hoogsteScore;

            // update de player gewonnen stat
            if (!gelijkSpel)
                playerGroup.players[playerGewonnen].gewonnen = true;

            // update de game status
            gameStatus.gelijkSpel = gelijkSpel;
            gameStatus.playerGewonnen = playerGewonnen;
        }
    }
}
