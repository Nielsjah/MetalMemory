using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

//using System.Diagnostics;

namespace MetalMemory
{
    public partial class Settings : Page
    {
        private Context context;

        public Settings(Context pContext)
        {
            context = pContext;
            Loaded += onLoaded;

            InitializeComponent();
        }

        public void onLoaded(object sender, EventArgs e)
        {
            LoadBttn.IsEnabled = context.memoryGame.CanLoad();
        }

        private void NewGame(object sender, RoutedEventArgs e)
        {
            // setup de spelers
            MemoryPlayers playerGroup = new MemoryPlayers();

            if (SpelerNaam1.Text.notEmpty()) playerGroup.AddPlayer(SpelerNaam1.Text);
            if (SpelerNaam2.Text.notEmpty()) playerGroup.AddPlayer(SpelerNaam2.Text);
            if (SpelerNaam3.Text.notEmpty()) playerGroup.AddPlayer(SpelerNaam3.Text);
            if (SpelerNaam4.Text.notEmpty()) playerGroup.AddPlayer(SpelerNaam4.Text);

            // er moet teminste een speler zijn
            if (playerGroup.players.Count == 0)
            {
                MessageBox.Show("Vul een speler naam in.");
                return;
            }

            // setup het speelveld groote
            int gridDiameter = 0;

            if ((bool)Grid_3x3.IsChecked) gridDiameter = 3;
            if ((bool)Grid_4x4.IsChecked) gridDiameter = 4;
            if ((bool)Grid_5x5.IsChecked) gridDiameter = 5;
            if ((bool)Grid_6x6.IsChecked) gridDiameter = 6;
            if ((bool)Grid_7x7.IsChecked) gridDiameter = 7;
            if ((bool)Grid_8x8.IsChecked) gridDiameter = 8;

            Point gridGroote = new Point(gridDiameter, gridDiameter);

            // setup het aantal kaarten die gelijk moeten zijn
            int aatalGelijk = 0;

            if ((bool)KaartenDouble.IsChecked) aatalGelijk = 2;
            if ((bool)KaartenTriple.IsChecked) aatalGelijk = 3;
            if ((bool)KaartenQuad.IsChecked) aatalGelijk = 4;


            // navigeer naar het spelbord
            context.contentFrame.NavigateBlank();
            context.gameGrid.Background = (ImageBrush)FindResource("SpelR");
            context.contentFrame.Background = null;
            context.menuFrame.Navigate(context.gameInterface);

            //maak een nieuwe game
            context.memoryGame.RunGame(playerGroup, gridGroote, aatalGelijk);
        }

        private void LoadGame(object sender, RoutedEventArgs e)
        {;
            context.memoryGame.RunGame();
            context.contentFrame.NavigateBlank();
            context.gameGrid.Background = (ImageBrush)FindResource("SpelR");
            context.menuFrame.Navigate(context.gameInterface);
        }
    }
}
