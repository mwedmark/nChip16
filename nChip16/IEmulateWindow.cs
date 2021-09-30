using System.Collections.Generic;

namespace nChip16
{
    public interface IEmulateWindow
    {
        void ClearBuffer();
        void SetBitmap(List<int> newValues);
    }
}