using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.RichEditor.Shared;
using Xamarin.Forms;

namespace RichEditorSample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            var html = $"<html><body>Testando....</body></html>";
            webview = new WebView
            {
                Source = new HtmlWebViewSource
                {
                    Html = html
                }
            };

            var texto = await CrossRichEditor.Current.ShowAsync("Começe por aqui");

            if (texto.IsSave)
            {
                var html2 = $"<html><body>{texto.Html}</body></html>";

                webview = new WebView
                {
                    Source = new HtmlWebViewSource
                    {
                        Html = html2
                    }
                };
            }
        }
    }
}