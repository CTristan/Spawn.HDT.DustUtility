using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;
using System.Windows.Input;

namespace Spawn.HDT.DustUtility.UI.Components
{
    public partial class CardsDataGrid
    {
        #region Member Variables
        private Point? m_startPosition;
        private GridItem m_draggedItem;
        #endregion

        #region Events
        #region OnDataGridMouseMove
        private void OnDataGridMouseMove(object sender, MouseEventArgs e)
        {
            //if (cardImagePopup.IsOpen)
            //{
            //    Point position = e.GetPosition(dataGrid);

            //    cardImagePopup.HorizontalOffset = position.X + 20;
            //    cardImagePopup.VerticalOffset = position.Y;
            //}
            //else { }

            if (AllowDrag && (m_startPosition != null && m_startPosition.HasValue))
            {
                if (m_draggedItem == null && dataGrid.SelectedIndex > -1)
                {
                    m_draggedItem = dataGrid.SelectedItem as GridItem;
                }
                else { }

                Point position = e.GetPosition(null);

                Vector diff = m_startPosition.Value - position;

                if (m_draggedItem != null
                    && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                    && Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    System.Diagnostics.Debug.WriteLine($"dragging {m_draggedItem.Name}");

                    DataObject data = new DataObject("item", m_draggedItem);

                    DragDrop.DoDragDrop(dataGrid, data, DragDropEffects.Copy);

                    m_startPosition = null;

                    m_draggedItem = null;
                }
                else { }
            }
            else { }
        }
        #endregion

        #region OnDataGridPreviewMouseLeftButtonDown
        private void OnDataGridPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_startPosition = e.GetPosition(null);
        }
        #endregion

        #region OnDataGridDragEnter
        private void OnDataGridDragEnter(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("drag enter");

            if (!e.Data.GetDataPresent("item"))
            {
                e.Effects = DragDropEffects.None;
            }
            else { }
        }
        #endregion

        #region OnDataGridDrop
        private async void OnDataGridDrop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("drop");

            if (e.Data.GetDataPresent("item"))
            {
                GridItem item = (e.Data.GetData("item") as GridItem).CreateCopy();

                System.Diagnostics.Debug.WriteLine($"Dropped {item.Name}");

                if (item.Count > 1)
                {
                    MetroWindow window = Window.GetWindow(this) as MetroWindow;

                    string strResult = await window.ShowInputAsync(string.Empty, "How many copies?");

                    int nNewCount = -1;

                    try
                    {
                        nNewCount = Convert.ToInt32(strResult);
                    }
                    catch
                    {
                        //Invalid input
                    }

                    if (nNewCount > -1)
                    {
                        item.Count = nNewCount;
                        item.Dust = item.Tag.GetDustValue(nNewCount);
                    }
                    else { }
                }
                else { }

                if (item.Count > 0)
                {
                    GridItems.Add(item);
                }
                else { }
            }
            else { }
        }
        #endregion
        #endregion
    }
}