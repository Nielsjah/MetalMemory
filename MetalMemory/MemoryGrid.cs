using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

//using System.Diagnostics;


namespace MetalMemory
{
    public class MemoryGrid
    {
        private Grid hGrid;
        private int nColumns;
        private int nRows;

        public MemoryGrid(Grid grid)
        {
            // setup de benodigde data
            hGrid = grid;
        }

        public void Add(UIElement obj)
        {
            if (!hGrid.Children.Contains(obj))
                hGrid.Children.Add(obj);
        }

        public void Add(UIElement obj, int column, int row)
        {
            SetLocation(obj, column, row);
            Add(obj);
        }

        public void AddByIndex(UIElement obj, int nIndex)
        {
            int column = nIndex % nRows;
            int row = nIndex / nRows;
            Add(obj, column, row);
        }

        public void SetLocation(UIElement obj, int column, int row)
        {
            Grid.SetColumn(obj, column);
            Grid.SetRow(obj, row);
        }

        public void Delete(List<UIElement> lObj)
        {
            foreach (UIElement obj in lObj) hGrid.Children.Remove(obj);
            if (hGrid.ColumnDefinitions.Count != 0)
                hGrid.ColumnDefinitions.RemoveRange(0, hGrid.ColumnDefinitions.Count);
            if (hGrid.RowDefinitions.Count != 0)
                hGrid.RowDefinitions.RemoveRange(0, hGrid.RowDefinitions.Count);
        }

        public void Create(int columns, int rows)
        {
            nColumns = columns;
            nRows = rows;

            for (int i = 0; i++ < nColumns;)
                hGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i++ < nRows;)
                hGrid.RowDefinitions.Add(new RowDefinition());
        }
    }
}