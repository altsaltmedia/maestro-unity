using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace AltSalt.Maestro.Logic
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField]
        private AppSettings _appSettings;

        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
        }

        [SerializeField]
        private string _bootstrapScene;

        public string bootstrapScene
        {
            get => _bootstrapScene;
            set => _bootstrapScene = value;
        }

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            Physics.autoSimulation = false;
            Application.targetFrameRate = 60;
            
            if (appSettings.useAddressables == true) {
                yield return new WaitForSeconds(1);
                Addressables.LoadSceneAsync(bootstrapScene, LoadSceneMode.Single);
            }
            else {
                SceneManager.LoadScene(bootstrapScene, LoadSceneMode.Single);
                yield return null;
            }
        }
    }
}
