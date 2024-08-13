#version 430 core

layout (location = 0) in uint meshData;

const int CHUNK_SIZE = 64;
const float BLOCK_SIZE = 1.;

layout(std430, binding = 1) buffer ChunkPosBuffer {
    int[] uChunkPos;
};

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec2 fUv;

void main() {
    float x = (meshData & 0x3F);
    float y = ((meshData >> 6) & 0x3F);
    float z = ((meshData >> 12) & 0x3F);

    float mesh_length = (((meshData >> 18) & 0x3F) + 1) * BLOCK_SIZE;
    float mesh_width = (((meshData >> 24) & 0x3F) + 1) * BLOCK_SIZE;

    vec3 instancePosition[4] = vec3[4](
        vec3(x, y, z),
        vec3(x + mesh_length, y, z),
        vec3(x, y, z + mesh_width),
        vec3(x + mesh_length, y, z + mesh_width)
    );
    vec3 blockPosition = instancePosition[gl_VertexID % 4].xyz;
    fUv = instancePosition[gl_VertexID % 4].xz;

    gl_Position = uProjection * uView * uModel * vec4(blockPosition + vec3(uChunkPos[0], uChunkPos[1], uChunkPos[2]), 1.0);   
}