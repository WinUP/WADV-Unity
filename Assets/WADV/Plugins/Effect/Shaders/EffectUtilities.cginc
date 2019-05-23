half to_grayscale(fixed4 color) {
    return saturate(dot(color.rgb, fixed3(0.299, 0.587, 0.114)));
}

half to_grayscale(float3 color) {
    return saturate(dot(color, fixed3(0.299, 0.587, 0.114)));
}