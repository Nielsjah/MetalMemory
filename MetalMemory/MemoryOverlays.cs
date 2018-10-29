using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;

//using System.Diagnostics;

namespace MetalMemory
{
    public class MemoryOverlays
    {
        private MemoryGrid memoryGrid;
        private MemoryGame currentGame;
        private MemoryResource overlayResource;

        public List<Image> aFeedbacks;

        private int nKaartenGelijk;

        public MemoryOverlays(MemoryGame game)
        {
            // copieer de benodige object handles
            currentGame = game;

            // copieer de benodige data vanuit het memorygame object
            nKaartenGelijk = currentGame.nKaartenGelijk;
            memoryGrid = currentGame.memoryGrid;

            // maak een resource object van de kaart image resources
            overlayResource = new MemoryResource(GameData.sOverlayLocatie);
            overlayResource.Add(GameData.sOverlayFout);
            overlayResource.Add(GameData.sOverlayGoed);

            //Uri testURI = new Uri("Images/Overlays/Checkmark.png", UriKind.Relative);
            // BitmapImage testBitmap = new BitmapImage(testURI);

            ImageSource voorKant = overlayResource.Find(GameData.sOverlayGoed);

            feedbackCreate();
        }

        public void Show(bool isZichtbaar)
        {
            Visibility zichtbaarheid;

            if (isZichtbaar)
                zichtbaarheid = Visibility.Visible;
            else
                zichtbaarheid = Visibility.Hidden;

            foreach (Image feedbackImage in aFeedbacks)
                feedbackImage.Visibility = zichtbaarheid;
        }

        public void SetGoedFout(bool isGoed)
        {
            ImageSource reactieImg;

            if (isGoed)
                reactieImg = overlayResource.Find(GameData.sOverlayGoed);
            else
                reactieImg = overlayResource.Find(GameData.sOverlayFout);

            setFeedbackImg(reactieImg);
        }

        public void SetLocation(List<Point> lLocations)
        {
            foreach (Image feedback in aFeedbacks)
            {
                int index = aFeedbacks.IndexOf(feedback);

                Point location = lLocations[index];

                memoryGrid.SetLocation(feedback, (int)location.X, (int)location.Y);
            }
        }

        private void setFeedbackImg(ImageSource feedbackImg)
        {
            foreach (Image feedbackImage in aFeedbacks)
                feedbackImage.Source = feedbackImg;
        }

        private void feedbackCreate()
        {
            aFeedbacks = new List<Image>();
            for (int index = 0; index < nKaartenGelijk; index++)
                aFeedbacks.Add(new Image());

            ScaleTransform testScale = new ScaleTransform(0.3, 0.3);

            foreach (Image feedbackImage in aFeedbacks)
            {
                feedbackImage.RenderTransform = testScale;
                feedbackImage.RenderTransformOrigin = new Point(0.5, 0.5);
                feedbackImage.IsHitTestVisible = false;
                memoryGrid.Add(feedbackImage);
            }
        }
    }
}
