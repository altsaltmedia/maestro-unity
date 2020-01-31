using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    public interface IContentExtensionConfig
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

        IContentExtensionConfig SetActive(bool targetStatus);
    }
}