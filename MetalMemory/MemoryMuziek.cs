using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

using System.Diagnostics;

namespace MetalMemory
{
    class MemoryMuziek
    {
        private MediaPlayer mediaPlayer;
        private List<Uri> lMuziek;
        private int nLaatstGespeeld = 0;
       
        public MemoryMuziek()
        {
            mediaPlayer = new MediaPlayer();

            // registreer de functie MuziekGestopt als eventHandler voor het event MediaEnded 
            // deze functie wordt aangeroepen als het einde van de muziek is berijkt
            mediaPlayer.MediaEnded += MuziekGestopt;

            // registreer de functie MuziekGeopend als eventhandler voor het event MediaOpened
            // deze functie wordt aangeroepen nadat een muziekbestand is geopend
            mediaPlayer.MediaOpened += MuziekGeopend;

            // maakt een lijst van URI objecten van de mp3 files in de "muziek" directory
            lMuziek = GetMuziekLijst("Muziek");

            // start met het spelen van een random gekozen mp3
            PlayRandom();
        }

        private void MuziekGestopt(object sender, EventArgs e)
        {
            // als de muziek is gestopt speel dan een andere random mp3
            PlayRandom();
        }

        private void MuziekGeopend(object sender, EventArgs e)
        {
            // als een mp3 file is geladen speel de dan af
            mediaPlayer.Play();
        }

        private void PlayRandom()
        {
            // als de muziek URI lijst leeg is doen we niks
            if (lMuziek.Count == 0) return;

            // als maar één URI beschikbaar is hoeft er geen random te worden gekozen
            // dit doen we door nLaatstGespeeld op 0 te zetten zodat RamdomIndex altijd anders is
            if (lMuziek.Count == 1) nLaatstGespeeld = 0;

            // kies een random index die de URI representeerd
            // als de random index gelijk is aan nLaatstGespeeld kies een ander
            int RandomIndex;
            do  RandomIndex = Global.RandomNumberGenerator.Next(lMuziek.Count);
            while (RandomIndex == nLaatstGespeeld);

            // bewaar de geselecteerde index voor het vergelijken de volgende keer
            nLaatstGespeeld = RandomIndex;

            // laad de URI die de index representeerd
            Uri RandomMuziek = lMuziek[RandomIndex];

            // open de muziek URI
            // dit triggered vervolgends de MediaOpened event
            mediaPlayer.Open(RandomMuziek);
        }

        private List<Uri> GetMuziekLijst(string muziekDirectory)
        {
            List<Uri> lMuziekURIs = new List<Uri>();

            // als de directry niet bestaat geef een lege lijst terug
            if (!Directory.Exists(muziekDirectory)) return lMuziekURIs;

            // maak een string array van alle files in de directory
            String[] fileEntries = Directory.GetFiles(muziekDirectory);

            // loop door alle file strings heen
            foreach (string sFile in fileEntries)
            {
                // als de extensie van een file geen mp3 is slaan we deze file over
                string fileExtension = Path.GetExtension(sFile);
                if (fileExtension != ".mp3") continue;

                // mmak een URI object van de file
                Uri muziekURI = new Uri(sFile, UriKind.Relative);

                lMuziekURIs.Add(muziekURI);
            }

            return lMuziekURIs;
        }
    }

    //TODO

    //Debug.WriteLine(Directory.GetCurrentDirectory());
    //String[] subdirectoryEntries = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\Muziek");

    //foreach(string text in subdirectoryEntries)
    //{
    //    Debug.WriteLine(text);
    //}
}
