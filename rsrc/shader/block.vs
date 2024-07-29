#version 330 core

layout (location = 0) in uint faceCount;

const float CHUNK_SIZE = 16.;
const float BLOCK_SIZE = 1.0;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 vBlockPosition;
out uint vFaceCount;

void main() {
    // Calculate block coordinates based on vertex ID
    int index = gl_VertexID;
    float z = floor(index / (CHUNK_SIZE * CHUNK_SIZE));
    float y = floor(mod((index / CHUNK_SIZE), CHUNK_SIZE));
    float x = floor(mod(index, CHUNK_SIZE));

    vBlockPosition = vec3(x, y, z) * BLOCK_SIZE;
    vFaceCount = faceCount;
}
