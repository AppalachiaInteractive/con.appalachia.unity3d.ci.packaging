﻿using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Appalachia.CI.Packaging.Editor.PackageRegistry.UI
{
    public class BulkAddPackages : EditorWindow
    {
        private string PackageList = "";

        private void OnEnable()
        {
            PackageList = "";
            minSize = new Vector2(640, 320);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Add Packages", EditorStyles.whiteLargeLabel);
            EditorGUILayout.Separator();

            PackageList = EditorGUILayout.TextArea(PackageList, GUILayout.Height(200));

            EditorGUILayout.LabelField("Add multiple packages. Place each package on a newline.");
            EditorGUILayout.LabelField("Format:.");
            EditorGUILayout.LabelField(
                "\tLatest version of package: com.halodi.halodi-unity-package-registry-manager"
            );
            EditorGUILayout.LabelField(
                "\tSpecific version: com.halodi.halodi-unity-package-registry-manager@0.1.0"
            );

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add packages"))
            {
                AddPackages();
                CloseWindow();
            }

            if (GUILayout.Button("Close"))
            {
                CloseWindow();
            }

            EditorGUILayout.EndHorizontal();
        }

        [MenuItem("Packages/Add packages (bulk)", false, 22)]
        internal static void ManageRegistries()
        {
            GetWindow<BulkAddPackages>(true, "Add packages", true);
        }

        private void AddPackages()
        {
            var result = "";

            var hasPackages = false;

            using (var reader = new StringReader(PackageList))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var request = Client.Add(line);

                        while (!request.IsCompleted)
                        {
                            Thread.Sleep(100);
                        }

                        if (request.Status == StatusCode.Success)
                        {
                            result += "Imported: " + line + Environment.NewLine;
                        }
                        else
                        {
                            result += "Cannot import " +
                                      line +
                                      ": " +
                                      request.Error.message +
                                      Environment.NewLine;
                        }

                        hasPackages = true;
                    }
                }
            }

            if (hasPackages)
            {
                EditorUtility.DisplayDialog(
                    "Added packages",
                    "Packages added:" + Environment.NewLine + Environment.NewLine + result,
                    "OK"
                );
            }
            else
            {
                EditorUtility.DisplayDialog("No packages entered", "No packages entered.", "OK");
            }
        }

        private void CloseWindow()
        {
            Close();
            GUIUtility.ExitGUI();
        }
    }
}
