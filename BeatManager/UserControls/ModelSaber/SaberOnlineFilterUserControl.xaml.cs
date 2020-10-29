using BeatManager.Events;
using ModelSaber.Entities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for ModelSaberOnlineFilterUserControl.xaml
    /// </summary>
    public partial class SaberOnlineFilterUserControl : UserControl
    {
        public Filter Filter { get; set; }

        public event EventHandler<ModelSaberFilterRemoveEventArgs> RemoveEvent;

        public SaberOnlineFilterUserControl(Filter filter)
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
