using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ATBbarre : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/ATBbarre")]
    public static void ShowExample()
    {
        ATBbarre wnd = GetWindow<ATBbarre>();
        wnd.titleContent = new GUIContent("ATBbarre");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }
}
