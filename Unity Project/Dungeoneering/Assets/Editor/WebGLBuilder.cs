using UnityEditor;
class WebGLBuilder {
    static void Build() {

        // Place all your scenes here
        string[] scenes = {
            "Assets/_Project/Scenes/DungeonEditor.unity"
        };

        string pathToDeploy = "Builds/WebGLversion/";       

        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.WebGL, BuildOptions.None);    
    }
}