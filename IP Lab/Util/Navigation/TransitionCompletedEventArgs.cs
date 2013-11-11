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
    /// EventArgs class which is used for the TransitionCompleted event
    /// fired by TransitionBase
    /// </summary>
    public class TransitionCompletedEventArgs : EventArgs
    {
        public UserControl NewPage { get; set; }
        public UserControl OldPage { get; set; }
    }
}
