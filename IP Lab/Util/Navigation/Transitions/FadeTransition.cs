using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

//*******************************************************************
//                                                                  *
//                            Page切换特效                   *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Util.Navigation.Transitions
{
    public class FadeTransition : TransitionBase
    {
        private UserControl newPage;
        private UserControl oldPage;
        private TimeSpan time;
        public FadeTransition(TimeSpan duration)
        {
            time = duration;
        }
        public FadeTransition() : this(TimeSpan.FromSeconds(2))
        { }

        public override void PerformTranstition(UserControl newPage, UserControl oldPage)
        {
            this.newPage = newPage;
            this.oldPage = oldPage;

            Duration duration = new Duration(time);

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = duration;
            animation.To = 0;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;
            sb.Children.Add(animation);
            sb.Completed += sb_Completed;

            Storyboard.SetTarget(animation, oldPage);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            //oldPage.Resources.Add(sb);
            
            sb.Begin();
        }

        void sb_Completed(object sender, EventArgs e)
        {
            OnTransitionCompleted(newPage, oldPage);
        }
    }
}
