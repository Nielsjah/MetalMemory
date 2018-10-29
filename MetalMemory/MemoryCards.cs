using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

//using System.Diagnostics;

namespace MetalMemory
{
    public class MemoryCards
    {
        public List<UIElement> lSymboolButtons { get; private set; }

        // data gebruikt om te loaden en saveen
        private List<int> lUsedKaartIds;
        private List<int> lGridKaarten;

        private MemoryGame currentGame;
        private MemoryGrid memoryGrid;
        private MemoryResource kaartResource;

        private int nColumns;
        private int nRows;
        private int nKaartenUniek;
        private int nKaartenGelijk;

        public MemoryCards(MemoryGame game, Stream loadStream = null)
        {
            // Check of we een spel moeten laden
            bool loadGame = loadStream != null;

            // copieer de benodige object handles
            currentGame = game;
            memoryGrid = currentGame.memoryGrid;

            // copieer de benodige data vanuit het memorygame object game
            nColumns = currentGame.nColumns;
            nRows = currentGame.nRows;
            nKaartenUniek = currentGame.nKaartenUniek;
            nKaartenGelijk = currentGame.nKaartenGelijk;

            if (loadGame)
                // laad de gebruikte kaarten
                lUsedKaartIds = loadStream.Load() as List<int>;
            else
                // maak een lijst van random kaart IDs
                lUsedKaartIds = GetRndmKaartLijst(nKaartenUniek);

            // maak een resource object van de kaart gebruikte image resources
            kaartResource = GetKaartResources(lUsedKaartIds);

            if (loadGame)
                // laad de kaart posities
                lGridKaarten = loadStream.Load() as List<int>;
            else
                // zorg dat alle kaarten twee keer voorkomen en zet ze in toevallige volgorde
                lGridKaarten = SplitAndRandomize(lUsedKaartIds);

            // maak een lijst van buttons voor de kaart speelveld
            lSymboolButtons = GetBttnLijst(lGridKaarten);

            if (loadGame)
                // laad en set de status van de kaarten
                SetKaartStatus(loadStream.Load() as List<KaartStatus>);

            // registreer de buttons in het grid van het speelveld
            int index = 0;
            foreach (Button symboolBttn in lSymboolButtons)
                memoryGrid.AddByIndex(symboolBttn, index++);
        }

        public void SaveGame(Stream saveStream)
        {
            saveStream.Save(lUsedKaartIds);
            saveStream.Save(lGridKaarten);
            saveStream.Save(GetKaartStatus(lSymboolButtons));
        }

        public void showKaart(Button kaartBttn, Kaart kaartKant)
        {
            Image kaartImg = kaartBttn.Content as Image;
            KaartData kaartData = kaartBttn.Tag as KaartData;

            switch (kaartKant)
            {
                case Kaart.voorkant:
                    kaartImg.Source = kaartData.voorKant;
                    break;
                case Kaart.achterkant:
                    kaartImg.Source = kaartData.achterKant;
                    break;
            }
        }

        public void VerwijderKaart(Button kaartBttn)
        {
            KaartData buttonData = kaartBttn.Tag as KaartData;

            kaartBttn.IsEnabled = false;
            kaartBttn.Visibility = Visibility.Hidden;

            KaartStatus kaartStatus = buttonData.status;

            kaartStatus.gespeeld = true;     
        }

        private List<KaartStatus> GetKaartStatus(List<UIElement> lButtons)
        {
            List<KaartStatus> lKaartStatussen = new List<KaartStatus>();

            foreach (Button button in lButtons)
            {
                KaartData buttonData = button.Tag as KaartData;
                KaartStatus kaartStatus = buttonData.status;

                lKaartStatussen.Add(kaartStatus);
            }

            return lKaartStatussen;
        }

        private void SetKaartStatus(List<KaartStatus> lKaartStatussen)
        {
            int index = 0;
            foreach (Button kaartBttn in lSymboolButtons)
            {
                KaartStatus kaartStatus = lKaartStatussen[index++];

                if (kaartStatus.gespeeld)
                    VerwijderKaart(kaartBttn);
            }
        }

