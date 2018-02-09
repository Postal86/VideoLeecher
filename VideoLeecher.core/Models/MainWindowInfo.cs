using System.Xml.Linq;
using VideoLeecher.shared.Extensions;


namespace VideoLeecher.core.Models
{
    public class MainWindowInfo
    {
        #region კონსტანტები

        public const string MAINWINDOW_EL = "MainWindow";

        private const string MAINWINDOW_WIDTH_EL = "Width";
        private const string MAINWINDOW_HEIGHT_EL = "Height";
        private const string MAINWINDOW_TOP_EL = "Top";
        private const string MAINWINDOW_LEFT_EL = "Left";
        private const string MAINWINDOW_ISMAXIMIZED_EL = "IsMaximized";



        #endregion კონსტანტები

        #region თვისებები

        public double Width { get; set; }

        public double Height { get; set; } 

        public double Top { get; set; }

        public double Left { get; set; }

        public bool IsMaximized { get; set; }


        #endregion თვისებები

        #region მეთოდები

        public XElement GetXML()
        {
            XElement mainWindowInfoEl = new XElement(MAINWINDOW_EL);

            XElement widthEl = new XElement(MAINWINDOW_WIDTH_EL);
            widthEl.SetValue(Width);
            mainWindowInfoEl.Add(widthEl);

            XElement heightEl = new XElement(MAINWINDOW_HEIGHT_EL);
            heightEl.SetValue(Height);
            mainWindowInfoEl.Add(heightEl);

            XElement topEl = new XElement(MAINWINDOW_TOP_EL);
            topEl.SetValue(Left);
            mainWindowInfoEl.Add(topEl);




        }







        #endregion  მეთოდები 


    }
}
