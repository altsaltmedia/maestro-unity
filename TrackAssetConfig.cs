using UnityEngine;

namespace AltSalt
{
    public class TrackAssetConfig : MonoBehaviour
    {
        [SerializeField]
        private AppSettings _appSettings;

        public AppSettings appSettings => _appSettings;
        
        [SerializeField]
        private BoolReference _isReversing;

        public BoolVariable isReversing => _isReversing.Variable;

        [SerializeField]
        private BoolReference _scrubberActive;

        public BoolVariable scrubberActive => _scrubberActive.Variable;
        
        
    }
}