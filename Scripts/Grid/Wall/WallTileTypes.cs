
using System;

public enum WallTileTypes
{
    Empty,
    Bevel_Out,
    Bevel_In,
    Middle_Left,
    Middle_Right,
    Up,
    Full_Wall
}


// Битовая маска с флагами. 
// Flags позволяет выбирать несколько пунктов в инспекторе (как слои в Unity).
[System.Flags]
public enum NeighborMask
{
    None        = 0,
    Vertical    = 1, // Бит 1
    Horizontal  = 2, // Бит 2
    Diagonal    = 4  // Бит 3
    // Можно добавить Everything = Vertical | Horizontal | Diagonal
}

