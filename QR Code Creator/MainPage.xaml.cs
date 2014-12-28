using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.IO;

namespace QR_Code_Creator
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if (!(App.Current as App).networkCheck())
            {
                textbox1.Visibility = Visibility.Collapsed;
                textblock1.Text = "No Internet Connection. Content can't be enabled!";
                // MessageBox.Show("No Active Internet Connection ");
               // textBlock1.Visibility = Visibility.Collapsed;
               // textBox1.Visibility = Visibility.Collapsed;
                button1.Visibility = Visibility.Collapsed;
               // textBlock2.Text = "No Internet Available. Please Check Your Data/Packet connection";
            }
            
          
        }
        
        private void button1_Click(object sender, RoutedEventArgs e)
        {

             try
            {
                if (textbox1.Text == "")
                {
                    MessageBox.Show("Enter Something Please!","QR Code Creator",MessageBoxButton.OK);
                }
                else
                {
                textbox1.Text = HttpUtility.UrlEncode(textbox1.Text);
               // ApplicationBar.IsVisible = false;
                WebClient webClient = new WebClient();
                string url = String.Format("http://api.qrserver.com/v1/create-qr-code/?size=150x150&data={0}", textbox1.Text);
                progressBar1.Visibility = System.Windows.Visibility.Visible;
                webClient.DownloadStringAsync(new Uri(url), UriKind.RelativeOrAbsolute);
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
                
                }
             }
                 catch(Exception )
             {
                 }
        
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            
            string url =String.Format("http://api.qrserver.com/v1/create-qr-code/?size=150x150&data={0}", textbox1.Text);
           // throw new NotImplementedException();
                     
            //image1.Source = imgSource;
            image1.SetValue(Image.SourceProperty,new BitmapImage(new Uri(url)));
            progressBar1.Visibility = System.Windows.Visibility.Collapsed;
            button1.IsEnabled = false;
            

        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (textbox1.Text != "")
            {
                WriteableBitmap wBmp = new WriteableBitmap(canvas1, null);
                MemoryStream ms = new MemoryStream();
                //bitmap.Render(canvas1, null);
                //bitmap.Invalidate();
                // bitmap.SaveJpeg
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("logo.jpg", FileMode.OpenOrCreate))
                    {
                        Extensions.SaveJpeg(wBmp, fileStream, wBmp.PixelWidth, wBmp.PixelHeight, 0, 100);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }

                if (ms.Length == 0 || ms == null || ms.Length == null)
                {
                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("logo.jpg", FileMode.Open))
                        {
                            fileStream.CopyTo(ms);
                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }
                }



                //Async operations can be pain ass!;
                //now wait!


                using (MediaLibrary lib = new MediaLibrary())
                {
                    lib.SavePicture("drawn", ms.GetBuffer());// Naam?? with extension?
                    lib.Dispose();
                    ms.Close();
                    ms.Dispose();

                    MessageBox.Show("Image Saved", "QR Code Creator", MessageBoxButton.OK);
                    button1.IsEnabled = true;

                }
            }
            else
            {
                MessageBox.Show("No Image to Save", "QR Code Creator", MessageBoxButton.OK);
            }
        }

        private void textbox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //canvas1.Children.Clear();
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {

        }
        private void clear_click(object sender, EventArgs e)
        {
            if (textbox1.Text == "")
            {
                MessageBox.Show("No Image to delete", "QR Code Creator", MessageBoxButton.OK);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Do you want to clear your image and enter a new text/URL?", "QR Code Creator", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    textbox1.Text = "";
                    textbox1.Focus();
                    //canvas1.Children.Clear();
                    image1.Source = null;
                    
                    button1.IsEnabled = true;
                   

                }
                else
                {
                    //do nothing
                }
            }
        }
    }
}