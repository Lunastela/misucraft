#version 460 core
in vec2 fUv;

uniform sampler2D uTexture0;
out vec4 FragColor;

vec4 hash42(vec2 p)
{
	vec4 p4 = fract(vec4(p.xyxy) * vec4(.1031, .1030, .0973, .1099));
    p4 += dot(p4, p4.wzxy+33.33);
    return fract((p4.xxyz+p4.yzzw)*p4.zywx);
}

vec4 noRepeat(vec2 uv)
{
    vec2 en = floor(uv);
    vec2 lo = fract(uv);
    
    vec2 ddx = dFdx(uv);
    vec2 ddy = dFdy(uv);
    
    vec4 o = vec4(0);
    float W = 0.;
    for(float x = -1.;x<2.;x++){
        for(float y = -1.;y<2.;y++){
            vec2 disp = vec2(x,y);
            vec2 p = en+disp;
            
            vec4 noise = hash42(p);
            //noise*=.5+.5*sin(iTime);
            
            float a = noise.x * 2.*acos(-1.);
            
            
            float c = cos(a),s = sin(a);
            mat2 r = mat2(c,s,-s,c);
            
            vec2 local = (lo-disp-.5)*r + noise.yz+.5;
            
            vec2 dist = lo-disp -.5;
            dist*=dist;
            dist*=dist;
            
            float w = (noise.w*10.+1.) / (dot(dist,dist)+.001);
            
            vec2 dx = ddx*r;
            vec2 dy = ddy*r;

            vec4 sam = textureGrad(uTexture0, local, dx, dy);
            //sam = noise;
            o += w*sam;
            W+=w;
        }
    }
    return o/W;
}

void main() {
    FragColor = texture2D(uTexture0, fUv);
    // FragColor = noRepeat(fUv);
}