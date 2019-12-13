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

        public AppSettings appSettings
        {
            get => _appSettings;
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
