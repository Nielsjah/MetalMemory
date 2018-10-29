using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

//using System.Diagnostics;

namespace MetalMemory
{
    class MemoryResource
    {
        private List<ImageSource> lBmpResources;

        private string sResourceLocatie;

        public MemoryResource(string sLocatie)
        {
            lBmpResources = new List<ImageSource>();
            sResourceLocatie = sLocatie;
        }

        public void Add(object sResource)
        {
            BitmapImage resourceImg = GetResourcBitmap(sResource);
            lBmpResources.Add(resourceImg);
        }

        public void AddList(List<int> lIdInts)
        {
            List<object> lIdObjects = lIdInts.Cast<object>().ToList();
            SetResourceLijst(lIdObjects);
        }

        public ImageSource Find(string sResource)
        {
            // zoek de eerste resource die een sResource string bevat
            ImageSource resource = lBmpResources.Find(rsrc => rsrc.ToString().Contains(sResource));

            return resource;
        }

        private void SetResourceLijst(List<object> lSymboolId)
        {
            // sorteer de gebruikte resource Id lijst
            lSymboolId.Sort();

            // loop door de geruikte resource Ids
            foreach (int symboold in lSymboolId)
            {
                // voeg de resource bitmap toe aan de resource lijst
                Add(symboold);
            }
        }

        private BitmapImage GetResourcBitmap(object resource)
        {
            // bouw de locatie string van de resource
            string locKaartImg = String.Format(sResourceLocatie, resource);

            // maak een URI van de locatie string
            Uri uriResourceBitmap = new Uri(locKaartImg, UriKind.Relative);

            // maak een bitmap van de URI
            BitmapImage resourceBitmap = new BitmapImage(uriResourceBitmap);

            return resourceBitmap;
        }
    }
}
