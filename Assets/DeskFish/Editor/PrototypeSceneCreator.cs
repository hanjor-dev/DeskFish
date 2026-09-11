using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using DeskFish.Runtime.Prototype;
using DeskFish.Runtime.Windows;

namespace DeskFish.Editor
{
    public static class PrototypeSceneCreator
    {
        [MenuItem("DeskFish/Create Prototype Scene &p")]
        public static void Create()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = new Color(1f, 0f, 1f, 1f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            cameraObject.tag = "MainCamera";

            var root = new GameObject("DeskFish Prototype");
            var tank = root.AddComponent<TankPrototype>();
            var interaction = root.AddComponent<TankInteraction>();
            root.AddComponent<DesktopWindowController>();
            SerializedObject interactionSerialized = new SerializedObject(interaction);
            interactionSerialized.FindProperty("tankCamera").objectReferenceValue = camera;
            interactionSerialized.FindProperty("tank").objectReferenceValue = tank;
            interactionSerialized.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, "Assets/DeskFish/Scenes/PrototypeTank.unity");
            Selection.activeGameObject = root;
        }
    }
}
