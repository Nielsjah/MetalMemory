using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;

namespace MetalMemory
{
    public partial class App : Application
    {
        MemoryMuziek achtergrondMuziek;

        public App()
        {
            // initialiseer de willekeurig nummer generator
            Global.RandomNumberGenerator = new Random();
            Global.BinaryFormatter = new BinaryFormatter();

            // start de achtergrond muziek
            achtergrondMuziek = new MemoryMuziek();
        }
    }

    // dingen globaal beschikbaar - die dus niet onnodig dubbel word gecreëerd
    public static class Global
    {
        public static Random RandomNumberGenerator;
        public static BinaryFormatter BinaryFormatter;
    }

    // class extensies - voor betere leesbaarheid etc
    public static class Extensies
    {
        // String class extensies
        public static bool notEmpty(this string text) { return !string.IsNullOrEmpty(text); }

        // Stream class extensies
        public static Object Load(this Stream stream) { return Global.BinaryFormatter.Deserialize(stream); }
        public static void Save(this Stream stream, Object obj) { Global.BinaryFormatter.Serialize(stream, obj); }

        // Frame class extensies
        public static void NavigateBlank(this Frame frame) { frame.Navigate("about:blank"); }

        // List class extensies
        //public static void RemoveAll<T>(this List<T> list) { list.RemoveRange(0, list.Count); }
        public static void RemoveFirst<T>(this List<T> list) { list.RemoveAt(0); }
        public static void RemoveLast<T>(this List<T> list) { list.RemoveAt(list.Count - 1); }
    }
}
