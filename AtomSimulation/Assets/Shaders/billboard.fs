#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

out vec4 finalColor;

void main() {
    vec2 d = fragTexCoord - vec2(0.5);
    if (dot(d, d) > 0.25) discard;
    finalColor = fragColor;
}
