using Game2048.ViewModels.Base;

namespace Game2048.Models
{
    // Модель для представления плитки на игровом поле.
    public class Tile : ViewModel
    {
        private int row;
        public int Row { get => row; set => Set(ref row, value); }

        private int col;
        public int Col { get => col; set => Set(ref col, value); }

        private int value;
        public int Value { get => value; set => Set(ref this.value, value); }

        private bool isNew;
        public bool IsNew { get => isNew; set => Set(ref isNew, value); }

        private bool isMerged;
        public bool IsMerged { get => isMerged; set => Set(ref isMerged, value); }
    }
}
