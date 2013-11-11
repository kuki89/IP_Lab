using System;
using System.Windows.Controls;

//*******************************************************************
//                                                                  *
//                            Page切换特效                   *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Util.Navigation.Transitions
{
    public class CompositeTransition : TransitionBase
    {
        private TransitionBase transitionOne;
        private TransitionBase[] transitions;
        private Int32 count;

        private UserControl newPage;
        private UserControl oldPage;

        public CompositeTransition(TransitionBase transitionOne, params TransitionBase[] transitions)
        {
            this.transitionOne = transitionOne;
            this.transitions = transitions;
        }

        public override void PerformTranstition(UserControl newPage, UserControl oldPage)
        {
            this.newPage = newPage;
            this.oldPage = oldPage;
            transitionOne.TransitionCompleted += transition_TransitionCompleted;
            transitionOne.PerformTranstition(newPage, oldPage);
            foreach (TransitionBase transition in transitions)
            {
                transition.TransitionCompleted += transition_TransitionCompleted;
                transition.PerformTranstition(newPage, oldPage);
            }
        }

        private void transition_TransitionCompleted(object sender, TransitionCompletedEventArgs e)
        {
            count++;
            if (count == transitions.Length + 1)
                OnTransitionCompleted(newPage, oldPage);
        }
    }
}
