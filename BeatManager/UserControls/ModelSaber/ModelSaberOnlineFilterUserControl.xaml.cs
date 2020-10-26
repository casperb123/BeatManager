using BeatManager.Events;
using ModelSaber.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for ModelSaberOnlineFilterUserControl.xaml
    /// </summary>
    public partial class ModelSaberOnlineFilterUserControl : UserControl
    {
        public Filter Filter { get; set; }

        public event EventHandler<ModelSaberFilterRemoveEventArgs> RemoveEvent;

        public ModelSaberOnlineFilterUserControl(Filter filter)
        {
            InitializeComponent();
            DataContext = this;
            Filter = filter;
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveEvent?.Invoke(this, new ModelSaberFilterRemoveEventArgs(Filter));
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RemoveEvent?.Invoke(this, new ModelSaberFilterRemoveEventArgs(Filter));
        }
    }
}
