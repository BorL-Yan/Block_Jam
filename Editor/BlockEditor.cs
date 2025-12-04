// using UnityEditor;
// using UnityEngine;
//
//     [CustomEditor(typeof(Block))]
// public class BlockEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         Block block = (Block)target;
//
//         EditorGUILayout.Space(4);
//         EditorGUILayout.LabelField("Block Settings", EditorStyles.boldLabel);
//
//         // Основной тип блока
//         block.BlockType = (BlockType)EditorGUILayout.EnumPopup("Type", block.BlockType);
//
//         EditorGUILayout.Space(5);
//
//         switch (block.BlockType)
//         {
//             case BlockType.Normal:
//             case BlockType.Blocked:
//                 DrawColorSelector(block);
//                 break;
//
//             case BlockType.List:
//                 DrawList(block);
//                 break;
//         }
//
//         if (GUI.changed)
//         {
//             EditorUtility.SetDirty(block);
//         }
//     }
//
//     private void DrawColorSelector(Block block)
//     {
//         EditorGUILayout.LabelField("Color", EditorStyles.boldLabel);
//         block.Color = (BlockColor)EditorGUILayout.EnumPopup("Block Color", block.Color);
//     }
//
//     private void DrawList(Block block)
//     {
//         EditorGUILayout.LabelField("Block List", EditorStyles.boldLabel);
//
//         if (GUILayout.Button("Add Item"))
//         {
//             block.Items.Add(new BlockItem());
//             EditorUtility.SetDirty(block);
//         }
//
//         EditorGUILayout.Space(5);
//
//         for (int i = 0; i < block.Items.Count; i++)
//         {
//             var item = block.Items[i];
//
//             EditorGUILayout.BeginVertical("box");
//
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField($"Item {i + 1}", EditorStyles.boldLabel);
//             if (GUILayout.Button("X", GUILayout.Width(20)))
//             {
//                 block.Items.RemoveAt(i);
//                 break;
//             }
//             EditorGUILayout.EndHorizontal();
//
//             item.Type = (BlockType)EditorGUILayout.EnumPopup("Type", item.Type);
//
//             if (item.Type == BlockType.Normal || item.Type == BlockType.Blocked)
//             {
//                 item.Color = (BlockColor)EditorGUILayout.EnumPopup("Color", item.Color);
//             }
//
//             EditorGUILayout.EndVertical();
//             EditorGUILayout.Space(5);
//         }
//     }
// }
