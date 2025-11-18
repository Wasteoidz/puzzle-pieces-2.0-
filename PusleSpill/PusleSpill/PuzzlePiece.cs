using System;
using System.Collections.Generic;
using System.Text;

namespace PusleSpill
{
    public class PuzzlePiece
    {
        public int PieceId { get; set; }
        public string ImageFile { get; set; } = string.Empty;  
        public Point CorrectPosition { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
