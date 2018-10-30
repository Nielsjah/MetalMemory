using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;

using System.Diagnostics;

namespace MetalMemory
{
    public partial class MMindow : Window
    {
        private Context context;

        public MMindow()
        {
            KeyDown += eventKeyDown;

            InitializeComponent();

            context = new Context(FrameMenu, FrameContent, GridGame);
            context.memoryGame = new MemoryGame(context);
            context.highScore = new HighScore();
            context.hoofdMenu = new HoofdMenu(context);
            context.settings = new Settings(context);
            context.gameInterface = new GameInterface(context);

            // initialiseer de navigatie panelen
            context.menuFrame.Navigate(context.highScore);
            context.contentFrame.Navigate(context.hoofdMenu);
        }

        private void eventKeyDown(Object sender, KeyEventArgs e)
        {
            // fullscreen mode
            if (e.Key == Key.F11)
            {
                if (WindowStyle == WindowStyle.SingleBorderWindow)      //checked of de game fullscreen staat
                {
                    ResizeMode = ResizeMode.NoResize;                   //zet de game in fullscreen
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    ResizeMode = ResizeMode.CanResize;                  //haalt de game uit fullscreen
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Normal;
                }
            } 
        }

        // forceerd alle threads te sluiten voordat de window sluit - voorkomt null call errors
        protected override void OnClosing(CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
