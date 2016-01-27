using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDemo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Provides a reusable ToolTip object used by GridSplitter_DragDelta and GridSplitter_DragComplete
        /// </summary>
        private ToolTip flyingToolTip = new ToolTip();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows a tooltip with the actual width of the left grid column near the registered GridSplitter column while it is dragging
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (sender.GetType() != typeof(GridSplitter))
            { return; }

            ShowActualWidthToolTip(gs: (sender as GridSplitter), tt: flyingToolTip);
        }

        private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            flyingToolTip.IsOpen = false;
        }
        /// <summary>
        /// ShowActualWidthToolTip shows actual width of the left and right column near the GridSplitter column, so one can split two columns precisely
        /// </summary>
        /// <param name="gs"></param>
        /// <param name="tt"></param>
        // TODO: MainWindow.ShowActualWidthToolTip seems to be to tricky for reusability, maybe one find a more scaleable solution
        private void ShowActualWidthToolTip(GridSplitter gs, ToolTip tt)
        {
            // If the GridSplitter isn't positioned correctly in a seperate column between two other columns, drop functionality
            Grid parentGrid = gs.Parent as Grid;
            double? leftColumnActualWidth = null;
            double? rightColumnActualWidth = null;
            try 
            {
                leftColumnActualWidth = parentGrid.ColumnDefinitions[(Grid.GetColumn(gs) - 1)].ActualWidth;
                rightColumnActualWidth = parentGrid.ColumnDefinitions[Grid.GetColumn(gs) + 1].ActualWidth;
 
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("Something went wrong in your GridSplitter layout. Splitter must been set in a column between the two columns who method tries to evaluate actual width. \n\n" + ex.Message, "Error", MessageBoxButton.OK);
            }

            tt.Content = String.Format("\u21E4 Width left {0} | {1} Width right \u21E5", leftColumnActualWidth, rightColumnActualWidth);
            tt.PlacementTarget = this;
            tt.Placement = PlacementMode.Relative;
            tt.HorizontalOffset = (Mouse.GetPosition(this).X - (tt.ActualWidth / 2));
            tt.VerticalOffset = (Mouse.GetPosition(this).Y + 10);
            tt.IsOpen = true;
            return;
        }
        /// <summary>
        /// Prepares a button for moving from one panel to another panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// TODO: Implement fallback, if button wasn't drop down to another panel
        private void btnDragDrop_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = sender as Button;

            // TODO: This should be done on another place, but where?
            Panel parent = btn.Parent as Panel;
            parent.Children.Remove(btn);
            
            DragDrop.DoDragDrop(btn, btn, DragDropEffects.Move);
        }
        /// <summary>
        /// Prepares a panel for moving in an element via drag and drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_Drop(object sender, DragEventArgs e)
        {
           Panel panel = sender as Panel;
           panel.Children.Add(((UIElement)e.Data.GetData(typeof(Button))));
        }
    }
}
