using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

//using System.Diagnostics;

namespace MetalMemory
{
    public partial class HoofdMenu : Page
    {
        private Context context;
        private bool startButtonState = start;

        public const bool start = false;
        public const bool highscore = true;

        public HoofdMenu(Context pContext)
        {
            context = pContext;
            InitializeComponent();
        }

        private void Start(object sender, System.Windows.RoutedEventArgs e)
        {
            switch (startButtonState)
            {
                case start:
                    context.menuFrame.Navigate(context.settings);
                    context.contentFrame.Background = (ImageBrush)FindResource("SpelR");

                    SetStartButton(highscore);
                    break;

                case highscore:
                    context.menuFrame.Navigate(context.highScore);
                    context.contentFrame.Background = (ImageBrush)FindResource("MenuR");

                    SetStartButton(start);
                    break;
            }
        }

        private void Exit(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void ExitGame()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                context.memoryGame.ExitGame();

                context.menuFrame.Navigate(context.highScore);

                context.contentFrame.Background = (ImageBrush)FindResource("MenuR");
                context.contentFrame.Navigate(context.hoofdMenu);

                SetStartButton(start);
            });
        }

        private void SetStartButton(bool state)
        {
            startButtonState = state;

            if (startButtonState == start)
                StartButton.Text = "Start";

            if (startButtonState == highscore)
                StartButton.Text = "Score";
        }
    }
}
