using System;
using System.Collections.Generic;
using Game;
using Lib;
using UnityEngine;

public class MergeController : SingletonScene<MergeController>
{
    [SerializeField] private List<MergePoint> _joinPoints;
    private int Count => _joinPoints.Count;
    public bool IsFull => _joinPoints.Count > 0 && _joinPoints[_joinPoints.Count - 1].Active;
    private int blocks;

    private void Start()
    {
        if (_joinPoints == null) Debug.LogError("Merge points not set");
    }

    #region Set & Get Position

    public (Vector3, MergePoint) GetPosition(BlockColor blockColor, IMovable movable, IDisposable d)
    {
        // 1. Ищем первый и последний блоки такого цвета
        int first = FindFirstOfColor(blockColor);
        int last = FindLastOfColor(blockColor);

        // 2. Если есть хотя бы 1 блок такого цвета
        if (first != -1)
        {
            int count = CountConsecutiveColor(first);

            // Если слева уже два подряд — вставляем после них
            if (count >= 2)
            {
                int insertIndex = first + count;

                if (insertIndex >= Count)
                {
                    Debug.LogWarning("Нет места для создания тройки!");
                    return (Vector3.zero, null);
                }

                int emptyRight = FindFirstEmptyIndexToRight(insertIndex);
                if (emptyRight < 0)
                {
                    Debug.LogWarning("Нет свободного места справа для сдвига!");
                    return (Vector3.zero, null);
                }

                ShiftRightUntil(insertIndex, emptyRight);
                SetPoint(insertIndex, blockColor, movable, d);
                return (_joinPoints[insertIndex].Pos, _joinPoints[insertIndex]);
            }

            // Иначе просто вставляем после последнего такого цвета
            int insertAfter = last + 1;
            if (insertAfter >= Count)
            {
                Debug.LogWarning("Нет места для вставки!");
                return (Vector3.zero, null);
            }

            int emptyRight2 = FindFirstEmptyIndexToRight(insertAfter);
            if (emptyRight2 >= 0)
            {
                ShiftRightUntil(insertAfter, emptyRight2);
                SetPoint(insertAfter, blockColor, movable, d);
                return (_joinPoints[insertAfter].Pos, _joinPoints[insertAfter]);
            }
        }

        // 3. Если таких блоков нет — вставляем в первую пустую
        int firstEmpty = FindFirstEmptyIndex();
        if (firstEmpty >= 0)
        {
            SetPoint(firstEmpty, blockColor, movable, d);
            return (_joinPoints[firstEmpty].Pos, _joinPoints[firstEmpty]);
        }

        Debug.LogWarning("Нет места для вставки блока!");
        return (Vector3.zero, null);
    }

    private void SetPoint(int index, BlockColor blockColor, IMovable movable, IDisposable d)
    {
        if (index < 0 || index >= Count)
        {
            Debug.LogError($"MergController: индекс {index} вне диапазона!");
            return;
        }

        var p = _joinPoints[index];
        p.Active = true;
        p.BlockColor = blockColor;
        p.Movable = movable;
        p.Disposable = d;
    }


    #endregion

    #region Find

    // Подсчёт подряд идущих блоков одного цвета, начиная с startIndex
    private int CountConsecutiveColor(int startIndex)
    {
        if (startIndex < 0 || startIndex >= _joinPoints.Count)
        {
            return 0;
        }
        BlockColor color = _joinPoints[startIndex].BlockColor;
        int count = 1;

        for (int i = startIndex + 1; i < _joinPoints.Count; i++)
        {
            try
            {
                if (_joinPoints[i].Active && _joinPoints[i].BlockColor == color)
                    count++;
                else
                    break;

            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log($"{e}, Index {i}");
            }
            
        }

        return count;
    }

    private int FindFirstEmptyIndex()
    {
        for (int i = 0; i < Count; i++)
            if (!_joinPoints[i].Active)
                return i;
        return -1;
    }

    private int FindFirstEmptyIndexToRight(int startIndex)
    {
        for (int i = startIndex; i < Count; i++)
            if (!_joinPoints[i].Active)
                return i;
        return -1;
    }

