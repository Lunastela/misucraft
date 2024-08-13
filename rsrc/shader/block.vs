#version 460 core

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
    fUv = blockPosition.xz;
    
    int chunkIndex = gl_DrawID / 6;
    vec3 chunkPosition = vec3(
        uChunkPos[chunkIndex * 3], 
        uChunkPos[(chunkIndex * 3) + 1], 
        uChunkPos[(chunkIndex * 3) + 2]
    );

    // Thanks Vercidium I was doing this all out manually
    int faceIndex = gl_DrawID % 6;
    if (faceIndex == 0) // Top & Bottom
        blockPosition.y++;
    else {
        fUv.y = 1. - fUv.y;
        if (faceIndex == 1 || faceIndex == 2) { // Left & Right
            blockPosition.xy = blockPosition.yx;
            if (faceIndex == 2)
                blockPosition.x++;
            else fUv.y = 1. - fUv.y;
            fUv.xy = vec2(fUv.y, 1. - fUv.x);
        }
        else if (faceIndex == 3 || faceIndex == 4) { // North & South
            blockPosition.zxy = blockPosition.yxz;
            if (faceIndex == 4)
                blockPosition.z++;
            else fUv.x = 1. - fUv.x;
        }
    }
    gl_Position = uProjection * uView * uModel * vec4(blockPosition + (chunkPosition * CHUNK_SIZE), 1.0);   
}