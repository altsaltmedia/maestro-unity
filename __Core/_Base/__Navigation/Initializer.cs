using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace AltSalt
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField]
        string bootstrapScene;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            Addressables.LoadSceneAsync(bootstrapScene, LoadSceneMode.Single);
        }
    }
}
