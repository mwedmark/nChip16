namespace nChip16
{
    public enum LockTo {Address,Label};
    public enum WatchType {Byte,Word};

    public class Watch
    {
        public string Name { get; set; }
        public ushort Address { get; set; }
        public LockTo LockTo { get; set; }
        public WatchType Type { get; set; }
        public ShowAs ShowAs { get; set; }
    }
}
