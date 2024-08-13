namespace Misucraft.Server {
    public enum BlockType {
        Air = 0,
        Dirt = 1,
        NumType
    }

    // TODO: BlockInformation JSON format or otherwise that gets imported before to reduce overhead
    public struct BlockInformation {
        public float breakSpeed = 1.0f;
        public bool isTransparent = false;
        public BlockInformation() {}
    }

    public enum BlockFace {
        Top = 0,
        Left = 1,
        Right = 2,
        North = 3,
        South = 4,
        Bottom = 5
    }

    public class Block {
        public BlockType Type;
        public Block(BlockType _type) {
            this.Type = _type;
        }
    }
}