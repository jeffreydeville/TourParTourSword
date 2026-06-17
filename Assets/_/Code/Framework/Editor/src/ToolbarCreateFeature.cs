using UnityEditor;
using UnityEditor.Toolbars;

namespace Framework.Editor
{
	public class ToolbarCreateFeature : EditorWindow
	{
		[MainToolbarElement("Create Feature", defaultDockPosition = MainToolbarDockPosition.Left)]
		public static MainToolbarButton ShowWindow()
		{
			MainToolbarContent content = new MainToolbarContent("Create Feature", "Create Feature with Assembly Definition & Script");
			
			return new MainToolbarButton(content, OpenWindow);
		}

		private static void OpenWindow()
		{
			CreateNewFeatureTool.ShowWindow();
		}
	}
}