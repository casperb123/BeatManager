using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for ModelSaberTagUserControl.xaml
    /// </summary>
    public partial class ModelSaberTagUserControl : UserControl
    {
        private bool isCopying;

        public string ModelTag { get; set; }

        public ModelSaberTagUserControl(string modelTag)
        {
            InitializeComponent();
            DataContext = this;
            ModelTag = modelTag;
        }

        private async void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(ModelTag);

            if (!isCopying)
            {
                isCopying = true;

                await Task.Run(async () =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        border.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        border.Background.Opacity = .2;
                    });
                    await Task.Delay(500);
                    Dispatcher.Invoke(() =>
                    {
                        border.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        border.Background.Opacity = .2;
                    });
                });

                isCopying = false;
            }
        }
    }
}
