using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace AltSalt.Maestro.Logic
{
    [ExecuteInEditMode]
    public class Initializer : MonoBehaviour
    {
        [SerializeField]
        [Required]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

        [SerializeField]
        private string _bootstrapScene;

        private string bootstrapScene => _bootstrapScene;
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            _appSettings.PopulateVariable(this, nameof(_appSettings));
        }
#endif

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            if (Application.isPlaying == false) yield break;
            
            Physics.autoSimulation = false;
            Application.targetFrameRate = 60;
            appSettings.playStartedFromInitializer = true;
            
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
