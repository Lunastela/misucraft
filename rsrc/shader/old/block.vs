#version 430 core

layout (location = 0) in uint faceCount;

const int CHUNK_SIZE = 32;
const float BLOCK_SIZE = 1.;

layout(std430, binding = 1) buffer ChunkPosBuffer {
    int[] uChunkPos;
};

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 vBlockPosition;
out uint vFaceCount;

void main() {
    // Calculate block coordinates based on vertex ID
    int index = gl_VertexID;
    float z = mod(floor(index / (CHUNK_SIZE * CHUNK_SIZE)), CHUNK_SIZE);
    float y = mod(floor(index / CHUNK_SIZE), CHUNK_SIZE);
    float x = mod(index, CHUNK_SIZE);

    int chunkIndex = int(index / pow(CHUNK_SIZE, 3));
    vBlockPosition = (vec3(x, y, z) 
    + (vec3(
        uChunkPos[chunkIndex * 3], 
        uChunkPos[(chunkIndex * 3) + 1], 
        uChunkPos[(chunkIndex * 3) + 2]
    ) * CHUNK_SIZE)) * BLOCK_SIZE;

    vFaceCount = faceCount;
}