    private int FindFirstOfColor(BlockColor color)
    {
        for (int i = 0; i < Count; i++)
        {
            if (_joinPoints[i].Active && _joinPoints[i].BlockColor == color)
                return i;
        }

        return -1;
    }

    private int FindLastOfColor(BlockColor color)
    {
        for (int i = Count - 1; i >= 0; i--)
        {
            if (_joinPoints[i].Active && _joinPoints[i].BlockColor == color)
                return i;
        }

        return -1;
    }


    #endregion

    #region Moving blocks

    // <summary>
    /// Сдвигаем блоки вправо, начиная с fromIndex, до emptyIndex (emptyIndex должен быть >= fromIndex).
    /// Делает движения объектов (MoveOneShut) с корректным вектором, и копирует все поля (Disposable и т.д.).
    /// </summary>
    private void ShiftRightUntil(int fromIndex, int emptyIndex)
    {
        if (fromIndex < 0 || emptyIndex >= Count || emptyIndex < fromIndex) return;

        // Передвигаем каждый элемент на 1 ячейку вправо, начиная с emptyIndex-1 до fromIndex
        for (int i = emptyIndex; i > fromIndex; i--)
        {
            var src = _joinPoints[i - 1];
            var dst = _joinPoints[i];

            if (!src.Active)
            {
                // очистить dst если src пуст
                dst.Clear();
                continue;
            }

            // перенести все поля
            dst.CopyFrom(src);
            // выполнить физическое движение (если есть)
            if (dst.IsStanding)
            {
                if (dst.Movable != null)
                {
                    Vector3 delta = dst.Pos - src.Pos; // куда нужно переместить
                    if (delta != Vector3.zero)
                    {
                        dst.Movable.MoveOneShot(delta);
                    }
                }
            }
            else
            {
                dst?.OnChangePosition(dst);
            }

            // очистить источник
            src.Clear();
        }
    }

    /// <summary>
    /// Сжатие всех блоков влево (выравнивание к началу списка), сохраняет порядок.
    /// </summary>
    private void CompactLeft()
    {
        int write = 0;
        for (int read = 0; read < Count; read++)
        {
            if (!_joinPoints[read].Active) continue;
            if (write == read)
            {
                write++;
                continue;
            }

            // перемещение read -> write
            var src = _joinPoints[read];
            var dst = _joinPoints[write];

            dst.CopyFrom(src);
            Vector3 oldPos = src.Pos;
            src.Clear();
            
            if(dst.IsStanding)
            {
                if (dst.Movable != null)
                {
                    Vector3 delta = dst.Pos - oldPos;
                    if (delta != Vector3.zero)
                        dst.Movable.MoveOneShot(delta);
                }
            }
            else
            {
                dst.OnChangePosition?.Invoke(dst);
            }

            // очистить источник
            write++;
        }

        // очистить оставшиеся
        for (int i = write; i < Count; i++)
        {
            _joinPoints[i].Clear();
        }
    }

    #endregion

    #region Merge
    
    public void MergeToColor(BlockColor color)
    {
        int firstIndex = FindFirstOfColor(color);
        
        if (firstIndex == -1) return;
        int count = CountConsecutiveColor(firstIndex);

        if (count < 3) return;

        for (int i = firstIndex; i < firstIndex + 3; i++)
        {
            if (!_joinPoints[i].IsStanding)
            {
                return;
            }
        }
        for (int k = firstIndex; k < firstIndex + 3; k++)
        {
            _joinPoints[k].Movable.JumpAndMerge(k - firstIndex + 1);
            try
            {
                RemoveBlock();
                LevelController.Instance.RemoveBlock();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при DestroyBlock: {ex}");
            }

            _joinPoints[k].Clear();
        }

        // после удаления — уплотняем влево
        CompactLeft();
    }

    public void AddNewBlock()
    {
        blocks++;
        if (IsFull && OpeningPoints()) LevelController.Instance.levelActions.OnEndLevel(false);
    }

    private void RemoveBlock()
    {
        blocks--;
    }

    public bool OpeningPoints()
    {
        return blocks == Count;
    }

    #endregion


}