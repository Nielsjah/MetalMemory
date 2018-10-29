using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Windows.Data;

//using System.Diagnostics;

namespace MetalMemory
{
    public partial class HighScore : Page
    {
        public HighScore()
        {
            // registreer de functie onLoaded als eventHandler voor het event Loaded 
            Loaded += onLoaded;

            InitializeComponent();
        }

        public void AddScores(List<MemoryPlayer> lPlayers)
        {
            // load de highscores xml file of maak een nieuwe
            XDocument highScoreXml = new XDocument();
            highScoreXml.Add(new XElement("HighScores"));

            if (File.Exists("HighScore.xml"))
                highScoreXml = XDocument.Load("HighScore.xml");

            // vul de highscores van de spelers in in de highscore xml file
            foreach (MemoryPlayer player in lPlayers)
            {
                XElement highScore = new XElement("highScore");
                highScore.Add(new XAttribute("Naam", player.naam));
                highScore.Add(new XAttribute("Score", string.Format("{0}", player.score)));
                highScore.Add(new XAttribute("Gewonnen", string.Format("{0}", player.gewonnen)));
                highScoreXml.Element("HighScores").Add(highScore);
            }

            // zet de highscores of volgorde van hoog naar laag
            IEnumerable<XElement> highScores = highScoreXml.Element("HighScores").Elements();
            IOrderedEnumerable<XElement> highScoresGesorteerd = 
                highScores.OrderByDescending(node => Convert.ToInt32(node.Attribute("Score").Value));
            highScoreXml = new XDocument(new XElement("HighScores", highScoresGesorteerd));

            // save de highscores xml file
            highScoreXml.Save("HighScore.xml");
       }

        public void onLoaded(object sender, EventArgs e)
        {
            // herlaad de HighScore.xml data
            HighScoreData.Refresh();
        }
    }
}



