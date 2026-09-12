using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

public static class AddButtonClickSFX
{
    [MenuItem("Tools/UI/Add Button Click SFX")]
    private static void AddSFX()
    {
        AudioManager audioManager = Object.FindFirstObjectByType<AudioManager>();

        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene");
            return;
        }

        foreach (GameObject obj in Selection.gameObjects)
        {
            if (!obj.TryGetComponent<Button>(out var button))
                continue;

            UnityEventTools.AddVoidPersistentListener(button.onClick, audioManager.PlayButtonClick);

            EditorUtility.SetDirty(button);
        }
    }
}