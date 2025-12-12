/*****************************************************************************
// File Name : TemplateCreator.cs
// Author : Brandon Koederitz
// Creation Date : August 26, 2025
// Last Modified : September 16, 2025
//
// Brief Description : Allows for the creation of custom script templates in the assets window
*****************************************************************************/
using UnityEditor;

namespace Templates
{
    public static class TemplateCreator
    {
        #region CONSTS
        private const string MONOBEHAVIOUR_PATH = "Assets/Templates/Editor/TemplateFiles/MonoBehaviour.cs.txt";
        private const string SCRIPTABLEOBJECT_PATH = "Assets/Templates/Editor/TemplateFiles/ScriptableObject.cs.txt";
        private const string ENUM_PATH = "Assets/Templates/Editor/TemplateFiles/Enum.cs.txt";
        private const string RAWCLASS_PATH = "Assets/Templates/Editor/TemplateFiles/RawC#Class.cs.txt";
        private const string CUSTOMEDITOR_PATH = "Assets/Templates/Editor/TemplateFiles/CustomEditor.cs.txt";
        private const string INTERFACE_PATH = "Assets/Templates/Editor/TemplateFiles/Interface.cs.txt";
        #endregion

        /// <summary>
        /// Functions to be run by the Unity create menu that allows scripts to be created from templates.
        /// </summary>
        [MenuItem("Assets/Create/Scripts/MonoBehaviour")]
        public static void CreateMonoBehaviour()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(MONOBEHAVIOUR_PATH, "NewMonoBehaviour.cs");
        }

        [MenuItem("Assets/Create/Scripts/ScriptableObject")]
        public static void CreateScriptableObject()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(SCRIPTABLEOBJECT_PATH, "NewScriptableObject.cs");
        }

        [MenuItem("Assets/Create/Scripts/Enum")]
        public static void CreateEnum()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(ENUM_PATH, "NewEnum.cs");
        }

        [MenuItem("Assets/Create/Scripts/Raw C# Class")]
        public static void CreateRawClass()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(RAWCLASS_PATH, "NewRawC#Class.cs");
        }

        [MenuItem("Assets/Create/Scripts/Custom Editor")]
        public static void CreateCustomEditor()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(CUSTOMEDITOR_PATH, "NewCustomEditor.cs");
        }

        [MenuItem("Assets/Create/Scripts/Interface")]
        public static void CreateInterface()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(INTERFACE_PATH, "NewInterface.cs");
        }
    }
}
