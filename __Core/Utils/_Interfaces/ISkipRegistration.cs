using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    public interface ISkipRegistration
    {
        bool DoNotRecord {
            get;
        }
    }
}
