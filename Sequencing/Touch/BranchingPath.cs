using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{

    [Serializable]
    public class BranchingPath
    {
        [SerializeField]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        [ValueDropdown("branchTypeValues")]
        private BranchType _branchType;

        public BranchType branchType
        {
            get => _branchType;
            set => _branchType = value;
        }

        private ValueDropdownList<BranchType> branchTypeValues = new ValueDropdownList<BranchType>(){
            {"Y Negative", BranchType.yNeg },
            {"Y Positive", BranchType.yPos },
            {"X Negative", BranchType.xNeg },
            {"X Positive", BranchType.xPos }
        };

    }
    
}