        private MemoryResource GetKaartResources(List<int> lUsedKaartIds)
        {
            // maak een resource object van de kaart image resources
            MemoryResource kaartResources = new MemoryResource(GameData.sKaartLocatie);
            kaartResources.Add(GameData.sKaartAchterkant);
            kaartResources.AddList(lUsedKaartIds);

            return kaartResources;
        }

        private List<int> GetRndmKaartLijst(int nHoeveel)
        {
            // maak een lijst van alle mogelijke kaart IDs
            List<int> AlleKaarten = new List<int>();
            for (int nKaart = 1; nKaart <= GameData.nKaartPNGs; nKaart++)
                AlleKaarten.Add(nKaart);

            // Selecteer random een nHoeveel aantal kaarten
            List<int> RandomKaarten = new List<int>();
            for (int i = 0; i < nHoeveel; i++)
            {
                // selecteer een radom kaart ID uit de AlleKaarten lijst
                int RandomIndex = Global.RandomNumberGenerator.Next(AlleKaarten.Count);
                int RandomKaart = AlleKaarten[RandomIndex];

                // voeg de ID toe aan de nieuwe lijst
                RandomKaarten.Add(RandomKaart);

                // verwijder de kaart ID uit de AlleKaarten lijst
                // zodat deze kaart ID niet opnieuw kan worden geselecteerd
                AlleKaarten.RemoveAt(RandomIndex);
            }

            return RandomKaarten;
        }

        private List<int> SplitAndRandomize(List<int> lKaartIds)
        {
            // copieer de kaart ID lijst in een nieuwe lijst
            List<int> lRandomKaartIds = new List<int>(lKaartIds);

            // copieer de lijst nogmaals achter zichzelf aan
            // zodat alle kaart ID een x aantal voorkomen in de lijst
            for(int index = 1; index++ < nKaartenGelijk; )
            {
                lRandomKaartIds.AddRange(lKaartIds);
            }

            // reorganiseer de lijst op volgorde van random getallen
            return lRandomKaartIds.OrderBy(x => Global.RandomNumberGenerator.Next()).ToList();
        }

        private List<UIElement> GetBttnLijst(List<int> lGridKaartIds)
        {
            List<UIElement> lButtons = new List<UIElement>();

            // maak een eigen button style voor de kaarten
            Style bttnStyle = new Style(typeof(Button));
            bttnStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent));
            bttnStyle.Setters.Add(new Setter(Button.BorderBrushProperty, Brushes.Transparent));

            // laad de bitmap resource voor de achterkant van de kaart
            ImageSource achterKant = kaartResource.Find(GameData.sKaartAchterkant); ;

            foreach (int kaartId in lGridKaartIds)
            {
                // laad de  bitmap resource voor de voorkant van de kaart
                ImageSource voorKant = kaartResource.Find(kaartId.ToString());

                // maak de image die als content word gezet in de kaart button
                Image kaartImg = new Image();

                // laat vanuit het begin de achterkant van de kaart zien
                kaartImg.Source = achterKant;

                // maak de kaarData informatie die word gebruikt in de game logic
                // deze word in de tag dataveld geladen zodat die beschikbaar is
                // vanuit de object sender bij een click event
                KaartData kaartData = new KaartData();
                kaartData.ID = kaartId;          // Hiermee vergelijkt de logica de kaarten
                kaartData.voorKant = voorKant;       // resource voor de voorkant van de kaart
                kaartData.achterKant = achterKant;      // resource voor de achterkant van de kaart

                // maak de button voor de kaart
                Button kaartBttn = new Button();
                kaartBttn.Style = bttnStyle;    // onze custom style
                kaartBttn.Content = kaartImg;   // link de kaart image
                kaartBttn.Tag = kaartData;      // link de benodigde kaart data

                // TODO een eigen cursor
                kaartBttn.Cursor = Cursors.Hand;

                // registreer de callback functie voor de click event
                kaartBttn.Click += new RoutedEventHandler(Card_Click);

                lButtons.Add(kaartBttn);
            }
            return lButtons;
        }

        // callback voor de buttons
        // deze linkt het event naar de memory game logic MemoryLogic
        private void Card_Click(object sender, EventArgs e)
        {
            currentGame.memoryLogic.CardClick(sender, e);
        }
    }
}

