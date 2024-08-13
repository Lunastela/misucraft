#version 430 core

layout(triangles) in;
layout(triangle_strip, max_vertices = 64) out;

in vec3 vBlockPosition[];
in uint vFaceCount[];

out vec2 fUv;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

vec4 calculatePosition(int index, vec3 displacementPos) {
    return uProjection * uView * uModel * vec4(vBlockPosition[index] + displacementPos, 1.0);
}

const float BLOCK_SIZE = 1.;

void main() {
    vec3 blockVertices[24] = vec3[24](
        // Top Face
        vec3(0.0, BLOCK_SIZE, 0.0), vec3(0.0, BLOCK_SIZE, BLOCK_SIZE),
        vec3(BLOCK_SIZE, BLOCK_SIZE, 0.0), vec3(BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE),
        // Bottom Face
        vec3(0.0, 0.0, 0.0), vec3(0.0, 0.0, BLOCK_SIZE),
        vec3(BLOCK_SIZE, 0.0, 0.0), vec3(BLOCK_SIZE, 0.0, BLOCK_SIZE),
        // North Face
        vec3(0.0, 0.0, 0.0), vec3(0.0, BLOCK_SIZE, 0.0),
        vec3(BLOCK_SIZE, 0.0, 0.0), vec3(BLOCK_SIZE, BLOCK_SIZE, 0.0),
        // South Face
        vec3(0.0, 0.0, BLOCK_SIZE), vec3(0.0, BLOCK_SIZE, BLOCK_SIZE),
        vec3(BLOCK_SIZE, 0.0, BLOCK_SIZE), vec3(BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE),
        // East Face
        vec3(BLOCK_SIZE, 0.0, 0.0), vec3(BLOCK_SIZE, BLOCK_SIZE, 0.0),
        vec3(BLOCK_SIZE, 0.0, BLOCK_SIZE), vec3(BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE),
        // West Face
        vec3(0.0, 0.0, 0.0), vec3(0.0, BLOCK_SIZE, 0.0),
        vec3(0.0, 0.0, BLOCK_SIZE), vec3(0.0, BLOCK_SIZE, BLOCK_SIZE)
    );

    vec2 uvCoords[24] = vec2[24](
        // Top Face
        vec2(1.0, 0.0), vec2(1.0, 1.0), vec2(0.0, 0.0), vec2(0.0, 1.0),
        // Bottom Face
        vec2(0.0, 1.0), vec2(0.0, 0.0), vec2(1.0, 1.0), vec2(1.0, 0.0),
        // North Face
        vec2(1.0, 1.0), vec2(1.0, 0.0), vec2(0.0, 1.0), vec2(0.0, 0.0),
        // South Face
        vec2(0.0, 1.0), vec2(0.0, 0.0), vec2(1.0, 1.0), vec2(1.0, 0.0),
        // East Face
        vec2(1.0, 1.0), vec2(1.0, 0.0), vec2(0.0, 1.0), vec2(0.0, 0.0),
        // West Face
        vec2(0.0, 1.0), vec2(0.0, 0.0), vec2(1.0, 1.0), vec2(1.0, 0.0)
    );

    for (int i = 0; i < gl_in.length(); i++) {
        uint bitmask = vFaceCount[i];
        if (bitmask > 0u) {
            for (int face = 0; face < 6; face++) {
                if ((bitmask & (1u << face)) != 0u) {
                    int baseIndex = face * 4;
                    for (int vert = 0; vert < 4; vert++) {
                        fUv = uvCoords[baseIndex + vert];
                        gl_Position = calculatePosition(i, blockVertices[baseIndex + vert]);
                        EmitVertex();
                    }
                    EndPrimitive();
                }
                // 
            }
        }
    }
}