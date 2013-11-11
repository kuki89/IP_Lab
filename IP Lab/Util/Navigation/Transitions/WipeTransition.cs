using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


//*******************************************************************
//                                                                  *
//                            Page切换特效                   *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Util.Navigation.Transitions
{
    public class WipeTransition : TransitionBase
    {
        private WipeDirection direction;
        private UserControl newPage;
        private UserControl oldPage;
        private TimeSpan time;
        public WipeTransition(WipeDirection direction, TimeSpan duration)
        {
            time = duration;
            this.direction = direction;
        }
        public WipeTransition(WipeDirection direction)
            : this(direction, TimeSpan.FromSeconds(2))
        { }
        public override void PerformTranstition(UserControl newPage, UserControl oldPage)
        {
            this.newPage = newPage;
            this.oldPage = oldPage;

            Duration duration = new Duration(TimeSpan.FromSeconds(1));

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = duration;
            switch (direction)
            {
                case WipeDirection.LeftToRight:
                    animation.To = oldPage.ActualWidth;
                    break;
                case WipeDirection.RightToLeft:
                    animation.To = -oldPage.ActualWidth;
                    break;
            }

            Storyboard sb = new Storyboard();
            sb.Duration = duration;
            sb.Children.Add(animation);
            sb.Completed += sb_Completed;

            TranslateTransform sc = new TranslateTransform();
            oldPage.RenderTransform = sc;

            Storyboard.SetTarget(animation, sc);
            Storyboard.SetTargetProperty(animation, new PropertyPath("X"));

            //oldPage.Resources.Add(sb);

            sb.Begin();
        }

        void sb_Completed(object sender, EventArgs e)
        {
            OnTransitionCompleted(newPage, oldPage);
        }

        public enum WipeDirection
        {
            LeftToRight,
            RightToLeft
        }
    }
}
