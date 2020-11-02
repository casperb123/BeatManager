using BeatManager.UserControls.ModelSaber;
using ModelSaber.Entities;
using System.Linq;

namespace BeatManager.ViewModels.ModelSaberModels
{
    public class ModelSaberBaseLocalUserControlViewModel
    {
        public readonly ModelSaberLocalUserControl localUserControl;

        public bool IsLoaded { get; set; }

        public ModelSaberBaseLocalUserControlViewModel(MainWindow mainWindow, ModelSaberBaseLocalUserControl userControl)
        {
            localUserControl = new ModelSaberLocalUserControl(mainWindow, ModelType.Saber);
            userControl.userControlMain.Content = localUserControl;
        }

        public void GetModels(bool useCachedModels)
        {
            if (useCachedModels)
                localUserControl.ViewModel.GetSabers(localUserControl.ViewModel.LocalModels);
            else
                localUserControl.ViewModel.GetSabers();
        }

        public LocalModel GetModel(string name)
        {
            return localUserControl.ViewModel.LocalModels?.Models.FirstOrDefault(x => x.Name == name);
        }

        public void RemoveModel(LocalModel model)
        {
            localUserControl.ViewModel.LocalModels.Models.Remove(model);
        }
    }
}
