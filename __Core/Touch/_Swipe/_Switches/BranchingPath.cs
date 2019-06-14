using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{

    [Serializable]
    public class BranchingPath
    {
        public bool isOrigin;
        public Sequence sequence;
        public GameObject directorObject;
        
        [HideInInspector]
        public DirectorUpdater director;
        
        [ValueDropdown("branchTypeValues")]
        public BranchName branchType;
        
        private ValueDropdownList<BranchName> branchTypeValues = new ValueDropdownList<BranchName>(){
            {"Y Negative", BranchName.yNeg },
            {"Y Positive", BranchName.yPos },
            {"X Negative", BranchName.xNeg },
            {"X Positive", BranchName.xPos }
        };
    }
    
}