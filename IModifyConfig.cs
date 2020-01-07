using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    public interface IModifyConfig
    {
        bool active
        {
            get;
            set;
        }
        int priority
        {
            get;
            set;
        }

        IModifyConfig SetActive(bool targetStatus);
    }
}