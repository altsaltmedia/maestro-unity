using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEditor;
using SimpleJSON;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AltSalt
{

    public abstract class EventBase : RegisterableScriptableObject, IDependable
    {

        [SerializeField]
        [Required]
        protected AppSettings appSettings;

        protected GameObject callerObject;
        protected string callerScene = "";
        protected string callerName = "";

        [SerializeField]
        protected bool logCallersOnRaise;

        [SerializeField]
        protected bool logListenersOnRegister;

        [SerializeField]
        protected bool logListenersOnRaise;

        void Awake()
        {
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }
        }

        public void StoreCaller(GameObject caller)
        {
            callerObject = caller;
            callerScene = caller.scene.name;
            callerName = caller.name;
            //RegisterDependent(callerScene, callerName);
        }

        public void StoreCaller(GameObject caller, string sourceScene, string sourceName)
        {
            callerObject = caller;
            callerScene = sourceScene;
            callerName = sourceName;
            //RegisterDependent(callerScene, callerName);
        }

        protected bool CallerRegistered()
        {
            if (callerObject == null && callerName.Length < 1) {
                Debug.LogError("Caller not registered on " + this.name + ". Are you calling this event directly? Please use an event trigger instead.", this);
                return false;
            }

            return true;
        }

        protected void ClearCaller()
        {
            callerObject = null;
            callerScene = "";
            callerName = "";
        }

        abstract protected void LogCaller();

        protected void LogListenersHeading(int listCount)
        {
            if (listCount > 0) {
                Debug.Log(string.Format("[event] [{0}] [{1}] Following listeners activated:", callerScene, this.name), this);
            } else {
                Debug.Log(string.Format("[event] [{0}] [{1}] No listeners registered.", callerScene, this.name), this);
            }
        }

        protected void LogClosingLine()
        {
            Debug.Log("[event] ---------");
        }

        public object GetCaller()
        {
            if (callerObject != null) {
                return callerObject;
            }

            if (callerName.Length > 0) {
                return callerName;
            }

            return null;
        }

        string GetDirectory()
        {
            string directoryPath = Utils.GetProjectPath();
            string[] subfolders = { "/z_Dependencies", "/Events" };

            for (int i = 0; i < subfolders.Length; i++) {
                directoryPath += subfolders[i];
                if (!Directory.Exists(directoryPath)) {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            return directoryPath;
        }

        string GetFilePath()
        {
            return GetDirectory() + "/" + this.name + ".json";
        }

        protected void RegisterDependent(string dependentScene, string dependentName)
        {
            if (File.Exists(GetFilePath())) {
                WriteDependentToExistingFile(dependentScene, dependentName);
            } else {
                WriteDependentToNewFile(dependentScene, dependentName);
            }
            AssetDatabase.Refresh();
        }

        void WriteDependentToExistingFile(string dependentScene, string dependentName)
        {
            string savedData = File.ReadAllText(GetFilePath());
            var dataAsJson = JSON.Parse(savedData);

            bool dependencyExists = false;
            if (dataAsJson[dependentScene] != null) {
                foreach (string value in dataAsJson[dependentScene].Children) {
                    if (value == dependentName) {
                        dependencyExists = true;
                    }
                }
            }
            if (dependencyExists == false) {
                dataAsJson[dependentScene].Add(dependentName);
                File.WriteAllText(GetFilePath(), dataAsJson.ToString(2));
            }
        }

        void WriteDependentToNewFile(string dependentScene, string dependentName)
        {
            var newData = JSON.Parse("{}");
            newData[dependentScene].Add(dependentName);
            File.WriteAllText(GetFilePath(), newData.ToString(2));
        }
    }

    //public class EventBaseTests
    //{
       
    //    public void _Test_RegisterDependency()
    //    {
    //        SimpleEvent simpleEvent = new SimpleEvent();

    //        var savedData = File.Open("/Project/Dependencies/" + simpleEvent.name + ".json", FileMode.Open);

    //    }

    //}
}