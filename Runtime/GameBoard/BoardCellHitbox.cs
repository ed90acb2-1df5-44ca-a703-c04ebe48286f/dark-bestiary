using UnityEngine;

namespace DarkBestiary.GameBoard
{
    public class BoardCellHitbox : MonoBehaviour
    {
        public BoardCell Cell { get; private set; }

        public void Initialize(BoardCell cell)
        {
            Cell = cell;
        }
    }
}