using BeatManager.UserControls.ModelSaber;
using ModelSaber.Entities;
using System.Linq;

namespace BeatManager.ViewModels.ModelSaberModels
{
    public class ModelSaberBaseOnlineUserControlViewModel
    {
        private readonly ModelSaberOnlineUserControl onlineUserControl;

        public bool IsLoaded { get; set; }

        public ModelSaberBaseOnlineUserControlViewModel(MainWindow mainWindow, ModelSaberBaseOnlineUserControl userControl)
        {
            onlineUserControl = new ModelSaberOnlineUserControl(mainWindow, userControl, ModelType.Saber);
            userControl.userControlMain.Content = onlineUserControl;
        }

        public void GetSabers()
        {
            onlineUserControl.ViewModel.GetSabers();
        }

        public OnlineModel GetModel(string name)
        {
            return onlineUserControl.ViewModel.OnlineModels?.Models.FirstOrDefault(x => x.Name == name);
        }
    }
}
