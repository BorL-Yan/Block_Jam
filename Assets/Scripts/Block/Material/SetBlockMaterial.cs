
    using System.Collections.Generic;
    using com.cyborgAssets.inspectorButtonPro;
    using UnityEngine;

    public class SetBlockMaterial : MonoBehaviour
    {
        [Header("Material Settings")]
        [SerializeField] private Material[] activeMaterials;
        [SerializeField] private Material[] inactiveMaterials;
    
        [Header("References")]
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

        
        [ProButton]
        public void Activate(bool value)
        {
            if (value)
                skinnedMeshRenderer.materials = activeMaterials;
            else
                skinnedMeshRenderer.materials = inactiveMaterials;
        }
    }
