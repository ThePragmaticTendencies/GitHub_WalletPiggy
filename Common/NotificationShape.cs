using System;
using CostDaily;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CostDaily.Common
{
    class NotificationShape : FrameworkElement
    {
        private int _stroke = 2;
        private int _pointerPositionX = 7;
        private int _pointerPositionY = 0;
        private int _pointerHeight = 5;
        private int _pointerLength = 8;
        private int _notificationLength = 0;
        private int _notificationHeight = 0;
        private double _scaleFactor = App.ScaleFactor;
        private static int _margin = 1;
        private double _topMargin;


        public NotificationShape()
        {

        }

        public Polygon ReturnNotificationPolygon(Grid Parent, double height, double width)
        {
            //_notificationHeight = (int)Math.Floor((Parent.ActualHeight));
            //_notificationLength = (int)Math.Floor((Parent.ActualWidth));

            _notificationHeight = (int)Math.Floor(height);
            _notificationLength = (int)Math.Floor(width);
            _topMargin = (_pointerHeight + 1 * _margin) * _scaleFactor;

            if (_topMargin < 9.6d) _topMargin = 9.6;
            int _notificationStartX = (_stroke/2);

            Polygon notificationPolygon = new Polygon();

            notificationPolygon.Stroke = new SolidColorBrush(Colors.MediumTurquoise);
            notificationPolygon.StrokeThickness = _stroke;
            notificationPolygon.Fill = new SolidColorBrush(Colors.White);
            notificationPolygon.Margin = new Thickness(_margin, _topMargin, _scaleFactor*_margin, _margin);


            notificationPolygon.Points.Add(new Windows.Foundation.Point(_notificationStartX, _notificationHeight-_pointerHeight));
            notificationPolygon.Points.Add(new Windows.Foundation.Point(_notificationLength-_pointerPositionX-_pointerLength, _notificationHeight - _pointerHeight));
            notificationPolygon.Points.Add(new Windows.Foundation.Point(_notificationLength - _pointerPositionX, _notificationHeight - _pointerPositionY));
            notificationPolygon.Points.Add(new Windows.Foundation.Point(_notificationLength - _pointerPositionX, _notificationHeight - _pointerHeight));
            notificationPolygon.Points.Add(new Windows.Foundation.Point(_notificationLength, _notificationHeight - _pointerHeight));
            notificationPolygon.Points.Add(new Windows.Foundation.Point(_notificationLength, _notificationHeight - _notificationHeight));
            notificationPolygon.Points.Add(new Windows.Foundation.Point(_notificationStartX, _notificationHeight - _notificationHeight));

            notificationPolygon.HorizontalAlignment = HorizontalAlignment.Center;
            notificationPolygon.VerticalAlignment = VerticalAlignment.Center;

            return notificationPolygon; 
        }


    }
}
