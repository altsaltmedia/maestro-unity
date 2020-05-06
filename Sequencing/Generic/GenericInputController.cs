using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Generic
{
    public class GenericInputController : Input_Controller
    {
        [SerializeField]
        private List<GenericInputModule> _genericInputModules = new List<GenericInputModule>();

        public List<GenericInputModule> genericInputModules => _genericInputModules;

        private List<Input_Data> _inputData = new List<Input_Data>();

        public List<Input_Data> inputData => _inputData;
        
        public override void ConfigureData()
        {
            for (int i = 0; i < rootConfig.masterSequences.Count; i++) {

                for (int j = 0; j < rootConfig.masterSequences[i].sequenceControllers.Count; j++) {
                    Input_Data inputItem = new Input_Data();
                    inputItem.sequence = rootConfig.masterSequences[i].sequenceControllers[j].sequence;  
                    inputData.Add(inputItem);
                }
            }
        }
    }
}