﻿using System;
using JetBrains.Annotations;
using MichisMeshMakers.Editor.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MichisMeshMakers.Editor.Containers.Generic
{
    public abstract class MeshContainerEditor<TMeshContainer> : MeshContainerEditor where TMeshContainer : MeshContainer
    {
        [PublicAPI] protected TMeshContainer Target { get; private set; }

        [PublicAPI] protected TMeshContainer[] Targets { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            Target = (TMeshContainer)target;
            Targets = Array.ConvertAll(targets, t => (TMeshContainer)t);
        }

        protected override void DrawMeshPreview(Rect previewRect, Object targetObject)
        {
            DrawMeshPreview(previewRect, (TMeshContainer)targetObject);
        }

        protected abstract void DrawMeshPreview(Rect rect, TMeshContainer meshContainer);

        protected static void Create(string assetName, Func<string, Object> getCreationTarget)
        {
            Object selection = Selection.activeObject;
            Object creationTarget = getCreationTarget(AssetDatabase.GetAssetPath(selection));

            assetName = creationTarget != null ? creationTarget.name : AssetDatabase.Contains(selection) ? selection.name : assetName;

            string path = AssetDatabaseUtility.GetCreateAssetPath(assetName);

            var meshContainer = CreateInstance<TMeshContainer>();
            meshContainer.name = assetName;
            var childMesh = new Mesh
            {
                name = assetName
            };
            AssetDatabase.CreateAsset(meshContainer, path);
            AssetDatabase.AddObjectToAsset(childMesh, meshContainer);
            meshContainer.Initialize(creationTarget, childMesh);
            AssetDatabaseUtility.ForceSaveAsset(meshContainer);

            // select the newly created Parent Asset in the Project Window
            Selection.activeObject = meshContainer;
        }

        protected void ApplyAll()
        {
            foreach (TMeshContainer octagonMesh in Targets)
            {
                Undo.RecordObject(octagonMesh.Mesh, "Apply Mesh Container");
                octagonMesh.Apply();
            }
        }
    }
}