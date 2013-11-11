using System;
using System.Windows.Controls;

//*******************************************************************
//                                                                  *
//                           用于Page之间的切换                   *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Util.Navigation
{
    /// <summary>
    /// Represents a transition between two UserControls
    /// </summary>
    public abstract class TransitionBase
    {
        /// <summary>
        /// Fired when the transition is complete and the NavigationHelper can
        /// remove the old UserControl. Implementors must fire the event when
        /// the transition is complete.
        /// </summary>
        public event EventHandler<TransitionCompletedEventArgs> TransitionCompleted;
        protected void OnTransitionCompleted(UserControl newPage, UserControl oldPage)
        {
            if (TransitionCompleted != null)
                TransitionCompleted(this, new TransitionCompletedEventArgs() { NewPage = newPage, OldPage = oldPage });
        }

        /// <summary>
        /// Performs a transition between two UserControls.
        /// </summary>
        /// <param name="newPage">The new UserControl to be displayed.</param>
        /// <param name="oldPage">The old UserControl to be removed.</param>
        public abstract void PerformTranstition(UserControl newPage, UserControl oldPage);
    }
}
