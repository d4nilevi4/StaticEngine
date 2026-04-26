#version 330

layout(location = 0) in vec3 vertexPosition;

uniform mat4 mvp;
uniform sampler2D uBodies;
uniform int   uBodyCount;
uniform float uBaseY;
uniform float uSoftSq;

out vec4 vColor;

void main()
{
    float well = 0.0;
    for (int i = 0; i < uBodyCount; ++i)
    {
        vec4 b = texelFetch(uBodies, ivec2(i, 0), 0);
        float dx = b.x - vertexPosition.x;
        float dz = b.y - vertexPosition.z;
        well += b.z / (dx * dx + dz * dz + uSoftSq);
    }

    vec3 p = vec3(vertexPosition.x, uBaseY - well, vertexPosition.z);
    gl_Position = mvp * vec4(p, 1.0);
    vColor = vec4(1.0, 1.0, 1.0, 0.235);
}
