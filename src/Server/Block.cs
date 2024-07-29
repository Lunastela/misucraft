namespace Misucraft.Server {
    public enum BlockType {
        Air = 0,
        Dirt = 1,
        NumType
    }

    [Flags]
    public enum CornerState : byte {
        Full,
    }

    [Flags]
    public enum BlockFaces : uint {
        None = 0,
        Top = 1 << 0,
        Bottom = 1 << 1,
        North = 1 << 2,
        South = 1 << 3,
        East = 1 << 4,
        West = 1 << 5,
    }

    public class Block {
        public BlockType Type;
        public CornerState blockCorners = CornerState.Full;
        public Block(BlockType _type) {
            this.Type = _type;
        }
    }
}