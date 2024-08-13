using System.Numerics;
using Misucraft.Util;
using Silk.NET.Maths;

namespace Misucraft.Server {
    public class Chunk {
        public static readonly int CHUNK_SIZE = 64;
        public static Dictionary<Vector3D<int>, Chunk> chunkMap 
            = new Dictionary<Vector3D<int>, Chunk>();

        public Block[,,] Blocks;
        public Vector3D<int> Position;
        public Chunk(int x, int y, int z) {
            // Create Position and add to Chunk Map
            Position = new Vector3D<int>(x, y, z);
            chunkMap.Add(Position, this);

            Blocks = new Block[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
            for (int i = 0; i < CHUNK_SIZE; i++) {
                for (int j = 0; j < CHUNK_SIZE; j++) {
                    for (int k = 0; k < CHUNK_SIZE; k++) {
                        Blocks[i, j, k] = new Block(BlockType.Dirt);
                    }
                }
            }
            BuildMesh();
        }

        public static int CoordToIndex(int x, int y, int z) {
            return x + (y * CHUNK_SIZE) + (z * CHUNK_SIZE * CHUNK_SIZE);
        }

        public static Vector3D<int> IndexToCoord(int index) {
            int z = index / (CHUNK_SIZE * CHUNK_SIZE);
            int y = index / CHUNK_SIZE % CHUNK_SIZE;
            int x = index % CHUNK_SIZE;
            return new Vector3D<int>(x, y, z);
        }

        public static int CoordToChunk(int coord) {
            if (coord < 0)
                return (coord / CHUNK_SIZE) - 1;
            return coord / CHUNK_SIZE;
        }

        public BlockType blockRelativeToChunk(int x, int y, int z) {
            if (x < 0 || y < 0 || z < 0 || x >= CHUNK_SIZE || y >= CHUNK_SIZE || z >= CHUNK_SIZE) {
                Vector3D<int> trueChunkPosition = Position 
                    + new Vector3D<int>(
                        CoordToChunk(x / CHUNK_SIZE), 
                        CoordToChunk(y / CHUNK_SIZE), 
                        CoordToChunk(z / CHUNK_SIZE)
                    );
                if (chunkMap[trueChunkPosition] != null)
                    return chunkMap[trueChunkPosition]
                        .Blocks[x % CHUNK_SIZE, y % CHUNK_SIZE, z % CHUNK_SIZE].Type;
            } else 
                return Blocks[x, y, z].Type;
            return BlockType.Air;
        }

        public static BlockType blockAtAbsoluteCoord(int x, int y, int z) {
            // Implicitly rounds downwards? I hope???
            Vector3D<int> chunkPosition = new Vector3D<int>(CoordToChunk(x / CHUNK_SIZE), CoordToChunk(y / CHUNK_SIZE), CoordToChunk(z / CHUNK_SIZE));
            Chunk focusedChunk = chunkMap[chunkPosition];
            if (focusedChunk != null)
                return focusedChunk.Blocks[x % CHUNK_SIZE, y % CHUNK_SIZE, z % CHUNK_SIZE].Type;
            return BlockType.Air;
        }

        public static bool isTransparent(BlockType blockType) {
            // TODO: Change to BlockInformation eventually
            return blockType == BlockType.Air;
        }

        private uint PackMeshData(int x, int y, int z, int length, int width) {
            uint meshData = (uint)(x & 0x3F) | ((uint)(y & 0x3F) << 6) | 
                ((uint)(z & 0x3F) << 12) | ((uint)((length - 1) & 0x3F) << 18) |
                ((uint)((width - 1) & 0x3F) << 24);
            return meshData;
        }

        public List<uint>[] FaceMesh = new List<uint>[6];
        public void BuildMesh() {
            // Goal: Output a uint value that allocates 6 bits for X, Y, Z positions of the Mesh and Length and Width of the greedy mesh.
            for (int i = 0; i < 6; i++) {
                if (FaceMesh[i] == null)
                    FaceMesh[i] = new List<uint>();
                for (int z = 0; z < 16; z++) {
                    for (int y = 0; y < 16; y++) {
                        for (int x = 0; x < 16; x++) {
                            FaceMesh[i].Add(PackMeshData(x, y, z, 1, 1));
                        }
                    }
                }
            }

            
        }
    }
}