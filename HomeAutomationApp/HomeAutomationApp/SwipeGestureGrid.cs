using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeAutomationApp
{
    public class SwipeGestureGrid : Grid
    {
        Command c = new Command(() =>
        {

        });

        public static readonly BindableProperty SwipeLeftCommandProperty =
            BindableProperty.Create("SwipeLeftCommand", typeof(ICommand), typeof(ICommand), null);
        public static readonly BindableProperty SwipeRightCommandProperty =
            BindableProperty.Create("SwipeRightCommand", typeof(ICommand), typeof(ICommand), null);
        public static readonly BindableProperty SwipeUpCommandProperty =
            BindableProperty.Create("SwipeUpCommand", typeof(ICommand), typeof(ICommand), null);
        public static readonly BindableProperty SwipeDownCommandProperty =
            BindableProperty.Create("SwipeDownCommand", typeof(ICommand), typeof(ICommand), null);
        public static readonly BindableProperty TappedCommandProperty =
            BindableProperty.Create("TappedCommand", typeof(ICommand), typeof(ICommand), null);

        private double _gestureStartX;
        private double _gestureStartY;
        private double _gestureDistanceX;
        private double _gestureDistanceY;

        public double GestureStartX
        {
            get => _gestureStartX;
            private set
            {
                _gestureStartX = value;
                OnPropertyChanged();
            }
        }

        public double GestureStartY
        {
            get => _gestureStartY;
            private set
            {
                _gestureStartY = value;
                OnPropertyChanged();
            }
        }

        private bool IsSwipe { get; set; }

        public ICommand TappedCommand
        {
            get => (ICommand)GetValue(TappedCommandProperty);
            set => SetValue(TappedCommandProperty, value);
        }
        public ICommand SwipeLeftCommand
        {
            get => (ICommand)GetValue(SwipeLeftCommandProperty);
            set => SetValue(SwipeLeftCommandProperty, value);
        }
        public ICommand SwipeRightCommand
        {
            get => (ICommand)GetValue(SwipeRightCommandProperty);
            set => SetValue(SwipeRightCommandProperty, value);
        }
        public ICommand SwipeUpCommand
        {
            get => (ICommand)GetValue(SwipeUpCommandProperty);
            set => SetValue(SwipeUpCommandProperty, value);
        }
        public ICommand SwipeDownCommand
        {
            get => (ICommand)GetValue(SwipeDownCommandProperty);
            set => SetValue(SwipeDownCommandProperty, value);
        }

        public SwipeGestureGrid()
        {
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += PanGesture_PanUpdated;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += TapGesture_Tapped;

            GestureRecognizers.Add(panGesture);
            GestureRecognizers.Add(tapGesture);
        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!IsSwipe)
                    TappedCommand?.Execute(this);

                IsSwipe = false;
            }
            catch (Exception ex)
            {

            }
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    {
                        GestureStartX = e.TotalX;
                        GestureStartY = e.TotalY;
                    }
                    break;
                case GestureStatus.Running:
                    {
                        _gestureDistanceX = e.TotalX;
                        _gestureDistanceY = e.TotalY;
                    }
                    break;
                case GestureStatus.Completed:
                    {
                        IsSwipe = true;

                        if (Math.Abs(_gestureDistanceX) > Math.Abs(_gestureDistanceY))
                        {
                            if (_gestureDistanceX > 0)
                            {
                                SwipeRightCommand?.Execute(this);
                            }
                            else
                            {
                                SwipeLeftCommand?.Execute(null);
                            }
                        }
                        else
                        {
                            if (_gestureDistanceY > 0)
                            {
                                SwipeDownCommand?.Execute(null);
                            }
                            else
                            {
                                SwipeUpCommand?.Execute(null);
                            }
                        }
                    }
                    break;
            }
        }
    }
}
