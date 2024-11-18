using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MainMenuManager : Manager
    {
        public override void InstallBindings()
        {
            Container.Bind<MainMenuManager>().To<MainMenuManager>().AsSingle();
        }

        public void LoadScene (string name)
        {
            SceneManager.LoadScene(name);
        }

        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
