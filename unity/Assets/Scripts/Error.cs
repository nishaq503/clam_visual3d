using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clam
{
    /// Possible errors in our library.
    public enum FFIError
    {
        /// All went fine.
        Ok = 0,
        /// Naughty API call detected.
        NullPointerPassed = 1,
        InvalidStringPassed = 2,
        HandleInitFailed = 3,
    }
}
