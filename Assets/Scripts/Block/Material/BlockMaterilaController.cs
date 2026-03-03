using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BlockMaterilaController : MonoBehaviour
{
    [Header("Mesh")] [SerializeField] private List<SkinnedMeshRenderer> _blockMeshes;
    
    [Header("Material")]
    [SerializeField] private List<BlockMaterial> _blockMaterials;
    
    [SerializeField] private SpriteRenderer _eyeSpriteRenderer;
    
    [SerializeField] private Sprite _happySprite;
    [SerializeField] private Sprite _diactiveSprite;
    
    
    public void SetColor(BlockColor color)
    {
        foreach (var item in _blockMaterials)
        {
            if (item.blockColor == color)
            {
                SetAllMaterials(item.activateMaterial);
                ActivateHat(item.Hats, true);
            }
            else
            {
                ActivateHat(item.Hats, false);
            }
        }
    }

    public void SetActivateColor(BlockColor color, bool activate)
    {
        foreach (var item in _blockMaterials)
        {
            if (item.blockColor == color)
            {
                if(activate)
                    SetAllMaterials(item.activateMaterial);
                else
                {
                    SetAllMaterials(item.diactivateMaterial);
                }
                break;
            }
        }
    }
    
    public void SetEyes(BlockColor color, BlockEyesType type)
    {
        switch (type)
        {
            case BlockEyesType.Active:
            {
                _eyeSpriteRenderer.sprite = _blockMaterials.Find(c => c.blockColor == color).eyeSprite;
                break;
            }
            case BlockEyesType.Diactive:
            {
                _eyeSpriteRenderer.sprite = _diactiveSprite;
                break;
            }
            case BlockEyesType.Happy:
            {
                _eyeSpriteRenderer.sprite = _happySprite;
                break;
            }
        }
    }

    private void SetAllMaterials(Material material)
    {
        foreach (var mesh in _blockMeshes)
        {
            mesh.material = material;
        }
    }

    private void ActivateHat(List<GameObject> hats, bool active)
    {
        foreach (var item in hats)
        {
            item.SetActive(active);
        }
    }
}


[Serializable]
public class BlockMaterial
{
    public BlockColor blockColor;
    public Material activateMaterial;
    public Material diactivateMaterial;
    
    public Sprite eyeSprite;
    public List<GameObject> Hats;
}

public enum BlockEyesType
{
    Active,
    Diactive,
    Happy,
}


