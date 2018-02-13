﻿using System.Xml.Linq;
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
            topEl.SetValue(Top);
            mainWindowInfoEl.Add(topEl);


            XElement leftEl = new XElement(MAINWINDOW_LEFT_EL);
            leftEl.SetValue(Left);
            mainWindowInfoEl.Add(leftEl);

            XElement isMaximizedEl = new XElement(MAINWINDOW_ISMAXIMIZED_EL);
            isMaximizedEl.SetValue(IsMaximized);
            mainWindowInfoEl.Add(isMaximizedEl);

            return mainWindowInfoEl;
        }

        #endregion  მეთოდები 


        #region  სტატიკური_მეთოდები

        public static MainWindowInfo GetFromXml(XElement mainWindowInfoEl)
        {
            MainWindowInfo mainWindowInfo = new MainWindowInfo();

            if (mainWindowInfoEl != null)
            {
                XElement widthEl = mainWindowInfoEl.Element(MAINWINDOW_WIDTH_EL);

                if (widthEl != null)
                {
                    try
                    {
                        mainWindowInfo.Width = widthEl.GetValueAsDouble();
                    }
                    catch
                    {
                        // Malformed XML
                        return null;
                    }


                }
                else
                {
                    // Malformed XML
                    return null;
                }

                XElement heightEl = mainWindowInfoEl.Element(MAINWINDOW_HEIGHT_EL);

                if (heightEl != null)
                {
                    try
                    {
                        mainWindowInfo.Height = heightEl.GetValueAsDouble();
                    }
                    catch
                    {
                        //  Malformed XML
                        return null;
                    }

                    XElement topEl = mainWindowInfoEl.Element(MAINWINDOW_TOP_EL);

                    if (topEl != null)
                    {
                        try
                        {
                            mainWindowInfo.Top = topEl.GetValueAsDouble();
                        }
                        catch
                        {
                            // Malformed XML
                            return null;
                        }
                    }
                    else
                    {
                        //  Malformed XML
                        return null;
                    }

                    XElement leftEl = mainWindowInfoEl.Element(MAINWINDOW_LEFT_EL);

                    if (leftEl != null)
                    {
                        try
                        {
                            mainWindowInfo.Left = leftEl.GetValueAsDouble();
                        }
                        catch
                        {
                            // Malformed  XML 
                            return null;
                        }


                    }
                    else
                    {
                        // Malformed  XML 
                        return null;
                    }

                    XElement isMaximizedEl = mainWindowInfoEl.Element(MAINWINDOW_ISMAXIMIZED_EL);

                    if (isMaximizedEl != null)
                    {
                        try
                        {
                            mainWindowInfo.IsMaximized = isMaximizedEl.GetValueAsBool();
                        }
                        catch
                        {
                            // Malformed  XML
                            return null;
                        }
                    }
                    else
                    {
                        //  Malformed  XML
                        return null;
                    }


                }

            }
                return mainWindowInfo;
         }
            #endregion სტატიკური_მეთოდები

    }
}

