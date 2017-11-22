/*
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

namespace Inferno {
    [CustomEditor(typeof(MeshBuilder))]
    public class LevelEditor : Editor {
        public string loadChunkName;

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            MeshBuilder meshBuilder = (MeshBuilder)target;
            if(GUILayout.Button("Init Chunk")) {
                meshBuilder.InitChunk();
                meshBuilder.BuildMesh();
                meshBuilder.UpdateMesh();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Chunk name: ");
            loadChunkName = EditorGUILayout.TextField(loadChunkName);
            if(GUILayout.Button("Load Chunk")) {
                meshBuilder.InitChunk();
                meshBuilder.LoadChunk(loadChunkName);
                meshBuilder.BuildMesh();
                meshBuilder.UpdateMesh();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Save Chunk: ");
            if(GUILayout.Button("Save Chunk")) {
                meshBuilder.SaveChunk(loadChunkName);
            }
            EditorGUILayout.EndHorizontal();

            if(GUILayout.Button("Randomize Chunk")) {
                meshBuilder.InitChunk();
               // meshBuilder.RandomizeChunk();
               
                meshBuilder.BuildMesh();
                meshBuilder.UpdateMesh();
            }
            
        }
    }
}
*/