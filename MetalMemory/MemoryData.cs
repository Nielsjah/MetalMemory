using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

//using System.Diagnostics;

namespace MetalMemory
{
    public struct MemoryScore
    {
        public struct Match
        {
            public const int ScorePerKaart = 5;
            public const double BonusMultiplier = 1.5;
        }
    }

    public struct GameData
    {
        public const int nKaartPNGs = 32;
        public const string sKaartLocatie = "Images/Cards/Card{0}.png";
        public const string sKaartAchterkant = "Back";
        public const string sSaveFile = "memory.sav";
        public const string sOverlayLocatie = "Images/Overlays/{0}.png";
        public const string sOverlayFout = "Cancel";
        public const string sOverlayGoed = "Checkmark";
        public const int speeltijdTimerTurn = 30;
    }

    public enum Kaart { voorkant, achterkant }
    public enum TimerMode { timerTurn, roundAbout }

    public class Context
    {
        public Frame menuFrame { get; }
        public Frame contentFrame { get; }
        public Grid gameGrid { get; }
        public HighScore highScore { get; set; }
        public HoofdMenu hoofdMenu { get; set; }
        public Settings settings { get; set; }
        public GameInterface gameInterface { get; set; }
        public MemoryGame memoryGame { get; set; }

        public Context(Frame frameMenu, Frame frameContent, Grid gridGame)
        {
            menuFrame = frameMenu;
            contentFrame = frameContent;
            gameGrid = gridGame;
        }
    }

    [Serializable]
    public class GameStatus
    {
        public bool kaartenGelijk { get; set; }
        public bool gelijkSpel { get; set; }
        public int aantalGoed { get; set; }
        public int aantalOver { get; set; }
        public int playerGewonnen { get; set; }
        public int speeltijd { get; set; }
        public int timeRemaining { get; set; } = GameData.speeltijdTimerTurn;
        public TimerMode timerMode { get; set; }

        public GameStatus(TimerMode timerMode, int aantalUniek) { this.timerMode = timerMode; aantalOver = aantalUniek; }
        public void Reset() { kaartenGelijk = false; }
    }

    [Serializable]
    public class MemoryPlayer
    {
        public bool gewonnen { get; set; } = false;
        public string naam { get; private set; }
        public int score { get; set; } = 0;
        public int speeltijd { get; set; } = 0;

        public MemoryPlayer(string naam) { this.naam = naam; }
    }

    [Serializable]
    public class MemoryPlayers
    {
        public int aanZet;
        public int prevSpeler;
        public List<MemoryPlayer> players { get; set; } = new List<MemoryPlayer>();

        public void AddPlayer(string name) { players.Add(new MemoryPlayer(name)); }
        public MemoryPlayer CurrentPlayer() { return players[aanZet]; }
        public void NextPlayer() { prevSpeler = aanZet; if (++aanZet == players.Count) aanZet = 0; }
    }

    public struct Move
    {
        public Button kaartBttn { get; set; }
        public KaartData kaartData { get; set; }
        public Image kaartImg { get; set; }
    }

    public class KaartData
    {
        public int ID { get; set; }
        public ImageSource voorKant { get; set; }
        public ImageSource achterKant { get; set; }
        public KaartStatus status { get; set; } = new KaartStatus();
    }

    [Serializable]
    public class KaartStatus
    {
        public bool gespeeld { get; set; }
    }

    public class PlayerBord : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string Naam;
        private string PopupScore;
        private int Score;
        private string Speeltijd;

        public Control pControl { get; private set; }
        public Image status { get; private set; }
        public string naam { get { return Naam; } set { Naam = value; nieuweData("naam"); } }
        public string popupScore { get { return PopupScore; } set { PopupScore = value; nieuweData("popupScore"); } }
        public int score { get { return Score; } set { Score = value; nieuweData("score"); } }
        public string speeltijd { get { return Speeltijd; } set { Speeltijd = value; nieuweData("speeltijd"); } }
        public PlayerBord(FrameworkElement root)
        {
            pControl = new Control();
            pControl.Template = (ControlTemplate)root.FindResource("playerBordTemplate");
            pControl.ApplyTemplate();
            pControl.DataContext = this;
            pControl.Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e) { status = (Image)pControl.Template.FindName("playerIcon", pControl); }
        private void nieuweData(string dataNaam) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(dataNaam)); }
    }
}
