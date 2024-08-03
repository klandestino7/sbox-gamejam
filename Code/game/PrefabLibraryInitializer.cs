// Stolen from https://github.com/Small-Fish-Dev/My-Summer-Cottage/blob/main/code/Game/PrefabInitializer.cs

public sealed class PrefabLibraryInitializer : Component, Component.ExecuteInEditor
{
	protected override void OnAwake() => PrefabLibrary.Initialize();
}
