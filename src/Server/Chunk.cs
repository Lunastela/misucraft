using Misucraft.Util;
using Silk.NET.Maths;

namespace Misucraft.Server {
    public class Chunk {
        public static readonly int CHUNK_SIZE = 16;
        public Block[,,] Blocks;
        public Chunk(int x, int y, int z) {
            Blocks = new Block[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
            for (int i = 0; i < CHUNK_SIZE; i++) {
                for (int j = 0; j < CHUNK_SIZE; j++) {
                    for (int k = 0; k < CHUNK_SIZE; k++) {
                        // double heightDisplacement = (Math.Sin((i * Math.PI) / CHUNK_SIZE) * 4);
                        // if ((8 + heightDisplacement) >= j)
                        if ((i <= 0 || i >= CHUNK_SIZE - 1)
                        || (k <= 0 || k >= CHUNK_SIZE - 1))
                            Blocks[i, j, k] = new Block(BlockType.Dirt);
                        // Console.WriteLine(heightDisplacement);
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

        public uint[] FaceMesh = new uint[CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE];
        public void BuildMesh() {
            for (int x = 0; x < CHUNK_SIZE; x++) {
                for (int y = 0; y < CHUNK_SIZE; y++) {
                    for (int z = 0; z < CHUNK_SIZE; z++) {
                        if (Blocks[x, y, z] == null || Blocks[x, y, z].Type == BlockType.Air) {
                            FaceMesh[CoordToIndex(x, y, z)] = (uint)BlockFaces.None;
                            continue;
                        }

                        uint bitmask = 0;
                        if (z == CHUNK_SIZE - 1 || Blocks[x, y, z + 1] == null || Blocks[x, y, z + 1].Type == BlockType.Air) {
                            bitmask |= (uint) BlockFaces.South;
                        }

                        if (z == 0 || Blocks[x, y, z - 1] == null || Blocks[x, y, z - 1].Type == BlockType.Air) {
                            bitmask |= (uint)BlockFaces.North;
                        }

                        if (x == 0 || Blocks[x - 1, y, z] == null || Blocks[x - 1, y, z].Type == BlockType.Air) {
                            bitmask |= (uint)BlockFaces.West;
                        }

                        if (x == CHUNK_SIZE - 1 || Blocks[x + 1, y, z] == null || Blocks[x + 1, y, z].Type == BlockType.Air) {
                            bitmask |= (uint)BlockFaces.East;
                        }

                        if (y == CHUNK_SIZE - 1 || Blocks[x, y + 1, z] == null || Blocks[x, y + 1, z].Type == BlockType.Air) {
                            bitmask |= (uint)BlockFaces.Top;
                        }

                        if (y == 0 || Blocks[x, y - 1, z] == null || Blocks[x, y - 1, z].Type == BlockType.Air) {
                            bitmask |= (uint)BlockFaces.Bottom;
                        }

                        FaceMesh[CoordToIndex(x, y, z)] = bitmask;
                    }
                }
            }
        }

    }
}