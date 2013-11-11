using System.Windows;
using System.Windows.Controls;

//*******************************************************************
//                                                                  *
//                           用于Page之间的切换                   *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Util.Navigation
{
    /// <summary>
    /// Provides methods to allow page transitions between two UserControls.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// A reference to our root xaml element for our app.
        /// </summary>
        private static Grid root;
        /// <summary>
        /// Provides methods to allow page transitions between two UserControls.
        /// </summary>
        static NavigationHelper()
        {
            root = Application.Current.RootVisual as Grid;
        }

        /// <summary>
        /// Causes a new UserControl to be displayed taking over from the old one
        /// using the supplied transition.
        /// </summary>
        /// <param name="transition">The transition to use.</param>
        /// <param name="newPage">The new UserControl to display.</param>
        public static void Navigate(TransitionBase transition, UserControl newPage)
        {
            UserControl oldPage = root.Children[0] as UserControl;
            root.Children.Insert(0, newPage);

            transition.TransitionCompleted += transition_TransitionCompleted;
            transition.PerformTranstition(newPage, oldPage);
        }

        /// <summary>
        /// Causes a new UserControl to be displayed taking over from the old one.
        /// </summary>
        /// <param name="newPage">The new UserControl to display.</param>
        public static void Navigate(UserControl newPage)
        {
            UserControl oldPage = root.Children[0] as UserControl;
            root.Children.Add(newPage);
            root.Children.Remove(oldPage);
        }

        /// <summary>
        /// Fired when a transition is completed, this allows us to remove the
        /// old UserControl from the tree.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The TransitionCompletedEventArgs.</param>
        private static void transition_TransitionCompleted(object sender, TransitionCompletedEventArgs e)
        {
            root.Children.Remove(e.OldPage);
        }
    }
}
