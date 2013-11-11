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
    public class RotateTransition : TransitionBase
    {
        private UserControl newPage;
        private UserControl oldPage;
        private Point centre;
        private TimeSpan time;
        public RotateTransition(Point centre, TimeSpan duration)
        {
            time = duration;
            this.centre = centre;
        }
        public RotateTransition(Point centre)
            : this(centre, TimeSpan.FromSeconds(1))
        { }
        public RotateTransition()
            : this(new Point(0, 0))
        { }

        public override void PerformTranstition(UserControl newPage, UserControl oldPage)
        {
            this.newPage = newPage;
            this.oldPage = oldPage;

            Duration duration = new Duration(time);

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = duration;
            animation.To = 90;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;
            sb.Children.Add(animation);
            sb.Completed += sb_Completed;

            RotateTransform sc = new RotateTransform();
            sc.CenterX = centre.X * oldPage.ActualWidth;
            sc.CenterY = centre.Y * oldPage.ActualHeight;
            oldPage.RenderTransform = sc;

            Storyboard.SetTarget(animation, sc);
            //Storyboard.SetTargetProperty(animation, "Angle");
            Storyboard.SetTargetProperty(animation, new PropertyPath("Angle"));

            //oldPage.Resources.Add(sb);
            
            sb.Begin();
        }

        void sb_Completed(object sender, EventArgs e)
        {
            OnTransitionCompleted(newPage, oldPage);
        }
    }
}
