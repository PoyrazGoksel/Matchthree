using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extensions.Unity.Editor
{
    public static class ExportFbxAnims
    {
        private const string FBX = ".fbx";
        private const string PreviewAnimClipPrefix = "__preview__";
        public static string GetPath(this Object o) => AssetDatabase.GetAssetPath(o);
        public static string GetExtension(this string path) => Path.GetExtension(path);
        public static string ToGlobalPath(this string path) => Application.dataPath.Remove("Assets") + path;
        public static string ParentPath(this string path) => path.Substring(0, path.LastIndexOf('/'));
        public static string Remove(this string s, string toRemove)
        {
            if (toRemove == "") return s;
            return s.Replace(toRemove, "");
        }
        public static string ToLocalPath(this string path) => path.Remove(Application.dataPath.Remove("Assets"));
        
        [MenuItem("Assets/Extract Animations")]
        private static void ExtractAnims(MenuCommand menuCommand)
        {
            bool isAllSelectionsMesh = Selection.objects.All
            (
                e =>
                {
                    string path = e.GetPath();

                    string extension = path.GetExtension();

                    return extension == FBX;
                }
            );

            if(isAllSelectionsMesh)
            {
                IEnumerable<Object[]> objectsEnumerable = Selection.objects.Select
                (obj => AssetDatabase.LoadAllAssetsAtPath(obj.GetPath()));
                const string newFolderName = "Animations";

                AssetDatabase.DisallowAutoRefresh();
                
                foreach(Object[] assetObjects in objectsEnumerable)
                foreach(Object assetObject in assetObjects)
                {
                    if(assetObject.name.Contains(PreviewAnimClipPrefix)) continue;
                    if(assetObject is not AnimationClip) continue;

                    string fbxPath = assetObject.GetPath();
                    string parentFolder = fbxPath.ParentPath();
                    
                    string animationsFolderPath = parentFolder + "/" + newFolderName;
                    string fbxFileName = Path.GetFileNameWithoutExtension(fbxPath);
                    string newAnimPath = animationsFolderPath  + "/" + fbxFileName + ".anim";
                    string globalPath = newAnimPath.ToGlobalPath();
                    string localPath = globalPath.ToLocalPath();

                    if(Directory.Exists(animationsFolderPath) == false)
                    {
                        AssetDatabase.CreateFolder
                        (parentFolder, newFolderName);
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    }
                    else if (File.Exists(globalPath))
                    {
                        AssetDatabase.DeleteAsset(localPath);
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    }

                    AnimationClip animationClip = Object.Instantiate(assetObject as AnimationClip);
                    AssetDatabase.CreateAsset(animationClip, newAnimPath);
                }
                
                AssetDatabase.AllowAutoRefresh();
            }
        }
        //TODO: bake rot bake y and copy asset name to anim clip
        // [MenuItem("Assets/Bake Anim Rotations")]
        // private static void BakeRotation(MenuCommand menuCommand)
        // {
        //     bool isAllSelectionsMesh = Selection.objects.All
        //     (
        //         e =>
        //         {
        //             string path = e.GetPath();
        //
        //             string extension = path.GetExtension();
        //
        //             return extension == FBX;
        //         }
        //     );
        //
        //     if(isAllSelectionsMesh)
        //     {
        //         IEnumerable<Object[]> objectsEnumerable = Selection.objects.Select
        //         (obj => AssetDatabase.LoadAllAssetsAtPath(obj.GetPath()));
        //         const string newFolderName = "Animations";
        //
        //         AssetDatabase.DisallowAutoRefresh();
        //         
        //         foreach(Object[] assetObjects in objectsEnumerable)
        //         foreach(Object assetObject in assetObjects)
        //         {
        //             if(assetObject.name.Contains(PreviewAnimClipPrefix)) continue;
        //             if(assetObject is not AnimationClip) continue;
        //
        //             AnimationClip animClip = assetObject as AnimationClip;
        //             ModelImporter importer = new();
        //             
        //             string fbxPath = assetObject.GetPath();
        //             string parentFolder = fbxPath.ParentPath();
        //             
        //             string animationsFolderPath = parentFolder + "/" + newFolderName;
        //             string fbxFileName = Path.GetFileNameWithoutExtension(fbxPath);
        //             string newAnimPath = animationsFolderPath  + "/" + fbxFileName + ".anim";
        //             string globalPath = newAnimPath.ToGlobalPath();
        //             string localPath = globalPath.ToLocalPath();
        //
        //             AssetImporter modelAssetImporter = ModelImporter.GetAtPath(globalPath);
        //             
        //             AnimationClip animationClip = Object.Instantiate(assetObject as AnimationClip);
        //             AssetDatabase.CreateAsset(animationClip, newAnimPath);
        //         }
        //         
        //         AssetDatabase.AllowAutoRefresh();
        //     }
        // }
    }
}