#if UNITY_EDITOR
using UnityEngine;

class LimitEditorFrameRate
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeInitialized()
    {
        Application.targetFrameRate = 30;
        Cursor.visible = false;
    }
}
#endif
